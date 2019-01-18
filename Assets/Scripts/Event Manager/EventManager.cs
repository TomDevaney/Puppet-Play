﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    // Singleton so classes can reference without reference
    public static EventManager instance = null;

    // Event for when an event is finished
    // Anything that is interested in this will subscribe
    public delegate void EventDone();
    public static event EventDone OnEventDone;

    

    // Awake is called before Start
    void Awake()
    {
        // Set up singleton
        if (!instance)
        {
            instance = this;
        }

        //Sets this to not be destroyed when reloading scene... Not sure it's needed for this game
        //DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Two ways for an event to marked as done
    //
    // 1) Certain events cannot be determined when they are done, so we will mark them as done right away
    // Example: an animation cannot be determined when it is done easily, so a timer event can be used here.
    // A sound needs to happen after the animation. The animation takes around 3.5 seconds, so we set a TimerEvent on a sound for 3.5 seconds
    //
    // 2) Other events can be marked as completed when they are actually done.
    // This is handled in separate classes though so this function will be called by those classes
    public void MarkEventAsDone()
    {
        if (OnEventDone != null)
        {
            OnEventDone();
        }
        else
        {
            Debug.Log("No one was listening for OnEventDone");
        }
    }

    /* Events */
    public void PlayAnimation(string stateName)
    {
        MarkEventAsDone();
    }

    public void PlayDialogue(string numOfDialogues)
    {
        DialogueManager.instance.StartDialogue(int.Parse(numOfDialogues));
    }

    public void PlaySound(string soundName)
    {
        MarkEventAsDone();
    }

    public void ZoomCamera(string zoomValue)
    {

    }

    public void UnzoomCamera(string zoomValue)
    {

    }

    public void PanCamera(string panValue)
    {

    }

    // Could also make it where if they sent in one number, it's just an offset
    // If it's two numbers, it's a coordinate

    // PathToPuppet WorldXCoordinate
    public void MovePuppet(string PuppetPathAndLocation)
    {
        string[] args = PuppetPathAndLocation.Split(' ');
        Puppet puppet = GameObject.Find(args[0]).GetComponent<Puppet>();
        puppet.MoveToLocation(float.Parse(args[1]));
    }

    public void MakePuppetJump(string PathToPuppet)
    {
        Puppet puppet = GameObject.Find(PathToPuppet).GetComponent<Puppet>();
        puppet.Jump();
        MarkEventAsDone();
    }

    public void OpenCurtains(string unused)
    {
        StageController stageController = GameObject.Find("Stage").GetComponent<StageController>();
        stageController.OpenCurtains();
    }

    public void CloseCurtains(string unused)
    {
        StageController stageController = GameObject.Find("Stage").GetComponent<StageController>();
        stageController.CloseCurtains();
    }

    public void SetMainMenuVisible(string visible)
    {
        MenuManager.instance.MainMenuSetActive(visible == "y");
        MarkEventAsDone();
    }
}
