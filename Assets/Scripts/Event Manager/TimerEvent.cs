using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using UnityEngine;

[Serializable]
public class StringUnityEvent : UnityEvent<string>
{
}

// A  timed event will only happen after a certain amount of seconds have passed 
[Serializable]
public class TimerEvent : MonoBehaviour
{
    // An event that can be assigned to by other classes and through the editor
    public StringUnityEvent OnTimerEnd;

    // An argument that will be passed to Invoke
    public string eventArgument;

    // The current count down time
    float currentTimer;

    // The time until the event happens
    [SerializeField]
    public float desiredTimer;

    // When count down time will be incremented
    bool doCountdown;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        doCountdown = false;
        currentTimer = 0.0f;
        desiredTimer = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (doCountdown)
        {
            currentTimer += Time.deltaTime;

            if (currentTimer >= desiredTimer)
            {
                // Broadcast event to all subscribers
                OnTimerEnd.Invoke(eventArgument);

                // To prevent further counting down
                doCountdown = false;
            }
        }
    }

    // Allows update to start running
    public void StartCountdown()
    {
        doCountdown = true;
    }

    /* Setters */
    void SetDesiredTimer(float timer)
    {
        desiredTimer = timer;
    }
}