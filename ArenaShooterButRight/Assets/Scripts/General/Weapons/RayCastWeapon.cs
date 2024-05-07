using System;
using Fabian.KI.EnemyFSM;
using General.Interfaces;
using UnityEngine;

namespace General.Weapons
{
    public class RayCastWeapon : Weapon
    {
        [SerializeField] private Transform raycastOrigin;
        // [SerializeField] private Transform raycastTarget;
        [SerializeField] private bool isFullAuto;
        private Ray _ray;
        private RaycastHit _hit;


        public override void Shoot()
        {
            _ray.origin = raycastOrigin.position;
            _ray.direction = Camera.main.ScreenPointToRay(Input.mousePosition).direction;
            Debug.DrawRay(_ray.origin, _ray.direction  * 10f, Color.red, 1.0f);
            
            if (!Physics.Raycast(_ray, out _hit)) return;

            if (_hit.collider.gameObject.GetComponent<IDamageable<float>>() == null || _hit.collider.CompareTag("Player")) return;
            
            
            _hit.collider.gameObject.GetComponent<IDamageable<float>>().TakeDamage(directDamage);
        }

        private void OnDrawGizmos()
        {
        }
    }
}
