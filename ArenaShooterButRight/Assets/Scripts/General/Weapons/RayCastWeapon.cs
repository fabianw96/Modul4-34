using Fabian.KI.EnemyFSM;
using General.Interfaces;
using UnityEngine;

namespace General.Weapons
{
    public class RayCastWeapon : Weapon
    {
        [SerializeField] private Transform raycastOrigin;
        [SerializeField] private Transform raycastTarget;
        [SerializeField] private bool isFullAuto;
        private Ray _ray;
        private RaycastHit _hit;


        public override void Shoot()
        {
            _ray.origin = raycastOrigin.position;
            _ray.direction = raycastTarget.position - raycastOrigin.position;
            
            if (!Physics.Raycast(_ray, out _hit)) return;

            if (_hit.collider.gameObject.GetComponent<IDamageable<float>>() == null) return;
            
            
            _hit.collider.gameObject.GetComponent<IDamageable<float>>().TakeDamage(directDamage);
            Debug.DrawLine(_ray.origin, _hit.point, Color.red, 1.0f);
        }
    }
}
