using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy data", menuName = "Enemy data")]
public class EnemyData : UnitData
{
    public EnemyType Type;
    public int Weight;
    public int AttackRange;
    public int MaxNbrOfAttackPerTurn = 1;
    public int MaxNbrOfMovementPerTurn = 1;
    public int MinimumPathCount = 1;
}
