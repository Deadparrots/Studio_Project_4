﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float verticalInputAcceleration = 1;
    public float horizontalInputAcceleration = 20;
    public float movementSpeed = 0.0f;
    PlayerManager manager;
    public float maxSpeed = 10;
    public float maxRotationSpeed = 100;

    public float velocityDrag = 1;
    public float rotationDrag = 1;

    public bool isPlayer = false;

    private Vector3 velocity = new Vector3();
    private Vector3 rotation;
    private Vector3 defaultScale;
    //Task 2 Step 1
    // Add new variables
    public Vector3 server_pos;
    public Vector3 serverRotation;
    public Vector3 client_pos;
    public Vector3 clientRotation;
    public Vector3 server_Velocity;
    public float ratio;


    public Vector3 pVelocity
    {
        get { return velocity; }
        set { velocity = value; }
    }

    public Vector3 pRotation
    {
        get { return rotation; }
        set { rotation = value; }
    }

    public Vector3 GetDefScale()
    {
        return defaultScale;
    }

    public int Init(bool _isPlayer)
    {
        isPlayer = _isPlayer;
        velocity = Vector3.zero;
        
        if (_isPlayer)
        {
        }
        else
        {
            serverRotation = server_Velocity = clientRotation = Vector3.zero;
            ratio = 1;
            // step 4: At first, set all server & client variables to the same 
            transform.position = server_pos = client_pos = Vector3.zero;
            transform.rotation.eulerAngles.Set(clientRotation.x, clientRotation.y, clientRotation.z);
        }

        return 0;
    }


    private void Update()
    {
        //float zTurnAcceleration = 0;
        //float pi = 3.141592654f * 2;
        // apply forward input

        if (isPlayer)
        {

            if (InputManager.inputManager != null)
                if (InputManager.inputManager.Menu != null)
                    if (InputManager.inputManager.Menu.activeSelf)
                        return;

            velocity.Set(0, 0, 0);
            if (Input.GetKey(InputManager.MoveUp))
            {
                //print("w key is held down");
                velocity.z = movementSpeed;
            }

            else if (Input.GetKey(InputManager.MoveDown))
            {
                //print("s key is held down");
                velocity.z = -movementSpeed;

            }

            if (Input.GetKey(InputManager.MoveLeft))
            {
                //print("a key is held down");
                velocity.x = -movementSpeed;
            }

            else if (Input.GetKey(InputManager.MoveRight))
            {
                //print("d key is held down");
                velocity.x = movementSpeed;
            }

            if (velocity.magnitude > 0) // Forces the player's velocity to be actually the movement speed
            {
                velocity = velocity * (movementSpeed / velocity.magnitude);
                if (manager)
                    manager.isMoving = true;            
            }
            else if (manager)
                    manager.isMoving = false;

            if (Input.GetKey(InputManager.MoveForward))
            {
                float radian = -(Globals.MouseToCenterAngle - 90) * Mathf.Deg2Rad;
                velocity = (new Vector3(Mathf.Cos(radian), 0, Mathf.Sin(radian))) * movementSpeed;
            }

            if (Input.GetKeyDown(InputManager.Weapon1))
            {
                Client_Demo.Instance.Sendgun();
            }
            
            if (transform.position.y > 0)
            {
                velocity.y -= 5;
            }
            else if (0 > transform.position.y)
            {
                transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            }

            //if (Input.GetKey("up"))
            //{
            //    print("up arrow key is held down");
            //    rotation.y += movementSpeed * Time.deltaTime;

            //    if (rotation.y > 360)
            //        rotation.y = 0;
            //}

            //else if (Input.GetKey("down"))
            //{
            //    print("down arrow key is held down");
            //    rotation.y -= movementSpeed * Time.deltaTime;

            //    if (rotation.y < 0)
            //        rotation.y = 360;
            //}
            rotation.y = Globals.MouseToCenterAngle;
        }
    }
    private void FixedUpdate()
    {

        // update transform
        if (isPlayer)
        {
            transform.position += velocity * Time.deltaTime;
            transform.eulerAngles = new Vector3(rotation.x, rotation.y, rotation.z);
        }
        else
        {
            // step 6 : change the way movement is updated by using ratio.
            server_pos += server_Velocity * Time.deltaTime;

            /* do interpolation if position goes out of screen
             * 
             * 
             */

            client_pos += velocity * Time.deltaTime;
            //clientRotation = rotation;
            /* do interpolation if position goes out of screen
             * 
             * 
             */
            float x = ratio * server_pos.x + (1 - ratio) * client_pos.x;
            float y = ratio * server_pos.y + (1 - ratio) * client_pos.y;
            float z = ratio * server_pos.z + (1 - ratio) * client_pos.z;

            transform.position = new Vector3(x, y, z);

            if (ratio < 1)
            {
                // interpolating ratio step
                ratio += Time.deltaTime * 4;
                if (ratio > 1)
                {
                    client_pos.x = server_pos.x;
                    client_pos.y = server_pos.y;
                    client_pos.z = server_pos.z;
                    ratio = 0.1f;
                    //ratio = 1;
                }
            }
            transform.eulerAngles = new Vector3(serverRotation.x, serverRotation.y, serverRotation.z);
            //serverRotation.Set(0, 0, 0);
        }
    }

    private void Awake()
    {
        defaultScale = gameObject.transform.lossyScale; //either lossyScale or local Scale?
        manager = GetComponentInParent<PlayerManager>();
    }
}
