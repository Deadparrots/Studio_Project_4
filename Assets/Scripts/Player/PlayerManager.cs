using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private uint id;
    public GameObject childPlayer;
    public TextMesh childName;
    public TextMesh scoreText;
    private PlayerMovement childScript;
    private float health;
    public bool isShooting = false;
    public bool isMoving = false;
    public Animator animator;
    public uint pid
    {
        get { return id; }
        set { id = value; }
    }

    public float hp
    {
        get { return health; }
        set { health = value; }
    }

    public Vector3 pRotation
    {
        get { return childScript.pRotation; }
        set { childScript.pRotation = value; }
    }

    public Vector3 velocity
    {
        get { return childScript.pVelocity; }
        set { childScript.pVelocity = value; }
    }

    public Vector3 position
    {
        get { return childPlayer.transform.position; }
        set { childPlayer.transform.position = value; }
    }

    public Vector3 GetDefaultScale()
    {
        return childScript.GetDefScale();
    }

    public void SetPlayer(bool _boolean)
    {
        Debug.Log("player set!!");

        //childScript.isPlayer = _boolean;
    }
    public string pName
    {
        get { return childName.text; }
        set
        {
            childName.text = value;
            GetComponentInChildren<TextMesh>().text = value;
        }
    }

    public Vector3 server_pos
    {
        get { return childScript.server_pos; }
        set { childScript.server_pos = value; }
    }

    public Vector3 serverVelocity
    {
        get { return childScript.server_Velocity; }
        set { childScript.server_Velocity = value; }
    }

    public Vector3 serverRotation
    {
        get { return childScript.serverRotation; }
        set { childScript.serverRotation = value; }
    }

    void DoInterpolateUpdate()
    {
        childScript.client_pos = new Vector3(childPlayer.transform.position.x, childPlayer.transform.position.y, childPlayer.transform.position.z);
        childScript.clientRotation = pRotation;
        velocity = childScript.server_Velocity;
        childScript.ratio = 0;

    }
    protected void Awake()
    {
        childScript = GetComponentInChildren<PlayerMovement>();
        animator = GetComponentInChildren<Animator>();
        health = 100.0f;
    }

    private void FixedUpdate()
    {


        //if(isShooting)
        //    animator.SetBool("isShooting", true);
        //else
        //    animator.SetBool("isShooting", false);
    }
    private void Update()
    {
        if (isMoving)
            animator.SetBool("isMoving", true);
        else
            animator.SetBool("isMoving", false);
    }
}
