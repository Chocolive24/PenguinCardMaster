using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookOfMana : PermanentRelic
{
    [SerializeField] private IntReference _playerMaxMana;
    [SerializeField] private int _manaToAdd;

    protected override void Start()
    {
        base.Start();
        _valueToBuff = _playerMaxMana;
        _valueToAdd = _manaToAdd;
    }
}
