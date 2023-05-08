using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    //[SerializeField] private GameObject _primaryButton;

    //[SerializeField] private AI_TypeSO _aiTypeSO;

    //[SerializeField] private HeroData _playerData;
    [SerializeField] private DungeonData _dungeonData;
    
    private List<HeroData> _playerData;
    private List<EnemyData> _enemiesData;
    private List<EnemyData> _spawnedEnemiesData;

    // Start is called before the first frame update
    void Start()
    {
        //EventSystem.current.SetSelectedGameObject(_primaryButton);
        
        _playerData = Resources.LoadAll<HeroData>("Units/Heroes").ToList();
        _enemiesData = Resources.LoadAll<EnemyData>("Units/Enemies").ToList();
        _spawnedEnemiesData = Resources.LoadAll<EnemyData>("Units/SpawnedEnemies").ToList();
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
        
        // Dungeon Data ------------------------------------------------------------------------------------------------
        foreach (var intRef in _dungeonData.IntReferences)
        {
            intRef.ResetValue();
        }
    }
}
