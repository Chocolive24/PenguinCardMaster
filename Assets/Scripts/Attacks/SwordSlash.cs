using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSlash : MonoBehaviour
{
    [SerializeField] private float _lifeTime;

    private void OnEnable()
    {
        GetComponent<Animator>().SetBool("HasAttack", true);

        StartCoroutine(nameof(DeathCo));
    }

    private IEnumerator DeathCo()
    {
        yield return new WaitForSeconds(_lifeTime);
        
        Destroy(this.gameObject);
    }
}
