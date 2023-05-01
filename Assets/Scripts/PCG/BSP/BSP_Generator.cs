using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;


public class BSP_Generator : MonoBehaviour
{
    [SerializeField] private Vector2Int _blockSize;
    [SerializeField] private Vector2Int _min;
    [SerializeField] private Vector2Int _max;

    [SerializeField]private Tilemap _tilemap;

    [SerializeField]private RuleTile _floorRuleTile;
    [SerializeField]private RuleTile _wallsRuleTile;

    private List<BoundsInt> _rooms;

    // Methods ---------------------------------------------------------------------------------------------------------
    public void Process(BoundsInt mapToProcess, List<BoundsInt> roomsList, float minWidth, float minHeight)
    {
        _rooms.Clear();
        
        Queue<BoundsInt> roomsQueue = new Queue<BoundsInt>();

        roomsQueue.Enqueue(mapToProcess);

        SplitDirection direction = SplitDirection.HORIZONTAL;

        int crashCounter = 0;

        do
        {
            var room = roomsQueue.Dequeue();

            if (room.size.x <= (_blockSize.x) && room.size.y <= (_blockSize.y))
            {
                roomsList.Add(room);
            }
            else
            {
                int cutRatio = 0;
                
                if (direction == SplitDirection.HORIZONTAL)
                {
                    direction = SplitDirection.VERTICAL;
                    cutRatio = Random.Range(1, room.size.x / _blockSize.x) * _blockSize.x;
                }
                else if (direction == SplitDirection.VERTICAL)
                {
                    direction = SplitDirection.HORIZONTAL;
                    cutRatio = Random.Range(1, room.size.y / _blockSize.y) * _blockSize.y;
                }

                
                BoundsSpliter.SplitBounds(room, cutRatio,
                    direction, out BoundsInt room1, out BoundsInt room2);

                roomsQueue.Enqueue(room1);
                roomsQueue.Enqueue(room2);

                // Debug -----------------------------------------------------------------------------------------------
                // Debug.Log("BIG_ROOM : xMin " + room.xMin + " / xMax " + room.xMax + " / yMin " + 
                //           room.yMin + "/ yMax " + room.yMax);
                //
                // int idx = 1;
                //
                // foreach (var item in roomsQueue)
                // {
                //     Debug.Log("ROOM " + idx + " : xMin " + item.xMin + " / xMax " + item.xMax + " / yMin " + 
                //               item.yMin + "/ yMax " + item.yMax);
                //     idx++;
                // }

                // -----------------------------------------------------------------------------------------------------
            }

            crashCounter++;
        } while (roomsQueue.Count > 0 && crashCounter < 1000);
        
        Debug.Log("Crash Counter : " + crashCounter + " QUeue.COunt " + roomsQueue.Count);
    }

    public void Generate()
    {
        Vector3Int size = new Vector3Int(_blockSize.x * Random.Range(_min.x, _max.x), 
            _blockSize.y * Random.Range(_min.y, _max.y), 0);
        Vector3Int center = new Vector3Int((int)0.5f * size.x, (int)0.5f * size.y, 0);

        BoundsInt bigRoom = new BoundsInt(center, size);
        
        // -------------------------------------------------------------------------------------------------------------
        Process(bigRoom, _rooms, _min.x, _min.y);

        _tilemap.ClearAllTiles();
        
        int idx = 1;
        
        Debug.Log("Big Room size : " + bigRoom.size);
        
        foreach (var room in _rooms)
        {
            Debug.Log(room.size);
            
            // Debug.Log("ROOM " + idx + " : xMin " + room.xMin + " / xMax " + room.xMax + " / yMin " + 
            //           room.yMin + "/ yMax " + room.yMax);
            //     idx++;
            
            HashSet<Vector2Int> floor = GetFloorPositions(room);

            HashSet<Vector2Int> walls = GetWallsPositions(room);
            
            PaintMap(_tilemap, _floorRuleTile, _wallsRuleTile, room, floor, walls);
        }
    }

    private void PaintMap(Tilemap tilemap, RuleTile floorRuleTile, RuleTile wallsRuleTile,
        BoundsInt bounds, HashSet<Vector2Int> floor,
        HashSet<Vector2Int> walls)
    {
        PaintTilesFromAListOfPositions(tilemap, _floorRuleTile, floor);
        
        PaintTilesFromAListOfPositions(tilemap, _wallsRuleTile, walls);
    }

    private void PaintTilesFromAListOfPositions(Tilemap tilemap, RuleTile ruleTile, HashSet<Vector2Int> positions)
    {
        foreach (var pos in positions)
        {
            Vector3Int tilePos = new Vector3Int(pos.x, pos.y, 0);
            tilemap.SetTile(tilePos, ruleTile);
        }
    }
    
    private HashSet<Vector2Int> GetWallsPositions(BoundsInt bigRoom)
    {
        HashSet<Vector2Int> walls = new HashSet<Vector2Int>();

        for (int x = bigRoom.xMin; x < bigRoom.xMax; x++)
        {
            walls.Add(new Vector2Int(x, bigRoom.yMin));
            walls.Add(new Vector2Int(x, bigRoom.yMax - 1));
        }

        for (int y = bigRoom.yMin; y < bigRoom.yMax; y++)
        {
            walls.Add(new Vector2Int(bigRoom.xMin, y));
            walls.Add(new Vector2Int(bigRoom.xMax - 1, y));
        }

        return walls;
    }

    private HashSet<Vector2Int> GetFloorPositions(BoundsInt bigRoom)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();

        for (int x = bigRoom.xMin + 1; x < bigRoom.xMax - 1; x++)
        {
            for (int y = bigRoom.yMin + 1; y < bigRoom.yMax - 1; y++)
            {
                floor.Add(new Vector2Int(x, y));
            }
        }

        return floor;
    }

    // private IEnumerator DebugCo(Tilemap tilemap, RuleTile floorRUleTile, RuleTile wallRuleTile,
    //     BoundsInt room, HashSet<Vector2Int> floor, HashSet<Vector2Int> walls)
    // {
    //     yield return new WaitForSeconds(1f);
    //     
    //     PaintMap(tilemap, floorRUleTile, wallRuleTile, room, floor, walls);
    // }
}
