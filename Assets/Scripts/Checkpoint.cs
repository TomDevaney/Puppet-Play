using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This checkpoint does not need any hooking up in the editor as it's all done in C++
public class Checkpoint : TriggerEvent
{
    [SerializeField]
    Material activeMaterial = null;

    AudioSource audioSource = null;
    MeshRenderer meshRenderer;

    // Start is called before the first frame update
    void Start()
    {
        // Grab references to components
        meshRenderer = GetComponent<MeshRenderer>();
        audioSource = GetComponent<AudioSource>();


        // This will happen when someone goes through the trigger
        OnTrigger.AddListener(SetMostRecentCheckpoint);
    }

    // Override ontriggerenter to be more specific
    public override bool OnTriggerEnter(Collider other)
    {
        bool result = false;

        // Only trigger if the player was the one to walk through
        if (other.name == "Player")
        {
            // Trigger the event that the player hooked up
            if (base.OnTriggerEnter(other))
            {
                // Change the material of the object indicating that the checkpoint has been hit
                meshRenderer.material = activeMaterial;

                // Play the sound
                audioSource.Play();
            }
            result = true;
        }

        return result;
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
