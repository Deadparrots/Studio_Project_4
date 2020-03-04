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
        if (collision.gameObject.tag.Equals("EnemyWeapon"))
        {
            if (gameObject.name == "Body")
            {
                PlayerManager player = gameObject.GetComponentInParent<PlayerManager>();
                // Call server to deal weapon dmg to player
                // TODO: CHANGE THE DMG TO REFERENCE FROM WEAPONS DMG STAT
                Server_Demo.Instance.DmgPlayer(player.pid, 10.0f);
            }
        }


        if (gameObject.name == "bullet(Clone)")
        {
            BulletManager bulletManager = gameObject.GetComponent<BulletManager>();
            if (collision.gameObject.name == "Wall")
            {
                // Destroy Bullet
                Server_Demo.Instance.DestroyBullet(bulletManager.pid);
                DamagePopUp popup = Instantiate(Client_Demo.Instance.popUpReference).GetComponent<DamagePopUp>();
                popup.text = "Hit a Wall";
                popup.position = gameObject.transform.position;
            }
            else if (collision.gameObject.name == "MeleeEnemyAI(Clone)")
            {
                // Deal dmg to enemy
                EnemyAI enemy = collision.gameObject.GetComponent<EnemyAI>();
                Server_Demo.Instance.DmgEnemy(enemy.pid, 25.0f);
                if(enemy.hp <= 0)
                {
                    Server_Demo.Instance.AddScore(bulletManager.ownerID, 100.0f);
                }
                // Destroy Bullet
                Server_Demo.Instance.DestroyBullet(bulletManager.pid);
            }
            else if(collision.gameObject.name == "breakable")
            {
                // TODO: Destroy breakable
            }
        }

        if(collision.gameObject.tag == "PickUp")
        {
            PickupManager pickup = collision.gameObject.GetComponent<PickupManager>();
            if (gameObject.name == "Body")
            {
                PlayerManager player = gameObject.GetComponentInParent<PlayerManager>();
                Server_Demo.Instance.AddMoney(player.pid, 25.0f);
                // TODO: Destroy Pickups
                Server_Demo.Instance.DestroyHealthPickUp(player.pid, pickup.pid);
            }
        }

        if (gameObject.name == "Player(Clone)"|| gameObject.name == "Body")
        {
            if (collision.gameObject.name.Equals("Exit"))
            {
                Animation temp = collision.gameObject.GetComponent<Animation>();
                GameObject border = collision.gameObject.transform.Find("border").gameObject;
                if (!temp.isPlaying && (int)border.transform.position.y == 0 )
                    temp.Play("ExitAnimEnter");
            }
            
        }

    }

    private void OnCollisionStay(Collision collision)
    {
        if (gameObject.name == "Player(Clone)" || gameObject.name == "Body")
        {
            if (collision.gameObject.name.Equals("border"))
            {
                GameObject exit = collision.gameObject.transform.parent.gameObject;
                Animation temp = exit.GetComponent<Animation>();
                if (!temp.isPlaying && collision.gameObject.transform.position.y == 1)
                    temp.Play("ExitAnimLeave");
            }

        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (gameObject.name == "Player(Clone)" || gameObject.name == "Body")
        {
            if (collision.gameObject.name.Equals("border"))
            {
                GameObject exit = collision.gameObject.transform.parent.gameObject;
                Animation temp = exit.GetComponent<Animation>();
                if (!temp.isPlaying && collision.gameObject.transform.position.y == 1)
                {
                    temp.Play("ExitAnimLeave");
                }
            }

        }
    }

}
