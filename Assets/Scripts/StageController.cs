using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageController : MonoBehaviour
{
    public GameObject LeftCurtain = null;
    public GameObject RightCurtain = null;

	const float FullyOpenScale = 0.10f;
	const float FullyClosedScale = 1.0f;

	public AudioClip CurtainAudioClip;

	// Delegate that can be subscribed to if you need to do something when curtains are done opening or closing
	public delegate void CurtainsDoneMoving();
	public event CurtainsDoneMoving OnCurtainsDoneMoving; 

    // Start is called before the first frame update
    void Start()
    {
        LeftCurtain = GameObject.Find("CurtainLeftParent");
        RightCurtain = GameObject.Find("CurtainRightParent");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OpenCurtains()
    {
		AudioManager.instance.PlaySoundFX(CurtainAudioClip);

		StopCoroutine("Closecurtains");
		StartCoroutine(OpeningCurtains());
    }

    IEnumerator OpeningCurtains()
    {
		float TimeBetween = 0.0075f;
		float ratio = 0.0f;

		while (true)
        {
            float NextZ = Mathf.Lerp(FullyClosedScale, FullyOpenScale, ratio);

			LeftCurtain.transform.localScale = new Vector3(
                LeftCurtain.transform.localScale.x,
                LeftCurtain.transform.localScale.y,
                NextZ);

            RightCurtain.transform.localScale = new Vector3(
                RightCurtain.transform.localScale.x,
                RightCurtain.transform.localScale.y,
                NextZ);

			if (NextZ == FullyOpenScale)
			{
				break;
            }

			ratio += TimeBetween;

			yield return new WaitForSeconds(TimeBetween);
        }

		// Call delegate event
		if (OnCurtainsDoneMoving != null)
		{
			OnCurtainsDoneMoving();
		}

		// Tell event manager you're done
		EventManager.instance.MarkEventAsDone();
    }

    public void CloseCurtains()
    {
		AudioManager.instance.PlaySoundFX(CurtainAudioClip);

		// Restore scale caused from opening curtains
		LeftCurtain.transform.localScale = new Vector3(1.0f, 1.0f, 0.0f);
		RightCurtain.transform.localScale = new Vector3(1.0f, 1.0f, 0.0f);

		StopCoroutine("OpenCurtains");
		StartCoroutine(ClosingCurtains());
    }

    IEnumerator ClosingCurtains()
    {
		float TimeBetween = 0.0075f;
		float ratio = 0.0f;

		while (true)
        {
			float NextZ = Mathf.Lerp(FullyOpenScale, FullyClosedScale, ratio);

            LeftCurtain.transform.localScale = new Vector3(
                LeftCurtain.transform.localScale.x,
                LeftCurtain.transform.localScale.y,
                NextZ);

            RightCurtain.transform.localScale = new Vector3(
                RightCurtain.transform.localScale.x,
                RightCurtain.transform.localScale.y,
                NextZ);

			if (NextZ == FullyClosedScale)
            {
                break;
            }

			ratio += TimeBetween;

			yield return new WaitForSeconds(TimeBetween);
        }

		// Call delegate event
		if (OnCurtainsDoneMoving != null)
		{
			OnCurtainsDoneMoving();
		}

		// Tell event manager you're done
		EventManager.instance.MarkEventAsDone();
    }
}
