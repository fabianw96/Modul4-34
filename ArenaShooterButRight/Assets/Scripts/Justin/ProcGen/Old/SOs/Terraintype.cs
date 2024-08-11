using UnityEngine;

namespace Justin.ProcGen.SOs
{
    [CreateAssetMenu(menuName = "ScriptableObjects/JustinGen/TerrainType")]
    public class TerrainType : ScriptableObject
    {
        public string terrainName;
        public float terraingHeight;
        public Color Color;
    }
}
