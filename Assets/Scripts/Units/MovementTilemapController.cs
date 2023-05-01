using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MovementTilemapController : MonoBehaviour
{
    // //[SerializeField] private GridManager _gridManager;
    // [SerializeField] private Tilemap _movementTilemap;
    // [SerializeField] private RuleTile _ruleTile;
    // [SerializeField] private Character _character;
    //
    // [SerializeField] private PlayerMoveTile _playerMoveTile;
    //
    // private List<TileCell> _neighbours;
    // private List<TileCell> _walkableNeighbours;
    //
    // private List<TileCell> _availableNeighbours;
    //
    // // Start is called before the first frame update
    // void Start()
    // {
    //     
    // }
    //
    // // Update is called once per frame
    // void Update()
    // {
    //     
    // }
    //
    //
    // // private void OnMouseEnter()
    // // {
    // //     
    // // }
    //
    // // private void OnMouseExit()
    // // {
    // //     foreach (var item in _availableNeighbours)
    // //     {
    // //         Destroy(item.gameObject);
    // //     }
    // // }
    //
    // public List<TileCell> GenerateTilemap(TileCell startingTile, BaseUnit unit)
    // {
    //     var inRangeTiles = new List<TileCell>();
    //     int stepcount = 0;
    //     
    //     inRangeTiles.Add(startingTile);
    //     
    //     _walkableNeighbours = new List<TileCell>();
    //     _walkableNeighbours.Add(startingTile);
    //
    //     _availableNeighbours = new List<TileCell>();
    //     
    //     //  starting tile
    //     // Vector3 tileCellToAnalyze = character.transform.position;
    //
    //     while (stepcount < unit.Movement)
    //     {
    //         _neighbours = new List<TileCell>();
    //
    //         foreach (var walkableNeighbour in _walkableNeighbours)
    //         {
    //             Vector2 north =
    //                 new Vector2(walkableNeighbour.Position.x, walkableNeighbour.Position.y + 1); // North neighbour
    //             Vector2 south =
    //                 new Vector2(walkableNeighbour.Position.x, walkableNeighbour.Position.y - 1); // South neighbour
    //             Vector2 east =
    //                 new Vector2(walkableNeighbour.Position.x + 1, walkableNeighbour.Position.y); // East neighbour
    //             Vector2 west =
    //                 new Vector2(walkableNeighbour.Position.x - 1, walkableNeighbour.Position.y); // West neighbour
    //
    //             if ((north.x >= 0 && north.x < GridManager.Instance.Size.x) &&
    //                 (north.y >= 0 && north.y < GridManager.Instance.Size.y))
    //             {
    //                 _neighbours.Add(GridManager.Instance.GetTileAtPosition(north));
    //             }
    //             if ((south.x >= 0 && south.x < GridManager.Instance.Size.x) &&
    //                 (south.y >= 0 && south.y < GridManager.Instance.Size.y))
    //             {
    //                 _neighbours.Add(GridManager.Instance.GetTileAtPosition(south));
    //             }
    //             if ((east.x >= 0 && east.x < GridManager.Instance.Size.x) &&
    //                 (east.y >= 0 && east.y < GridManager.Instance.Size.y))
    //             {
    //                 _neighbours.Add(GridManager.Instance.GetTileAtPosition(east));
    //             }
    //             if ((west.x >= 0 && west.x < GridManager.Instance.Size.x) &&
    //                 (west.y >= 0 && west.y < GridManager.Instance.Size.y))
    //             {
    //                 _neighbours.Add(GridManager.Instance.GetTileAtPosition(west));
    //             }
    //             
    //             
    //             // if (north.x < GridManager.Instance.WorldTilemap.localBounds.max.x &&
    //             //     north.y < GridManager.Instance.WorldTilemap.localBounds.max.y &&
    //             //     north.x > GridManager.Instance.WorldTilemap.localBounds.min.x &&
    //             //     north.y > GridManager.Instance.WorldTilemap.localBounds.min.y)
    //             // {
    //             //     _neighbours.Add(GridManager.Instance.GetTileAtPosition(north));
    //             // }
    //             //
    //             // if (south.x < GridManager.Instance.WorldTilemap.localBounds.max.x &&
    //             //     south.y < GridManager.Instance.WorldTilemap.localBounds.max.y &&
    //             //     south.x > GridManager.Instance.WorldTilemap.localBounds.min.x &&
    //             //     south.y > GridManager.Instance.WorldTilemap.localBounds.min.y)
    //             // {
    //             //     _neighbours.Add(GridManager.Instance.GetTileAtPosition(south));
    //             // }
    //             //
    //             // if (east.x < GridManager.Instance.WorldTilemap.localBounds.max.x &&
    //             //     east.y < GridManager.Instance.WorldTilemap.localBounds.max.y &&
    //             //     east.x > GridManager.Instance.WorldTilemap.localBounds.min.x &&
    //             //     east.y > GridManager.Instance.WorldTilemap.localBounds.min.y)
    //             // {
    //             //     _neighbours.Add(GridManager.Instance.GetTileAtPosition(east));
    //             // }
    //             //
    //             // if (west.x < GridManager.Instance.WorldTilemap.localBounds.max.x &&
    //             //     west.y < GridManager.Instance.WorldTilemap.localBounds.max.y &&
    //             //     west.x > GridManager.Instance.WorldTilemap.localBounds.min.x &&
    //             //     west.y > GridManager.Instance.WorldTilemap.localBounds.min.y)
    //             // {
    //             //     _neighbours.Add(GridManager.Instance.GetTileAtPosition(west));
    //             // }
    //         }
    //
    //         inRangeTiles.AddRange(_neighbours);
    //         _walkableNeighbours = _neighbours.Distinct().ToList();
    //         stepcount++;
    //
    //         foreach (var item in inRangeTiles) // perhaps inRangeTiles
    //         {
    //             var playerMoveTile = Instantiate(_playerMoveTile, item.Position, Quaternion.identity);
    //             
    //             _availableNeighbours.Add(playerMoveTile);
    //             
    //             // Vector3Int pos = new Vector3Int((int)(item.position.x), (int)(item.position.y));
    //             //
    //             // _movementTilemap.SetTile(pos, _ruleTile);
    //         }
    //     }
    //
    //     return inRangeTiles.Distinct().ToList();
    // }
}
