using General;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicEffect : MonoBehaviour
{

    //daten vom spieler holen: Healthsystem, playerstats (movement settings scriptable serializefield)
    //daten vom spell: dmg, dot?, debuff
    [SerializeField] private SpellData spelldata;
    [SerializeField] private HealthSystem healthSystem;

    private float dmg;



    public void InitEffect()
    {
        //wird aufgerufen wenn von spell getroffen
        //beispiel freeze
        //daten aus SO holen
        //dmg = spelldata.dmg;
        //dot
        //debuff = slow 

    }

    public void ApplyEffect(GameObject target)
    {
      

    }
}

