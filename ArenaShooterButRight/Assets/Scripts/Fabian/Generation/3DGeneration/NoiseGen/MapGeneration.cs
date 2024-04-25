using System;
using System.Collections.Generic;
using System.Threading;
using Fabian.Generation._3DGeneration.MeshGen;
using Fabian.Generation.ScriptableObjects;
using UnityEngine;
using UnityEngine.Serialization;

namespace Fabian.Generation._3DGeneration.NoiseGen
{
    public class MapGeneration : MonoBehaviour
    {
        public enum DrawStyle
        {
            Noise,
            Color
        };
        
        public enum NoiseStyle
        {
            Perlin,
            Simplex
        }

        public const int MapChunkSize = 241;
        [SerializeField] [Range(0, 6)] private int levelOfDetailInEditor;
        [SerializeField] private DrawStyle drawStyle;
        [SerializeField] private NoiseStyle noiseStyle;
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

        private Queue<MapThreadInfo<FWMapData>> _mapDataThreadInfoQueue = new();
        private Queue<MapThreadInfo<FWMeshData>> _meshDataThreadInfoQueue = new();

        private void Start()
        {
            DrawMapInEditor();
        }

        public void DrawMapInEditor()
        {
            FWMapData fwMapData = GenerateMapData(Vector2.zero);
            MapDisplay display = FindObjectOfType<MapDisplay>();
            
            
            if (drawStyle == DrawStyle.Noise)
            {
                display.DrawMesh(MeshGeneration.Generate(fwMapData.HeightMap, heightMultiplier, curve, levelOfDetailInEditor),TextureGeneration.TextureFromHeightMap(fwMapData.HeightMap));
            }
            else if (drawStyle == DrawStyle.Color)
            {
                display.DrawMesh(MeshGeneration.Generate(fwMapData.HeightMap, heightMultiplier, curve, levelOfDetailInEditor),TextureGeneration.TextureFromColorMap(fwMapData.ColorMap, MapChunkSize, MapChunkSize));
            }
        }

        public void RequestMapData(Vector2 centre, Action<FWMapData> callback)
        {
            ThreadStart threadStart = delegate
            {
                MapDataThread(centre, callback);
            };

            new Thread(threadStart).Start();
        }

        void MapDataThread(Vector2 centre ,Action<FWMapData> callback)
        {
            FWMapData fwMapData = GenerateMapData(centre);
            lock (_mapDataThreadInfoQueue)
            {
                _mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<FWMapData>(callback, fwMapData));
            }
        }

        public void RequestMeshData(FWMapData fwMapData, int levelOfDetail, Action<FWMeshData> callback)
        {
            ThreadStart threadStart = delegate
            {
                MeshDataThread(fwMapData, levelOfDetail, callback);
            };
            
            new Thread(threadStart).Start();
        }

        void MeshDataThread(FWMapData fwMapData, int levelOfDetail, Action<FWMeshData> callback)
        {
            FWMeshData fwMeshData = MeshGeneration.Generate(fwMapData.HeightMap, heightMultiplier, curve, levelOfDetail);
            lock (_meshDataThreadInfoQueue)
            {
                _meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<FWMeshData>(callback, fwMeshData));
            }
        }

        private void Update()
        {
            if (_mapDataThreadInfoQueue.Count > 0)
            {
                for (int i = 0; i < _mapDataThreadInfoQueue.Count; i++)
                {
                    MapThreadInfo<FWMapData> threadInfo = _mapDataThreadInfoQueue.Dequeue();
                    threadInfo.Callback(threadInfo.Parameter);
                }
            }

            if (_meshDataThreadInfoQueue.Count > 0)
            {
                for (int i = 0; i < _meshDataThreadInfoQueue.Count; i++)
                {
                    MapThreadInfo<FWMeshData> threadInfo = _meshDataThreadInfoQueue.Dequeue();
                    threadInfo.Callback(threadInfo.Parameter);
                }
            }
        }

        FWMapData GenerateMapData(Vector2 centre)
        {
            FastNoiseLite noiseLite = new FastNoiseLite();
            
            float[,] noiseMap = new float[MapChunkSize,MapChunkSize];
            
            if (noiseStyle == NoiseStyle.Perlin)
            {
                noiseMap = Noise.GenNoiseMap(MapChunkSize, MapChunkSize, seed ,noiseScale, octaves, persistance, lacunarity, centre + offset);
            }
            else if (noiseStyle == NoiseStyle.Simplex)
            {
                noiseLite.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
                
                noiseLite.SetSeed(seed);
                noiseLite.SetFractalOctaves(octaves);
                noiseLite.SetFractalLacunarity(lacunarity);
                noiseLite.SetFractalType(FastNoiseLite.FractalType.DomainWarpIndependent);
                noiseLite.SetDomainWarpType(FastNoiseLite.DomainWarpType.OpenSimplex2);
                
                for (int x = 0; x < MapChunkSize; x++)
                {
                    for (int y = 0; y < MapChunkSize; y++)
                    {
                        noiseMap[x, y] = noiseLite.GetNoise(x, y);
                    }
                }
            }
            
            return new FWMapData(noiseMap, ColorGeneratedMap(noiseMap));
        }

        private Color[] ColorGeneratedMap(float[,] noiseMap)
        {
            Color[] colorMap = new Color[MapChunkSize * MapChunkSize];
            
            for (int y = 0; y < MapChunkSize; y++)
            {
                for (int x = 0; x < MapChunkSize; x++)
                {
                    float currentHeight = noiseMap[x, y];

                    for (int i = 0; i < terrainTypes.Length; i++)
                    {
                        if (currentHeight <= terrainTypes[i].terraingHeight)
                        {
                            colorMap[y * MapChunkSize + x] = terrainTypes[i].Color;
                            break;
                        }
                    }
                }
            }

            return colorMap;
        }

        private void OnValidate()
        {
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

        struct MapThreadInfo<T>
        {
            public readonly Action<T> Callback;
            public readonly T Parameter;

            public MapThreadInfo(Action<T> callback, T parameter)
            {
                Callback = callback;
                Parameter = parameter;
            }
        }
    }

    public struct FWMapData
    {
        public readonly float[,] HeightMap;
        public readonly Color[] ColorMap;

        public FWMapData(float[,] heightMap, Color[] colorMap)
        {
            HeightMap = heightMap;
            ColorMap = colorMap;
        }
    }
}
