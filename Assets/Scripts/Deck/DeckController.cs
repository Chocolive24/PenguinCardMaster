using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public abstract class DeckController : MonoBehaviour
{
    [SerializeField] protected List<BaseCard> _deck;

    [SerializeField] protected HeroClass _heroClass;
    
    [SerializeField] protected TextMeshProUGUI _carNbrTxt;
    [SerializeField] protected TextMeshProUGUI _cardNbrDrawnTxt;
    [SerializeField] protected TextMeshProUGUI _tooMuchCardDrawnTxt;
    [SerializeField] protected Image _cardDrawnImage;
        
    [SerializeField] protected int _size;

    [SerializeField] protected Button _button;

    // References ------------------------------------------------------------------------------------------------------
    [SerializeField] protected DeckData _deckData;
    
    protected UnitsManager _unitsManager;

    [SerializeField] protected GameObject _parentObject;

    [SerializeField] protected IntReference _maxNbrOfCardToDrawnPerTurn;

    // Events ----------------------------------------------------------------------------------------------------------
    public static event Action<DeckController> OnDeckEmpty;

    // Getters and Setters ---------------------------------------------------------------------------------------------
    public List<BaseCard> Deck => _deck;
    public HeroClass HeroClass => _heroClass;

    public int Size
    {
        get => _size;
        set => _size = value;
    }

    protected virtual void Awake()
    {
        BattleManager.OnBattleStart += SetDeck;
        BattleManager.OnPlayerTurnStart += ResetValues;
        BaseMoveCard.OnPathStarted += DesactivateDeckButton;
        BaseUnit.OnPathEnded += ActivateDeckButton;
    }
    
    protected virtual void SetDeck(BattleManager battleManager, RoomData room)
    {
        if (!_unitsManager)
        {
            _unitsManager = UnitsManager.Instance;
        }
        
        UpdateCardTxtNbr();
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        _unitsManager = UnitsManager.Instance;

        UpdateNbrCardDrawnTxt();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void ResetValues(BattleManager obj)
    {
        UpdateNbrCardDrawnTxt();
    }

    public virtual void SetButtonInteractavity(bool interactable)
    {
        _button.interactable = interactable;
    }

    public void InstantiateBasicCard(List<ScriptableCard> scriptableCards, int cardNbr)
    {
        List<ScriptableCard> allreadySpawnedCards = new List<ScriptableCard>();
        foreach (var card in scriptableCards)
        {
            allreadySpawnedCards.Add(card);
        }

        if (_deckData.CardDeckData.Count == 0)
        {
            for (int i = 0; i < cardNbr; i++)
            {
                // if (i == 3)
                // {
                //     foreach (var cardData in scriptableCards)
                //     {
                //         allreadySpawnedCards.Add(cardData);
                //     }
                // }
                
                var card = CardsManager.Instance.InstantiateARandomCard(allreadySpawnedCards, Rarety.Legendary);

                card.IsCollected = true;
                
                AddCardWithData(card);

                //allreadySpawnedCards.Remove(card.CardData);
            }
        }
        else
        {
            foreach (var cardData in _deckData.CardDeckData)
            {
                var card = CardsManager.Instance.InstantiateACardFromData(cardData);
                
                card.IsCollected = true;
                
                AddCardWithoutData(card);
            }

            _size = Deck.Count;
        }
    }
    
    public void AddCardWithoutData(BaseCard card)
    {
        card.transform.parent = gameObject.transform;
        card.GetComponent<RectTransform>().position = gameObject.GetComponent<RectTransform>().position;

        card.gameObject.SetActive(false);

        card.IsCollected = true;

        _deck.Add(card);

        UpdateCardTxtNbr();
    }

    public void AddCardWithData(BaseCard card)
    {
        AddCardWithoutData(card);

        _size++;
        
        _deckData.AddACardToData(card.CardData);
    }

    public void DrawACard()
    {
        if (_deck.Count >= 1 && 
            _unitsManager.HeroPlayer.NbrOfCardDrawnThisTurn < _maxNbrOfCardToDrawnPerTurn.Value)
        {
            BaseCard rndCard = _deck[Random.Range(0, _deck.Count)];

            for (int i = 0; i < CardPlayedManager.Instance.AvailableCardSlots.Length; i++)
            {
                if (CardPlayedManager.Instance.AvailableCardSlots[i] == true)
                {
                    rndCard.gameObject.SetActive(true);
                    rndCard.HandIndex = i;
                    rndCard.transform.position = CardPlayedManager.Instance.CardSlots[i].position;
                    CardPlayedManager.Instance.AvailableCardSlots[i] = false;
                    _deck.Remove(rndCard);

                    if (_deck.Count == 0)
                    {
                        OnDeckEmpty?.Invoke(this);
                    }
                    
                    UpdateCardTxtNbr();
                    _unitsManager.HeroPlayer.NbrOfCardDrawnThisTurn++;
                    UpdateNbrCardDrawnTxt();
                    return;
                }
            }
        }
        else
        {
            StopCoroutine(nameof(TxtCo));
            StartCoroutine(nameof(TxtCo));
        }
    }

    public void UpdateCardTxtNbr()
    {
        _carNbrTxt.text = _deck.Count.ToString();
    }

    private void UpdateNbrCardDrawnTxt()
    {
        _cardNbrDrawnTxt.text = (_maxNbrOfCardToDrawnPerTurn.Value -
                                 _unitsManager.HeroPlayer.NbrOfCardDrawnThisTurn).ToString();

        _cardDrawnImage.fillAmount = (_maxNbrOfCardToDrawnPerTurn.Value -
            _unitsManager.HeroPlayer.NbrOfCardDrawnThisTurn) / (float)_maxNbrOfCardToDrawnPerTurn.Value;
    }
    
    private IEnumerator TxtCo()
    {
        _tooMuchCardDrawnTxt.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        _tooMuchCardDrawnTxt.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        BattleManager.OnBattleStart -= SetDeck;
        BattleManager.OnPlayerTurnStart -= ResetValues;
        BaseMoveCard.OnPathStarted -= DesactivateDeckButton;
        BaseUnit.OnPathEnded -= ActivateDeckButton;
    }

    private void ActivateDeckButton(BaseUnit unit)
    {
        if (unit.Faction == Faction.Hero)
        {
            _button.interactable = true;
        }
    }

    private void DesactivateDeckButton(BaseMoveCard obj)
    {
        _button.interactable = false;
    }
}
