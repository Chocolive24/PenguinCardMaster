using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnpackToUI : MonoBehaviour
{
    [SerializeField] private SO_Storage _datas;
    [SerializeField] private TMP_Text _int;
    [SerializeField] private TMP_Text _float;
    [SerializeField] private TMP_Text _string;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _int.text = _datas.INTValues.ToString();
        _float.text = _datas.FloatValue.ToString();
        _string.text = _datas.StringValue;
    }
}
