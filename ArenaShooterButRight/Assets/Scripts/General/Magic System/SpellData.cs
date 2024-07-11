using System;
using UnityEngine;
using UnityEngine.VFX;

public enum EffectTypes
{
    None,
    Burn,
    Slow,
    Electrified,
}

public enum SpellTypes
{
    None,
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

    [Header("Spellstats")]
    public float BaseSpeed; // the spells projectile speed
    public float BaseDamage; // spell damage
    public float BaseCost; // Mana cost of the spell
    public float BaseCooldown;
    public int UnlockCost;

    [Header("Spelleffect Stats")]
    public bool IsDot;
    public bool IsSlow;
    public float EffectDuration; // e.g, dot duration, slow duration
    public float DotDamagePerTick;
    public float DotTickRate;
    public float SlowStrength;

    [Header("Spellstats per Level")]
    public float DamagePerLevel;
    public float CostPerLevel;
    public float SpeedPerLevel;
    public float CooldownPerLevel;

    [Header("Explosion Effect Stats")]
    public float ExplosionDamage;
    public float ExplosionRadius;

    public FPSController FPSController;
    public VisualEffectAsset VisualEffectAsset; // visual Asset of the spell
    public VisualEffectAsset VisualLingeringEffectAsset; // visual Asset of the status effect caused by the spell

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
        return BaseCost + (CostPerLevel * (_level - 1));
    }
    public float CalculateCooldown(int _level)
    {
        return BaseCooldown + (CooldownPerLevel * (_level - 1));
    }
}
