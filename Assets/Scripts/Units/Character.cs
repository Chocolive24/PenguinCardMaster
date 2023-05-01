using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    // public Character(int hp, int attack, int shield)
    // {
    //     _hp = hp;
    //     _attack = attack;
    //     _shield = shield;
    // }
    //
    // [SerializeField] protected int _hp;
    // [SerializeField] protected int _attack;
    // [SerializeField] protected int _shield;
    // [SerializeField] protected int _movement;
    //
    // public int Movement
    // {
    //     get => _movement;
    //     set => _movement = value;
    // }
    //
    // // Start is called before the first frame update
    // void Start()
    // {
    //     
    // }
    //
    // // Update is called once per frame
    // void Update()
    // {
    //     
    // }
    //
    // public virtual void TakeDamage(int damage)
    // {
    //     _hp -= damage;
    // }
    //
    // public virtual void DisplayStats()
    // {
    //     Debug.Log(_hp + " " +  _attack + " " + _shield);
    // }
}
