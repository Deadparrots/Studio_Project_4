using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISpawner : MonoBehaviour
{
    // Start is called before the first frame update
    public float spawnCooldown = 10;
    public float timer = 10;



    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(timer < spawnCooldown)
        {
            // spawn ai
            //EnemyAI newAI = new EnemyAI();
            //newAI.gameObject.transform.position = gameObject.transform.position;
            //timer = spawnCooldown;

            // Ask server call spawn AI
        }
        else
        {
            timer -= Time.deltaTime;
        }
    }
}
