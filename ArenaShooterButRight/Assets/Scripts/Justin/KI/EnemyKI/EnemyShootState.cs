using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.XR;

namespace Justin.KI
{
    public class EnemyShootState : BaseState
    {
        private EnemyController controller;
        private bool _hasShot;
        private float enemyShootCooldown = 1f;

        private float ProjectileSpeed = 15f;
        private float ProjectileDecayDelay = 1f;

        private float minDistanceFromPlayer = 5f;
        private float maxDistanceFromPlayer = 10f;

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
            // Move to a safe distance if too close to the player
            controller.transform.LookAt(controller.Player.transform);
            if (controller.distanceToPlayer <= minDistanceFromPlayer)
            {
                MaintainSafeDistance();
            }

            if (_hasShot) return;
            controller.StartCoroutine(ShootPlayer());
        }

        private void MaintainSafeDistance()
        {
            // Calculate a position that is within the desired range from the player
            Vector3 directionToPlayer = controller.Player.transform.position - controller.transform.position;
            Vector3 desiredPosition = controller.Player.transform.position - directionToPlayer.normalized * Random.Range(minDistanceFromPlayer, maxDistanceFromPlayer);
            controller.agent.SetDestination(desiredPosition);
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
