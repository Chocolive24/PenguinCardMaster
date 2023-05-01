using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowTranslator 
{
    /// <summary>
    /// Draw the right arrow part in function to the current path tile. 
    /// </summary>
    /// <param name="previousTile"></param>
    /// <param name="currentTile"></param>
    /// <param name="futureTile"></param>
    /// <param name="arrowSpriteRender"></param>
    /// <returns></returns>
    public void DrawArrowPath(TileCell previousTile, TileCell currentTile, TileCell futureTile)
    {
        bool isFinal = futureTile == null;

        Vector3 pastDirection = previousTile != null ? 
            currentTile.Position - previousTile.Position : new Vector3(0, 0, 0);
        Vector3 futureDirection = futureTile != null ? 
            futureTile.Position - currentTile.Position : new Vector3(0, 0, 0);

        Vector3 direction = pastDirection != futureDirection ? pastDirection + futureDirection : futureDirection;

        var arrow = currentTile.Arrow;
        var arrowSprite = arrow.GetComponent<SpriteRenderer>();

        // Straight Lines ----------------------------------------------------------------------------------------------
        if (direction == new Vector3(0, 1) && !isFinal)
        {
            arrowSprite.sprite = currentTile.Arrows[1];
        }
        if (direction == new Vector3(0, -1) && !isFinal)
        {
            arrowSprite.sprite = currentTile.Arrows[1];
            arrow.transform.Rotate(0, 0, 180);
        }
        if (direction == new Vector3(1, 0) && !isFinal)
        {
            arrowSprite.sprite = currentTile.Arrows[1];
            arrow.transform.Rotate(0, 0, 270);
        }
        if (direction == new Vector3(-1, 0) && !isFinal)
        {
            arrowSprite.sprite = currentTile.Arrows[1];
            arrow.transform.Rotate(0, 0, 90);
        }
        
        // Corners -----------------------------------------------------------------------------------------------------
        if (direction == new Vector3(1, 1))
        {
            if (pastDirection.y < futureDirection.y)
            {
                arrowSprite.sprite = currentTile.Arrows[2];
            }
            else
            {
                arrowSprite.sprite = currentTile.Arrows[2];
                arrow.transform.Rotate(0, 0, 180);
            }
        }
        if (direction == new Vector3(-1, 1))
        {
            if (pastDirection.y < futureDirection.y)
            {
                arrowSprite.sprite = currentTile.Arrows[2];
                arrow.transform.Rotate(0, 0, 270);
            }
            else
            {
                arrowSprite.sprite = currentTile.Arrows[2];
                arrow.transform.Rotate(0, 0, 90);
            }
        }
        if (direction == new Vector3(1, -1))
        {
            if (pastDirection.y > futureDirection.y)
            {
                arrowSprite.sprite = currentTile.Arrows[2];
                arrow.transform.Rotate(0, 0, 90);
            }
            else
            {
                arrowSprite.sprite = currentTile.Arrows[2];
                arrow.transform.Rotate(0, 0, 270);
            }
        }
        if (direction == new Vector3(-1, -1))
        {
            if (pastDirection.y > futureDirection.y)
            {
                arrowSprite.sprite = currentTile.Arrows[2];
                arrow.transform.Rotate(0, 0, 180);
            }
            else
            {
                arrowSprite.sprite = currentTile.Arrows[2];
            }
        }
        
        // Final Arrows ------------------------------------------------------------------------------------------------
        if (direction == new Vector3(0, 1) && isFinal)
        {
            arrowSprite.sprite = currentTile.Arrows[0];
        }
        if (direction == new Vector3(0, -1) && isFinal)
        {
            arrowSprite.sprite = currentTile.Arrows[0];
            arrow.transform.Rotate(0, 0, 180);
        }
        if (direction == new Vector3(1, 0) && isFinal)
        {
            arrowSprite.sprite = currentTile.Arrows[0];
            arrow.transform.Rotate(0, 0, 270);
        }
        if (direction == new Vector3(-1, 0) && isFinal)
        {
            arrowSprite.sprite = currentTile.Arrows[0];
            arrow.transform.Rotate(0, 0, 90);
        }
    }
}
