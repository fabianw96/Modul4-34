namespace Fabian.KI
{
    public delegate bool TransitionCondition();
    public struct Transition
    {
        public TransitionCondition Condition { get; set; }
        public BaseState NextState { get; set; }

        public Transition(TransitionCondition condition, BaseState nextState)
        {
            Condition = condition;
            NextState = nextState;
        }
    }
}
