using System.Collections;
using System.Collections.Generic;
using General;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private List<SpellData> spellDataList;
    [SerializeField] private Transform casterPoint;
    [SerializeField] private Mana mana;
    [SerializeField] private SpellLevelManager spellLevelManager;
    private bool isCooldown = false;
    
    [Space(5)]
    [Header("DEBUG")]
    [SerializeField] private SpellData spellData;
    [SerializeField] private MagicEffect magicEffect;
    [SerializeField] private float damage;
    [SerializeField] private Mesh mesh;

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

        MagicEffect lingeringEffect = this.gameObject.GetComponent<MagicEffect>();

        if (lingeringEffect != null && spellData.Type == SpellTypes.Fireball && lingeringEffect.GetSpellData().EffectType == EffectTypes.Electrified)
        {
            lingeringEffect.TriggerExplosion();
        }
        else if (magicEffect == null)
        {
            magicEffect = this.gameObject.AddComponent<MagicEffect>();
        }
        magicEffect.InitEffect(spellData, this.gameObject.GetComponent<HealthSystem>(), damage, mesh);
        Destroy(gameObject); // Destroy Projectile upon impact
    }

    public void ChooseSpell(SpellTypes _chosenSpell)
    {
        SpellData spellData = null;
        switch (_chosenSpell)
        {
            case SpellTypes.Fireball:
                spellData = spellDataList[0];
                break;
            case SpellTypes.Iceball:
                spellData = spellDataList[1];
                break;
            case SpellTypes.Electroball:
                spellData = spellDataList[2];
                break;
        }

        int spellLevel = spellLevelManager.GetSpellLevel(_chosenSpell);
        float manaCost = spellData.CalculateManaCost(spellLevel);

        if (spellData != null && mana.HasEnoughMana(manaCost) && isCooldown == false)
        {
            Shoot(spellData, spellLevel);
            mana.UseMana(manaCost);
            StartCoroutine(CooldownRoutine(spellData.CalculateCooldown(spellLevel)));
        }
        else
        {
            Debug.Log("Not enough mana to cast the spell");
        }
    }

    private IEnumerator CooldownRoutine(float _cooldownDuration)
    {
        isCooldown = true;
        Debug.Log((_cooldownDuration));
        yield return new WaitForSeconds(_cooldownDuration);
        isCooldown = false;
    }

    private void Shoot(SpellData _spellData, int _level)
    {
        GameObject projectileInstance = Instantiate(projectilePrefab);
        Physics.IgnoreCollision(projectileInstance.GetComponent<Collider>(), gameObject.GetComponent<Collider>());
        projectileInstance.transform.position = casterPoint.transform.position;
        Vector3 rotation = projectileInstance.transform.root.eulerAngles;
        projectileInstance.transform.rotation = Quaternion.Euler(rotation.x, gameObject.transform.eulerAngles.y, rotation.z);

        MeshRenderer meshRenderer = projectileInstance.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.enabled = false;
        }
        // Get the direction the player is looking
        Vector3 launchDirection = Camera.main.transform.forward;

        Projectile projectile = projectileInstance.GetComponent<Projectile>();
        projectile.Launch(_spellData, launchDirection, _level);
    }
}
