using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class IntReference
{
    [SerializeField] private bool _useConstant = false;
    public int ConstantValue;
    public IntVariable Variable;

    public int Value { get => _useConstant ? ConstantValue : Variable.Value; }

    // Events ----------------------------------------------------------------------------------------------------------

    // Getters and Setters ---------------------------------------------------------------------------------------------
    public bool UseConstant => _useConstant;

    // Methods ---------------------------------------------------------------------------------------------------------
    public void SetValue(int newValue)
    {
        if (_useConstant)
        {
            ConstantValue = newValue;
        }
        else
        {
            Variable.Value = newValue;
        }
    }

    public void AddValue(int value)
    {
        if (_useConstant)
        {
            ConstantValue += value;
        }
        else
        {
            Variable.Value += value;
        }
    }
    
    public void SubstractValue(int value)
    {
        if (_useConstant)
        {
            ConstantValue -= value;
        }
        else
        {
            Variable.Value -= value;
        }
    }
    
    
    
}
