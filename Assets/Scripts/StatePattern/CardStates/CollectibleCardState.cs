using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleCardState : IState
{
    public CollectibleCardState(BaseCard card)
    {
        _card = card;
    }

    private BaseCard _card;
    
    // Methods ---------------------------------------------------------------------------------------------------------
    public void OnEnter()
    {
        _card.gameObject.SetActive(true);
    }

    public void OnExit()
    {
        
    }

    public void Tick()
    {
        
    }
}
