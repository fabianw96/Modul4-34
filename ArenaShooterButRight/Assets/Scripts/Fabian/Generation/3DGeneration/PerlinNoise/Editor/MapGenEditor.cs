using Fabian.Generation._3DGeneration.MeshGen;
using UnityEngine;
using UnityEditor;

namespace Fabian.Generation._3DGeneration.PerlinNoise.Editor
{
    [CustomEditor(typeof(MapGeneration))]
    public class MapGenEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            MapGeneration mapGeneration = (MapGeneration)target;

            if (DrawDefaultInspector())
            {
                if (mapGeneration.autoUpdate)
                {
                    mapGeneration.GenerateMap();
                }
            }

            if (GUILayout.Button("Generate"))
            {
                mapGeneration.GenerateMap();
            }
        }
    }
}
