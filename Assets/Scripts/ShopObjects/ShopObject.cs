using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopObject : MonoBehaviour
{
    private int _objectCost;

    private BaseCard _cardRef;
    private Relic _relicRef;
    
    [SerializeField] private IntReference _playerGolds;

    // Events ----------------------------------------------------------------------------------------------------------
    public static event Action<ShopObject> OnBuy; 

    // Getters and Setters ---------------------------------------------------------------------------------------------
    public int ObjectCost
    {
        get => _objectCost;
        set => _objectCost = value;
    }

    public BaseCard CardRef
    {
        get => _cardRef;
        set => _cardRef = value;
    }

    public Relic RelicRef
    {
        get => _relicRef;
        set => _relicRef = value;
    }

    // Methods ---------------------------------------------------------------------------------------------------------
    public void OnClick()
    {
        if (_objectCost <= _playerGolds.Value)
        {
            OnBuy?.Invoke(this);
            _cardRef = null;
            _relicRef = null;
        }
    }
}
