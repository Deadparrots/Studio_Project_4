using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateStuff;
using UnityEngine.AI;
using UnityEngine.UI;
public class EnemyAI : MonoBehaviour
{
    private List<playerObject> playerList = new List<playerObject>();

    private uint id;
    private bool isController;
    private float healthPoints = 150;
    private float damage = 25;
    private float movementSpeed = 10000;
    public NavMeshAgent agent;
    public Image healthBar;
    public StateMachine<EnemyAI> sm {get;set;}
    // Start is called before the first frame update

    // might make an enum for enemy types if having multiple types of enemies

    // For Patrol
    private List<Transform> wayPoints = new List<Transform>();
    private Transform targetWaypoint;
    private int currentWaypointIndex;
    private float m_fDistanceTolerance = 1.0f;
    private float engagementRangeSquared = 100.0f;
    private float attackRangeSquared = 5.0f;
    private int lastWaypointIndex;


    // Animations
    Animator anim;
    private void Awake()
    {
        Debug.Log("Enemy starting position: " + gameObject.transform.position);
        //rotation.y -= 180;
    }
    void Start()
    {

        sm = new StateMachine<EnemyAI>(this);
        sm.AddState(new PatrolState(this, "Patrol"));
        sm.AddState(new ChaseState(this, "Chase"));
        sm.AddState(new AttackState(this, "Attack"));
        sm.AddState(new DeadState(this, "Dead"));
        sm.SetNextState("Patrol");
        isController = false;
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = true;
        agent.updatePosition = true;
    }

    public uint pid
    {
        get { return id; }
        set { id = value; }
    }

    public int WaypointIndex
    {
        get { return currentWaypointIndex; }
        set { currentWaypointIndex = value; }
    }

    public float hp
    {
        get { return healthPoints; }
        set { healthPoints = value; }
    }

    public float dmg
    {
        get { return damage; }
        set { damage = value; }
    }

    public float MovementSpeed
    {
        get { return movementSpeed; }
        set { movementSpeed = value; }
    }

    public Vector3 rotation
    {
        get { return gameObject.transform.eulerAngles; }
        set { gameObject.transform.eulerAngles = value; }
    }
    public Vector3 position
    {
        get { return gameObject.transform.position; }
        set { gameObject.transform.position = value; }
    }

    public float EngagementRangeSquared
    {
        get { return engagementRangeSquared; }
        set { engagementRangeSquared = value; }
    }

    public float AttackRangeSquared
    {
        get { return attackRangeSquared; }
        set { attackRangeSquared = value; }
    }


    public bool controller
    {
        get { return isController; }
        set { isController = value; }
    }

    // Update is called once per frame
    void Update()
    {
        // so that client doesnt update statemachine
        // statemachine should only be updated by server for consitency
        // isController is to identify whether this AI is in server or in client
        if (isController)
        {
            sm.Update(Time.deltaTime);
            Server_Demo.Instance.UpdateEnemyInClient(id, position, rotation,sm.GetCurrentState(),healthPoints);
        }

        //gameObject.transform.eulerAngles = rotation;
        //rotation = gameObject.transform.rotation.eulerAngles;

        if (sm.GetCurrentState() == "Patrol" || sm.GetCurrentState() == "Chase")
        {
            anim.SetBool("isWalking", true);
            anim.SetBool("isAttacking", false);
            anim.SetBool("isDead", false);
        }
        else if (sm.GetCurrentState() == "Attack")
        {
            anim.SetBool("isWalking", false);
            anim.SetBool("isAttacking", true);
            anim.SetBool("isDead", false);
        }
        else if(sm.GetCurrentState() == "Dead")
        {
            anim.SetBool("isWalking", false);
            anim.SetBool("isAttacking", false);
            anim.SetBool("isDead", true);
        }
        healthBar.fillAmount = healthPoints * 0.01f;
    }

    public List<playerObject> PlayerList
    {
        get { return playerList; }
        set { playerList = value; }
    }

    public List<Transform> WayPoints
    {
        get { return wayPoints; }
        set { wayPoints = value; }
    }

    public int GetNextWayPointIndex()
    {
        int temp = currentWaypointIndex + 1;

        if(temp > wayPoints.Count - 1)
        {
            temp = 0;
        }

        return temp;
    }

    public bool HasReachedWayPoint()
    {
        // Check if there is at least 1 WayPoint
        if (wayPoints.Count > 0)
        {
            // Get the position of current WayPoint
            Vector3 currentWaypointPosition = wayPoints[currentWaypointIndex].position;

            // Calculate the x- and z- component distance
            float xDistance = position.x - currentWaypointPosition.x;
            float zDistance = position.z - currentWaypointPosition.z;
            // Calculate the distance squared value. We avoid square root as it is expensive
            float distanceSquared = (float)(xDistance * xDistance + zDistance * zDistance);
            // if the distance between aPosition and the current WayPoint is within m_fDistanceTolerance value
            if (distanceSquared < m_fDistanceTolerance)
            {
                return true;
            }
        }
        return false;
    }


    public int GetNearestWaypointIndex()
    {
        int theNearestWaypoint = -1;
        float m_fDistance = float.MaxValue;

        // If Waypoints has related Waypoints, then we proceed to search.
        if (wayPoints.Count != 0)
        {
            int index = -1;
            // Iterate through all the Waypoints to find the nearest WayPoint
            foreach(Transform waypoint in wayPoints)
            {
                ++index;
                Vector3 aRelatedWaypoint = waypoint.position;

                float xDistance = position.x - aRelatedWaypoint.x;
                float zDistance = position.z - aRelatedWaypoint.z;
                float distanceSquared = (float)(xDistance * xDistance + zDistance * zDistance);
                if (m_fDistance > distanceSquared)
                {
                    // Update the m_fDistance to this lower distance
                    m_fDistance = distanceSquared;
                    // Set this WayPoint as the nearest WayPoint
                    theNearestWaypoint = index;
                }
            }
        }

        return theNearestWaypoint;
    }

    public void FaceTarget(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        //rotation.y += (lookRotation.eulerAngles.y * 5.0f * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5.0f);
    }
}
