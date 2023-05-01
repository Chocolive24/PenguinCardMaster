using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

[CreateAssetMenu(fileName = "New deck data", menuName = "Deck data")]
public class DeckData : ScriptableObject, ISerializationCallbackReceiver
{
    public List<ScriptableCard> CardDeckData = new List<ScriptableCard>();

    public void AddACardToData(ScriptableCard scrCard)
    {
        CardDeckData.Add(scrCard);
    }

    public void OnBeforeSerialize()
    {
        
    }

    public void OnAfterDeserialize()
    {
        CardDeckData.Clear();
    }
}
