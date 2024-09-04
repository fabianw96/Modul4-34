using General;
using General.Weapons;
using UnityEngine;
using UnityEngine.VFX;

public class MagicProjectile : BaseProjectile
{
    private SpellData spellData;
    private VisualEffect spellProjectileEffect;
    private SpellHitEffect spellHitEffect;
    private OnHitEffects previousEffect;
    protected override void Start()
    {
        projectileSpeed = spellData.CalculateSpeed();
    }

    public void InitSpellProjectile(SpellData _spellData)
    {
        spellData = _spellData;
        spellProjectileEffect = gameObject.GetComponent<VisualEffect>();
        if (spellProjectileEffect != null) 
        {
            spellProjectileEffect.visualEffectAsset = spellData.SpellEffectAsset;
            spellProjectileEffect.Play();
        }
        else
        {
            Debug.LogError("No VisualEffect component found on the projectile");
        }
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void OnTriggerEnter(Collider other)
    {
        // Check if the object has a HealthSystem component
        HealthSystem targetHealthSystem = other.gameObject.GetComponent<HealthSystem>();

        if (targetHealthSystem == null)
        {
            Destroy(gameObject);
            return;
        }

        // Try to get the existing SpellHitEffect component
        spellHitEffect = other.gameObject.GetComponent<SpellHitEffect>();

        if (spellHitEffect == null)
        {
            // If no SpellHitEffect exists, add it to the target
            spellHitEffect = other.gameObject.AddComponent<SpellHitEffect>();
        }

        previousEffect = other.gameObject.GetComponent<SpellHitEffect>().currentEffect;
        Debug.Log($"previous Effect: {previousEffect}");
        if (spellData.OnHitEffect == OnHitEffects.Burn && previousEffect == OnHitEffects.Electrified)
        {
            spellHitEffect.TriggerExplosion(spellData);
            Destroy(gameObject);
            return;
        }

        spellHitEffect.InitSpellHitEffect(spellData, targetHealthSystem);

        // Destroy Projectile after applying effect
        Destroy(gameObject); 
    }
}