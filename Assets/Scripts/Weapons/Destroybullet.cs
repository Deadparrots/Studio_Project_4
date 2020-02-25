using UnityEngine;
using System.Collections;

public class Destroybullet : MonoBehaviour
{
    public GameObject bullet;
    // Use this for initialization
    void Start()
    {
        Destroy(bullet, 1.5f);
    }

}
