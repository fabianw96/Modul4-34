using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public Transform shootPoint;
    public GameObject projectilePrefab;
    public List<SpellData> spellDataList;
    // Spelltype from UI

    private void ChooseSpell()
    {
       switch (/*Welcher spell in UI gewählt*/)
       {
            //case SpellType.Fireball:
            //    Shoot()
       }
    }

    public void Shoot(Vector3 direction, SpellData spellData)
    {
        GameObject projectileInstance = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
        Projectile projectile = projectileInstance.GetComponent<Projectile>();
        projectile.Launch(direction);
    }
}
