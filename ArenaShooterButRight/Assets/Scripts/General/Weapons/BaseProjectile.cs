using General.Player;
using UnityEngine;

namespace General.Weapons
{
    public class BaseProjectile : MonoBehaviour
    {
        [SerializeField] private float damage = 0;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<PlayerController>() != null)
            {
                other.gameObject.GetComponent<HealthSystem>().TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}
