using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

[CreateAssetMenu(fileName = "New deck data", menuName = "Deck data")]
public class DeckData : ScriptableObject
{
    public List<ScriptableCard> CardDeckData = new List<ScriptableCard>();

    public void AddACardToData(ScriptableCard scrCard)
    {
        CardDeckData.Add(scrCard);
    }
}
