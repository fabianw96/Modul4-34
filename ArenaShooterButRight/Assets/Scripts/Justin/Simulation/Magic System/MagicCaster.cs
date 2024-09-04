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

        float manaCost = spellData.CalculateManaCost();

        if (spellData != null && mana.HasEnoughMana(manaCost) && isCooldown == false)
        {
            mana.UseMana(manaCost);
            Shoot(spellData);
            StartCoroutine(CooldownRoutine(spellData.CalculateCooldown()));
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
        // Instantiate the projectile
        GameObject projectileInstance = Instantiate(projectilePrefab);
        
        // Ignore collision between the projectile and the player
        Physics.IgnoreCollision(projectileInstance.GetComponent<Collider>(), gameObject.GetComponent<Collider>());
        
        //Set the projectile's position at the cast point
        projectileInstance.transform.position = castPoint.transform.position;

        // Set the direction to the camera's forward direction
        Vector3 launchDirection = Camera.main.transform.forward;

        // Rotate the projectile to face the direction it will be moving
        projectileInstance.transform.rotation = Quaternion.LookRotation(launchDirection);

        // Get the MagicProjectile component and initialize it
        MagicProjectile projectile = projectileInstance.GetComponent<MagicProjectile>();
        projectile.InitSpellProjectile(_spellData);

        Rigidbody projectileRigidbody = projectileInstance.GetComponent<Rigidbody>();
        projectileRigidbody.velocity = launchDirection * _spellData.CalculateSpeed();
    }
}
