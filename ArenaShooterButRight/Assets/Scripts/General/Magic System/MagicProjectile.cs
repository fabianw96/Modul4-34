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
        projectileSpeed = spellData.CalculateSpeed(SpellLevelManager.Instance.GetSpellLevel(spellData.Element));
        //Debug.Log($"The Spells Visual Effect: {spellData.SpellEffectAsset}");
        //Debug.Log($"The currently saved visual Effect: {spellProjectileEffect.visualEffectAsset}");
        spellProjectileEffect = gameObject.GetComponent<VisualEffect>();
        spellProjectileEffect.visualEffectAsset = spellData.SpellEffectAsset;
        spellProjectileEffect.Play();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<HealthSystem>() != null)
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
        Destroy(gameObject); // Destroy Projectile after applying effect
    }
}