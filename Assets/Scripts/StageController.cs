using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageController : MonoBehaviour
{
    public GameObject LeftCurtain = null;
    public GameObject RightCurtain = null;

	public AudioClip CurtainAudioClip;

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
        float TimeBetween = 0.015f * 1.5f;
        float FullyOpenScale = 0.0f;
        float CloseEnoughRange = 0.02f;

        while (true)
        {
            float NextZ = Mathf.Lerp(LeftCurtain.transform.localScale.z, FullyOpenScale, TimeBetween);

            LeftCurtain.transform.localScale = new Vector3(
                LeftCurtain.transform.localScale.x,
                LeftCurtain.transform.localScale.y,
                NextZ);

            RightCurtain.transform.localScale = new Vector3(
                RightCurtain.transform.localScale.x,
                RightCurtain.transform.localScale.y,
                NextZ);


            if(Mathf.Abs(NextZ - FullyOpenScale) <= CloseEnoughRange)
            {
				LeftCurtain.transform.localScale = Vector3.zero;
				RightCurtain.transform.localScale = Vector3.zero;
				break;
            }


            yield return new WaitForSeconds(TimeBetween);
        }

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
		float TimeBetween = 0.015f * 1.5f;
		float FullyClosedScale = 1.1f;
        float CloseEnoughRange = 0.02f;

        while (true)
        {
            float NextZ = Mathf.Lerp(LeftCurtain.transform.localScale.z, FullyClosedScale, TimeBetween);

            LeftCurtain.transform.localScale = new Vector3(
                LeftCurtain.transform.localScale.x,
                LeftCurtain.transform.localScale.y,
                NextZ);

            RightCurtain.transform.localScale = new Vector3(
                RightCurtain.transform.localScale.x,
                RightCurtain.transform.localScale.y,
                NextZ);


            if (Mathf.Abs(NextZ - FullyClosedScale) <= CloseEnoughRange)
            {
                break;
            }


            yield return new WaitForSeconds(TimeBetween);
        }
        EventManager.instance.MarkEventAsDone();
    }
}
