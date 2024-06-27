using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellLevelManager : MonoBehaviour
{
    [SerializeField] private List<SpellLevel> spellLevels;

    [System.Serializable]
    public class SpellLevel
    {
        public SpellType spellType;
        public int level = 1;
    }

    public int GetSpellLevel(SpellType _spellType)
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

    public void LevelUpSpell(SpellType _spellType)
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
