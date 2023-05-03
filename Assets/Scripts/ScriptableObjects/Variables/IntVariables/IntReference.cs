using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class IntReference
{
    [SerializeField] private bool _useConstant = false;
    [SerializeField] private int _baseValue;
    public int ConstantRunValue;
    
    public IntVariable Variable;

    public int Value { get => _useConstant ? ConstantRunValue : Variable.RunValue; }

    // Events ----------------------------------------------------------------------------------------------------------
    public event Action OnValueChanged;
    // Getters and Setters ---------------------------------------------------------------------------------------------
    public bool UseConstant => _useConstant;

    // Methods ---------------------------------------------------------------------------------------------------------
    public void SetValue(int newValue)
    {
        if (_useConstant)
        {
            ConstantRunValue = newValue;
        }
        else
        {
            Variable.RunValue = newValue;
        }
        
        OnValueChanged?.Invoke();
    }

    public void AddValue(int value)
    {
        if (_useConstant)
        {
            ConstantRunValue += value;
        }
        else
        {
            Variable.RunValue += value;
        }
        
        OnValueChanged?.Invoke();
    }
    
    public void SubstractValue(int value)
    {
        if (_useConstant)
        {
            ConstantRunValue -= value;
        }
        else
        {
            Variable.RunValue -= value;
        }
        
        OnValueChanged?.Invoke();
    }

    public void ResetValue()
    {
        SetValue(_baseValue);
    }
    
}
