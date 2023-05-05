using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingStrike : BaseAttackCard
{
    public HealingStrike(string name, int manaCost, Rarety rarety, CardType cardType, HeroClass heroClass, 
        int aeraOfEffect, int baseDamage) : base(name, manaCost, rarety, cardType, heroClass, aeraOfEffect, baseDamage)
    {
    }

    [SerializeField] private int _heal;

    protected override void Start()
    {
        base.Start();

        _cardEffectTxt.text += "Heal " + _heal + " HP";
    }

    protected override void UpdateText()
    {
        base.UpdateText();
        _cardEffectTxt.text += "Heal " + _heal + " HP";
    }

    public override void ActivateCardEffect(TileCell tile)
    {
        base.ActivateCardEffect(tile);
        
        if (_hasPerformed)
        {
            _unitsManager.HeroPlayer.HealHp(_heal);
        }
    }
}
