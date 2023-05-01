using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerfomCardState : IState
{
    public PerfomCardState(BaseCard card)
    {
        _card = card;
    }

    private BaseCard _card;
    
    // Methods ---------------------------------------------------------------------------------------------------------
    
    public void OnEnter()
    {
        Debug.Log("ENter Peform");
    }

    public void OnExit()
    {
        _card.ExitPerform();
    }

    public void Tick()
    {
        
    }
}
