using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyStateManager : MonoBehaviour
{
    private EnemyBaseState currentState;
    public EnemyIdleState idleState = new EnemyIdleState();
    public EnemyPatrolState patrolState = new EnemyPatrolState();
    public EnemyChaseState chaseState = new EnemyChaseState();
    public EnemyFightState fightState = new EnemyFightState();

    //private Dictionary<EnemyBaseState,List<Transitions>> enemyStateDictionary;

    //public EnemyStateManager(EnemyBaseState _startState, Dictionary<EnemyBaseState, List<Transitions>> _transitions)
    //{
    //   currentState = _startState;
    //   enemyStateDictionary = _transitions;
    //}

    private void Start()
    {
        initFSM();
    }

    private void initFSM()
    {
        currentState = idleState;
        currentState.EnterState(this);
    }
    private void Update()
    {
        UpdateFSM();
    }

    private void UpdateFSM()
    {
        currentState.UpdateState(this);
    }

    public void GetNextState()
    {

    }

    public void SwitchState(EnemyBaseState state)
    {
        currentState.ExitState(this);
        currentState = state;
        currentState.EnterState(this);
    }
}
