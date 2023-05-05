using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PermanentRelic : Relic
{
    protected IntReference _valueToBuff;
     protected int _valueToAdd;
    
    protected override void Awake()
    {
        base.Awake();

        OnCollected += AddPermanentBuff;
    }

    private void AddPermanentBuff(Collectible obj)
    {
        if (obj == this)
        {
            _valueToBuff.AddValue(_valueToAdd);
        }
    }

    protected override void PerformEffect()
    {
        
    }
}
