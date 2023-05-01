using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neighbourhood
{
    public enum Direction
    {
        UP,
        RIGHT,
        LEFT,
        DOWN,
        UP_RIGHT,
        DOWN_RIGHT,
        DOWN_LEFT,
        UP_LEFT,
        NULL
    }
    
    public static Dictionary<Direction, Vector2> CardinalNeighbours => new Dictionary<Direction, Vector2>()
    { 
        {Direction.UP, Vector2.up},
        {Direction.RIGHT, Vector2.right},
        {Direction.DOWN, Vector2.down},
        {Direction.LEFT, Vector2.left},
    };
    
    public static Dictionary<Direction, Vector2> DiagonalNeighbours => new Dictionary<Direction, Vector2>()
    { 
        {Direction.UP_RIGHT, new Vector2(1, 1)},
        {Direction.DOWN_RIGHT, new Vector2(-1, 1)},
        {Direction.DOWN_LEFT, new Vector2(-1, -1)},
        {Direction.UP_LEFT, new Vector2(1, -1)}
    };
    
    public static Dictionary<Direction, Vector2> AllNeighbours => new Dictionary<Direction, Vector2>()
    { 
        {Direction.UP, Vector2.up},
        {Direction.UP_RIGHT, new Vector2(1, 1)},
        {Direction.RIGHT, Vector2.right},
        {Direction.DOWN_RIGHT, new Vector2(-1, 1)},
        {Direction.DOWN, Vector2.down},
        {Direction.DOWN_LEFT, new Vector2(-1, -1)},
        {Direction.LEFT, Vector2.left},
        {Direction.UP_LEFT, new Vector2(1, -1)}
    };
}
