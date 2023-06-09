using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor.Rendering;
using UnityEngine;

public class Archer : BaseEnemy
{
    [SerializeField] private GameObject _projectilePrefab;
    
    public override void Attack(BaseHero heroTarget)
    {
        Projectile projectile = Instantiate(_projectilePrefab, transform.position, 
            Quaternion.identity).GetComponent<Projectile>();

        BaseHero player = _unitsManager.HeroPlayer;

        if (transform.position.x > player.transform.position.x)
        {
            projectile.transform.Rotate(0, 0, 180f);
        }
        else if (transform.position.y > player.transform.position.y)
        {
            projectile.transform.Rotate(0, 0, 270f);
        }
        if (transform.position.y < player.transform.position.y)
        {
            projectile.transform.Rotate(0, 0, 90f);
        }
        
        StartCoroutine(projectile.InterpolateMoveCo(transform.position, 
            player.transform.position));

        StartCoroutine(WaitForProjCo(heroTarget));
    }

    private IEnumerator WaitForProjCo(BaseHero heroTarget)
    {
        _spriteRenderer.flipX = heroTarget.transform.position.x < transform.position.x;
        _nbrOfAttackPerformed++;
        yield return new WaitForSeconds(0.5f);
        base.Attack(heroTarget);
    }

    public override Dictionary<Vector3, int> GetTilesInAttackRange(Vector3 startPos, int range)
    {
        var distances = new Dictionary<Vector3, int> { { startPos, 0 } };
        
        foreach (var direction in new Vector3[] { Vector3.up, Vector3.down, Vector3.left, Vector3.right })
        {
            Vector3 currentPos = startPos;
                
            for (int i = 1; i <= range; i++)
            {
                var neighbor = _gridManager.WorldToCellCenter(currentPos + direction) ;

                bool isPosValid = _tilemapsManager.IsPositionAvailable(neighbor, true, false) ||
                                  neighbor == transform.position;
                
                if ( isPosValid && !distances.ContainsKey(neighbor))
                {
                    distances.Add(neighbor, distances[currentPos] + 1);
                    currentPos = neighbor;
                }
                else
                {
                    break;
                }
            }
        }

        distances.Remove(distances.First().Key);
        
        return distances;
    }


    private bool CheckIfObstacleInTheLine(Vector3 targetPos, Vector3 direction, int range)
    {
        bool isObstacle = false;
        
        for (int i = 1; i <= range; i++)
        {
            var tile = _gridManager.GetTileAtPosition(targetPos + (direction * i));
            
            if (tile)
            {
                if (!tile.Walkable)
                {
                    if (i == 0 || tile.OccupiedUnit)
                    {
                        return true;
                    }
                    else
                    {
                        isObstacle = true;
                    }
                }
                
            }
            else
            {
                isObstacle = false;
            }
        }

        return isObstacle;
    }
    
    
    
    public override void FindAvailablePathToTarget(Vector3 targetPos, int minimumPathCount, 
                                                    bool countHeroes, bool countEnemies, bool countWalls)
    {
        int bestValidDistance = int.MaxValue;
        Vector3 bestPos = targetPos;
        
        int bestDistance = int.MaxValue;
        
        bool isPlayerInRange = false;
        bool isPositionValid = false;

        bool foundBestPos = false;

        Vector3 playerPos = _unitsManager.HeroPlayer.transform.position;
        
        for (int i = _attackRange; i >= 2; i--)
        {
            if (!foundBestPos)
            {
                foreach (var direction in new Vector3[]
                             { Vector3.up * i, Vector3.down * i, Vector3.left * i, Vector3.right * i })
                {
                    Vector3 neighbor = playerPos + direction;

                    var attackTiles = GetTilesInAttackRange(neighbor, _attackRange);

                    List<Vector3> attackPositions = new List<Vector3>();

                    foreach (var item in attackTiles)
                    {
                        if (item.Value > 1)
                        {
                            attackPositions.Add(item.Key);
                        }
                    }
                    
                    isPlayerInRange = attackPositions.Contains(playerPos);
                    
                    isPositionValid = IsPositionAvailable(neighbor, false, false, false);
                    
                    if (neighbor == transform.position && isPlayerInRange)
                    {
                        _path = new List<Vector3>();
                        return;
                    }

                    int neighborDistance = CalculDistanceFromSelf(neighbor, false, 
                        false, false);
                    
                    bool isNeighborTooFarAway = neighborDistance > 3f; 
                    
                    if (neighborDistance < bestValidDistance && isPositionValid &&
                        isPlayerInRange && !isNeighborTooFarAway)
                    {
                        bestValidDistance = neighborDistance;
                        bestPos = neighbor;
                        foundBestPos = true;
                    }
                }
            }
            
            if (foundBestPos)
            {
                break;
            }
        }

        if (!foundBestPos && CalculDistanceFromSelf(playerPos, true, false, false) <= 5)
        {
            TileCell fleeTile = FindFleeTile();

            if (fleeTile)
            {
                bestPos = fleeTile.transform.position;
            }
        }
        
        base.FindAvailablePathToTarget(bestPos, 0, countHeroes, countEnemies, countWalls);

        Debug.Log("IA == player pos :" + targetPos + " enemy pos : " + transform.position);
        Debug.Log("IA == Best pos : "+ bestPos + " isPlayerinRange "+ isPlayerInRange + " isPosValid" + isPositionValid);
    }

    private TileCell FindFleeTile()
    {
        int index = _baseMovement.Value;

        float bestDistance = float.MinValue;
        TileCell bestTile = null;

        var avalaileTiles = GetAvailableTilesInRange(
            transform.position, _baseMovement.Value, false, false);

        while (index > 0)
        {
            foreach (var item in avalaileTiles)
            {
                if (item.Value == index)
                {
                    TileCell tile = _gridManager.GetTileAtPosition(item.Key);

                    float distance = Vector3.Distance(tile.transform.position,
                        _unitsManager.HeroPlayer.transform.position);

                    if (tile.Walkable && distance > bestDistance)
                    {
                        bestDistance = distance;
                        bestTile = tile;
                    }
                }
            }

            if (bestTile)
            {
                return bestTile;
            }

            index--;
        }

        return null;
    }


    protected override void CorrectPathIfOnlyOneTile()
    {
        return;
    }

    protected override void CorrectTargetPos()
    {
        return;
    }
}
