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

            EnemyAI enemy = collision.gameObject.GetComponentInParent<EnemyAI>();

            if (enemy.hp > 0)
            {
                if (gameObject.name == "Body")
                {
                    PlayerManager player = gameObject.GetComponentInParent<PlayerManager>();
                    // Call server to deal weapon dmg to player
                    // TODO: CHANGE THE DMG TO REFERENCE FROM WEAPONS DMG STAT
                    Server_Demo.Instance.DmgPlayer(player.pid, 10.0f);
                }
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

                DamagePopUp popUp = Instantiate(Client_Demo.Instance.popUpReference).GetComponent<DamagePopUp>();
                popUp.text = "25";
                popUp.position = gameObject.transform.position;
                popUp.color = Color.red;

                if(enemy.hp <= 0)
                {
                    Server_Demo.Instance.AddScore(bulletManager.ownerID, 100.0f);
                }
                // Destroy Bullet
                Server_Demo.Instance.DestroyBullet(bulletManager.pid);
            }
            else if(collision.gameObject.name == "breakable(Clone)")
            {
                // TODO: Destroy breakable
                Destroy(collision.gameObject);
                Server_Demo.Instance.DestroyBullet(bulletManager.pid);
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
                Globals.countDown = 0;
                Globals.countDownlastsecond = 0;
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

            if (collision.gameObject.name == "Exit") // since inside shld only run once per frame
            {
                Globals.countDown += Time.deltaTime;
                
                if (Globals.countDown - 1 > Globals.countDownlastsecond)
                {

                    DamagePopUp popUp = Instantiate(Client_Demo.Instance.popUpReference).GetComponent<DamagePopUp>();
                    popUp.text = ((int)(Globals.ExitTimer - Globals.countDown)).ToString();
                    popUp.position = gameObject.transform.position;
                    popUp.color = Color.red;
                    popUp.lifetime = 1f;
                    Globals.countDownlastsecond = Globals.countDown;
                }

                if (Globals.countDown > Globals.ExitTimer)
                {
                    Client_Demo.Instance.GetSceneManager().ToEndScreen();
                }
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
