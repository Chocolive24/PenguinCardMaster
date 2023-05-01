using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Tank : BaseEnemy
{
    private int _nbrOfMovement = 0;
    
    public override bool IsPositionAvailable(Vector3 position, bool countHeroes, bool countEnemies, bool countWalls)
    {
        bool isPosAvalaible = false;

        if (position == _unitsManager.HeroPlayer.transform.position)
        {
            return true;
        }
        
        var possibleAvalaibleTiles = GetTilesInOccupiedRange(position);
    
        if (possibleAvalaibleTiles != null)
        {
            foreach (var tile in possibleAvalaibleTiles)
            {
                if (base.IsPositionAvailable(tile.transform.position, true, false, 
                        false) || GetOccupiedTiles().Contains(tile))
                {
                    isPosAvalaible = true;
                }
                else
                {
                    isPosAvalaible = false;
                    break;
                }
            }
        }
        
        return isPosAvalaible;
    }

    public override void FindAvailablePathToTarget(Vector3 targetPos, int minimumPathCount, 
        bool countHeroes, bool countEnemies, bool countWalls)
    {
        if (GetTilesInAttackRange(transform.position, _attackRange).ContainsKey(
                _unitsManager.HeroPlayer.transform.position))
        {
            _path = new List<Vector3>();
        }
        else
        {
            base.FindAvailablePathToTarget(targetPos, minimumPathCount, countHeroes, countEnemies, countWalls);
        }
        
    }

    protected override void FollowThePath(List<Vector3> pathToFollow)
    {
        if (_currentTargetIndex < pathToFollow.Count - 1 && _nbrOfMovement < _baseMovement.Value - 1) 
        {
            _currentTargetIndex++;

            if (GetTilesInAttackRange(transform.position, _attackRange).ContainsKey(
                    _unitsManager.HeroPlayer.transform.position))
            {
                StopThePath();
                _nbrOfMovement = 0;
            }
            else
            {
                _targetPos = _gridManager.WorldToCellCenter(pathToFollow[_currentTargetIndex]);
                _nbrOfMovement++;
            }
        }
        
        else
        {
            StopThePath();
            _nbrOfMovement = 0;
        }
    }
    
    public override Dictionary<Vector3, int> GetAvailableTilesInRange(Vector3 startPos, int range, 
        bool countHeroes, bool countEnemies)
    {
        Dictionary<Vector3, int> tmpDico = new Dictionary<Vector3, int>();
        Dictionary<Vector3, int> finalDico = new Dictionary<Vector3, int>();
    
        foreach (var pos in new Vector3[] { startPos, startPos + Vector3.right, startPos + Vector3.up, 
                     new Vector3(startPos.x + 1, startPos.y + 1, 0) })
        {
            tmpDico = base.GetAvailableTilesInRange(pos, range, countHeroes, countEnemies);
    
            foreach (var item in tmpDico)
            {
                if (!finalDico.ContainsKey(item.Key))
                {
                    finalDico.Add(item.Key, item.Value);
                }
            }
        }
    
        foreach (var tile in GetOccupiedTiles())
        {
            finalDico[tile.transform.position] = 0;
        }
        
        return finalDico;
    }
    
    public override Dictionary<Vector3, int> GetTilesInAttackRange(Vector3 startPos, int range)
    {
        Dictionary<Vector3, int> dico = GetAvailableTilesInRange(startPos, 1, true, false);
    
        foreach (var tile in GetOccupiedTiles())
        {
            dico.Remove(tile.transform.position);
        }
    
        return dico;
    }
    
    public override List<TileCell> GetOccupiedTiles()
    {
        return GetTilesInOccupiedRange(transform.position);
    }
    
    private List<TileCell> GetTilesInOccupiedRange(Vector3 startPos)
    {
        TileCell downLeft = GridManager.Instance.GetTileAtPosition(startPos);
        TileCell downRight = GridManager.Instance.GetTileAtPosition(new Vector3(startPos.x + 1, startPos.y, 0));
        TileCell upLeft = GridManager.Instance.GetTileAtPosition(new Vector3(startPos.x, startPos.y + 1, 0));
        TileCell upRight = GridManager.Instance.GetTileAtPosition(new Vector3(startPos.x + 1, startPos.y + 1, 0));
    
        if (downLeft && downRight && upLeft && upRight)
        {
            return new List<TileCell>
            {
                downLeft, downRight, upLeft, upRight
            };
        }
    
        return null;
    }
}
