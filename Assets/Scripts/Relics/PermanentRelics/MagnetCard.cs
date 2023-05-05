using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetCard : PermanentRelic
{
   [SerializeField] private IntReference _playerMaxNbrOfCardToDrawPerTurn;
   [SerializeField] private int _supplCardNbrToDraw;

   protected override void Start()
   {
      base.Start();
      _valueToBuff = _playerMaxNbrOfCardToDrawPerTurn;
      _valueToAdd = _supplCardNbrToDraw;
   }
}
