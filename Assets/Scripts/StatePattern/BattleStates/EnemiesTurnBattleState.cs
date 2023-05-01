using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesTurnBattleState : IState
{
    public void OnEnter()
    {
        BattleManager.Instance.StartEnemiesTurn();
        Debug.Log("Enter Enemy");
        
    }

    public void OnExit()
    {
        BattleManager.Instance.ExitEnemiesTurn();
        Debug.Log("Exit Enemy");
    }

    public void Tick()
    {
        Debug.Log("Tick Enemy");
    }
}
