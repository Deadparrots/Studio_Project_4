using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockRelativePosition : MonoBehaviour
{
    public GameObject Reference;
    public Vector3 Position;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPos = Reference.transform.position + Position;
        transform.position = newPos;
    }
}
