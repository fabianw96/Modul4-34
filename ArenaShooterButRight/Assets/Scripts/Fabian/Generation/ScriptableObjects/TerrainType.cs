using UnityEngine;

namespace Fabian.Generation.ScriptableObjects
{
    [CreateAssetMenu(menuName = "ScriptableObjects/FabianGen/TerrainType")]
    public class TerrainType : ScriptableObject
    {
        public string terrainName;
        public float terraingHeight;
        public Color Color;
    }
}
