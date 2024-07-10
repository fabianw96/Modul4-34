using General;
using System.Collections;
using UnityEngine;

public class MagicEffect : MonoBehaviour
{
    private HealthSystem healthSystem;
    private float dmg;
    private float effectDuration;
    private bool isDot;
    private bool hasEffectApplied = false;
    private SpellData spellData;
    
    public void InitEffect(SpellData _spellData, HealthSystem _healthSys, float _damage)
    {
        healthSystem = _healthSys;
        dmg = _damage;
        effectDuration = _spellData.EffectDuration;
        isDot = _spellData.IsDot;
        spellData = _spellData;

        if (isDot)
        {
            StartCoroutine(ApplyDotEffect());
        }
        else
        {
            ApplyEffect();
        }
    }

    private IEnumerator ApplyDotEffect()
    {
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
        yield return new WaitForSeconds(effectDuration);
        // Reset any modified properties to their original state
        // e.g., reset movement speed
        Destroy(this); // Remove the effect after duration
    }

    private void ApplyEffect()
    {
        if (!hasEffectApplied)
        {
            hasEffectApplied = true;
            if (spellData.EffectType == EffectTypes.Slow)
            {
                
            }
            // Implement other effect types here
            // Remove the effect after duration
            StartCoroutine(RemoveEffectAfterDuration());
        }
        healthSystem.TakeDamage(dmg);
    }
}

