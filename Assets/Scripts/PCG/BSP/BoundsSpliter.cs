using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SplitDirection
{
    HORIZONTAL,
    VERTICAL
}
public class BoundsSpliter
{
    public static void SplitBounds(BoundsInt mapToProcess, int cutRatio, SplitDirection direction, 
                                    out BoundsInt mapLeft, out BoundsInt mapRight)
    {
        mapLeft = new BoundsInt();
        mapRight = new BoundsInt();
            
        if (direction == SplitDirection.HORIZONTAL)
        {
            HorizonSplitBounds(mapToProcess, cutRatio, out mapLeft, out mapRight);
        }
        else if (direction == SplitDirection.VERTICAL)
        {
            VerticalSplitBounds(mapToProcess, cutRatio, out mapLeft, out mapRight);
        }
    }

    private static void HorizonSplitBounds(BoundsInt mapToProcess, int cutRatio, 
                                            out BoundsInt mapLeft, out BoundsInt mapRight)
    {
        mapLeft = new BoundsInt();
        mapLeft.xMin = mapToProcess.xMin;
        mapLeft.xMax = mapToProcess.xMax;
        mapLeft.yMin = mapToProcess.yMin;
        mapLeft.yMax = mapToProcess.yMin + cutRatio;
            
        mapRight = new BoundsInt();
        mapRight.xMin = mapToProcess.xMin;
        mapRight.xMax = mapToProcess.xMax;
        mapRight.yMin = mapLeft.yMax;
        mapRight.yMax = mapToProcess.yMax;
            
    }

    private static void VerticalSplitBounds(BoundsInt mapToProcess, int cutRatio, out BoundsInt mapLeft, out BoundsInt mapRight)
    {
        mapLeft = new BoundsInt();
        mapLeft.xMin = mapToProcess.xMin;
        mapLeft.xMax = mapToProcess.xMin + cutRatio;
        mapLeft.yMin = mapToProcess.yMin;
        mapLeft.yMax = mapToProcess.yMax;

        mapRight = new BoundsInt();
        mapRight.xMin = mapLeft.xMax;
        mapRight.xMax = mapToProcess.xMax;
        mapRight.yMin = mapToProcess.yMin;
        mapRight.yMax = mapToProcess.yMax;
    }
}
