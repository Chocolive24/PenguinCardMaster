using System;
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
    
    public enum NeighboursType
    {
        CARDINAL,
        DIAGONAL,
        ALL_DIRECTION,
        UP_AND_DOWN,
        RIGHT_AND_LEFT,
        UP,
        RIGHT,
        LEFT,
        DOWN,
        NULL
    }

    public static Dictionary<Direction, Vector2> NeighboursTypeToDico(NeighboursType type)
    {
        switch (type)
        {
            case NeighboursType.CARDINAL:
                return CardinalNeighbours;
            case NeighboursType.DIAGONAL:
                return DiagonalNeighbours;
            case NeighboursType.ALL_DIRECTION:
                return AllDirectionsNeighbours;
            case NeighboursType.UP_AND_DOWN:
                return UpAndDownNeighbours;
            case NeighboursType.RIGHT_AND_LEFT:
                return RightAndLeftNeighbours;
            case NeighboursType.UP:
                return UpNeighbours;
            case NeighboursType.RIGHT:
                return RightNeighbours;
            case NeighboursType.LEFT:
                return LeftNeighbours;
            case NeighboursType.DOWN:
                return DownNeighbours;
            default:
                return new Dictionary<Direction, Vector2>();
        }
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
    
    public static Dictionary<Direction, Vector2> AllDirectionsNeighbours => new Dictionary<Direction, Vector2>()
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

    public static Dictionary<Direction, Vector2> UpAndDownNeighbours => new Dictionary<Direction, Vector2>()
    {
        { Direction.UP, Vector2.up },
        { Direction.DOWN, Vector2.down }
    };
    
    public static Dictionary<Direction, Vector2> RightAndLeftNeighbours => new Dictionary<Direction, Vector2>()
    {
        { Direction.RIGHT, Vector2.right },
        { Direction.LEFT, Vector2.left }
    };

    public static Dictionary<Direction, Vector2> UpNeighbours => new Dictionary<Direction, Vector2>()
    {
        { Direction.UP, Vector2.up }
    };
    
    public static Dictionary<Direction, Vector2> RightNeighbours => new Dictionary<Direction, Vector2>()
    {
        { Direction.RIGHT, Vector2.right }
    };
    
    public static Dictionary<Direction, Vector2> LeftNeighbours => new Dictionary<Direction, Vector2>()
    {
        { Direction.LEFT, Vector2.left }
    };
    
    public static Dictionary<Direction, Vector2> DownNeighbours => new Dictionary<Direction, Vector2>()
    {
        { Direction.DOWN, Vector2.down }
    };
}
