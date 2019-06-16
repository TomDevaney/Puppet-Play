using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Direction
{
	LEFT,
	RIGHT,
};

public class Living : MonoBehaviour
{
    // Their health
    int healthPoints;

    // Movement speed
    public float moveSpeed = 0.01f;

    // How much damage it will deal to the puppet
    int attackDamage;

    // hp is 0
    bool isDead;

    // Whether the living is in air or not
    bool standingOnSurface = true;

    // Where the living will spawn if they respawn after dying
    Vector3 spawnPoint;

	// Position the living was first spawned at
	Vector3 initialPosition;

    // Only to be used for footsteps
    // Make sure it's the second audiosource in the object
    AudioSource footstepsAudioSource = null;

    // Clips of all the sounds needed for a living being
    [SerializeField]
    AudioClip deathSound = null;

    [SerializeField]
    AudioClip footstepsSound = null;

    [SerializeField]
    AudioClip gotHitSound = null;

	[SerializeField]
	AudioClip hitSound = null;

	[SerializeField]
	AudioClip idleSound = null;

	// Idle sound variables
	[SerializeField]
	float minSecondsTilIdleSound = 2.0f;

	[SerializeField]
	float maxSecondsTilIdleSound = 4.0f;

	// Set this to a random time between min and max
	float desiredSecondsTilIdleSound = 0.0f;

	// Counts how much time has passed since last idle sound
	// Will play an idle sound once this timer == desiredSoundTimer
	float idleSoundTimer = 0.0f;

	[HideInInspector]
	public Animator animator;

	// Allows the living to not take damage
	public bool isInvincible;

	/* Facing variables */
	Direction facingDirection = Direction.RIGHT;

	[HideInInspector]
	public bool doFacing = true;

	const float LEFT_DEGREE = 0.0f;
	const float RIGHT_DEGREE = -180.0f;
	const float FACING_SPEED = 720.0f;
	float currentFacingDegree = RIGHT_DEGREE;
	float desiredFacingDegree = RIGHT_DEGREE;

    // Start is called before the first frame update
    public virtual void Start()
    {
		// Initialize variables
		initialPosition = transform.position;
		spawnPoint = initialPosition;

		// Get references
		footstepsAudioSource = GetComponents<AudioSource>()[0];
		animator = GetComponentInChildren<Animator>();

		// Set clip to footsteps source because that's the only sound it'll play
		footstepsAudioSource.clip = footstepsSound;

		// Set up desired idle time
		desiredSecondsTilIdleSound = Random.Range(minSecondsTilIdleSound, maxSecondsTilIdleSound);
    }

    // Update is called once per frame
    public virtual void Update()
    {
		if (doFacing)
		{
			// Do lerp at a constant rate towards desired degree
			currentFacingDegree = Mathf.MoveTowards(currentFacingDegree, desiredFacingDegree, Time.deltaTime * FACING_SPEED);

			transform.rotation = Quaternion.Euler(transform.rotation.x, currentFacingDegree, transform.rotation.z);
		}

		// Check if should play idle sound
		idleSoundTimer += Time.deltaTime;

		if (idleSoundTimer >= desiredSecondsTilIdleSound)
		{
			// Play idle sound
			AudioManager.instance.Play3DSoundFX(idleSound, transform.position);

			// Reset timer
			idleSoundTimer = 0.0f;

			// Set up new desired idle time
			desiredSecondsTilIdleSound = Random.Range(minSecondsTilIdleSound, maxSecondsTilIdleSound);
		}
	}

    virtual public void Move(float xAxis)
    {
		Direction prevFacingDirection = facingDirection;

		// Determine new facing direction
		if (xAxis > 0.0f)
		{
			// Face right
			facingDirection = Direction.RIGHT;
		}
		else if (xAxis < 0.0f)
		{
			// Face left
			facingDirection = Direction.LEFT;
		}

		// Check to see if the facing direction needs to be played
		// If so, stop the previous one if that one is still playing
		if (prevFacingDirection != facingDirection)
		{
			switch (facingDirection)
			{
				case Direction.LEFT:
					desiredFacingDegree = LEFT_DEGREE;
					break;
				case Direction.RIGHT:
					desiredFacingDegree = RIGHT_DEGREE;
					break;
			}
		}

		// Move the player
        transform.Translate(new Vector2(xAxis * moveSpeed * Time.deltaTime, 0.0f), Space.World);

		// Only play footsteps if they're actually moving and it's not playing already
		const float walkingThreshold = 0.15f;

        if (Mathf.Abs(xAxis) >= walkingThreshold)
        {
			// Don't play if they're in the air!
			if (!footstepsAudioSource.isPlaying && standingOnSurface)
			{
				PlayFootstepSound();
			}

			if (animator != null)
			{
				animator.ResetTrigger("Idle");
				animator.SetTrigger("Walk");
			}
		}
		else
        {
			if (animator != null)
			{
				animator.SetTrigger("Idle");
				animator.ResetTrigger("Walk");
			}
        }
    }

    public void DoDamage(Living living)
    {
		// To prevent a bug where the enemy gets hit and the player gets hit later in the same frame, and they both die
		// In the future, if any projectile based enemies are killed and I want the projectile to still be alive, I will have to modify this
		if (!isDead)
		{
			if (hitSound != null)
			{
				AudioManager.instance.PlaySoundFXAtPosition(hitSound, transform.position);
			}

			living.TakeDamage(attackDamage);
		}
    }

    public void TakeDamage(int damage)
    {
		if (isInvincible)
			return;

        // Apply damage
        healthPoints -= damage;

        // Play got hit sound
        if (gotHitSound != null)
        {
            AudioManager.instance.PlaySoundFXAtPosition(gotHitSound, transform.position);
        }

        // Check if dead
        if (healthPoints <= 0)
        {
            JustDied();
        }
    }

    // Override in the child and react to the death
    public virtual void JustDied()
    {
		// Do an animation before activating death
		if (animator != null)
		{
			animator.SetBool("Dead", true);
		}

		// Figure out when to actually mark as dead
		float deathAnimationTime = 0.0f;

		if (animator != null)
		{
			AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;

			foreach (AnimationClip clip in clips)
			{
				if (clip.name == "Death")
				{
					deathAnimationTime = clip.length;
				}
			}
		}

		// Play death sound
		if (deathSound != null)
        {
            AudioManager.instance.PlaySoundFXAtPosition(deathSound, transform.position);
        }

		// Do here because the living shouldn't be able to harm anyone when their death animation is happening
		isDead = true;

		// Delay death until animation done
		Invoke("MarkAsDead", deathAnimationTime);
	}

	public virtual void MarkAsDead()
	{
		GameManager.instance.GetStageController().OnCurtainsDoneMoving -= MarkAsDead;

		// Disable so it can be respawned later if needed
		gameObject.SetActive(false);

		// For kill events, tell EventManager
		EventManager.instance.NotifyOfDeath(this);
	}

    public virtual void Respawn()
    {
		// Go back to idle
		if (animator != null)
		{
			animator.SetBool("Dead", false);
		}

		// Spawn the object at the spawn point
		transform.position = new Vector3(spawnPoint.x, spawnPoint.y, transform.position.z);

        // Reset variables
        gameObject.SetActive(true);
        isDead = false;
    }

	// Will place Living right where they were when game was loaded
	public void ResetToInitialPosition()
	{
		transform.position = initialPosition;
	}

	public virtual void PlayFootstepSound()
	{
		if (!footstepsAudioSource.isPlaying)
		{
			footstepsAudioSource.Play();
		}
	}

	/* Setters */
	public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }

    public void SetAttackDamage(int dam)
    {
        attackDamage = dam;
    }

    public void SetSpawnPoint(Vector3 point)
    {
        spawnPoint = point;
    }

    public void SetHealthPoints(int hp)
    {
        healthPoints = hp;
    }

    public void SetStandingOnSurface(bool standing)
    {
        standingOnSurface = standing;
    }

    /* Getters */
    public float GetMoveSpeed()
    {
        return moveSpeed;
    }

    public float GetAttackDamage()
    {
        return attackDamage;
    }

    public bool IsDead()
    {
        return isDead;
    }

    public bool IsStandingOnSurface()
    {
        return standingOnSurface;
    }

    public Vector3 GetSpawnPoint()
    {
        return spawnPoint;
    }

    public int GetHealthPoints()
    {
        return healthPoints;
    }

    AudioClip GetFootstepsSound()
    {
        return footstepsSound;
    }
}
