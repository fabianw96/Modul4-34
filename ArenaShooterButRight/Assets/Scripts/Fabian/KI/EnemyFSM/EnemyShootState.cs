using System.Collections;
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
            if (_hasShot) return;

            _controller.StartCoroutine(ShootPlayer());
            
            base.OnUpdateState();
        }

        private IEnumerator ShootPlayer()
        {
            GameObject bullet = Object.Instantiate(_controller.bulletPrefab);
            Physics.IgnoreCollision(bullet.GetComponent<Collider>(), _controller.gunArm.GetComponent<Collider>());
            bullet.transform.position = _controller.gunArm.transform.position;
            Vector3 rotation = bullet.transform.rotation.eulerAngles;
            bullet.transform.rotation = Quaternion.Euler(rotation.x, _controller.transform.eulerAngles.y, rotation.z);
            bullet.GetComponent<Rigidbody>().AddForce(_controller.gunArm.transform.forward * _controller.projectileSpeed, ForceMode.Impulse);
            _controller.StartCoroutine(DestroyProjectile(bullet, _controller.projectileDecayDelay));
            _hasShot = true;
            yield return new WaitForSeconds(_controller.enemyShootCooldown);
            _hasShot = false;
        }

        private IEnumerator DestroyProjectile(GameObject proj, float delay)
        {
            yield return new WaitForSeconds(delay);
            Object.Destroy(proj);
        }
    }
}
