using Fabian.KI.EnemyFSM;
using General.Interfaces;
using General.Player;
using UnityEngine;

namespace General
{
    public class HealthSystem : MonoBehaviour, IDamageable<float>
    {
        [SerializeField] private float maxHealth;
        [SerializeField] private float currentHealth;
        private const float DefaultHealth = 100f;

        void Start()
        {
            maxHealth = SetDefaultHealth(maxHealth);
            currentHealth = maxHealth;
        }
        
        private float SetDefaultHealth(float currentMaxValue)
        {
            return currentMaxValue > 0 ? currentMaxValue : DefaultHealth;
        }
        

        public void TakeDamage(float damageTaken)
        {
            currentHealth -= damageTaken;
            if (currentHealth < 0f)
            {
                Kill();
            }
        }

        public void HealDamage(float healingReceived)
        {
            currentHealth += healingReceived;
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
        }

        private void Kill()
        {
            if (this.gameObject.GetComponent<PlayerController>() != null)
            {
                Camera.main.transform.parent = null;
            }
            Destroy(this.gameObject);
        }
    }
}
