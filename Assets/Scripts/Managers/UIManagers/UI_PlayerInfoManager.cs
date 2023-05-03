using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UI_PlayerInfoManager : MonoBehaviour
{
    [Header("Int References")]
    [SerializeField] private IntReference _playerCurrentHp;
    [SerializeField] private IntReference _playerGolds;

    [Header("Text References")]
    [SerializeField] private TextMeshProUGUI _playerGoldsTxt;
    
    // Methods ---------------------------------------------------------------------------------------------------------

    private void OnEnable()
    {
        UpdateTextes();
    }

    private void UpdateTextes()
    {
        _playerGoldsTxt.text = _playerGolds.Value.ToString();
    }
}
