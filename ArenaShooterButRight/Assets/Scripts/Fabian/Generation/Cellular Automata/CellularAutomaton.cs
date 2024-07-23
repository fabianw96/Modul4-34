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
            public int Neighbors;
        }

        [SerializeField] private float noiseScale = 0.1f;
        [SerializeField] private float aliveThreshold = 0.5f;
        [SerializeField] private int seed;
        [SerializeField] private int minNeighborCount = 4;
        [SerializeField] private bool useCoroutine = false;
        [SerializeField] private int iterationAmount = 1;

        private MeshSpawner _meshSpawner;
        private List<Cell> _cellList = new List<Cell>();
        private bool _isRunning = false;

        private void Awake()
        {
            _meshSpawner = GetComponent<MeshSpawner>();
        }

        private void OnGUI()
        {
            if (GUI.Button(new Rect(10,70,200,50), "Generate"))
            {
                _meshSpawner.EnableMesh();
            }
            
            if (GUI.Button(new Rect(10,150,200,50), "Iterate"))
            {
                if (!_isRunning)
                {
                    CalculateCellularAutomata();
                    _isRunning = true;
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

            //use coroutine if bool is true
            if (useCoroutine)
            {
                StartCoroutine(ApplyNoiseCoroutine());
            }
            else
            {
                ApplyNoise();
            }
        }

        private void CalculateCellularAutomata()
        {
            //apply cellular automata to the rest of the cells in _cellList
            //increase each cells neighbor count by one for each neighboring alive cell in -x, +x, -y, +y, -z, +z direction
            
            for (int i = 0; i < _cellList.Count; i++)
            {
                Cell cell = _cellList[i];
                cell.Neighbors = 0;

                Vector3[] directions =
                {
                    new Vector3(-1,0,0),
                    new Vector3(1,0,0),
                    new Vector3(0,-1,0),
                    new Vector3(0,1,0),
                    new Vector3(0,0,-1),
                    new Vector3(0,0,1),
                };

                foreach (Vector3 direction in directions)
                {
                    Vector3 neighborPosition = cell.CellTransform.position + direction;

                    if (IsWithinBounds(neighborPosition))
                    {
                        Cell? neighborCell = _cellList.Find(c => c.CellTransform.position == neighborPosition);
                    
                        if (neighborCell.HasValue && neighborCell.Value.IsAlive)
                        {
                            cell.Neighbors++;
                        }
                    }
                }
                _cellList[i] = cell;
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
        
        private bool IsWithinBounds(Vector3 position)
        {
            return position.x >= 0 && position.x < _meshSpawner.size.x &&
                   position.y >= 0 && position.y < _meshSpawner.size.y &&
                   position.z >= 0 && position.y < _meshSpawner.size.z;
        }
        
        private void ApplyCellularAutomata()
        {
            for (int i = 0; i < _cellList.Count; i++)
            {
                if (_cellList[i].Neighbors < minNeighborCount)
                {
                    Cell cell = _cellList[i];
                    cell.IsAlive = false;
                    _cellList[i] = cell;
                    
                    _cellList[i].CellGameObject.SetActive(false);
                }
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
            CalculateCellularAutomata();
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
                yield return new WaitForSeconds(0.01f);
            }
            CalculateCellularAutomata();
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
                yield return new WaitForSeconds(0.01f);
            }
            _isRunning = false;
        }
    }
}
