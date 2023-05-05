using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RescueMana : ActivableRelic
{
    [SerializeField] private int _manaToRestore = 3;
    
    // Getters and Setters ---------------------------------------------------------------------------------------------
    public int ManaToRestore => _manaToRestore;
}
