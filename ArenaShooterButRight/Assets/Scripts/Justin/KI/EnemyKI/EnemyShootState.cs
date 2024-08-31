using System.Collections;
using UnityEngine;

namespace Justin.KI
{
    public class EnemyShootState : BaseState
    {
        private EnemyController controller;
        private bool _hasShot;
        public EnemyShootState(EnemyController _controller) : base(_controller)
        {
            controller = _controller;
        }

        public override void EnterState()
        {
            base.EnterState();
        }

        public override void UpdateState()
        {
            if (_hasShot) return;

            controller.StartCoroutine(ShootPlayer());
        }

        private IEnumerator ShootPlayer()
        {
            GameObject bullet = Object.Instantiate(controller.BulletPrefab);
            Physics.IgnoreCollision(bullet.GetComponent<Collider>(), controller.Gun.GetComponent<Collider>());
            bullet.transform.position = controller.GunMuzzlePos.transform.position;
            Vector3 rotation = bullet.transform.rotation.eulerAngles;
            bullet.transform.rotation = Quaternion.Euler(rotation.x, controller.transform.eulerAngles.y, rotation.z);
            bullet.GetComponent<Rigidbody>().AddForce(controller.GunMuzzlePos.transform.forward * controller.ProjectileSpeed, ForceMode.Impulse);
            controller.StartCoroutine(DestroyProjectile(bullet, controller.ProjectileDecayDelay));
            _hasShot = true;
            yield return new WaitForSeconds(controller.enemyShootCooldown);
            _hasShot = false;
        }

        private IEnumerator DestroyProjectile(GameObject proj, float delay)
        {
            yield return new WaitForSeconds(delay);
            Object.Destroy(proj);
        }
    }
}
