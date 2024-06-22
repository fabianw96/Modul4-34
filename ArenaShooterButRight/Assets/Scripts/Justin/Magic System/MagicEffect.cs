using System;
using General;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicEffect : MonoBehaviour
{
    private HealthSystem healthSystem;
    private float dmg;
    private float effectDuration;
    private bool isDot;
    private bool hasEffectApplied = false;
    private SpellData spellData;
    
    public void InitEffect(SpellData _spellData, HealthSystem _healthSys)
    {
        Debug.Log("In InitEffect Method");
        healthSystem = _healthSys;
        dmg = _spellData.damage;
        effectDuration = _spellData.effectDuration;
        isDot = _spellData.isDot;
        spellData = _spellData;

        if (isDot)
        {
            StartCoroutine(ApplyDotEffect());
        }
        else
        {
            ApplyEffect();
        }

        //TODO: isdot abfragen. Wenn ja, applyeffect, wenn nein, dealdmg.
        // should be done

        if (spellData.isDot)
        {
            ApplyEffect();
            // StartCoroutine(DotDamage());
        }
        healthSystem.TakeDamage(spellData.damage);
        //TODO: delete self when damage has been taken.
        // Should be done with ApplyDotEffect and RemoveEffectAfterDuration Methods
    }

    private IEnumerator ApplyDotEffect()
    {
        Debug.Log("In ApplyDotEffect Method");
        float elapsedTime = 0f;
        while (elapsedTime < effectDuration)
        {
            healthSystem.TakeDamage(dmg);
            yield return new WaitForSeconds(1f); // Damage applied every second
            elapsedTime += 1f;
        }
        Destroy(this); // Remove the effect after duration
    }

    private IEnumerator RemoveEffectAfterDuration()
    {
        Debug.Log("In RemoveEffectAfterDuration Method");
        yield return new WaitForSeconds(effectDuration);
        // Reset any modified properties to their original state
        // e.g., reset movement speed
        Destroy(this); // Remove the effect after duration
    }

    private void ApplyEffect()
    {
        Debug.Log("In ApplyEffect Method");
        if (!hasEffectApplied)
        {
            hasEffectApplied = true;
            if (spellData.effectType == EffectTypes.Slow)
            {
                // Implement slow effect logic here
                // e.g., reduce movement speed
            }
            // Implement other effect types here

            // Remove the effect after duration
            StartCoroutine(RemoveEffectAfterDuration());
        }
        healthSystem.TakeDamage(dmg);
    }
}

