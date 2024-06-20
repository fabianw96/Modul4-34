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
    public float speed;
    public SpellType spellType;
    public float damage;
    public bool isDot;
    public EffectTypes effectType;
    public float effectDuration;
    public PlayerController movementSettings;
    public VisualEffect effect;
}
