using General;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

namespace Justin.KI
{
    [RequireComponent(typeof(NavMeshAgent), typeof(VisualEffectAsset), typeof(Animator))]
    public class EnemyController : BaseController
    {
        [SerializeField] public HealthSystem Enemy;
        [SerializeField] public HealthSystem Player;
        [SerializeField] private Animator animator;
        private string currentAnimation = "";
        private float distanceToPlayer;

        [SerializeField] private float VisionRange = 20f;
        [SerializeField] private float AttackRange = 10f;

        private EnemyPatrolState PatrolState;
        private EnemyChaseState ChaseState;
        private EnemyShootState ShootState;
        private EnemyFleeState FleeState;

        [SerializeField] public GameObject BulletPrefab;
        [SerializeField] public GameObject Gun;
        [SerializeField] public Transform GunMuzzlePos;


        protected override void Start()
        {
            InitFSM();
            Player = GameObject.Find("Player").GetComponent<HealthSystem>();
            Enemy = GetComponent<HealthSystem>();
            animator = GetComponent<Animator>();
        }

        protected override void InitFSM()
        {
            PatrolState = new EnemyPatrolState(this);
            ChaseState = new EnemyChaseState(this);
            ShootState = new EnemyShootState(this);
            FleeState = new EnemyFleeState(this);
            CurrentState = PatrolState;
            CurrentState.EnterState();
            StateDictionary = new Dictionary<BaseState, List<Transition>>
            {
                {
                    FleeState,
                    new List<Transition>
                    {
                         new Transition(CheckFleeCondition, FleeState),
                    }
                },
                {
                    PatrolState,
                    new List<Transition>
                    {
                        new Transition(() => Enemy.hasTakenDamage == true, ChaseState),
                        new Transition(IsInChaseRange, ChaseState ),
                        new Transition(IsInAttackRange, ShootState ),
                    }
                },
                {
                    ChaseState,
                    new List<Transition>
                    {
                        new Transition(() => !IsInAttackRange() && !IsInChaseRange(), PatrolState),
                        new Transition(IsInAttackRange, ShootState ),
                    }
                },
                {
                    ShootState,
                    new List<Transition>
                    {
                         new Transition(() => !IsInAttackRange() && !IsInChaseRange(), PatrolState),
                         new Transition(IsInChaseRange, ChaseState),
                    }
                },
            };
        }

        protected override void Update()
        {
            distanceToPlayer = Vector3.Distance(this.transform.position, Player.transform.position);
            UpdateFSM();
            CheckAnimation();
        }

        protected override void UpdateFSM()
        {
            CurrentState.UpdateState();

            foreach (var Tran in StateDictionary[CurrentState])
            {
                if (Tran.Condition() == true)
                {
                    CurrentState.ExitState();
                    CurrentState = Tran.NextState;
                    CurrentState.EnterState();
                }
            }
        }

        public void ChangeAnimation(string _animation, float _crossFade = 0.2f)
        {
            if (currentAnimation != _animation)
            {
                currentAnimation = _animation;
                animator.CrossFade(_animation, _crossFade);
            }
        }
        public void CheckAnimation()
        {
            if (agent.velocity != Vector3.zero && CurrentState == PatrolState)
                ChangeAnimation("Walk");
            else if (agent.velocity != Vector3.zero && CurrentState == ChaseState)
                ChangeAnimation("Chase");
            else if (agent.velocity != Vector3.zero && CurrentState == ShootState)
                ChangeAnimation("Shoot Walking");
            else if (agent.velocity == Vector3.zero && CurrentState == ShootState)
                ChangeAnimation("Shoot Standing");
            else
                ChangeAnimation("Idle");
        }

        private bool IsInChaseRange()
        {
            return (distanceToPlayer < VisionRange && distanceToPlayer > AttackRange);
        }

        private bool IsInAttackRange()
        {
            return (distanceToPlayer < AttackRange);
        }

        private bool CheckFleeCondition()
        {
            return (Enemy.GetHealthPercentage() <= 0.2);
        }

        private void OnDrawGizmos()
        { 
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(this.transform.position, VisionRange);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(this.transform.position, AttackRange);
        }
    }
}