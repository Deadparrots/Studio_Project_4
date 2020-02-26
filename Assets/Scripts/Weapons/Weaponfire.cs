using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weaponfire : MonoBehaviour
{
    public Rigidbody bullet;
    public Transform gun;
  //  GameObject bullets = GameObject.Find("bullet");
    void Update()
    {
       // if ((bullets.GetComponent<GameData>().inventory1 == 1 && bullets.GetComponent<GameData>().inventorychoice == 1) || (bullets.GetComponent<GameData>().inventorychoice == 2 && bullets.GetComponent<GameData>().inventory2 == 1))
      // {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Rigidbody gunInstance;
                gunInstance = Instantiate(bullet, gun.position, gun.rotation) as Rigidbody;
                gunInstance.AddForce(gun.forward * 500);
            Debug.Log("shot");
            }
       // }
    }
    void OnCollisionEnter3D(Collision collision)
    {
        Debug.Log("hit");
       // if ((bullets.GetComponent<GameData>().inventory1 == 1 && bullets.GetComponent<GameData>().inventorychoice == 1) || (bullets.GetComponent<GameData>().inventorychoice == 2 && bullets.GetComponent<GameData>().inventory2 == 1))
       // {
            collision.gameObject.GetComponent<GameData>().hp -= 1;
        Destroy(bullet);
        //bullet.GetComponent<ActiveStateToggler>().ToggleActive();
        //  }
    }
}
