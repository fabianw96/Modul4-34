using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace Justin.KI
{
    public class EnemyFleeState : BaseState
    {
        private EnemyController controller;
        private float fleeDistance = 25f;
        private float fleeSpeedMultiplier = 2.5f;

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
            controller.agent.speed = controller.defaultSpeed; // Reset the speed when exiting the flee state
            base.ExitState();
        }

        private void Flee()
        {
            controller.agent.speed *= fleeSpeedMultiplier; // Increase speed during fleeing
            Vector3 directionToPlayer = controller.Player.transform.position - controller.transform.position; // Calculate direction away from the player
            Vector3 oppositeDirection = -directionToPlayer.normalized; // Normalize and reverse the direction
            Vector3 destination = controller.transform.position + oppositeDirection * fleeDistance; // Set the destination

            controller.agent.SetDestination(destination); // Move the agent to the destination
            Debug.Log("Fleeing from player!");
        }
    }
}
