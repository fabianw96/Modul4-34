using General;
using General.Weapons;
using UnityEngine;
using UnityEngine.VFX;

public class MagicProjectile : BaseProjectile
{
    private SpellData spellData;
    private VisualEffect spellProjectileEffect;
    private SpellHitEffect spellHitEffect;
    protected override void Start()
    {
        
    }

    public void InitSpellProjectile(SpellData _spellData)
    {
        spellData = _spellData;
        projectileSpeed = spellData.CalculateSpeed();

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

        // Apply SpellHitEffect Component or if already on Target, check for combos in SpellHitEffect itself
        if (other.gameObject.GetComponent<SpellHitEffect>() == null)
        {
            spellHitEffect = other.gameObject.AddComponent<SpellHitEffect>();
        }

        spellHitEffect = other.gameObject.GetComponent<SpellHitEffect>();
        spellHitEffect.InitSpellHitEffect(spellData, other.gameObject.GetComponent<HealthSystem>());

        // Destroy Projectile after applying effect
        Destroy(gameObject); 
    }
}