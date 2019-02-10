using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    public State()
    {

    }

    virtual public State Update()
    {
        return null;
    }

    // Abstract functions that child states need to implement
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


public class StateMachine : MonoBehaviour
{
    //State previousState;
    State currentState;
    //State nextState;

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
        State tempState = currentState.Update();

        if (tempState != null)
        {
            currentState.OnStateExit();

            currentState = tempState;

            currentState.OnStateEnter();
            //nextState = tempState;
        }
    }

    /* Setters */
    public void SetCurrentState(State state)
    {
        if (currentState != null)
        {
            currentState.OnStateExit();
        }

        currentState = state;
        currentState.OnStateEnter();
    }

    /* Getters */
}
