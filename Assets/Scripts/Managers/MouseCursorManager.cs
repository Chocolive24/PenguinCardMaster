using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCursorManager : MonoBehaviour
{
    [SerializeField] private Texture2D _baseMouseCursor;
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.SetCursor(_baseMouseCursor, Vector2.zero, CursorMode.ForceSoftware);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
