namespace Justin.KI
{
    /// <summary>
    /// Represents a transition in the finite state machine (FSM). Defines a condition and the next state to transition to.
    /// </summary>
    public delegate bool TransitionCondition();
    public struct Transition
    {
        public TransitionCondition Condition { get; }
        public BaseState NextState { get; }

        public Transition(TransitionCondition _condition, BaseState _nextState)
        {
            Condition = _condition;
            NextState = _nextState;
        }
    }
}