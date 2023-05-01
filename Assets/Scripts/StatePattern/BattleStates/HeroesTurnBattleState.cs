using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroesTurnBattleState : IState
{
    public HeroesTurnBattleState(BattleManager battleManager, UIBattleManager uiBattleManager)
    {
        _battleManager = battleManager;
        _uiBattleManager = uiBattleManager;
    }

    private BattleManager _battleManager;
    private UIBattleManager _uiBattleManager;
    
    // Methods ---------------------------------------------------------------------------------------------------------
    public void OnEnter()
    {
        _battleManager.EnterHeroesTurn();
    }

    public void OnExit()
    {
        _battleManager.ExitHeroesTurn();
        Debug.Log("Exit Hero");
    }

    public void Tick()
    {
        Debug.Log("Tick Hero");
    }
}
