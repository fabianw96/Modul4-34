// Editor: SpellDatabaseEditor.cs
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// This is the main editor window class, which sets up the MVC components.
/// It initializes the view and controller, making sure everything is correctly connected when the editor window is opened.
/// </summary>
public sealed class SpellDatabaseEditor : EditorWindow
{
    // Editor tool can be found in the Tools menu in unity
    [MenuItem("Tools/Spell Database")]
    public static void Init()
    {
        SpellDatabaseEditor wnd = GetWindow<SpellDatabaseEditor>();
        wnd.titleContent = new GUIContent("Spell Database");

        // Sets a fixed window size
        Vector2 size = new Vector2(1000, 500);
        wnd.minSize = size;
        wnd.maxSize = size;
    }

    public void CreateGUI()
    {
        // Initialize the view by passing the root visual element and the paths to UI assets
        var view = new SpellDatabaseView(
            rootVisualElement,
            "Assets/Scripts/Justin/Editor/Engine Tool/SpellDatabaseEditor.uxml",
            "Assets/Scripts/Justin/Editor/Engine Tool/SpellDatabaseEditor.uss",
            "Assets/Scripts/Justin/Editor/Engine Tool/SpellRowTemplate.uxml",
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
