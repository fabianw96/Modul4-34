using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace Justin.KI
{
    public class EnemyChaseState : BaseState
    {
        EnemyController controller;
        public EnemyChaseState(EnemyController _controller) : base(_controller)
        {
            controller = _controller;
        }

        public override void EnterState()
        {

        }

        public override void ExitState()
        {

        }

        public override void UpdateState()
        {
            Debug.Log("Entered Chase State");
            controller.transform.LookAt(controller.Player.transform);
            controller.agent.SetDestination(controller.Player.transform.position);
        }
    }
}

