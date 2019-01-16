using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This checkpoint does not need any hooking up in the editor as it's all done in C++
public class Checkpoint : TriggerEvent
{
    CheckpointManager checkpointManager;

    // Start is called before the first frame update
    void Start()
    {
        // You are parented to the checkpoint manager, so use that to grab a reference to it
        checkpointManager = GetComponentInParent<CheckpointManager>();

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
        checkpointManager.SetRecentCheckpoint(this);
    }
}
