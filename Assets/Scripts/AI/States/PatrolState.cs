using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateStuff;
using System;

public class PatrolState : State<EnemyAI>
{
    public EnemyAI m_go;
    public PatrolState(EnemyAI _gameObject, string stateID) 
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
        throw new System.NotImplementedException();

    }
    public override void Update(double dt)
    {
        throw new System.NotImplementedException();
        //m_go.sm.SetNextState("")
    }
    public override void Exit()
    {
        throw new System.NotImplementedException();
    }


}
