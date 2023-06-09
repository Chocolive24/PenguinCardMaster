using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class UnitsManager : MonoBehaviour
{
    private static UnitsManager _instance;
    public static UnitsManager Instance { get { return _instance; } }

    // Attributes ------------------------------------------------------------------------------------------------------
    private List<HeroData> _heroesData;
    private List<EnemyData> _enemiesData;
    private List<EnemyData> _spawnedEnemiesData;

    private List<BaseHero> _heroes;
    private List<BaseEnemy> _enemies;
    
    private BaseEnemy _currentEnemyPlaying;

    private int _enemyCount = 0;

    private int _roomVisitedNbr = 0;

    // References ------------------------------------------------------------------------------------------------------

    private GridManager _gridManager;
    private UIBattleManager _uiBattleManager;

    [SerializeField] private IntReference _minEnemySpawnWeight;
    [SerializeField] private IntReference _maxEnemySpawnWeight;
    
    private float _betweenTurnsTime = 0.5f;

    private RoomData _currentRoom;
    private Vector3 _heroSpawnPos;

    // Events ----------------------------------------------------------------------------------------------------------
    public static event Action OnEnemiesTurnEnd;

    public static event Action<UnitsManager, BaseHero> OnHeroSpawn;
    
    // Getters and Setters ---------------------------------------------------------------------------------------------
    public BaseHero HeroPlayer => _heroes[0];

    public BaseEnemy CurrentEnemyPlaying { get => _currentEnemyPlaying; set => _currentEnemyPlaying = value; }

    public List<BaseHero> Heroes => _heroes;
    public List<BaseEnemy> Enemies => _enemies;

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

        _heroesData = Resources.LoadAll<HeroData>("Units/Heroes").ToList();
        _enemiesData = Resources.LoadAll<EnemyData>("Units/Enemies").ToList();
        _spawnedEnemiesData = Resources.LoadAll<EnemyData>("Units/SpawnedEnemies").ToList();
        
        _heroes = new List<BaseHero>();
        _enemies = new List<BaseEnemy>();

        DoorTileCell.OnDoorTileEnter += SpawnHeroes;
        DoorTileCell.OnDoorTileEnter += SpawnRoomEnemies;
        ShopManager.OnShopExit += SpawnHeroOutsideShop;
        TileCell.OnTileSelected += HandleSelectedEnemy;

        BattleManager.OnVictoryEnter += DebuffEnemies;
    }

    private void HandleSelectedEnemy(TileCell tile)
    {
        if (tile.OccupiedUnit)
        {
            BaseEnemy enemy = tile.OccupiedUnit.GetComponent<BaseEnemy>();

            if (enemy)
            {
                enemy.IsSelected = !enemy.IsSelected;
            }
        }
    }

    private void OnDestroy()
    {
        DoorTileCell.OnDoorTileEnter -= SpawnHeroes;
        DoorTileCell.OnDoorTileEnter -= SpawnRoomEnemies;
        ShopManager.OnShopExit -= SpawnHeroOutsideShop;
        TileCell.OnTileSelected -= HandleSelectedEnemy;
        BattleManager.OnVictoryEnter -= DebuffEnemies;
    }
    
    private void SpawnRoomEnemies(DoorTileCell doorTile)
    {
        _currentRoom = doorTile.GetRoomNeighbour();

        if (_currentRoom == null)
        {
            return;
        }
        
        if (_currentRoom.HasEnemiesToFight)
        {
            _roomVisitedNbr++;
            
            HandleEnemyStatBoost();

            HandleEnemySpawnWeight();
            
            int remainginWeight = Random.Range(_minEnemySpawnWeight.Value, _maxEnemySpawnWeight.Value + 1);

            while (remainginWeight > 0)
            {
                BaseEnemy enemy = GetRandomEnemyUnderWeight(remainginWeight);
            
                bool isPosValid = false;
                
                Vector3Int randomSpawnPos = new Vector3Int();
                
                do
                {
                    randomSpawnPos = _currentRoom.GetARandomTilePosition();
                
                    enemy.transform.position = _gridManager.WorldToCellCenter(randomSpawnPos);
                    
                    foreach (var tile in enemy.GetOccupiedTiles())
                    {
                        if (!_gridManager.GetTileAtPosition(_gridManager.WorldToCellCenter(tile.transform.position)))
                        {
                            isPosValid = false;
                            break;
                        }
            
                        if (!tile.Walkable)
                        {
                            isPosValid = false;
                            break;
                        }
                        
                        isPosValid = true;
                    }
                    
                } while (!isPosValid);
                
                var spawnedEnemy = Instantiate(enemy);
                
                spawnedEnemy.transform.position = _gridManager.WorldToCellCenter(randomSpawnPos);
            
                spawnedEnemy.PreviousOccupiedTiles = spawnedEnemy.GetOccupiedTiles();
            
                foreach (var tile in spawnedEnemy.GetOccupiedTiles())
                {
                    tile.SetUnit(spawnedEnemy);
                }
            
                _enemies.Add(spawnedEnemy);
            
                spawnedEnemy.OnTurnFinished += SetNextEnemyTurn;
                spawnedEnemy.OnDeath += HandleEnemyDeath;
            
                remainginWeight -= spawnedEnemy.Weight;
            }
        }
        
        if (_currentRoom.Type == RoomData.RoomType.END)
        {
            foreach (var enemy in _enemies)
            {
                enemy.SetParticleSystemActive(true);
            }
        }
    }
    
    private void HandleEnemyStatBoost()
    {
        if (_currentRoom.Type == RoomData.RoomType.END)
        {
            foreach (var enemyData in _enemiesData)
            {
                enemyData.MaxHP.AddValue(Random.Range(5, 10));
                enemyData.Attack.AddValue(Random.Range(4, 5));
            }

            foreach (var spawnedEnemyData in _spawnedEnemiesData)
            {
                spawnedEnemyData.MaxHP.AddValue(Random.Range(3, 4));
                spawnedEnemyData.Attack.AddValue(Random.Range(2, 3));
            }
        }
        else
        {
            if (_roomVisitedNbr == 3)
            {
                foreach (var enemyData in _enemiesData)
                {
                    enemyData.MaxHP.AddValue(Random.Range(2, 3));
                    enemyData.Attack.AddValue(Random.Range(1, 2));
                }

                foreach (var spawnedEnemyData in _spawnedEnemiesData)
                {
                    spawnedEnemyData.MaxHP.AddValue(Random.Range(1, 2));
                    spawnedEnemyData.Attack.AddValue(Random.Range(1, 2));
                }
            }
        }
    }

    private void HandleEnemySpawnWeight()
    {
        if (_roomVisitedNbr == 4)
        {
            _minEnemySpawnWeight.AddValue(1);
            _maxEnemySpawnWeight.AddValue(1);

            _roomVisitedNbr = 0;
        }
    }
    
    private void DebuffEnemies(BattleManager arg1, int arg2)
    {
        if (_currentRoom.Type == RoomData.RoomType.END)
        {
            foreach (var enemyData in _enemiesData)
            {
                enemyData.MaxHP.SubstractValue(5);
                enemyData.Attack.SubstractValue(3);
            }

            foreach (var spawnedEnemyData in _spawnedEnemiesData)
            {
                spawnedEnemyData.MaxHP.SubstractValue(3);
                spawnedEnemyData.Attack.SubstractValue(2);
            }
        }
    }
    
    private BaseEnemy GetRandomEnemyUnderWeight(int weight)
    {
        return (BaseEnemy)(_enemiesData.Where(enemyUnit => (
            (BaseEnemy)enemyUnit.BaseUnitPrefab).Weight <= weight).OrderBy(
            o => Random.value).First()).BaseUnitPrefab;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        _gridManager = GridManager.Instance;
        _uiBattleManager = UIBattleManager.Instance;
        
        // Instantiate the hero.
        var randomPrefab = GetRandomUnit<BaseHero>(Faction.Hero);

        BaseHero heroPlayer = FindObjectOfType<BaseHero>();
        
        var rndSpawnedTile = _gridManager.GetHeroSpawnTile();

        heroPlayer.transform.position = rndSpawnedTile.transform.position;
        
        heroPlayer.OnDeath += HandleHeroDeath;
        
        heroPlayer.PreviousOccupiedTiles = heroPlayer.GetOccupiedTiles();
        
        foreach (var tile in heroPlayer.GetOccupiedTiles())
        {
            tile.SetUnit(heroPlayer);
        }
        
        _heroes.Add(heroPlayer);
        
        OnHeroSpawn?.Invoke(this, heroPlayer);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SpawnHeroOutsideShop(ShopManager shop)
    {
        SpawnHeroes(shop.Door);
    }
    
    public void SpawnHeroes(DoorTileCell doorTile)
    {
        _heroSpawnPos = _gridManager.WorldToCellCenter(doorTile.GetNextRoomSpawnPos());
        
        foreach (var hero in _heroes)
        {
            if (doorTile.GetRoomNeighbour().HasEnemiesToFight)
            {
                OnHeroSpawn?.Invoke(this, hero);
            }
            
            hero.transform.position = _heroSpawnPos;

            foreach (var tile in hero.PreviousOccupiedTiles)
            {
                tile.OccupiedUnit = null;
            }
            
            hero.PreviousOccupiedTiles = hero.GetOccupiedTiles();

            foreach (var tile in hero.GetOccupiedTiles())
            {
                tile.SetUnit(hero);
            }
        }
    }

    private void HandleHeroDeath(BaseUnit obj)
    {
        obj.GetComponent<BaseHero>().OnDeath -= HandleHeroDeath;
    }
    
    public void SetNextEnemyTurn()
    {
        StartCoroutine(WaitBeforeNextActionCo());
    }

    private void HandleEnemyDeath(BaseUnit obj)
    {
        StartCoroutine(UnitDeathCo(obj));
    }

    private IEnumerator UnitDeathCo(BaseUnit obj)
    {
        obj.GetComponent<BaseEnemy>().OnTurnFinished -= SetNextEnemyTurn;
        obj.GetComponent<BaseEnemy>().OnDeath -= HandleEnemyDeath;
        
        yield return new WaitForSeconds(0.4f);
        
        _enemies.Remove((BaseEnemy)obj);

        if (_enemies.Count == 0)
        {
            _currentRoom.HasEnemiesToFight = false;
            _currentRoom.SetDoorsOpen(true);
        }
    }
    
    private T GetRandomUnit<T>(Faction faction) where T : BaseUnit
    {
        return (T)_heroesData.Where(u => u.Faction == faction).OrderBy
            (o => Random.value).First().BaseUnitPrefab;
    }

    private BaseEnemy GetAnEnemyByType(EnemyType type)
    {
        var test = (BaseEnemy)_enemiesData.Find(
            x => x.BaseUnitPrefab.GetComponent<BaseEnemy>().Type == type).BaseUnitPrefab;

        return test;
    }
    
    private IEnumerator WaitBeforeNextActionCo()
    {
        yield return new WaitForSeconds(_betweenTurnsTime);
        
        if (_enemyCount < _enemies.Count - 1)
        {
            _enemyCount++;
            _currentEnemyPlaying = _enemies[_enemyCount];
        }
        else
        {
            OnEnemiesTurnEnd?.Invoke();
            _enemyCount = 0;
        }
    }
}
