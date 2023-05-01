using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class BattleManager : MonoBehaviour
{
    // Singleton -------------------------------------------------------------------------------------------------------
    private static BattleManager _instance;
    public static BattleManager Instance { get { return _instance; } }
    
    // Attributes ------------------------------------------------------------------------------------------------------
    private RoomData _battleRoom;
    
    [SerializeField] private int _minGoldReward = 75, _maxGoldReward = 100;
    [SerializeField] private int _rewardsNbr = 3;

    [SerializeField] private int _relicRewardPercentage = 10;
    
    private bool _isPlayerTurn;

    private List<BaseCard> _cardRewards = new List<BaseCard>();
    private List<Relic> _relicRewards = new List<Relic>();

    private Vector3 _heroSpawnPos;

    private int _currentGoldReward;
    private int _rewardCount = 0;

    private bool _canStartBattle = false;

    // References ------------------------------------------------------------------------------------------------------

    #region Managers

    private GameManager _gameManager;
    private GridManager _gridManager;
    private UnitsManager _unitsManager;
    private UIBattleManager _uiBattleManager;
    private CardsManager _cardsManager;
    private RelicsManager _relicsManager;

    #endregion

    [SerializeField] private IntReference _playerGolds;
    
    // Events ----------------------------------------------------------------------------------------------------------
    public static event Action<BattleManager, RoomData> OnBattleStart;
    public static event Action<BattleManager> OnPlayerTurnStart;
    public static event Action<BattleManager> OnPlayerTurnEnd;
    public static event Action<BattleManager> OnEnemyTurnStart;
    public static event Action<BattleManager> OnEnemyTurnEnd;
    public static event Action<BattleManager, RoomData> OnBattleEnd;
    public static event Action<BattleManager, int> OnVictory; 

    // State Pattern ---------------------------------------------------------------------------------------------------
    
    #region Battle States

    private HeroesTurnBattleState _heroesTurnBattleState;
    private EnemiesTurnBattleState _enemiesTurnBattleState;
    private VictoryBattleState _victoryBattleState;
    private DefeatBattleState _defeatBattleState;
    private EndBattleState _endBattleState;
    private WaitBattleState _waitBattleState;

    #endregion
    
    // State Machine
    private StateMachine _stateMachine;
    private float _endTurnTime = 0.5f;

    // Public Methods --------------------------------------------------------------------------------------------------

    #region Getters and Setters

    public RoomData BattleRoom => _battleRoom;

    public bool IsPlayerTurn { get => _isPlayerTurn; set => _isPlayerTurn = value; }
    
    public StateMachine StateMachine => _stateMachine;

    #endregion
    
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

        UIBattleManager.OnNextReward += HandleNextReward;
        UnitsManager.OnEnemiesTurnEnd += SetTurnToPlayerTurn;
        DoorTileCell.OnDoorTileEnter += GetRoomData;
        BaseCard.OnCollected += DestroyCardRewards;
    }

    private void OnDestroy()
    {
        UIBattleManager.OnNextReward -= HandleNextReward;
        UnitsManager.OnEnemiesTurnEnd -= SetTurnToPlayerTurn;
        DoorTileCell.OnDoorTileEnter -= GetRoomData;
        BaseCard.OnCollected -= DestroyCardRewards;
    }

    private void SetTurnToPlayerTurn()
    {
        OnPlayerTurnStart?.Invoke(this);
        _isPlayerTurn = true;
    }

    private void GetRoomData(DoorTileCell doorTile)
    {
        _battleRoom = doorTile.GetRoomNeighbour();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        ReferenceManagers();

        CreateStatePattern();
    }

    private void ReferenceManagers()
    {
        _gameManager = GameManager.Instance;
        _gridManager = GridManager.Instance;
        _unitsManager = UnitsManager.Instance;
        _uiBattleManager = UIBattleManager.Instance;
        _cardsManager = CardsManager.Instance;
        _relicsManager = FindObjectOfType<RelicsManager>();
    }

    private void CreateStatePattern()
    {
        _heroesTurnBattleState = new HeroesTurnBattleState(this, _uiBattleManager);
        _enemiesTurnBattleState = new EnemiesTurnBattleState();
        _victoryBattleState = new VictoryBattleState(this);
        _defeatBattleState = new DefeatBattleState();
        _endBattleState = new EndBattleState(this);
        _waitBattleState = new WaitBattleState();
        
        _stateMachine = new StateMachine();

        _stateMachine.AddTransition(_waitBattleState, _heroesTurnBattleState, () => _canStartBattle);
        _stateMachine.AddTransition(_heroesTurnBattleState, _enemiesTurnBattleState,
            () => !_isPlayerTurn);
        _stateMachine.AddTransition(_enemiesTurnBattleState, _heroesTurnBattleState,
            () => _isPlayerTurn);
        _stateMachine.AddTransition(_heroesTurnBattleState, _victoryBattleState, 
            () => _unitsManager.Enemies.Count == 0);
        _stateMachine.AddTransition(_enemiesTurnBattleState, _victoryBattleState, 
            () => _unitsManager.Enemies.Count == 0);
        _stateMachine.AddTransition(_victoryBattleState, _endBattleState, 
            () => !_uiBattleManager.VictoryPanel.activeSelf);
    }
    
    public void StartBattle()
    {
        _stateMachine.SetState(_waitBattleState);
        StartCoroutine(nameof(StartBattleCo));
    }

    private IEnumerator StartBattleCo()
    {
        yield return new WaitForSeconds(1f);
        
        _uiBattleManager.BattlePanel.SetActive(true);
        _uiBattleManager.ExploringPanel.SetActive(false);
        
        // TODO petite anim du battlePanel

        OnBattleStart?.Invoke(this, _battleRoom);

        _canStartBattle = true;
    }
    
    public void EndBattle()
    {
        _uiBattleManager.BattlePanel.SetActive(false);
        _uiBattleManager.ExploringPanel.SetActive(true);

        _canStartBattle = false;
        
        OnBattleEnd?.Invoke(this, _battleRoom);
    }
    
    public void EnterHeroesTurn()
    {
        _isPlayerTurn = true;
        
        OnPlayerTurnStart?.Invoke(this);
        
        _uiBattleManager.EndTurnButton.interactable = true;
        _uiBattleManager.SetCurrentTurnText("Player Turn");
    }

    public void ExitHeroesTurn()
    {
        _uiBattleManager.EndTurnButton.interactable = false;
        
        OnPlayerTurnEnd?.Invoke(this);
    }

    public void StartEnemiesTurn()
    {
        OnEnemyTurnStart?.Invoke(this);
        _unitsManager.CurrentEnemyPlaying = _unitsManager.Enemies[0];
        _uiBattleManager.SetCurrentTurnText("Enemies Turn");
    }
    
    public void ExitEnemiesTurn()
    {
        _unitsManager.CurrentEnemyPlaying = null;
    }

    private IEnumerator EndTurnCo()
    {
        yield return new WaitForSeconds(_endTurnTime);
    }

    public void EnterVictory()
    {
        _uiBattleManager.VictoryPanel.SetActive(true);

        _currentGoldReward = Random.Range(_minGoldReward, _maxGoldReward);
        
        OnVictory?.Invoke(this, _currentGoldReward);
        
        
    }

    private void HandleNextReward(UIBattleManager obj)
    {
        if (_rewardCount == 0)
        {
            _playerGolds.AddValue(_currentGoldReward);
            
            _cardRewards = _cardsManager.CreateCardRewards(_rewardsNbr);

            foreach (var card in _cardRewards)
            {
                //card.OnCollected += DestroyCardRewards;
            }
        }
    }

    private void DestroyCardRewards(BaseCard obj)
    {
        _cardRewards.Remove(obj);
        
        foreach (var card in _cardRewards)
        {
            Destroy(card.gameObject);
        }
        
        _cardRewards.Clear();

        if (_battleRoom.Type != RoomData.RoomType.END)
        {
            if (Random.Range(1, 101) <= _relicRewardPercentage)
            {
                _relicRewards = _relicsManager.CreateRelicRewards();

                foreach (var relic in _relicRewards)
                {
                    relic.OnCollected += DestroyRelicRewards;
                }
            }
            else
            {
                _uiBattleManager.VictoryPanel.SetActive(false);
            }
        }
        else
        {
            _relicRewards = _relicsManager.CreateRelicRewards();

            foreach (var relic in _relicRewards)
            {
                relic.OnCollected += DestroyRelicRewards;
            }
        }
    }
    
    private void DestroyRelicRewards(Relic obj)
    {
        _relicRewards.Remove(obj);
        
        foreach (var relic in _relicRewards)
        {
            Destroy(relic.gameObject);
        }
        
        _relicRewards.Clear();
        
        _uiBattleManager.VictoryPanel.SetActive(false);
    }

    public void ExitVictory()
    {
        foreach (var card in _cardRewards)
        {
            Destroy(card.gameObject);
        }
        
        _cardRewards.Clear();

        _uiBattleManager.VictoryPanel.SetActive(false);
        
        _gameManager.IsInBattleState = false;
    }
}
