using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerBT : BaseEnemyBT
{
    public override void SetupTree()
    {
        _actionSelector.AddNode(new BT_Leaf("Spawn a Minion", SpawnAMinion));
        base.SetupTree();
    }

    private BT_Status SpawnAMinion()
    {
        Spawner spawnerRef = (Spawner)_enemyRef;
        
        if (spawnerRef.CanAttack && spawnerRef.CurrentNbrOfMinion < spawnerRef.MaxNbrOfMinion)
        {
            float bestDistance = float.MaxValue;
            TileCell bestTile = null;
        
            var attackTiles = _enemyRef.GetTilesInAttackRange(
                _enemyRef.transform.position, _enemyRef.AttackRange);

            foreach (var item in attackTiles)
            {
                TileCell attackTile = _gridManager.GetTileAtPosition(item.Key);

                float distance = Vector3.Distance(attackTile.transform.position, _playerRef.transform.position);
                
                if (attackTile.Walkable && distance < bestDistance)
                {
                    bestDistance = distance;
                    bestTile = attackTile;
                }
            }

            if (bestTile)
            {
                spawnerRef.SpawnAMinion(bestTile.transform.position);
                return BT_Status.SUCCESS;
            }
        }
        
        return BT_Status.FAILURE;
    }

    protected override BT_Status FindPath()
    {
        if (_enemyRef)
        {
            if (_enemyRef.CanMove && _enemyRef.CalculDistanceFromSelf(
                    _unitsManager.HeroPlayer.transform.position, true, false, false) <= 10f)
            {
                int index = _enemyRef.Movement.Value;
                
                float bestDistance = float.MinValue;
                TileCell bestTile = null;
                
                var avalaileTiles = _enemyRef.GetAvailableTilesInRange(
                    transform.position, _enemyRef.Movement.Value, false, false);

                while (index > 0)
                {
                    foreach (var item in avalaileTiles)
                    {
                        if (item.Value == index)
                        {
                            TileCell tile = _gridManager.GetTileAtPosition(item.Key);

                            float distance = Vector3.Distance(tile.transform.position,
                                _playerRef.transform.position);

                            if (tile.Walkable && distance > bestDistance)
                            {
                                bestDistance = distance;
                                bestTile = tile;
                            }
                        }
                    }

                    if (bestTile)
                    {
                        break;
                    }

                    index--;
                }

                if (bestTile)
                {
                    _enemyRef.FindAvailablePathToTarget(bestTile.transform.position, 
                        _enemyRef.MinimumPathCount, true, false, false);

                    if (_enemyRef.Path.Count > 0)
                    {
                        return BT_Status.SUCCESS;
                    }
                }
            }
        }

        _enemyRef.NbrOfMovementPerformed = _enemyRef.MaxNbrOfMovementPerTurn;
        return BT_Status.FAILURE;
    }
}
