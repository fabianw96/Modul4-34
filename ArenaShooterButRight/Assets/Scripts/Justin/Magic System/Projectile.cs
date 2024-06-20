using General;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private SpellData spellData;
    private MagicEffect magicEffect;
    private Vector3 direction;

    public void Launch(Vector3 _launchDirection, SpellData _spellData)
    {
        spellData = _spellData;
        direction = _launchDirection;
        // Implement movement logic
    }

    private void Update()
    {
        transform.Translate(direction * spellData.speed * Time.deltaTime);
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    spellData.(collision.gameObject);
    //    Destroy(gameObject);
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<HealthSystem>() != null)
        {
            
        }
    }
}
