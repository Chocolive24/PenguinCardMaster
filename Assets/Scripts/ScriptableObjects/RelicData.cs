using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New relic data", menuName = "Relic data")]
public class RelicData : ScriptableObject
{
    public enum RelicType
    {
        PERMANENT,
        ACTIVABLE,
        NULL,
    }

    public RelicType Type;
    public Relic RelicPrefab;
}
