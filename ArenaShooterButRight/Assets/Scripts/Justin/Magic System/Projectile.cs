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
        spellData = _spellData;
        // direction = _launchDirection;
        // Implement movement logic
    }

    private void Update()
    {
        transform.Translate(direction * (spellData.speed * Time.deltaTime));
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    spellData.(collision.gameObject);
    //    Destroy(gameObject);
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<HealthSystem>() != null && other.gameObject.GetComponent<MagicEffect>() == null)
        {
            magicEffect = other.gameObject.AddComponent<MagicEffect>();
            magicEffect.InitEffect(spellData, other.gameObject.GetComponent<HealthSystem>());
        }
        
        //TODO: check if other gameobject already has magiceffect. initeffect on that one then.
    }
}
