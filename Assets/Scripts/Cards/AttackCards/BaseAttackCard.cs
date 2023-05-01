using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BaseAttackCard : BaseCard
{
    public BaseAttackCard(string name, int manaCost, Rarety rarety, CardType cardType, HeroClass heroClass, 
                        int aeraOfEffect,int damage) 
        : base(name, manaCost, rarety, cardType, heroClass, aeraOfEffect)
    {
        _damage = damage;
    }

    // Attributes ------------------------------------------------------------------------------------------------------
    [SerializeField] private int _damage;

    private bool _hasPerformed;
    
    // References ------------------------------------------------------------------------------------------------------

    // Getters and Setters ---------------------------------------------------------------------------------------------
    public int Damage => _damage;

    // Methods ---------------------------------------------------------------------------------------------------------
    
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        _cardEffectTxt.text = "Deal " + _damage + "\n damage \n";
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override void ActivateCardEffect(TileCell tile)
    {
        if (tile.OccupiedUnit)
        {
            if (tile.OccupiedUnit.Faction == Faction.Enemy)
            {
                var enemy = (BaseEnemy)tile.OccupiedUnit;

                enemy.IsSelected = !enemy.IsSelected;

                if (_unitsManager.HeroPlayer)
                {
                    if (_availableTiles.ContainsKey(tile.transform.position) && enemy != null)
                    {
                        enemy.IsSelected = false;
                        enemy.TakeDamage(_damage);
                        _hasPerformed = true;
                    }
                }
            }
        }
    }

    protected override bool CheckIfHasPerformed()
    {
        return _hasPerformed;
    }

    public override void DrawTilemap(Dictionary<Vector3, int> availableNeighbours, Tilemap tilemap, RuleTile ruleTile)
    {
        if (availableNeighbours.ContainsKey(_gridManager.WorldToCellCenter(
                GetStartingTile().transform.position)))
        {
            availableNeighbours.Remove(_gridManager.WorldToCellCenter(
                GetStartingTile().transform.position));
        }
        
        base.DrawTilemap(availableNeighbours, tilemap, ruleTile);
    }

    public override void ResetProperties()
    {
        _hasPerformed = false;
    }
}
