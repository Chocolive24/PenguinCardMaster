using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    // Attributes ------------------------------------------------------------------------------------------------------
    [SerializeField] private GameObject _exploringCam, _exploringCam2;
    [SerializeField] private GameObject _battleCam;

    [SerializeField] private float _battleCamYoffset = 1.225f;
    
    // Methods ---------------------------------------------------------------------------------------------------------
    private void Awake()
    {
        GridManager.OnDungeonGenerate += SetCamerasSpawnPoint;
        BattleManager.OnBattleStart += ActiveBattleCam;
        BattleManager.OnBattleEnd += DesactivateBattleCam;
        DoorTileCell.OnDoorTileEnter += MoveCamera;
        ShopManager.OnShopExit += ExitShop;
    }
    
    private void SetCamerasSpawnPoint(GridManager arg1, RoomData startRoom)
    {
        Vector3 spawnPos = new Vector3(startRoom.Bounds.center.x, startRoom.Bounds.center.y, -10);
        
        _exploringCam.transform.position = spawnPos;
        _battleCam.transform.position = new Vector3(spawnPos.x, spawnPos.y - _battleCamYoffset, spawnPos.z);
    
        _exploringCam.SetActive(true);
        _battleCam.SetActive(false);
    }

    private void ActiveBattleCam(BattleManager obj, RoomData battleRoom)
    {
        _battleCam.SetActive(true);
        _exploringCam.SetActive(false);
        _exploringCam2.SetActive(false);
    }

    private void DesactivateBattleCam(BattleManager arg1, RoomData arg2)
    {
        _battleCam.SetActive(false);
        _exploringCam.SetActive(true);
        _exploringCam2.SetActive(false);
    }
    
    private void MoveCamera(DoorTileCell doorTile)
    {
        RoomData battleRoom = doorTile.GetRoomNeighbour();
        
        Vector3 spawnPos = new Vector3(battleRoom.Bounds.center.x, battleRoom.Bounds.center.y, -10);
        
        _battleCam.transform.position = new Vector3(spawnPos.x, spawnPos.y - _battleCamYoffset, spawnPos.z);

        _exploringCam2.transform.position = spawnPos;
        
        _exploringCam.SetActive(false);
        _exploringCam2.SetActive(true);

        StartCoroutine(MoveCamCo(spawnPos));
    }

    private IEnumerator MoveCamCo(Vector3 spawnPos)
    {
        yield return new WaitForSeconds(1f);
        _exploringCam.transform.position = spawnPos;
        _exploringCam.SetActive(true);
        _exploringCam2.SetActive(false);
    }

    private IEnumerator BattleCamCo()
    {
        yield return new WaitForSeconds(1f);
        _exploringCam.SetActive(false);
        _exploringCam2.SetActive(false);
        _battleCam.SetActive(true);
    }

    private void ExitShop(ShopManager shop)
    {
        MoveCamera(shop.Door);
    }
    
    private void OnDestroy()
    {
        GridManager.OnDungeonGenerate -= SetCamerasSpawnPoint;
        BattleManager.OnBattleStart -= ActiveBattleCam;
        BattleManager.OnBattleEnd -= DesactivateBattleCam;
        DoorTileCell.OnDoorTileEnter -= MoveCamera;
        ShopManager.OnShopExit -= ExitShop;
    }
}
