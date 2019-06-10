using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
	public float openToDegrees;
	public AudioClip openAudioClip;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void ResetGate()
	{
		transform.rotation = Quaternion.Euler(transform.rotation.x, 0.0f, transform.rotation.z);
	}

	public void Open()
	{
		// Play animation
		StartCoroutine("OpenGate");

		// Play sound
		AudioManager.instance.PlaySoundFXAtPosition(openAudioClip, transform.position);

		// Disable collision so it doesn't move the player when opening
		GetComponentInChildren<BoxCollider>().enabled = false;
	}

	IEnumerator OpenGate()
	{
		float openSpeed = 0.0035f;
		float TimeBetween = 0.0f;
		float ratio = 0.0f;

		while (true)
		{
			float nextY = Mathf.Lerp(0.0f, openToDegrees, ratio);

			transform.rotation = Quaternion.Euler(transform.rotation.x, nextY, transform.rotation.z);

			if (nextY == openToDegrees)
			{
				break;
			}

			ratio += openSpeed;

			yield return new WaitForSeconds(TimeBetween);
		}

		EventManager.instance.MarkEventAsDone();
	}
}
