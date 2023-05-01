using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image _healthBarImage;
    
    // Methods ---------------------------------------------------------------------------------------------------------
    
    // Start is called before the first frame update
    void Awake()
    {
        
    }

    public void UpdateHealthBar(int currentHP, int maxHP)
    {
        _healthBarImage.fillAmount = currentHP / (float)maxHP;
    }
}
