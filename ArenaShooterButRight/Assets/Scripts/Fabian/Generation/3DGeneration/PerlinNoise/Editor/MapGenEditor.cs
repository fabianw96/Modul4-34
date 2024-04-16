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
            MeshGeneration meshGen = FindObjectOfType<MeshGeneration>();

            if (DrawDefaultInspector())
            {
                if (mapGeneration.autoUpdate)
                {
                    mapGeneration.GenerateMap();
                    meshGen.Start();
                }
            }

            if (GUILayout.Button("Generate"))
            {
                mapGeneration.GenerateMap();
                meshGen.Start();
            }
        }
    }
}
