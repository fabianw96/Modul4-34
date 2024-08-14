using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Fabian.KI
{
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class BaseController : MonoBehaviour
    {
        protected BaseState CurrentState;
        protected Transition Transition;
        protected Dictionary<BaseState, List<Transition>> StateDictionary;
        
        [SerializeField] public NavMeshAgent agent;
        
        // Start is called before the first frame update
        protected virtual void Start()
        {
            InitFsm();
        }

        protected virtual void InitFsm()
        {
            CurrentState.OnEnterState();
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            UpdateFsm();
        }

        private void UpdateFsm()
        {
            CurrentState.OnUpdateState();
            
            foreach (Transition tran in StateDictionary[CurrentState])
            {
                if (!tran.Condition()) continue;
                
                CurrentState.OnExitState();
                CurrentState = tran.NextState;
                CurrentState.OnEnterState();

                break;
            }
        }
    }
}
