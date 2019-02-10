using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIContoller : StateMachine
{
    public class AIState : State
    {
        AIContoller aiController;

        public AIState(AIContoller controller) : base()
        {
            aiController = controller;
        }

        public void SetAIController(AIContoller controller)
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
        // Dad's variables
        Puppet dadPuppet;
        float dadInitialPosition;

        // AI variables
        Transform aiTransform;

        // How much the dad needs to move before you start following
        //const float dadNeedsTomove = 0.3f;

        // Don't start following until this distance is reached
        const float maxDistanceBetweenDad = 4.0f;

        public IdleState(AIContoller controller) : base(controller)
        {
            // Get references
            dadPuppet = GameManager.instance.GetPlayerPuppet();
            aiTransform = GetAIController().transform;

            print("IdleState");
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

            // Regardless of which side dad is on, start following him when he gets far
            if (Mathf.Abs(dadPuppet.transform.position.x - aiTransform.position.x) >= maxDistanceBetweenDad)
            {
                state = new FollowState(GetAIController());
            }

            return state;
        }

        // 
        override public void OnStateEnter()
        {
            // TODO: Uncomment animation
            // Go to idle animation
            //GetAIController().GetAnimator().SetTrigger("idle");

            dadInitialPosition = dadPuppet.transform.position.x;
        }

        // 
        override public void OnStateExit()
        {

        }
    }

    // Could've even done a copy state of some sorts
    // Whatever the player does, this player does as well


    public class FollowState : AIState
    {
        Transform aiTransform;

        // Dad variables
        Transform dadTransform;

        // Only focus on x for now
        float positionToGoTo;

        // Give him some breathing room
        const float offsetFromDad = 3.0f;

        // Stop when you're within this range to the dad
        const float closeEnough = 0.09f;

        //
        //const float heMoved = 0.3f;

        //
        const float moveSpeed = 0.05f;

        public FollowState(AIContoller controller) : base(controller)
        {
            // Retrieve references
            dadTransform = GameManager.instance.GetPlayerPuppet().transform;
            aiTransform = GetAIController().transform;

            print("FollowState");
        }

        override public State Update()
        {
            // Do follow stuff
            int direction = -1;

            // Position is to the right
            if (dadTransform.position.x > aiTransform.position.x)
            {
                direction = 1;
            }

            // TODO: use puppet move function instead
            // Move them closer to dad
            aiTransform.Translate(new Vector3(direction * moveSpeed, 0.0f, 0.0f));

            // Check if you're in the desired spot
            return CheckForTransition();
        }

        override public State CheckForTransition()
        {
            State state = null;

            // How far away from our ideal distance are we?
            float distance = Mathf.Abs(aiTransform.position.x - dadTransform.position.x) - offsetFromDad;

            // Are we within a threshold
            if (distance <= closeEnough)
            {
                // Did the player move since?
                //if (Mathf.Abs(positionToGoTo - dadTransform.position.x) >= offsetFromDad)
                //{
                //    positionToGoTo = dadTransform.position.x;
                //    positionToGoTo -= offsetFromDad;
                //}
                //else
                {
                    // Move them the rest of the threshold
                    aiTransform.Translate(new Vector3(distance, 0.0f, 0.0f));

                    // He didn't move so do idle
                    state = new IdleState(GetAIController());
                }
            }

            return state;
        }

        // 
        override public void OnStateEnter()
        {
            // TODO: Uncomment animation
            // Do walk animation
            //GetAIController().GetAnimator().SetTrigger("Walk");

            // Go to dad's position but offset
            //positionToGoTo = dadTransform.position.x;
            //positionToGoTo -= offsetFromDad;
        }

        // 
        override public void OnStateExit()
        {

        }
    }

    // For when she needs to platform around
    // Raycast for walls
    // Raycast in front of her for jumps
    public class ParkourState : AIState
    {
        Transform aiTransform;

        // Dad variables
        Transform dadTransform;

        // Only focus on x for now
        float positionToGoTo;

        // Give him some breathing room
        const float offsetFromDad = 3.0f;

        // Stop when you're within this range to the dad
        const float closeEnough = 0.09f;

        //
        //const float heMoved = 0.3f;

        //
        const float moveSpeed = 0.05f;

        public ParkourState(AIContoller controller) : base(controller)
        {
            // Retrieve references
            dadTransform = GameManager.instance.GetPlayerPuppet().transform;
            aiTransform = GetAIController().transform;

            print("FollowState");
        }

        override public State Update()
        {
            // Do follow stuff
            int direction = -1;

            // Position is to the right
            if (dadTransform.position.x > aiTransform.position.x)
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

            // How far away from our ideal distance are we?
            float distance = Mathf.Abs(aiTransform.position.x - dadTransform.position.x) - offsetFromDad;

            // Are we within a threshold
            if (distance <= closeEnough)
            {
                // Move them the rest of the threshold
                aiTransform.Translate(new Vector3(distance, 0.0f, 0.0f));

                // He didn't move so do idle
                state = new IdleState(GetAIController());
            }

            return state;
        }

        // 
        override public void OnStateEnter()
        {

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
