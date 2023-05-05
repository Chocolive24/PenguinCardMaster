using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ShopManager : MonoBehaviour
{
    // Attributes ------------------------------------------------------------------------------------------------------
    
    
    // References ------------------------------------------------------------------------------------------------------
    [SerializeField] private GameObject _shopPanel;
    [SerializeField] private GameObject _exploringPanel;

    [SerializeField] private GameObject _shopObjectPrefab;

    private RoomData _room;
    private DoorTileCell _door;

    private bool _hasCreateObjects = false;

    private RelicsManager _relicsManager;

    [SerializeField] private IntReference _playerGolds;

    [SerializeField] private TextMeshProUGUI _playerGoldsTxt;
    
    [Header("Cards Rect Transform")]
    [SerializeField] private RectTransform _rareCard1RectTrans;
    [SerializeField] private RectTransform _rareCard2RectTrans;
    [SerializeField] private RectTransform _epicCard1RectTrans;
    [SerializeField] private RectTransform _epicCard2RectTrans;
    [SerializeField] private RectTransform _legCard1RectTrans;
    [SerializeField] private RectTransform _legCard2RectTrans;
    [SerializeField] private RectTransform _aoeTilemapRectTrans;

    [Header("Cards Cost Textes")] 
    [SerializeField] private TextMeshProUGUI _rareCard1Cost;
    [SerializeField] private TextMeshProUGUI _rareCard2Cost;
    [SerializeField] private TextMeshProUGUI _epicCard1Cost;
    [SerializeField] private TextMeshProUGUI _epicCard2Cost;
    [SerializeField] private TextMeshProUGUI _legCard1Cost;
    [SerializeField] private TextMeshProUGUI _legCard2Cost;
    
    [Header("Relics Rect Transfrom")]
    [SerializeField] private RectTransform _relic1RectTrans;
    [SerializeField] private RectTransform _relic2RectTrans;

    [Header("Relics Cost Textes")] 
    [SerializeField] private TextMeshProUGUI _relic1Cost;
    [SerializeField] private TextMeshProUGUI _relic2Cost;
    
    private CardsManager _cardsManager;
    private GridManager _gridManager;
    private UnitsManager _unitsManager;

    // Events ----------------------------------------------------------------------------------------------------------
    public static event Action<ShopManager, Collectible> OnObjectBuy; 
    public static event Action<ShopManager> OnShopExit; 

    // Getters ans Setters ---------------------------------------------------------------------------------------------
    public RoomData Room => _room;

    public DoorTileCell Door => _door;

    // Methods ---------------------------------------------------------------------------------------------------------
    // Start is called before the first frame update
    private void Awake()
    {
        DoorTileCell.OnDoorTileEnter += ActivateShop;
        Collectible.OnBuy += BuyObject;
    }

    private void UpdateGoldsTxt()
    {
        _playerGoldsTxt.text = _playerGolds.Value.ToString();
    }

    private void Start()
    {
        _cardsManager = CardsManager.Instance;
        _gridManager = GridManager.Instance;
        _unitsManager = UnitsManager.Instance;

        _relicsManager = FindObjectOfType<RelicsManager>();
    }
    
    private void OnDestroy()
    {
        DoorTileCell.OnDoorTileEnter -= ActivateShop;
        Collectible.OnBuy -= BuyObject;
    }

    private void BuyObject(Collectible obj)
    {
        if (obj.ObjectCost <= _playerGolds.Value)
        {
            OnObjectBuy?.Invoke(this, obj);
            UpdateGoldsTxt();
        }
    }

    private void ActivateShop(DoorTileCell doorTile)
    {
        if (doorTile.GetRoomNeighbour().Type == RoomData.RoomType.SHOP)
        {
            _room = doorTile.GetRoomNeighbour();
            Vector3 pos = Neighbourhood.CardinalNeighbours[doorTile.Direction];
            var tile = _gridManager.GetTileAtPosition(doorTile.transform.position + pos);

            if ((DoorTileCell)(tile))
            {
                _door = (DoorTileCell)tile;
            }
            
            _shopPanel.SetActive(true);
            _exploringPanel.SetActive(false);
            
            UpdateGoldsTxt();

            DesactivateTiles(doorTile.GetRoomNeighbour());

            if (!_hasCreateObjects)
            {
                _hasCreateObjects = true;
                CreateObjects();
            }
            
        }
    }

    private void DesactivateTiles(RoomData room)
    {
        for (int x = room.Bounds.xMin; x < room.Bounds.xMax; x++)
        {
            for (int y = room.Bounds.yMin; y < room.Bounds.yMax; y++)
            {
                TileCell tile = _gridManager.GetTileAtPosition(
                    _gridManager.WorldToCellCenter(new Vector3(x, y, 0)));
                tile.BoxCollider2D.enabled = false;
            }
        }
    }

    private void CreateObjects()
    {
        CreateACard(_cardsManager.ScrMoveCards, Rarety.Rare, _rareCard1RectTrans, 
            _rareCard1Cost,50, 75);
        CreateACard(_cardsManager.ScrAttackCards, Rarety.Rare, _rareCard2RectTrans, 
            _rareCard2Cost,50, 75);
        
        CreateACard(_cardsManager.ScrMoveCards, Rarety.Epic, _epicCard1RectTrans,
            _epicCard1Cost, 120, 160);
        CreateACard(_cardsManager.ScrAttackCards, Rarety.Epic, _epicCard2RectTrans, 
            _epicCard2Cost,120, 160);
        
        CreateACard(_cardsManager.ScrMoveCards, Rarety.Legendary, _legCard1RectTrans, 
            _legCard1Cost, 200, 250);
        CreateACard(_cardsManager.ScrAttackCards, Rarety.Legendary, _legCard2RectTrans, 
            _legCard2Cost, 200, 250);

        Relic relic1 = CreateARelic(_relic1RectTrans, _relic1Cost, null);
        Relic relic2 = CreateARelic(_relic2RectTrans, _relic2Cost, relic1);
    }
    
    private void CreateACard(List<ScriptableCard> scrCards, Rarety rarety, RectTransform rect, TextMeshProUGUI costText,
        int minCost, int maxCost)
    {
        BaseCard cardRare1 = _cardsManager.InstantiateARandomCard(scrCards, rarety);
        cardRare1.transform.position = rect.position;
        cardRare1.transform.parent = rect.transform.parent;

        cardRare1.Init(Collectible.CollectibleType.SHOP_OBJECT, Random.Range(minCost, maxCost), rect.gameObject);
        
        costText.text = cardRare1.ObjectCost.ToString();
    }
    
    private Relic CreateARelic(RectTransform rect, TextMeshProUGUI costText, Relic relicToAvoid)
    {
        Relic rndRelic = _relicsManager.GetARandomRelic(relicToAvoid);
        Relic relic = Instantiate(rndRelic, rect.position, Quaternion.identity, rect.transform);

        relic.Init(Collectible.CollectibleType.SHOP_OBJECT, Random.Range(300, 350), rect.gameObject);

        relic.GetComponent<RectTransform>().sizeDelta = 2 * relic.BaseSizeDelta;
        
        costText.text = relic.ObjectCost.ToString();
        
        return relic;
    }

    public int GetARndCostInRange(int min, int max)
    {
        return Random.Range(min, max + 1);
    }
    
    public void OnExit()
    {
        _shopPanel.SetActive(false);
        _exploringPanel.SetActive(true);

        OnShopExit?.Invoke(this);
    }
}
