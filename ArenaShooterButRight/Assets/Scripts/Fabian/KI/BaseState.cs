using UnityEngine;

namespace Fabian.KI
{
    public abstract class BaseState
    {
        protected BaseController Controller;

        protected BaseState(BaseController controller)
        {
            Controller = controller;
        }
        
        public virtual void OnEnterState()
        {
            Debug.Log(this + " enter");
        }

        public virtual void OnUpdateState()
        {
            Debug.Log(this + " update");

        }

        public virtual void OnExitState()
        {
            Debug.Log(this + " exit");

        }
    }
}
