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

		// Reference to daughter
		Puppet daughterPuppet;

		// AI variables
		Transform aiTransform;

        // Don't start following until this distance is reached
        const float maxDistanceBetweenDad = 4.0f;

        public IdleState(AIContoller controller) : base(controller)
        {
            // Get references
            dadPuppet = GameManager.instance.GetPlayerPuppet();
            aiTransform = GetAIController().transform;
			daughterPuppet = GameManager.instance.GetDaughterPuppet();

			GetAIController().animator.SetTrigger("Idle");
        }

        override public State Update()
        {
			// Do idle stuff
			daughterPuppet.Move(0);

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

		// Reference to daughter
		Puppet daughterPuppet;

		// To prevent double or even triple jumping
		bool bJustJumped = false;
		int framesSinceJumped = 0;

		const float smallJumpRayDistance = 1.0f;

		// Big jump needs more space to jump higher
		const float bigJumpRayDistance = 1.5f;

		public FollowState(AIContoller controller) : base(controller)
        {
            // Retrieve references
            dadTransform = GameManager.instance.GetPlayerPuppet().transform;
            aiTransform = GetAIController().transform;
			daughterPuppet = GameManager.instance.GetDaughterPuppet();
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

			// Don't let the daughter move past the left bounds (-1 is left movement)
			if (direction == -1 && aiTransform.position.x <= GetAIController().GetLeftBound())
			{
				direction = 0;
				print("Daughter can't move past left bound!");
			}

			// Move them closer to dad
			daughterPuppet.Move(direction);

			// Check to see if somethings in the way
			if (!bJustJumped)
			{
				Vector3 position = aiTransform.position;

				// Only look for environment layer
				int LayerMask = 1 << 10;

				// Origin is a bit below feet. Offset it to be towards torso
				RaycastHit hitInfo;

				// Is there something above the princess?
				if (Physics.Raycast(new Vector3(position.x, position.y + 1.5f, position.z), aiTransform.right * -1, out hitInfo, bigJumpRayDistance, LayerMask))
				{
					// Do a big jump
					daughterPuppet.SetDoBigJump(true);
					daughterPuppet.Jump();

					bJustJumped = true;
					framesSinceJumped = 0;
				}
				// Is something in front of princess?
				else if (Physics.Raycast(new Vector3(position.x, position.y + 0.5f, position.z), aiTransform.right * -1, out hitInfo, smallJumpRayDistance, LayerMask))
				{
					// Do a regular jump
					daughterPuppet.SetDoBigJump(false);
					daughterPuppet.Jump();

					bJustJumped = true;
					framesSinceJumped = 0;
				}
			}
			else
			{
				++framesSinceJumped;

				if (framesSinceJumped == 60)
				{
					bJustJumped = false;
				}
			}

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
            // TODO: Uncomment animation
            // Do walk animation
            //GetAIController().GetAnimator().SetTrigger("Walk");

        }

        // 
        override public void OnStateExit()
        {

        }
    }

    Animator animator;
	float leftXBoundPosition;

	// Start is called before the first frame update
	void Start()
    {
        // Set references
        animator = GetComponentInChildren<Animator>();

        // Set to the default state
        SetCurrentState(new IdleState(this));
    }

    public override void Reset()
    {
        SetCurrentState(new IdleState(this));

		animator.SetTrigger("Idle");
		animator.ResetTrigger("Walk");
	}

	/* Setters */
	public void SetLeftBound(float position)
	{
		leftXBoundPosition = position;
	}

	/* Getters */
	Animator GetAnimator()
    {
        return animator;
    }

	float GetLeftBound()
	{
		return leftXBoundPosition;
	}
}
