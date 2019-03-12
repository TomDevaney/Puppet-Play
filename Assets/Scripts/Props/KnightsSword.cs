using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightsSword : MonoBehaviour
{

	BoxCollider boxCollider;

	Living owner;

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponentInChildren<BoxCollider>();
        owner = GetComponentInParent<Living>();
    }

    // Update is called once per frame
    void Update()
    {
        if(owner == null)
        {
        	print("owner is null");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
    	print("Sword OnCollisionEnter");
    	if(collision.transform.tag.Equals("Enemy"))
    	{
    		print("Sword hit Enemy");
    		owner.DoDamage(collision.transform.GetComponent<Living>());
    	}
    }

    private void OnTriggerEnter(Collider other)
    {
    	print("Sword OnTriggerEnter");

    	if(other.transform.tag.Equals("Enemy"))
    	{
    		print("Sword OnTriggerEnter hit Enemy");
    		owner.DoDamage(other.transform.GetComponent<Living>());
    	}
    }
}
