using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace Justin.KI
{
    public class EnemyFleeState : BaseState
    {
        private EnemyController controller;
        private float fleeDistance = 10f;
        public EnemyFleeState(EnemyController _controller) : base(_controller)
        {
            controller = _controller;
        }

        public override void EnterState()
        {
            Debug.Log("Entered Flee State");
        }
        public override void UpdateState()
        {
            Flee();
        }
        public override void ExitState()
        {
            base.ExitState();
        }

        private void Flee()
        {
            Vector3 directionToPlayer = controller.Player.transform.position - controller.transform.position;
            Vector3 oppositeDirection = -directionToPlayer.normalized;
            Vector3 destination = controller.transform.position + oppositeDirection * fleeDistance;

            controller.agent.SetDestination(destination);
            Debug.Log("Fleeing from player!");
        }
    }
}
