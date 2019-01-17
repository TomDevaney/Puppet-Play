using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIContoller : StateMachine
{
    public class AIState : State
    {
        AIContoller aiController;

        public AIState(AIContoller controller)
        {
            aiController = controller;
        }

        public AIContoller GetAIController()
        {
            return aiController;
        }
    }

    public class IdleState : AIState
    {
        FollowState followState;

        // Dad's variables
        Puppet dadPuppet;
        Vector3 dadInitialPosition;

        public IdleState(AIContoller controller) : base(controller)
        {
            // Retrieve references
            dadPuppet = GameManager.instance.GetPlayerPuppet();

            // Set up states
            followState = new FollowState(controller);
        }

        override public State Update()
        {
            // Do idle stuff

            //
            return CheckForTransition();
        }


        override public State CheckForTransition()
        {
            State state = null;

            // Did the player move. Only look at x so she doesn't try to follow base on jump
            if (dadPuppet.transform.position.x != dadInitialPosition.x)
            {
                // If so, do the follow state
                state = followState;
            }

            return state;
        }

        // 
        override public void OnStateEnter()
        {
            GetAIController().GetAnimator().SetTrigger("idle");

            dadInitialPosition = dadPuppet.transform.position;
        }

        // 
        override public void OnStateExit()
        {

        }
    }

    public class FollowState : AIState
    {
        IdleState idleState;

        Transform aiTransform;

        // Dad variables
        Transform dadTransform;

        // Only focus on x for now
        float positionToGoTo;

        // Give him some breathing room
        const float offsetFromDad = 1.0f;

        //
        const float moveSpeed = 0.1f;

        public FollowState(AIContoller controller) : base(controller)
        {
            // Retrieve references
            dadTransform = GameManager.instance.GetPlayerPuppet().transform;
            aiTransform = controller.transform;

            // Set up states
            idleState = new IdleState(controller);
        }

        override public State Update()
        {
            // Do follow stuff
            int direction = -1;

            // Position is to the right
            if (positionToGoTo > aiTransform.position.x)
            {
                direction = 1;
            }

            // Move them closer to dad
            aiTransform.Translate(new Vector3(direction * moveSpeed, 0.0f, 0.0f));

            // Check if you're in the desired spot
            return CheckForTransition();
        }

        override public State CheckForTransition()
        {
            State state = null;

            // Did we reach the position we wanted to reach?
            if (positionToGoTo == aiTransform.position.x)
            {
                // Did the player stop moving?
                if (positionToGoTo == dadTransform.position.x)
                {
                    // Do the idle state
                    state = idleState;
                }
                else
                {
                    positionToGoTo = dadTransform.position.x;
                }
            }

            return state;
        }

        // 
        override public void OnStateEnter()
        {
            GetAIController().GetAnimator().SetTrigger("Walk");

            positionToGoTo = dadTransform.position.x;
            positionToGoTo -= offsetFromDad;
        }

        // 
        override public void OnStateExit()
        {

        }
    }

    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        // Set references
        animator = GetComponent<Animator>();

        // Set to the default state
        SetCurrentState(new IdleState(this));
    }

    /* Getters */
    Animator GetAnimator()
    {
        return animator;
    }
}
