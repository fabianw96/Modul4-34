using General;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Justin.KI
{
    public class EnemyController : BaseController
    {
        EnemyPatrolState PatrolState;
        EnemyChaseState ChaseState;
        EnemyAttackState AttackState;
        public EnemyHealthSystem enemyHealthSystem;
        public HealthSystem Player;
        private float VisionRange, AttackRange;
        private bool gotHit;

        protected override void Start()
        {
            InitFSM();
        }

        protected override void InitFSM()
        {
            InitVariables();
            PatrolState = new EnemyPatrolState(this);
            ChaseState = new EnemyChaseState(this);
            AttackState = new EnemyAttackState(this);
            CurrentState = PatrolState;
            CurrentState.EnterState(this);
            StateDictionary = new Dictionary<BaseState, List<Transition>>
            {
                {
                    PatrolState,
                    new List<Transition>
                    {
                        new Transition(() => enemyHealthSystem.hasTakenDamage == true, ChaseState),
                        new Transition(() => agent.remainingDistance < VisionRange && agent.remainingDistance > AttackRange, ChaseState ),
                        new Transition(() => agent.remainingDistance > AttackRange, AttackState ),
                    }
                },
                {
                    ChaseState,
                    new List<Transition>
                    {
                        new Transition(() => agent.remainingDistance > VisionRange, PatrolState ),
                        new Transition(() => agent.remainingDistance > AttackRange, AttackState ),
                    }
                },
                {
                    AttackState,
                    new List<Transition>
                    {

                         new Transition(() => agent.remainingDistance > VisionRange, PatrolState),
                         new Transition(() => agent.remainingDistance > AttackRange, ChaseState),
                    }
                },
            };
        }

        private void InitVariables()
        {
            AttackRange = 5.0f;
            VisionRange = 10.0f;
            gotHit = false;
        }

        protected override void Update()
        {
            UpdateFSM();
        }

        protected override void UpdateFSM()
        {
            CurrentState.UpdateState(this);

            foreach (var Tran in StateDictionary[CurrentState])
            {
                if (Tran.Condition() == true)
                {
                    CurrentState.ExitState(this);
                    CurrentState = Tran.NextState;
                    CurrentState.EnterState(this);
                }
            }
        }
    }
}