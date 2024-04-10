using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Justin.KI
{
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