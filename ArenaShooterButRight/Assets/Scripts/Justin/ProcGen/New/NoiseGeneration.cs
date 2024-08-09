using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Justin.ProcGen.New
{
    public static class NoiseGeneration
    {
        public static float[,] GenerateNoiseMap(int meshWidth, int meshDepth, int seed, int octaves)
        {
            float[,] noiseMap = new float[meshWidth, meshDepth];
            System.Random rnd = new System.Random(seed);

            Vector2[] octaveOffsets = new Vector2[octaves];
            return noiseMap;

            // frequency zooms in and out of the noise
        }
    }
}
