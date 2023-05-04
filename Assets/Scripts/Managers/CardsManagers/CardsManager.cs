using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class CardsManager : MonoBehaviour
{
    private static CardsManager _instance;
    public static CardsManager Instance => _instance;

    private List<ScriptableCard> _scrMoveCards;
    private List<ScriptableCard> _scrAttackCards;
    private List<ScriptableCard> _scrBasicDefenseCards;

    private int _totalRaretyWeight = (int)Rarety.Basic + (int)Rarety.Rare + (int)Rarety.Epic + (int)Rarety.Legendary;
    
    // References ------------------------------------------------------------------------------------------------------

    [SerializeField] private MainDeckContoller _mainDeckContoller;
    [SerializeField] private MovementDeckController _movementDeckController;
    
    [SerializeField] private RectTransform _reward1Trans;
    [SerializeField] private RectTransform _reward2Trans;
    [SerializeField] private RectTransform _reward3Trans;

    [SerializeField] private GameObject _victoryPanel;

    // Getters and Setters ---------------------------------------------------------------------------------------------
    public List<ScriptableCard> ScrMoveCards => _scrMoveCards;
    public List<ScriptableCard> ScrAttackCards => _scrAttackCards;
    public List<ScriptableCard> ScrBasicDefenseCards => _scrBasicDefenseCards;

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
        
        _scrMoveCards = Resources.LoadAll<ScriptableCard>("Cards/MoveCards").ToList();
        _scrAttackCards = Resources.LoadAll<ScriptableCard>("Cards/AttackCards").ToList();
        
        Collectible.OnCollected += AddCollectedCardToDeck;
    }

    private void OnDestroy()
    {
        Collectible.OnCollected -= AddCollectedCardToDeck;
    }

    public BaseCard InstantiateACardFromData(ScriptableCard cardData)
    {
        return Instantiate(cardData.BaseCardPrefab);
    }
    
    public BaseCard InstantiateARandomCard(List<ScriptableCard> scriptableCards, Rarety rarety)
    {
        var randomPrefab = GetRandomCard<BaseCard>(scriptableCards, rarety);
        var spawnedCard = Instantiate(randomPrefab);

        return spawnedCard;
    }

    public List<BaseCard> CreateCardRewards(int rewardNbr)
    {
        List<BaseCard> spawnedCards = new List<BaseCard>();

        List<ScriptableCard> moveCardsNotSpawned = Resources.LoadAll<ScriptableCard>("Cards/MoveCards").ToList();;
        List<ScriptableCard> attackCardsNotSpawned = Resources.LoadAll<ScriptableCard>("Cards/AttackCards").ToList();

        List<ScriptableCard> scriptableCards = new List<ScriptableCard>();

        RectTransform rect = new RectTransform();
        Rarety rarety = Rarety.Rare;

        switch (Random.Range(1, 101))
        {
            case <= 10:
                rarety = Rarety.Legendary;
                break;
            case <= 20:
                rarety = Rarety.Epic;
                break;
            case <= 100:
                rarety = Rarety.Rare;
                break;
        }
        
        for (int i = 0; i < rewardNbr; i++)
        {
            switch (i)
            {
                case 0:
                    scriptableCards = moveCardsNotSpawned;
                    rect = _reward1Trans;
                    break;
                case 1:
                    scriptableCards = attackCardsNotSpawned;
                    rect = _reward2Trans;
                    break;
                case 2:
                    switch (Random.Range(1, 3))
                    {
                        case 1:
                            scriptableCards = moveCardsNotSpawned;
                            break;
                        case 2:
                            scriptableCards = attackCardsNotSpawned;
                            break;
                    }
                    rect = _reward3Trans;
                    break;
            }

            if (scriptableCards.Count > 0)
            {
                BaseCard cardData = GetRandomCard<BaseCard>(scriptableCards, rarety);
                BaseCard spawnedCard = Instantiate(cardData, rect.position, Quaternion.identity);
                spawnedCard.transform.parent = rect.transform;
                spawnedCards.Add(spawnedCard);
                
                switch (spawnedCard.CardType)
                {
                    case CardType.MOVE_CARD:
                        moveCardsNotSpawned.Remove(spawnedCard.CardData);
                        break;
                    case CardType.BASE_ATTACK_CARD:
                    case CardType.AOE_ATTACK_CARD:
                        if (rarety != Rarety.Legendary)
                        {
                            attackCardsNotSpawned.Remove(spawnedCard.CardData);
                        }
                        break;
                }
            }
        }
        
        
        
        // for (int i = 0; i < rewardNbr; i++)
        // {
        //     BaseCard cardData = null;
        //     
        //     int rndRaretyNbr = Random.Range(1, 101);
        //     
        //     switch (rndRaretyNbr)
        //     {
        //         case <= 50:
        //            cardData = GetRandomCard<BaseCard>(scriptableCards, Rarety.Rare);
        //             break;
        //         case <= 80:
        //             cardData = GetRandomCard<BaseCard>(scriptableCards, Rarety.Epic);
        //             break;
        //         case <= 100:
        //             cardData = GetRandomCard<BaseCard>(scriptableCards, Rarety.Legendary);
        //             break;
        //     }
        //     
        //     BaseCard spawnedCard = null;

        //     switch (i)
        //     {
        //         case 0:
        //             spawnedCard = Instantiate(cardData, _reward1Trans.position, Quaternion.identity);
        //             spawnedCard.transform.parent = _reward1Trans.transform;
        //             spawnedCards.Add(spawnedCard);
        //             break;
        //         case 1:
        //             spawnedCard = Instantiate(cardData, _reward2Trans.position, Quaternion.identity);
        //             spawnedCard.transform.parent = _reward2Trans.transform;
        //             spawnedCards.Add(spawnedCard);
        //             break;
        //         case 2:
        //             spawnedCard = Instantiate(cardData, _reward3Trans.position, Quaternion.identity);
        //             spawnedCard.transform.parent = _reward3Trans.transform;
        //             spawnedCards.Add(spawnedCard);
        //             break;
        //     }
        // }
        
        return spawnedCards;
    }
    
    public void AddCollectedCardToDeck(Collectible obj)
    {
        BaseCard card = obj.GetComponent<BaseCard>();

        if (card)
        {
            switch (card.CardType)
            {
                case CardType.BASE_ATTACK_CARD:
                case CardType.AOE_ATTACK_CARD:
                    _mainDeckContoller.AddCardWithData(card);
                    break;
                case CardType.MOVE_CARD:
                    _movementDeckController.AddCardWithData(card);
                    break;
            } 
        }
    }
    
    private T GetRandomCard<T>(List<ScriptableCard> scriptableCards, Rarety rarety) where T : BaseCard
    {
        return (T)scriptableCards.Where(u => u.Rarety == rarety).OrderBy
            (o => Random.value).First().BaseCardPrefab;
    }
}
