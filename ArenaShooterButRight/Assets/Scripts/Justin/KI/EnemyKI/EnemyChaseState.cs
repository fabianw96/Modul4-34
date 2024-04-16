using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;
namespace Justin.KI
{
    public class EnemyChaseState : BaseState
    {
        public override void EnterState(EnemyController enemy)
        {

        }

        public override void ExitState(EnemyController enemy)
        {

        }

        public override void UpdateState(EnemyController enemy)
        {
            // Chase behavior
            // Example: Move towards the player's position
            //transform.LookAt(player.transform);
            //transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

            // Check if player is out of range to switch back to patrol state
            //if (Vector3.Distance(transform.position, player.transform.position) > detectionRange)
            //{
            //    currentState = EnemyState.Patrol;
            //}

            // Example: If close enough, switch to attack state
            //if (Vector3.Distance(transform.position, player.transform.position) < 2f)
            //{
            //    currentState = EnemyState.Attack;
            //}
        }
    }
}

