using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BT_Sequence : BT_Node
{
    public BT_Sequence(string name) : base("SEQUENCE" + name)
    {
        
    }

    // Attributes ------------------------------------------------------------------------------------------------------
    protected int _idxCurrentChild = 0;
    
    // Methods ---------------------------------------------------------------------------------------------------------
    public override BT_Status Process()
    {
        if (_idxCurrentChild < _children.Count)
        {
            BT_Status status = _children[_idxCurrentChild].Process();

            Debug.Log("I'm the node : " + _children[_idxCurrentChild].Name + 
                      " / Depth : " + _children[_idxCurrentChild].Depth + " / Status : " + status);
            
            if (status == BT_Status.FAILURE)
            {
                _idxCurrentChild = 0;
                return BT_Status.FAILURE;
            }
            else if (status == BT_Status.RUNNING)
            {
                return BT_Status.RUNNING;
            }
            else
            {
                _idxCurrentChild++;
                return BT_Status.RUNNING;
            }
        }
        else
        {
            _idxCurrentChild = 0;
            return BT_Status.SUCCESS;
        }
    }
}
