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
            base.OnUpdateState();
        }
        
        public override void OnExitState()
        {
            _controller.shootTimer = _controller.idleBeforeShootTime;
            base.OnExitState();
        }

        private IEnumerator ShootPlayer()
        {
            // GameObject bullet = Object.Instantiate(_controller.bulletPrefab);
            // yield return new WaitForSeconds(shotDelay);
            if (Physics.Raycast(_controller.gunArm.transform.position, _controller.gunArm.transform.forward, _controller.distanceToPlayer))
            {
                _controller.player.TakeDamage(10f);
            }
            yield return new WaitForSeconds(5f);
        }
    }
}
