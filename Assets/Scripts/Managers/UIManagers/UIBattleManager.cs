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
    [SerializeField] private GameObject _battlePanel;
    [SerializeField] private GameObject _exploringPanel;
    [SerializeField] private GameObject _VictoryPanel;
    [SerializeField] private Button _endTurnButton;
    [SerializeField] private TextMeshProUGUI _currentTurnTxt;
    [SerializeField] private TextMeshProUGUI _notEnoughManaTxt;
    [SerializeField] private TextMeshProUGUI _manaNbrTxt;
    [SerializeField] private Image _manaContainerImage;
    [SerializeField] private IntReference _playerMaxMana;
    [SerializeField] private GameObject _selectedHeroObject, _tileObject, _tileUnitObject;
    [SerializeField] private GameObject _goldsPanel;
    [SerializeField] private TextMeshProUGUI _golds;
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
        get => _VictoryPanel;
        set => _VictoryPanel = value;
    }

    public Button EndTurnButton => _endTurnButton;

    public GameObject CardAoeRenderer => _cardAoeRenderer;

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
        _VictoryPanel.SetActive(false);
        _notEnoughManaTxt.gameObject.SetActive(false);

        BattleManager.OnVictoryEnter += HandleVictoryEnterUI;
        BaseCard.OnNoTEnoughMana += DisplayNotEnoughManaTxt;
        BaseMoveCard.OnPathStarted += DesactivateEndTurnButton;
        BaseUnit.OnPathEnded += ActivateEndTurnButton;
    }

    

    private void OnDestroy()
    {
        BattleManager.OnVictoryEnter -= HandleVictoryEnterUI;
        BaseCard.OnNoTEnoughMana -= DisplayNotEnoughManaTxt;
        BaseMoveCard.OnPathStarted -= DesactivateEndTurnButton;
        BaseUnit.OnPathEnded -= ActivateEndTurnButton;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        _battleManager = BattleManager.Instance;
        _cardPlayedManager = CardPlayedManager.Instance;
        _unitsManager = UnitsManager.Instance;
        _gameManager = GameManager.Instance;
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
        _VictoryPanel.SetActive(true);
        _goldsPanel.SetActive(true);
        _cardAoeRenderer.SetActive(false);
        _golds.text = "You received " + golds + " golds !";
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
