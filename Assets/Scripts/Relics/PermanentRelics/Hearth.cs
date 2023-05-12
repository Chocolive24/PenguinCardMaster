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

    protected override void OnDestroy()
    {
        base.OnDestroy();
        BattleManager.OnBattleEnd -= HealPlayer;
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
        if (_unitsManager.HeroPlayer && _isCollected)
        {
            _unitsManager.HeroPlayer.HealHp(_heal);
        }
    }
}
