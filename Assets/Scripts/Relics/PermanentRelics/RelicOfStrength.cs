using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelicOfStrength : PermanentRelic
{
    [SerializeField] private IntReference _playerAttack;
    [SerializeField] private int _damageToAdd;

    protected override void Start()
    {
        base.Start();
        _valueToBuff = _playerAttack;
        _valueToAdd = _damageToAdd;
    }
}
