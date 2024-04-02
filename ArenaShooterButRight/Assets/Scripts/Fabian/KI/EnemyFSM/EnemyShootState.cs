using System.Collections;
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
            _controller.agent.isStopped = true;
            base.OnEnterState();
        }

        public override void OnUpdateState()
        {
            if (_controller.shootTimer > 0)
            {
                _controller.shootTimer -= Time.deltaTime;
                return;
            }

            //shoot at player
            while (_controller.remainingShootCooldown > 0)
            {
                _controller.remainingShootCooldown -= Time.deltaTime;
                return;
            }
            Debug.Log("Pew pew!");
            _controller.remainingShootCooldown = _controller.enemyShootCooldown;
            
            
            base.OnUpdateState();
        }
        
        public override void OnExitState()
        {
            _controller.shootTimer = _controller.idleBeforeShootTime;
            base.OnExitState();
        }
    }
}
