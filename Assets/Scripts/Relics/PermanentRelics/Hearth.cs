using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hearth : Relic
{
    [SerializeField] protected IntReference _playerCurrentHp;
    [SerializeField] private int _heal;

    private UnitsManager _unitsManager;

    protected override void Awake()
    {
        base.Awake();
        BattleManager.OnBattleEnd += HealPlayer;
    }

    protected override void Start()
    {
        base.Start();
        _unitsManager = UnitsManager.Instance;
    }

    protected override void PerformEffect()
    {
        
    }

    private void HealPlayer(BattleManager arg1, RoomData arg2)
    {
        _unitsManager.HeroPlayer.HealHp(_heal);
    }
}
