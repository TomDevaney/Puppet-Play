using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Living
{
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        // All enemies will deal 1 damage as of now
        SetAttackDamage(1); 
    }

    // Update is called once per frame
    public override void Update()
    {
		base.Update();
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
