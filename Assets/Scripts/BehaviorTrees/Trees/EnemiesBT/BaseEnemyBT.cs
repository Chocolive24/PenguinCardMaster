using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BaseEnemyBT : MonoBehaviour
{
    // Attributes ------------------------------------------------------------------------------------------------------
    protected BT_Node _root;
    
    protected BT_Selector _actionSelector = new BT_Selector("Action selec");

    protected BT_Sequence _moveToTargetSequence = new BT_Sequence("move to target sequence");
    
    // References ------------------------------------------------------------------------------------------------------
    protected BaseEnemy _enemyRef;

    protected UnitsManager _unitsManager;
    protected GridManager _gridManager;
    protected BaseHero _playerRef;

    // Methods ---------------------------------------------------------------------------------------------------------
    
    // Start is called before the first frame update
    protected virtual void Start()
    {
        _enemyRef = GetComponent<BaseEnemy>();
        _enemyRef.ResetTurnValues();
        _unitsManager = UnitsManager.Instance;
        _gridManager = GridManager.Instance;

        _playerRef = FindObjectOfType<BaseHero>();
        
        if (_enemyRef)
        {
            SetupTree();
        }
    }

    public virtual void SetupTree()
    {
        _root = new BT_Root("Root Node");
        
        _root.AddNode(_actionSelector);

        _actionSelector.AddNode(new BT_Leaf("Direct Attack leaf node", AttackTarget));
        _actionSelector.AddNode(_moveToTargetSequence);
        _actionSelector.AddNode(new BT_Leaf("End the turn node", EndTheTurn));

        _moveToTargetSequence.AddNode(new BT_Leaf("Find Path leaf", FindPath));
        _moveToTargetSequence.AddNode(new BT_Leaf("Move to Target leaf", MoveToTheTarget));
    }

    protected virtual BT_Status AttackTarget()
    {
        if (_enemyRef.FindATargetToAttack(_unitsManager.Heroes[0]))
        {
            if (_enemyRef.CanAttack)
            {
                _enemyRef.Attack(_unitsManager.Heroes[0]);
                Debug.Log("attack :" + _unitsManager.Heroes[0].CurrentHp.Value);
                return BT_Status.SUCCESS;
            }
        }
        
        return BT_Status.FAILURE;
    }

    protected virtual BT_Status FindPath()
    {
        if (_enemyRef)
        {
            if (_enemyRef.CanMove)
            {
                _enemyRef.FindAvailablePathToTarget(_unitsManager.Heroes[0].transform.position, 
                    _enemyRef.MinimumPathCount, true, false, false);

                if (_enemyRef.Path.Count > 0)
                {
                    return BT_Status.SUCCESS;
                }
            }
        }

        _enemyRef.NbrOfMovementPerformed = _enemyRef.MaxNbrOfMovementPerTurn;
        return BT_Status.FAILURE;
    }
    
    protected virtual BT_Status MoveToTheTarget()
    {
        if (_enemyRef.TargetPos.HasValue)
       {
           _enemyRef.MoveOnGrid();
           return BT_Status.RUNNING;
       }

       return BT_Status.SUCCESS;
    }

    protected virtual BT_Status EndTheTurn()
    {
        if (_enemyRef.CanFinishTheTurn && !_enemyRef.HasFinishedTheTurn)
        {
            _enemyRef.InvokeEndTurnEvent();
            return BT_Status.SUCCESS;
        }

        return BT_Status.FAILURE;
    }
    
    // Update is called once per frame
    protected virtual void Update()
    {
        if (_unitsManager.CurrentEnemyPlaying == _enemyRef && _unitsManager.HeroPlayer)
        {
            _root.Process();
        }
    }
}
