using UnityEngine;

namespace Justin.ProcGen
{
    /// <summary>
    /// The ProcGenManager class is responsible for generating procedural meshes using Perlin noise.
    /// It manages mesh generation parameters, handles mesh creation, applies falloff maps
    /// and updates the mesh in the Unity scene. 
    /// </summary>
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class ProcGenManager : MonoBehaviour
    {
        [Header("Mapvalues")]
        // The size of the map. Must be an odd number and should be divisible by factor of (MAP_SIZE - 1)
        // for the LoD system to work properly.
        private const int MAP_SIZE = 481;
        // Level of Detail: Determines the level of simplification for the mesh.
        // 0 means full detail, and higher numbers reduce the number of vertices by skipping some of them.
        [Range(0,4)][SerializeField] private int levelOfDetail;

        [Header("Noisevalues")]
        [SerializeField] private float noiseScale; // Controls the scale of the noise; larger values zoom out.
        [SerializeField] private int octaves; // Number of layers of noise added together to create more complex terrain.
        [SerializeField] private float persistence; // Controls the amplitude decrease for each octave.
        [SerializeField] private float lacunarity; // Controls the frequency increase for each octave.
        [SerializeField] private int seed; // Seed for random generation; ensures consistent results with the same seed.
        [SerializeField] private Vector2 offset; // Offsets the noise pattern in the X and Y directions.
        [SerializeField] private float meshHeightMultiplier; // Multiplier to control the height of the terrain.
        [SerializeField] private AnimationCurve meshHeightCurve; // Allows more control over the height distribution of the terrain.

        [Header("Texture Values")]
        [SerializeField] private Shader shader; // Reference to the shader used for rendering the terrain.
        private Material material; // Material that will use the shader.

        [Header("Mesh References")]
        [SerializeField] private MeshRenderer meshRenderer; // Reference to the MeshRenderer component.
        [SerializeField] private MeshFilter meshFilter; // Reference to the MeshFilter component.

        [Header("FalloffMap Values")]
        [SerializeField] private AnimationCurve falloffCurve; // Determines the shape of the falloffMap
        private float[,] falloffMap; // Stores the falloff map for blending the edges of the terrain.
        public bool useFalloffMap; // Determines whether the falloff map should be applied.

        public bool autoUpdate; // Automatically updates the terrain in the editor when parameters change.

        // Start is called before the first frame update
        void Start()
        {
            // Generate the falloff map based on the map size
            falloffMap = FalloffGenerator.GenerateFalloffMap(MAP_SIZE, falloffCurve);
            GenerateMap();
        }

        // Generates the terrain map using noise and optional falloff.
        public void GenerateMap()
        {
            // Generate Noisemap based on given parameters.
            float[,] noiseMap = NoiseGenerator.GenerateNoiseMap(MAP_SIZE, MAP_SIZE, seed, noiseScale, octaves, persistence, lacunarity, offset);

            // Apply the falloff map to the noise map if enabled.
            if (useFalloffMap)
            {
                falloffMap = FalloffGenerator.GenerateFalloffMap(MAP_SIZE, falloffCurve);
                for (int y = 0; y < MAP_SIZE; y++) 
                {
                    for (int x = 0; x < MAP_SIZE; x++) 
                    {
                        // Combine noise map with falloff map, ensuring the values stay within [0, 1].
                        noiseMap[x, y] = Mathf.Clamp01(noiseMap[x,y] - falloffMap[x,y]);
                    }
                }
            }
            // Generate the terrain mesh from the noisemapbased on noisemap
            MeshData meshData = MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail);

            // Apply the generated mesh to the meshfilter
            meshFilter.mesh = meshData.CreateMesh();

            // Apply the shader material to the MeshRenderer
            meshRenderer.sharedMaterial.shader = shader;

            // Set the MeshHeightMultiplier property in the shader to match the value used in mesh generation.
            meshRenderer.sharedMaterial.SetFloat("_MeshHeightMultiplier", meshHeightMultiplier);
        }

        // Ensures that certain parameters stay within valid ranges when modified in the editor.
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

            if (autoUpdate)
            {
                GenerateMap();
            }
        }
    }
}