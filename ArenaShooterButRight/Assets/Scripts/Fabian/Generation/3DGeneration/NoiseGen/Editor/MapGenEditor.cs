using UnityEditor;
using UnityEngine;

namespace Fabian.Generation._3DGeneration.NoiseGen.Editor
{
    [CustomEditor(typeof(MapGeneration))]
    public class MapGenEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            MapGeneration mapGeneration = (MapGeneration)this.target;

            if (DrawDefaultInspector())
            {
                if (mapGeneration.autoUpdate)
                {
                    mapGeneration.DrawMapInEditor();
                }
            }

            if (GUILayout.Button("Generate"))
            {
                mapGeneration.DrawMapInEditor();
            }
        }
    }
}
