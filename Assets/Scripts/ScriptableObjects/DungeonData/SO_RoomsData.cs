using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Rooms data", fileName = "rooms data")]
public class SO_RoomsData : ScriptableObject
{
    public  Dictionary<Vector3Int, RoomData> RoomDatas;

    public List<RoomData> Rooms;
}
