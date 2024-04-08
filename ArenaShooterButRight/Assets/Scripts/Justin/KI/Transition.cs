using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Justin.KI
{
    public delegate bool TransitionCondition();
    public struct Transition
    {
        public TransitionCondition Condition { get; set; }
        public EnemyBaseState NextState { get; set; }

        public Transition(TransitionCondition _condition, EnemyBaseState _nextState)
        {
            Condition = _condition;
            NextState = _nextState;
        }
    }
}