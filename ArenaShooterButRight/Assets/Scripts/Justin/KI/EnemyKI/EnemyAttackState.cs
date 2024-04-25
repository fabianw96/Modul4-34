using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Justin.KI
{
    public class EnemyAttackState : BaseState
    {
        private EnemyController controller;

        public EnemyAttackState(EnemyController _controller)
        {
            controller = _controller;
        }

        public override void EnterState(EnemyController enemy)
        {
        
        }

        public override void ExitState(EnemyController enemy)
        {
        
        }

        public override void UpdateState(EnemyController enemy)
        {

            controller.agent.SetDestination(controller.transform.position);
            controller.transform.LookAt(controller.Player.transform.position);

            
        }
    }
}
