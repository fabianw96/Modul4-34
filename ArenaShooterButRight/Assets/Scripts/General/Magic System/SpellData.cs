using General.Player;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.VFX;

public enum EffectTypes
{
    Burn,
    Slow
}

public enum SpellType
{
    Fireball,
    Iceball,
    Electroball
}

[CreateAssetMenu(menuName = "ScriptableObjects/SpellData")]
public class SpellData : ScriptableObject
{
    [Header("General")]
    public EffectTypes effectType; // Status effects
    public SpellType spellType;
    public Sprite spellIcon;
    public float unlockPrice;

    [Header("Spellstats")]
    public float baseSpeed; // the spells projectile speed
    public float baseDamage; // spell damage
    public float baseManaCost; // Mana cost of the spell
    public float baseCooldown;
    public float effectDuration; // e.g, dot duration

    [Header("Spellstats per Level")]
    public float damagePerLevel;
    public float manaCostPerLevel;
    public float speedPerLevel;
    public float cooldownPerLevel;

    public bool isDot; 
    public PlayerController movementSettings; // to apply status effects
    public VisualEffectAsset visualEffectAsset; // visual Asset of the spell

    public float CalculateSpeed(int _level)
    {
        return baseSpeed + (speedPerLevel * (_level - 1));
    }

    public float CalculateDamage(int _level)
    {
        return baseDamage + (damagePerLevel * (_level - 1));
    }

    public float CalculateManaCost(int _level)
    {
        return baseManaCost + (manaCostPerLevel * (_level - 1));
    }
    public float CalculateCooldown(int _level)
    {
        return baseCooldown + (cooldownPerLevel * (_level - 1));
    }
}
