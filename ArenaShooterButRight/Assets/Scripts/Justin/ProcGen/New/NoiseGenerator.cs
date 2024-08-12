using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Justin.ProcGen.New
{
    public static class NoiseGenerator
    {
        public static float[,] GenerateNoiseMap(int width, int height, int seed, float noiseScale, int octaves, float persistence, float lacunarity, Vector2 offset)
        {
            float[,] noiseMap = new float[width, height];

            // Initialize generator with provided seed
            System.Random rnd = new System.Random(seed);
            Vector2[] octaveOffsets = new Vector2[octaves];

            // Set offset for every octaves
            for (int i = 0; i < octaves; i++)
            {
                float offsetX = rnd.Next(-100000,100000) + offset.x;
                float offsetY = rnd.Next(-100000,100000) + offset.y;
                octaveOffsets[i] = new Vector2(offsetX, offsetY);
            }

            // prohibit division with zero
            if (noiseScale <= 0)
            {
                noiseScale = 0.0001f;
            }

            float maxNoiseHeight = float.MinValue;
            float minNoiseHeight = float.MaxValue;

            // calculate Noise-Value for every position on the Map
            for (int y = 0; y < height; y++) 
            {
                for (int x = 0; x < width; x++)
                {
                    float amplitude = 1f;
                    float frequency = 1f;
                    float noiseHeight = 1f;

                    for ( int i = 0; i < octaves; i++)
                    {
                        // calculate the position in the Noise-Function
                        float sampleX = (x - width / 2f) / noiseScale * frequency + octaveOffsets[i].x;
                        float sampleY = (y - height / 2f) / noiseScale * frequency + octaveOffsets[i].y;

                        // Calculate Perlin Noise value
                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                        noiseHeight += perlinValue * amplitude;

                        amplitude *= persistence;
                        frequency *= lacunarity;
                    }

                    // Refresh the min/max height, to normalize later on
                    if (noiseHeight > maxNoiseHeight)
                    {
                        maxNoiseHeight = noiseHeight;
                    }
                    if (noiseHeight < minNoiseHeight)
                    {
                        minNoiseHeight = noiseHeight;
                    }

                    noiseMap[x, y] = noiseHeight;
                }
            }

            // normalize the Noisemap-Values
            for (int y = 0; y < height; y++)
            {
                for(int x = 0; x < width; x++)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
                }
            }
            return noiseMap;
        }
    }
}
