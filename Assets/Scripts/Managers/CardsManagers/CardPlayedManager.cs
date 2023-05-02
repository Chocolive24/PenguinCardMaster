using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CardPlayedManager : MonoBehaviour
{
    // Singleton -------------------------------------------------------------------------------------------------------
    private static CardPlayedManager _instance;
    public static CardPlayedManager Instance { get { return _instance; } }

    // Attributes ------------------------------------------------------------------------------------------------------
    [SerializeField] private Texture2D _baseMouseCursor;

    private BaseCard _currentCard;
    
    #region Card Emplacements

    [Header("Card Emplacements")]
    [SerializeField] private RectTransform _cardLocation;
    [SerializeField] public RectTransform _cardLimit;
    [SerializeField] private RectTransform[] _cardSlots;
    [SerializeField] private bool[] _availableCardSlots;

    #endregion

    #region Decks

    [Header("Paladin's Discard Decks")]
    [SerializeField] private DiscardDeckController _paladinMovDiscDeckContr;
    [SerializeField] private DiscardDeckController _paladinMainDiscDeckContr;


    #endregion

    #region Has a Card on it Booleans

    private bool _hasACardOnIt;
    private bool _hasAMoveCardOnIt = false;
    private bool _hasAnAttackCardOnIt = false;
    private bool _hasADefendCardOnIt = false;

    #endregion
    
    // References ------------------------------------------------------------------------------------------------------
    private TilemapsManager _tilemapsManager;
    private GridManager _gridManager;
    private UnitsManager _unitsManager;
    
    // Getters and Setters ---------------------------------------------------------------------------------------------

    #region Getters and Setters

    public BaseCard CurrentCard
    {
        get => _currentCard;
        set => _currentCard = value;
    }
    public bool HasACardOnIt => _hasAMoveCardOnIt || _hasAnAttackCardOnIt || _hasADefendCardOnIt;

    public RectTransform[] CardSlots => _cardSlots;
    public bool[] AvailableCardSlots
    {
        get => _availableCardSlots;
        set => _availableCardSlots = value;
    }

    public RectTransform CardLocation => _cardLocation;

    #endregion
    
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

        _availableCardSlots = new bool[_cardSlots.Length];

        for (int i = 0; i < _availableCardSlots.Length; i++)
        {
            _availableCardSlots[i] = true;
        }

        BaseCard.OnPlayEnter += OnCardEnter;
        BaseCard.OnPlayExit += OnCardExit;
        
        TileCell.OnTileSelected += PlayCurrentCard;
    }

    private void OnDestroy()
    {
        BaseCard.OnPlayEnter -= OnCardEnter;
        BaseCard.OnPlayExit -= OnCardExit;
        
        TileCell.OnTileSelected -= PlayCurrentCard;
    }

    // Start is called before the first frame update
    void Start()
    {
        _tilemapsManager = TilemapsManager.Instance;
        _gridManager = GridManager.Instance;
        _unitsManager = UnitsManager.Instance;

        Cursor.SetCursor(_baseMouseCursor, Vector2.zero, CursorMode.ForceSoftware);
    }
    
    private void OnCardEnter(BaseCard card)
    {
        if (!HasACardOnIt)
        {
            _currentCard = card;
            
            if (card.GetComponent<BaseMoveCard>())
            {
                _hasAMoveCardOnIt = true;
            }
            else if (card.GetComponent<BaseAttackCard>())
            {
                _hasAnAttackCardOnIt = true;
            }

            DrawCurrentCardTilemap(card);
        }
    }

    private void OnCardExit(BaseCard obj)
    {
        // TODO Change the mouse cursor
        
        ClearCurrentCardTilemap();
        
        ClearCardLocation();
    }
    
    private void DrawCurrentCardTilemap(BaseCard card)
    {
        if (!card.AoeTilemap)
        {
            card.AoeTilemap = _tilemapsManager.InstantiateTilemap(card.name + " aoe");
            
            card.GetAvailableTiles(); 
        
            card.DrawTilemap(card.AvailableTiles, card.AoeTilemap, _tilemapsManager.GetRuleTile(card));
        }
    }
    
    private void PlayCurrentCard(TileCell tile)
    {
        if (_currentCard)
        {
            _currentCard.ActivateCardEffect(tile);
        }
    }

    /// <summary>
    /// Manage the actions to do when a card has been played.
    /// </summary>
    public void HandlePlayedCard()
    {
        if (_currentCard)
        {
            _unitsManager.HeroPlayer.CurrentMana -= _currentCard.ManaCost;
            
            // Free the current card slot.
            _availableCardSlots[_currentCard.HandIndex] = true;

            _currentCard.ResetProperties();
        
            ClearCurrentCardTilemap();

            MoveCardToHisDiscardPile();

            ClearCardLocation();
        }
        
        // TODO Change the mouse cursor
    }
    
    private void ClearCurrentCardTilemap()
    {
        if (_currentCard)
        {
            if (_currentCard.AoeTilemap)
            {
                Destroy(_currentCard.AoeTilemap.gameObject);

                if (_currentCard.CardType == CardType.MOVE_CARD)
                {
                    BaseMoveCard card = _currentCard.GetComponent<BaseMoveCard>();
            
                    foreach (var item in card.Path)
                    {
                        var tile = _gridManager.GetTileAtPosition(item.Key);
                        tile.Arrow.SetActive(false);
                    }
                }
            }
        }
    }
    
    private void MoveCardToHisDiscardPile()
    {
        if (_currentCard.CardType == CardType.MOVE_CARD)
        {
            if (_currentCard.HeroClass == HeroClass.PALADIN)
            {
                MoveToDiscardPile(_paladinMovDiscDeckContr, _unitsManager.HeroPlayer.MovementDeck);
            }
        }
        else if (_currentCard.CardType == CardType.BASE_ATTACK_CARD || 
                 _currentCard.CardType == CardType.AOE_ATTACK_CARD)
        {
            if (_currentCard.HeroClass == HeroClass.PALADIN)
            {
                MoveToDiscardPile(_paladinMainDiscDeckContr, _unitsManager.HeroPlayer.MainDeck);
            }
        }
        
        _currentCard.HasBeenPlayed = true;
    }

    private void MoveToDiscardPile(DiscardDeckController discardDeckController, DeckController deck)
    {
        discardDeckController.AddACard(_currentCard, deck);
    }

    private void ClearCardLocation()
    {
        _currentCard = null;
        
        _hasAMoveCardOnIt = false;
        _hasAnAttackCardOnIt = false;
        _hasADefendCardOnIt = false;
    }

    public void ResetSlots()
    {
        for (int i = 0; i < _availableCardSlots.Length; i++)
        {
            _availableCardSlots[i] = true;
        }
    }
}