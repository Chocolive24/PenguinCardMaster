using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawnCardState : IState
{
    public DrawnCardState(BaseCard card)
    {
        _card = card;
    }

    private BaseCard _card;
    
    // Methods ---------------------------------------------------------------------------------------------------------
    public void OnEnter()
    {
        _card.EnterDrawn();
    }

    public void OnExit()
    {
        Debug.Log("Exit Drawn");
    }

    public void Tick()
    {
        Debug.Log("Tick Drawn");
    }
}
