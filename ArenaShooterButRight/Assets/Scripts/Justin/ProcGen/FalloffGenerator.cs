using UnityEngine;

namespace Justin.ProcGen
{
    public static class FalloffGenerator
    {
        // Generates a falloff map that fades out towards the edges. Useful for island-like terrains.
        public static float[,] GenerateFalloffMap(int _size, AnimationCurve _falloffCurve)
        {
            float[,] map = new float[_size, _size];

            for (int i = 0; i < _size; i++)
            {
                for (int j = 0; j < _size; j++)
                {
                    // Normalize the coordinates to be between -1 and 1.
                    float x = i / (float)_size * 2 - 1;
                    float y = j / (float)_size * 2 - 1;

                    // Determine the distance from the edge using the maximum of the absolute values of x and y.
                    float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));

                    // Use the AnimationCurve to evaluate the falloff based on the distance from the edge.
                    map[i, j] = _falloffCurve.Evaluate(value);
                }
            }
            return map;
        }
    }
}

