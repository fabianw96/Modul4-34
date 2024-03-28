using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace Fabian.KI
{
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class BaseController : MonoBehaviour
    {
        public string myCurrentState;
        protected BaseState _currentState;
        protected Transition _transition;
        protected Dictionary<BaseState, List<Transition>> _stateDictionary;
        
        [SerializeField] public NavMeshAgent agent;

        
        
        // Start is called before the first frame update
        protected void Start()
        {
            InitFsm();
        }

        protected virtual void InitFsm()
        {
            _currentState.OnEnterState();
        }

        // Update is called once per frame
        protected void Update()
        {
            UpdateFsm();
        }

        protected void UpdateFsm()
        {
            _currentState.OnUpdateState();
            
            foreach (var tran in _stateDictionary[_currentState])
            {
                if (tran.Condition())
                {
                    _currentState.OnExitState();
                    _currentState = tran.NextState;
                    _currentState.OnEnterState();

                    break;
                }
            }
        }
    }
}
