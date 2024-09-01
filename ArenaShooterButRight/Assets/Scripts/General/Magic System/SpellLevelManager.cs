using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpellLevelManager : MonoBehaviour
{

    public static SpellLevelManager Instance { get; private set; }

    private Dictionary<string, int> spellLevels = new Dictionary<string, int>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        InitializeSpellLevels();
    }

    // Initialize the levels of all spells
    private void InitializeSpellLevels()
    {
        SpellData[] allSpells = Resources.LoadAll<SpellData>("Assets/ScriptableObjects/Spells");
        foreach (var spell in allSpells)
        {
            if (!spellLevels.ContainsKey(spell.ID))
            {
                spellLevels.Add(spell.ID, spell.Level);
            }
        }
    }

    public int GetSpellLevel(SpellData spell)
    {
        if (spellLevels.TryGetValue(spell.ID, out int level))
        {
            return level;
        }
        return 1; // Default level if not found
    }

    public void SetSpellLevel(SpellData spell, int level)
    {
        if (spellLevels.ContainsKey(spell.ID))
        {
            spellLevels[spell.ID] = level;
            spell.Level = level; // Sync the level with the spell data
        }
        else
        {
            spellLevels.Add(spell.ID, level);
            spell.Level = level;
        }
    }

    public void LevelUpSpell(SpellData spell)
    {
        if (spellLevels.ContainsKey(spell.ID))
        {
            spellLevels[spell.ID]++;
            spell.Level = spellLevels[spell.ID]; // Sync the level with the spell data
        }
    }

}
