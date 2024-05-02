using UnityEngine;

namespace Fabian.Generation._3DGeneration.NoiseGen
{
    public static class Noise
    {
        public enum NormalizeMode
        {
            Local,
            Global
        }
        
        public static float[,] GenNoiseMap(int mapWidth, int mapHeight,int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, NormalizeMode normalizeMode)
        {
            System.Random rndm = new System.Random(seed);
            Vector2[] octaveOffsets = new Vector2[octaves];
            float maxPossibleHeight = 0;
            float amp = 1;
            float freq = 1;
            float[,] noiseMap = new float[mapWidth,mapHeight];
            
            float maxLocalNoiseHeight = float.MinValue;
            float minLocalNoiseHeight = float.MaxValue;

            float halfWidth = mapWidth / 2f;
            float halfHeight = mapHeight / 2f;

            if (scale <= 0)
            {
                scale = 0.001f;
            }
            
            for (int i = 0; i < octaves; i++)
            {
                float offsetX = rndm.Next(-100000, 100000) + offset.x;
                float offsetY = rndm.Next(-100000, 100000) - offset.y;

                octaveOffsets[i] = new Vector2(offsetX, offsetY);
                maxPossibleHeight += amp;
                amp *= persistance;
            }
            
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    amp = 1;
                    freq = 1;
                    float noiseHeight = 0;
                
                    for (int i = 0; i < octaves; i++)
                    {
                        float sampleX = (x - halfWidth + octaveOffsets[i].x) / scale * freq;
                        float sampleY = (y - halfHeight + octaveOffsets[i].y) / scale * freq;

                        float perlinVal = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                        // noiseMap[x, y] = perlinVal;
                        noiseHeight += perlinVal * amp;

                        //persistance 0 - 1, decreases amp each octave
                        amp *= persistance;
                        //lacunarity 1+, increases freq each octave
                        freq *= lacunarity;
                    }

                    if (noiseHeight > maxLocalNoiseHeight)
                    {
                        maxLocalNoiseHeight = noiseHeight;
                    }
                    else if (noiseHeight < minLocalNoiseHeight)
                    {
                        minLocalNoiseHeight = noiseHeight;
                    }
                
                    noiseMap[x, y] = noiseHeight;
                }
            }
        
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    if (normalizeMode == NormalizeMode.Local)
                    {
                        noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
                    }
                    else
                    {
                        float normalizedHeight = (noiseMap[x, y] + 1) / maxPossibleHeight;
                        noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                    }
                }
            }
        
            return noiseMap;
        }
    }
}
