using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DiagonalStrike : BaseAttackCard
{
    public DiagonalStrike(string name, int manaCost, Rarety rarety, CardType cardType, 
        HeroClass heroClass, int aeraOfEffect, int damage) : 
        base(name, manaCost, rarety, cardType, heroClass, aeraOfEffect, damage)
    {
    }
    
    // Methods ---------------------------------------------------------------------------------------------------------
    // public override void GetAvailableTiles()
    // {
    //     _availableTiles = _tilemapsManager.GetAvailableTilesInRange(
    //         _gridManager.WorldToCellCenter(GetStartingTile().transform.position),
    //         _aeraOfEffect, Neighbourhood.DiagonalNeighbours, false, true);
    // }
    //
    // protected override void GetAvailableTilesInCardRenderer()
    // {
    //     _availableTiles = _tilemapsManager.GetAvailableTilesInCardRenderer(
    //         _tilemapsManager.GetCardAoeRendererCenter(), _aeraOfEffect, Neighbourhood.DiagonalNeighbours,
    //         false, true);
    //
    //     _availableTiles.Remove(_availableTiles.First().Key);
    // }
}
