using UnityEngine;

namespace Fabian.Generation.ScriptableObjects
{
    [CreateAssetMenu(menuName = "ScriptableObjects/FabianGen/NoiseWave")]
    public class NoiseWave : ScriptableObject
    {
        public float seed;
        public float frequency;
        public float amplitude;
    }
}
