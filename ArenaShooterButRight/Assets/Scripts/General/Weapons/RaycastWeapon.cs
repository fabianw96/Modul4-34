using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace General.Weapons
{
    public abstract class RaycastWeapon : MonoBehaviour
    {
        [SerializeField] protected Transform raycastOrigin;
        [SerializeField] protected Transform raycastTarget;
        [SerializeField] protected int maxAmmo = 0;
        [SerializeField] private int currentAmmo = 0;
        [SerializeField] protected float reloadTime = 0;
        [SerializeField] protected float directDamage = 0;
        public bool isReloading = false;

        public virtual void Start()
        {
            currentAmmo = maxAmmo;
        }

        //preemptive name
        public virtual void UseGun()
        {
            if (currentAmmo > 0 && !isReloading)
            {
                Shoot();
                currentAmmo--;
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
