using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardedCardState : IState
{
    public DiscardedCardState(BaseCard card)
    {
        _card = card;
    }

    private BaseCard _card;
    
    // Methods ---------------------------------------------------------------------------------------------------------
    public void OnEnter()
    {
        //_card.EnterDiscarded();
    }

    public void OnExit()
    {
        Debug.Log("Exit Discarded");
    }

    public void Tick()
    {
        Debug.Log("Tick Discarded");
    }
}
