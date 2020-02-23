using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateStuff;
using System;

public class DeadState : State<EnemyAI>
{
    public EnemyAI m_go;
    float deathTimer = 10.0f;
    float timer = 0;
    public DeadState(EnemyAI _gameObject, string stateID)
        : base(State<EnemyAI>(stateID))
    {
        m_go = _gameObject;
    }

    private static string State<T>(string stateID)
    {
        return stateID;
    }

    public override void Enter()
    {
        timer = deathTimer;
        Server_Demo.Instance.spawnPickup(m_go.ePosition);
    }
    public override void Update(double dt)
    {
        if (timer < 0.0f)
        {
            // Send Message to destroy Enemy in clients
            Server_Demo.Instance.DestroyEnemy(m_go.pid);

        }
        else
            timer -= Time.deltaTime;
    }
    public override void Exit()
    {
    }


}
