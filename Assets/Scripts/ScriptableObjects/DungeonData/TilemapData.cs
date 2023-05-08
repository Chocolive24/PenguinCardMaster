using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Tilemap data", menuName = "Tilemap data")]
public class TilemapData : ScriptableObject
{
    public Tilemap Tilemap;
}
