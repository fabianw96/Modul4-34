using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public Transform shootPoint;
    public GameObject projectilePrefab;

    public void Shoot(Vector3 direction)
    {
        GameObject projectileInstance = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
        IProjectile projectile = projectileInstance.GetComponent<IProjectile>();
        projectile.Launch(direction);
    }
}
