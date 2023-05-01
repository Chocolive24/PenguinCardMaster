using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BT_Selector : BT_Node
{
    // Constructor -----------------------------------------------------------------------------------------------------
    public BT_Selector(string name) : base(name)
    {
        
    }

    // Attributes ------------------------------------------------------------------------------------------------------
    private int _idxSelectedChild = 0;
    
    // Methods ---------------------------------------------------------------------------------------------------------
    public override BT_Status Process()
    {
        BT_Status status = _children[_idxSelectedChild].Process();
        
        Debug.Log("I'm the node : " + _children[_idxSelectedChild].Name + 
                  " / Depth : " + _children[_idxSelectedChild].Depth + " / Status : " + status);

        switch (status)
        {
            case BT_Status.RUNNING:
            case BT_Status.SUCCESS:
                return status;
            
            case BT_Status.FAILURE:
                _idxSelectedChild++;
                if (_idxSelectedChild >= _children.Count)
                {
                    _idxSelectedChild = 0;
                    return BT_Status.FAILURE;
                }
                else
                {
                    return BT_Status.RUNNING;
                }
            
            default:
                return BT_Status.FAILURE;
        }
    }
}
