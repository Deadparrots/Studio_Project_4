using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Input.mousePosition;

        Vector3 temp = new Vector3(Screen.width / 2, Screen.height / 2, 0) - Input.mousePosition;
        float angle = -(Mathf.Atan2(temp.y, temp.x) * Mathf.Rad2Deg) - 90;
        Globals.MouseToCenterAngle = angle;
        transform.rotation = Quaternion.Euler(0, 0, -angle);
    }
}
