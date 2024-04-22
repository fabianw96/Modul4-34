using UnityEngine;

namespace Fabian.Generation._3DGeneration.NoiseGen
{
    public class MapDisplay : MonoBehaviour
    {
        [SerializeField] private Renderer textureRenderer;

        public void DrawTexture(Texture2D texture)
        {
            textureRenderer.sharedMaterial.mainTexture = texture;
        }
    }
}
