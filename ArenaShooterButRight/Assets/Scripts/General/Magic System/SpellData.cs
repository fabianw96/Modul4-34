using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.VFX;

public enum OnHitEffects
{
    None,
    Burn,
    Slow,
    Electrified,
}

public enum Elements
{
    None,
    Fire,
    Ice,
    Electro
}

[CreateAssetMenu(menuName = "ScriptableObjects/SpellData")]
public class SpellData : ScriptableObject
{
    [Header("General")]
    public string ID = Guid.NewGuid().ToString().ToUpper();
    public OnHitEffects OnHitEffect; // Status effects
    public Elements Element;
    public string Name;
    public Sprite Icon;
    public int Level = 1; // New property for spell Level

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
    public VisualEffectAsset ExplosionEffect;
    public float ExplosionDamage;
    public float ExplosionRadius;

    public VisualEffectAsset SpellEffectAsset; // visual Asset of the spell as projectile
    public VisualEffectAsset LingeringEffectAsset; // visual Asset of the effect remaining on the enemy

    public float CalculateSpeed()
    {
        return BaseSpeed + (SpeedPerLevel * (Level - 1));
    }

    public float CalculateDamage()
    {
        return BaseDamage + (DamagePerLevel * (Level - 1));
    }

    public float CalculateManaCost()
    {
        return BaseCost + (CostPerLevel * (Level - 1));
    }
    public float CalculateCooldown()
    {
        return BaseCooldown + (CooldownPerLevel * (Level - 1));
    }
}
