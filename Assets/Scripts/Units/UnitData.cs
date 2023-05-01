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
}
