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
            if (gameObject.name == "Player(Clone)")
            {
                PlayerManager player = gameObject.GetComponent<PlayerManager>();
                // Call server to deal weapon dmg to player
                // TODO: CHANGE THE DMG TO REFERENCE FROM WEAPONS DMG STAT
                Server_Demo.Instance.DmgPlayer(player.pid, 25.0f);
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
            else if (collision.gameObject.name == "MeleeEnemyAI")
            {
                PlayerManager player = gameObject.GetComponent<PlayerManager>();
                // Deal dmg to enemy
                EnemyAI enemy = collision.gameObject.GetComponent<EnemyAI>();
                Server_Demo.Instance.DmgEnemy(enemy.pid, 25.0f);
                if(enemy.hp <= 0)
                {
                    Server_Demo.Instance.AddScore(player.pid, 100.0f);
                }
                // Destroy Bullet
                Server_Demo.Instance.DestroyBullet(bulletManager.pid);
            }
            else if(collision.gameObject.name == "breakable")
            {
                // TODO: Destroy breakable
            }
        }

        if(collision.gameObject.tag.Equals("PickUp"))
        {
            PickupManager pickup = gameObject.GetComponent<PickupManager>();
            if (gameObject.name == "Player(Clone)")
            {
                PlayerManager player = gameObject.GetComponent<PlayerManager>();
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
                temp.Play("ExitAnimEnter");

                GameObject border = collision.gameObject.transform.Find("border").gameObject;
            }
        }

    }

}
