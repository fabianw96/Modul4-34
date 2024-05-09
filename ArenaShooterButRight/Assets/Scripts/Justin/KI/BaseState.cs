using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Justin.KI
{
    public abstract class BaseState
    {
        public abstract void EnterState(EnemyController enemy);
        public abstract void UpdateState(EnemyController enemy);
        public abstract void ExitState(EnemyController enemy);
    }
}
