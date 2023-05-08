using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO Storage/List", fileName = "List")]
public class SO_List : ScriptableObject
{
    [SerializeField] private List<SO_Storage> _storages;

    public int HowMany
    {
        get
        {
            // --------------------------------------------------
            // Fill the list
            // ---------------------------------------------------
            
            return _storages.Count;
            
        }
    }
    
}
