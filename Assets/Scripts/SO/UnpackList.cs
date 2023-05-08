using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnpackList : MonoBehaviour
{

    [SerializeField] private SO_List _list;
    [SerializeField] private Text _textList;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _textList.text = _list.HowMany.ToString();
    }
}
