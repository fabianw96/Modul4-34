using Fabian.Generation.ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace Fabian.Generation._3DGeneration
{
    public class NoiseMapGeneration : MonoBehaviour
    {
        public float[,] GenerateNoiseMap(int mapDepth, int mapWidth, float scale, float offsetX, float offsetZ, NoiseWave[] waves)
        {
            float[,] noiseMap = new float[mapDepth, mapWidth];
            
            for (int zIndex = 0; zIndex < mapDepth; zIndex ++) {
                for (int xIndex = 0; xIndex < mapWidth; xIndex++) {
                    
                    float sampleX = (xIndex + offsetX) / scale;
                    float sampleZ = (zIndex + offsetZ) / scale;

                    float noise = 0f;
                    float normalization = 0f;
                    foreach (NoiseWave wave in waves)
                    {
                        noise += wave.amplitude * Mathf.PerlinNoise (sampleX * wave.frequency + wave.seed, sampleZ * wave.frequency + wave.seed);
                        normalization += wave.amplitude;
                    }

                    noise /= normalization;
                    
                    noiseMap [zIndex, xIndex] = noise;
                }
            }
            return noiseMap;
        }
        
    }
}
