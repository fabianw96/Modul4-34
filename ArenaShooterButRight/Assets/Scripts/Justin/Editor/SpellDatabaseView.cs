// View: SpellDatabaseView.cs
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class SpellDatabaseView
{
    public VisualElement Root { get; private set; }
    public VisualElement SpellTab { get; private set; }
    public ListView SpellListView { get; set; }
    public ScrollView SpellDetails { get; private set; }
    public VisualElement LargeDisplayIcon { get; private set; }
    public Button AddSpellButton { get; private set; }
    public Button DeleteSpellButton { get; private set; }
    public TextField SpellNameField { get; private set; }
    public ObjectField IconPickerField { get; private set; }

    private VisualTreeAsset spellRowTemplate;
    private Sprite defaultSpellIcon;

    public SpellDatabaseView(VisualElement root, string uxmlPath, string ussPath, string rowTemplatePath, string defaultIconPath)
    {
        Root = root;

        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
        if (visualTree == null)
        {
            Debug.LogError($"Failed to load UXML from path: {uxmlPath}");
            return;
        }
        Root.Add(visualTree.CloneTree());

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(ussPath);
        if (styleSheet == null)
        {
            Debug.LogError($"Failed to load USS from path: {ussPath}");
            return;
        }
        Root.styleSheets.Add(styleSheet);

        spellRowTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(rowTemplatePath);
        if (spellRowTemplate == null)
        {
            Debug.LogError($"Failed to load row template from path: {rowTemplatePath}");
            return;
        }

        defaultSpellIcon = AssetDatabase.LoadAssetAtPath<Sprite>(defaultIconPath);
        if (defaultSpellIcon == null)
        {
            Debug.LogError($"Failed to load default icon from path: {defaultIconPath}");
            return;
        }

        SpellTab = Root.Q<VisualElement>("SpellTab");
        if (SpellTab == null)
        {
            Debug.LogError("SpellTab is not found in UXML.");
            return;
        }

        SpellDetails = Root.Q<ScrollView>("SpellDetails");
        if (SpellDetails == null)
        {
            Debug.LogError("SpellDetails is not found in UXML.");
            return;
        }

        LargeDisplayIcon = SpellDetails.Q<VisualElement>("Icon");
        if (LargeDisplayIcon == null)
        {
            Debug.LogError("LargeDisplayIcon is not found in SpellDetails.");
            return;
        }

        AddSpellButton = Root.Q<Button>("AddSpellButton");
        if (AddSpellButton == null)
        {
            Debug.LogError("AddSpellButton is not found in UXML.");
            return;
        }

        DeleteSpellButton = Root.Q<Button>("DeleteSpellButton");
        if (DeleteSpellButton == null)
        {
            Debug.LogError("DeleteSpellButton is not found in UXML.");
            return;
        }

        SpellNameField = SpellDetails.Q<TextField>("SpellName");
        if (SpellNameField == null)
        {
            Debug.LogError("SpellNameField is not found in SpellDetails.");
            return;
        }

        IconPickerField = SpellDetails.Q<ObjectField>("IconPicker");
        if (IconPickerField == null)
        {
            Debug.LogError("IconPickerField is not found in SpellDetails.");
            return;
        }
    }

    public VisualElement MakeSpell()
    {
        return spellRowTemplate.CloneTree();
    }

    public Sprite DefaultSpellIcon => defaultSpellIcon;
}
