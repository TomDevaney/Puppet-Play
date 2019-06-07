using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
	public float openToDegrees;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void Open()
	{
		// Play animation
		StartCoroutine("OpenGate");

		// TODO: Play sound

		// Disable collision so it doesn't move the player when opening
		GetComponentInChildren<BoxCollider>().enabled = false;
	}

	IEnumerator OpenGate()
	{
		float TimeBetween = 0.0035f;
		float ratio = 0.0f;

		while (true)
		{
			float nextY = Mathf.Lerp(0.0f, openToDegrees, ratio);

			transform.rotation = Quaternion.Euler(transform.rotation.x, nextY, transform.rotation.z);

			if (nextY == openToDegrees)
			{
				break;
			}

			ratio += TimeBetween;

			yield return new WaitForSeconds(TimeBetween);
		}

		EventManager.instance.MarkEventAsDone();
	}
}
