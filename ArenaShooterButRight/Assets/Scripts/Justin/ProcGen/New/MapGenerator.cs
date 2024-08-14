using UnityEngine;

namespace Justin.ProcGen.New
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class MapGenerator : MonoBehaviour
    {
        [Header("Mapvalues")]
        [SerializeField] private int mapWidth = 100;
        [SerializeField] private int mapHeight = 100;

        [Header("Noisevalues")]
        [SerializeField] private float noiseScale = 20f;
        [SerializeField] private int octaves = 4;
        [SerializeField] private float persistence = 0.5f;
        [SerializeField] private float lacunarity = 2f;
        [SerializeField] private int seed;
        [SerializeField] private Vector2 offset;

        [SerializeField] private float meshHeightMultiplier = 20f;
        [SerializeField] private AnimationCurve meshHeightCurve;

        [Header("Texturevalues")]
        [SerializeField] private TerrainTypeConfig terrainTypeConfig;

        [Header("Meshreferenzen")]
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private MeshFilter meshFilter;

        public bool autoUpdate;

        // Start is called before the first frame update
        void Start()
        {
            GenerateMap();
        }

        public void GenerateMap()
        {
            // Generate Noisemap based on given parameters
            float[,] noiseMap = NoiseGenerator.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistence, lacunarity, offset);

            // Generate Terrain-Mesh based on noisemap
            MeshData meshData = MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve);

            // Apply generated mesh to meshfilter
            meshFilter.mesh = meshData.CreateMesh();

            // Generate and apply the texture to the meshrenderer
            Texture2D texture = TextureGenerator.TextureFromHeightMap(noiseMap, terrainTypeConfig);
            meshRenderer.sharedMaterial.mainTexture = texture;

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
            if (meshHeightMultiplier < 0)
            {
                meshHeightMultiplier = 0;
            }
        }
    }
}



