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
        public HealthSystem Enemy;
        public HealthSystem Player;
        private float distanceToPlayer;
        private float VisionRange, AttackRange;

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
                        new Transition(() => Enemy.hasTakenDamage == true, ChaseState),
                        new Transition(IsInChaseRange, ChaseState ),
                        new Transition(IsInAttackRange, AttackState ),
                    }
                },
                {
                    ChaseState,
                    new List<Transition>
                    {
                        new Transition(() => !IsInAttackRange() && !IsInChaseRange(), PatrolState),
                        new Transition(IsInAttackRange, AttackState ),
                    }
                },
                {
                    AttackState,
                    new List<Transition>
                    {

                         new Transition(() => !IsInAttackRange() && !IsInChaseRange(), PatrolState),
                         new Transition(IsInChaseRange, ChaseState),
                    }
                },
            };
        }

        private void InitVariables()
        {
            AttackRange = 5.0f;
            VisionRange = 10.0f;
        }

        protected override void Update()
        {
            distanceToPlayer = Vector3.Distance(this.transform.position, Player.transform.position);
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

        private void OnDrawGizmos()
        { 
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(this.transform.position, VisionRange);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(this.transform.position, AttackRange);
        }

        private bool IsInChaseRange()
        {
            return (distanceToPlayer < VisionRange && distanceToPlayer > AttackRange);
        }

        private bool IsInAttackRange()
        {
            return (distanceToPlayer < AttackRange);
        }
    }
}