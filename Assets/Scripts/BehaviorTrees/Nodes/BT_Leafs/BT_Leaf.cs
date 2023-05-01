using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BT_Leaf : BT_Node
{
    // Constructor -----------------------------------------------------------------------------------------------------
    public BT_Leaf(string name, Func<BT_Status> performAction) : base(name)
    {
        _performAction = performAction;
    }
    
    // Attributes ------------------------------------------------------------------------------------------------------
    private Func<BT_Status> _performAction;
    
    // Methods ---------------------------------------------------------------------------------------------------------
    public override BT_Status Process()
    {
        return _performAction();
    }
}
