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
            startPos = controller.transform.position; // Set the start position
            SetNewPatrolDestination(); // Set the initial patrol destination
        }

        public override void ExitState()
        {
            controller.agent.isStopped = false; // Ensure the agent is not stopped when exiting the state
        }

        public override void UpdateState()
        {
            if (controller.agent.remainingDistance <= 0.1f && !timerStarted)
            {   
                controller.StartCoroutine(C_WaitForNewPosition());
            }
        }

        private void SetNewPatrolDestination()
        {
            Vector3 newPos = Random.insideUnitSphere * searchWalkRadius + startPos;  // Generate a new random patrol position
            controller.agent.SetDestination(newPos);  // Set the destination for the agent
            controller.agent.isStopped = false;  // Ensure the agent is not stopped
        }


        private IEnumerator C_WaitForNewPosition()
        {
            timerStarted = true;
            controller.agent.isStopped = true; // Stop the agent during the wait time
            yield return new WaitForSeconds(waitTimeAtPosition);
            controller.agent.isStopped = false; // Resume the agent's movement
            goToStart = !goToStart; // Toggle between going to the start position and a new random position

            if (!goToStart)
            {
                controller.agent.SetDestination(startPos);
            }
            else
            {
                SetNewPatrolDestination(); 
            }

            timerStarted = false;
        }
    }
}

