using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    //[SerializeField] private GameObject _primaryButton;

    //[SerializeField] private AI_TypeSO _aiTypeSO;

    //[SerializeField] private HeroData _playerData;
    //[SerializeField] private DungeonData _dungeonData;
    
    [SerializeField] private Texture2D _baseMouseCursor;
    
    private List<HeroData> _playerData;
    private List<EnemyData> _enemiesData;
    private List<EnemyData> _spawnedEnemiesData;

    private List<DeckData> _movementDecksData;
    private List<DeckData> _mainDecksData;

    private List<DungeonData> _dungeonData;
    private List<SO_RoomsData> _dungeonRoomsData;

    // Start is called before the first frame update
    void Start()
    {
        //EventSystem.current.SetSelectedGameObject(_primaryButton);

        //Cursor.SetCursor(_baseMouseCursor, Vector2.zero, CursorMode.ForceSoftware);
        
        _playerData = Resources.LoadAll<HeroData>("Units/Heroes").ToList();
        _enemiesData = Resources.LoadAll<EnemyData>("Units/Enemies").ToList();
        _spawnedEnemiesData = Resources.LoadAll<EnemyData>("Units/SpawnedEnemies").ToList();

        _movementDecksData = Resources.LoadAll<DeckData>("Decks/MovementDecks").ToList();
        _mainDecksData = Resources.LoadAll<DeckData>("Decks/MainDecks").ToList();

        _dungeonData = Resources.LoadAll<DungeonData>("Dungeon/IntReferences").ToList();
        _dungeonRoomsData = Resources.LoadAll<SO_RoomsData>("Dungeon/RoomsList").ToList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void QuiteWithSave()
    {
        Application.Quit();
    }

    public void ResetValues()
    {
        // Player Data -------------------------------------------------------------------------------------------------
        foreach (var heroData in _playerData)
        {
            foreach (var intRef in heroData.IntReferences)
            {
                intRef.ResetValue();
            }
        }
        
        // Enemies Data ------------------------------------------------------------------------------------------------
        foreach (var enemyData in _enemiesData)
        {
            foreach (var intRef in enemyData.IntReferences)
            {
                intRef.ResetValue();
            }
        }
        
        foreach (var spawnedEnemyData in _spawnedEnemiesData)
        {
            foreach (var intRef in spawnedEnemyData.IntReferences)
            {
                intRef.ResetValue();
            }
        }
        
        // Decks Data --------------------------------------------------------------------------------------------------
        foreach (var moveDeckData in _movementDecksData)
        {
            moveDeckData.CardDeckData.Clear();
        }
        
        foreach (var mainDeckData in _mainDecksData)
        {
            mainDeckData.CardDeckData.Clear();
        }
        
        // Dungeon Data ------------------------------------------------------------------------------------------------
        foreach (var dungeonData in _dungeonData)
        {
            foreach (var intRef in dungeonData.IntReferences)
            {
                intRef.ResetValue();
            }
        }

        foreach (var roomData in _dungeonRoomsData)
        {
            roomData.Rooms.Clear();
        }
    }
}
