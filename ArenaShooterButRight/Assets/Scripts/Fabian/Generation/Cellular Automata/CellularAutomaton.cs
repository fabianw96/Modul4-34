using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;
using Random = System.Random;

namespace Fabian.Generation.Cellular_Automata
{
    public class CellularAutomaton : MonoBehaviour
    {
        private struct Cell
        {
            public Vector3 CellPosition;
            // public int IsAlive;
            public int States;
            // public int Neighbors;
        }

        private enum CellularAutomatonType
        {
            Moore,
            Neumann
        }

        [SerializeField] private float noiseScale = 0.1f;
        [SerializeField] private float aliveThreshold = 0.5f;
        [SerializeField] private int seed;
        [SerializeField] private int minNeighborCount = 4;
        [SerializeField] private int rebirthNeighborCount = 4;
        [SerializeField] private int numberOfStates = 5;
        [SerializeField] private bool useCoroutine;
        [SerializeField] private CellularAutomatonType cellularType;
        [SerializeField] private bool useComputeShader;
        [SerializeField] private ComputeShader computeShader;

        private MeshSpawner _meshSpawner;
        private List<Cell> _cellList = new List<Cell>();
        private List<GameObject> _cellObjects = new List<GameObject>();
        private bool _isRunning;
        private ComputeBuffer _cellsBuffer;
        private ComputeBuffer _cellsBufferCopy;
        private ComputeBuffer _debugBuffer;

        private const int CHUNKSIZE = 512;

        private void Awake()
        {
            _meshSpawner = GetComponent<MeshSpawner>();
            Debug.Log(CHUNKSIZE);
        }

        private void OnGUI()
        {
            if (GUI.Button(new Rect(10, 70, 200, 50), "Generate"))
            {
                _meshSpawner.EnableMesh();
            }

            if (GUI.Button(new Rect(10, 150, 200, 50), "Iterate"))
            {
                if (!_isRunning)
                {
                    ChooseCellularAutomata();
                }
            }
        }

        public void GetListFromSpawner(List<GameObject> gosList)
        {
            _cellList.Clear();
            _cellObjects.Clear();

            for (int i = 0; i < gosList.Count; i++)
            {
                Cell tempCell = new Cell
                {
                    CellPosition = gosList[i].transform.position,
                    // IsAlive = 1,
                    States = numberOfStates,
                    // Neighbors = 0
                };
                _cellList.Add(tempCell);
                _cellObjects.Add(gosList[i].gameObject);
            }
            
            CalculateNoise();
        }

        private void CalculateNoise()
        {
            Random rndm = new Random(seed);

            float offsetX = rndm.Next(-100000, 100000);
            float offsetY = rndm.Next(-100000, 100000);
            float offsetZ = rndm.Next(-100000, 100000);

            for (int i = _cellList.Count - 1; i >= 0; i--)
            {
                if (!_cellObjects[i].gameObject)  //TODO: Find way to check if list at index is null
                {
                    continue;
                }

                float xInput = (_cellList[i].CellPosition.x + offsetX) * noiseScale;
                float yInput = (_cellList[i].CellPosition.y + offsetY) * noiseScale;
                float zInput = (_cellList[i].CellPosition.z + offsetZ) * noiseScale;

                float noiseValueXZ = Mathf.PerlinNoise(xInput, zInput);
                float noiseValueYZ = Mathf.PerlinNoise(yInput, zInput);

                float combinedNoiseValue = (noiseValueXZ + noiseValueYZ) / 2;

                Cell cell = _cellList[i];
                cell.States = combinedNoiseValue > aliveThreshold ? numberOfStates : 0;
                _cellList[i] = cell;
            }
            
            ApplyNoise();

        }

        private void ChooseCellularAutomata()
        {
            Stopwatch stopwatch = new Stopwatch();
            _isRunning = true;
            stopwatch.Restart();


            if (!useComputeShader)
            {
                for (int i = 0; i < _cellList.Count; i++)
                {
                    int neighbors = 0;
                    //TODO: Move calculation to Compute shader so that 8x8x8 chunks can be compiled at once!
                    switch (cellularType)
                    {
                        case CellularAutomatonType.Moore:
                            //increase each cells neighbor count by one for each neighboring alive cell in a 3x3x3 cube around the cell
                            neighbors = CalculateMooreNeighbors(_cellList[i]);
                            break;
                        case CellularAutomatonType.Neumann:
                            //increase each cells neighbor count by one for each neighboring alive cell in -x, +x, -y, +y, -z, +z direction
                            neighbors = CalculateNeumannNeighbors(_cellList[i]);
                            break;
                    }

                    Cell cell = _cellList[i];
                    ApplyLogic(ref cell, neighbors);
                    _cellList[i] = cell;
                }
                ApplyCellularAutomata();
            }
            else
            {
                DispatchComputeShader();
                ApplyCellularAutomata();
            }

            
            stopwatch.Stop();
            Debug.Log(stopwatch.Elapsed);
        }

        private void ApplyLogic(ref Cell cell, int neigbors)
        {
            if (cell.States > 0)
            {
                if (neigbors < minNeighborCount)
                {
                    cell.States--;
                }
            }
            else
            {
                if (neigbors >= rebirthNeighborCount)
                {
                    cell.States = numberOfStates;
                }
            }
        }
        
        void InitializeBuffers()
        {
            int totalCells = _cellList.Count;
            _cellsBuffer = new ComputeBuffer(totalCells, System.Runtime.InteropServices.Marshal.SizeOf(typeof(Cell)));
            _cellsBufferCopy = new ComputeBuffer(totalCells, System.Runtime.InteropServices.Marshal.SizeOf(typeof(Cell)));
        }

        private void DispatchComputeShader()
        {
            int kernelPrepare = computeShader.FindKernel("PrepareBuffer");
            int kernelApply = computeShader.FindKernel("ApplyCellularAutomata");
            int kernelApplyBuffer = computeShader.FindKernel("ApplyBuffer");

            if (kernelPrepare < 0 || kernelApply < 0 || kernelApplyBuffer < 0)
            {
                Debug.LogError("Invalid kernel index");
                return;
            }

            InitializeBuffers();

            int totalCells = _cellList.Count;
            int numChunks = Mathf.CeilToInt((float)totalCells / CHUNKSIZE);

            for (int chunk = 0; chunk < numChunks; chunk++)
            {
                int chunkStart = chunk * CHUNKSIZE;
                int chunkSize = Mathf.Min(CHUNKSIZE, totalCells - chunkStart);

                Cell[] chunkCells = new Cell[chunkSize];
                Array.Copy(_cellList.ToArray(), chunkStart, chunkCells, 0, chunkSize);

                _cellsBuffer.SetData(chunkCells);
                _cellsBufferCopy.SetData(chunkCells);

                computeShader.SetBuffer(kernelPrepare, "cells", _cellsBuffer);
                computeShader.SetBuffer(kernelPrepare, "cells_buffer", _cellsBufferCopy);
                computeShader.SetInt("size.x", Mathf.CeilToInt(_meshSpawner.size.x));
                computeShader.SetInt("size.y", Mathf.CeilToInt(_meshSpawner.size.y));
                computeShader.SetInt("size.z", Mathf.CeilToInt(_meshSpawner.size.z));

                computeShader.Dispatch(kernelPrepare, Mathf.CeilToInt(_meshSpawner.size.x / 8.0f), Mathf.CeilToInt(_meshSpawner.size.y / 8.0f), Mathf.CeilToInt(_meshSpawner.size.z / 8.0f));

                computeShader.SetBuffer(kernelApply, "cells", _cellsBuffer);
                computeShader.SetBuffer(kernelApply, "cells_buffer", _cellsBufferCopy);

                computeShader.Dispatch(kernelApply, Mathf.CeilToInt(_meshSpawner.size.x / 8.0f), Mathf.CeilToInt(_meshSpawner.size.y / 8.0f), Mathf.CeilToInt(_meshSpawner.size.z / 8.0f));

                computeShader.SetBuffer(kernelApplyBuffer, "cells", _cellsBuffer);
                computeShader.SetBuffer(kernelApplyBuffer, "cells_buffer", _cellsBufferCopy);

                computeShader.Dispatch(kernelApplyBuffer, Mathf.CeilToInt(_meshSpawner.size.x / 8.0f), Mathf.CeilToInt(_meshSpawner.size.y / 8.0f), Mathf.CeilToInt(_meshSpawner.size.z / 8.0f));

                Cell[] processedCells = new Cell[chunkSize];
                _cellsBuffer.GetData(processedCells);
                
                for (int i = 0; i < processedCells.Length; i++)
                {
                    _cellList[i] = processedCells[i];
                }
            }

            _cellsBuffer.Release();
            _cellsBufferCopy.Release();

            foreach (var cell in _cellList)
            {
                Debug.Log(cell.States);
            }
        }
        

        private int CalculateNeumannNeighbors(Cell cell)
        {
            int neighbors = 0;
            
            Vector3[] directions =
            {
                new Vector3(-1, 0, 0),
                new Vector3(1, 0, 0),
                new Vector3(0, -1, 0),
                new Vector3(0, 1, 0),
                new Vector3(0, 0, -1),
                new Vector3(0, 0, 1),
            };

            foreach (Vector3 direction in directions)
            {
                Vector3 neighborPosition = cell.CellPosition + direction;

                if (IsWithinBounds(neighborPosition))
                {
                    Cell neighborCell = _cellList.Find(c => c.CellPosition == neighborPosition);

                    if (neighborCell.States > 0)
                    {
                        neighbors++;
                    }
                }
            }
            return neighbors;
        }

        private int CalculateMooreNeighbors(Cell cell)
        {
            int neighbors = 0;
            for (int x = -1; x <= 1; ++x)
            for (int y = -1; y <= 1; ++y)
            for (int z = -1; z <= 1; ++z)
                if (x != 0 || y != 0 || z != 0)
                {
                    Vector3 neighborPosition = cell.CellPosition + new Vector3(x, y, z);
                    if (IsWithinBounds(neighborPosition))
                    {
                        Cell neighborCell = _cellList.Find(c => c.CellPosition == neighborPosition);

                        if (neighborCell.States > 0)
                        {
                            neighbors++;
                        }
                    }
                }
            return neighbors;
        }

        private bool IsWithinBounds(Vector3 position)
        {
            return position.x >= 0 && position.x < _meshSpawner.size.x &&
                   position.y >= 0 && position.y < _meshSpawner.size.y &&
                   position.z >= 0 && position.z < _meshSpawner.size.z;
        }

        private void ApplyCellularAutomata()
        {
            for (int i = 0; i < _cellList.Count; i++)
            {
                if (_cellList[i].States <= 0)
                {
                    _cellObjects[i].SetActive(false);
                }
                else
                {
                    _cellObjects[i].SetActive(true);
                }
            }
            _isRunning = false;
        }

        private void ApplyNoise()
        {
            for (int i = 0; i < _cellObjects.Count; i++)
            {
                if (_cellList[i].States <= 0)
                {
                    _cellObjects[i].SetActive(false);
                }
            }
        }
    }
}
