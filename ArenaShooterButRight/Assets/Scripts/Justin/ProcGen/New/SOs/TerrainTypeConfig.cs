using UnityEngine;

namespace Justin.ProcGen.New
{
    [CreateAssetMenu(menuName = "ScriptableObjects/JustinGen/New/TerrainDataset", order = 1)]
    public class TerrainTypeConfig : ScriptableObject
    {
        public TerrainType[] regions;
    }

    [System.Serializable]
    public class TerrainType
    {
        public string name;
        public float height;
        public Color color;
    }
}
