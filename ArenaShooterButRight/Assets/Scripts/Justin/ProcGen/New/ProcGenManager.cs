using UnityEngine;

namespace Justin.ProcGen
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class ProcGenManager : MonoBehaviour
    {
        public enum DrawMode
        {
            ShaderGraph,
            HeightMap,
        }

        [Header("Mapvalues")]
        // Has to be divisible by (Size - 1) / 2,4,6,8 for LODs to work, because of Vertices per Line
        [SerializeField] private const int MAP_SIZE = 241;
        [Range(0,4)][SerializeField] private int levelOfDetail;

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
        [SerializeField] private DrawMode drawMode;
        [SerializeField] private Shader shader;
        [SerializeField] private TerrainTypeConfig terrainTypeConfig;
        private Material material;

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
            float[,] noiseMap = NoiseGenerator.GenerateNoiseMap(MAP_SIZE, MAP_SIZE, seed, noiseScale, octaves, persistence, lacunarity, offset);

            // Generate Terrain-Mesh based on noisemap
            MeshData meshData = MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail);

            // Apply generated mesh to meshfilter
            meshFilter.mesh = meshData.CreateMesh();


            if (drawMode == DrawMode.HeightMap)
            {
                // Generate and apply the texture to the meshrenderer
                Texture2D texture = TextureGenerator.TextureFromHeightMap(noiseMap, terrainTypeConfig);
                meshRenderer.sharedMaterial.mainTexture = texture;
            }
            else if (drawMode == DrawMode.ShaderGraph)
            {
                meshRenderer.sharedMaterial.shader = shader;
            }


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