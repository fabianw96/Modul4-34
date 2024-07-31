using General;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class Projectile : MonoBehaviour
{
    private SpellData spellData;
    private MagicEffect magicEffect;
    private Vector3 direction;
    private float speed;
    private float damage;

    public void Launch(SpellData _spellData, Vector3 _launchDirection)
    {
        spellData = _spellData;
        direction = _launchDirection.normalized;
        speed = _spellData.CalculateSpeed(SpellLevelManager.Instance.GetSpellLevel(_spellData.Type));
        damage = _spellData.CalculateDamage(SpellLevelManager.Instance.GetSpellLevel(_spellData.Type));

        // Disable gravity
        GetComponent<Rigidbody>().useGravity = false;

        if (_spellData.VisualEffectAsset != null)
        {
            VisualEffect visualEffect = gameObject.GetComponent<VisualEffect>();
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
            if (magicEffect == null)
            {
                Debug.Log("MagicEffect Added to Enemy");
                magicEffect = other.gameObject.AddComponent<MagicEffect>();
            }
            magicEffect.InitEffect(spellData, other.gameObject.GetComponent<HealthSystem>(), damage);
            Destroy(gameObject); // Destroy Projectile upon impact
        }
    }
}
