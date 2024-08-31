using UnityEngine;

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
            Debug.Log("Entered Chase State");
        }

        public override void ExitState()
        {

        }

        public override void UpdateState()
        {
            controller.transform.LookAt(controller.Player.transform);
            controller.agent.SetDestination(controller.Player.transform.position);
        }
    }
}

