using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    // Attributes ------------------------------------------------------------------------------------------------------

    #region Tweakable Values

    [SerializeField] private string _unitName;
    [SerializeField] private Faction _faction;
    [SerializeField] protected IntReference _maxHP;
    [SerializeField] protected IntReference _currentHP;
    [SerializeField] protected IntReference _baseAttack;
    [SerializeField] protected IntReference _currentAttack;
    [SerializeField] protected IntReference _shield;
    [SerializeField] protected IntReference _baseMovement;
    [SerializeField] protected IntReference _currentMovement;
    [SerializeField] protected IntReference _speed;
   
    
    #endregion
    
    // protected TileCell _occupiedTile;
    protected List<TileCell> _previousOccupiedTiles = new List<TileCell>();
    
    
    protected Vector3? _targetPos = null;
    protected Dictionary<Vector3, int> _availableTiles;
    protected Dictionary<Vector3, int> _attackTiles;
    protected List<Vector3> _path;
    protected int _currentTargetIndex = 0;
    protected bool _stopFollowingPath = false;

    protected List<Vector3> _avalaiblePath;

    protected List<Vector3> _exploringPath;
    
    // References ------------------------------------------------------------------------------------------------------
    [SerializeField] protected UnitData _unitData;
    
    protected GameManager _gameManager;
    protected GridManager _gridManager;
    protected TilemapsManager _tilemapsManager;
    protected UnitsManager _unitsManager;

    protected HealthBar _healthBar;
    
    // Events ----------------------------------------------------------------------------------------------------------
    public event Action<BaseUnit, int> OnDamageTaken;
    public event Action<BaseUnit> OnDeath;

    // Getters and Setters ---------------------------------------------------------------------------------------------

    #region Getters and Setters

    public string UnitName => _unitName;
    //public TileCell OccupiedTile { get => _occupiedTile; set => _occupiedTile = value; }

    //public List<TileCell> OccupiedTiles => GetOccupiedTiles();

    public Faction Faction => _faction;

    public IntReference MaxHp => _maxHP;
    public IntReference CurrentHp => _currentHP;

    public IntReference BaseAttackDamage => _baseAttack;

    public IntReference Movement { get => _baseMovement; }

    public List<TileCell> PreviousOccupiedTiles
    {
        get => _previousOccupiedTiles;
        set => _previousOccupiedTiles = value;
    }
    public Vector3? TargetPos
    {
        get => _targetPos;
        set => _targetPos = value;
    }

    public List<Vector3> Path
    {
        get => _path;
        set => _path = value;
    }

    public List<Vector3> AvailablePath => _avalaiblePath;

    #endregion

    // Methods ---------------------------------------------------------------------------------------------------------
    protected virtual void Awake()
    {
        SetData();
        
        _healthBar = GetComponent<HealthBar>();
    }

    protected virtual void Start()
    {
        _currentHP.SetValue(_maxHP.Value);
        _currentAttack.SetValue(_baseAttack.Value);
        _currentMovement.SetValue(_baseMovement.Value);
        
        _availableTiles = new Dictionary<Vector3, int>();
        _path = new List<Vector3>();
        _avalaiblePath = new List<Vector3>();
        
        _gameManager = GameManager.Instance;
        _gridManager = GridManager.Instance;
        _tilemapsManager = TilemapsManager.Instance;
        _unitsManager = UnitsManager.Instance;
    }

    protected virtual void Update()
    {
        //MoveOnGrid();
    }

    protected virtual void SetData()
    { 
        _unitName = _unitData.UnitName;
        _faction = _unitData.Faction;
        _maxHP = _unitData.MaxHP;
        _baseAttack = _unitData.Attack;
        _baseMovement = _unitData.Movement;
        _speed = _unitData.Speed;
    }
    
    public virtual void TakeDamage(int damage)
    {
        _currentHP.SubstractValue(damage);
        
        _healthBar.UpdateHealthBar(_currentHP.Value, _maxHP.Value);
        
        OnDamageTaken?.Invoke(this, damage);
        
        if (_currentHP.Value <= 0)
        {
            Kill();
        }
    }

    public virtual List<TileCell> GetOccupiedTiles()
    {
        return new List<TileCell>{GridManager.Instance.GetTileAtPosition(transform.position)};
    }

    // public virtual Dictionary<Vector3, int> GetAvailableTiles(Vector3 startPos, int range, 
    //                                                             bool countHeroes, bool countEnemies)
    // {
    //     return GetAvailableTilesInRange(startPos, range, countHeroes, countEnemies);
    // }

    public virtual void FindAvailablePathToTarget(Vector3 targetPos, int minimumPathCount, 
        bool countHeroes, bool countEnemies, bool countWalls)
    {
        _path = FindPath(transform.position, targetPos, countHeroes, countEnemies, countWalls);
        
        if (_path.Count > 0)
        {
            _avalaiblePath = _availableTiles.Keys.Intersect(_path).ToList();
        
            _targetPos = _avalaiblePath.First();
        }
        else
        {
            _targetPos = null;
        }
    }
    
    public virtual void MoveOnGrid()
    {
        if (_targetPos.HasValue)
        {
            // Move the player to the target position
            transform.position = Vector3.MoveTowards(transform.position, _targetPos.Value,
                _speed.Value * Time.deltaTime);

            // The distance between the player and the target point would never be exactly equal to 0.
            // So we check with an Epsilon value if the player as reached the target position.
            // Then we set his position to the target position in order to be precise.
            if (Vector3.Distance(transform.position, _targetPos.Value) <= 0.01f)
            {
                transform.position = _targetPos.Value;
                _targetPos = null;

                // if (_gameManager.IsInBattleState)
                // {

                List<Vector3> pathToFollow = _gameManager.IsInBattleState ? _avalaiblePath : _exploringPath;
                FollowThePath(pathToFollow);
                //}
            }
        }
    }

    protected virtual void FollowThePath(List<Vector3> path)
    {
        if (_currentTargetIndex < path.Count - 1) 
        {
            _currentTargetIndex++;

            var nextTile = _gridManager.GetTileAtPosition(path[_currentTargetIndex]);
            
            if (nextTile.OccupiedUnit || GetOccupiedTiles().Contains(nextTile))
            {
                StopThePath();
            }
            else
            {
                _targetPos = _gridManager.WorldToCellCenter(path[_currentTargetIndex]);
            }
        }
        
        else
        {
            StopThePath();
        }
    }

    protected virtual void StopThePath()
    {
        foreach (var tile in _previousOccupiedTiles)
        {
            tile.OccupiedUnit = null;
        }

        _previousOccupiedTiles = GetOccupiedTiles();
        
        foreach (var tile in GetOccupiedTiles())
        {
            tile.SetUnit(this);
        }
        
        //_gridManager.GetTileAtPosition(transform.position).SetUnit(this);
        _targetPos = null;
        _currentTargetIndex = 0;
        _path.Clear();
        _avalaiblePath.Clear();
        if (_exploringPath != null)
        {
            _exploringPath.Clear();
        }
    }

    public virtual void DisplayStats()
    {
        Debug.Log(_maxHP + " " +  _baseAttack + " " + _shield);
    }
    
    protected virtual void Kill()
    {
        OnDeath?.Invoke(this);
        Destroy(this.gameObject);
    }
    
    public virtual Dictionary<Vector3, int> GetAvailableTilesInRange(Vector3 startPos, int range, 
        bool countHeroes, bool countEnemies)
    {
        return _tilemapsManager.GetAvailableTilesInRange(startPos, range, 
            Neighbourhood.CardinalNeighbours, countHeroes, countEnemies);
    }
    
    public virtual List<Vector3> FindPath(Vector3 startPos, Vector3 endPos, bool countHeroes, 
                                            bool countEnemies, bool countWalls)
    {
        List<Vector3> openList = new List<Vector3>();
        List<Vector3> closedList = new List<Vector3>();
        
        openList.Add(startPos);

        while (openList.Count > 0)
        {
            Vector3 currentPos = openList.OrderBy(x => _gridManager.GetTileAtPosition(x).F).First();

            openList.Remove(currentPos);
            closedList.Add(currentPos);

            if (currentPos == endPos)
            {
                return RestrucutrePath(startPos, endPos);
            }
            
            foreach (var direction in new Vector3[] { Vector3.up, Vector3.down, Vector3.left, Vector3.right })
            {
                var neighbor = _gridManager.WorldToCellCenter(currentPos + direction);

                if (!IsPositionAvailable(neighbor, countHeroes, countEnemies, countWalls) || closedList.Contains(neighbor))
                {
                    continue;
                }
                
                _gridManager.GetTileAtPosition(neighbor).G = GetManhattenDistance(startPos, neighbor);
                _gridManager.GetTileAtPosition(neighbor).H = GetManhattenDistance(endPos, neighbor);

                _gridManager.GetTileAtPosition(neighbor).PreviousTilePos = currentPos;

                if (!openList.Contains(neighbor))
                {
                    openList.Add(neighbor);
                }
            }
        }

        return new List<Vector3>();
    }
    
    private int GetManhattenDistance(Vector3 startPos, Vector3 neighbor)
    {
        return (int)(Mathf.Abs(startPos.x - neighbor.x) + Mathf.Abs(startPos.y - neighbor.y));
    }
    
    protected virtual List<Vector3> RestrucutrePath(Vector3 startPos, Vector3 endPos)
    {
        List<Vector3> finishedList = new List<Vector3>();

        Vector3 currentPos = endPos;

        while (currentPos != startPos)
        {
            finishedList.Add(currentPos);
            currentPos = _gridManager.GetTileAtPosition(currentPos).PreviousTilePos;
        }

        finishedList.Reverse();
        
        return finishedList;
    }
    
    public virtual bool IsPositionAvailable(Vector3 position, bool countHeroes, bool countEnemies, bool countWalls)
    {
        TileCell tile = _gridManager.GetTileAtPosition(position);
        
        if (tile)
        {
            if (countWalls)
            {
                return true;
            }
            
            if (countHeroes && tile.OccupiedUnit != null)
            {
                if (tile.OccupiedUnit.Faction == Faction.Hero)
                {
                    return true;
                }
                
            }
            if (countEnemies && tile.OccupiedUnit != null)
            {
                if (tile.OccupiedUnit.Faction == Faction.Enemy)
                {
                    return true;
                }
            }

            return tile.Walkable;
        }

        return false;
    }

    public int CalculDistanceFromSelf(Vector3 endPos, bool countHeroes, bool countEnemies, bool countWalls)
    {
        _path = FindPath(transform.position, endPos, 
            countHeroes, countEnemies, countWalls);
        return _path.Count;
    }
}
