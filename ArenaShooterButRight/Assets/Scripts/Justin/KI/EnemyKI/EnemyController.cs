using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Justin.KI
{
    public class EnemyController : BaseController
    {
        EnemyIdleState IdleState;
        EnemyPatrolState PatrolState;
        EnemyChaseState ChaseState;
        EnemyAttackState AttackState;
        EnemyRetreatState RetreatState;
        private Transform PlayerPosition;
        private float VisionRange, AttackRange;

        protected override void Start()
        {
            InitFSM();
        }

        protected override void InitFSM()
        {
            initVariables();
            IdleState = new EnemyIdleState(); ;
            PatrolState = new EnemyPatrolState(); ;
            ChaseState = new EnemyChaseState(); ;
            AttackState = new EnemyAttackState(); ;
            RetreatState = new EnemyRetreatState(); ;
            CurrentState = IdleState;
            CurrentState.EnterState(this);
            StateDictionary = new Dictionary<BaseState, List<Transition>>
            {
                {
                    IdleState,
                    new List<Transition>
                    {
                        new Transition(() => agent.remainingDistance > VisionRange, PatrolState ),
                        new Transition(() => agent.remainingDistance < VisionRange && agent.remainingDistance > AttackRange, ChaseState ),
                        new Transition(() => agent.remainingDistance > AttackRange, AttackState ),
                    }
                },
                {
                    PatrolState,
                    new List<Transition>
                    {
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
                        // Keine Munition mehr oder wenig Leben -> Retreat State
                        // Spieler nicht mehr in Angriffsreichweite -> ChaseState
                        // Spieler Verloren -> Patrol State
                    }
                },
                {
                    RetreatState,
                    new List<Transition> 
                    {
                        // Munition besorgt -> Patrol State
                        // Objekt zwischen Spieler und Gegner -> Patrol State 
                    }

                }
            };
        }

        private void initVariables()
        {
            AttackRange = 5.0f;
            VisionRange = 7.0f;
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