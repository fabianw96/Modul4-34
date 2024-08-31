using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Contains all the logic that decides the UIs behavior. 
/// It handles events like adding, deleting, and selecting spells, as well as updating the UI when changes to values occur. 
/// It loads the existing spell data and initializes the ListView dynamically, ensuring that the UI remains responsive to changes.
/// </summary>
public class SpellDatabaseController
{
    private List<SpellData> spellDatabase;  
    private SpellDatabaseView view;         
    private SpellData activeSpell;          
    private float spellHeight = 50f; // Height of each spell row in the ListView

    public SpellDatabaseController(SpellDatabaseView view)
    {
        this.view = view;
        spellDatabase = new List<SpellData>();
        LoadAllItems();
        InitializeView();
    }

    // Loads all SpellData assets from the designated folder into the spell database
    private void LoadAllItems()
    {
        spellDatabase.Clear();
        string[] allPaths = Directory.GetFiles("Assets/ScriptableObjects/Spells", "*.asset", SearchOption.AllDirectories);

        foreach (string path in allPaths)
        {
            string cleanedPath = path.Replace("\\", "/"); // Ensure consistent path formatting
            spellDatabase.Add((SpellData)AssetDatabase.LoadAssetAtPath(cleanedPath, typeof(SpellData)));
        }
    }

    // Initializes the view by setting up UI event handlers and generating the ListView
    private void InitializeView()
    {
        view.AddSpellButton.clicked += AddSpell_OnClick;
        view.DeleteSpellButton.clicked += DeleteSpell_OnClick;

        // Register callbacks for when the spell name or icon changes in the details view and rebuild to show the changes
        view.SpellNameField.RegisterValueChangedCallback(evt =>
        {
            activeSpell.Name = evt.newValue; 
            view.SpellListView.Rebuild();    
        });

        view.IconPickerField.RegisterValueChangedCallback(evt =>
        {
            Sprite newSprite = evt.newValue as Sprite;
            activeSpell.Icon = newSprite == null ? view.DefaultSpellIcon : newSprite; // Use default icon if none selected
            view.LargeDisplayIcon.style.backgroundImage = newSprite == null ? view.DefaultSpellIcon.texture : newSprite.texture; // The same for the bigger icon in the details panel
            view.SpellListView.Rebuild();
        });

        GenerateListView();

    }

    // Generates the ListView to display the spells in the database
    public void GenerateListView()
    {
        // Create a new viusal element for each spell in the ListView
        Func<VisualElement> makeSpell = () => view.MakeSpell();

        // Bind spell data to each visual element in the ListView
        Action<VisualElement, int> bindItem = (e, i) =>
        {
            e.Q<VisualElement>("Icon").style.backgroundImage =                      // Set the spell's icon or a default icon
                spellDatabase[i] == null ? view.DefaultSpellIcon.texture :
                spellDatabase[i].Icon.texture;                                      
            e.Q<Label>("Name").text = spellDatabase[i].Name;                        // Set the spell's name
        };

        view.SpellListView = new ListView(spellDatabase, 50, makeSpell, bindItem);  // Initialize the ListView with the spell data and the make/bind functions
        view.SpellListView.selectionType = SelectionType.Single;                    // Only allow one spell to be selected at a time
        view.SpellListView.style.height = spellDatabase.Count * spellHeight;        // Set the height of the ListView
        view.SpellTab.Add(view.SpellListView);                                      // Add the ListView to the view

        view.SpellListView.selectionChanged += ListView_OnSelectionChanged;

        // Initially hide the spell details view until a spell is selected
        view.SpellDetails.style.visibility = Visibility.Hidden;
        Debug.Log("GenerateListView completed successfully.");
    }

    // Handles the selection change event in the ListView, binds the selected spell's data to the details view and makes the details view visible. 
    private void ListView_OnSelectionChanged(IEnumerable<object> selectedSpells)
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

    // Event handler for adding a new spell to the database, saving the asset in a designated folder and adjusting the Listview height for the new spell
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

    // Event handler for deleting the currently selected spell from the database, rebuild to show the changes and hide details view since no spell is selected
    private void DeleteSpell_OnClick()
    {
        string path = AssetDatabase.GetAssetPath(activeSpell);      
        AssetDatabase.DeleteAsset(path);
        spellDatabase.Remove(activeSpell);                          
        view.SpellListView.Rebuild();                               
        view.SpellDetails.style.visibility = Visibility.Hidden;     
    }
}
