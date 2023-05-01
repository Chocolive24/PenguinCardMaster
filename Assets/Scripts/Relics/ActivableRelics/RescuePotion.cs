using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RescuePotion : Relic
{
    private bool _hasBeenActivated = false;
    
    // References ------------------------------------------------------------------------------------------------------
    [SerializeField] private int _heal = 10;

    // Events ----------------------------------------------------------------------------------------------------------
    public static event Action<RescuePotion, int> OnPerformEffect;
    
    // Methods ---------------------------------------------------------------------------------------------------------
    private void Awake()
    {
        BattleManager.OnBattleStart += SetValues;
    }

    private void OnDestroy()
    {
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
            
            OnPerformEffect?.Invoke(this, _heal);
        }
    }
}
