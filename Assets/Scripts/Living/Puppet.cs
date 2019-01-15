using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puppet : Living
{

    public Rigidbody TheRigidBody;

    public bool IsStandingOnSurface = true;

    public float FootSpread = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        TheRigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckStandingOnSurface();
    }

    void CheckStandingOnSurface()
    {
        //Ignore the Player Layer
        int LayerMask = 1 << 9;
        LayerMask = ~LayerMask;

        float MaxRayDistance = 0.2f;
        RaycastHit HitInfo;
        Vector3 FootSpreadOffset = new Vector3(FootSpread, 0, 0);
        if (Physics.Raycast(transform.position, transform.up * -1, out HitInfo, MaxRayDistance, LayerMask)
            || Physics.Raycast(transform.position + FootSpreadOffset, transform.up * -1, out HitInfo, MaxRayDistance, LayerMask)
            || Physics.Raycast(transform.position - FootSpreadOffset, transform.up * -1, out HitInfo, MaxRayDistance, LayerMask)
            )
        {
            IsStandingOnSurface = true;
            //print(HitInfo.distance + "");
        }
        else
        {
            IsStandingOnSurface = false;
        }
        
        
    }

    public void Jump(float force)
    {
        //print("Jumped");
        if (IsStandingOnSurface)
        {
            TheRigidBody.AddForce(0, force, 0);
        }
    }

    public void Move(float xAxis)
    {
        transform.Translate(new Vector2(xAxis * moveSpeed, 0.0f));
    }
}
