using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BT_Status
{
    RUNNING,
    FAILURE,
    SUCCESS,
}

public class BT_Node 
{
    // Constructor ------------------------------------------------------------------------------------------------------
    public BT_Node(string name)
    {
        _name = name;
        _depth = 0;
    }
   
    // Attributes -------------------------------------------------------------------------------------------------------
    protected string _name;
    protected int _depth;

    protected List<BT_Node> _children = new List<BT_Node>();

    // Getters and Setters ----------------------------------------------------------------------------------------------

    #region Getters and Setters

    public string Name
    {
        get => _name;
        set => _name = value;
    }
    public int Depth
    {
        get => _depth;
        set => _depth = value;
    }

    #endregion
    
    // Methods ----------------------------------------------------------------------------------------------------------
    public virtual BT_Status Process()
    {
        foreach (var child in _children)
        {
            BT_Status status = child.Process();
            Debug.Log("I'm the node : " + _name + " / Depth : " + _depth + " / Status : " + status);
        }

        return BT_Status.SUCCESS;
    }

    public void AddNode(BT_Node newNode)
    {
        newNode.Depth = this.Depth + 1;
        _children.Add(newNode);
    }
}
