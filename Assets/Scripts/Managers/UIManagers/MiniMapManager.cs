using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapManager : MonoBehaviour
{
    [SerializeField] private Image _miniMapImage;
    [SerializeField] private GameObject _roomUIPrefab;

    private DungeonGenerator _dungeonGenerator;
    
    private Dictionary<Vector3, Image> _roomsUI;

    private Image _currentRoomUI;
    private Vector3 _miniMapCenter;
    private RectTransform _miniMapRectTransfrom;
    private RectTransform _roomRectTransform;
    
    // Methods ---------------------------------------------------------------------------------------------------------
    private void Awake()
    {
        _roomsUI = new Dictionary<Vector3, Image>();
        
        DoorTileCell.OnDoorTileEnter += UpdateMiniMap;
        ShopManager.OnShopExit += ExitShopUpdate;
    }

    private void OnDestroy()
    {
        DoorTileCell.OnDoorTileEnter -= UpdateMiniMap;
        ShopManager.OnShopExit -= ExitShopUpdate;
    }

    // Start is called before the first frame update
    void Start()
    {
        _dungeonGenerator = FindObjectOfType<DungeonGenerator>();
        
        _miniMapRectTransfrom = _miniMapImage.GetComponent<RectTransform>();
        _roomRectTransform = _roomUIPrefab.GetComponent<RectTransform>();
        
        _miniMapCenter = _miniMapRectTransfrom.position;
        
        _currentRoomUI = CreateRoomInMiniMap(_miniMapCenter, Color.green);
        
        CreateRoomNeighboursInMiniMap(_dungeonGenerator.Rooms.First().Value);
    }

    private Image CreateRoomInMiniMap(Vector3 position, Color color)
    {
        var roomUI = Instantiate(_roomUIPrefab, position, Quaternion.identity, _miniMapImage.transform);

        var roomImage = roomUI.GetComponent<Image>();
        roomImage.color = color;

        _roomsUI[position] = roomImage;

        return roomImage;
    }

    private void UpdateMiniMap(DoorTileCell doorTile)
    {
        ChangeColor(doorTile.Room.Type);

        Vector3 posInMiniMap = GetNextRoomPosInMiniMap(doorTile.Direction);
        
        if (!_roomsUI.ContainsKey(posInMiniMap))
        {
            _currentRoomUI = CreateRoomInMiniMap(posInMiniMap, Color.green);
        }
        else
        {
            _currentRoomUI = _roomsUI[posInMiniMap];
            _currentRoomUI.color = Color.green;
        }
        
        CreateRoomNeighboursInMiniMap(doorTile.GetRoomNeighbour());
    }

    private void CreateRoomNeighboursInMiniMap(RoomData room)
    {
        RoomData roomNeighbour = room;
        var roomNeighbourDoors = roomNeighbour.DoorsData;

        foreach (var door in roomNeighbourDoors)
        {
            Vector3 neighbourPosInMiniMap = GetNextRoomPosInMiniMap(door.Value);

            if (!_roomsUI.ContainsKey(neighbourPosInMiniMap))
            {
                Color color = new Color(0.35f, 0.35f, 0.35f, 1f);

                foreach (var neighbour in roomNeighbour.RoomNeighbours)
                {
                    foreach (var neighbourDoor in neighbour.Value.DoorsData)
                    {
                        var position = new Vector3(
                            Neighbourhood.CardinalNeighbours[neighbourDoor.Value].x,
                            Neighbourhood.CardinalNeighbours[neighbourDoor.Value].y, 0);

                        if (neighbour.Value.DoorsData.ContainsKey(door.Key - position))
                        {
                            if (neighbour.Value.Type == RoomData.RoomType.END)
                            {
                                color = Color.red;
                            }
                        }
                    }
                }

                CreateRoomInMiniMap(neighbourPosInMiniMap, color);
            }
        }
    }

    private void ChangeColor(RoomData.RoomType roomType)
    {
        switch (roomType)
        {
            case RoomData.RoomType.START:
            case RoomData.RoomType.BASIC:
                _currentRoomUI.color = Color.white;
                break;
            case RoomData.RoomType.SHOP:
                _currentRoomUI.color = Color.yellow;
                break;
            case RoomData.RoomType.END:
                _currentRoomUI.color = Color.red;
                break;
            default:
                _currentRoomUI.color = Color.white;
                break;
        }
    }

    private Vector3 GetNextRoomPosInMiniMap(Neighbourhood.Direction direction)
    {
        Vector3 basePos = _currentRoomUI.transform.position;
        
        switch (direction)
        {
            case Neighbourhood.Direction.UP:
                return new Vector3(basePos.x, basePos.y + _roomRectTransform.rect.height, basePos.z);
            case Neighbourhood.Direction.RIGHT:
                return new Vector3(basePos.x + _roomRectTransform.rect.width, basePos.y, basePos.z);
            case Neighbourhood.Direction.LEFT:
                return new Vector3(basePos.x - _roomRectTransform.rect.width, basePos.y, basePos.z);
            case Neighbourhood.Direction.DOWN:
                return new Vector3(basePos.x, basePos.y - _roomRectTransform.rect.height, basePos.z);
            default:
                return Vector3.zero;
        }
    }
    
    private void ExitShopUpdate(ShopManager shop)
    {
        UpdateMiniMap(shop.Door);
    }
}
