using UnityEngine;

namespace Justin.ProcGen.New
{
    public static class TextureGenerator
    {
        internal static Texture2D TextureFromHeightMap(float[,] heightMap, TerrainTypeConfig terrainTypeConfig)
        {
            int width = heightMap.GetLength(0);
            int height = heightMap.GetLength(1);

            Texture2D texture = new Texture2D(width, height);
            Color[] colorMap = new Color[width * height];

            // Assign to every Point in the map a Color based on the height
            for (int y = 0; y < height; y++) 
            {
                for (int x = 0; x < width; x++)
                {
                    float currentHeight = heightMap[x, y];
                    for (int i = 0; i < terrainTypeConfig.regions.Length; i++) 
                    {
                        if (currentHeight <= terrainTypeConfig.regions[i].height)
                        {
                            colorMap[y * width + x] = terrainTypeConfig.regions[i].color;
                            break;
                        }
                    }
                }
            }

            texture.filterMode = FilterMode.Point; // Wichtiger Schritt, um die Pixeligkeit (Low-Poly-Stil) zu erhalten
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.SetPixels(colorMap);
            texture.Apply();

            return texture;
        }
    }
}

