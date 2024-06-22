using General.Player;
using UnityEngine;
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
    public EffectTypes effectType; // Status effects
    public SpellType spellType; 
    public float speed; // the spells projectile speed
    public float damage; // spell damage
    public bool isDot; 
    public float effectDuration; // e.g, dot duration
    public PlayerController movementSettings; // to apply status effects
    public VisualEffectAsset visualEffectAsset; // visual Asset of the spell
    public float manaCost; // Mana cost of the spell
}
