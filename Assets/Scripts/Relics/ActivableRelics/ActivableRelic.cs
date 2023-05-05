using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivableRelic : Relic
{
    protected int _valueToChange;
    
    protected bool _hasBeenActivated = false;
    
    // Events ----------------------------------------------------------------------------------------------------------
    public static event Action<ActivableRelic> OnPerformEffect;
    
    // Methods ---------------------------------------------------------------------------------------------------------
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
            
            OnPerformEffect?.Invoke(this);
        }
    }
}
