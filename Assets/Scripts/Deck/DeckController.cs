using System;
using System.Collections;
using System.Collections.Generic;
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
        
    [SerializeField] protected int _size;

    [SerializeField] protected Button _button;

    // References ------------------------------------------------------------------------------------------------------
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
        for (int i = 0; i < cardNbr; i++)
        {
            var card = CardsManager.Instance.InstantiateCard(scriptableCards, Rarety.Basic);
            
            AddCard(card);
        }
    }
    
    public void AddCard(BaseCard card)
    {
        card.transform.parent = gameObject.transform;

        card.gameObject.SetActive(false);

        card.IsCollected = true;

        _deck.Add(card);

        _size++;
        
        UpdateCardTxtNbr();

        // if (!card.IsHeroSubscibed)
        // {
        //     card.OnDrawn += UnitsManager.Instance.HeroPlayer.AddCardToHand;
        //     card.OnPerformed += UnitsManager.Instance.HeroPlayer.RemoveCardFromHand;
        //
        //     card.IsHeroSubscibed = true;
        // }
        
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
        _cardNbrDrawnTxt.text = _unitsManager.HeroPlayer.NbrOfCardDrawnThisTurn + " / " + 
                                _maxNbrOfCardToDrawnPerTurn.Value;
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
    }
}
