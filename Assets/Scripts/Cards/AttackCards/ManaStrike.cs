using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaStrike : BaseAttackCard
{
    public ManaStrike(string name, int manaCost, Rarety rarety, CardType cardType, HeroClass heroClass, 
        int aeraOfEffect, int baseDamage) : base(name, manaCost, rarety, cardType, heroClass, aeraOfEffect, baseDamage)
    {
    }
    
    [SerializeField] private int _manaToAdd;
        
    protected override void Start()
    {
        base.Start();

        _cardEffectTxt.text += "Gain " + _manaToAdd + " Mana";
    }
    
    public override void ActivateCardEffect(TileCell tile)
    {
        base.ActivateCardEffect(tile);
        
        if (_hasPerformed)
        {
            _unitsManager.HeroPlayer.CurrentMana += _manaToAdd;
        }
    }

        
}
