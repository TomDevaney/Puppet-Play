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
        LeftCurtain = GameObject.Find("CurtainLeft");
        RightCurtain = GameObject.Find("CurtainRight");

        OpenCurtains();
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

    }

    IEnumerator OpeningCurtains()
    {

        yield return new WaitForSeconds(.01f);
    }

    public void CloseCurtains()
    {

    }
}
