using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public sealed class SpellDatabaseEditor : EditorWindow
{
    private Sprite defaultSpellIcon;
    private static List<SpellData> spellDatabase = new List<SpellData>();

    // Spell List View
    private VisualElement spellTab;
    private static VisualTreeAsset spellRowTemplate;
    private ListView spellListView;
    private float spellHeight = 50;

    // Spell Details Panel
    private ScrollView spellDetails;
    private VisualElement largeDisplayIcon;
    private SpellData activeSpell;

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
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Justin/Editor/SpellDatabaseEditor.uxml");
        VisualElement rootFromUXML = visualTree.Instantiate();
        rootVisualElement.Add(rootFromUXML);

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/Justin/Editor/SpellDatabaseEditor.uss");
        rootVisualElement.styleSheets.Add(styleSheet);

        spellRowTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Justin/Editor/SpellRowTemplate.uxml");
        defaultSpellIcon = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Sprites/UnknownIcon.png", typeof(Sprite));

        LoadAllItems();

        // SpellTab Init
        spellTab = rootVisualElement.Q<VisualElement>("SpellTab");
        GenerateListView();


        // SpellDetails Init
        spellDetails = rootVisualElement.Q<ScrollView>("SpellDetails");
        spellDetails.style.visibility = Visibility.Hidden;
        largeDisplayIcon = spellDetails.Q<VisualElement>("Icon");

        // Add Spell Init
        rootVisualElement.Q<Button>("AddSpellButton").clicked += AddSpell_OnClick;
        rootVisualElement.Q<Button>("DeleteSpellButton").clicked += DeleteSpell_OnClick;

    }

    private void DeleteSpell_OnClick()
    {
        // Get the path of the file and delete it through AssetDatabase
        string path = AssetDatabase.GetAssetPath(activeSpell);
        AssetDatabase.DeleteAsset(path);

        // remove the reference from the list and refresh the ListView
        spellDatabase.Remove(activeSpell);
        spellListView.Rebuild();
    }

    private void AddSpell_OnClick()
    {
        // Create a new Scriptable Object and set default parameters
        SpellData newSpell = new SpellData();
        newSpell.SpellName = $"New Spell";
        newSpell.SpellIcon = defaultSpellIcon;

        // Create the asset, using the unique ID for the name
        AssetDatabase.CreateAsset(newSpell, $"Assets/ScriptableObjects/Spells/{newSpell.ID}.asset");

        // Add it to the item list
        spellDatabase.Add(newSpell);

        // Refresh the ListView so everything is redrawn again
        spellListView.Rebuild();
        spellListView.style.height = spellDatabase.Count * spellHeight;
    }

    private void GenerateListView()
    {
        Func<VisualElement> makeSpell = () => spellRowTemplate.CloneTree();

        Action<VisualElement, int> bindItem = (e, i) =>
        {
            e.Q<VisualElement>("Icon").style.backgroundImage =
                spellDatabase[i] == null ? defaultSpellIcon.texture :
                spellDatabase[i].SpellIcon.texture;
            e.Q<Label>("Name").text = spellDatabase[i].name;
        };

        spellListView = new ListView(spellDatabase, 50, makeSpell, bindItem);
        spellListView.selectionType = SelectionType.Single;
        spellListView.style.height = spellDatabase.Count * spellHeight;
        spellTab.Add(spellListView);

        spellListView.selectionChanged += ListView_onSelectionChanged;
    }

    private void ListView_onSelectionChanged(IEnumerable<object> selectedSpells)
    {
        activeSpell = (SpellData)selectedSpells.First();

        SerializedObject so = new SerializedObject(activeSpell);
        spellDetails.Bind(so);

        if (activeSpell.SpellIcon != null)
        {
            largeDisplayIcon.style.backgroundImage = activeSpell.SpellIcon.texture;
        }
        spellDetails.style.visibility = Visibility.Visible;
    }

    private void LoadAllItems()
    {
        spellDatabase.Clear();
        string[] allPaths = Directory.GetFiles("Assets/ScriptableObjects/Spells", "*.asset",SearchOption.AllDirectories);

        foreach (string path in allPaths) 
        {
            string cleanedPath = path.Replace("\\", "/");
            spellDatabase.Add((SpellData)AssetDatabase.LoadAssetAtPath(cleanedPath,typeof(SpellData)));
        }
    }
}
