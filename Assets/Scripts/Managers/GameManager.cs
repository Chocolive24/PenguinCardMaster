using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton -------------------------------------------------------------------------------------------------------
    #region Singleton

    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    #endregion
    
    // Attributes ------------------------------------------------------------------------------------------------------
    private bool _isInBattleState = false;
    
    // References ------------------------------------------------------------------------------------------------------
    #region Gamebjects

    [SerializeField] private Transform _camTrans;
    [SerializeField] private TextMeshProUGUI _currentState;
    
    #endregion
    
    private BattleManager _battleManager;
    
    // State Pattern ---------------------------------------------------------------------------------------------------
    #region Game States

    private ExploringGameState _exploringGameState;
    private BattleGameState _battleGameState;

    #endregion
    
    // State Machine
    private StateMachine _stateMachine;

    // Getters and Setters ---------------------------------------------------------------------------------------------
    public static event Action OnGameStateChange;

    public bool IsInBattleState
    {
        get => _isInBattleState;
        set => _isInBattleState = value;
    }

    private float _time = 0;

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
        
        DoorTileCell.OnDoorTileEnter += SetBattleState;
    }

    private void OnDestroy()
    {
        DoorTileCell.OnDoorTileEnter -= SetBattleState;
    }

    private void SetBattleState(DoorTileCell doorTile)
    {
        RoomData roomToSetFight = doorTile.GetRoomNeighbour();

        if (roomToSetFight != null)
        {
            if (roomToSetFight.HasEnemiesToFight)
            {
                _isInBattleState = true;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        ReferenceManagers();

        CreateStatePattern();

        _camTrans.transform.position = new Vector3(7.5f, -10, -10);
    }
    
    private void ReferenceManagers()
    {
        _battleManager = BattleManager.Instance;
    }
    
    private void CreateStatePattern()
    {
        _exploringGameState = new ExploringGameState(this);
        _battleGameState = new BattleGameState(_battleManager);

        _stateMachine = new StateMachine();

        _stateMachine.AddTransition(_exploringGameState, _battleGameState, () => _isInBattleState);
        _stateMachine.AddTransition(_battleGameState, _exploringGameState, () => !_isInBattleState);

        _stateMachine.SetState(_exploringGameState);
    }

    // Update is called once per frame
    void Update()
    {
        _time += Time.deltaTime;

        _stateMachine.Tick();
    }
    
    public void UpdateGameState()
    {
        OnGameStateChange?.Invoke();
    }
}
