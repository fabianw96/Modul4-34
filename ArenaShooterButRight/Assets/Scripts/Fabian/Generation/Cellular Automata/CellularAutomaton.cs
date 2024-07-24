using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

namespace Fabian.Generation.Cellular_Automata
{
    public class CellularAutomaton : MonoBehaviour
    {
        private struct Cell
        {
            public GameObject CellGameObject;
            public Transform CellTransform;
            public bool IsAlive;
            public int States;
            public int Neighbors;
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
        [SerializeField] private bool useCoroutine = false;
        [SerializeField] private CellularAutomatonType cellularType;

        private MeshSpawner _meshSpawner;
        private List<Cell> _cellList = new List<Cell>();
        private bool _isRunning = false;

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

            foreach (GameObject obj in gosList)
            {
                Cell tempCell = new Cell
                {
                    CellGameObject = obj,
                    CellTransform = obj.transform,
                    IsAlive = true,
                    States = numberOfStates,
                    Neighbors = 0
                };

                _cellList.Add(tempCell);
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
                if (_cellList[i].CellTransform is null)
                {
                    continue;
                }

                float xInput = (_cellList[i].CellTransform.position.x + offsetX) * noiseScale;
                float yInput = (_cellList[i].CellTransform.position.y + offsetY) * noiseScale;
                float zInput = (_cellList[i].CellTransform.position.z + offsetZ) * noiseScale;

                float noiseValueXZ = Mathf.PerlinNoise(xInput, zInput);
                float noiseValueYZ = Mathf.PerlinNoise(yInput, zInput);

                float combinedNoiseValue = (noiseValueXZ + noiseValueYZ) / 2;

                Cell cell = _cellList[i];
                cell.IsAlive = combinedNoiseValue > aliveThreshold;
                _cellList[i] = cell;
            }

            if (useCoroutine)
            {
                StartCoroutine(ApplyNoiseCoroutine());
            }
            else
            {
                ApplyNoise();
            }
        }

        private void ChooseCellularAutomata()
        {
            _isRunning = true;
            for (int i = 0; i < _cellList.Count; i++)
            {
                Cell cell = _cellList[i];
                cell.Neighbors = 0;

                //TODO: Move calculation to Compute shader so that 8x8x8 chunks can be compiled at once!
                switch (cellularType)
                {
                    case CellularAutomatonType.Moore:
                        //increase each cells neighbor count by one for each neighboring alive cell in a 3x3x3 cube around the cell
                        _cellList[i] = CalculateMooreNeighbors(cell);
                        break;
                    case CellularAutomatonType.Neumann:
                        //increase each cells neighbor count by one for each neighboring alive cell in -x, +x, -y, +y, -z, +z direction
                        _cellList[i] = CalculateNeumannNeighbors(cell);
                        break;
                }
            }

            if (useCoroutine)
            {
                StartCoroutine(ApplyCaCoroutine());
            }
            else
            {
                ApplyCellularAutomata();
            }
        }

        private Cell CalculateNeumannNeighbors(Cell cell)
        {
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
                Vector3 neighborPosition = cell.CellTransform.position + direction;

                if (IsWithinBounds(neighborPosition))
                {
                    Cell neighborCell = _cellList.Find(c => c.CellTransform.position == neighborPosition);

                    if (neighborCell.IsAlive)
                    {
                        cell.Neighbors++;
                    }
                }
            }
            return cell;
        }

        private Cell CalculateMooreNeighbors(Cell cell)
        {
            for (int x = -1; x <= 1; ++x)
            for (int y = -1; y <= 1; ++y)
            for (int z = -1; z <= 1; ++z)
                if (x != 0 || y != 0 || z != 0)
                {
                    Vector3 neighborPosition = cell.CellTransform.position + new Vector3(x, y, z);
                    if (IsWithinBounds(neighborPosition))
                    {
                        Cell neighborCell = _cellList.Find(c => c.CellTransform.position == neighborPosition);

                        if (neighborCell.IsAlive)
                        {
                            cell.Neighbors++;
                        }
                    }
                }
            return cell;
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
                //TODO: check if the cell still has a "life" left. eg: _cellList[i].States < numberOfStates. if not, set IsAlive to false and disable.
                Cell cell = _cellList[i];

                if (_cellList[i].Neighbors < minNeighborCount)
                {
                    cell.States--;
                    _cellList[i] = cell;
                }

                if (_cellList[i].States == 0)
                {
                    cell.IsAlive = false;
                    _cellList[i] = cell;

                    _cellList[i].CellGameObject.SetActive(false);
                }

                if (_cellList[i].Neighbors >= rebirthNeighborCount && !_cellList[i].CellGameObject.activeSelf)
                {
                    cell.IsAlive = true;
                    cell.States = numberOfStates;
                    _cellList[i] = cell;

                    _cellList[i].CellGameObject.SetActive(true);
                }

                //TODO: set cell active again if the gameobject is inactive but has more minNeighborCount neighbors, set IsAlive to true again.
            }
            _isRunning = false;
        }

        private void ApplyNoise()
        {
            foreach (Cell cell in _cellList)
            {
                if (!cell.IsAlive)
                {
                    cell.CellGameObject.SetActive(false);
                }
            }
        }

        private IEnumerator ApplyNoiseCoroutine()
        {
            // ApplyNoise();
            foreach (Cell cell in _cellList)
            {
                if (!cell.IsAlive)
                {
                    cell.CellGameObject.SetActive(false);
                }
                yield return new WaitForSeconds(0.0001f);
            }
        }

        private IEnumerator ApplyCaCoroutine()
        {
            // ApplyCellularAutomata();
            for (int i = 0; i < _cellList.Count; i++)
            {
                if (_cellList[i].Neighbors < minNeighborCount)
                {
                    Cell cell = _cellList[i];
                    cell.IsAlive = false;
                    _cellList[i] = cell;

                    _cellList[i].CellGameObject.SetActive(false);
                }
                yield return new WaitForSeconds(0.0001f);
            }
            _isRunning = false;
        }
    }
}
