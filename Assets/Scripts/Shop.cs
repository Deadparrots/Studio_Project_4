using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Shop : MonoBehaviour
{
    public Transform Shopin, Shopout, RevivalItem, Shopbg;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShopEnter()
    {
        Shopbg.DOMoveX(1280, 1);
    }

    public void ShopExit()
    {
        Shopbg.DOMoveX(-1280, 1);
    }

    public void Revivalitem()
    {
        PlayerManager player = gameObject.GetComponent<PlayerManager>();
        if (player.previve == false && player.Pmoney >= 1000)
        {
            Server_Demo.Instance.Revive(player.pid, true);
        }
    }
}
