using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BaseMoveCard : BaseCard
{
    public BaseMoveCard(string name, int manaCost, Rarety rarety, CardType cardType, HeroClass heroClass,
                        int aeraOfEffect)
        : base(name, manaCost, rarety, cardType, heroClass, aeraOfEffect)
    {
        
    }

    // Attributes ------------------------------------------------------------------------------------------------------
    private Vector3? _targetPos;
    
    private Dictionary<Vector3, int> _path;
    private Tilemap _pathTilemap;
    
    // Getters and Setters ---------------------------------------------------------------------------------------------

    #region Getters and Setters

    public Dictionary<Vector3, int> Path
    {
        get => _path;
        set => _path = value;
    }

    public Tilemap PathTilemap
    {
        get => _pathTilemap;
        set => _pathTilemap = value;
    }

    #endregion
    
    // Methods ---------------------------------------------------------------------------------------------------------
    protected override void Awake()
    {
        base.Awake();
        _cardEffectTxt.text = "Move " + _aeraOfEffect + "\n squares \n";
    }

    protected override void Start()
    {
        base.Start();
        _path = new Dictionary<Vector3, int>();
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void ActivateCardEffect(TileCell tile)
    {
        if (!tile.OccupiedUnit)
        {
            //Check if we have a selected hero and if we have played a moveCard.
            if (_unitsManager.HeroPlayer)
            {
                // If so, move the hero to the tile where the player clicked if it is in the range of the aoe moveCard
                // and it is walkable.
                if (_availableTiles.ContainsKey(tile.transform.position) && tile.Walkable)
                {
                    _targetPos = tile.transform.position;

                    if (_targetPos.HasValue)
                    {
                        _unitsManager.HeroPlayer.FindAvailablePathToTarget(_targetPos.Value, 0,
                            false, false, false);
                    }
                }
            }
        }
    }

    protected override bool CheckIfHasPerformed()
    {
        if (_targetPos.HasValue)
        {
            return Vector3.Distance(_unitsManager.HeroPlayer.transform.position, _targetPos.Value) < 0.1f;
        }

        return false;
    }

    public override void GetAvailableTiles()
    {
        _availableTiles = _tilemapsManager.GetAvailableTilesInRange(
            _gridManager.WorldToCellCenter(GetStartingTile().transform.position),
            _aeraOfEffect, Neighbourhood.CardinalNeighbours, false, false);
    }

    public override void ResetProperties()
    {
        _targetPos = null;
    }
}
