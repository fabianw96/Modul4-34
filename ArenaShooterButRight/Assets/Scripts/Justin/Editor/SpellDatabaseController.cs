// Controller: SpellDatabaseController.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class SpellDatabaseController
{
    private List<SpellData> spellDatabase;
    private SpellDatabaseView view;
    private SpellData activeSpell;
    private float spellHeight = 50f;

    public SpellDatabaseController(SpellDatabaseView view)
    {
        this.view = view;
        spellDatabase = new List<SpellData>();
        LoadAllItems();
        InitializeView();
    }

    private void InitializeView()
    {
        view.AddSpellButton.clicked += AddSpell_OnClick;
        view.DeleteSpellButton.clicked += DeleteSpell_OnClick;

        view.SpellNameField.RegisterValueChangedCallback(evt =>
        {
            activeSpell.Name = evt.newValue;
            view.SpellListView.Rebuild();
        });

        view.IconPickerField.RegisterValueChangedCallback(evt =>
        {
            Sprite newSprite = evt.newValue as Sprite;
            activeSpell.Icon = newSprite == null ? view.DefaultSpellIcon : newSprite;
            view.LargeDisplayIcon.style.backgroundImage = newSprite == null ? view.DefaultSpellIcon.texture : newSprite.texture;
            view.SpellListView.Rebuild();
        });

        GenerateListView();
    }

    private void LoadAllItems()
    {
        spellDatabase.Clear();
        string[] allPaths = Directory.GetFiles("Assets/ScriptableObjects/Spells", "*.asset", SearchOption.AllDirectories);

        foreach (string path in allPaths)
        {
            string cleanedPath = path.Replace("\\", "/");
            spellDatabase.Add((SpellData)AssetDatabase.LoadAssetAtPath(cleanedPath, typeof(SpellData)));
        }
    }

    public void GenerateListView()
    {
        Func<VisualElement> makeSpell = () => view.MakeSpell();

        Action<VisualElement, int> bindItem = (e, i) =>
        {
            e.Q<VisualElement>("Icon").style.backgroundImage =
                spellDatabase[i] == null ? view.DefaultSpellIcon.texture :
                spellDatabase[i].Icon.texture;
            e.Q<Label>("Name").text = spellDatabase[i].Name;
        };

        view.SpellListView = new ListView(spellDatabase, 50, makeSpell, bindItem);
        view.SpellListView.selectionType = SelectionType.Single;
        view.SpellListView.style.height = spellDatabase.Count * spellHeight;
        view.SpellTab.Add(view.SpellListView);

        view.SpellListView.selectionChanged += ListView_onSelectionChanged;

        Debug.Log("GenerateListView completed successfully.");
    }

    private void BindItem(VisualElement e, int i)
    {
        e.Q<VisualElement>("Icon").style.backgroundImage =
            spellDatabase[i] == null ? view.DefaultSpellIcon.texture :
            spellDatabase[i].Icon.texture;
        e.Q<Label>("Name").text = spellDatabase[i].Name;
    }

    private void ListView_onSelectionChanged(IEnumerable<object> selectedSpells)
    {
        activeSpell = (SpellData)selectedSpells.First();
        SerializedObject so = new SerializedObject(activeSpell);
        view.SpellDetails.Bind(so);

        if (activeSpell.Icon != null)
        {
            view.LargeDisplayIcon.style.backgroundImage = activeSpell.Icon.texture;
        }
        view.SpellDetails.style.visibility = Visibility.Visible;
    }

    private void AddSpell_OnClick()
    {
        SpellData newSpell = ScriptableObject.CreateInstance<SpellData>();
        newSpell.Name = $"New Spell";
        newSpell.Icon = view.DefaultSpellIcon;

        AssetDatabase.CreateAsset(newSpell, $"Assets/ScriptableObjects/Spells/{newSpell.ID}.asset");

        spellDatabase.Add(newSpell);
        view.SpellListView.Rebuild();
        view.SpellListView.style.height = spellDatabase.Count * spellHeight;
    }

    private void DeleteSpell_OnClick()
    {
        string path = AssetDatabase.GetAssetPath(activeSpell);
        AssetDatabase.DeleteAsset(path);

        spellDatabase.Remove(activeSpell);
        view.SpellListView.Rebuild();
        view.SpellDetails.style.visibility = Visibility.Hidden;
    }
}
