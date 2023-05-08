using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SO_Room : ScriptableObject
{
    [SerializeField] private BoundsInt _bounds;
    [SerializeField] private List<Vector3Int> _tilePositions;

    [SerializeField] private List<RoomData> _roomNeighboursData;

    [SerializeField]private int _distanceFromStart;

    private Dictionary<Vector3, Neighbourhood.Direction> _doorsData;
    [SerializeField] private List<Vector3> _doorsDataPos;
    [SerializeField] private List<Neighbourhood.Direction> _doorsDataDir;

    [SerializeField]private List<DoorTileCell> _doors;

    [SerializeField] private List<Vector3Int> _wallsPositions;
    
    [SerializeField]private int _enemySpawnWeight;

    [SerializeField]private bool _hasEnemiesToFight;
    
    [SerializeField]private RoomType _type;
    
    public void Init(BoundsInt bounds, List<SO_Room> roomNeighboursSO, List<Vector3> _doorsDataPos)
    {
        
    }
}
