using Justin.KI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class BaseController : MonoBehaviour
{
    protected BaseState CurrentState;
    protected Transition Transition;
    protected Dictionary<BaseState, List<Transition>> StateDictionary;
    [SerializeField] public NavMeshAgent agent;
    
    protected virtual void Start()
    {
        InitFSM();
    }

    
    protected virtual void Update()
    {
        UpdateFSM();
    }

    protected virtual void InitFSM() 
    {

    }

    protected virtual void UpdateFSM()
    {

    }

}
