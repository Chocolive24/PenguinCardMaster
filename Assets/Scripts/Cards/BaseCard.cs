using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public enum Rarety
{
    Basic = 10,
    Rare = 7,
    Epic = 3,
    Legendary = 1
}

public enum CardType
{
    MOVE_CARD,
    BASE_ATTACK_CARD,
    AOE_ATTACK_CARD,
}

public abstract class BaseCard : Collectible
{
    public BaseCard(string name, int manaCost, Rarety rarety, CardType cardType, HeroClass heroClass, 
                    int aeraOfEffect)
    {
        _name = name;
        _manaCost = manaCost;
        _rarety = rarety;
        _cardType = cardType;
        _heroClass = heroClass;
        _aeraOfEffect = aeraOfEffect;
    }

    // Attributes ------------------------------------------------------------------------------------------------------

    #region Tweakable Values

    // Constructor Values
    [SerializeField] protected string _name;
    [SerializeField] protected int _manaCost;
    [SerializeField] protected Rarety _rarety;
    [SerializeField] protected CardType _cardType;
    [SerializeField] protected HeroClass _heroClass;
    [SerializeField] protected int _aeraOfEffect;
    [SerializeField] protected Neighbourhood.NeighboursType _neighboursType;

    // Other Values
    [SerializeField] private float _moveTime = 0.2f;

    protected Dictionary<Neighbourhood.Direction, Vector2> _neighboursData;

    #endregion

    #region Tile Attributes

    protected Tilemap _aoeTilemap;
    
    protected Dictionary<Vector3, int> _availableTiles;

    #endregion

    #region State Pattern Attributes

    protected StateMachine _stateMachine;

    protected CollectibleCardState _collectibleCardState;
    protected InDeckCardState _inDeckCardState;
    protected DrawnCardState _drawnCardState;
    protected PerfomCardState _perfomCardState;
    protected DiscardedCardState _discardedCardState;

    #endregion
    
    protected int _handIndex;

    protected bool _hasBeenPlayed = false;

    protected BoxCollider2D _boxCollider2D;

    protected bool _isHeroSubscibed = false;

    protected bool _isMouveOver;

    // References ------------------------------------------------------------------------------------------------------

    #region Managers

    protected CardPlayedManager _cardPlayedManager;
    protected GridManager _gridManager;
    protected TilemapsManager _tilemapsManager;
    protected UIBattleManager _uiBattleManager;
    protected UnitsManager _unitsManager;
    
    #endregion

    [SerializeField] protected ScriptableCard _cardData;
    
    [SerializeField] protected TextMeshProUGUI _manaNbrTxt;
    [SerializeField] protected TextMeshProUGUI _cardEffectTxt;
    private bool _canBePlayed;

    // Events ----------------------------------------------------------------------------------------------------------
    public static event Action<BaseCard> OnDrawn;
    public static event Action<BaseCard> OnPlayEnter; 
    public static event Action<BaseCard> OnPlayExit;
    public static event Action<BaseCard> OnSwitchCard;
    public static  event Action<BaseCard> OnPerformed;

    public static event Action<BaseCard> OnNoTEnoughMana;

    // Getters and Setters ---------------------------------------------------------------------------------------------

    #region Getters and Setters

    public ScriptableCard CardData => _cardData;

    public Rarety Rarety => _rarety;
    public int ManaCost
    {
        get => _manaCost;
        set => _manaCost = value;
    }
    public CardType CardType => _cardType;
    public HeroClass HeroClass => _heroClass;
    public int AeraOfEffect => _aeraOfEffect;
    public int HandIndex { get => _handIndex; set => _handIndex = value; }

    public Dictionary<Vector3, int> AvailableTiles => _availableTiles;

    public Tilemap AoeTilemap
    {
        get => _aoeTilemap;
        set => _aoeTilemap = value;
    }
    
    public bool HasBeenPlayed
    {
        get => _hasBeenPlayed;
        set => _hasBeenPlayed = value;
    }

    public bool CanBePlayed
    {
        get => _canBePlayed;
        set => _canBePlayed = value;
    }

    #endregion
    
    // Methods ---------------------------------------------------------------------------------------------------------
    protected virtual void Awake()
    {
        base.Awake();
        
        _availableTiles = new Dictionary<Vector3, int>();
        
        _boxCollider2D = GetComponent<BoxCollider2D>();

        _neighboursData = Neighbourhood.NeighboursTypeToDico(_neighboursType);
        
        UpdateText();
        
        BaseHero.OnShuffleHandBackToDeck += ResetState;

        BaseUnit.OnPathEnded += UpdateAoeTilemap;
    }

    protected virtual void OnDestroy()
    {
        base.OnDestroy();
        
        BaseHero.OnShuffleHandBackToDeck -= ResetState;
        
        BaseUnit.OnPathEnded -= UpdateAoeTilemap;
    }

    private void OnEnable()
    {
        UpdateText();
    }

    protected virtual void UpdateText()
    {
        _manaNbrTxt.text = _manaCost.ToString();
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        ReferenceManagers();
        
        CreateStatePattern();
    }

    private void ReferenceManagers()
    {
        _cardPlayedManager = CardPlayedManager.Instance;
        _gridManager = GridManager.Instance;
        _tilemapsManager = TilemapsManager.Instance;
        _uiBattleManager = UIBattleManager.Instance;
        _unitsManager = UnitsManager.Instance;
    }

    private void CreateStatePattern()
    {
        _collectibleCardState = new CollectibleCardState(this);
        _inDeckCardState = new InDeckCardState();
        _drawnCardState = new DrawnCardState(this);
        _perfomCardState = new PerfomCardState(this);
        _discardedCardState = new DiscardedCardState(this);

        _stateMachine = new StateMachine();

        _stateMachine.AddTransition(_collectibleCardState, _inDeckCardState, () => _isCollected);
        _stateMachine.AddTransition(_inDeckCardState, _drawnCardState, () => gameObject.activeSelf);
        _stateMachine.AddTransition(_drawnCardState, _perfomCardState, () => CheckIfIsPlayed());
        _stateMachine.AddTransition(_perfomCardState, _inDeckCardState, () => CheckIfHasPerformed());
        
        _stateMachine.SetState(_collectibleCardState);
    }
    
    // Update is called once per frame
    protected virtual void Update()
    {
        _stateMachine.Tick();
    }   
    
    private void ResetState(BaseHero hero)
    {
        if (hero.CardHand.Contains(this))
        {
            _stateMachine.SetState(_inDeckCardState);
        }
    }
    
    public void OnCardClick()
    {
        if (_isCollected && _canBePlayed)
        {
            PlayCard();
        }
        else
        {
            if (_aoeTilemap)
            {
                Destroy(_aoeTilemap.gameObject);
                _isMouveOver = false;
            }
        }
    }

    public void PlayCard()
    {
        if (_cardPlayedManager.CurrentCard)
        {
            BaseCard currentCard = _cardPlayedManager.CurrentCard;
            
            if (currentCard == this)
            {
                StartCoroutine(InterpolateMoveCo(transform.position,
                    _cardPlayedManager.CardSlots[_handIndex].position));
                
                OnPlayExit?.Invoke(this);
            }
            else
            {
                if (_unitsManager.HeroPlayer.CurrentMana >= _manaCost)
                {
                    StartCoroutine(currentCard.InterpolateMoveCo(currentCard.transform.position,
                        _cardPlayedManager.CardSlots[currentCard.HandIndex].position));
                    
                    StartCoroutine(InterpolateMoveCo(transform.position,
                        _cardPlayedManager.CardLocation.position));
                    
                    OnSwitchCard?.Invoke(this);
                }
                else
                {
                    OnNoTEnoughMana?.Invoke(this);
                }
            }
        }
        
        else if (CheckIfCanBePlayed())
        {
            Vector3 pos = _cardPlayedManager.CardLocation.position;
            pos.z = 0;
            
            StartCoroutine(InterpolateMoveCo(transform.position, pos));
            
            OnPlayEnter?.Invoke(this);
        }
    }

    public abstract void ActivateCardEffect(TileCell tile);

    public virtual void OnEventTriggerEnter()
    {
        _isMouveOver = true;
        
        if (!_isCollected)
        {
            _aoeTilemap = _tilemapsManager.InstantiateTilemap(_name + " aoe");
            
            GetAvailableTilesInCardRenderer();
            
            this.DrawTilemap(_availableTiles, _aoeTilemap, _tilemapsManager.GetRuleTile(this));
        }

        if (!_aoeTilemap && _isCollected && !_unitsManager.HeroPlayer.IsMoving)
        {
            _aoeTilemap = _tilemapsManager.InstantiateTilemap(_name + " aoe");

            this.GetAvailableTiles();
            
            this.DrawTilemap(_availableTiles, _aoeTilemap, _tilemapsManager.GetRuleTile(this));
        }
    }

    protected virtual void GetAvailableTilesInCardRenderer()
    {
        _availableTiles = _tilemapsManager.GetAvailableTilesInCardRenderer(
            _tilemapsManager.GetCardAoeRendererCenter(), _aeraOfEffect, _neighboursData,
            false, true);
    }

    public virtual void OnEventTriggerExit()
    {
        _isMouveOver = false;
        
        if (!_isCollected && _aoeTilemap)
        {
            Destroy(_aoeTilemap.gameObject);
        }
        
        else if (_cardPlayedManager.CurrentCard != this && _aoeTilemap)
        {
            Destroy(_aoeTilemap.gameObject);
        }
    }
    
    private bool CheckIfCanBePlayed()
    {
        if (_manaCost > _unitsManager.HeroPlayer.CurrentMana)
        {
            OnNoTEnoughMana?.Invoke(this);
        }
        
        return !_cardPlayedManager.HasACardOnIt && 
               _unitsManager.HeroPlayer.CurrentMana >= _manaCost && _unitsManager.HeroPlayer.CanPlay;
    }
    
    protected virtual bool CheckIfIsPlayed()
    {
        return transform.position == _cardPlayedManager.CardLocation.transform.position;
    }

    protected abstract bool CheckIfHasPerformed();
    
    
    public virtual void GetAvailableTiles()
    {
        _availableTiles = _tilemapsManager.GetAvailableTilesInRange(
            _gridManager.WorldToCellCenter(GetStartingTile()),
            _aeraOfEffect, _neighboursData, false, true);
    }

    public virtual void DrawTilemap(Dictionary<Vector3, int> availableNeighbours, 
                                    Tilemap tilemap, RuleTile ruleTile)
    {
        _tilemapsManager.DrawTilemap(availableNeighbours, tilemap, ruleTile);
    }

    // TODO Would not work with cards that not start from the player !!!!
    public virtual Vector3 GetStartingTile()
    {
        return _gridManager.WorldToCellCenter(_unitsManager.HeroPlayer.transform.position);
        
        // foreach (var item in _gridManager.Tiles)
        // {
        //     if (item.Value.OccupiedUnit)
        //     {
        //         if (item.Value.OccupiedUnit.Faction == Faction.Hero)
        //         {
        //             return item.Value;
        //         }
        //     }
        // }
        //
        // return null;
    }
    
    private void UpdateAoeTilemap(BaseUnit obj)
    {
        if (_isMouveOver)
        {
            if (_aoeTilemap)
            {
                Destroy(_aoeTilemap.gameObject);
            }
            
            _aoeTilemap = _tilemapsManager.InstantiateTilemap(_name + " aoe");

            this.GetAvailableTiles();
            
            this.DrawTilemap(_availableTiles, _aoeTilemap, _tilemapsManager.GetRuleTile(this));
        }
    }

    public IEnumerator InterpolateMoveCo(Vector3 startPos, Vector3 endPos) 
    {
        float countTime = 0;
        
        while( countTime <= _moveTime) 
        { 
            float percentTime = countTime / _moveTime;
            transform.position = Vector3.Lerp(startPos, endPos, percentTime);
            
            yield return null; // wait for next frame
            countTime += Time.deltaTime;
        }
        
        // because of the frame rate and the way we are running Lerp(),
        // the last timePercent in the loop may not = 1
        // therefore, this line ensures we end exactly where desired.
        transform.position = endPos;
    }

    public void EnterDrawn()
    {
        OnDrawn?.Invoke(this);
        gameObject.SetActive(true);
        _hasBeenPlayed = false;
        _canBePlayed = true;
    }

    public void ExitPerform()
    {
        if (_isCollected)
        {
            OnPerformed?.Invoke(this);
            _cardPlayedManager.HandlePlayedCard();
            gameObject.SetActive(false);
        }
    }

    public void EnterDiscarded()
    {
        gameObject.SetActive(false);
    }

    public abstract void ResetProperties();
}