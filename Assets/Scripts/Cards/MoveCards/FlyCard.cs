using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyCard : BaseMoveCard
{
    public FlyCard(string name, int manaCost, Rarety rarety, CardType cardType, HeroClass heroClass, int aeraOfEffect) : base(name, manaCost, rarety, cardType, heroClass, aeraOfEffect)
    {
    }

    public override void GetAvailableTiles()
    {
        _availableTiles = _tilemapsManager.GetAvailableTilesInRange(
            _gridManager.WorldToCellCenter(GetStartingTile()),
            _aeraOfEffect, _neighboursData, false, false);
    }
}
