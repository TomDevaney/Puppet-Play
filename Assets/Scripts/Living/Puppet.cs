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

    Rigidbody TheRigidBody;

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
		else if (TheRigidBody.velocity.y > 0 && !Input.GetButton("Jump") && !bDoBigJump)
		{
			TheRigidBody.velocity += Vector3.up * Physics.gravity.y * (smallJumpMultiplier - 1.0f) * Time.deltaTime;
		}
    }

	public override void JustDied()
	{
		base.JustDied();

		// Disable collision between enemies and player while death animation happens
		Physics.IgnoreLayerCollision(9, 12, true);

		EventManager.instance.CloseCurtains("");

		InputManager.instance.DisablePlayerActions();
	}

	public override void MarkAsDead()
	{
		base.MarkAsDead();

		// Tell the manager it's game over after curtains are done closing
		GameManager.instance.GetStageController().OnCurtainsDoneMoving += TellGameOver;
	}

	// Used to delay game over until curtains are done closing
	public void TellGameOver()
	{
		GameManager.instance.GetStageController().OnCurtainsDoneMoving -= TellGameOver;

		GameManager.instance.GameOver();
	}

    public void Jump()
    {
        if (IsStandingOnSurface())
        {
			TheRigidBody.velocity = Vector2.up * jumpVelocity;

            // Play jump sound
            AudioManager.instance.PlaySoundFXAtPosition(jumpClip, gameObject.transform.position);

            animator.SetTrigger("Jump");
			animator.SetBool("OnGround", false);

			SetStandingOnSurface(false);
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

	public void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.layer == LayerMask.NameToLayer("Environment") && !IsStandingOnSurface())
		{
			SetStandingOnSurface(true);

			AudioManager.instance.PlaySoundFXAtPosition(landClip, gameObject.transform.position);

			animator.SetBool("OnGround", true);
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
		bDoBigJump = bigJump;
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
