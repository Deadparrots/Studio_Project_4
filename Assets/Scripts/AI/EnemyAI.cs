using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateStuff;
public class EnemyAI : MonoBehaviour
{
    private List<playerObject>
    public StateMachine<EnemyAI> sm {get;set;}
    // Start is called before the first frame update
    void Start()
    {
        sm = new StateMachine<EnemyAI>(this);
        sm.AddState(new PatrolState(this, "Patrol"));
    }

    // Update is called once per frame
    void Update()
    {
    }
}
