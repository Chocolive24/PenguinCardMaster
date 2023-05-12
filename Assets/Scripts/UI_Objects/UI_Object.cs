using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Object : MonoBehaviour
{
    [Header("UI_Object Attributes")]
    [SerializeField] protected GameObject _infoObject;
    [SerializeField] protected float _infoTriggerTime = 0.6f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnEventTriggerEnter()
    {
        StartCoroutine(nameof(TriggerEnterCo));
    }

    public void OnEventTriggerExit()
    {
        StopCoroutine(nameof(TriggerEnterCo));
        _infoObject.SetActive(false);
    }

    private IEnumerator TriggerEnterCo()
    {
        yield return new WaitForSeconds(_infoTriggerTime);
        _infoObject.SetActive(true);
    }
}
