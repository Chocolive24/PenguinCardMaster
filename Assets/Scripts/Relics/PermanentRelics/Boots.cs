using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boots : PermanentRelic
{
    [SerializeField] private IntReference _playerMovement;
    [SerializeField] private int _movementToAdd;

    protected override void Start()
    {
        base.Start();
        _valueToBuff = _playerMovement;
        _valueToAdd = _movementToAdd;
    }
}
