using System.Collections.Generic;
using General.Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace Fabian.KI.EnemyFSM
{
    public class EnemyController : BaseController
    {
        [SerializeField] public float idleTimer;
        [SerializeField] public GameObject player;
        [SerializeField] public Transform idleSpot;
        private EnemyIdleState _enemyIdleState;
        private EnemyChaseState _enemyChaseState;
        protected override void InitFsm()
        {
            _enemyIdleState = new EnemyIdleState(this);
            _enemyChaseState = new EnemyChaseState(this);

            _currentState = _enemyIdleState;
            
            _stateDictionary = new Dictionary<BaseState, List<Transition>>
            {
                {
                    _enemyIdleState,
                    new List<Transition>
                    {
                        new(() => idleTimer <= 0f, _enemyChaseState),
                    }
                },
                {
                    _enemyChaseState,
                    new List<Transition>
                    {
                        new(() => agent.remainingDistance <= 1f, _enemyIdleState)
                    }
                }
            };
            
            base.InitFsm();
        }
    }
}
