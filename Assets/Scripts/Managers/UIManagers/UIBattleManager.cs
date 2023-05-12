using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

public class UIBattleManager : MonoBehaviour
{
    // Singleton -------------------------------------------------------------------------------------------------------
    #region Singleton

    private static UIBattleManager _instance;
    public static UIBattleManager Instance { get { return _instance; } }

    #endregion
    
    // References ------------------------------------------------------------------------------------------------------
    #region UIGameobjects
    [Header("Panels")]
    [SerializeField] private GameObject _battlePanel;
    [SerializeField] private GameObject _exploringPanel;
    [SerializeField] private GameObject _victoryPanel;
    [SerializeField] private GameObject _goldsPanel;
    [SerializeField] private GameObject _pauseMenuPanel;
    [SerializeField] private GameObject _nextFloorQuestionPanel;
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private GameObject _gameWonPanel;
    
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI _currentTurnTxt;
    [SerializeField] private TextMeshProUGUI _notEnoughManaTxt;
    [SerializeField] private TextMeshProUGUI _manaNbrTxt;
    [SerializeField] private TextMeshProUGUI _golds;
    
    [Header("IntReferences")]
    [SerializeField] private IntReference _playerMaxMana;
    [SerializeField] private IntReference _currentFloorNbr;
    
    [SerializeField] private Button _endTurnButton;
    [SerializeField] private Image _manaContainerImage;
    
    [SerializeField] private GameObject _selectedHeroObject, _tileObject, _tileUnitObject;
    
    [SerializeField] private GameObject _cardAoeRenderer;

    
    
    
    #endregion

    #region Managers

    private BattleManager _battleManager;
    private CardPlayedManager _cardPlayedManager;
    private UnitsManager _unitsManager;
    private GameManager _gameManager;
    

    #endregion
    
    // Events ----------------------------------------------------------------------------------------------------------
    public static event Action<UIBattleManager> OnNextReward; 

    // Getters and Setters ---------------------------------------------------------------------------------------------
    #region Getters and Setters

    public GameObject BattlePanel { get => _battlePanel; set => _battlePanel = value; }

    public GameObject ExploringPanel => _exploringPanel;
    public GameObject VictoryPanel
    {
        get => _victoryPanel;
        set => _victoryPanel = value;
    }

    public Button EndTurnButton => _endTurnButton;

    public GameObject CardAoeRenderer => _cardAoeRenderer;
    public GameObject NextFloorQuestionPanel => _nextFloorQuestionPanel;

    public bool MustTilesBeDisabled => _victoryPanel.activeSelf ||
                                       _nextFloorQuestionPanel.activeSelf ||
                                       _pauseMenuPanel.activeSelf ||
                                       _gameOverPanel.activeSelf ||
                                       _gameWonPanel.activeSelf;

    #endregion
    
    // Methods ---------------------------------------------------------------------------------------------------------
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        
        //_battlePanel.SetActive(false);
        _victoryPanel.SetActive(false);
        _notEnoughManaTxt.gameObject.SetActive(false);

        BattleManager.OnVictoryEnter += HandleVictoryEnterUI;
        BaseCard.OnNoTEnoughMana += DisplayNotEnoughManaTxt;
        BaseMoveCard.OnPathStarted += DesactivateEndTurnButton;
        BaseUnit.OnPathEnded += ActivateEndTurnButton;
        DoorTileCell.OnDoorTileEnter += ActivateQuestionPanel;
    }

    private void ActivateQuestionPanel(DoorTileCell tile)
    {
        var room = tile.GetRoomNeighbour();

        if (room.Type == RoomData.RoomType.END && !room.HasEnemiesToFight)
        {
            _victoryPanel.SetActive(true);
            _nextFloorQuestionPanel.SetActive(true);
        }
    }


    private void OnDestroy()
    {
        BattleManager.OnVictoryEnter -= HandleVictoryEnterUI;
        BaseCard.OnNoTEnoughMana -= DisplayNotEnoughManaTxt;
        BaseMoveCard.OnPathStarted -= DesactivateEndTurnButton;
        BaseUnit.OnPathEnded -= ActivateEndTurnButton;
        DoorTileCell.OnDoorTileEnter -= ActivateQuestionPanel;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        _battleManager = BattleManager.Instance;
        _cardPlayedManager = CardPlayedManager.Instance;
        _unitsManager = UnitsManager.Instance;
        _gameManager = GameManager.Instance;

        _unitsManager.HeroPlayer.OnDeath += SetGameOverPanel;
    }

    private void SetGameOverPanel(BaseUnit obj)
    {
        obj.OnDeath -= SetGameOverPanel;
        
        _gameOverPanel.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (_gameManager.IsInBattleState)
        {
            if (_unitsManager.HeroPlayer)
            {
                if (!_cardPlayedManager.HasACardOnIt)
                {
                    _manaNbrTxt.text = _unitsManager.HeroPlayer.CurrentMana.ToString();
                    _manaContainerImage.fillAmount = _unitsManager.HeroPlayer.CurrentMana / (float)_playerMaxMana.Value;
                }
                else
                {
                    _manaNbrTxt.text = _unitsManager.HeroPlayer.CurrentMana.ToString();
                    _manaContainerImage.fillAmount = _unitsManager.HeroPlayer.CurrentMana / (float)_playerMaxMana.Value;
                }
            }
        }
    }

    public void EndTheTurn()
    {
        _battleManager.IsPlayerTurn = false;
    }

    public void ShowTileInfo(TileCell tile)
    {
        if (tile == null)
        {
            _tileObject.SetActive(false);
            _tileUnitObject.SetActive(false);
            return;
        }
        
        _tileObject.GetComponentInChildren<TextMeshProUGUI>().text = tile.Name;
        _tileObject.SetActive(true);
        
        if (tile.OccupiedUnit)
        {
            string tileUnitObjTxt;
            tileUnitObjTxt = tile.OccupiedUnit.UnitName;
            
            BaseEnemy enemy = tile.OccupiedUnit.GetComponent<BaseEnemy>();
            BaseHero hero = tile.OccupiedUnit.GetComponent<BaseHero>();
            
            if (enemy)
            {
                if (_unitsManager.HeroPlayer.CanPlay)
                {
                    tileUnitObjTxt += "\n dist. " + 
                                      enemy.CalculDistanceFromSelf(
                                          _unitsManager.HeroPlayer.transform.position, 
                                          true, false, false);

                    tileUnitObjTxt += "\n HP : " + enemy.CurrentHp.Value;
                
                    tileUnitObjTxt += "\n Damage : " + enemy.BaseAttackDamage.Value;
                }
            }
            else if (hero)
            {
                tileUnitObjTxt += "\n HP : " + hero.CurrentHp.Value;
            }

            _tileUnitObject.GetComponentInChildren<TextMeshProUGUI>().text = tileUnitObjTxt;
        
            _tileUnitObject.SetActive(true);
        }
    }

    private void DesactivateEndTurnButton(BaseMoveCard obj)
    {
        _endTurnButton.interactable = false;
    }
    
    private void ActivateEndTurnButton(BaseUnit unit)
    {
        if (unit.Faction == Faction.Hero)
        {
            _endTurnButton.interactable = true;
        }
        
    }
    
    private void HandleVictoryEnterUI(BattleManager battleManager, int golds)
    {
        if (_battleManager.BattleRoom.Type == RoomData.RoomType.END && _currentFloorNbr.Value == 3)
        {
            _gameWonPanel.SetActive(true);
        }
        else
        {
            _victoryPanel.SetActive(true);
            _goldsPanel.SetActive(true);
            _cardAoeRenderer.SetActive(false);
            _golds.text = "You received " + golds + " golds !";
        }
    }

    public void OnNextButtonClick()
    {
        OnNextReward?.Invoke(this);
        
        _goldsPanel.SetActive(false);
        _cardAoeRenderer.SetActive(true);
    }
    
    private void DisplayNotEnoughManaTxt(BaseCard obj)
    {
        StopCoroutine(nameof(NotEnoughManaCo));
        StartCoroutine(nameof(NotEnoughManaCo));
    }
    
    public IEnumerator NotEnoughManaCo()
    {
        _notEnoughManaTxt.gameObject.SetActive(true);

        yield return new WaitForSeconds(1f);
        
        _notEnoughManaTxt.gameObject.SetActive(false);
    }

    public void SetCurrentTurnText(string currentTurn)
    {
        _currentTurnTxt.text = currentTurn;
    }
}
