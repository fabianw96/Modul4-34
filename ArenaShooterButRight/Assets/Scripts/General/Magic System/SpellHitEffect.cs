using General;
using Justin.KI;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

public class SpellHitEffect : MonoBehaviour
{
    public OnHitEffects currentEffect;
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
    private float elapsedTime = 0;

    public void InitSpellHitEffect(SpellData _spellData, HealthSystem _healthSys)
    {
        currentEffect = _spellData.OnHitEffect;
        spellData = _spellData;
        healthSystem = _healthSys;
        damage = _spellData.CalculateDamage();
        effectDuration = _spellData.EffectDuration;
        isDot = _spellData.IsDot;
        targetMesh = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        navMeshAgent = gameObject.GetComponent<NavMeshAgent>();

        // Stop any ongoing effects and start the new ones
        StopAllCoroutines(); // Stop any existing coroutines to prevent conflicts
        ApplyEffect();
    }

    private void Start()
    {
        // The original speed is stored for slow effects
        originalSpeed = GetComponent<EnemyController>().defaultSpeed;
    }

    private void ApplyEffect()
    {
        // Apply the visual effects first
        ApplyVisualEffects();

        // Apply the appropriate effect based on the spell's properties
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
            StartCoroutine(ApplyBurnEffect());
        }
        healthSystem.TakeDamage(damage);
        StartCoroutine(RemoveEffectAfterDuration());
    }

    private void ApplyVisualEffects()
    {
        // Handle the lingering visual effect
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
    }

    private IEnumerator ApplyBurnEffect()
    {
        elapsedTime = 0f;
        while (elapsedTime <= effectDuration)
        {
            healthSystem.TakeDamage(spellData.DotDamagePerTick);
            yield return new WaitForSeconds(spellData.DotTickRate);
            elapsedTime += spellData.DotTickRate;
        }
        CleanupEffect();
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

    public void TriggerExplosion(SpellData _spellData)
    {
        explosionEffect = gameObject.GetComponent<VisualEffect>();
        if (explosionEffect != null)
        {
            explosionEffect.visualEffectAsset = _spellData.ExplosionEffect;
            explosionEffect.transform.localPosition += new Vector3(0, 5f, 0);
            explosionEffect.Play();
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, spellData.ExplosionRadius);
        foreach (var hitCollider in hitColliders)
        {
            HealthSystem targetHealth = hitCollider.GetComponent<HealthSystem>();
            if (targetHealth != null)
            {
                // Apply Damage from the explosion
                targetHealth.TakeDamage(spellData.ExplosionDamage);

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
                    fireEffect.InitSpellHitEffect(fireSpellData, targetHealth);
                }
            }
        }
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position,spellData.ExplosionRadius);
    }
}