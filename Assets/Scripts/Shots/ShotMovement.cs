using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotMovement : MonoBehaviour
{
    public float verticalInputAcceleration = 1;
    public float horizontalInputAcceleration = 20;

    public float maxSpeed = 10;

    public float velocityDrag = 1;
    public float rotationDrag = 1;

    public uint id;

    [SerializeField]
    private Sprite[] sprites;

    private Vector3 velocity;
    [SerializeField]
    private float zRotationVelocity;

    public Vector3 server_pos;
    public float serverRotation;
    public Vector3 client_pos;
    public float clientRotation;
    public Vector3 server_Velocity;
    public float ratio;

    public GameObject source;
    private Rigidbody2D Rigidbody;

    public Vector3 pVelocity
    {
        get { return velocity; }
        set { velocity = value; }
    }
    public float pRotationVelocity
    {
        get { return zRotationVelocity; }
        set { zRotationVelocity = value; }
    }

    public void SetSprite(int _num)
    {
        GetComponent<SpriteRenderer>().sprite = sprites[_num];
    }

    public int Init()
    {
        int spriteNum = 0;
        velocity = Vector3.zero;
        zRotationVelocity = 0.0f;

        serverRotation = 0;
        clientRotation = 0;
        server_Velocity = Vector3.zero;
        ratio = 1;

        Rigidbody = gameObject.GetComponent<Rigidbody2D>();

        // step 4: At first, set all server & client variables to the same 
        transform.position = server_pos = client_pos = Vector3.zero;

        return spriteNum;
    }


    private void Update()
    {
        float pi = 3.141592654f * 2;
        // apply forward input

        // step 5 : change the way angular velocity is updated (by ratio)
        //server_Velocity += transform.up * Time.deltaTime;
        //serverRotation += pRotationVelocity * Time.deltaTime;

        

        if (serverRotation > pi)
            serverRotation -= pi;

        if (serverRotation < 0.0f)
            serverRotation += pi;

        clientRotation += pRotationVelocity * Time.deltaTime;

        if (clientRotation > pi)
            clientRotation -= pi;

        if (clientRotation < 0.0f)
            clientRotation += pi;

        pRotationVelocity = ratio * serverRotation + (1 - ratio) * clientRotation;

        /*  if (pRotationVelocity > pi)
              pRotationVelocity -= pi;

          if (pRotationVelocity < 0.0f)
              pRotationVelocity += pi;*/

    }
    private void FixedUpdate()
    {
        // apply velocity drag
        velocity = velocity * (1 - Time.deltaTime * velocityDrag);

        // clamp to maxSpeed
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        {
            // step 6 : change the way movement is updated by using ratio.
            server_pos += server_Velocity * Time.deltaTime;

            /* do interpolation if position goes out of screen
             * 
             * 
             */

            client_pos += velocity * Time.deltaTime;

            /* do interpolation if position goes out of screen
             * 
             * 
             */
            float x = ratio * server_pos.x + (1 - ratio) * client_pos.x;
            float y = ratio * server_pos.y + (1 - ratio) * client_pos.y;

            transform.position = new Vector3(x, y, 0);
            Rigidbody.position.Set(x, y);

            if (ratio < 1)
            {
                // interpolating ratio step
                ratio += Time.deltaTime * 4;
                if (ratio > 1)
                    ratio = 1;
            }

        }
    }
}
