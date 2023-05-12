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

    [SerializeField] protected Vector2 _baseSizeDelta;

    // References ------------------------------------------------------------------------------------------------------
    [SerializeField] protected Button _button;

    [SerializeField] protected RelicData _relicDataRef;

    [SerializeField] private GameObject _effectBox;

    // Events ----------------------------------------------------------------------------------------------------------
    //public event Action<Relic> OnCollected;
    
    // Getters and Setters ---------------------------------------------------------------------------------------------
    public RelicData RelicDataRef => _relicDataRef;
    
    public GameObject EffectBox => _effectBox;

    public Vector2 BaseSizeDelta => _baseSizeDelta;

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
        if (_isCollected && _relicType == RelicData.RelicType.ACTIVABLE)
        {
            PerformEffect();
        }
    }

    public void OnRelicTriggerEnter()
    {
        if (_isCollected)
        {
            _effectBox.SetActive(true);
        }
    }
    
    public void OnRelicTriggerExit()
    {
        if (_isCollected)
        {
            _effectBox.SetActive(false);
        }
    }
    
    protected abstract void PerformEffect();
}
