using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapsManager : MonoBehaviour
{
    // Singleton -------------------------------------------------------------------------------------------------------
    private static TilemapsManager _instance;
    public static TilemapsManager Instance { get { return _instance; } }
    
    // References ------------------------------------------------------------------------------------------------------

    #region Tilemaps and Tiles

    [SerializeField] private Tilemap _movementTilemap;
    [SerializeField] private Tilemap _attackTilemap;
    [SerializeField] private RuleTile _movementRuleTile;
    [SerializeField] private RuleTile _attackRuleTile;
    [SerializeField] private RuleTile _enemyAttackRuleTile;
    [SerializeField] private TileCell _movementTile;
    [SerializeField] private TileCell _attackTile;

    [SerializeField] private GameObject _tilemapPrefab;

    #endregion

    #region Managers

    private GridManager _gridManager;

    #endregion

    // Getters and Setters ---------------------------------------------------------------------------------------------
    public Tilemap AttackTilemap => _attackTilemap;
    public RuleTile AttackRuleTile => _attackRuleTile;

    public RuleTile EnemyAttackRuleTile => _enemyAttackRuleTile;
    public TileCell AttackTile => _attackTile;

    // -----------------------------------------------------------------------------------------------------------------
    
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
    }
    
    // Start is called before the first frame update
    void Start()
    {
        _gridManager = GridManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Tilemap InstantiateTilemap(string name)
    {
        var tilemmap = Instantiate(_tilemapPrefab, Vector3.zero, Quaternion.identity);

        tilemmap.name = name;
        
        var grid = FindObjectOfType<Grid>();

        if (grid)
        {
            tilemmap.transform.parent = grid.transform;
        }

        return tilemmap.GetComponent<Tilemap>();
    }
    
    /// <summary>
    /// Dijkstra Research within a specified range.
    /// Returns a Dictionary which has for Key the Tile's position and for Value the Tile's Distance.
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="range"></param>
    /// <param name="gameObject">Check if the gameObject is a BaseAttackCard, if so, we need to add the enemies
    ///                          Tiles to the availableTiles.</param>
    /// <returns></returns>
    public Dictionary<Vector3, int> GetAvailableTilesInRange(Vector3 startPos, int range, 
        Dictionary<Neighbourhood.Direction, Vector2> neighbourhood,
        bool countHeroes, bool countEnemies)
    {
        var queue = new Queue<Vector3>();
        var distances = new Dictionary<Vector3, int> { { startPos, 0 } };

        queue.Enqueue(startPos);

        while (queue.Count > 0)
        {
            var currentPos = queue.Dequeue();
            
            foreach (var direction in neighbourhood)
            {
                Vector3 dir = new Vector3(direction.Value.x, direction.Value.y, 0);
                
                var neighbor = _gridManager.WorldToCellCenter(currentPos + dir);

                if (IsPositionAvailable(neighbor, countHeroes, countEnemies) && 
                    !distances.ContainsKey(neighbor))
                {
                    if (distances[currentPos] + 1 <= range)
                    {
                        distances.Add(neighbor, distances[currentPos] + 1);
                        queue.Enqueue(neighbor);
                    }
                }
            }
        }
        
        return distances;
    }

    public bool IsPositionAvailable(Vector3 position, bool countHeroes, bool countEnemies)
    {
        if (_gridManager.GetTileAtPosition(position) != null)
        {
            TileCell tile = _gridManager.GetTileAtPosition(position);
            
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

    /// <summary>
    /// Draw the tiles corresponding to the area of effect of the current object
    /// </summary>
    /// <param name="availableTiles"></param> In range and available tile neighbours.
    /// <param name="tilemap"></param> The Tilemap on we want to draw our tiles
    /// <param name="ruleTile"></param> The RuleTile to apply to the tilemap
    public void DrawTilemap(Dictionary<Vector3, int> availableTiles, Tilemap tilemap, RuleTile ruleTile)
    {
        foreach (var availableTile in availableTiles)
        {
            var pos = tilemap.WorldToCell(new Vector3(availableTile.Key.x, availableTile.Key.y, 0));
            
            tilemap.SetTile(pos, ruleTile);
        }
    }

    public List<Vector3> FindPath(Vector3 startPos, Vector3 endPos, bool countHeroes, bool countEnemies)
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

                if (!IsPositionAvailable(neighbor, countHeroes, countEnemies) || closedList.Contains(neighbor))
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
    
    private List<Vector3> RestrucutrePath(Vector3 startPos, Vector3 endPos)
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

    public Dictionary<Vector3, int> FindPathWithinRange(Vector3 startPos, Dictionary<Vector3, int> availableTiles)
    {
        var queue = new Queue<Vector3>();
        Dictionary<Vector3, int> path = new Dictionary<Vector3, int>();
    
        queue.Enqueue(startPos);
        path[startPos] = availableTiles[startPos];
    
        while (queue.Count > 0)
        {
            var currentPos = queue.Dequeue();

            // We don't need the tile with the distance 0 because it's our startPos, so we stop the loop before
            // accessing the 0 distance tile.
            if (availableTiles[currentPos] == 1)
            {
                break;
            }

            // Get the neighbor with the shortest distance from the current position
            var neighbor = GetShortestNeighbor(currentPos, availableTiles, path);

            // If we found a neighbor, add it to the path and queue for further processing
            if (neighbor != null)
            {
                var neighborPos = _gridManager.WorldToCellCenter(neighbor.Value.Key);
                var neighborDistance = neighbor.Value.Value;
    
                path[neighborPos] = neighborDistance;
                queue.Enqueue(neighborPos);
            }
        }
    
        return path;
    }
    
    // The KeyValuePair type is a struct, which is a value type that can't be null.
    // By adding the ? after the type name, we're making it nullable, which means it can have a null value.
    private KeyValuePair<Vector3, int>? GetShortestNeighbor(Vector3 currentPos, 
                                                            Dictionary<Vector3, int> availableTiles, 
                                                            Dictionary<Vector3, int> path)
    {
        int distance = int.MaxValue;
        KeyValuePair<Vector3, int>? bestNeighbor = null;
    
        foreach (var direction in new Vector3[] 
                     { Vector3.up, Vector3.down, Vector3.left, Vector3.right })
        {
            var neighborPos = currentPos + direction;
    
            if (availableTiles.ContainsKey(neighborPos) && !path.ContainsKey(neighborPos))
            {
                var neighborDistance = availableTiles[neighborPos];
    
                if (neighborDistance < distance)
                {
                    distance = neighborDistance;
                    bestNeighbor = new KeyValuePair<Vector3, int>(neighborPos, distance);
                }
            }
        }
    
        return bestNeighbor;
    }

    
    public void DrawPathTilemap(Dictionary<Vector3, int> path, Tilemap tilemap, RuleTile ruleTile)
    {
        foreach (var availableTile in path)
        {
            var pos = tilemap.WorldToCell(new Vector3(availableTile.Key.x, availableTile.Key.y, 0));

            tilemap.SetTile(pos, ruleTile);
        }
    }
    
    public RuleTile GetRuleTile(BaseCard baseCard)
    {
        if (baseCard.CardType == CardType.MoveCard)
        {
            return _movementRuleTile;
        }
        if (baseCard.CardType == CardType.Attackcard)
        {
            return _attackRuleTile;
        }

        return null;
    }
}
