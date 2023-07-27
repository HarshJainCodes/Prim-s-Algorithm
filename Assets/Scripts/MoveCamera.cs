using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{

    Vector3 prevMousePos = Vector3.zero;
    Vector3 currMousePos = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetMouseButton(2))
        {
            currMousePos = Input.mousePosition;
            currMousePos = Camera.main.ScreenToWorldPoint(currMousePos);
            Vector3 diff = currMousePos - prevMousePos;
            if (diff != Vector3.zero)
            {
                Camera.main.transform.position -= diff;
            }
            
            prevMousePos = currMousePos;
        }
    }
}
