using UnityEngine;
using UnityEditor;
using Justin.ProcGen;

[CustomEditor(typeof(ProcGenManager))]
public class MapGeneratorEditor : Editor
{
    // Override the default Inspector GUI for the ProcGenManager component
    public override void OnInspectorGUI()
    {
        ProcGenManager mapGen = (ProcGenManager)target;

        if (DrawDefaultInspector())
        {
            if(mapGen.autoUpdate)
            {
                mapGen.GenerateMap();
            }
        }

        if (GUILayout.Button("Generate"))
        {
            mapGen.GenerateMap();
        }
    }
}
