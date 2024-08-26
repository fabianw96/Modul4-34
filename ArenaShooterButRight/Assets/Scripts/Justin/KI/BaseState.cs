using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Justin.KI
{
    public abstract class BaseState
    {
        protected BaseController Controller;

        protected BaseState(EnemyController _controller) 
        {
            Controller = _controller;
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
