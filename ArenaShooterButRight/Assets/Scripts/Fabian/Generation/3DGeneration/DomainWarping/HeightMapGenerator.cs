using System;
using System.Collections;
using System.Collections.Generic;
using Fabian.Generation.ScriptableObjects;
using UnityEngine;
public class NoiseFunctions
{
    private static float Fbm(float x, float y, float scale = 1, int octaves = 1, float lacunarity = 2, float gain = 0.5f)
    {
        float total = 0;
        float amplitude = 1;
        float frequency = 1;

        for (int i = 0; i < octaves; i++)
        {
            float sampleX = x / scale;
            float sampleY = y / scale;
            
            float v = Mathf.PerlinNoise(sampleX * frequency, sampleY * frequency) * amplitude;

            total += v;
            frequency *= lacunarity;
            amplitude *= gain;
        }
        
        return total;
    }

    public static float Pattern(float x, float y, float scale, int octaves, float lacunarity, float gain)
    {
        float[] q = { Fbm(x, y, scale, octaves, lacunarity, gain), Fbm(x + 5.2f, y + 1.3f, scale, octaves, lacunarity, gain) };
        return Fbm(x + 4.0f * q[0], y + 4.0f * q[1], scale, octaves, lacunarity, gain);
    }
}
