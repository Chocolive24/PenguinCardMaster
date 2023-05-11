using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class RoomData : ScriptableObject
{
    public enum RoomType
    {
        START,
        BASIC,
        SHOP,
        END
    }

    // Attributes ------------------------------------------------------------------------------------------------------
    #region Positions Attributes

    [SerializeField] private BoundsInt _bounds;
    [SerializeField] private List<Vector3Int> _tilePositions;
    [SerializeField] private Dictionary<Vector3Int, RoomData> _roomNeighbours;

    [SerializeField] private List<RoomData> _roomNeighboursData;

    [SerializeField]private int _distanceFromStart;

    private Dictionary<Vector3, Neighbourhood.Direction> _doorsData;
    [SerializeField] private List<Vector3> _doorsDataPos;
    [SerializeField] private List<Neighbourhood.Direction> _doorsDataDir;

    [SerializeField]private List<DoorTileCell> _doors;

    [SerializeField] private List<Vector3Int> _wallsPositions;

    #endregion;
    
    #region Enemies Attributes
    
    [SerializeField]private int _enemySpawnWeight;

    [SerializeField]private bool _hasEnemiesToFight;
    

    #endregion
    
    [SerializeField]private RoomType _type;

    // Getters and Setters ---------------------------------------------------------------------------------------------

    #region Getters and Setters

    public BoundsInt Bounds
    {
        get => _bounds;
        set => _bounds = value;
    }

    public int EnemySpawnWeight
    {
        get => _enemySpawnWeight;
        set => _enemySpawnWeight = value;
    }

    public bool HasEnemiesToFight
    {
        get => _hasEnemiesToFight;
        set => _hasEnemiesToFight = value;
    }

    public RoomType Type => _type;

    public int DistanceFromStart
    {
        get => _distanceFromStart;
        set => _distanceFromStart = value;
    }

    public Dictionary<Vector3Int, RoomData> RoomNeighbours => _roomNeighbours;

    public List<RoomData> RoomNeighboursData => _roomNeighboursData;

    public List<Vector3Int> DoorPositions => new List<Vector3Int>()
    {
        // Up Door.
        new (_bounds.xMin + (_bounds.size.x / 2), _bounds.yMax - 1, 0),
        // Right Door
        new (_bounds.xMax - 1, _bounds.yMin + (_bounds.size.y / 2), 0),
        // Left Door
        new (_bounds.xMin, _bounds.yMin + (_bounds.size.y / 2), 0),
        // Down Door
        new (_bounds.xMin + (_bounds.size.x / 2), _bounds.yMin, 0),
    };

    public Dictionary<Vector3, Neighbourhood.Direction>  DoorsData => _doorsData;

    public List<Vector3> DoorsDataPos => _doorsDataPos;

    public List<Neighbourhood.Direction> DoorsDataDir => _doorsDataDir;

    public List<Vector3Int> WallsPositions => _wallsPositions;

    public List<Vector3Int> TilePositions => _tilePositions;

    public List<DoorTileCell> Doors => _doors;

    #endregion

    // Methods ---------------------------------------------------------------------------------------------------------
    public void Init(BoundsInt bounds, int distanceFromStart, int enemySpawnWeight, bool hasEnemiesToFight)
    {
        _bounds = bounds;
        _distanceFromStart = distanceFromStart;
        _enemySpawnWeight = enemySpawnWeight;
        _hasEnemiesToFight = hasEnemiesToFight;

        _roomNeighbours = new Dictionary<Vector3Int, RoomData>();

        if (_roomNeighboursData == null)
        {
            _roomNeighboursData = new List<RoomData>();
        }
        
        _doorsData = new Dictionary<Vector3, Neighbourhood.Direction>();

        if (_doorsDataPos == null)
        {
            _doorsDataPos = new List<Vector3>();
        }

        if (_doorsDataDir == null)
        {
            _doorsDataDir = new List<Neighbourhood.Direction>();
        }
        
        _doors = new List<DoorTileCell>();
        SetDoorsOpen(false);

        _wallsPositions = new List<Vector3Int>();

        _tilePositions = new List<Vector3Int>();
    }
    
    public void SetType(RoomType type, Tilemap rndWallPattern)
    {
        _type = type;

        switch (_type)
        {
            case RoomType.START:
                _hasEnemiesToFight = false;
                SetDoorsOpen(true);
                SetAllTilePositions();
                break;
            case RoomType.BASIC:
                _hasEnemiesToFight = true;
                SetDoorsOpen(false);
                SetWallPositions(rndWallPattern);
                SetAllTilePositions();
                break;
            case RoomType.SHOP:
                _hasEnemiesToFight = false;
                SetDoorsOpen(true);
                SetAllTilePositions();
                break;
            case RoomType.END:
                _hasEnemiesToFight = true;
                SetDoorsOpen(false);
                SetAllTilePositions();
                break;
        }
    }

    public void CreateDoorsDataDico()
    {
        if (_doorsData == null)
        {
            _doorsData = new Dictionary<Vector3, Neighbourhood.Direction>();
            
            for (int i = 0; i < _doorsDataPos.Count; i++)
            {
                _doorsData.Add(_doorsDataPos[i], _doorsDataDir[i]);
            }
        }
        
        
    }
    
    private void SetAllTilePositions()
    {
        _tilePositions.Clear();

        for (int x = _bounds.xMin + 2; x < _bounds.xMax - 2; x++)
        {
            for (int y = _bounds.yMin + 2; y < _bounds.yMax - 3; y++)
            {
                if (!_wallsPositions.Contains(new Vector3Int(x, y, 0)))
                {
                    _tilePositions.Add(new Vector3Int(x, y, 0));
                }
            }
        }
    }

    public void SetWallPositions(Tilemap rndWallPattern)
    {
        var tilemapBounds = rndWallPattern.cellBounds;

        for (int x = tilemapBounds.xMin; x < tilemapBounds.xMax; x++)
        {
            for (int y = tilemapBounds.yMin; y < tilemapBounds.yMax; y++)
            {
                var tile = rndWallPattern.GetTile(new Vector3Int(x, y, 0));
                
                if (tile)
                {
                    //tilemap.SetTile(new Vector3Int(x, y, 0), tile);
                    Vector3Int boundsCenterInt = new Vector3Int((int)_bounds.center.x, (int)_bounds.center.y, 0);

                    Vector3Int wallTilePos = boundsCenterInt + new Vector3Int(x, y, 0);
                    
                    _wallsPositions.Add(wallTilePos);
                }
            }
        }
    }
    
    public Vector3Int GetARandomTilePosition()
    {
        return _tilePositions[Random.Range(0, _tilePositions.Count)];
    }
    
    public List<Vector3Int> GetDoorPositions()
    {
        List<Vector3Int> doorPositions = new List<Vector3Int>();
        
        foreach (var neighbour in Neighbourhood.CardinalNeighbours)
        {
            Vector3Int gridNeighbour = new Vector3Int(
                (int)neighbour.Value.x * _bounds.size.x, (int)neighbour.Value.y * _bounds.size.y, 0);

            Vector3Int roomNeighbourPos = _bounds.position + gridNeighbour;

            if (_roomNeighboursData != null)
            {
                foreach (var roomNeighbour in _roomNeighboursData)
                {
                    if (roomNeighbour.Bounds.position == roomNeighbourPos)
                    {
                        Vector3 doorPos = DoorPositions[(int)neighbour.Key];
                        
                        doorPositions.Add(new Vector3Int((int)doorPos.x, (int)doorPos.y, 0));
                        _doorsData[doorPos] = neighbour.Key;
                        
                        _doorsDataPos.Add(doorPos);
                        _doorsDataDir.Add(neighbour.Key);
                    }
                }
            }
            
            // if (_roomNeighbours != null)
            // {
            //     if (_roomNeighbours.ContainsKey(roomNeighbourPos))
            //     {
            //         Vector3 doorPos = DoorPositions[(int)neighbour.Key];
            //         
            //         doorPositions.Add(new Vector3Int((int)doorPos.x, (int)doorPos.y, 0));
            //         _doorsData[doorPos] = neighbour.Key;
            //     }
            // }
        }
        
        return doorPositions;
    }

    public void AddDoor(DoorTileCell door)
    {
        _doors.Add(door);
    }

    public void SetDoorsOpen(bool areOpen)
    {
        foreach (var door in _doors)
        {
            door.SetDoorOpen(areOpen);
        }
    }
    
    public void AddRoomNeighbour(Vector3Int neighbourPos, RoomData roomNeighbour)
    {
        _roomNeighbours[neighbourPos] = roomNeighbour;
        _roomNeighboursData.Add(roomNeighbour);
    }

    public RoomData GetRoomNeighbourByAPosition(Vector3Int position)
    {
        foreach (var room in _roomNeighbours)
        {
            if (room.Value.IsPositionInBounds(position))
            {
                return room.Value;
            }
        }

        return null;
    }

    public bool IsPositionInBounds(Vector3Int position)
    {
        for (int x = _bounds.xMin; x < _bounds.xMax; x++)
        {
            for (int y = _bounds.yMin; y < _bounds.yMax; y++)
            {
                if (position.x == x && position.y == y)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
