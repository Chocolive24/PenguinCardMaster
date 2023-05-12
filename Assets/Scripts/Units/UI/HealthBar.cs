using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image _healthBarImage;
    [SerializeField] private TextMeshProUGUI _hpText;
    
    // Methods ---------------------------------------------------------------------------------------------------------
    
    // Start is called before the first frame update
    void Awake()
    {
        
    }

    public void UpdateHealthBar(int currentHP, int maxHP)
    {
        _healthBarImage.fillAmount = currentHP / (float)maxHP;
        
        if (currentHP >= 0)
        {
            _hpText.text = currentHP + " / " + maxHP;
        }
        else
        {
            _hpText.text = 0 + " / " + maxHP;
        }
    }
}
