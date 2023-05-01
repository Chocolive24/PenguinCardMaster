using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InDeckCardState : IState
{
    public void OnEnter()
    {
        Debug.Log("Enter InDeck");
    }

    public void OnExit()
    {
        Debug.Log("Exit InDeck");
    }

    public void Tick()
    {
        Debug.Log("Tick InDeck");
    }
}
