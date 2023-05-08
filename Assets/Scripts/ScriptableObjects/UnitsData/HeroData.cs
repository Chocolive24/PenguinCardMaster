using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Hero data", menuName = "Hero data")]
public class HeroData : UnitData
{
    public HeroClass HeroClass;
    public IntReference CurrentHp;
    public IntReference Golds;
    public IntReference MaxMana;
    public IntReference ExploreSpeed;
    public IntReference BattleSpeed;

    protected override void OnEnable()
    {
        if (IntReferences.Count == 0)
        {
            base.OnEnable();
        
        
            IntReferences.Add(CurrentHp);
            IntReferences.Add(Golds);
            IntReferences.Add(MaxMana);
            IntReferences.Add(ExploreSpeed);
            IntReferences.Add(BattleSpeed);
        }
    }
}
