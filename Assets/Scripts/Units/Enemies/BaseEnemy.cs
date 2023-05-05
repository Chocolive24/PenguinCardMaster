using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum EnemyType
{
    GOBLIN,
    ARCHER,
    TANK,
    SPAWNER,
    MINION,
    MIX
}

public class BaseEnemy : BaseUnit
{
    // Attributes ------------------------------------------------------------------------------------------------------
    private Tilemap _movementTilemap;
    private Tilemap _attackTilemap;

    private bool _isSelected = false;

    [SerializeField] protected EnemyType _type;
    [SerializeField] protected int _weight;
    [SerializeField] protected int _attackRange;
    [SerializeField] protected int _maxNbrOfAttackPerTurn = 1;
    [SerializeField] protected int _maxNbrOfMovementPerTurn = 1;
    [SerializeField] protected int _minimumPathCount = 1;

    protected int _nbrOfAttackPerformed = 0;
    protected int _nbrOfMovementPerformed = 0;

    protected bool _hasFinishedTheTurn = false;



    // References ------------------------------------------------------------------------------------------------------
    private EnemyData _enemyData;
    
    private BaseEnemyBT _behaviorTree;
    

    // Events ----------------------------------------------------------------------------------------------------------
    public event Action OnTurnFinished; 
    
    //public event Action<BaseUnit, int> OnDamageTaken;
    
    // Getters and Setters ---------------------------------------------------------------------------------------------

    public Dictionary<Vector3, int> DebugDico = new Dictionary<Vector3, int>();
    
    #region Getters and Setters

    public EnemyType Type => _type;

    public int Weight => _weight;
    public int AttackRange => _attackRange;

    public int MaxNbrOfMovementPerTurn => _maxNbrOfMovementPerTurn;
    public int NbrOfMovementPerformed
    {
        get => _nbrOfMovementPerformed;
        set => _nbrOfMovementPerformed = value;
    }

    public bool CanAttack => _nbrOfAttackPerformed < _maxNbrOfAttackPerTurn;

    public bool CanMove => _nbrOfMovementPerformed < _maxNbrOfMovementPerTurn;

    public int MinimumPathCount => _minimumPathCount;

    public bool CanFinishTheTurn => !CanAttack && !CanMove;

    public bool HasFinishedTheTurn => _hasFinishedTheTurn;

    public Dictionary<Vector3, int> AvailableTiles
    {
        get => _availableTiles;
        set => _availableTiles = value;
    }

    public Tilemap MovementTilemap
    {
        get => _movementTilemap;
        set => _movementTilemap = value;
    }

    public Tilemap AttackTilemap
    {
        get => _attackTilemap;
        set => _attackTilemap = value;
    }

    public bool IsSelected
    {
        get => _isSelected;
        set => _isSelected = value;
    }
    
    public BaseEnemyBT BehaviorTree => _behaviorTree;

    #endregion
    
    // Methods ---------------------------------------------------------------------------------------------------------

    protected override void Awake()
    {
        base.Awake();
        SetData();
    }

    protected override void Start()
    {
        base.Start();
        _behaviorTree = GetComponent<BaseEnemyBT>();
        
        _nbrOfAttackPerformed = _maxNbrOfAttackPerTurn;
        _nbrOfMovementPerformed = _maxNbrOfMovementPerTurn;

        BattleManager.OnEnemyTurnStart += SetTurnValues;
    }

    private void OnDestroy()
    {
        BattleManager.OnEnemyTurnStart -= SetTurnValues;
    }

    protected override void Update()
    {
        base.Update();
        Debug.Log("can attack " + CanAttack);
        Debug.Log("can move " + CanMove);
    }

    protected override void SetData()
    {
        base.SetData();
        _enemyData = (EnemyData)_unitData;
        
        _type = _enemyData.Type;
        _weight = _enemyData.Weight;
        _attackRange = _enemyData.AttackRange;
        _maxNbrOfAttackPerTurn = _enemyData.MaxNbrOfAttackPerTurn;
        _maxNbrOfMovementPerTurn = _enemyData.MaxNbrOfMovementPerTurn;
        _minimumPathCount = _enemyData.MinimumPathCount;
    }

    private void SetTurnValues(BattleManager battleManager)
    {
        ResetTurnValues();
    }

    public void ResetTurnValues()
    {
        _nbrOfAttackPerformed = 0;
        _nbrOfMovementPerformed = 0;
        _hasFinishedTheTurn = false;
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        //OnDamageTaken?.Invoke(this, damage);
    }

    public virtual void InvokeEndTurnEvent()
    {
        _hasFinishedTheTurn = true;
        OnTurnFinished?.Invoke();
    }

    public bool FindATargetToAttack(BaseHero heroTarget)
    {
        _attackTiles = GetTilesInAttackRange(transform.position, _attackRange);

        foreach (var tile in _attackTiles)
        {
            if (_gridManager.GetTileAtPosition(tile.Key) != null)
            {
                if (_gridManager.GetTileAtPosition(tile.Key).OccupiedUnit != null)
                {
                    return _gridManager.GetTileAtPosition(tile.Key).OccupiedUnit == heroTarget;
                }
            }
        }
        
        if (!CanMove)
        {
            _nbrOfAttackPerformed = _maxNbrOfAttackPerTurn;
        }
        
        return false;
    }

    public virtual Dictionary<Vector3, int> GetTilesInAttackRange(Vector3 startPos, int range)
    {
        _attackTiles = GetAvailableTilesInRange(startPos, range,
            true, false);
            
        _attackTiles.Remove(_attackTiles.First().Key);
            
        return _attackTiles;
    }

    public void DrawAttackTiles()
    {
        _attackTiles = GetTilesInAttackRange(transform.position, _attackRange);

        foreach (var tile in DebugDico)
        {
            if (!_attackTiles.ContainsKey(tile.Key))
            {
                _attackTiles.Add(tile.Key, tile.Value);
            }
        }
        
        foreach (var tile in _attackTiles)
        {
            var pos = _attackTilemap.WorldToCell(new Vector3
                (tile.Key.x, tile.Key.y, 0));
            
            _attackTilemap.SetTile(pos, _tilemapsManager.EnemyAttackRuleTile);
        }
    }
    
    public virtual void Attack(BaseHero heroTarget)
    {
        heroTarget.TakeDamage(_currentAttack.Value);
        _nbrOfAttackPerformed++;
    }
    
    public override void FindAvailablePathToTarget(Vector3 targetPos, int minimumPathCount, 
        bool countHeroes, bool countEnemies, bool countWalls)
    {
        _availableTiles = GetAvailableTilesInRange(transform.position, _currentMovement.Value, 
            false, false);

        Vector3 startPos = _gridManager.WorldToCellCenter(transform.position);
        Vector3 endPos = _gridManager.WorldToCellCenter(targetPos);
        
        _path = FindPath(startPos, endPos, countHeroes, countEnemies, countWalls);

        if (_path.Count == minimumPathCount)
        {
            SetNoMovement();
        }
        
        if (_path.Count > minimumPathCount)
        {
            CorrectTargetPos();
            
            _avalaiblePath = _availableTiles.Keys.Intersect(_path).ToList();
        
            if (_avalaiblePath.Count > 0)
            {
                _targetPos = _avalaiblePath.First();
                _isSelected = false;
                DestroyTilemaps();
            }
            else
            {
                SetNoMovement();
            }
        }
        else
        {
            if (!countEnemies)
            {
                FindAvailablePathToTarget(targetPos, minimumPathCount, true, true, false);
            }
            else
            {
                SetNoMovement();
            }
        }
    }

    protected virtual void CorrectPathIfOnlyOneTile()
    {
        if (_path.Count == 1)
        {
            SetNoMovement();
        }
    }


    private void SetNoMovement()
    {
        _targetPos = null;
        _nbrOfMovementPerformed = _maxNbrOfMovementPerTurn;
    }

    protected virtual void CorrectTargetPos()
    {
        _path.Remove(_path.Last());
    }
    
    // protected override void FollowThePath()
    // {
    //     base.FollowThePath();
    //
    //     if (_currentTargetIndex >= _avalaiblePath.Count)
    //     {
    //         _nbrOfMovementPerformed++;
    //     }
    // }

    protected override void StopThePath()
    {
        base.StopThePath();
        _nbrOfMovementPerformed++;
    }


    public void DestroyTilemaps()
    {
        if (_movementTilemap)
        {
            Destroy(_movementTilemap.gameObject);
            Destroy(_attackTilemap.gameObject);
        }
    }
    
    protected override void Kill()
    {
        DestroyTilemaps();
        
        _unitsManager.Enemies.Remove(this);
        base.Kill();
    }
}
