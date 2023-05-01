using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTileCell : TileCell
{
    [SerializeField] private Color _baseColor, _offsetColor;
    public bool _isOffset = false;
    
    public override void Init(int x, int y)
    {
        base.Init(x, y);
        
        var isOffset = (x + y) % 2 == 1;

        _isOffset = isOffset;

        _spriteRender.color = isOffset ? _offsetColor : _baseColor;
    }
}
