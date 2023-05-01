using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New RelicInventory data", menuName = "RelicInventory data")]
public class RelicInventoryData : ScriptableObject, ISerializationCallbackReceiver
{
    public List<RelicData> RelicData = new List<RelicData>();

    public void AddRelicData(RelicData relicData)
    {
        RelicData.Add(relicData);
    }
    
    public void OnBeforeSerialize()
    {
        
    }

    public void OnAfterDeserialize()
    {
        RelicData.Clear();
    }
}
