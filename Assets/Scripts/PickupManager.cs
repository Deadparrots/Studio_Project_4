using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupManager : MonoBehaviour
{
    private uint id;
    private Vector3 position;
    public uint pid
    {
        get { return id; }
        set { id = value; }
    }
    public Vector3 pPosition
    {
        get { return position; }
        set { position = value; }
    }

    private void Update()
    {
        gameObject.transform.position = position;
    }
}
