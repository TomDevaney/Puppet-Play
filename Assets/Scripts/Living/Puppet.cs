using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// There are three puppets in the game: Count Duradel, Knight, and Princess
// All of their core logic are handled here. The only difference is what is controlling this script
// For Count, it'll be an evil AI player controller
// For Princess, it'll be a good AI player controller
// For Knight, it'll be your player controller

public class Puppet : Living
{

    public Rigidbody TheRigidBody;

    public bool IsStandingOnSurface = true;

    public float FootSpread = 0.1f;

    public float JumpForce = 300;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        TheRigidBody = GetComponent<Rigidbody>();

        // Initialize parent variables
        SetHealthPoints(1);
    }

    // Update is called once per frame
    void Update()
    {
        CheckStandingOnSurface();
    }

    public override void JustDied()
    {
        // Call parent function
        base.JustDied();

        // Tell the manager it's game over
        GameManager.instance.EndGame();
    }

    void CheckStandingOnSurface()
    {
        //Ignore the Player Layer
        int LayerMask = 1 << 9;
        LayerMask = ~LayerMask;

        float MaxRayDistance = 2.0f;
        RaycastHit HitInfo;
        Vector3 FootSpreadOffset = new Vector3(FootSpread, 0, 0);
        if (Physics.Raycast(transform.position, transform.up * -1, out HitInfo, MaxRayDistance, LayerMask)
            || Physics.Raycast(transform.position + FootSpreadOffset, transform.up * -1, out HitInfo, MaxRayDistance, LayerMask)
            || Physics.Raycast(transform.position - FootSpreadOffset, transform.up * -1, out HitInfo, MaxRayDistance, LayerMask)
            )
        {
            IsStandingOnSurface = true;
            //print(HitInfo.distance + "");
        }
        else
        {
            IsStandingOnSurface = false;
        }
        
        
    }

    public void Jump()
    {
        //print("Jumped");
        if (IsStandingOnSurface)
        {
            TheRigidBody.AddForce(0, JumpForce, 0);
        }
    }

    public void Attack()
    {
        if (true)
        {
            print("attack");

            //Ignore the Player Layer
            int LayerMask = 1 << 9 | 1 << 10;
            //int LayerMask = 0;
            LayerMask = ~LayerMask;


            float MaxRayDistance = 1.5f;
            print("transform.right : " + transform.right);
            RaycastHit HitInfo;
            if(Physics.Raycast(new Vector3(transform.position.x - 1, transform.position.y, transform.position.z),
                transform.right, out HitInfo, MaxRayDistance, LayerMask) 
                || Physics.Raycast(new Vector3(transform.position.x, transform.position.y, transform.position.z),
                transform.right, out HitInfo, MaxRayDistance, LayerMask)

                )
            {
                print("hit " + HitInfo.transform.tag);
                if(HitInfo.transform.tag.Equals("Enemy"))
                {
                    print("dodamage");
                    DoDamage(HitInfo.transform.GetComponent<Living>());

                }

            }
        }
    }

    public void MoveToLocation(float Loc)
    {
        StartCoroutine(MovingToLocation(Loc));
    }

    IEnumerator MovingToLocation(float Loc)
    {
        float axis = 1.0f;
        if(transform.position.x > Loc)
        {
            axis *= -1;
        }
        
        float range = 0.1f;

        while (true)
        {
            float distance = transform.position.x - Loc;
            if (axis > 0 && (Mathf.Abs(distance) < range || distance > 0))
            {
                break;
            }
            if(axis < 0 && (Mathf.Abs(distance) < range || distance < 0))
            {
                break;
            }


            Move(axis);
            yield return new WaitForSeconds(.01f);
        }
        EventManager.instance.MarkEventAsDone();
    }

    public override void Respawn()
    {
        // Respawn player at recent checkpoint
        SetSpawnPoint(GameManager.instance.GetRecentCheckpoint().GetLocation());
        base.Respawn();

        // Reinitialize parent variables
        SetHealthPoints(1);
    }
}
