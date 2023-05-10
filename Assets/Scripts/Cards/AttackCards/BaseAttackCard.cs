using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BaseAttackCard : BaseCard
{
    public BaseAttackCard(string name, int manaCost, Rarety rarety, CardType cardType, HeroClass heroClass, 
                        int aeraOfEffect,int baseDamage) 
        : base(name, manaCost, rarety, cardType, heroClass, aeraOfEffect)
    {
        _baseDamage = baseDamage;
    }

    // Attributes ------------------------------------------------------------------------------------------------------
    [SerializeField] private IntReference _playerAttack;
    [SerializeField] private int _baseDamage;

    public int CurrentDamage => _baseDamage + _playerAttack.Value;
    
    protected bool _hasPerformed;
    
    // References ------------------------------------------------------------------------------------------------------
    [SerializeField] private GameObject _swordSlashPrefab;
    [SerializeField] private float _swordSlashLifeTime = 0.5f;

    // Events ----------------------------------------------------------------------------------------------------------
    public static event Action OnAttackCardPlayed; 

    // Getters and Setters ---------------------------------------------------------------------------------------------
    public int BaseDamage => _baseDamage;

    // Methods ---------------------------------------------------------------------------------------------------------
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    protected override void UpdateText()
    {
        base.UpdateText();
        _cardEffectTxt.text = "Deal " + CurrentDamage + "\n damage \n";
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        _cardEffectTxt.text = "Deal " + CurrentDamage + "\n damage \n";
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override void ActivateCardEffect(TileCell tile)
    {
        List<GameObject> swordSlashes = new List<GameObject>();
        List<TileCell> tiles = new List<TileCell>();

        if (tile.OccupiedUnit)
        {
            if (_cardType == CardType.BASE_ATTACK_CARD && tile.OccupiedUnit.Faction == Faction.Enemy)
            {
                var enemy = (BaseEnemy)tile.OccupiedUnit;

                enemy.IsSelected = !enemy.IsSelected;

                if (_unitsManager.HeroPlayer)
                {
                    if (_availableTiles.ContainsKey(tile.transform.position) && enemy != null)
                    {
                        enemy.TakeDamage(CurrentDamage);
                        
                        tiles.Add(tile);
                        
                        HandleSwordSlashes(tiles);
                        _hasPerformed = true;
                        
                        OnAttackCardPlayed?.Invoke();
                    }
                }
            }
        }
        if (_cardType == CardType.AOE_ATTACK_CARD && _availableTiles.ContainsKey(tile.transform.position))
        {
            if (_availableTiles.ContainsKey(tile.transform.position) || tile.OccupiedUnit)
            {
                if (_unitsManager.HeroPlayer)
                {
                    List<BaseEnemy> allreadyAttackedEnemies = new List<BaseEnemy>();

                    var baseEnemy = (BaseEnemy)tile.OccupiedUnit;

                    if (baseEnemy)
                    {
                        baseEnemy.IsSelected = !baseEnemy.IsSelected;
                    }
                    
                    foreach (var attackTile in _availableTiles)
                    {
                        var enemy = (BaseEnemy)_gridManager.GetTileAtPosition(attackTile.Key).OccupiedUnit;

                        if (enemy)
                        {
                            if (!allreadyAttackedEnemies.Contains(enemy))
                            {
                                enemy.TakeDamage(CurrentDamage);
                                
                                tiles.Add(_gridManager.GetTileAtPosition(attackTile.Key));
                                
                                allreadyAttackedEnemies.Add(enemy);
                            }
                        }
                    }

                    HandleSwordSlashes(tiles);
                    _hasPerformed = true;
                    OnAttackCardPlayed?.Invoke();
                }
            }
        }
    }

    private void HandleSwordSlashes(List<TileCell> tiles)
    {
        foreach (var tile in tiles)
        {
            Instantiate(_swordSlashPrefab, tile.transform.position, Quaternion.identity);
        }
    }

    protected override bool CheckIfHasPerformed()
    {
        return _hasPerformed;
    }

    public override void DrawTilemap(Dictionary<Vector3, int> availableNeighbours, Tilemap tilemap, RuleTile ruleTile)
    {
        if (availableNeighbours.ContainsKey(_gridManager.WorldToCellCenter(
                GetStartingTile())))
        {
            availableNeighbours.Remove(_gridManager.WorldToCellCenter(
                GetStartingTile()));
        }
        
        base.DrawTilemap(availableNeighbours, tilemap, ruleTile);
    }

    public override void ResetProperties()
    {
        _hasPerformed = false;
    }
}
