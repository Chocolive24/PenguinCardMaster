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

    private List<BaseHero> _heroes;
    private List<BaseEnemy> _enemies;
    
    private BaseEnemy _currentEnemyPlaying;

    private int _enemyCount = 0;

    // References ------------------------------------------------------------------------------------------------------
    private GridManager _gridManager;
    private UIBattleManager _uiBattleManager;

    [SerializeField] private AI_TypeSO _aiTypeSO;
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

        _heroesData = Resources.LoadAll<HeroData>("Units").ToList();
        _enemiesData = Resources.LoadAll<EnemyData>("Units/Enemies").ToList();
        
        _heroes = new List<BaseHero>();
        _enemies = new List<BaseEnemy>();

        DoorTileCell.OnDoorTileEnter += SpawnHeroes;
        DoorTileCell.OnDoorTileEnter += SpawnRoomEnemies;
        ShopManager.OnShopExit += SpawnHeroOutsideShop;

        //Room.OnRoomEnter += SpawnEntities;
    }

    private void OnDestroy()
    {
        DoorTileCell.OnDoorTileEnter -= SpawnHeroes;
        DoorTileCell.OnDoorTileEnter -= SpawnRoomEnemies;
        ShopManager.OnShopExit -= SpawnHeroOutsideShop;
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
            int remainginWeight = _currentRoom.EnemySpawnWeight;
        
            while (remainginWeight > 0)
            {
                //BaseEnemy enemy = GetAnEnemyByType(EnemyType.GOBLIN);
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
                            break;
                        }

                        if (!tile.Walkable)
                        {
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
    }

    private BaseEnemy GetRandomEnemyUnderWeight(int weight)
    {
        return (BaseEnemy)(_enemiesData.Where(enemyUnit => (
            (BaseEnemy)enemyUnit.BaseUnitPrefab).Weight <= weight).OrderBy(
            o => Random.value).First()).BaseUnitPrefab;
    }
    
    // private void SpawnEntities(Room room)
    // {
    //     SpawnHeroes();
    //     HandleSpawnEnemies();
    // }


    // Start is called before the first frame update
    void Start()
    {
        _gridManager = GridManager.Instance;
        _uiBattleManager = UIBattleManager.Instance;
        
        // Instantiate the hero.
        var randomPrefab = GetRandomUnit<BaseHero>(Faction.Hero);
        var rndSpawnedTile = _gridManager.GetHeroSpawnTile();

        var spawnedHero = Instantiate(randomPrefab, rndSpawnedTile.transform.position, Quaternion.identity);
        
        spawnedHero.OnDeath += HandleHeroDeath;
        
        spawnedHero.PreviousOccupiedTiles = spawnedHero.GetOccupiedTiles();
        
        foreach (var tile in spawnedHero.GetOccupiedTiles())
        {
            tile.SetUnit(spawnedHero);
        }
        
        _heroes.Add(spawnedHero);
        
        OnHeroSpawn?.Invoke(this, spawnedHero);
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
        _heroSpawnPos = doorTile.GetNextRoomSpawnPos();
        
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

    public void HandleSpawnEnemies()
    {
        switch (_aiTypeSO.Type)
        {
            case EnemyType.GOBLIN:
                SpawnEnemy(3, EnemyType.GOBLIN);
                break;
            case EnemyType.ARCHER:
                SpawnEnemy(3, EnemyType.ARCHER);
                break;
            case EnemyType.TANK:
                SpawnEnemy(2, EnemyType.TANK);
                break;
            case EnemyType.SPAWNER:
                SpawnEnemy(1, EnemyType.SPAWNER);
                break;
            case EnemyType.MIX:
                SpawnEnemy(1, EnemyType.MIX);
                break;
        }
        
        // var enemyCount = 3;
        //
        // for (int i = 0; i < enemyCount; i++)
        // {
        //     var randomPrefab = GetRandomUnit<BaseEnemy>(Faction.Enemy);
        //     var spawnedEnemy = Instantiate(randomPrefab);
        //     var randomSpawnTile = _gridManager.GetEnemySpawnTile();
        //     spawnedEnemy.transform.position = randomSpawnTile.transform.position;
        //     randomSpawnTile.SetUnit(spawnedEnemy);
        //     _enemies.Add(spawnedEnemy);
        //     
        //     spawnedEnemy.OnTurnFinished += SetNextEnemyTurn;
        // }
    }
    
    public void SpawnEnemy(int enemyNbr, EnemyType enemyType)
    {
        for (int i = 0; i < enemyNbr; i++)
        {
            BaseEnemy enemyData;
            
            if (enemyType == EnemyType.MIX)
            {
                // BaseEnemy enemyData2;
                //
                // BaseEnemy enemyData3;
                //
                // enemyData = GetAnEnemyByType(EnemyType.GOBLIN);
                //
                // var spawnedEnemy = Instantiate(enemyData);
                //
                // spawnedEnemy.transform.position = _gridManager.WorldToCellCenter(new Vector3(0, 6.5f, 0));
                //
                // spawnedEnemy.PreviousOccupiedTiles = spawnedEnemy.GetOccupiedTiles();
                //
                // foreach (var tile in spawnedEnemy.GetOccupiedTiles())
                // {
                //     tile.SetUnit(spawnedEnemy);
                // }
                //
                // //randomSpawnTile.SetUnit(spawnedEnemy);
                // _enemies.Add(spawnedEnemy);
                //
                // spawnedEnemy.OnTurnFinished += SetNextEnemyTurn;
                // spawnedEnemy.OnDeath += HandleEnemyDeath;
                //
                // // ----------------------------------------------------------------------------------------
                //
                // // enemyData = GetAnEnemyByType(EnemyType.GOBLIN);
                // //
                // // var spawnedEnemy = Instantiate(enemyData);
                // //
                // // spawnedEnemy.transform.position = _gridManager.WorldToCellCenter(new Vector3(0, 1.5f, 0));
                // //
                // // spawnedEnemy.PreviousOccupiedTiles = spawnedEnemy.GetOccupiedTiles();
                // //
                // // foreach (var tile in spawnedEnemy.GetOccupiedTiles())
                // // {
                // //     tile.SetUnit(spawnedEnemy);
                // // }
                // //
                // // //randomSpawnTile.SetUnit(spawnedEnemy);
                // // _enemies.Add(spawnedEnemy);
                // //
                // // spawnedEnemy.OnTurnFinished += SetNextEnemyTurn;
                // // spawnedEnemy.OnDeath += HandleEnemyDeath;
                //
                // // ----------------------------------------------------------------------------------------
                //
                // enemyData2 = GetAnEnemyByType(EnemyType.TANK);
                //
                // var spawnedEnemy2 = Instantiate(enemyData2);
                //
                // spawnedEnemy2.transform.position = _gridManager.WorldToCellCenter(new Vector3(3, 6.5f, 0));
                //
                // spawnedEnemy2.PreviousOccupiedTiles = spawnedEnemy2.GetOccupiedTiles();
                //
                // foreach (var tile in spawnedEnemy2.GetOccupiedTiles())
                // {
                //     tile.SetUnit(spawnedEnemy2);
                // }
                //
                // //randomSpawnTile.SetUnit(spawnedEnemy);
                // _enemies.Add(spawnedEnemy2);
                //
                // spawnedEnemy2.OnTurnFinished += SetNextEnemyTurn;
                // spawnedEnemy2.OnDeath += HandleEnemyDeath;

                do
                {
                    enemyData = GetRandomUnit<BaseEnemy>(Faction.Enemy);
                } while (enemyData.UnitName == "Minion");
            }
            else
            {
                enemyData = GetAnEnemyByType(enemyType);
            }
            
            var spawnedEnemy = Instantiate(enemyData);
            
            var randomSpawnTile = _gridManager.GetEnemySpawnTile();
            spawnedEnemy.transform.position = randomSpawnTile.transform.position;
            
            spawnedEnemy.PreviousOccupiedTiles = spawnedEnemy.GetOccupiedTiles();
            
            foreach (var tile in spawnedEnemy.GetOccupiedTiles())
            {
                tile.SetUnit(spawnedEnemy);
            }
            
            //randomSpawnTile.SetUnit(spawnedEnemy);
            _enemies.Add(spawnedEnemy);
            
            spawnedEnemy.OnTurnFinished += SetNextEnemyTurn;
            spawnedEnemy.OnDeath += HandleEnemyDeath;
        }
    }

    

    public void SetNextEnemyTurn()
    {
        StartCoroutine(WaitBeforeNextActionCo());
    }

    private void HandleEnemyDeath(BaseUnit obj)
    {
        _enemies.Remove((BaseEnemy)obj);

        if (_enemies.Count == 0)
        {
            _currentRoom.HasEnemiesToFight = false;
            _currentRoom.SetDoorsOpen(true);
        }

        obj.GetComponent<BaseEnemy>().OnTurnFinished -= SetNextEnemyTurn;
        obj.GetComponent<BaseEnemy>().OnDeath -= HandleEnemyDeath;
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
            //_currentEnemyPlaying.BehaviorTree.SetupTree();
        }
        else
        {
            OnEnemiesTurnEnd?.Invoke();
            _enemyCount = 0;
        }
    }
    
    // public void SetSelectedHero(BaseHero hero)
    // {
    //     _selectedHero = hero;
    //     _uiBattleManager.ShowSelectedHero(hero);
    // }
}
