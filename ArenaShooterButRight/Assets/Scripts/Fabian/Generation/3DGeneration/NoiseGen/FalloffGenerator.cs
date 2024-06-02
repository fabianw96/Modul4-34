// using System.Collections;
// using System.Collections.Generic;
// using Fabian.Generation._3DGeneration.NoiseGen;
// using Unity.Mathematics;
// using UnityEngine;
//
// public static class FalloffGenerator
// {
//     public static float[,] GenerateFalloffMap(int size)
//     {
//         float[,] map = new float[size, size];
//
//         for (int i = 0; i < size; i++)
//         {
//             for (int j = 0; j < size; j++)
//             {
//                 float x = i / (float)size * 2 - 1;
//                 float y = j / (float)size * 2 - 1;
//
//                 float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
//                 map[i, j] = value;
//             } 
//         }
//
//         return map;
//     }
//
//     public static float[,] GenerateCaFalloffMap(int size, float cutOffValue, int iterations, int maxNeighbors, float maxHeightDiff)
//     {
//         //wo kommentar
//         float minHeight = cutOffValue * cutOffValue;
//         float[,] map = Noise.GenNoiseMap(size, size, 0, 100, 3, 0.5f, 2f, new Vector2(0,0), Noise.NormalizeMode.Local);
//
//         // float[,] emptyMap = new float[size, size];
//         
//         for (int i = 0; i < iterations; i++)
//         {
//             float[,] tempMap = map;
//             cutOffValue += 0.1f;
//
//             for (int j = 0; j < size; j++)
//             {
//                 for (int k = 0; k < size; k++)
//                 {
//                     int neighborCount = 0;
//
//                     for (int y = j - 1; y <= j + 1; y++)
//                     {
//                         for (int x = k - 1; x <= k + 1; x++)
//                         {
//                             if (IsWithinMap(x, y, size))
//                             {
//                                 if (y != j || x != k)
//                                 {
//                                     if (tempMap[x,y] < cutOffValue)
//                                     {
//                                         neighborCount++;
//                                     }
//                                 }
//                             }
//                             else
//                             {
//                                 neighborCount++;
//                             }
//                         }
//                     }
//
//                     
//                     // 1/8 weil maximum neighbors = 8 wenn diagonal gechecked wird im CA
//                     float heightDifference = 1f/8f * (8 - neighborCount);
//
//                     if (heightDifference > maxHeightDiff)
//                     {
//                         map[k, j] -= 0.01f;
//                     }
//                     else
//                     {
//                         map[k, j] -= heightDifference;
//                     }
//                     // Debug.Log(neighborCount);
//
//                     // if (neighborCount > maxNeighbors)
//                     // {
//                     //     emptyMap[k, j] +=  0.1f;
//                     // }
//                     //
//                     // if (emptyMap[k,j] < minHeight)
//                     // {
//                     //     emptyMap[k, j] = 0f;
//                     // }
//                 }    
//             }
//         }
//         
//         return map;
//     }
//
//     private static bool IsWithinMap(int x, int y, int size)
//     {
//         return x >= 0 && y >= 0 && x < size && y < size;
//     }
// }
