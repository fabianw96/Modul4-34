using System.Collections.Generic;
using General;
using UnityEngine;

namespace Fabian.KI.EnemyFSM
{
    public class EnemyController : BaseController
    {
        [SerializeField] public float enemyShootCooldown;
        [SerializeField] public float distanceToPlayer;
        [SerializeField] public HealthSystem player;
        private EnemyShootState _enemyShootState;
        private EnemyChaseState _enemyChaseState;

        [SerializeField] public GameObject bulletPrefab;
        [SerializeField] public GameObject gunArm;
        [SerializeField] public float projectileSpeed;
        [SerializeField] public float projectileDecayDelay;

        protected override void Update()
        {
            //find and look at player constantly needed since there's no hiding from the enemy
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
