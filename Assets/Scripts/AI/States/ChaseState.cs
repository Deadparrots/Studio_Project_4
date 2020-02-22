using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateStuff;
using System;

public class ChaseState : State<EnemyAI>
{
    public EnemyAI m_go;
    public ChaseState(EnemyAI _gameObject, string stateID)
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
    }
    public override void Update(double dt)
    {
        if (m_go.hp < 0)
            m_go.sm.SetNextState("Dead");
        else
        {
            float shortestDistanceSquared = float.MinValue;
            uint closestPlayerID = 0;
            foreach (playerObject player in m_go.PlayerList)
            {
                Vector3 playerPos = new Vector3(player.m_x, player.m_y, player.m_z);
                float DistanceSquared = (playerPos - m_go.ePosition).sqrMagnitude;
                if (DistanceSquared < shortestDistanceSquared)
                {
                    shortestDistanceSquared = DistanceSquared;
                    closestPlayerID = player.id;
                }
            }
        }
    }
    public override void Exit()
    {
        throw new System.NotImplementedException();
    }


}
