using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShipMovement : MonoBehaviour
{
    public float verticalInputAcceleration = 1;
    public float horizontalInputAcceleration = 20;

    public float maxSpeed = 10;
    public float maxRotationSpeed = 100;

    public float velocityDrag = 1;
    public float rotationDrag = 1;

    private bool isPlayer = false;

    [SerializeField]
    private Sprite[] sprites;

    private Vector3 velocity;
    [SerializeField]
    private float zRotationVelocity;
   // [SerializeField]
    public bool isShip = false;

    //Task 2 Step 1
    // Add new variables
    public Vector3 server_pos;
    public float serverRotation;
    public Vector3 client_pos;
    public float clientRotation;
    public Vector3 server_Velocity;
    public float ratio;


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
    
    public int Init(bool _isPlayer)
    {
        isPlayer = _isPlayer;
        int spriteNum = 0;
        velocity = Vector3.zero;
        zRotationVelocity = 0.0f;

        if (_isPlayer)
        {
            spriteNum = Random.Range(0, 3);
            GetComponent<SpriteRenderer>().sprite = sprites[spriteNum];
        }
        else
        {
            serverRotation = 0;
            clientRotation = 0;
            server_Velocity = Vector3.zero;
            ratio = 0.8f;

            // step 4: At first, set all server & client variables to the same 
            transform.position = server_pos = client_pos = Vector3.zero;
        }

        return spriteNum;
    }

  
    private void Update()
    {
        float zTurnAcceleration = 0;
        float pi = 3.141592654f * 2;
        // apply forward input
        if (isShip)
        {
            if (isPlayer)
            {
                Vector3 acceleration = Input.GetAxis("Vertical") * verticalInputAcceleration * transform.up;
                velocity += acceleration * Time.deltaTime;

                // apply turn input
                zTurnAcceleration = -1 * Input.GetAxis("Horizontal") * horizontalInputAcceleration;

                zRotationVelocity += zTurnAcceleration * Time.deltaTime;

                //Debug.Log(zRotationVelocity + "  " + Time.timeScale);

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    //TODO Spawn Shot
                   // Client_Demo.Instance.SendShotRequest();
                }
            }
            else
            {

                // step 5 : change the way angular velocity is updated (by ratio)
                server_Velocity += transform.up * Time.deltaTime;
                serverRotation += pRotationVelocity * Time.deltaTime;

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

                velocity = server_Velocity;


            }
        }

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

        // apply rotation drag
        zRotationVelocity = zRotationVelocity * (1 - Time.deltaTime * rotationDrag);

        // clamp to maxRotationSpeed
        zRotationVelocity = Mathf.Clamp(zRotationVelocity, -maxRotationSpeed, maxRotationSpeed);

        // update transform
        if(isPlayer)
            transform.position += velocity * Time.deltaTime;
        else
        {

            // step 6 : change the way movement is updated by using ratio.
            server_pos += server_Velocity * Time.deltaTime;

            /* do interpolation if position goes out of screen
             * 
             * 
             */

            client_pos += velocity * Time.deltaTime;
            client_pos = Vector3.MoveTowards(client_pos,server_pos,velocity.magnitude * Time.deltaTime);

            /* do interpolation if position goes out of screen
             * 
             * 
             */
            float x = ratio * server_pos.x + (1 - ratio) * client_pos.x;
            float y = ratio * server_pos.y + (1 - ratio) * client_pos.y;

            transform.position = new Vector3(x, y, 0);

            //if (ratio < 1)
            //{
            //    // interpolating ratio step
            //    ratio += Time.deltaTime;
            //    if (ratio > 1)
            //        ratio = 0.5f;
            //}

        }

        transform.Rotate(0, 0, zRotationVelocity * Time.deltaTime);
    }
}
