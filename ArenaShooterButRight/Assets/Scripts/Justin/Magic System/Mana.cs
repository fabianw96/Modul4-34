using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mana : MonoBehaviour
{
    [SerializeField] private float maxMana = 100f;
    [SerializeField] private float currentMana;
    [SerializeField] private float manaRegenRate;

    // Start is called before the first frame update
    void Start()
    {
        currentMana = maxMana;
        StartCoroutine(RegenerateMana());
    }

    public bool HasEnoughMana(float amount)
    {
        return currentMana >= amount;
    }

    public void UseMana(float amount)
    {
        if (HasEnoughMana(amount))
        {
            currentMana -= amount;
        }
    }

    private IEnumerator RegenerateMana()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (currentMana < maxMana)
            {
                currentMana += manaRegenRate;
                if (currentMana > maxMana)
                {
                    currentMana = maxMana;
                }
            }
        }
    }

    public float GetCurrentMana()
    {
        return currentMana;
    }

    public float GetMaxMana()
    {
        return maxMana;
    }

}
