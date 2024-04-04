using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Fabian.KI.EnemyFSM
{
    public class EnemyShootState : BaseState
    {
        private EnemyController _controller;
        private bool _hasShot;
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

            if (_hasShot) return;

            _controller.StartCoroutine(ShootPlayer());
            _controller.remainingShootCooldown = _controller.enemyShootCooldown;
            
            base.OnUpdateState();
        }
        
        public override void OnExitState()
        {
            _controller.shootTimer = _controller.idleBeforeShootTime;
            base.OnExitState();
        }

        private IEnumerator ShootPlayer()
        {
            _hasShot = true;
            yield return new WaitForSeconds(3);
            _controller.player.TakeDamage(5);
            _hasShot = false;
        }
    }
}
