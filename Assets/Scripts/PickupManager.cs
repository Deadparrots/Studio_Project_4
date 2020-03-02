using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupManager : MonoBehaviour
{
    private uint id;
    private Vector3 position;
    private float heal = 25.0f;
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

    public float GetHeal()
    {
        return heal;
    }

    private void Update()
    {
        gameObject.transform.position = position;
    }
}
