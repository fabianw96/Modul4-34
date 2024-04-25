using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace Justin.KI
{
    public class EnemySearchCover : BaseState
    {
        public override void EnterState(EnemyController enemy)
        {

        }

        public override void ExitState(EnemyController enemy)
        {

        }

        public override void UpdateState(EnemyController enemy)
        {
            // Cover behavior
            // Example: Find a nearby cover position
            // Vector3 coverPosition = FindCoverPosition();

            // Example: Move towards the cover position
            // transform.position = Vector3.MoveTowards(transform.position, coverPosition, moveSpeed * Time.deltaTime);

            // Example: Hide behind cover
            // Here you would have additional logic to actually hide behind the cover,
            // such as crouching, changing animations, or activating a shield.
        }

        // private Vector3 FindCoverPosition()
        // {
            // Example: For simplicity, let's just return a position behind the player for now
            // Vector3 playerDirection = transform.position - player.transform.position;
            // return player.transform.position + playerDirection.normalized * 5f; // 5 units behind the player
        // }
    }
}
