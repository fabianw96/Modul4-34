using UnityEngine;

namespace Justin.ProcGen
{
    /// <summary>
    /// The NoiseGenerator class generates a 2D noise map using Perlin noise.
    /// The generated noise map is used to create height maps for procedural terrain generation.
    /// </summary>
    public static class NoiseGenerator
    {
        /// <summary>
        /// Generates a noise map based on the specified parameters, using Perlin noise.
        /// </summary>
        /// <param name="width">The width of the noise map.</param>
        /// <param name="height">The height of the noise map.</param>
        /// <param name="seed">Seed value for randomizing the noise</param>
        /// <param name="noiseScale">Scale of the noise; higher values zoom out the noise pattern</param>
        /// <param name="octaves">Number of layers of noise to combine for added complexity.</param>
        /// <param name="persistence">Controls how much the amplitude decreases with each octave.</param>
        /// <param name="lacunarity">Controls how much the frequency increases with each octave.</param>
        /// <param name="offset">Offset to move the noise map in X and Y direction.</param>
        /// <returns>A 2D float array representing the generated noise map.</returns>
        public static float[,] GenerateNoiseMap(int width, int height, int seed, float noiseScale, int octaves, float persistence, float lacunarity, Vector2 offset)
        {
            float[,] noiseMap = new float[width, height];
            System.Random rnd = new System.Random(seed);
            Vector2[] octaveOffsets = new Vector2[octaves];

            // Generate random offsets for each octave to ensure variation.
            for (int i = 0; i < octaves; i++)
            {
                float offsetX = rnd.Next(-100000,100000) + offset.x;
                float offsetY = rnd.Next(-100000,100000) + offset.y;
                octaveOffsets[i] = new Vector2(offsetX, offsetY);
            }

            // Prevent division by zero in the noise scale.
            if (noiseScale <= 0)
            {
                noiseScale = 0.0001f;
            }

            float maxNoiseHeight = float.MinValue;
            float minNoiseHeight = float.MaxValue;

            // Loop through each point in the noise map.
            for (int y = 0; y < height; y++) 
            {
                for (int x = 0; x < width; x++)
                {
                    float amplitude = 1f;
                    float frequency = 1f;
                    float noiseHeight = 1f;

                    // Generate noise for each octave.
                    for ( int i = 0; i < octaves; i++)
                    {
                        float sampleX = (x - width / 2f) / noiseScale * frequency + octaveOffsets[i].x;
                        float sampleY = (y - height / 2f) / noiseScale * frequency + octaveOffsets[i].y;

                        // Use Perlin noise function to generate values and map them to [-1, 1].
                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                        noiseHeight += perlinValue * amplitude;

                        // Amplitude decreases with each octave since persistence is always smaller than 1.
                        // Causes each subsequent octave to have less impact on the noisemap.
                        amplitude *= persistence;
                        // frequency increases with each octave since lacunarity is always greater than 1.
                        // Causes each subsequent octave to have a more detailed noise curve for more detail.
                        frequency *= lacunarity;
                    }

                    // Track the maximum and minimum noise height for normalization.
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

            // Normalize the noise map to be between 0 and 1.
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
