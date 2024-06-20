using System;
using General;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicEffect : MonoBehaviour
{
    //daten vom spieler holen: Healthsystem, playerstats (movement settings scriptable serializefield)
    //daten vom spell: dmg, dot?, debuff
    private HealthSystem _healthSystem;
    private float _dmg;
    
    public void InitEffect(SpellData spellData, HealthSystem healthSys)
    {
        //wird aufgerufen wenn von spell getroffen
        //beispiel freeze
        //daten aus SO holen
        //dmg = spelldata.dmg;
        //dot
        //debuff = slow 
        _healthSystem = healthSys;
        _dmg = spellData.damage;

        //TODO: isdot abfragen. Wenn ja, applyeffect, wenn nein, dealdmg.

        if (spellData.isDot)
        {
            ApplyEffect();
            // StartCoroutine(DotDamage());
        }
        healthSys.TakeDamage(spellData.damage);
        //TODO: delete self when damage has been taken.
    }

    private void ApplyEffect()
    {
        Debug.Log(_healthSystem.gameObject.name);
        //TODO: delete self when effect duration is over.
    }

    // IEnumerator DotDamage()
    // {
    //     //deal dmg every 1 second
    //     hasTakenDotDamage = true;
    //     yield return new WaitForSeconds(1f);
    //     hasTakenDotDamage = false;
    // }
}

