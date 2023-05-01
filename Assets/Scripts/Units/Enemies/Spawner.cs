using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : BaseEnemy
{
    [SerializeField] private UnitData minionUnitData;

    [SerializeField] private int _maxNbrOfMinion = 3;
    private List<BaseEnemy> _minions = new List<BaseEnemy>();

    // Getters and Setters ---------------------------------------------------------------------------------------------
    public int MaxNbrOfMinion => _maxNbrOfMinion;
    public int CurrentNbrOfMinion => _minions.Count;

    // Methods ---------------------------------------------------------------------------------------------------------
    public void SpawnAMinion(Vector3 spawnPos)
    {
        Vector3 pos = GridManager.Instance.WorldToCellCenter(spawnPos);
        var minionObject = minionUnitData.BaseUnitPrefab;
        BaseEnemy EnemyMinionRef = (BaseEnemy)minionObject;
        
        BaseEnemy spawnedMinion = Instantiate(EnemyMinionRef, pos, Quaternion.identity);
        _gridManager.GetTileAtPosition(pos).SetUnit(spawnedMinion);
        _minions.Add(spawnedMinion);
        _unitsManager.Enemies.Add(spawnedMinion);
        spawnedMinion.PreviousOccupiedTiles = spawnedMinion.GetOccupiedTiles();
        spawnedMinion.OnTurnFinished += _unitsManager.SetNextEnemyTurn;
        spawnedMinion.OnDeath += RemoveMinionFromList;

        _nbrOfAttackPerformed = _maxNbrOfAttackPerTurn;
    }

    private void RemoveMinionFromList(BaseUnit obj)
    {
        _minions.Remove((BaseEnemy)obj);

        obj.GetComponent<BaseEnemy>().OnTurnFinished -= _unitsManager.SetNextEnemyTurn;
        obj.GetComponent<BaseEnemy>().OnDeath -= RemoveMinionFromList;
    }

    protected override void Update()
    {
        base.Update();
        Debug.Log("COUNT == " + _minions.Count);
    }
}
