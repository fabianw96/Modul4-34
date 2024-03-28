using Unity.VisualScripting;
using UnityEngine;

namespace Fabian.KI.EnemyFSM
{
    public class EnemyIdleState : BaseState
    {
        private EnemyController _controller;
        public EnemyIdleState(EnemyController controller) : base(controller)
        {
            _controller = controller;
        }

        public override void OnEnterState()
        {
            Debug.Log(this + " enter");
            _controller.agent.destination = _controller.idleSpot.position;
            base.OnEnterState();
        }

        public override void OnUpdateState()
        {
            Debug.Log(this + " update");
            _controller.idleTimer -= Time.deltaTime;
            base.OnUpdateState();
        }

        public override void OnExitState()
        {
            Debug.Log(this + " exit");
            base.OnExitState();
        }
    }
}
