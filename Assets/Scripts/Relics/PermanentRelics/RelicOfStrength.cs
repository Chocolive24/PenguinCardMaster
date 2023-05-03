using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelicOfStrength : Relic
{
    [SerializeField] private IntReference _playerAttack;
    [SerializeField] private int _damageToAdd;
    
    protected override void Awake()
    {
        base.Awake();

        OnCollected += AddPermanentBuff;
    }

    private void AddPermanentBuff(Collectible obj)
    {
        if (obj == this)
        {
            _playerAttack.AddValue(_damageToAdd);
        }
    }

    protected override void PerformEffect()
    {
        
    }
}
