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

		if(InputManager.instance.CanPlayerJump())
		{
			if(Input.GetButtonUp("Jump"))
			{
				ThePuppet.Jump();
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
