using General;
using System;
using System.Collections;
using System.Threading.Tasks.Sources;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

public class MagicEffect : MonoBehaviour
{ 
    private EffectTypes currentLingeringEffect;
    private HealthSystem healthSystem;
    private NavMeshAgent navMeshAgent;
    private SpellData spellData;
    private float originalSpeed;
    private float dmg;
    private float effectDuration;
    private bool isDot;
    private bool hasEffectApplied = false;
    private VisualEffect lingeringEffect;
    private Mesh targetMesh;
    [SerializeField] private float elapsedTime = 0;
    
    public void InitEffect(SpellData _spellData, HealthSystem _healthSys, float _damage)
    {
        healthSystem = _healthSys;
        dmg = _damage;
        effectDuration = _spellData.EffectDuration;
        isDot = _spellData.IsDot;
        spellData = _spellData;
        targetMesh = gameObject.GetComponent<MeshFilter>().mesh;

        // Attaches the visual lingering effect to the target
        if (gameObject.GetComponent<VisualEffect>() == null)
        {
            lingeringEffect = gameObject.AddComponent<VisualEffect>();
        }

        if (lingeringEffect != null && spellData.Type == SpellTypes.Fireball && currentLingeringEffect == EffectTypes.Electrified)
        {
            TriggerExplosion();
        }

        currentLingeringEffect = spellData.EffectType;
        lingeringEffect = gameObject.GetComponent<VisualEffect>();
        lingeringEffect.visualEffectAsset = spellData.VisualLingeringEffectAsset;
        lingeringEffect.SetMesh(Shader.PropertyToID("TargetMesh"), targetMesh);
        lingeringEffect.Play();



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
        elapsedTime = 0f;
        while (elapsedTime < effectDuration)
        {
            healthSystem.TakeDamage(spellData.DotDamagePerTick);
            yield return new WaitForSeconds(spellData.DotTickRate);
            elapsedTime += spellData.DotTickRate;
        }
        CleanupEffect();
    }

    private IEnumerator RemoveEffectAfterDuration()
    {
        yield return new WaitForSeconds(effectDuration);
        CleanupEffect();
    }

    private void ApplyEffect()
    {
        if (!hasEffectApplied)
        {
            hasEffectApplied = true;
            if (spellData.EffectType == EffectTypes.Slow)
            {
                ApplySlowEffect();
            }
            else if (spellData.EffectType == EffectTypes.Electrified)
            {
                ApplyElectrifiedEffect();
            }
            // Implement other effect types here
            // Remove the effect after duration
            StartCoroutine(RemoveEffectAfterDuration());
        }
        healthSystem.TakeDamage(dmg);
    }

    private void ApplySlowEffect()
    {
        if (navMeshAgent != null) 
        {
            originalSpeed = navMeshAgent.speed;
            navMeshAgent.speed *= (1 - spellData.SlowStrength);
        }
    }
    private void ApplyElectrifiedEffect()
    {
        // TODO Logic for electrified effect
        // Not really important. Mostly just for the visuals
    }

    public void TriggerExplosion()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, spellData.ExplosionRadius);
        foreach (var hitCollider in hitColliders)
        {
            healthSystem = hitCollider.GetComponent<HealthSystem>();
            if (healthSystem != null)
            {
                healthSystem.TakeDamage(spellData.ExplosionDamage);
            }
        }
        CleanupEffect();
    }

    private void CleanupEffect()
    {
        // Reset all values affected by the Effect
        if (spellData.EffectType == EffectTypes.Slow && navMeshAgent != null)
        {
            navMeshAgent.speed = originalSpeed;
        }
        // Stop the effect and remove the component
        if (lingeringEffect != null)
        {
            lingeringEffect.Stop();
            Destroy(lingeringEffect);
        }
        Destroy(this);
    }
}

