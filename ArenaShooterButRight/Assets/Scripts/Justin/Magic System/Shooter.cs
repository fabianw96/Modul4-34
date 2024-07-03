using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private List<SpellData> spellDataList;
    [SerializeField] private Transform casterPoint;
    [SerializeField] private Mana mana;

    private void Start()
    {
        if (mana == null)
        {
            mana = GetComponent<Mana>();
        }
    }

    public void ChooseSpell(SpellType chosenSpell)
    {
        SpellData spellData = null;
        switch (chosenSpell)
        {
            case SpellType.Fireball:
                spellData = spellDataList[0];
                break;
            case SpellType.Iceball:
                spellData = spellDataList[1];
                break;
            case SpellType.Electroball:
                spellData = spellDataList[2];
                break;
        }

        if (spellData != null && mana.HasEnoughMana(spellData.manaCost))
        {
            Shoot(spellData);
            mana.UseMana(spellData.manaCost);
        }
        else
        {
            Debug.Log("Not enough mana to cast the spell");
        }
    }

    private void Shoot(SpellData spellData)
    {
        GameObject projectileInstance = Instantiate(projectilePrefab);
        Physics.IgnoreCollision(projectileInstance.GetComponent<Collider>(), gameObject.GetComponent<Collider>());
        projectileInstance.transform.position = casterPoint.transform.position;
        Vector3 rotation = projectileInstance.transform.root.eulerAngles;
        projectileInstance.transform.rotation = Quaternion.Euler(rotation.x, gameObject.transform.eulerAngles.y, rotation.z);
        projectileInstance.GetComponent<Rigidbody>().AddForce(gameObject.transform.forward * spellData.speed, ForceMode.Impulse);

        MeshRenderer meshRenderer = projectileInstance.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.enabled = false;
        }

        Projectile projectile = projectileInstance.GetComponent<Projectile>();
        projectile.Launch(spellData);
    }
}
