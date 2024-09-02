using General;
using General.Weapons;
using Justin.KI;
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
    private VisualEffect lingeringEffect;
    private VisualEffect explosionEffect;
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
        damage = _spellData.CalculateDamage();
        effectDuration = _spellData.EffectDuration;
        isDot = _spellData.IsDot;
        projectileElement = _spellData.Element;
        targetMesh = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        originalSpeed = GetComponent<EnemyController>().defaultSpeed;

        // Check for explosion
        explosionEffect = gameObject.GetComponent<VisualEffect>();
        if (currentElement == Elements.Electro && projectileElement == Elements.Fire)
        {
            if (explosionEffect != null)

            {
                explosionEffect.visualEffectAsset = spellData.ExplosionEffect;
                explosionEffect.Play();
                TriggerExplosion();
                return;
            }
            else
            {
                Debug.LogError("No VisualEffect component found for the explosion.");
            }
        }
            
        lingeringEffect = gameObject.GetComponent<VisualEffect>();
        if (lingeringEffect != null)
        {
            lingeringEffect.visualEffectAsset = spellData.LingeringEffectAsset;
            lingeringEffect.SetSkinnedMeshRenderer(Shader.PropertyToID("TargetMesh"), targetMesh);
            lingeringEffect.Play();
        }
        else
        {
            Debug.LogError("No VisualEffect component found on the enemy.");
        }

        ApplyEffect();
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
            else if (spellData.OnHitEffect == OnHitEffects.Burn) 
            {
                ApplyDotEffect();
            }
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
        navMeshAgent.speed = 0f;
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
                SpellData fireSpellData = Resources.Load<SpellData>("Assets/SriptableObjects/Spells/FireSpellData");
                if (fireSpellData != null) 
                {
                    fireEffect.InitSpellHitEffect(fireSpellData, healthSystem);
                }
            }
        }
        CleanupEffect();
    }
    private IEnumerator RemoveEffectAfterDuration()
    {
        yield return new WaitForSeconds(effectDuration);
        CleanupEffect();
    }

    private void CleanupEffect()
    {
        if (navMeshAgent.speed != originalSpeed) 
        {
            navMeshAgent.speed = originalSpeed;
        }

        // Stop the effect and set visualEffect to null
        if (lingeringEffect != null)
        {
            lingeringEffect.Stop();
            lingeringEffect.visualEffectAsset = null;
        }
        Destroy(this);
    }
}