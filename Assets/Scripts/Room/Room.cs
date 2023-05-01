using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType
{
    BASIC,
    START,
    END,
    FREE_FIGHT,
    OBLIGATORY_FIGHT,
    SHOP,
}

public class Room : MonoBehaviour
{
    // Attributes ------------------------------------------------------------------------------------------------------
    [SerializeField] private RoomType _type;
    
    private List<BaseEnemy> _enemies;

    private RoomData _roomData;
    
    // Events ----------------------------------------------------------------------------------------------------------
    public static event Action<Room> OnRoomEnter;
    
    // Getters and Setters ---------------------------------------------------------------------------------------------
    public RoomType Type => _type;
    
    // Methods ---------------------------------------------------------------------------------------------------------

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.GetComponent<BaseHero>())
        {
            OnRoomEnter?.Invoke(this);
        }
    }
}
