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
            if(m_go.PlayerList.Count > 0)
            {
                float shortestDistanceSquared = float.MaxValue;
                playerObject closestPlayer;
                Vector3 playerPos = new Vector3(0, 0, 0);
                foreach (playerObject player in m_go.PlayerList)
                {
                    playerPos.Set(player.m_x, player.m_y, player.m_z);
                    float DistanceSquared = (playerPos - m_go.ePosition).sqrMagnitude;
                    if (DistanceSquared < shortestDistanceSquared)
                    {
                        shortestDistanceSquared = DistanceSquared;
                        closestPlayer = player;
                    }
                }

                Vector3 movementDirection = (playerPos - m_go.ePosition).normalized;
                movementDirection.y = 0;
                m_go.ePosition += movementDirection * (float)dt;
                m_go.FaceTarget(playerPos);

                if (shortestDistanceSquared > m_go.EngagementRangeSquared)
                {
                    m_go.sm.SetNextState("Patrol");
                    m_go.WaypointIndex = m_go.GetNearestWaypointIndex();
                }
                else if (shortestDistanceSquared < m_go.AttackRangeSquared)
                {
                    m_go.sm.SetNextState("Attack");
                }
            }
            else
            {
                m_go.sm.SetNextState("Patrol");
                m_go.WaypointIndex = m_go.GetNearestWaypointIndex();
            }
 
        }
    }
    public override void Exit()
    {
    }


}
