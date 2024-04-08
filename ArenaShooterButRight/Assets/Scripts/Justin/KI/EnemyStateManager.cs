using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Justin.KI
{
    public class EnemyStateManager : MonoBehaviour
    {
        public EnemyBaseState CurrentState;
        public EnemyIdleState IdleState;
        public EnemyPatrolState PatrolState;
        public EnemyChaseState ChaseState;
        public EnemyAttackState AttackState;
        public EnemyRetreatState RetreatState;
        public Dictionary<EnemyBaseState, List<Transition>> StateDictionary;

        public NavMeshAgent Agent;
        public Transform PlayerPosition;
        public LayerMask Player;
        public float VisionRange, AttackRange;
        public bool playerInSightRange, playerInAttackRange;

        private void Start()
        {
            Agent = GetComponent<NavMeshAgent>();
            PlayerPosition = GameObject.Find("PlayerObj").transform;
            initFSM();
        }

        private void initFSM()
        {
            CurrentState = IdleState;
            CurrentState.EnterState(this);
            IdleState = new EnemyIdleState();
            PatrolState = new EnemyPatrolState();
            ChaseState = new EnemyChaseState();
            AttackState = new EnemyAttackState();
            RetreatState = new EnemyRetreatState();

            StateDictionary = new Dictionary<EnemyBaseState, List<Transition>>
            {
                {
                    IdleState,
                    new List<Transition>
                    {
                        new Transition(() => !playerInSightRange && !playerInAttackRange, PatrolState ),
                        new Transition(() => playerInSightRange && !playerInAttackRange, ChaseState ),
                        new Transition(() => playerInSightRange && playerInAttackRange, AttackState ),
                    }

                },
                {
                    PatrolState,
                    new List<Transition>
                    {
                        new Transition(() => playerInSightRange && !playerInAttackRange, ChaseState ),
                        new Transition(() => playerInSightRange && playerInAttackRange, AttackState ),
                    }
                },
                {
                    ChaseState,
                    new List<Transition>
                    {
                        new Transition(() => !playerInSightRange && !playerInAttackRange, PatrolState ),
                        new Transition(() => playerInSightRange && playerInAttackRange, AttackState ),
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
        private void Update()
        {
            UpdateFSM();
        }

        private void UpdateFSM()
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