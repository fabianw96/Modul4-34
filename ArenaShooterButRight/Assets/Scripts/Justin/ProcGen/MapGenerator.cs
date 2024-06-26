using Justin.ProcGen.SOs;
using UnityEngine;

namespace Justin.ProcGen
{
    public class MapGenerator : MonoBehaviour
    {
        public enum DrawMode
        {
            NoiseMap, 
            ColourMap,
            Mesh
        }
        public DrawMode drawMode;

        public int mapWidth;
        public int mapHeight;
        public float noiseScale;

        public int octaves;
        [Range(0, 1)]
        public float persistance;
        public float lacunarity;

        public int seed;
        public Vector2 offset;

        public float meshHeightMultiplier;
        public AnimationCurve meshHeightCurve;

        public bool autoUpdate;

        public TerrainType[] terrainType;

        public void GenerateMap()
        {
            float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);

            Color[] colourMap = new Color[mapWidth * mapHeight];
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    float currentHeight = noiseMap[x, y];
                    for (int i = 0; i < terrainType.Length; i++) 
                    {
                        if (currentHeight <= terrainType[i].terraingHeight)
                        {
                            colourMap[y * mapWidth + x] = terrainType[i].Color;
                            break;
                        }
                    }
                }
            }
            MapDisplay display = FindObjectOfType<MapDisplay>();
            if (drawMode == DrawMode.NoiseMap)
            {
                display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
            }
            else if (drawMode == DrawMode.ColourMap) 
            {
                display.DrawTexture(TextureGenerator.TextureFromColourMap(colourMap, mapWidth, mapHeight));
            }
            else if (drawMode == DrawMode.Mesh)
            {
                display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve), TextureGenerator.TextureFromColourMap(colourMap, mapWidth, mapHeight));
            }
        }

        private void OnValidate()
        {
            if (mapWidth < 1) { mapWidth = 1; }
            if (mapHeight < 1) { mapHeight = 1; }
            if (lacunarity < 1) { lacunarity = 1; }
            if (octaves < 0) { octaves = 0; }
        }
    }
}
