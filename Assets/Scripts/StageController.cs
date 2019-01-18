using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageController : MonoBehaviour
{
    public GameObject LeftCurtain = null;
    public GameObject RightCurtain = null;

    // Start is called before the first frame update
    void Start()
    {
        LeftCurtain = GameObject.Find("CurtainLeftParent");
        RightCurtain = GameObject.Find("CurtainRightParent");

        //OpenCurtains();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OpenCurtains()
    {
        print("OpenCurtains");
        if(LeftCurtain == null)
        {
            print("null");
        }

        StartCoroutine(OpeningCurtains());

    }

    IEnumerator OpeningCurtains()
    {
        float TimeBetween = 0.015f;
        float FullyOpenScale = 0.1f;
        float CloseEnoughRange = 0.09f;

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
                break;
            }


            yield return new WaitForSeconds(TimeBetween);
        }

        EventManager.instance.MarkEventAsDone();
    }

    public void CloseCurtains()
    {
        StartCoroutine(ClosingCurtains());
    }

    IEnumerator ClosingCurtains()
    {
        float TimeBetween = 0.015f;
        float FullyClosedScale = 1.1f;
        float CloseEnoughRange = 0.09f;

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
