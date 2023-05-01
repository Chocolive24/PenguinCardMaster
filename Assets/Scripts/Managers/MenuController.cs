using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    //[SerializeField] private GameObject _primaryButton;

    [SerializeField] private AI_TypeSO _aiTypeSO;
    
    
    // Start is called before the first frame update
    void Start()
    {
        //EventSystem.current.SetSelectedGameObject(_primaryButton);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
