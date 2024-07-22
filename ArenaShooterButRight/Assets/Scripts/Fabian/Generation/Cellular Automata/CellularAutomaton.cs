using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Fabian.Generation.Cellular_Automata
{
    public class CellularAutomaton : MonoBehaviour
    {
        private struct Cell
        {
            public GameObject CellGameObject;
            public Transform CellTransform;
            public bool IsAlive;
        }
        
        private List<Cell> _cellList = new();

        public void GetListFromSpawner(List<GameObject> gosList)
        {
            foreach (GameObject obj in gosList)
            {
                Cell tempCell = new Cell
                {
                    CellGameObject = obj,
                    CellTransform = obj.transform,
                    IsAlive = true
                };
                
                _cellList.Add(tempCell);
            }
        }

        private void CalculateCellularAutomata()
        {
            // foreach (Cell c in _cellList)
            // {
            //     int neigborCount = 0;
            //
            //     for (int  = 0;  < UPPER; ++)
            //     {
            //         
            //     }
            // }
        }
        
        //apply the CA with different rules to this list.
        private void ApplyCellularAutomata()
        {
            
        }
    }
}
