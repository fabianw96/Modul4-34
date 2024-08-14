using System.Collections.Generic;
using System.Diagnostics;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = System.Random;

namespace Fabian.Generation.Cellular_Automata
{
    public class CellularAutomaton : MonoBehaviour
    {
        public struct Cell
        {
            public float3 CellPosition;
            public int3 GridPosition;
            public int States;
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
        [SerializeField] private CellularAutomatonType cellularType;
        [SerializeField] private bool useComputeShader;
        [SerializeField] private bool useJobSystem;
        [SerializeField] private ComputeShader computeShader;

        private MeshSpawner _meshSpawner;
        private List<Cell> _cellList = new List<Cell>();
        private List<GameObject> _cellObjects = new List<GameObject>();
        private bool _isRunning;
        private ComputeBuffer _cellsBuffer;
        private ComputeBuffer _cellsBufferCopy;
        private ComputeBuffer _colorBuffer;
        
        //Compute shader PropertyToID instead of string base eval
        private static readonly int ColorBuffer = Shader.PropertyToID("color_buffer");
        private static readonly int CellsCopyBuffer = Shader.PropertyToID("cells_copy_buffer");
        private static readonly int Cells = Shader.PropertyToID("cells");
        private static readonly int Size = Shader.PropertyToID("size");

        //Compute Shader Property to ID

        private void Awake()
        {
            _meshSpawner = GetComponent<MeshSpawner>();
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

            foreach (GameObject gameObj in gosList)
            {
                Cell tempCell = new Cell
                {
                    CellPosition = gameObj.transform.position,
                    States = numberOfStates,
                };
                _cellList.Add(tempCell);
                _cellObjects.Add(gameObj.gameObject);
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
                if (!_cellObjects[i].gameObject)
                {
                    continue;
                }

                Vector3 position = _cellList[i].CellPosition;
                
                int3 gridPos = new int3(
                    Mathf.FloorToInt(position.x),
                    Mathf.FloorToInt(position.y),
                    Mathf.FloorToInt(position.z)
                );

                float xInput = (position.x + offsetX) * noiseScale;
                float yInput = (position.y + offsetY) * noiseScale;
                float zInput = (position.z + offsetZ) * noiseScale;

                float noiseValueXZ = Mathf.PerlinNoise(xInput, zInput);
                float noiseValueYZ = Mathf.PerlinNoise(yInput, zInput);

                float combinedNoiseValue = (noiseValueXZ + noiseValueYZ) / 2;

                Cell cell = _cellList[i];
                cell.GridPosition = gridPos;
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

            if (!useComputeShader && !useJobSystem)
            {
                int neighbors;
                
                switch (cellularType)
                {
                    case CellularAutomatonType.Moore:
                        //TODO: Case currently does "too much". Fix bug if possible before portfolio
                        for (int i = 0; i < _cellList.Count; i++)
                        {
                            neighbors = CalculateMooreNeighbors(_cellList[i]);
                            Cell cell = _cellList[i];
                            ApplyLogic(ref cell, neighbors);
                            _cellList[i] = cell;
                        }
                        break;
                    case CellularAutomatonType.Neumann:
                        for (int i = 0; i < _cellList.Count; i++)
                        {
                            neighbors = CalculateNeumannNeighbors(_cellList[i]);
                            Cell cell = _cellList[i];
                            ApplyLogic(ref cell, neighbors);
                            _cellList[i] = cell;
                        }
                        break;
                }
                ApplyCellularAutomata();
            }
            else if (useComputeShader)
            {
                DispatchComputeShader();
                ApplyCellularAutomata();
            }
            else if (useJobSystem)
            {
                NativeArray<Cell> cells = new NativeArray<Cell>(_cellList.ToArray(), Allocator.TempJob);
                NativeArray<int> neighborCount = new NativeArray<int>(_cellList.Count, Allocator.TempJob);

                NativeHashMap<float3, Cell> cellMap = new NativeHashMap<float3, Cell>(_cellList.Count, Allocator.TempJob);
                for (int i = 0; i < cells.Length; i++)
                {
                    cellMap.TryAdd(cells[i].CellPosition, cells[i]);
                }

                JobHandle jobHandle = default;

                switch (cellularType)
                {
                    case CellularAutomatonType.Moore:
                        MooreNeighborsJob mooreJob = new MooreNeighborsJob()
                        {
                            Cells = cells,
                            NeighborCount = neighborCount,
                            CellMap = cellMap,
                            Size = _meshSpawner.size
                        };
                        jobHandle = mooreJob.Schedule(_cellList.Count, 64);
                        break;
                    case CellularAutomatonType.Neumann:
                        NeumannNeighborsJob neumannJob = new NeumannNeighborsJob
                        {
                            Cells = cells,
                            NeighborCounts = neighborCount,
                            CellMap = cellMap,
                            Size = _meshSpawner.size
                        };
                        jobHandle = neumannJob.Schedule(_cellList.Count, 64);
                        break;
                }
                
                jobHandle.Complete();

                for (int i = 0; i < _cellList.Count; i++)
                {
                    Cell cell = _cellList[i];
                    int neighbors = neighborCount[i];
                    ApplyLogic(ref cell, neighbors);
                    _cellList[i] = cell;
                }

                cells.Dispose();
                neighborCount.Dispose();
                cellMap.Dispose();
                
                ApplyCellularAutomata();
            }

            stopwatch.Stop();
            Debug.Log(stopwatch.Elapsed);
        }

        private void ApplyLogic(ref Cell cell, int neighbors)
        {
            if (cell.States > 0)
            {
                if (neighbors < minNeighborCount)
                {
                    cell.States--;
                }
            }
            else
            {
                if (neighbors >= rebirthNeighborCount)
                {
                    cell.States = numberOfStates;
                }
            }
        }

        private void InitializeBuffers()
        {
            int totalCells = _cellList.Count;
            int cellSize = sizeof(float) * 3 + sizeof(int) * 4;
            
            _cellsBuffer = new ComputeBuffer(totalCells, cellSize);
            _cellsBufferCopy = new ComputeBuffer(totalCells, cellSize);
            _colorBuffer = new ComputeBuffer(totalCells, sizeof(float) * 4);

            _cellsBuffer.SetData(_cellList.ToArray());
            _cellsBufferCopy.SetData(_cellList.ToArray());
        }

        private void DispatchComputeShader()
        {
            int kernelApplyCa = computeShader.FindKernel("ApplyCellularAutomata");

            if (kernelApplyCa < 0)
            {
                Debug.LogError("Invalid kernel index");
                return;
            }

            InitializeBuffers();
            
            int totalCells = _meshSpawner.size.x * _meshSpawner.size.y * _meshSpawner.size.z;
            
            computeShader.SetInts(Size, _meshSpawner.size.x, _meshSpawner.size.y, _meshSpawner.size.y);
            
            computeShader.SetBuffer(kernelApplyCa, Cells, _cellsBuffer);
            computeShader.SetBuffer(kernelApplyCa, CellsCopyBuffer, _cellsBufferCopy);
            computeShader.SetBuffer(kernelApplyCa, ColorBuffer, _colorBuffer);

            computeShader.GetKernelThreadGroupSizes(kernelApplyCa, out uint threadGroupSizeX, out uint threadGroupSizeY, out uint threadGroupSizeZ);

            int threadGroupsX = (int)(_meshSpawner.size.x / threadGroupSizeX);
            int threadGroupsY = (int)(_meshSpawner.size.y / threadGroupSizeY);
            int threadGroupsZ = (int)(_meshSpawner.size.z /threadGroupSizeZ);
            
            computeShader.Dispatch(kernelApplyCa, threadGroupsX, threadGroupsY, threadGroupsZ);
            
            Cell[] processedCells = new Cell[totalCells];
            _cellsBuffer.GetData(processedCells);
            
            for (int i = 0; i < totalCells; i++)
            {
                _cellList[i] = processedCells[i];
            }

            float4[] colorData = new float4[totalCells];
            _colorBuffer.GetData(colorData);

            for (int i = 0; i < _cellObjects.Count; i++)
            {
                Renderer objectRenderer = _cellObjects[i].GetComponent<Renderer>();
                if (objectRenderer != null)
                {
                    objectRenderer.material.color = new Color(colorData[i].x, colorData[i].y,colorData[i].z, colorData[i].w);
                }
            }
            
            _cellsBuffer.Release();
            _cellsBufferCopy.Release();
            _colorBuffer.Release();
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
                Vector3 neighborPosition = (Vector3)cell.CellPosition + direction;

                if (!IsWithinBounds(neighborPosition)) continue;
                
                Cell neighborCell = _cellList.Find(c => (Vector3)c.CellPosition == neighborPosition);

                if (neighborCell.States > 0)
                {
                    neighbors++;
                }
            }
            return neighbors;
        }

        private int CalculateMooreNeighbors(Cell cell)
        {
            int neighbors = 0;

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    for (int z = -1; z <= 1; z++)
                    {
                        if (x == 0 && y == 0 && z == 0) continue;
                        
                        Vector3 neighborPosition = (Vector3)cell.CellPosition + new Vector3(x, y, z);
                        if (!IsWithinBounds(neighborPosition)) continue;
                        
                        Cell neighborCell = _cellList.Find(c => (Vector3)c.CellPosition == neighborPosition);

                        if (neighborCell.States > 0)
                        {
                            neighbors++;
                        }
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
                _cellObjects[i].SetActive(_cellList[i].States > 0);
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
