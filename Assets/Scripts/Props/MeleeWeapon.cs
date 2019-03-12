using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
	BoxCollider boxCollider;

	Living owner;

	public bool AttackModeActive = false;


    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponentInChildren<BoxCollider>();
        owner = GetComponentInParent<Living>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
    	if( AttackModeActive && other.transform.tag.Equals("Enemy"))
    	{
    		owner.DoDamage(other.transform.GetComponent<Living>());
    	}
    }
}
