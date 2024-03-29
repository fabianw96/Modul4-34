using Unity.VisualScripting;
using UnityEngine;

namespace Fabian.KI.EnemyFSM
{
    public class EnemyShootState : BaseState
    {
        private EnemyController _controller;
        public EnemyShootState(EnemyController controller) : base(controller)
        {
            _controller = controller;
        }

        public override void OnEnterState()
        {
            Debug.Log(this + " enter");
            _controller.agent.isStopped = true;
            base.OnEnterState();
        }

        public override void OnUpdateState()
        {
            Debug.Log(this + " update");
            if (_controller.shootTimer > 0)
            {
                _controller.shootTimer -= Time.deltaTime;
                return;
            }
            
            base.OnUpdateState();
        }

        public override void OnExitState()
        {
            Debug.Log(this + " exit");
            _controller.shootTimer = _controller.idleBeforeShootTime;
            base.OnExitState();
        }
    }
}
