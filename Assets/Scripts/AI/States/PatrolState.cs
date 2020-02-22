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
    }
    public override void Update(double dt)
    {
        Vector3 movementDirection = (m_go.WayPoints[m_go.WaypointIndex].position - m_go.ePosition).normalized;
        m_go.ePosition += movementDirection * (float)dt;
        m_go.FaceTarget(m_go.WayPoints[m_go.WaypointIndex].position);

        if (m_go.hp < 0)
            m_go.sm.SetNextState("Dead");
        else
        {
            if(m_go.PlayerList.Count != 0)
            {
                foreach (playerObject player in m_go.PlayerList)
                {
                    Vector3 playerPos = new Vector3(player.m_x, player.m_y, player.m_z);
                    float DistanceSquared = (playerPos - m_go.ePosition).sqrMagnitude;
                    if (DistanceSquared < m_go.EngagementRangeSquared)
                    {
                        m_go.sm.SetNextState("Chase");
                        return;
                    }
                }
            }
 

            if(m_go.HasReachedWayPoint())
            {
                m_go.WaypointIndex = m_go.GetNextWayPointIndex();
            }


        }
    }
    public override void Exit()
    {
    }


}
