using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateStuff;
using System;

public class DeadState : State<EnemyAI>
{
    public EnemyAI m_go;
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
        // Send Message to destroy Enemy in clients
        Server_Demo.Instance.DestroyEnemy(m_go.pid);
    }
    public override void Update(double dt)
    {
    }
    public override void Exit()
    {
    }


}
