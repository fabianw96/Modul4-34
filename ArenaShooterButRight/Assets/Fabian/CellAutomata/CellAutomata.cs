using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class CellAutomata : MonoBehaviour
{
    public enum Grid
    {
        FLOOR,
        WALL
    }
    
    [SerializeField] private Grid[,] NoiseGrid;
    [SerializeField] private Tilemap TileMap;
    [SerializeField] private Tile Floor;
    [SerializeField] private Tile Wall;
    [SerializeField] private int MapWidth;
    [SerializeField] private int MapHeight;
    [SerializeField] private int Density;
    [SerializeField] private int IterationCount;

    private void Start()
    {
        NoiseGrid = new Grid[MapWidth, MapHeight];
        
        GenerateNoise();
        PlaceTile();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ApplyCellularAutomata(IterationCount);
            PlaceTile();
        }
    }

    private void GenerateNoise()
    {
        for (int x = 0; x < MapWidth; x++)
        {
            for (int y = 0; y < MapHeight; y++)
            {
                if (Random.Range(0, 100) > Density)
                {
                    NoiseGrid[x, y] = Grid.FLOOR;
                }
                else
                {
                    NoiseGrid[x, y] = Grid.WALL;
                }
            }
        }
    }

    private void PlaceTile()
    {
        for (int x = 0; x < MapHeight; x++)
        {
            for (int y = 0; y < MapWidth; y++)
            {
                if (NoiseGrid[y,x] == Grid.FLOOR)
                {
                    TileMap.SetTile(new Vector3Int(x, y), Floor);
                }
                else if (NoiseGrid[y,x] == Grid.WALL)
                {
                    TileMap.SetTile(new Vector3Int(x, y), Wall);

                }
            }
        }

       
    }

    private void ApplyCellularAutomata(int iterations)
    {
        for (int i = 0; i < iterations; i++)
        { 
            Grid[,] tempGrid = NoiseGrid;
            
            for (int j = 0; j < MapHeight; j++)
            {
                for (int k = 0; k < MapWidth; k++)
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
                        NoiseGrid[k, j] = Grid.WALL;
                        // PlaceTile();
                    }
                    else
                    {
                        NoiseGrid[k, j] = Grid.FLOOR;
                        // PlaceTile();
                    }
                    Debug.Log(neighborWallCount);
                }
            }
        }
    }

    private bool IsWithMapBounds(int x, int y)
    {
        return x >= 0 && y >= 0 && x < MapWidth && y < MapHeight;
    }
    
}
