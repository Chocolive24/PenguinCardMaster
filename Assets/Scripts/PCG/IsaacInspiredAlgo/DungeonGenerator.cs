using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;
using Range = UnityEngine.SocialPlatforms.Range;

public class DungeonGenerator : MonoBehaviour
{
    // Attributes ------------------------------------------------------------------------------------------------------
    
    // The size of the grid where we generate our rooms.
    private Vector2Int _gridSize = new Vector2Int(8, 8);
    
    // The size of a simple room.
    private Vector3Int _roomSize;

    [Header("Room Attributes")]
    [SerializeField][Range(5, 30)] private int _minNbrOfRooms = 7;
    [SerializeField] [Range(5, 30)] private int _maxNbrOfRooms = 10;
    private int _nbrOfRooms;
    
    [SerializeField][Range(1, 30)] private int _minEnemyWeight = 5, _maxEnemyWeight = 7;
    private int _enemyWeight;

    [SerializeField] private bool _withWallsInside = true;
    
    private Dictionary<Vector3Int, RoomData> _rooms;
    private RoomData _startRoom;
    private HashSet<RoomData> _endRooms;
    private RoomData _finalRoom;
    private RoomData _shopRoom;
    private HashSet<Vector3Int> _occupiedPositions;
    
    private HashSet<Vector2Int> _groundPositions;
    private HashSet<Vector2Int> _wallPositions;

    private Vector2Int _gridSizeMultipliedByRoomSize;

    // References ------------------------------------------------------------------------------------------------------

    [Header("Tiles References")]
    [SerializeField] private Tilemap _dungeonTilemap;

    [SerializeField] private RuleTile _groundRuleTile, _wallRuleTile, _doorRuleTile;

    [SerializeField] private List<Tilemap> _wallTilemapPatterns;

    // Debug ---------------------
    [SerializeField] private GameObject _startRoomDebug;
    [SerializeField] private GameObject _endRoomDebug;
    [SerializeField] private GameObject _shopRoomDebug;

    // Getters and Setters ---------------------------------------------------------------------------------------------
    public Dictionary<Vector3Int, RoomData> Rooms => _rooms;

    // Methods ---------------------------------------------------------------------------------------------------------
    private void Start()
    {
        _startRoomDebug.SetActive(false);
        _endRoomDebug.SetActive(false);
        _shopRoomDebug.SetActive(false);
    }

    public void Generate()
    {
        _roomSize = new Vector3Int(19, 11);
        
        _gridSizeMultipliedByRoomSize.x = _gridSize.x * _roomSize.x;
        _gridSizeMultipliedByRoomSize.y = _gridSize.y * _roomSize.y;
        
        _rooms = new Dictionary<Vector3Int, RoomData>();
        _endRooms = new HashSet<RoomData>();
        _occupiedPositions = new HashSet<Vector3Int>();

        if (_maxNbrOfRooms < _minNbrOfRooms)
        {
            _maxNbrOfRooms = _minNbrOfRooms;
        }

        _nbrOfRooms = _maxNbrOfRooms >= _minNbrOfRooms ?
            Random.Range(_minNbrOfRooms, _maxNbrOfRooms + 1) : _minNbrOfRooms;
        
        _enemyWeight = _maxEnemyWeight >= _minEnemyWeight ?
            Random.Range(_minEnemyWeight, _maxEnemyWeight + 1) : _minEnemyWeight;
        
        do
        {
            GenerateRooms();
        } while (_rooms.Count != _nbrOfRooms);

        SetUpRooms();
        
        // Draw tiles
        PaintDungeon(_dungeonTilemap);
        
        _startRoom.SetDoorsOpen(true);
        _shopRoom.SetDoorsOpen(true);
    }
    
    private void GenerateRooms()
    {
        Vector3Int startGridPos = new Vector3Int(_gridSizeMultipliedByRoomSize.x / 2, 
            _gridSizeMultipliedByRoomSize.y / 2, 0);

        RoomData startRoom = CreateRoom(startGridPos, 0,0, false);

        Queue<RoomData> roomQueue = new Queue<RoomData>();
        roomQueue.Enqueue(startRoom);
        
        if (_rooms.Count == 0)
        {
            _rooms[startGridPos] = startRoom;
        }
        
        _occupiedPositions.Add(startGridPos);
        
        while (roomQueue.Count > 0)
        {
            RoomData currentRoom = roomQueue.Dequeue();
            
            int neighbourAdded = 0;
            
            foreach (var neighbour in Neighbourhood.CardinalNeighbours)
            {
                Vector3Int gridNeighbour = new Vector3Int(
                    (int)neighbour.Value.x * _roomSize.x, (int)neighbour.Value.y * _roomSize.y, 0);

                Vector3Int roomNeighbourPos = currentRoom.Bounds.position + gridNeighbour;

                int distanceFromStart = currentRoom.DistanceFromStart + 1;

                if (CheckForAbandon(roomNeighbourPos, _occupiedPositions, distanceFromStart, true))
                {
                    continue;
                }
                
                int enemySpawnWeight = Random.Range(_minEnemyWeight, _maxEnemyWeight);
                RoomData newRoom = CreateRoom(roomNeighbourPos, distanceFromStart, 
                                                enemySpawnWeight, true);

                roomQueue.Enqueue(newRoom);
                _rooms[roomNeighbourPos] = newRoom;
                _occupiedPositions.Add(roomNeighbourPos);

                // Add neighbours to the current Room.
                if (currentRoom.Bounds.position == startGridPos)
                {
                    _rooms.First().Value.AddRoomNeighbour(roomNeighbourPos, newRoom);
                }
                else
                {
                    currentRoom.AddRoomNeighbour(roomNeighbourPos, newRoom);
                }
                
                // Add neighbour to the new Room.
                newRoom.AddRoomNeighbour(currentRoom.Bounds.position, currentRoom);
                
                neighbourAdded++;
            }

            if (neighbourAdded == 0)
            {
                HandleStartRoomBlocked(neighbourAdded, currentRoom, startRoom, roomQueue);
            }
        }
    }

    private RoomData CreateRoom(Vector3Int roomNeighbourPos, int distanceFromStart, 
                                int enemySpawnWeight, bool hasEnemiesToFight)
    {
        return new RoomData(new BoundsInt(roomNeighbourPos, _roomSize), distanceFromStart,
                            enemySpawnWeight, hasEnemiesToFight);
    }

    private bool CheckForAbandon(Vector3Int position, HashSet<Vector3Int> occupiedPositions,
                                 int distanceFromStart, bool withRandom)
    {
        // Out of grid limit.
        if (!IsPositionInGrid(position))
        {
            return true;
        }

        // The position has already been visited so there is a room.
        if (occupiedPositions.Contains(position))
        {
            return true;
        }

        // There is more than 1 Room neighbour at this position.
        if (CountNeighbours(position) > 1)
        {
            return true;
        }
        
        // There is already enough Rooms.
        if (_rooms.Count >= _nbrOfRooms)
        {
            return true;
        }

        // The room is too deep and we risked to have only one endRoom
        // (we want 2 because of the final and the shop room)
        if (distanceFromStart >= _nbrOfRooms - 2)
        {
            return true;
        }
        
        if (withRandom)
        {
            // 50 % chance to abandon.
            if (Random.Range(0, 2) == 1)
            {
                return true;
            }
        }
        
        return false;
    }
    
    private bool IsPositionInGrid(Vector3Int position)
    {
        int rightPos = position.x + _roomSize.x;
        int upPos = position.y + _roomSize.y;

        return position.x >= 0 && position.y >= 0 && rightPos <= 
            _gridSizeMultipliedByRoomSize.x && upPos <= _gridSizeMultipliedByRoomSize.y;
    }

    private int CountNeighbours(Vector3Int position)
    {
        int nbrNeighbour = 0;

        foreach (var neighbour in Neighbourhood.CardinalNeighbours)
        {
            Vector3Int gridNeighbour = new Vector3Int(
                (int)neighbour.Value.x * _roomSize.x, (int)neighbour.Value.y * _roomSize.y, 0);

            Vector3Int positionToAnalyse = position + gridNeighbour;

            if (_occupiedPositions.Contains(positionToAnalyse))
            {
                nbrNeighbour++;
            }
        }

        return nbrNeighbour;
    }
    
    private void HandleStartRoomBlocked(int neighbourAdded, RoomData currentRoom, RoomData startRoom, Queue<RoomData> roomQueue)
    {
        if (currentRoom == startRoom)
        {
            bool isStartRoomBlocked = false;

            foreach (var neighbour in Neighbourhood.CardinalNeighbours)
            {
                Vector3Int gridNeighbour = new Vector3Int(
                    (int)neighbour.Value.x * _roomSize.x, (int)neighbour.Value.y * _roomSize.y, 0);

                Vector3Int roomNeighbourPos = currentRoom.Bounds.position + gridNeighbour;

                if (!CheckForAbandon(roomNeighbourPos, _occupiedPositions, 1, false))
                {
                    isStartRoomBlocked = false;
                    break;
                }

                isStartRoomBlocked = true;
            }

            if (isStartRoomBlocked)
            {
                foreach (var room in _rooms)
                {
                    if (room.Value != _rooms.First().Value)
                    {
                        roomQueue.Enqueue(room.Value);
                    }
                }
            }
        }
    }
    
    private void SetUpRooms()
    {
        _startRoom = _rooms.First().Value;

        foreach (var room in _rooms)
        {
            if (room.Value.RoomNeighbours.Count == 1 && room.Value != _rooms.First().Value)
            {
                _endRooms.Add(room.Value);
            }
        }
        
        _finalRoom = _endRooms.OrderBy(x => x.DistanceFromStart).Last();
        _shopRoom = _endRooms.Where(room => room != _finalRoom).OrderBy(x => x.DistanceFromStart).Last();
        
        // Debug Part -----------------------------------------------------------------------------------
        _startRoomDebug.transform.position = _startRoom.Bounds.center;
        _endRoomDebug.transform.position = _finalRoom.Bounds.center;
        _shopRoomDebug.transform.position = _shopRoom.Bounds.center;
        // ----------------------------------------------------------------------------------------------
        
        foreach (var room in _rooms)
        {
            Tilemap rndWallPattern = _wallTilemapPatterns[Random.Range(0, _wallTilemapPatterns.Count)];
            
            if (room.Value == _startRoom)
            {
                room.Value.SetType(RoomData.RoomType.START, rndWallPattern);
            }
            else if (room.Value == _finalRoom)
            {
                room.Value.SetType(RoomData.RoomType.END, rndWallPattern);
            }
            else if (room.Value == _shopRoom)
            {
                room.Value.SetType(RoomData.RoomType.SHOP, rndWallPattern);
            }
            else
            {
                room.Value.SetType(RoomData.RoomType.BASIC, rndWallPattern);
            }
        }
    }
    
    private void PaintDungeon(Tilemap tilemap)
    {
        _dungeonTilemap.ClearAllTiles();
        
        foreach (var room in _rooms)
        {
            _groundPositions = GetGroundPositions(room.Value);

            _wallPositions = GetWallsPositions(room.Value);
            
            PaintRoom(tilemap, room.Value, _groundPositions, _wallPositions, 
                room.Value.GetDoorPositions());
        }
    }

    private void PaintRoom(Tilemap tilemap, RoomData room, HashSet<Vector2Int> groundPositions, 
                            HashSet<Vector2Int> wallPositions, List<Vector3Int> roomDoorsPosition)
    {
        PaintTilesFromAListOfPositions(tilemap, _groundRuleTile, groundPositions);
        
        PaintTilesFromAListOfPositions(tilemap, _wallRuleTile, wallPositions);

        PaintDoors(tilemap, room, roomDoorsPosition);

        if (_withWallsInside)
        {
            if (room != _startRoom && room != _finalRoom)
            {
                PaintRoomWallPattern(tilemap, room);
            }
        }
    }
    
    private void PaintTilesFromAListOfPositions(Tilemap tilemap, RuleTile ruleTile, HashSet<Vector2Int> positions)
    {
        foreach (var pos in positions)
        {
            Vector3Int tilePos = new Vector3Int(pos.x, pos.y, 0);
            tilemap.SetTile(tilePos, ruleTile);
        }
    }

    private void PaintDoors(Tilemap tilemap, RoomData room, List<Vector3Int> roomDoorsPosition)
    {
        foreach (var doorPos in roomDoorsPosition)
        {
            tilemap.SetTile(doorPos, _doorRuleTile);

            var doorGameObject = tilemap.GetInstantiatedObject(doorPos);

            DoorTileCell door = doorGameObject.GetComponent<DoorTileCell>();

            door.Room = room;

            room.DoorsData.TryGetValue(doorPos, out Neighbourhood.Direction direction);

            door.SetDirection(direction);
            
            room.AddDoor(door);
        }
    }

    private void PaintRoomWallPattern(Tilemap tilemap, RoomData room)
    {
        foreach (var wallTilePos in room.WallsPositions)
        {
            tilemap.SetTile(wallTilePos, _wallRuleTile);
        }
    }

    private HashSet<Vector2Int> GetGroundPositions(RoomData room)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();

        for (int x = room.Bounds.xMin + 1; x < room.Bounds.xMax - 1; x++)
        {
            for (int y = room.Bounds.yMin + 1; y < room.Bounds.yMax - 1; y++)
            {
                floor.Add(new Vector2Int(x, y));
            }
        }

        return floor;
    }
    
    private HashSet<Vector2Int> GetWallsPositions(RoomData room)
    {
        HashSet<Vector2Int> walls = new HashSet<Vector2Int>();

        for (int x = room.Bounds.xMin; x < room.Bounds.xMax; x++)
        {
            walls.Add(new Vector2Int(x, room.Bounds.yMin));
            walls.Add(new Vector2Int(x, room.Bounds.yMax - 1));
        }

        for (int y = room.Bounds.yMin; y < room.Bounds.yMax; y++)
        {
            walls.Add(new Vector2Int(room.Bounds.xMin, y));
            walls.Add(new Vector2Int(room.Bounds.xMax - 1, y));
        }

        return walls;
    }
}
