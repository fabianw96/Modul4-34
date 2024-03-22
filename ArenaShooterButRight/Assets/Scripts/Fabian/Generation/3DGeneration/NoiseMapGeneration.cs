using UnityEngine;

namespace Fabian.Generation._3DGeneration
{
    public class NoiseMapGeneration : MonoBehaviour
    {
        public float[,] GenerateNoiseMap(int mapDepth, int mapWidth, float scale, float offsetX, float offsetZ)
        {
            float[,] noiseMap = new float[mapDepth, mapWidth];
            for (int zIndex = 0; zIndex < mapDepth; zIndex ++) {
                for (int xIndex = 0; xIndex < mapWidth; xIndex++) {
                    float sampleX = (xIndex + offsetX) / scale;
                    float sampleZ = (zIndex + offsetZ) / scale;
                    float noise = Mathf.PerlinNoise (sampleX, sampleZ);
                    noiseMap [zIndex, xIndex] = noise;
                }
            }
            return noiseMap;
        }
    }
}
