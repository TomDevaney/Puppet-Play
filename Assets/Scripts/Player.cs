using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public float MoveRate = 0.01f;

	public Rigidbody TheRigidBody;

	public CapsuleCollider Capsule;

    public bool IsStandingOnSurface = true;

    // Start is called before the first frame update
    void Start()
    {
		TheRigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        int LayerMask = 1 << 9;
        LayerMask = ~LayerMask;
        float MaxRayDistance = 0.2f;
        RaycastHit HitInfo;
		if(Physics.Raycast(transform.position, transform.up * -1, out HitInfo, MaxRayDistance, LayerMask))
        {
            IsStandingOnSurface = true;
            print(HitInfo.distance + "");
        }
        else
        {
            IsStandingOnSurface = false;
        }


        if (InputManager.instance.CanPlayerMove())
        {
			//A Left D Right
            float xAxis = Input.GetAxis("Horizontal");

            //transform.Translate(new Vector2(xAxis * 5.0f, 0.0f));
			transform.Translate(new Vector2(xAxis * MoveRate, 0.0f));
        }

		if(InputManager.instance.CanPlayerJump() && IsStandingOnSurface)
		{
			if(Input.GetButtonUp("Jump"))
			{
				Jump(150);
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

	public void Jump(float force)
	{
		//print("Jumped");

		TheRigidBody.AddForce( 0, force , 0);
	}
}
