using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This checkpoint does not need any hooking up in the editor as it's all done in C++
public class Checkpoint : TriggerEvent
{
    // Start is called before the first frame update
    void Start()
    {
        // This will happen when someone goes through the trigger
        OnTrigger.AddListener(SetMostRecentCheckpoint);
    }

    // Override ontriggerenter to be more specific
    public override void OnTriggerEnter(Collider other)
    {
        // Only trigger if the player was the one to walk through
        if (other.name == "Player")
        {
            // Trigger the event that the player hooked up
            base.OnTriggerEnter(other);
        }
    }

    // Tell manager what's up
    void SetMostRecentCheckpoint()
    {
        GameManager.instance.SetRecentCheckpoint(this);
    }

    public Vector3 GetLocation()
    {
        return transform.position;
    }
}
