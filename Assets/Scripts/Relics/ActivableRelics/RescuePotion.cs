using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RescuePotion : ActivableRelic
{
    [SerializeField] private int _heal = 10;

    // Getter and Setters ----------------------------------------------------------------------------------------------
    public int Heal => _heal;
}
