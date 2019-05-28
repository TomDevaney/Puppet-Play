using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This checkpoint does not need any hooking up in the editor as it's all done in C++
public class Checkpoint : TriggerEvent
{
    [SerializeField]
    Material activeMaterial = null;

	Material defaultMaterial;

    AudioSource audioSource = null;
    MeshRenderer meshRenderer;

	SphereCollider spawnCollider = null;

	// The first checkpoint in the game should be placed at the player's initial position
	// And it should be the most recent checkpoint in GameManager and should have its material changed already
	public bool isDefaultCheckpoint = false;

    // Start is called before the first frame update
    void Start()
    {
        // Grab references to components
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        audioSource = GetComponent<AudioSource>();
		spawnCollider = GetComponent<SphereCollider>();
		defaultMaterial = meshRenderer.material;

		// This will happen when someone goes through the trigger
		OnTrigger.AddListener(SetMostRecentCheckpoint);

		// Make default checkpoint active automatically
		if (isDefaultCheckpoint)
		{
			meshRenderer.material = activeMaterial;

			// Trigger it so it won't happen again
			// This also sets most recent checkpoint
			base.OnTriggerEnter(null);
		}
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

	// Go to default state
	public void ResetCheckpoint()
	{
		if (isDefaultCheckpoint)
		{
			meshRenderer.material = activeMaterial;

			// Trigger it so it won't happen again
			// This also sets most recent checkpoint
			base.OnTriggerEnter(null);
		}
		else
		{
			// Reset material to off material
			meshRenderer.material = defaultMaterial;

			// Allow to be triggered again
			ResetTrigger();
		}
	}

	// Tell manager about this checkpoint being hit
	void SetMostRecentCheckpoint()
    {
        GameManager.instance.SetRecentCheckpoint(this);
    }

    public Vector3 GetLocation()
    {
		return spawnCollider.transform.position + spawnCollider.center;
    }
}
