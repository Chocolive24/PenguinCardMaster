using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateTransition
{
    public IState To { get; }
    public Func<bool> Condition { get; }

    public StateTransition(IState to, Func<bool> condition)
    {
        To = to;
        Condition = condition;
    }
}
