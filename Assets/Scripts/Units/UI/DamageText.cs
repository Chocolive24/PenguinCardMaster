using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [SerializeField] private BaseUnit _selfUnit;
    [SerializeField] private GameObject _textGameObject;
    [SerializeField] private TextMeshProUGUI _damageTxt;
    
    [SerializeField]private float _animationTime = 0.25f;
    [SerializeField]private float _startFontSize = 0f;
    [SerializeField]private float _finalFontSize = 10f;

    private float percentSize;

    private void Awake()
    {
        _textGameObject.SetActive(false);
    }

    private void Start()
    {
        _selfUnit = GetComponent<BaseUnit>();
        _selfUnit.OnDamageTaken += DisplayDamageTaken;

        percentSize = _startFontSize / _finalFontSize;
    }

    private void OnDestroy()
    {
        _selfUnit.OnDamageTaken -= DisplayDamageTaken;
    }

    private void DisplayDamageTaken(BaseUnit unit, int damage)
    {
        _textGameObject.SetActive(true);
        _damageTxt.fontSize = _startFontSize;
        _damageTxt.text = damage.ToString();

        StartCoroutine(InterpolateFontSizeCo());
    }
    
    private IEnumerator InterpolateFontSizeCo() 
    {
        float countTime = 0;

        while( countTime <= _animationTime ) 
        { 
            float percentTime = countTime / _animationTime;

            _damageTxt.fontSize += (percentTime / 1.5f);

            yield return null; // wait for next frame
            countTime += Time.deltaTime;
        }
        
        _textGameObject.SetActive(false);
    }
}
