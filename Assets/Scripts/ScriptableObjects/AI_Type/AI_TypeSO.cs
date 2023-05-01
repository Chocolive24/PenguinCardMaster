using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "New unit", menuName = "AI_TypeSO")]
public class AI_TypeSO : ScriptableObject
{
    public EnemyType Type;
    
    public void SetType(string typeName)
    {
        switch (typeName)
        {
            case "Goblin":
                Type = EnemyType.GOBLIN;
                break;
            case "Archer":
                Type = EnemyType.ARCHER;
                break;
            case "Tank":
                Type = EnemyType.TANK;
                break;
            case "Spawner":
                Type = EnemyType.SPAWNER;
                break;
            case "Mix":
                Type = EnemyType.MIX;
                break;
        }
    }
}
