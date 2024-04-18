using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Serialization;

namespace General.Weapons
{
    public abstract class Weapon : MonoBehaviour
    {
        [SerializeField] protected int maxAmmo = 0;
        [SerializeField] private int currentAmmo = 0;
        [SerializeField] protected float reloadTime = 0;
        [SerializeField] protected float directDamage = 0;
        [SerializeField] protected float fireRate = 0;
        // [SerializeField] private Transform leftHandIKTarget;
        // [SerializeField] private Transform rightHandIKTarget;
        // [SerializeField] private TwoBoneIKConstraint leftHandIKConstraint;
        // [SerializeField] private TwoBoneIKConstraint rightHandIKConstraint;
        public bool isReloading = false;
        private float _lastShotTime = 0;

        public virtual void Start()
        {
            // leftHandIKConstraint.data.target = leftHandIKTarget;
            // rightHandIKConstraint.data.target = rightHandIKTarget;
            
            currentAmmo = maxAmmo;
        }

        //preemptive name
        public virtual void TryShoot()
        {
            if (currentAmmo > 0 && !isReloading && Time.time - _lastShotTime >= 1/ (fireRate / 60f))
            {
                Shoot();
                currentAmmo--;
                _lastShotTime = Time.time;
            }
            
            if (!isReloading && currentAmmo < 1)
            {
                isReloading = true;
                StartCoroutine(ReloadGun());
            }
        }

        public void Reload()
        {
            isReloading = true;
            StartCoroutine(ReloadGun());
        }

        public abstract void Shoot();

        IEnumerator ReloadGun()
        {
            yield return new WaitForSeconds(reloadTime);
            currentAmmo = maxAmmo;
            isReloading = false;
            
        }

    }
}
