using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    // Current State can be whatever state, so it's an IState type. (kind of polymorphism)
    private IState _currentState;

    public IState To { get; }

    // Key is whatever type, Value is a List that store all the currentState transitions.
    // We can only store Types in C#, not in C++.
    private readonly Dictionary<Type, List<StateTransition>> _allTransitions = 
        new Dictionary<Type, List<StateTransition>>();
    
    // Methods ---------------------------------------------------------------------------------------------------------
    public void Tick()
    {
        List<StateTransition> currentTypeTransitions = new List<StateTransition>();
        
        // Check Transitions.
        // We get the type of the current and add all it's transitions to the list.
        if (_allTransitions.TryGetValue(_currentState.GetType(), out currentTypeTransitions))
        {
            foreach (var transition in currentTypeTransitions)
            {
                if (transition.Condition() == true)
                {
                    SetState(transition.To);
                }
            }
        }
        
        // Tick the current state. (currentState is null at the first turn.)
        if (_currentState != null)
        {
            _currentState.Tick();
        }
        
    }

    public void AddTransition(IState from, IState to, Func<bool> condition)
    {
        if (_allTransitions.TryGetValue(from.GetType(), out List<StateTransition> existingConditions))
        {
            // Already have transitions.
            existingConditions.Add(new StateTransition(to, condition));
        }
        else
        {
            // Doesn't have transitions.
            List<StateTransition> newConditions = new List<StateTransition>();
            newConditions.Add(new StateTransition(to, condition));
            _allTransitions.Add(from.GetType(), newConditions);
        }
       
    }
    
    public void SetState(IState newState)
    {
        if (_currentState != null)
        {
            // Perform the current state Exit method. (currentState is null at the first turn.)
            _currentState.OnExit();
        }
        
        // Change the state to a new one. (In the first turn, it's now that the current state is no longer null.)
        _currentState = newState;
        
        // Perform the Enter method of the new state that is now our current state.
        _currentState.OnEnter();
    }
}
