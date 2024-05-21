using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace Fabian.Generation.Cellular_Automata
{
    public class CellAutomata : MonoBehaviour
    {
        public enum Grid
        {
            FLOOR,
            WALL
        }
    
        private Grid[,] _noiseGrid;
        [SerializeField] private Tilemap tileMap;
        [SerializeField] private Tile floor;
        [SerializeField] private Tile wall;
        [SerializeField] private int mapWidth;
        [SerializeField] private int mapHeight;
        [SerializeField] private int density;
        [SerializeField] private int iterationCount;

        private void OnEnable()
        {
            _noiseGrid = new Grid[mapWidth, mapHeight];
            GenerateNoise();
        }

        public void GenerateNoise()
        {
            _noiseGrid = new Grid[mapWidth,mapHeight];
            
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    if (Random.Range(0, 100) > density)
                    {
                        _noiseGrid[x, y] = Grid.FLOOR;
                    }
                    else
                    {
                        _noiseGrid[x, y] = Grid.WALL;
                    }
                }
            }
            PlaceTile();
        }

        private void PlaceTile()
        {
            for (int x = 0; x < mapHeight; x++)
            {
                for (int y = 0; y < mapWidth; y++)
                {
                    if (_noiseGrid[y,x] == Grid.FLOOR)
                    {
                        tileMap.SetTile(new Vector3Int(x, y), floor);
                    }
                    else if (_noiseGrid[y,x] == Grid.WALL)
                    {
                        tileMap.SetTile(new Vector3Int(x, y), wall);
                    }
                }
            }
        }

        public void ApplyCellularAutomata(int iterations)
        {
            for (int i = 0; i < iterations; i++)
            { 
                Grid[,] tempGrid = _noiseGrid;
            
                for (int j = 0; j < mapHeight; j++)
                {
                    for (int k = 0; k < mapWidth; k++)
                    {
                        int neighborWallCount = 0;
                    
                        for (int y = j - 1; y <= j + 1; y++)
                        {
                            for (int x = k - 1; x <= k + 1; x++)
                            {
                                if (IsWithMapBounds(x, y))
                                {
                                    if (y != j || x != k)
                                    {
                                        if (tempGrid[x,y] == Grid.WALL)
                                        {
                                            neighborWallCount++;
                                        }
                                    }
                                }
                                else
                                {
                                    neighborWallCount++;
                                }
                            }
                        }
                        
                        if (neighborWallCount > 4)
                        {
                            _noiseGrid[k, j] = Grid.WALL;
                        }
                        else
                        {
                            _noiseGrid[k, j] = Grid.FLOOR;
                        }
                    }
                }
            }
            PlaceTile();
        }

        private bool IsWithMapBounds(int x, int y)
        {
            return x >= 0 && y >= 0 && x < mapWidth && y < mapHeight;
        }
    
    }
}
