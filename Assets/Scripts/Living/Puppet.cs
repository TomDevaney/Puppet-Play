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

    public float FootSpread = 0.1f;

    public float JumpForce = 300;

	// Different puppets have different origins so need to customize this
	public float checkStandingRayDistance;

    public MeleeWeapon meleeWeapon;

    [SerializeField]
    AudioClip jumpClip;

    [SerializeField]
    AudioClip landClip;

    [SerializeField]
    AudioClip attackClip;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        TheRigidBody = GetComponent<Rigidbody>();

        // Initialize parent variables
        SetHealthPoints(1);
    }

    // Update is called once per frame
    public override void Update()
    {
		base.Update();

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

        RaycastHit HitInfo;
        Vector3 FootSpreadOffset = new Vector3(FootSpread, 0, 0);
        if (Physics.Raycast(transform.position, transform.up * -1, out HitInfo, checkStandingRayDistance, LayerMask)
            || Physics.Raycast(transform.position + FootSpreadOffset, transform.up * -1, out HitInfo, checkStandingRayDistance, LayerMask)
            || Physics.Raycast(transform.position - FootSpreadOffset, transform.up * -1, out HitInfo, checkStandingRayDistance, LayerMask)
            )
        {
            // Only play landing sound if they were just in the air
            if (!IsStandingOnSurface())
            {
                AudioManager.instance.PlaySoundFXAtPosition(landClip, gameObject.transform.position);
                animator.SetTrigger("LandedOnSurface");
            }

            SetStandingOnSurface(true);
            //print(HitInfo.distance + "");
        }
        else
        {
            if(IsStandingOnSurface())
            {
                animator.ResetTrigger("LandedOnSurface");
            }

            SetStandingOnSurface(false);
        }


    }

    public void Jump()
    {
        //print("Jumped");
        if (IsStandingOnSurface())
        {
            TheRigidBody.AddForce(0, JumpForce, 0);

            // Play jump sound
            AudioManager.instance.PlaySoundFXAtPosition(jumpClip, gameObject.transform.position);

            animator.SetTrigger("Jump");
        }
        else
        {
            print("not standing on surface");
        }
    }

    public void Attack()
    {

        //standing
        if (true)
        {
            meleeWeapon.AttackModeActive = true;
            Invoke("DisableWeaponAttackMode",1);
            animator.SetTrigger("Attack");

            //Ignore the Player Layer
            int LayerMask = 1 << 9 | 1 << 10;
            //int LayerMask = 0;
            LayerMask = ~LayerMask;

            // Play sword swinging sound
            AudioManager.instance.PlaySoundFXAtPosition(attackClip, gameObject.transform.position);

            float MaxRayDistance = 1.5f;
            //print("transform.right : " + transform.right);
            RaycastHit HitInfo;
            if(Physics.Raycast(new Vector3(transform.position.x - 1, transform.position.y, transform.position.z),
                transform.right, out HitInfo, MaxRayDistance, LayerMask) 
                || Physics.Raycast(new Vector3(transform.position.x, transform.position.y, transform.position.z),
                transform.right, out HitInfo, MaxRayDistance, LayerMask)

                )
            {
                //print("hit " + HitInfo.transform.tag);
                if(HitInfo.transform.tag.Equals("Enemy"))
                {
                    //print("dodamage");
                    //DoDamage(HitInfo.transform.GetComponent<Living>());

                }

            }
        }
    }

    public void DisableWeaponAttackMode()
    {
        meleeWeapon.AttackModeActive = false;
        print("DWAM");
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
