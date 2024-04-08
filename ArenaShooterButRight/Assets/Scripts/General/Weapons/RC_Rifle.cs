using Fabian.KI.EnemyFSM;
using UnityEngine;

namespace General.Weapons
{
    public class RC_Rifle : RaycastWeapon
    {
        public override void Shoot()
        {
            RaycastHit hit;
            if (!Physics.Raycast(raycastOrigin.position, raycastTarget.position, out hit)) return;

            if (hit.collider.gameObject.GetComponent<IDamageable<float>>() != null)
            {
                hit.collider.gameObject.GetComponent<IDamageable<float>>().TakeDamage(directDamage);
            }
        }
    }
}
