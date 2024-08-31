using System.Collections;
using UnityEngine;

namespace Justin.KI
{
    public class EnemyPatrolState : BaseState
    {
        private float searchWalkRadius = 10f;
        private float waitTimeAtPosition = 5f;
        private bool timerStarted;
        private bool goToStart;
        private Vector3 startPos;

        private EnemyController controller;
        public EnemyPatrolState(EnemyController _controller) : base(_controller)
        {
            controller = _controller;
        }

        public override void EnterState()
        {
            Debug.Log("Entered Patrol State");
            startPos = controller.transform.position;
            Vector3 newPos = Random.insideUnitSphere * 20 * searchWalkRadius;
            if (controller.agent.SetDestination(newPos))
            {
                controller.agent.SetDestination(newPos);
            }
        }

        public override void ExitState()
        {

        }

        public override void UpdateState()
        {
            if (controller.agent.remainingDistance <= 0.1f)
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

