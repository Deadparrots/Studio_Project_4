using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotManager : MonoBehaviour
{
    private uint id;
    public GameObject childShip;
    private ShipMovement childScript;

    public uint pid
    {
        get { return id; }
        set { id = value; }
    }

    public float pRotation
    {
        get { return childShip.transform.eulerAngles.z; }
        set { childShip.transform.eulerAngles = new Vector3(0, 0, value); }
    }

    public Vector3 velocity
    {
        get { return childScript.pVelocity; }
        set { childScript.pVelocity = value; }
    }

    public float rotationVelocity
    {
        get { return childScript.pRotationVelocity; }
        set { childScript.pRotationVelocity = value; }
    }

    public Vector3 position
    {
        get { return childShip.transform.position; }
        set { childShip.transform.position = value; }
    }

    public void SetShip(bool _boolean)
    {
        Debug.Log("ship set!!");

        childScript.isShip = _boolean;
    }

    public void SetImg(int _num)
    {
        childScript.SetSprite(_num);
    }
    //Task 2
    // step 2: add getter setters here
    // look at how to do getter setters for C#
    // You should do for server_pos, serverRotation, server velocity

    public Vector3 server_pos
    {
        get { return childScript.server_pos; }
        set { childScript.server_pos = value; }
    }

    public Vector3 serverVelocity
    {
        get { return childScript.server_Velocity; }
        set { childScript.server_Velocity = value; }
    }

    public float serverRotation
    {
        get { return childScript.serverRotation; }
        set { childScript.serverRotation = value; }
    }

    void DoInterpolateUpdate()
    {
        childScript.client_pos = new Vector3(position.x, position.y, 0);
        childScript.clientRotation = pRotation;
        velocity = childScript.server_Velocity;
        childScript.ratio = 0;

    }
    protected void Awake()
    {
        childScript = GetComponentInChildren<ShipMovement>();
    }

}
