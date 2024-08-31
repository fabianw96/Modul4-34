using General;
using General.Weapons;
using System;
using System.Collections;
using System.Threading.Tasks.Sources;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

public class SpellHitEffect : MonoBehaviour
{ 
    private Elements currentElement;
    private Elements projectileElement;
    private VisualEffect lingeringEffectAsset;
    private VisualEffect explosionEffectAsset;
    private SkinnedMeshRenderer targetMesh;
    private SpellData spellData;
    private HealthSystem healthSystem;
    private NavMeshAgent navMeshAgent;
    private float originalSpeed;
    private float damage;
    private float effectDuration;
    private bool isDot;
    private bool hasEffectApplied = false;
    private float elapsedTime = 0;

    public void InitSpellHitEffect(SpellData _spellData, HealthSystem _healthSys)
    {
        spellData = _spellData;
        healthSystem = _healthSys;
        targetMesh = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        lingeringEffectAsset = gameObject.GetComponent<VisualEffect>();
        damage = spellData.CalculateDamage(SpellLevelManager.Instance.GetSpellLevel(spellData.Element));
        effectDuration = spellData.EffectDuration;
        isDot = spellData.IsDot;
        projectileElement = spellData.Element;
    }

    private void Start()
    {
        // Check for explosion
        if (currentElement == Elements.Electro && projectileElement == Elements.Fire)
        {
            explosionEffectAsset.visualEffectAsset = spellData.ExplosionEffect;
            explosionEffectAsset.Play();
            TriggerExplosion();
            return;
        }

        projectileElement = spellData.Element;
        lingeringEffectAsset.visualEffectAsset = spellData.LingeringEffectAsset;
        lingeringEffectAsset.SetSkinnedMeshRenderer(Shader.PropertyToID("TargetMesh"), targetMesh);
        lingeringEffectAsset.Play();

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
            if (spellData.OnHitEffect == OnHitEffects.Slow)
            {
                ApplySlowEffect();
            }
            else if (spellData.OnHitEffect == OnHitEffects.Electrified)
            {
                ApplyElectrifiedEffect();
            }
            // Implement other effect types here

            // Remove the effect after duration
            StartCoroutine(RemoveEffectAfterDuration());
        }
        healthSystem.TakeDamage(damage);
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
        navMeshAgent.velocity = Vector3.zero;
    }

    public void TriggerExplosion()
    {
        
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, spellData.ExplosionRadius);
        foreach (var hitCollider in hitColliders)
        {
            healthSystem = hitCollider.GetComponent<HealthSystem>();
            if (healthSystem != null)
            {
                // Apply Damage from the explosion
                healthSystem.TakeDamage(spellData.ExplosionDamage);

                // Check if the object should also catch fire
                SpellHitEffect fireEffect = hitCollider.gameObject.GetComponent<SpellHitEffect>();
                if (fireEffect == null) 
                {
                    fireEffect = hitCollider.gameObject.AddComponent<SpellHitEffect>();
                }

                // Initialize the fire effect on the hit object
                SpellData fireSpellData = Resources.Load<SpellData>("Assets/Sriptable Objects/Spells/FireSpellData");
                if (fireSpellData != null) 
                {
                    fireEffect.InitSpellHitEffect(fireSpellData, healthSystem);
                }
            }
        }
        CleanupEffect();
    }

    private void CleanupEffect()
    {
        // Reset all values affected by the MagicEffect
        if (spellData.OnHitEffect == OnHitEffects.Slow && navMeshAgent != null)
        {
            navMeshAgent.speed = originalSpeed;
        }
        // Stop the effect and remove the component
        if (lingeringEffectAsset != null)
        {
            lingeringEffectAsset.Stop();
            Destroy(lingeringEffectAsset);
        }
        Destroy(this);
    }
}