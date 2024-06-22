using System;
using General;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private SpellData spellData;
    private MagicEffect magicEffect;
    private Vector3 direction;

    public void Launch(SpellData _spellData)
    {
        Debug.Log("In Launch Method");
        spellData = _spellData;
        direction = transform.forward;
    }

    private void Update()
    {
        transform.Translate(direction * (spellData.speed * Time.deltaTime));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<HealthSystem>() != null)
        {
            MagicEffect magicEffect = other.gameObject.GetComponent<MagicEffect>();
            if (magicEffect == null)
            {
                magicEffect = other.gameObject.AddComponent<MagicEffect>();
            }
            magicEffect.InitEffect(spellData, other.gameObject.GetComponent<HealthSystem>());
            Destroy(gameObject); // Destroy Projectile upon impact
        }
        
        //TODO: check if other gameobject already has magiceffect. initeffect on that one then.
        // Should be checked by the logic above
    }
}
