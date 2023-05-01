using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiagonalStrike : BaseAttackCard
{
    public DiagonalStrike(string name, int manaCost, Rarety rarety, CardType cardType, 
        HeroClass heroClass, int aeraOfEffect, int damage) : 
        base(name, manaCost, rarety, cardType, heroClass, aeraOfEffect, damage)
    {
    }
    
    // Methods ---------------------------------------------------------------------------------------------------------
    public override void GetAvailableTiles()
    {
        
    }
}
