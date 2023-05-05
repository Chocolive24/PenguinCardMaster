using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputs : MonoBehaviour
{
    private BaseHero _playerHero;

    private void Start()
    {
        _playerHero = GetComponent<BaseHero>();
    }

    public void HandleExploringMove()
    {
        // Yo
    }
    
    public void HandlePauseMenu(GameObject pauseMenuPanel)
    {
        pauseMenuPanel.SetActive(!pauseMenuPanel.activeSelf);
    }
}
