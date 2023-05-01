using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryBattleState : IState
{
    public VictoryBattleState(BattleManager battleManager)
    {
        _battleManager = battleManager;
    }
    
    private BattleManager _battleManager;
    
    // Methods ---------------------------------------------------------------------------------------------------------
    public void OnEnter()
    {
        _battleManager.EnterVictory();
    }

    public void OnExit()
    {
        _battleManager.ExitVictory();
    }

    public void Tick()
    {
        // idk
    }
}
