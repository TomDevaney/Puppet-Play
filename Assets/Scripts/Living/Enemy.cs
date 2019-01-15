using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Living
{
    // Start is called before the first frame update
    void Start()
    {
        // All enemies will deal 1 damage as of now
        SetAttackDamage(1); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // React to them dying
    public override void JustDied()
    {
        // Get rid of the object
        GameObject.Destroy(gameObject);
    }

    // Check if it has hurt someone
    private void OnCollisionEnter(Collision collision)
    {
        // TODO: Check if the puppet is currently attacking maybe?

        // Check if it hit a puppet
        if (collision.gameObject.CompareTag("Puppet"))
        {
            Living puppet = collision.gameObject.GetComponent<Living>();

            DoDamage(puppet);
        }
    }
}
