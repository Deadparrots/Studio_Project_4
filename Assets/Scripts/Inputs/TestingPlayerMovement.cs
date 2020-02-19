using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingPlayerMovement : MonoBehaviour
{

    public GameObject model;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 Movement = new Vector3(0, 0, 0);
        if (Input.GetKey(InputManager.MoveUp))
        {
            Movement += new Vector3(0, 0, 0.1f);
        }

        if (Input.GetKey(InputManager.MoveDown))
        {
            Movement -= new Vector3(0, 0, 0.1f);
        }

        if (Input.GetKey(InputManager.MoveLeft))
        {
            Movement -= new Vector3(0.1f, 0, 0);
        }

        if (Input.GetKey(InputManager.MoveRight))
        {
            Movement += new Vector3(0.1f, 0, 0);
        }

        if (Input.GetKey(InputManager.MoveForward))
        {
            float radian = -(Globals.MouseToCenterAngle - 90) * Mathf.Deg2Rad;
            Movement += new Vector3(Mathf.Cos(radian), 0, Mathf.Sin(radian)) / 10;
        }

        Movement = Movement.normalized * 0.1f;

        transform.position += Movement; // SEND TO SERVER

        model.transform.rotation = Quaternion.Euler(0, Globals.MouseToCenterAngle, 0);
    }
}
