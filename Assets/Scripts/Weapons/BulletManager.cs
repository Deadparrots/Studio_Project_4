using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    private uint id;
    private uint owner_id;
    public uint pid
    {
        get { return id; }
        set { id = value; }
    }

    public uint ownerID
    {
        get { return owner_id; }
        set { owner_id = value; }
    }
}
