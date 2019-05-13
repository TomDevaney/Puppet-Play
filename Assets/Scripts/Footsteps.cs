using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
	[SerializeField]
	AudioClip footstepsSound = null;

	public float pitchMinimum;
	public float pitchMaximum;

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void PlayFootstepSound()
	{
		float randomPitch = Random.Range(pitchMinimum, pitchMaximum);
		AudioManager.instance.PlaySoundFXAtPosition(footstepsSound, transform.position, randomPitch);
	}
}
