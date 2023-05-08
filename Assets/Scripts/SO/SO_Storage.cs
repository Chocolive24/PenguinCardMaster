using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "SO Storage/Datas", fileName = "Datas")]
public class SO_Storage : ScriptableObject
{

    [SerializeField] private int _intValue;
    [SerializeField] private float _floatValue;
    [SerializeField] private string _stringValue;
    
    public int INTValues => _intValue;
    public float FloatValue => _floatValue;
    public string StringValue => _stringValue;

    public void AddToInt(int step)
    {
        _intValue += step;
    }
    public void AddToFloat(float step)
    {
        _floatValue += step;
    }

}
