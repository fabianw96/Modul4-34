using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;
namespace Justin.KI
{
    public class EnemyPatrolState : BaseState
    {
        public override void EnterState(EnemyController enemy)
        {

        }

        public override void ExitState(EnemyController enemy)
        {

        }

        public override void UpdateState(EnemyController enemy)
        {
            // Patrol behavior
            // Example: Move back and forth between two points
            //transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

            // Example: If reaching a boundary, turn around
            //if (transform.position.x > 5f || transform.position.x < -5f)
            //{
            //    transform.Rotate(Vector3.up, 180f);
            //}

            // Check for player within detection range to switch to chase state
            //if (Vector3.Distance(transform.position, player.transform.position) < detectionRange)
            //{
            //    currentState = EnemyState.Chase;
            //}
        }
    }
}

