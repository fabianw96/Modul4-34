using General;
using UnityEngine;
using UnityEngine.VFX;

public class Projectile : MonoBehaviour
{
    private SpellData spellData;
    private MagicEffect magicEffect;
    private Vector3 direction;
    private float speed;
    private float damage;
    [SerializeField] private Mesh mesh;

    public void Launch(SpellData _spellData, Vector3 _launchDirection, int _level)
    {
        spellData = _spellData;
        direction = _launchDirection.normalized;
        speed = _spellData.CalculateSpeed(_level);
        damage = _spellData.CalculateDamage(_level);

        // Disable gravity
        GetComponent<Rigidbody>().useGravity = false;

        if (spellData.VisualEffectAsset != null)
        {
            VisualEffect visualEffect = gameObject.AddComponent<VisualEffect>();
            visualEffect.visualEffectAsset = _spellData.VisualEffectAsset;
            visualEffect.Play();
        }
    }

    private void Update()
    {
        transform.position += direction * (speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<HealthSystem>() != null)
        {
            MagicEffect lingeringEffect = other.gameObject.GetComponent<MagicEffect>();

            if(lingeringEffect != null && spellData.Type == SpellTypes.Fireball && lingeringEffect.GetSpellData().EffectType == EffectTypes.Electrified)
            {
                lingeringEffect.TriggerExplosion();
            }
            else if (magicEffect == null)
            {
                magicEffect = other.gameObject.AddComponent<MagicEffect>();
            }
            magicEffect.InitEffect(spellData, other.gameObject.GetComponent<HealthSystem>(), damage, mesh);
            Destroy(gameObject); // Destroy Projectile upon impact
        }
    }
}
