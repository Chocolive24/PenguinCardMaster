using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RescueMana : Relic
{
    [SerializeField] private int _manaToRestore = 3;
    private bool _hasBeenActivated = false;
    
    // Events ----------------------------------------------------------------------------------------------------------
    public static event Action<RescueMana, int> OnPerformEffect;
    
    protected override void Awake()
    {
        base.Awake();
        BattleManager.OnBattleStart += SetValues;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        BattleManager.OnBattleStart -= SetValues;
    }


    protected override void Start()
    {
        base.Start();
    }

    private void SetValues(BattleManager arg1, RoomData arg2)
    {
        _hasBeenActivated = false;
        _button.interactable = true;
    }
    
    protected override void PerformEffect()
    {
        if (!_hasBeenActivated)
        {
            _hasBeenActivated = true;

            _button.interactable = false;
            
            OnPerformEffect?.Invoke(this, _manaToRestore);
        }
    }
}
