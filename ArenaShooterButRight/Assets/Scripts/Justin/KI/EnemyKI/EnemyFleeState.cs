using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Justin.KI
{
    public class EnemyFleeState : BaseState
    {
        private EnemyController controller;
        public EnemyFleeState(EnemyController _controller) : base(_controller)
        {
            controller = _controller;
        }

        public virtual void EnterState()
        {

        }
        public virtual void UpdateState()
        {

        }
        public virtual void ExitState()
        {

        }
    }
}
