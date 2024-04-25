using Fabian.Generation._3DGeneration.MeshGen;
using UnityEngine;

namespace Fabian.Generation._3DGeneration.NoiseGen
{
    public class MapDisplay : MonoBehaviour
    {
        [SerializeField] private Renderer textureRenderer;
        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private MeshRenderer meshRenderer;

        public void DrawTexture(Texture2D texture)
        {
            textureRenderer.sharedMaterial.mainTexture = texture;
        }

        public void DrawMesh(FWMeshData fwMeshData, Texture2D texture)
        {
            meshFilter.sharedMesh = fwMeshData.CreateMesh();
            meshRenderer.sharedMaterial.mainTexture = texture;
        }
    }
}
