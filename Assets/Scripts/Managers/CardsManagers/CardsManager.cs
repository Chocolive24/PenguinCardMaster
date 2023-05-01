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

    private List<ScriptableCard> _scrBasicMoveCards;
    private List<ScriptableCard> _scrBasicAttackCards;
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
    public List<ScriptableCard> ScrBasicMoveCards => _scrBasicMoveCards;
    public List<ScriptableCard> ScrBasicAttackCards => _scrBasicAttackCards;
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
        
        _scrBasicMoveCards = Resources.LoadAll<ScriptableCard>("Cards/MoveCards/BasicMoveCards").ToList();
        _scrBasicAttackCards = Resources.LoadAll<ScriptableCard>(
            "Cards/AttackCards/PaladinCards/BasicAttackCards").ToList();
        
        BaseCard.OnCollected += AddCollectedCardToDeck;
    }

    private void OnDestroy()
    {
        BaseCard.OnCollected -= AddCollectedCardToDeck;
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
        
        for (int i = 0; i < rewardNbr; i++)
        {
            int rndTypeNbr = Random.Range(1, 3);

            List<ScriptableCard> rndList = new List<ScriptableCard>();

            switch (rndTypeNbr)
            {
                case 1:
                {
                    rndList = ScrBasicAttackCards;
                    break;
                }
                case 2:
                    rndList = ScrBasicMoveCards;
                    break;
            }

            // int rndRaretyNbr = Random.Range(1, 101);
            //
            // switch (rndRaretyNbr)
            // {
            //     case <= 50:
            //         InstantiateCard(rndList, Rarety.Basic);
            //         break;
            //     case <= 80:
            //         InstantiateCard(rndList, Rarety.Rare);
            //         break;
            //     case <= 95:
            //         InstantiateCard(rndList, Rarety.Epic);
            //         break;
            //     case <= 100:
            //         InstantiateCard(rndList, Rarety.Legendary);
            //         break;
            // }
            
            var card = GetRandomCard<BaseCard>(rndList, Rarety.Basic);
            BaseCard spawnedCard = null;

            switch (i)
            {
                case 0:
                    spawnedCard = Instantiate(card, _reward1Trans.position, Quaternion.identity);
                    spawnedCard.transform.parent = _reward1Trans.transform;
                    spawnedCards.Add(spawnedCard);
                    break;
                case 1:
                    spawnedCard = Instantiate(card, _reward2Trans.position, Quaternion.identity);
                    spawnedCard.transform.parent = _reward2Trans.transform;
                    spawnedCards.Add(spawnedCard);
                    break;
                case 2:
                    spawnedCard = Instantiate(card, _reward3Trans.position, Quaternion.identity);
                    spawnedCard.transform.parent = _reward3Trans.transform;
                    spawnedCards.Add(spawnedCard);
                    break;
            }

            if (spawnedCard)
            {
                //spawnedCard.OnCollected += AddCollectedCardToDeck;
            }
        }
        
        return spawnedCards;
    }
    
    public void AddCollectedCardToDeck(BaseCard card)
    {
        switch (card.CardType)
        {
            case CardType.Attackcard:
                _mainDeckContoller.AddCardWithData(card);
                break;
            case CardType.MoveCard:
                _movementDeckController.AddCardWithData(card);
                break;
        }
    }
    
    private T GetRandomCard<T>(List<ScriptableCard> scriptableCards, Rarety rarety) where T : BaseCard
    {
        return (T)scriptableCards.Where(u => u.Rarety == rarety).OrderBy
            (o => Random.value).First().BaseCardPrefab;
    }
}
