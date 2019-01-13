using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class Cutscene : MonoBehaviour
{
    // This is all the events that need to be played
    public List<TimerEvent> events = new List<TimerEvent>();

    // Which event are we on?
    int currentEventIndex;

    // Start is called before the first frame update
    void Start()
    {
        // Iniitialize variables
        currentEventIndex = 0;
    }

    // Handles start up logic for cutscene
    public void StartCutscene()
    {
        // Do any graphical things wanted

        // Don't allow any player movement anymore
        InputManager.instance.SetCanPlayerMove(false);

        // TODO: Tell AI to stop doing its thing

        // Actually start
        events[currentEventIndex].StartCountdown();

        // There's an event after this one
        if (currentEventIndex + 1 != events.Count)
        {
            EventManager.OnEventDone += NextCutscene;
        }
        else
        {
            EventManager.OnEventDone += StopCutscene;
        }
    }

    public void NextCutscene()
    {
        // Increment index 
        ++currentEventIndex;

        // Start the next event's timer
        events[currentEventIndex].StartCountdown();

        // There's an event after this one
        if (currentEventIndex + 1 != events.Count)
        {
            EventManager.OnEventDone += NextCutscene;
        }
        else
        {
            EventManager.OnEventDone += StopCutscene;
        }
    }

    public void StopCutscene()
    {
        // Undo those graphical things

        // Allow any player movement anymore
        InputManager.instance.SetCanPlayerMove(true);

        // TODO: Tell AI to start doing its thing

        // Reset variable
        currentEventIndex = 0;
    }
}
