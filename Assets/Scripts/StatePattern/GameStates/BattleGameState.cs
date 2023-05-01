using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleGameState : IState
{
    public BattleGameState(BattleManager battleManager)
    {
        _battleManager = battleManager;
    }
    
    // References ------------------------------------------------------------------------------------------------------
    private BattleManager _battleManager;

    // Methods ---------------------------------------------------------------------------------------------------------
    public void OnEnter()
    {
        _battleManager.StartBattle();
    }

    public void OnExit()
    {
        _battleManager.EndBattle();
    }

    public void Tick()
    {
        Debug.Log("Tick BattleGameState");
        _battleManager.StateMachine.Tick();
    }
}
