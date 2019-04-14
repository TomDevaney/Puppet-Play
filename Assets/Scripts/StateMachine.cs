using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    public State()
    {

    }

	// Returns the state that the machine needs to switch to
    virtual public State Update()
    {
        return null;
    }

    /* Abstract functions that child states need to implement */
	// Sees if there's a reason to go to a different state
    virtual public State CheckForTransition()
    {
        return null;
    }

    // Have to store references here sometimes because the constructors are called before Awake()
    virtual public void OnStateEnter()
    {

    }

    // 
    virtual public void OnStateExit()
    {

    }
}


// The big problem I have with the current state machine is changing the state externally
// For example, in EventManager I want to change the state for the CameraFSM
// This looks stupid because every state needs to know about the CameraFSM and trying to get a refernce and setting it in constructor sucks
// This state machine looks much more flexible and easier to maintain and set up: https://unity3d.com/learn/tutorials/topics/navigation/finite-state-ai-delegate-pattern
// One of the benefits are that the states in this FSM are ScriptableObjects, so they actually exist as assets
// This is different than my states which are just C# classes
// Being able to set a state by draggin in a reference to that state for an event in EventManger would be much better
// I'm gonna keep this poor state machine for this project, but next time implement the one above

public class StateMachine : MonoBehaviour
{
    //State previousState;
    State currentState;
    //State nextState;

    bool updateMachine = true;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // Update returning a state means that's the next state
        if (currentState == null)
            return;

        // Don't update if it's been told not to
        if (updateMachine)
        {
            State tempState = currentState.Update();

            if (tempState != null)
            {
                currentState.OnStateExit();

                currentState = tempState;

                currentState.OnStateEnter();
                //nextState = tempState;
            }
        }
    }

    public virtual void Reset()
    {

    }

    /* Setters */
    public virtual void SetCurrentState(State state)
    {
        if (currentState != null)
        {
            currentState.OnStateExit();
        }

        currentState = state;
        currentState.OnStateEnter();
    }

    public void SetUpdateMachine(bool doUpdate)
    {
        updateMachine = doUpdate;

        // Reset to base state
        Reset();
    }

	/* Getters */
	public State GetCurrentState()
	{
		return currentState;
	}
}
