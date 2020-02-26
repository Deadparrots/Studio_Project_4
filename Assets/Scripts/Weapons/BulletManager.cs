using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    private uint id;
    public uint pid
    {
        get { return id; }
        set { id = value; }
    }
}
