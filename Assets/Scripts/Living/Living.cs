using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Usage: Only use PlayOneShot with this because we need to play multiple sounds through one AudioSource and that function allows this
    //AudioSource audioSource = null;

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

    // Start is called before the first frame update
    public virtual void Start()
    {
        // Initialize variables
        spawnPoint = transform.position;

        // Get references
        //audioSource = GetComponents<AudioSource>()[0];
        footstepsAudioSource = GetComponents<AudioSource>()[1];

        // Set clip to footsteps source because that's the only sound it'll play
        footstepsAudioSource.clip = footstepsSound;
    }

    // Update is called once per frame
    void Update()
    {
        // Only destroy object if dead and the sounds are done
        //if (isDead)
        //{
        //    if (!audioSource.isPlaying)
        //    {
        //        Destroy(gameObject);
        //    }
        //    else
        //    {
        //        print("still playing");
        //    }
        //}
    }

    public void Move(float xAxis)
    {
        transform.Translate(new Vector2(xAxis * moveSpeed, 0.0f));

        // Only play footsteps if they're actually moving and it's not playing already
        const float walkingThreshold = 0.15f;

        if (Mathf.Abs(xAxis) >= walkingThreshold)
        {
            // Don't play if they're in the air!
            if (!footstepsAudioSource.isPlaying && standingOnSurface)
            {
                footstepsAudioSource.Play();
            }
            //else
            //{
            //    print("still playing");
            //}
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
            //audioSource.PlayOneShot(gotHitSound);
            //audioSource.clip = gotHitSound;
            //audioSource.Play();

            print("ouch");
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
        // TODO: Do a fancy animation before destroying it

        // Play death sound
        // TODO: might have to delay it somehow depending on how it sounds being played exactly same time 
        if (deathSound != null)
        {
            AudioManager.instance.PlaySoundFXAtPosition(deathSound, transform.position);
            //audioSource.PlayOneShot(deathSound);
        }

        // Disable so it can finish playing sounds but so that it disappears
        //gameObject.SetActive(false);
        //audioSource.enabled = true;

        Destroy(gameObject);

        // Destroy once the sounds are done
        //Invoke("DestroyThis", Mathf.Max(deathSound.length, gotHitSound.length) + 0.5f);

        // Mark as dead
        isDead = true;
    }

    public virtual void Respawn()
    {
        transform.position = new Vector3(spawnPoint.x, spawnPoint.y, transform.position.z);
    }

    //public void DestroyThis()
    //{
    //    Destroy(gameObject);
    //}

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
