using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEffect : MonoBehaviour, IMagicEffect
{
    public int damageAmount;

    public void ApplyEffect(GameObject _target)
    {
        // Implement Damage Logic
    }
}

public class BurnEffect : MonoBehaviour, IMagicEffect
{
    public float burnDuration;

    public void ApplyEffect(GameObject _target)
    {
        // Implement Burn Logic
    }
}

public class FreezeEffect : MonoBehaviour, IMagicEffect
{
    public void ApplyEffect(GameObject _target)
    {
        // Implement Freeze Logic
    }
}

public class ElectricEffect : MonoBehaviour, IMagicEffect
{
    public void ApplyEffect(GameObject _target)
    {
        // Implement Electricity Logic
    }
}
