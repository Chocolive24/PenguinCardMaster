using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _speed = 8;
    [SerializeField] private float _moveTime = 0.5f;

    private BoxCollider2D _boxCollider2D;

    private void Update()
    {
        if (Vector3.Distance(transform.position, UnitsManager.Instance.HeroPlayer.transform.position) <= 0.5)
        {
            Destroy(gameObject);
        }
    }

    public IEnumerator InterpolateMoveCo(Vector3 startPos, Vector3 endPos) 
    {
        float countTime = 0;
        
        while( countTime <= _moveTime && this) 
        { 
            float percentTime = countTime / _moveTime;
            transform.position = Vector3.Lerp(startPos, endPos, percentTime);
            
            yield return null; // wait for next frame
            countTime += Time.deltaTime;
        }
        
        // because of the frame rate and the way we are running Lerp(),
        // the last timePercent in the loop may not = 1
        // therefore, this line ensures we end exactly where desired.
        if (this)
        {
            transform.position = endPos;
        }
    }
}
