using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;
namespace Justin.KI
{
    public class EnemyPatrolState : BaseState
    {
        private float searchWalkRadius = 10f;
        private float waitTimeAtPosition = 2f;
        private bool timerStarted;
        private bool goToStart;
        private Vector3 startPos;

        private EnemyController controller;
        public EnemyPatrolState(EnemyController _controller)
        {
            controller = _controller;
        }

        public override void EnterState(EnemyController enemy)
        {
            startPos = controller.transform.position;
            Vector3 newPos = Random.insideUnitSphere * 20 * searchWalkRadius;
            if (enemy.agent.SetDestination(newPos))
            {
                enemy.agent.SetDestination(newPos);
            }
        }

        public override void ExitState(EnemyController enemy)
        {

        }

        public override void UpdateState(EnemyController enemy)
        {
            if (controller.agent.remainingDistance <= 0.2f)
            {
                if (!timerStarted)
                {
                    controller.StartCoroutine(C_WaitForNewPosition());
                }
            }
        }

        private IEnumerator C_WaitForNewPosition()
        {
            timerStarted = true;
            yield return new WaitForSeconds(waitTimeAtPosition);
            goToStart = !goToStart;

            if (!goToStart)
            {
                controller.agent.SetDestination(startPos);
            }
            else
            {
                Vector3 newPos = startPos + Random.insideUnitSphere * searchWalkRadius;
                controller.agent.SetDestination(newPos);
            }

            timerStarted = false;
        }
    }
}

