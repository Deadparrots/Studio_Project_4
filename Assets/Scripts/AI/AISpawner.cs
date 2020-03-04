using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISpawner : MonoBehaviour
{
    // Start is called before the first frame update
    public float spawnCooldown = 10;
    public float timer = 10;

    public bool inGame = false;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(inGame)
        {
            if (timer < 0)
            {
                // Ask server call spawn AI
                if (Server_Demo.Instance)
                {
                    Server_Demo.Instance.SpawnEnemy();
                    timer = spawnCooldown;
                }
            }
            else
            {
                timer -= Time.deltaTime;
            }
        }
    }
}
