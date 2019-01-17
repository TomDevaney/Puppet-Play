using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Living : MonoBehaviour
{
    // Their health
    int healthPoints;

    // Movement speed
    // Set to 0 if you don't want to move
    public float moveSpeed = 0.01f;

    // How much damage it will deal to the puppet
    int attackDamage;

    // hp is 0
    bool isDead;

    // Where the living will spawn if they respawn after dying
    Vector3 spawnPoint;

    // 
    // Animator animator;

    // Start is called before the first frame update
    public void Start()
    {
        // Initialize variables
        spawnPoint = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DoDamage(Living living)
    {
        living.TakeDamage(attackDamage);
    }

    public void TakeDamage(int damage)
    {
        // Apply damage
        healthPoints -= damage;

        Debug.Log("Ouch");

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

        isDead = true;
    }

    public virtual void Respawn()
    {
        transform.position = new Vector3(spawnPoint.x, spawnPoint.y, transform.position.z);
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

    public Vector3 GetSpawnPoint()
    {
        return spawnPoint;
    }

    public int GetHealthPoints()
    {
        return healthPoints;
    }
}
