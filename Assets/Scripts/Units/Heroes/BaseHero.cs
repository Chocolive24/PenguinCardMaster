using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;

public enum HeroClass
{
    PALADIN,
    NOT_THE_HERO,
}

public class BaseHero : BaseUnit
{
    // Attributes ------------------------------------------------------------------------------------------------------

    #region Tweakable Values

    [SerializeField] protected HeroClass _heroClass;
    [SerializeField] protected IntReference _maxMana;
    [SerializeField] protected IntReference _exploreSpeed;
    [SerializeField] protected IntReference _battleSpeed;
    [SerializeField] protected IntReference _maxNbrOfCardToDrawnPerTurn;

    #endregion

    [SerializeField] protected IntReference _golds;
    
    #region Decks

    protected DeckController _movementDeck;
    protected DeckController _mainDeck;
    protected DiscardDeckController _movDiscardDeck;
    protected DiscardDeckController _mainDiscardDeck;

    #endregion

    // #region Pathfinding Attributes
    //
    // protected Vector3? _targetPos1 = null;
    // protected Dictionary<Vector3, int> _path1;
    // protected List<Vector3> _pathPositions1;
    // protected int _currentTargetIndex1 = 0;
    //
    // #endregion

    protected List<BaseCard> _cardHand = new List<BaseCard>();

    protected int _currentMana;

    private bool _canPlay;

    protected int _nbrOfCardDrawnThisTurn;
    
    // References ------------------------------------------------------------------------------------------------------
    private HeroData _heroData;

    // private GameManager _gameManager;
    // private GridManager _gridManager;
    private CardPlayedManager _cardPlayedManager;
    
    // Events ----------------------------------------------------------------------------------------------------------
    public static event Action<BaseHero> OnShuffleHandBackToDeck;

    // Getters and Setters ---------------------------------------------------------------------------------------------

    #region Getters and Setters

    public IntReference MaxMana { get => _maxMana; }

    public int CurrentMana
    {
        get => _currentMana;
        set => _currentMana = value;
    }

    public bool CanPlay => _canPlay;

    public DeckController MovementDeck
    {
        get => _movementDeck;
        set => _movementDeck = value;
    }

    public DeckController MainDeck
    {
        get => _mainDeck;
        set => _mainDeck = value;
    }

    public DiscardDeckController MovDiscardDeck
    {
        get => _movDiscardDeck;
        set => _movDiscardDeck = value;
    }

    public DiscardDeckController MainDiscardDeck
    {
        get => _mainDiscardDeck;
        set => _mainDiscardDeck = value;
    }

    public IntReference MaxNbrOfCardToDrawnPerTurn => _maxNbrOfCardToDrawnPerTurn;

    public int NbrOfCardDrawnThisTurn
    {
        get => _nbrOfCardDrawnThisTurn;
        set => _nbrOfCardDrawnThisTurn = value;
    }
    // public Dictionary<Vector3, int> Path
    // {
    //     get => _path1;
    //     set => _path1 = value;
    // }

    public List<BaseCard> CardHand => _cardHand;

    #endregion
    
    // -----------------------------------------------------------------------------------------------------------------

    protected override void Awake()
    {
        base.Awake();
        GameManager.OnGameStateChange += GameManagerOnOnGameStateChange;
        BattleManager.OnPlayerTurnStart += StartTurn;
        BattleManager.OnPlayerTurnEnd += EndTurn;
        BattleManager.OnBattleEnd += PutAllCardsInDecks;
        TileCell.OnTileSelected += FindExploringPath;
        DiscardDeckController.OnDiscarFull += ShuffleCardsBackToDeck;
        ActivableRelic.OnPerformEffect += HealWithPotion;
        ActivableRelic.OnPerformEffect += RestoreMana;
        ShopManager.OnObjectBuy += UpdateGolds;

        BaseCard.OnDrawn += AddCardToHand;
        BaseCard.OnPerformed += RemoveCardFromHand;
        BaseMoveCard.OnPathStarted += DesactivateCardInHand;

        _cardHand = new List<BaseCard>();
    }
    
    private void OnDestroy()
    {
        GameManager.OnGameStateChange -= GameManagerOnOnGameStateChange;
        BattleManager.OnPlayerTurnStart -= StartTurn;
        BattleManager.OnPlayerTurnEnd -= EndTurn;
        BattleManager.OnBattleEnd -= PutAllCardsInDecks;
        TileCell.OnTileSelected -= FindExploringPath;
        DiscardDeckController.OnDiscarFull -= ShuffleCardsBackToDeck;
        ActivableRelic.OnPerformEffect -= HealWithPotion;
        ActivableRelic.OnPerformEffect -= RestoreMana;
        ShopManager.OnObjectBuy -= UpdateGolds;
        
        BaseCard.OnDrawn -= AddCardToHand;
        BaseCard.OnPerformed -= RemoveCardFromHand;
        BaseMoveCard.OnPathStarted -= DesactivateCardInHand;
    }

    private void DesactivateCardInHand(BaseMoveCard obj)
    {
        foreach (var card in _cardHand)
        {
            card.CanBePlayed = false;
        }
    }

    protected override void SetData()
    {
        _heroData = (HeroData)_unitData;
        
        base.SetData();
        
        _heroClass = _heroData.HeroClass;
        _maxMana = _heroData.MaxMana;
        _exploreSpeed = _heroData.ExploreSpeed;
        _battleSpeed = _heroData.BattleSpeed;
      
    }

    private void FindExploringPath(TileCell tile)
    {
        if (!_gameManager.IsInBattleState)
        {
            if (_exploringPath != null)
            {
                _exploringPath.Clear();
            }
            
            Vector3 tileTargetPos = _gridManager.WorldToCellCenter(tile.transform.position);

            _exploringPath = FindPath(_gridManager.WorldToCellCenter(transform.position), tileTargetPos, 
                false, false, false);
        
            if (_exploringPath.Count > 0)
            {
                _targetPos = _exploringPath.First();
            }
            else
            {
                _targetPos = null;
            }
        }
    }

    private void GameManagerOnOnGameStateChange()
    {
        _targetPos = null;
    }
    
    private void StartTurn(BattleManager obj)
    { 
        //_unitsManager.SetSelectedHero(this);
        _currentMana = _maxMana.Value;
        _canPlay = true;
        _nbrOfCardDrawnThisTurn = 0;

        _mainDeck.SetButtonInteractavity(true);
        _movementDeck.SetButtonInteractavity(true);

        if (!_mainDiscardDeck.Deck && !_movDiscardDeck.Deck)
        {
            _mainDiscardDeck.Deck = _mainDeck;
            _movDiscardDeck.Deck = _movementDeck;
        }
        
        foreach (var card in _cardHand)
        {
            card.CanBePlayed = true;
        }
    }

    private void EndTurn(BattleManager obj)
    {
        _canPlay = false;
        _mainDeck.SetButtonInteractavity(false);
        _movementDeck.SetButtonInteractavity(false);

        foreach (var card in _cardHand)
        {
            card.CanBePlayed = false;
        }
        
        //_unitsManager.SetSelectedHero(null);
    }
    
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        _cardPlayedManager = CardPlayedManager.Instance;
        
        //InitializeDecks();
        
        _currentMana = _maxMana.Value;
    }

    public void HealWithPotion(ActivableRelic relic)
    {
        if (relic.TryGetComponent(out RescuePotion rescuePotion))
        {
            HealHp(rescuePotion.Heal);
        }
    }

    public void HealHp(int heal)
    {
        _currentHP.AddValue(heal);

        if (_currentHP.Value > _maxHP.Value)
        {
            _currentHP.SetValue(_maxHP.Value);
        }

        if (!_healthBar)
        {
            _healthBar = GetComponent<HealthBar>();
        }

        _healthBar.UpdateHealthBar(_currentHP.Value, _maxHP.Value);
    }

    private void RestoreMana(ActivableRelic relic)
    {
        if (relic.TryGetComponent(out RescueMana rescueMana))
        {
            _currentMana += rescueMana.ManaToRestore;
        }
    }
    
    private void UpdateGolds(ShopManager arg1, Collectible obj)
    {
        _golds.SubstractValue(obj.ObjectCost);
    }
    
    // Update is called once per frame
    protected override void Update()
    {
        _speed = _gameManager.IsInBattleState ? _battleSpeed : _exploreSpeed;
        
        base.Update();
    }

    private void FixedUpdate()
    {
        MoveOnGrid();
    }

    public override void FindAvailablePathToTarget(Vector3 targetPos, int minimumPathCunt, 
        bool countHeroes, bool countEnemies, bool countWalls)
    {
        if (_targetPos.HasValue && _cardPlayedManager.HasACardOnIt)
        {
            return;
        }
        
        _availableTiles = _cardPlayedManager.CurrentCard.AvailableTiles;
        base.FindAvailablePathToTarget(targetPos, 0, countHeroes, countEnemies, countWalls);
    }


    protected override void StopThePath()
    {
        base.StopThePath();
        foreach (var card in _cardHand)
        {
            card.CanBePlayed = true;
        }
    }

    private void ShuffleCardsBackToDeck(DiscardDeckController discardDeckController)
    {
        discardDeckController.ShuffleCardsBackToDeck();
    }
    
    private void PutAllCardsInDecks(BattleManager obj, RoomData battleRoom)
    {
        OnShuffleHandBackToDeck?.Invoke(this);
        
        foreach (var card in _cardHand)
        {
            if (card.CardType == CardType.BASE_ATTACK_CARD || card.CardType == CardType.AOE_ATTACK_CARD)
            {
                _mainDeck.AddCardWithoutData(card);
            }
            else if (card.CardType == CardType.MOVE_CARD)
            {
                _movementDeck.AddCardWithoutData(card);
            }

            _cardPlayedManager.ResetSlots();
        }

        _cardHand.Clear();

        
        _movDiscardDeck.ShuffleCardsBackToDeck();
        _mainDiscardDeck.ShuffleCardsBackToDeck();
    }

    public void AddCardToHand(BaseCard card)
    {
        _cardHand.Add(card);
    }

    public void RemoveCardFromHand(BaseCard card)
    {
        _cardHand.Remove(card);

        // if (card.GetComponent<BaseMoveCard>())
        // {
        //     StopThePath();
        // }
    }
}
