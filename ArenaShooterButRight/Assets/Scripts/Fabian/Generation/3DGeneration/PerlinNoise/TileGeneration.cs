using Fabian.Generation.ScriptableObjects;
using UnityEngine;

namespace Fabian.Generation._3DGeneration.PerlinNoise
{
    public class TileGeneration : MonoBehaviour
    {
        [SerializeField] private NoiseMapGeneration noiseMapGeneration;
        [SerializeField] private MeshRenderer tileRenderer;
        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private MeshCollider meshCollider;
        [SerializeField] private float mapScale;
        [SerializeField] private float heightMultiplier;
        [SerializeField] private TerrainType[] terrainTypes;
        [SerializeField] private AnimationCurve heightCurve;
        [SerializeField] private NoiseWave[] waves;
        
        [SerializeField] private bool useDomainWarping;
        [SerializeField] private int octaves = 4;
        [SerializeField] private float lacunarity = 2f;
        [SerializeField] private float gain = 0.5f;
        
        private static readonly int Smoothness = Shader.PropertyToID("_Smoothness");

        private void Start()
        {
            GenerateTile();
        }

        private void GenerateTile()
        {
            Vector3[] meshvertices = meshFilter.mesh.vertices;
            int tileDepth = (int)Mathf.Sqrt(meshvertices.Length);
            int tileWidth = tileDepth;

            float offsetX = -this.gameObject.transform.position.x;
            float offsetZ = -this.gameObject.transform.position.z;

            float[,] heightMap = new float[tileDepth, tileWidth];
            // float[,] heightMap;

            if (!useDomainWarping)
            {
                heightMap = noiseMapGeneration.GenerateNoiseMap(tileDepth, tileWidth, mapScale, offsetX, offsetZ, waves);
            }
            else
            {
                for (int zIndex = 0; zIndex < tileDepth; zIndex++)
                {
                    for (int xIndex = 0; xIndex < tileWidth; xIndex++)
                    {
                        float xPos = (float)xIndex / tileWidth * mapScale;
                        float zPos = (float)zIndex / tileDepth * mapScale;
                        heightMap[zIndex, xIndex] = NoiseFunctions.Pattern(xPos, zPos, mapScale, octaves, lacunarity, gain);
                    }
                }
            }
            

            Texture2D tileTexture = BuildTexture(heightMap);
            tileRenderer.material.mainTexture = tileTexture;
            
            tileRenderer.material.SetFloat(Smoothness, 0);
            UpdateMeshVertices(heightMap);
        }

        private Texture2D BuildTexture(float[,] heightMap)
        {
            int tileDepth = heightMap.GetLength(0);
            int tileWidth = heightMap.GetLength(1);

            Color[] colorMap = new Color[tileDepth * tileWidth];
            for (int zIndex = 0; zIndex < tileDepth; zIndex++)
            {
                for (int xIndex = 0; xIndex < tileWidth; xIndex++)
                {
                    int colorIndex = zIndex * tileWidth + xIndex;
                    float height = heightMap[zIndex, xIndex];

                    TerrainType terrainType = ChooseTerrainType(height);

                    colorMap[colorIndex] = terrainType.Color;

                    // colorMap[colorIndex] = Color.Lerp(Color.black, Color.white, height);
                }
            }

            Texture2D tileTexture = new Texture2D(tileWidth, tileDepth);
            tileTexture.wrapMode = TextureWrapMode.Clamp;
            tileTexture.SetPixels(colorMap);
            tileTexture.Apply();

            return tileTexture;
        }

        private TerrainType ChooseTerrainType(float height)
        {
            foreach (TerrainType terrainType in terrainTypes)
            {
                if (height < terrainType.terraingHeight)
                {
                    return terrainType;
                }
            }
            return terrainTypes[terrainTypes.Length - 1];
        }

        private void UpdateMeshVertices(float[,] heightMap)
        {
            int tileDepth = heightMap.GetLength(0);
            int tileWidth = heightMap.GetLength(1);

            Vector3[] meshVertices = meshFilter.mesh.vertices;

            int vertexIndex = 0;

            for (int zIndex = 0; zIndex < tileDepth; zIndex++)
            {
                for (int xIndex = 0; xIndex < tileWidth; xIndex++)
                {
                    float height = heightMap[zIndex, xIndex];
                    Vector3 vertex = meshVertices[vertexIndex];

                    meshVertices[vertexIndex] = new Vector3(vertex.x, heightCurve.Evaluate(height) * heightMultiplier, vertex.z);
                    vertexIndex++;
                }
            }

            meshFilter.mesh.vertices = meshVertices;
            meshFilter.mesh.RecalculateBounds();
            meshFilter.mesh.RecalculateNormals();
            meshCollider.sharedMesh = meshFilter.mesh;
        }
    }
}
