using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Direction
{
	LEFT = -1,
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

    public Animator animator;

	MeshRenderer meshRenderer;

	/* Facing variables */
	Direction facingDirection = Direction.RIGHT;

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
		meshRenderer = GetComponent<MeshRenderer>();
		animator = GetComponentInChildren<Animator>();

		// Set clip to footsteps source because that's the only sound it'll play
		footstepsAudioSource.clip = footstepsSound;
    }

    // Update is called once per frame
    public virtual void Update()
    {
		// Do lerp at a constant rate towards desired degree
		currentFacingDegree = Mathf.MoveTowards(currentFacingDegree, desiredFacingDegree, Time.deltaTime * FACING_SPEED);

		transform.rotation = Quaternion.Euler(0.0f, currentFacingDegree, 0.0f);
	}

    public void Move(float xAxis)
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
        transform.Translate(new Vector2(xAxis * moveSpeed, 0.0f), Space.World);

		// Only play footsteps if they're actually moving and it's not playing already
		const float walkingThreshold = 0.15f;

        if (Mathf.Abs(xAxis) >= walkingThreshold)
        {
            // Don't play if they're in the air!
            if (!footstepsAudioSource.isPlaying && standingOnSurface)
            {
                footstepsAudioSource.Play();
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
        living.TakeDamage(attackDamage);
    }

    public void TakeDamage(int damage)
    {
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
        // TODO: might have to delay it somehow depending on how it sounds being played exactly same time 
        if (deathSound != null)
        {
            AudioManager.instance.PlaySoundFXAtPosition(deathSound, transform.position);
        }

		// Delay death until animation done
		Invoke("MarkAsDead", deathAnimationTime);
    }

	public virtual void MarkAsDead()
	{
		// Disable so it can be respawned later if needed
		gameObject.SetActive(false);

		EventManager.instance.NotifyOfDeath(this);

		// Mark as dead
		isDead = true;
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

		// Reset alpha
		Color color = meshRenderer.material.color;
		color.a = 1.0f;

		meshRenderer.material.color = color;

        // Reset variables
        gameObject.SetActive(true);
        isDead = false;
    }

	/* Coroutines */

	// Was gonna use for player puppet when he died, but because of the current model having multiple meshrenderers
	// I decided against using this right now
	IEnumerator FadeTransparency()
	{
		const float TimeTilZero = 3.0f;
		float TotalTime = 0.0f;

		while (true)
		{
			Color CurrentColor = meshRenderer.material.color;
			CurrentColor.a = Mathf.Lerp(1.0f, 0.0f, TotalTime / TimeTilZero);

			meshRenderer.material.color = CurrentColor;

			if (CurrentColor.a == 0.0f)
			{
				break;
			}

			TotalTime += Time.deltaTime;

			yield return new WaitForSeconds(0.1f);
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
