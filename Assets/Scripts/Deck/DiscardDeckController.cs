using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum DiscardCardType
{
    Movement,
    Main
}

public class DiscardDeckController : MonoBehaviour
{
    [SerializeField] protected List<BaseCard> _discardDeck;

    [SerializeField] protected HeroClass _heroClass;
    
    [SerializeField] private TextMeshProUGUI _carNbrTxt;

    [SerializeField] private DiscardCardType _discardCardType;
    
    // References ------------------------------------------------------------------------------------------------------
    private UnitsManager _unitsManager;

    private DeckController _deck;
    
    // Events ----------------------------------------------------------------------------------------------------------
    public static event Action<DiscardDeckController> OnDiscarFull; 

    // Getters and Setters ---------------------------------------------------------------------------------------------
    public List<BaseCard> DiscardDeck
    {
        get => _discardDeck;
        set => _discardDeck = value; 
    }
   
    public HeroClass HeroClass => _heroClass;
    public DiscardCardType DiscardCardType => _discardCardType;

    public DeckController Deck
    {
        get => _deck;
        set => _deck = value;
    }

    // -----------------------------------------------------------------------------------------------------------------

    private void Awake()
    {
        BattleManager.OnBattleStart += SetDiscardDeck;
    }

    private void OnDestroy()
    {
        BattleManager.OnBattleStart -= SetDiscardDeck;
    }

    private void SetDiscardDeck(BattleManager obj, RoomData room)
    {
        if (!_unitsManager)
        {
            _unitsManager = UnitsManager.Instance;
        }
        
        _carNbrTxt.text = _discardDeck.Count.ToString();
        
        if (!_unitsManager.HeroPlayer.MainDiscardDeck)
        {
            if (_discardCardType == DiscardCardType.Main)
            {
                _unitsManager.HeroPlayer.MainDiscardDeck = this;
            }
        }
        if (!_unitsManager.HeroPlayer.MovDiscardDeck)
        {
            if (_discardCardType == DiscardCardType.Movement)
            {
                _unitsManager.HeroPlayer.MovDiscardDeck = this;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// I'm sorry for this thing but I was late for the playTest so I implement this stuff quickly.
    /// </summary>
    /// <param name="deckController"></param>
    /// <returns></returns>

    public void AddACard(BaseCard card, DeckController deck)
    {
        _discardDeck.Add(card);
        
        if (_discardDeck.Count == deck.Size)
        {
            OnDiscarFull?.Invoke(this);
        }
        
        _carNbrTxt.text = _discardDeck.Count.ToString();
    }
    
    public void ShuffleCardsBackToDeck()
    {
        foreach (var card in _discardDeck)
        {
            _deck.Deck.Add(card);
        }
        
        _deck.UpdateCardTxtNbr();
        _discardDeck.Clear();
    }
}
