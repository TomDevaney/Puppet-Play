using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Puppet ThePuppet;

    

    // Start is called before the first frame update
    void Start()
    {
        ThePuppet = GetComponent<Puppet>();
    }

    // Update is called once per frame
    void Update()
    {
		if (InputManager.instance.CanPlayerMove())
		{
			//A Left D Right
			float xAxis = Input.GetAxis("Horizontal");
			ThePuppet.Move(xAxis);
		}
		else
		{
			// Important for idle animation
			ThePuppet.Move(0.0f);
		}

		if (InputManager.instance.CanPlayerJump())
		{
			if(Input.GetButtonDown("Jump"))
			{
				ThePuppet.Jump();
                print("Jumped");
			}
		}
        else
        {
            //print("Cant Jump!");
        }

        if (InputManager.instance.CanPlayerAttack())
        {
            if (Input.GetButtonDown("Fire1"))
            {
                ThePuppet.Attack();
            }
        }


    }

	public void OnCollisionEnter(Collision Col)
	{
		//print("OnCollisionEnter");

	}

	public void OnCollisionExit()
	{
		//print("OnCollisionExit");
	}

	
}
