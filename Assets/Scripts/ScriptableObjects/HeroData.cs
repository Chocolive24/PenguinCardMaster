using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Hero data", menuName = "Hero data")]
public class HeroData : UnitData
{
    public HeroClass HeroClass;
    public IntReference MaxMana;
    public IntReference ExploreSpeed;
    public IntReference BattleSpeed;
}
