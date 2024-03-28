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
            
        }

        public virtual void OnUpdateState()
        {
            
        }

        public virtual void OnExitState()
        {
            
        }
    }
}
