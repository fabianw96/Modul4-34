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
            Debug.Log(this + " enter");
            _controller.agent.isStopped = false;
            base.OnEnterState();
        }

        public override void OnUpdateState()
        {
            Debug.Log(this + " update");
            base.OnUpdateState();
        }

        public override void OnExitState()
        {
            Debug.Log(this + " exit");
            base.OnExitState();
        }
    }
}
