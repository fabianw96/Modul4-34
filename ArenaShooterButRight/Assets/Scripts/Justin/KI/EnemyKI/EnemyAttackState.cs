using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Justin.KI
{
    public class EnemyAttackState : BaseState
    {
        private EnemyController enemyController;

        public EnemyAttackState(EnemyController _enemyController)
        {
            enemyController = _enemyController;
        }

        public override void EnterState(EnemyController enemy)
        {
        
        }

        public override void ExitState(EnemyController enemy)
        {
        
        }

        public override void UpdateState(EnemyController enemy)
        {
            // Attack behavior
            // Example: Play attack animation
            // Example: Deal damage to the player
            Debug.Log("Attacking player!");

            // Example: Play attack animation (if using Animator component)
            // animator.SetTrigger("Attack");

            // Example: Deal damage to the player (if player has health)
            // player.GetComponent<PlayerHealth>().TakeDamage(attackDamage);
        }
    }
}
