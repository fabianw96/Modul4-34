using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private List<SpellData> spellDataList;
    [SerializeField] private Transform casterPoint;

    public void ChooseSpell(SpellType chosenSpell)
    {
        Debug.Log("In ChooseSpeel Method");
        switch (chosenSpell)
        {
            case SpellType.Fireball:
                Shoot(spellDataList[0]);
                break;
            case SpellType.Iceball:
                Shoot(spellDataList[1]);
                break;
            case SpellType.Electroball:
                Shoot(spellDataList[2]);
                break;
        }
    }

    private void Shoot(SpellData spellData)
    {
        Debug.Log("In Shoot Method");
        GameObject projectileInstance = Instantiate(projectilePrefab);
        Physics.IgnoreCollision(projectileInstance.GetComponent<Collider>(), gameObject.GetComponent<Collider>());
        projectileInstance.transform.position = casterPoint.transform.position;
        Vector3 rotation = projectileInstance.transform.root.eulerAngles;
        projectileInstance.transform.rotation = Quaternion.Euler(rotation.x, gameObject.transform.eulerAngles.y, rotation.z);
        projectileInstance.GetComponent<Rigidbody>().AddForce(gameObject.transform.forward * 100f, ForceMode.Impulse);
        Projectile projectile = projectileInstance.GetComponent<Projectile>();
        projectile.Launch(spellData);
    }
}
