using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExploringGameState : IState
{
    public ExploringGameState(GameManager gameManager)
    {
        _gameManager = gameManager;
    }

    // References ------------------------------------------------------------------------------------------------------
    private GameManager _gameManager;
    
    // Methods ---------------------------------------------------------------------------------------------------------
    public void OnEnter()
    {
        _gameManager.IsInBattleState = false;
        Debug.Log("Enter Explo");
    }

    public void OnExit()
    {
        _gameManager.IsInBattleState = true;
        _gameManager.UpdateGameState();
        Debug.Log("Exit Explo");
    }

    public void Tick()
    {
        Debug.Log("Tick Explo");
    }
}
