using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpellLevelManager : MonoBehaviour
{

    public static SpellLevelManager Instance { get; private set; }
    [SerializeField] private List<SpellLevel> spellLevels;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    [System.Serializable]
    public class SpellLevel
    {
        public Elements spellType;
        public int level = 1;
    }

    public int GetSpellLevel(Elements _spellType)
    {
        foreach (var spellLevel in spellLevels)
        {
            if (spellLevel.spellType == _spellType)
            {
                return spellLevel.level;
            }
        }
        return 1;
    }

    public void LevelUpSpell(Elements _spellType)
    {
        foreach (var spellLevel in spellLevels)
        {
            if (spellLevel.spellType == _spellType)
            {
                spellLevel.level++;
                return;
            }
        }
    }
}
