// Editor: SpellDatabaseEditor.cs
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public sealed class SpellDatabaseEditor : EditorWindow
{
    [MenuItem("Tools/Spell Database")]
    public static void Init()
    {
        SpellDatabaseEditor wnd = GetWindow<SpellDatabaseEditor>();
        wnd.titleContent = new GUIContent("Spell Database");

        Vector2 size = new Vector2(1000, 500);
        wnd.minSize = size;
        wnd.maxSize = size;
    }

    public void CreateGUI()
    {
        var view = new SpellDatabaseView(
            rootVisualElement,
            "Assets/Scripts/Justin/Editor/SpellDatabaseEditor.uxml",
            "Assets/Scripts/Justin/Editor/SpellDatabaseEditor.uss",
            "Assets/Scripts/Justin/Editor/SpellRowTemplate.uxml",
            "Assets/Sprites/UnknownIcon.png"
        );

        if (view == null)
        {
            Debug.LogError("Failed to initialize SpellDatabaseView.");
            return;
        }

        var controller = new SpellDatabaseController(view);
        if (controller == null)
        {
            Debug.LogError("Failed to initialize SpellDatabaseController.");
            return;
        }
    }
}
