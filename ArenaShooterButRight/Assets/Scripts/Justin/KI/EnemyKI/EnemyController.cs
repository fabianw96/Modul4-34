using General;
using System.Collections.Generic;
using UnityEngine;

namespace Justin.KI
{
    public class EnemyController : BaseController
    {
        [SerializeField] public HealthSystem Enemy;
        [SerializeField] public HealthSystem Player;
        [SerializeField] private Animator animator;
        private string currentAnimation = "";
        private float distanceToPlayer;
        
        [SerializeField] public float VisionRange, AttackRange;
        [SerializeField] public float enemyShootCooldown;

        EnemyPatrolState PatrolState;
        EnemyChaseState ChaseState;
        EnemyShootState AttackState;
        EnemyFleeState FleeState;

        [SerializeField] public GameObject BulletPrefab;
        [SerializeField] public GameObject Gun;
        [SerializeField] public Transform GunMuzzlePos;
        [SerializeField] public float ProjectileSpeed;
        [SerializeField] public float ProjectileDecayDelay;

        protected override void Start()
        {
            InitFSM();
            Player = GameObject.Find("Player").GetComponent<HealthSystem>();
            Enemy = GetComponent<HealthSystem>();
            animator = GetComponent<Animator>();
        }

        protected override void InitFSM()
        {
            InitVariables();
            PatrolState = new EnemyPatrolState(this);
            ChaseState = new EnemyChaseState(this);
            AttackState = new EnemyShootState(this);
            FleeState = new EnemyFleeState(this);
            CurrentState = PatrolState;
            CurrentState.EnterState();
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
                {
                    FleeState,
                    new List<Transition>
                    {
                         new Transition(() => Enemy.GetHealthPercentage() < 0.2, FleeState),
                    }
                },
            };
        }

        private void InitVariables()
        {
            AttackRange = 10.0f;
            VisionRange = 20.0f;
        }

        private void ChangeAnimation(string _animation, float _crossFade = 0.2f)
        {
            if (currentAnimation != _animation)
            {
                currentAnimation = _animation;
                animator.CrossFade(_animation, _crossFade);
            }
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

        private void CheckAnimation()
        {
            if (agent.velocity != Vector3.zero && CurrentState == PatrolState)
                ChangeAnimation("Walk");
            else if (agent.velocity != Vector3.zero && CurrentState == ChaseState)
                ChangeAnimation("Chase");
            else if (agent.velocity != Vector3.zero && CurrentState == AttackState)
                ChangeAnimation("Shoot Walking");
            else if (agent.velocity == Vector3.zero && CurrentState == AttackState)
                ChangeAnimation("Shoot Standing");
            else
                ChangeAnimation("Idle");

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