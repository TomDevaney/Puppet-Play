using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

// A  trigger event can be placed anywhere in the world and when triggered, cannot be done again 
public class TriggerEvent : MonoBehaviour
{
    // An event that can be assigned to by other classes and through the editor
    public UnityEvent OnTrigger;

    // Trigger events can only happen once
    bool mHasBeenTriggeredAlready;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        mHasBeenTriggeredAlready = false;
    }

    // Return: returns true if the event was triggered
    public virtual bool OnTriggerEnter(Collider other)
    {
        bool result = false;

        // Only do if it hasn't been triggered already and if OnTrigger has been set
        if (!mHasBeenTriggeredAlready && OnTrigger != null)
        {
            // Broadcast event to all subscribers
            OnTrigger.Invoke();

            // To make sure it cannot be triggered again
            mHasBeenTriggeredAlready = true;

            result = true;
        }

        return result;
    }
}