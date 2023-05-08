using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New data", menuName = "Dungeon data")]
public class DungeonData : ScriptableObject
{
    public IntReference MinNbrOfRooms;
    public IntReference MaxNbrOfRooms;
    
    public IntReference MinEnemyWeight;
    public IntReference MaxEnemyWeight;
    
    public IntReference CurrentFloor;
    public IntReference MaxNbrOfFloors;
    
    public IntReference EpicCardPercent;
    public IntReference LegCardPercent;
    public IntReference RelicPercent;

    public List<IntReference> IntReferences;

    private void OnEnable()
    {
        if (IntReferences.Count == 0)
        {
            IntReferences.Add(MinNbrOfRooms);
            IntReferences.Add(MaxNbrOfRooms);
        
            IntReferences.Add(MinEnemyWeight);
            IntReferences.Add(MaxEnemyWeight);
        
            IntReferences.Add(CurrentFloor);
            IntReferences.Add(MaxNbrOfFloors);
        
            IntReferences.Add(EpicCardPercent);
            IntReferences.Add(LegCardPercent);
            IntReferences.Add(RelicPercent);
        }
    }
}
