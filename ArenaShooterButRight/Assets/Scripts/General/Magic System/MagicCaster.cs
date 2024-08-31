using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class MagicCaster : MonoBehaviour
{
    [SerializeField] private SpellLevelManager spellLevelManager;
    [SerializeField] private List<SpellData> spellDataList;
    [SerializeField] private Transform castPoint;
    [SerializeField] private Mana mana;
    [SerializeField] private GameObject projectilePrefab;
    private bool isCooldown = false;

    private void Start()
    {
        if (mana == null)
        {
            mana = GetComponent<Mana>();
        }

        if (spellLevelManager == null)
        {
            spellLevelManager = GetComponent<SpellLevelManager>();
        }
    }

    public void ChooseSpell(Elements _chosenSpell)
    {
        SpellData spellData = null;
        switch (_chosenSpell)
        {
            case Elements.Fire:
                spellData = spellDataList[0];
                break;
            case Elements.Ice:
                spellData = spellDataList[1];
                break;
            case Elements.Electro:
                spellData = spellDataList[2];
                break;
        }

        int spellLevel = spellLevelManager.GetSpellLevel(_chosenSpell);
        float manaCost = spellData.CalculateManaCost(spellLevel);

        if (spellData != null && mana.HasEnoughMana(manaCost) && isCooldown == false)
        {
            mana.UseMana(manaCost);
            Shoot(spellData);
            StartCoroutine(CooldownRoutine(spellData.CalculateCooldown(spellLevel)));
        }
        else 
        {

        }
    }

    private IEnumerator CooldownRoutine(float _cooldownDuration)
    {
        isCooldown = true;
        yield return new WaitForSeconds(_cooldownDuration);
        isCooldown = false;
    }

    private void Shoot(SpellData _spellData)
    {
        GameObject projectileInstance = Instantiate(projectilePrefab);
        Physics.IgnoreCollision(projectileInstance.GetComponent<Collider>(), gameObject.GetComponent<Collider>());
        projectileInstance.transform.position = castPoint.transform.position;
        Vector3 rotation = projectileInstance.transform.root.eulerAngles;
        projectileInstance.transform.rotation = Quaternion.Euler(rotation.x, gameObject.transform.eulerAngles.y, rotation.z);
        Vector3 launchDirection = Camera.main.transform.forward;
        MagicProjectile projectile = projectileInstance.GetComponent<MagicProjectile>();
        projectile.InitSpellProjectile(_spellData);
    }
}
