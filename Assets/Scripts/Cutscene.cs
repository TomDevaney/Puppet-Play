using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

// How to use:
// Make a game object with a cutscene in the editor
// Attach a gameobject with timerevent to the cutscene
// The cutscene will play every single one of those events

public class Cutscene : MonoBehaviour
{
    // This is all the events that need to be played
    TimerEvent[] events = null;

    // Which event are we on?
    int currentEventIndex;

    // Start is called before the first frame update
    void Start()
    {
        // Iniitialize variables
        currentEventIndex = 0;

        // Fill in the list of event through children
        events = GetComponentsInChildren<TimerEvent>();
    }

    // Handles start up logic for cutscene
    public void StartCutscene()
    {
        // Do any graphical things wanted

        // Don't allow any player movement anymore
        InputManager.instance.DisablePlayerActions();

        // TODO: Tell AI to stop doing its thing

        // Actually start
        events[currentEventIndex].StartCountdown();

        // There's an event after this one
        if (currentEventIndex + 1 != events.Length)
        {
            EventManager.OnEventDone += NextEvent;
        }
        else
        {
            EventManager.OnEventDone += StopCutscene;
        }
    }

    public void NextEvent()
    {
        // Get rid of event that just called this
        EventManager.OnEventDone -= NextEvent;

        // Increment index 
        ++currentEventIndex;

        // Start the next event's timer
        events[currentEventIndex].StartCountdown();

        // There's an event after this one
        if (currentEventIndex + 1 != events.Length)
        {
            EventManager.OnEventDone += NextEvent;
        }
        else
        {
            EventManager.OnEventDone += StopCutscene;
        }
    }

    public void StopCutscene()
    {
        // Get rid of event that just called this
        EventManager.OnEventDone -= StopCutscene;

        // Undo those graphical things

        // Allow any player movement anymore
        InputManager.instance.EnablePlayerActions();

        // TODO: Tell AI to start doing its thing

        // Reset variable
        currentEventIndex = 0;
    }
}
