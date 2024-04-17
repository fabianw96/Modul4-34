using System;
using Fabian.Generation._3DGeneration.MeshGen;
using Fabian.Generation.ScriptableObjects;
using UnityEngine;
using UnityEngine.Serialization;

namespace Fabian.Generation._3DGeneration.PerlinNoise
{
    public class MapGeneration : MonoBehaviour
    {
        public enum DrawStyle
        {
            Noise,
            Color
        };

        public DrawStyle drawStyle;
        [SerializeField] public int mapWidth;
        [SerializeField] public int mapHeight;
        [SerializeField] public float heightMultiplier;
        [SerializeField] private float noiseScale;
        [SerializeField] private int octaves;
        [SerializeField] [Range(0, 1)] private float persistance;
        [SerializeField] private float lacunarity;
        [SerializeField] private int seed;
        [SerializeField] private Vector2 offset;
        [SerializeField] private TerrainType[] terrainTypes;
        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private AnimationCurve curve;

        [SerializeField] public bool autoUpdate;

        private void Start()
        {
            GenerateMap();
        }

        public void GenerateMap()
        {
            float[,] noiseMap = Noise.GenNoiseMap(mapWidth, mapHeight, seed ,noiseScale, octaves, persistance, lacunarity, offset);

            ColorGeneratedMap(noiseMap);
            meshFilter.mesh = MeshGeneration.Generate(mapWidth, mapHeight, noiseMap, heightMultiplier, curve);

            MapDisplay display = FindObjectOfType<MapDisplay>();
            if (drawStyle == DrawStyle.Noise)
            {
                display.DrawTexture(TextureGeneration.TextureFromHeightMap(noiseMap));
            }
            else if (drawStyle == DrawStyle.Color)
            {
                display.DrawTexture(TextureGeneration.TextureFromColorMap(ColorGeneratedMap(noiseMap), mapWidth, mapHeight));
            }
        }

        private Color[] ColorGeneratedMap(float[,] noiseMap)
        {
            Color[] colorMap = new Color[mapWidth * mapHeight];
            
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    float currentHeight = noiseMap[x, y];

                    for (int i = 0; i < terrainTypes.Length; i++)
                    {
                        if (currentHeight <= terrainTypes[i].terraingHeight)
                        {
                            colorMap[y * mapWidth + x] = terrainTypes[i].Color;
                            break;
                        }
                    }
                }
            }

            return colorMap;
        }

        private void OnValidate()
        {
            if (mapWidth < 1)
            {
                mapWidth = 1;
            }
            if (mapHeight < 1)
            {
                mapHeight = 1;
            }
            if (lacunarity < 1)
            {
                lacunarity = 1;
            }
            if (octaves < 0)
            {
                octaves = 0;
            }

            if (heightMultiplier < 0)
            {
                heightMultiplier = 0;
            }
        }
        
        
    }
}
