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

    public float jumpVelocity = 7.5f;

	public float fallMultiplier = 2.5f;

	public float smallJumpMultiplier = 2.0f;

	// For ai that jump, but need to jump big
	bool bDoBigJump = false;

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
		SetAttackDamage(1);
	}

    // Update is called once per frame
    public override void Update()
    {
		base.Update();

		// Have a weightier fall
		if (TheRigidBody.velocity.y < 0)
		{
			TheRigidBody.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1.0f) * Time.deltaTime;
		}
		// Have a little jump if you tapped it
		else if (TheRigidBody.velocity.y > 0 && !Input.GetButton("Jump") && !bDoBigJump)
		{
			TheRigidBody.velocity += Vector3.up * Physics.gravity.y * (smallJumpMultiplier - 1.0f) * Time.deltaTime;
		}

		CheckStandingOnSurface();
    }

	public override void JustDied()
	{
		base.JustDied();

		InputManager.instance.DisablePlayerActions();
	}

	public override void MarkAsDead()
	{
		base.MarkAsDead();

		EventManager.instance.CloseCurtains("");

		// Tell the manager it's game over
		// TODO: Do I want to invoke this to after curtain?
		Invoke("TellGameOver", 6.0f);
	}

	// Used to delay game over until curtains are done closing
	public void TellGameOver()
	{
		GameManager.instance.GameOver();
	}

	void CheckStandingOnSurface()
    {
        //Ignore the Player and daughter Layer
        int LayerMask = 1 << 9 | 1 << 11;
        LayerMask = ~LayerMask;

        RaycastHit HitInfo;
        Vector3 FootSpreadOffset = new Vector3(FootSpread, 0, 0);
        if (Physics.Raycast(transform.position, transform.up * -1, out HitInfo, checkStandingRayDistance, LayerMask)
            || Physics.Raycast(transform.position + FootSpreadOffset, transform.up * -1, out HitInfo, checkStandingRayDistance, LayerMask)
            || Physics.Raycast(transform.position - FootSpreadOffset, transform.up * -1, out HitInfo, checkStandingRayDistance, LayerMask)
            )
        {
            // Only play landing sound if they were just in the air
            if (!animator.GetBool("OnGround"))
            {
                AudioManager.instance.PlaySoundFXAtPosition(landClip, gameObject.transform.position);
            }

            SetStandingOnSurface(true);

			animator.SetBool("OnGround", true);
        }
        else
        {
            SetStandingOnSurface(false);
			animator.SetBool("OnGround", false);
		}


	}

    public void Jump()
    {
        if (IsStandingOnSurface())
        {
			TheRigidBody.velocity = Vector2.up * jumpVelocity;

            // Play jump sound
            AudioManager.instance.PlaySoundFXAtPosition(jumpClip, gameObject.transform.position);

            animator.SetTrigger("Jump");
		}
    }

    public void Attack()
    {
        meleeWeapon.AttackModeActive = true;
        Invoke("DisableWeaponAttackMode",1);
        animator.SetTrigger("Attack");

        // Play sword swinging sound
        AudioManager.instance.PlaySoundFXAtPosition(attackClip, gameObject.transform.position);
    }

    public void DisableWeaponAttackMode()
    {
        meleeWeapon.AttackModeActive = false;
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

		// Move to location is separate from Move which handles the animator trigger stuff
		// So reset to idle here
		animator.SetTrigger("Idle");
		animator.ResetTrigger("Walk");

		EventManager.instance.MarkEventAsDone();
    }

	public void SetDoBigJump(bool bigJump)
	{
		bDoBigJump = bigJump;
	}

    public override void Respawn()
    {
        // Respawn player at recent checkpoint
        SetSpawnPoint(GameManager.instance.GetRecentCheckpoint().GetLocation());
        base.Respawn();

        // Reinitialize parent variables
        SetHealthPoints(1);

		EventManager.instance.OpenCurtains("");

		Invoke("OnRespawnDone", 6.0f);
    }

	// Used to delay player respawn until curtains are done opening
	public void OnRespawnDone()
	{
		InputManager.instance.EnablePlayerActions();
	}
}
