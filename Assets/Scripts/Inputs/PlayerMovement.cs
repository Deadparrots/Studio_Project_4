using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(InputManager.MoveUp))
        {
            transform.position += new Vector3(0, 0, 0.1f); // temp
            //TODO: Send MoveUp to server
            //TODO: Constrain Camera
        }

        if (Input.GetKey(InputManager.MoveDown))
        {
            transform.position -= new Vector3(0, 0, 0.1f);
        }

        if (Input.GetKey(InputManager.MoveLeft))
        {
            transform.position -= new Vector3(0.1f, 0, 0);
        }

        if (Input.GetKey(InputManager.MoveRight))
        {
            transform.position += new Vector3(0.1f, 0, 0);
        }
    }
}
