using System;using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum Faction
{
    Hero = 0,
    Enemy,
    NONE
}

[CreateAssetMenu(fileName = "New data", menuName = "Unit data")]
public class UnitData : ScriptableObject
{
    public string UnitName;
    public Faction Faction;
    public IntReference MaxHP;
    public IntReference Attack;
    public IntReference Movement;
    public IntReference Speed;
    
    public BaseUnit BaseUnitPrefab;

    public List<IntReference> IntReferences;

    protected virtual void OnEnable()
    {
        IntReferences = new List<IntReference>();
        
        IntReferences.Add(MaxHP);
        IntReferences.Add(Attack);
        IntReferences.Add(Movement);
        IntReferences.Add(Speed);
    }
}
