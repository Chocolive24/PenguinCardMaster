using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Relic : MonoBehaviour
{
    // Attributes ------------------------------------------------------------------------------------------------------
    protected RelicData.RelicType _type;

    protected bool _canBeCollected = false;
    protected bool _isCollected = false;
    
    // References ------------------------------------------------------------------------------------------------------
    [SerializeField] protected Button _button;

    [SerializeField] protected RelicData _relicDataRef;

    [SerializeField] private GameObject _effectBox;

    // Events ----------------------------------------------------------------------------------------------------------
    public event Action<Relic> OnCollected;
    
    // Getters and Setters ---------------------------------------------------------------------------------------------
    public RelicData RelicDataRef => _relicDataRef;

    public bool IsCollected
    {
        get => _isCollected;
        set => _isCollected = value;
    }

    public GameObject EffectBox => _effectBox;

    //Methods ----------------------------------------------------------------------------------------------------------
    protected virtual void Start()
    {
        _type = _relicDataRef.Type;
    }

    public void OnClick()
    {
        if (!_isCollected)
        {
            _isCollected = true;
            OnCollected?.Invoke(this);
        }
        else if (_type == RelicData.RelicType.ACTIVABLE)
        {
            PerformEffect();
        }
    }

    protected abstract void PerformEffect();
}
