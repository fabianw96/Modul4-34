using UnityEngine;
using UnityEditor;
using Justin.ProcGen;

[CustomEditor(typeof(ProcGenManager))]
public class MapGeneratorEditor : Editor
{
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
