using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementDeckController : DeckController
{
    [Header("Number of First Cards in the Deck")]
    [SerializeField] private int _basicMoveCardNbr = 5;
    
    // Start is called before the first frame update
    
    protected override void SetDeck(BattleManager battleManager, RoomData room)
    {
        base.SetDeck(battleManager, room);
        if (!_unitsManager.HeroPlayer.MovementDeck)
        {
            _unitsManager.HeroPlayer.MovementDeck = this;
            InstantiateBasicCard(CardsManager.Instance.ScrBasicMoveCards, _basicMoveCardNbr);
        }
    }
}
