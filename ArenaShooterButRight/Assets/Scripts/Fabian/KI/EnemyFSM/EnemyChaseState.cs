using UnityEngine;

namespace Fabian.KI.EnemyFSM
{
    public class EnemyChaseState : BaseState
    {
        private EnemyController _controller;
        public EnemyChaseState(EnemyController controller) : base(controller)
        {
            _controller = controller;
        }
        
        public override void OnEnterState()
        {
            _controller.agent.isStopped = false;
            base.OnEnterState();
        }

        public override void OnUpdateState()
        {
            base.OnUpdateState();
        }

        public override void OnExitState()
        {
            base.OnExitState();
        }
    }
}
