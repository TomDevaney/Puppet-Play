﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// There are three puppets in the game: Count Duradel, Knight, and Princess
// All of their core logic are handled here. The only difference is what is controlling this script
// For Count, it'll be an evil AI player controller
// For Princess, it'll be a good AI player controller
// For Knight, it'll be your player controller

public class Puppet : Living
{

    Rigidbody TheRigidBody;

    public float FootSpread = 0.1f;

    public float jumpVelocity = 7.5f;
	public float fallMultiplier = 2.5f;
	public float smallJumpMultiplier = 2.0f;

	public float disableAttackModeTimer;

	// For ai that jump, but need to jump big
	bool doBigJump = false;

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

		// Set as standing to prevent landing sound
		SetStandingOnSurface(true);
		animator.SetBool("OnGround", true);

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
		else if (TheRigidBody.velocity.y > 0 && !Input.GetButton("Jump") && !doBigJump)
		{
			TheRigidBody.velocity += Vector3.up * Physics.gravity.y * (smallJumpMultiplier - 1.0f) * Time.deltaTime;
		}
    }


	public override void Move(float xAxis)
	{
		// Don't allow player to move while attacking
		//if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") && !animator.GetNextAnimatorStateInfo(0).IsName("Attack"))
		{
			base.Move(xAxis);
		}
	}

	public override void JustDied()
	{
		base.JustDied();

		// Disable attack so that if they swung and died, the enemy can't be killed
		DisableWeaponAttackMode();

		// Undo Living.JustDied call to Invoke this function
		CancelInvoke("MarkAsDead");

		// Instead, do it after curtains are done moving
		GameManager.instance.GetStageController().OnCurtainsDoneMoving += MarkAsDead;

		// Disable collision between enemies and player while death animation happens
		Physics.IgnoreLayerCollision(9, 12, true);

		EventManager.instance.CloseCurtains("");

		InputManager.instance.DisablePlayerActions();
	}

	public override void MarkAsDead()
	{
		base.MarkAsDead();

		GameManager.instance.GameOver();
	}

	public void Jump()
    {
        if (IsStandingOnSurface())
        {
			TheRigidBody.velocity = Vector2.up * jumpVelocity;

            animator.SetTrigger("Jump");

			// Play jump sound
			AudioManager.instance.PlaySoundFXAtPosition(jumpClip, gameObject.transform.position);
		}
	}

    public void Attack()
    {
		// Don't allow player to attack if they are currently doing attack animation in any form
		if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") && !animator.GetNextAnimatorStateInfo(0).IsName("Attack"))
		{
			meleeWeapon.AttackModeActive = true;
			Invoke("DisableWeaponAttackMode", disableAttackModeTimer);
			animator.SetTrigger("Attack");

			// Play sword swinging sound
			AudioManager.instance.PlaySoundFXAtPosition(attackClip, gameObject.transform.position);
		}
    }

    public void DisableWeaponAttackMode()
    {
        meleeWeapon.AttackModeActive = false;
    }

	// I switched from ray trace to collision for figuring out when the player could jump again
	// The only problem is that the environment isn't just the floor, so they could collide with walls and be able to jump again
	// I still think the collision benefits outweigh ray trace though
	public void OnCollisionStay(Collision collision)
	{
		if (collision.gameObject.layer == LayerMask.NameToLayer("Environment"))
		{
			ContactPoint[] contacts = new ContactPoint[collision.contactCount];
			collision.GetContacts(contacts);

			// Check if it's the floor
			bool onFloor = false;

			for (int i = 0; i < contacts.Length; ++i)
			{
				// Allow for some deviance from a completly flat floor
				if (contacts[i].normal.y >= 0.5f && contacts[i].normal.y <= 1.0f)
				{
					onFloor = true;
					break;
				}
			}

			if (onFloor)
			{
				if (!IsStandingOnSurface())
				{
					AudioManager.instance.PlaySoundFXAtPosition(landClip, gameObject.transform.position);
				}

				SetStandingOnSurface(true);

				animator.SetBool("OnGround", true);
			}
		}
	}

	public void OnCollisionExit(Collision collision)
	{
		if (collision.gameObject.layer == LayerMask.NameToLayer("Environment"))
		{
			SetStandingOnSurface(false);
			animator.SetBool("OnGround", false);
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

		// Move to location is separate from Move which handles the animator trigger stuff
		// So reset to idle here
		animator.SetTrigger("Idle");
		animator.ResetTrigger("Walk");

		EventManager.instance.MarkEventAsDone();
    }

	public void SetDoBigJump(bool bigJump)
	{
		doBigJump = bigJump;
	}

	// Don't do anything as puppet has a special footsteps script to handle this
	public override void PlayFootstepSound()
	{
	}

	public override void Respawn()
    {
        // Respawn player at recent checkpoint
        SetSpawnPoint(GameManager.instance.GetRecentCheckpoint().GetLocation());
        base.Respawn();

        // Reinitialize parent variables
        SetHealthPoints(1);

		// Enable collision between enemies and player now that death animation is over
		Physics.IgnoreLayerCollision(9, 12, false);

		// Set camera position at player's position to prevent camera lerping to player position
		Vector3 camPosition = GameManager.instance.GetCameraFSM().transform.position;
		camPosition.x = transform.position.x;

		GameManager.instance.GetCameraFSM().transform.position = camPosition;

		// Set as standing to prevent landing sound
		SetStandingOnSurface(true);
		animator.SetBool("OnGround", true);

		// Open curtains after delay because opening curtains right after they just got closed looks funny
		Invoke("OpenCurtains", 1.5f);
	}

	// Encapsulated into its own function for delay purposes
	void OpenCurtains()
	{
		EventManager.instance.OpenCurtains("");

		// Hook up delegate to our function
		GameManager.instance.GetStageController().OnCurtainsDoneMoving += OnRespawnDone;
	}

	// Used to delay player respawn until curtains are done opening
	public void OnRespawnDone()
	{
		GameManager.instance.GetStageController().OnCurtainsDoneMoving -= OnRespawnDone;

		InputManager.instance.EnablePlayerActions();
	}
}
