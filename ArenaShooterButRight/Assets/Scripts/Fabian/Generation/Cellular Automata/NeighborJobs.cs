using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using static Fabian.Generation.Cellular_Automata.CellularAutomaton;

namespace Fabian.Generation.Cellular_Automata
{
    [BurstCompile]
    public struct MooreNeighborsJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<Cell> Cells;
        public NativeArray<int> NeighborCount;
        [ReadOnly] public NativeHashMap<float3, Cell> CellMap;
        public int3 Size;
        
        
        public void Execute(int index)
        {
            Cell cell = Cells[index];
            
            int neighbors = 0;

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    for (int z = -1; z <= 1; z++)
                    {
                        if (x == 0 && y == 0 && z == 0) continue;
                        
                        Vector3 neighborPosition = cell.CellPosition + new float3(x, y, z);

                        if (!IsWithinBounds(neighborPosition)) continue;
                        
                        if (CellMap.TryGetValue(neighborPosition, out Cell neighborCell))
                        {
                            if (neighborCell.States > 0)
                            {
                                neighbors++;
                            }
                        }
                    }
                }
            }
            NeighborCount[index] = neighbors;
        }
        
        private bool IsWithinBounds(Vector3 position)
        {
            return position.x >= 0 && position.x < Size.x &&
                   position.y >= 0 && position.y < Size.y &&
                   position.z >= 0 && position.z < Size.z;
        }
    }
    
    [BurstCompile]
    public struct NeumannNeighborsJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<Cell> Cells;
        public NativeArray<int> NeighborCounts;
        [ReadOnly] public NativeHashMap<float3, Cell> CellMap;
        public float3 Size;

        public void Execute(int index)
        {
            Cell cell = Cells[index];
            int neighbors = 0;
            
            CheckNeighbor(cell, new float3(-1, 0, 0), ref neighbors);
            CheckNeighbor(cell, new float3(1, 0, 0), ref neighbors);
            CheckNeighbor(cell, new float3(0, -1, 0), ref neighbors);
            CheckNeighbor(cell, new float3(0, 1, 0), ref neighbors);
            CheckNeighbor(cell, new float3(0, 0, -1), ref neighbors);
            CheckNeighbor(cell, new float3(0, 0, 1), ref neighbors);
            
            NeighborCounts[index] = neighbors;
        }

        private void CheckNeighbor(Cell cell, float3 direction, ref int neighbors)
        {
            float3 neighborPosition = cell.CellPosition + direction;

            if (IsWithinBounds(neighborPosition))
            {
                if (CellMap.TryGetValue(neighborPosition, out Cell neighborCell))
                {
                    if (neighborCell.States > 0)
                    {
                        neighbors++;
                    }
                }
            }
        }

        private bool IsWithinBounds(float3 position)
        {
            return position.x >= 0 && position.x < Size.x &&
                   position.y >= 0 && position.y < Size.y &&
                   position.z >= 0 && position.z < Size.z;
        }
    }
}
