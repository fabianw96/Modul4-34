using General.Player;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.VFX;

public enum EffectTypes
{
    Burn,
    Slow
}

public enum SpellTypes
{
    Fireball,
    Iceball,
    Electroball
}

[CreateAssetMenu(menuName = "ScriptableObjects/SpellData")]
public class SpellData : ScriptableObject
{
    [Header("General")]
    public string ID = Guid.NewGuid().ToString().ToUpper();
    public EffectTypes EffectType; // Status effects
    public SpellTypes Type;
    public string Name;
    public Sprite Icon;
    public float UnlockPrice;

    [Header("Spellstats")]
    public float BaseSpeed; // the spells projectile speed
    public float BaseDamage; // spell damage
    public float BaseManaCost; // Mana cost of the spell
    public float BaseCooldown;
    public bool IsDot;
    public float EffectDuration; // e.g, dot duration

    [Header("Spellstats per Level")]
    public float DamagePerLevel;
    public float ManaCostPerLevel;
    public float SpeedPerLevel;
    public float CooldownPerLevel;

    
    public PlayerController MovementSettings; // to apply status effects
    public VisualEffectAsset VisualEffectAsset; // visual Asset of the spell

    public float CalculateSpeed(int _level)
    {
        return BaseSpeed + (SpeedPerLevel * (_level - 1));
    }

    public float CalculateDamage(int _level)
    {
        return BaseDamage + (DamagePerLevel * (_level - 1));
    }

    public float CalculateManaCost(int _level)
    {
        return BaseManaCost + (ManaCostPerLevel * (_level - 1));
    }
    public float CalculateCooldown(int _level)
    {
        return BaseCooldown + (CooldownPerLevel * (_level - 1));
    }
}
