using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.XR;

namespace Justin.KI
{
    public class EnemyShootState : BaseState
    {
        private EnemyController controller;
        private bool _hasShot;
        private float enemyShootCooldown = 0.5f;

        private float ProjectileSpeed = 20f;
        private float ProjectileDecayDelay = 1f;
        public EnemyShootState(EnemyController _controller) : base(_controller)
        {
            controller = _controller;
        }

        public override void EnterState()
        {
            Debug.Log("Entered Shoot State");
            base.EnterState();
        }

        public override void UpdateState()
        {
            if (controller.agent.remainingDistance > 2.5)
            {
                controller.transform.LookAt(controller.Player.transform);
                controller.agent.SetDestination(controller.Player.transform.position);
            }
            if (_hasShot) return;

            controller.StartCoroutine(ShootPlayer());
        }

        private IEnumerator ShootPlayer()
        {
            GameObject bullet = Object.Instantiate(controller.BulletPrefab);
            Physics.IgnoreCollision(bullet.GetComponent<Collider>(), controller.Gun.GetComponent<Collider>());
            controller.GunMuzzlePos.transform.LookAt(controller.Player.transform);
            bullet.transform.position = controller.GunMuzzlePos.transform.position;
            Vector3 rotation = bullet.transform.rotation.eulerAngles;
            bullet.transform.rotation = Quaternion.Euler(rotation.x, controller.transform.eulerAngles.y, rotation.z);
            bullet.GetComponent<Rigidbody>().AddForce(controller.GunMuzzlePos.transform.forward * ProjectileSpeed, ForceMode.Impulse);
            controller.StartCoroutine(DestroyProjectile(bullet, ProjectileDecayDelay));
            _hasShot = true;
            yield return new WaitForSeconds(enemyShootCooldown);
            _hasShot = false;
        }

        private IEnumerator DestroyProjectile(GameObject proj, float delay)
        {
            yield return new WaitForSeconds(delay);
            Object.Destroy(proj);
        }
    }
}
