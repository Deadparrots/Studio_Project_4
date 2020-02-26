using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCheck : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(gameObject.name + " Collided with "+ collision.gameObject.name);

        //if(gameObject.name == "ShipObj(Clone)")
        //{
        ////    Debug.Log("moving");
        //    gameObject.GetComponent<ShipMovement>().pVelocity = collision.gameObject.GetComponent<ShipMovement>().pVelocity;
        //    return;
        //}

        //if(collision.gameObject.name == "ShotSprite")
        //{
        //    Debug.Log("HIT");
        //    if (collision.gameObject.GetComponent<ShotMovement>().source == null) //prevents first collision, supposedly its parent ship
        //    {
        //        collision.gameObject.GetComponent<ShotMovement>().source = gameObject; // Gameobject will be a ship
        //    }
        //    if (collision.gameObject.GetComponent<ShotMovement>().source != gameObject) //TODO: Replace with a message to delete
        //    {
        //        Client_Demo.Instance.SendShipDeleteRequest(gameObject.GetComponentInParent<ShipManager>());
        //        Client_Demo.Instance.SendShotDeleteRequest(collision.gameObject);
        //    }
        //    return;
        //}
    }
   
}
