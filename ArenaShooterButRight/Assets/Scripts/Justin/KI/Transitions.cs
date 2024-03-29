using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transitions : MonoBehaviour
{
    // Start is called before the first frame update
    private Func<bool> condition;
    private EnemyBaseState targetState;

    public Transitions(EnemyBaseState _targetState, Func<bool> _condition) 
    {
        condition = _condition;
        targetState = _targetState;
    }

}
