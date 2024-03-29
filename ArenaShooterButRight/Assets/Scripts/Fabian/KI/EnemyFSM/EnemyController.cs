using System.Collections.Generic;
using General.Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace Fabian.KI.EnemyFSM
{
    public class EnemyController : BaseController
    {
        [SerializeField] public float idleBeforeShootTime;
        [SerializeField] private float distanceToPlayer;
        [SerializeField] public GameObject player;
        [HideInInspector] public float shootTimer;
        private EnemyShootState _enemyShootState;
        private EnemyChaseState _enemyChaseState;

        protected override void Start()
        {
            shootTimer = idleBeforeShootTime;
            base.Start();
        }

        protected override void Update()
        {
            agent.SetDestination(player.transform.position);
            agent.transform.LookAt(player.transform);
            IsInRange();
            base.Update();
        }

        protected override void InitFsm()
        {
            _enemyShootState = new EnemyShootState(this);
            _enemyChaseState = new EnemyChaseState(this);

            CurrentState = _enemyChaseState;
            
            StateDictionary = new Dictionary<BaseState, List<Transition>>
            {
                {
                    _enemyShootState,
                    new List<Transition>
                    {
                        new Transition(() => !IsInRange(), _enemyChaseState)
                    }
                },
                {
                    _enemyChaseState,
                    new List<Transition>
                    {
                        new Transition(() => IsInRange(), _enemyShootState)
                    }
                }
            };
            
            base.InitFsm();
        }

        private bool IsInRange()
        {
            return agent.remainingDistance <= distanceToPlayer;
        }
    }
}
