using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public abstract class Relic : Collectible
{
    // Attributes ------------------------------------------------------------------------------------------------------
    protected RelicData.RelicType _relicType;

    // References ------------------------------------------------------------------------------------------------------
    [SerializeField] protected Button _button;

    [SerializeField] protected RelicData _relicDataRef;

    [SerializeField] private GameObject _effectBox;

    // Events ----------------------------------------------------------------------------------------------------------
    //public event Action<Relic> OnCollected;
    
    // Getters and Setters ---------------------------------------------------------------------------------------------
    public RelicData RelicDataRef => _relicDataRef;
    
    public GameObject EffectBox => _effectBox;

    //Methods ----------------------------------------------------------------------------------------------------------
    protected virtual void Awake()
    {
        base.Awake();
    }

    protected virtual void OnDestroy()
    {
        base.OnDestroy();
    }
    
    protected virtual void Start()
    {
        _relicType = _relicDataRef.Type;
    }

    public void OnRelicClick()
    {
        // if (!_isCollected)
        // {
        //     _isCollected = true;
        //     OnCollected?.Invoke(this);
        // }
        if (_isCollected && _relicType == RelicData.RelicType.ACTIVABLE)
        {
            PerformEffect();
        }
    }

    protected abstract void PerformEffect();
}
