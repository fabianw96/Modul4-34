using General;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthSystem : HealthSystem
{
    public bool hasTakenDamage;
    public override void TakeDamage(float damageTaken)
    {
        hasTakenDamage = true;
        base.TakeDamage(damageTaken);
    }
}
