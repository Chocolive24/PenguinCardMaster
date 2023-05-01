using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class GridManager : MonoBehaviour
{
    // Singleton -------------------------------------------------------------------------------------------------------
    private static GridManager _instance;
    public static GridManager Instance { get { return _instance; } }
    
    // Attributes ------------------------------------------------------------------------------------------------------
    [SerializeField] private int _width, _height;
    
    private Dictionary<Vector3, TileCell> _tiles;
    
    private GameObject _currentRoom;
    
    // References ------------------------------------------------------------------------------------------------------

    #region Gameobjects

    [SerializeField] private Tilemap _dungeonTilemap;
    [SerializeField] private Transform _camTrans;
    //[SerializeField] private Tilemap _currentRoomTilemap;
    [SerializeField] private RuleTile _groundRuleTile, _wallRuleTile;

    #endregion

    [SerializeField] private DungeonGenerator _dungeonGenerator;

    // Events ----------------------------------------------------------------------------------------------------------
    public static event Action<GridManager, RoomData> OnDungeonGenerate;
    
    // Getters and Setters ---------------------------------------------------------------------------------------------
    public Vector2 Size => new Vector2(_width, _height);
    //public Tilemap CurrentRoomTilemap => _currentRoomTilemap;
    public Dictionary<Vector3, TileCell> Tiles => _tiles;

    // Methods ---------------------------------------------------------------------------------------------------------
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        GenerateGrid();
    }

    private void GetRoomTiles(Room room)
    {
        if (room.Type != RoomType.SHOP)
        {
            _currentRoom = room.gameObject;
        }
    }

    private void Start()
    {
        
    }
    
    public void GenerateGrid()
    {
        _dungeonGenerator.Generate();
        
        _tiles = new Dictionary<Vector3, TileCell>();
        
        TileCell[] dungeonTiles = _dungeonTilemap.GetComponentsInChildren<TileCell>();
        //TileCell[] roomTiles = _currentRoom.GetComponentsInChildren<TileCell>();

        foreach (var tile in dungeonTiles)
        {
            _tiles[WorldToCellCenter(tile.transform.position)] = tile;
        }

        OnDungeonGenerate?.Invoke(this, _dungeonGenerator.Rooms.First().Value);
        
        //_camTrans.transform.position = new Vector3((float)_width / 2f - 0.5f, 
                     //(float)_height / 2f - 0.5f -1f, -10);
    }

    // public void DestroyGrid()
    // {
    //     foreach (var item in _tiles)
    //     {
    //         TileCell tile = GetTileAtPosition(item.Key);
    //
    //         if (tile)
    //         {
    //             tile.OccupiedUnit = null;
    //         }
    //     }
    //     
    //     _tiles.Clear();
    // }
    
    public TileCell GetHeroSpawnTile()
    {
        return GetTileAtPosition(WorldToCellCenter(_dungeonGenerator.Rooms.First().Value.Bounds.center));
        
        return _tiles.Where(t => t.Value.Walkable).OrderBy
            (t => Random.value).First().Value;
    }

    public TileCell GetEnemySpawnTile()
    {
        TileCell rndTile;

        do
        {
            rndTile = _tiles.Where(t => t.Value.Walkable).OrderBy
                (t => Random.value).First().Value;

            Vector3 pos = rndTile.transform.position;

            TileCell rightRndTile = GetTileAtPosition(pos + Vector3.right);
            TileCell upRndTile = GetTileAtPosition(pos + Vector3.up);
            TileCell rightUpRndTile = GetTileAtPosition(new Vector3(pos.x + 1, pos.y + 1, 0));

            if (rightRndTile && upRndTile && rightUpRndTile)
            {
                if (rightRndTile.Walkable && upRndTile.Walkable && rightUpRndTile.Walkable)
                {
                    break;
                }
            }

        } while (true);

        return rndTile;
    }

    public TileCell GetTileAtPosition(Vector3 pos)
    {
        if (_tiles.TryGetValue(pos, out TileCell tileCell))
        {
            return tileCell;
        }
        
        return null;
    }

    public Vector3 WorldToCellCenter(Vector3 position)
    {
        return _dungeonTilemap.GetCellCenterLocal(_dungeonTilemap.WorldToCell(position));
    }
}
