using UnityEngine;

namespace Justin.ProcGen
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class ProcGenManager : MonoBehaviour
    {
        [Header("Mapvalues")]
        // Has to be divisible by (Size - 1) / 2,4,6,8 for LODs to work, because of Vertices per Line
        [SerializeField] private const int MAP_SIZE = 481;
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
        [SerializeField] private Shader shader;
        private Material material;

        [Header("Meshreferenzen")]
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private MeshFilter meshFilter;

        private float[,] falloffMap;
        public bool useFalloffMap;
        public bool autoUpdate;

        // Start is called before the first frame update
        void Start()
        {
            falloffMap = FalloffGenerator.GenerateFalloffMap(MAP_SIZE);
            GenerateMap();
        }

        public void GenerateMap()
        {
            // Generate Noisemap based on given parameters
            float[,] noiseMap = NoiseGenerator.GenerateNoiseMap(MAP_SIZE, MAP_SIZE, seed, noiseScale, octaves, persistence, lacunarity, offset);

            if (useFalloffMap)
            {
                for (int y = 0; y < MAP_SIZE; y++) 
                {
                    for (int x = 0; x < MAP_SIZE; x++) 
                    {
                        noiseMap[x, y] = Mathf.Clamp01(noiseMap[x,y] - falloffMap[x,y]);
                    }
                }
            }
            // Generate Terrain-Mesh based on noisemap
            MeshData meshData = MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail);

            // Apply generated mesh to meshfilter
            meshFilter.mesh = meshData.CreateMesh();

            // Apply Shader Material
            meshRenderer.sharedMaterial.shader = shader;
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

            falloffMap = FalloffGenerator.GenerateFalloffMap(MAP_SIZE);
        }
    }
}