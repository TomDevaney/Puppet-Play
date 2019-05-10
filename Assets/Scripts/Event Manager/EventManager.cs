using System.Collections;
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

	KillEvent[] killEvents;
	TriggerEvent[] triggerEvents;

	// Prefabs to instantiate
	public GameObject patrollingEnemy;

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
		// Set up list of kill events
		killEvents = GetComponentsInChildren<KillEvent>();
		triggerEvents = GetComponentsInChildren<TriggerEvent>();
	}

	// Update is called once per frame
	void Update()
    {
        
    }

	// Will allow all trigger events to be triggered again
	public void ResetTriggerEvents()
	{
		for (int i = 0; i < triggerEvents.Length; ++i)
		{
			triggerEvents[i].ResetTrigger();
		}
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

	/* Event Manager functionality */
	public void NotifyOfDeath(Living living)
	{
		for (int i = 0; i < killEvents.Length; ++i)
		{
			killEvents[i].NotifyOfDeath(living);
		}
	}

    /* Custom Events */
    public void PlayAnimationForPlayer(string stateName)
    {
		GameManager.instance.GetPlayerPuppet().animator.SetTrigger(stateName);

		// Going to make a very lazy and bad assumption that they were in Idle so send them back after the desired state finishes
		//GameManager.instance.GetPlayerPuppet().animator.SetTrigger("Idle");

		MarkEventAsDone();
    }

	public void PlayAnimationForDaughter(string stateName)
	{
		GameManager.instance.GetDaughterPuppet().animator.SetTrigger(stateName);

		// Going to make a very lazy and bad assumption that they were in Idle so send them back after the desired state finishes
		//GameManager.instance.GetPlayerPuppet().animator.SetTrigger("Idle");

		MarkEventAsDone();
	}

	public void PlayDialogue(string nothing)
    {
        DialogueManager.instance.StartDialogue();
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

    public void MoveCamera(string positionXY)
    {
        string[] args = positionXY.Split(' ');

		GameManager.instance.GetCameraFSM().SetCurrentState(new CameraFSM.MoveState(GameManager.instance.GetCameraFSM(), float.Parse(args[0]), float.Parse(args[1]), true));
	}

	public void FollowObjectCamera(string objectName)
	{
		GameObject gameObject = GameObject.Find(objectName);

		GameManager.instance.GetCameraFSM().SetCurrentState(new CameraFSM.FollowState(GameManager.instance.GetCameraFSM(), gameObject, true));
	}

	public void DaughterDoAttack(string nothing)
	{
		GameManager.instance.GetDaughterPuppet().Attack();
        Invoke("MarkEventAsDone", 1.0f);
	}

	// TODO: Make it dynamic where it can spawn other types of enemies
	// For now just do patrolling to finish the game up
	public void SpawnEnemy(string location)
	{
        string[] args = location.Split(' ');
		Vector3 vectorLocation = new Vector3(float.Parse(args[0]), float.Parse(args[1]), float.Parse(args[2]));

		Instantiate(patrollingEnemy, vectorLocation, new Quaternion());

		MarkEventAsDone();
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

    public void MakePuppetJump(string pathToPuppetAndBigJump)
    {
        string[] args = pathToPuppetAndBigJump.Split(' ');
		Puppet puppet = GameObject.Find(args[0]).GetComponent<Puppet>();

        // Regardless of whether they can jump, mark them as being able to
        // Little hack for daughter who is never standing on surface because her origin is in the middle whereas the knight is at his fight
        puppet.SetStandingOnSurface(true);
        puppet.Jump();

		// If the big jump argument was entered, use the argument
		// Otherwise, default to small jump
		if (args.Length > 1)
		{
			puppet.SetDoBigJump(bool.Parse(args[1]));
		}
		else
		{
			puppet.SetDoBigJump(false);
		}


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
        //MenuManager.instance.MainMenuSetActive(visible == "y");
        MenuManager.instance.ToggleMenuEnabledState("MainMenuCanvas");
        MarkEventAsDone();
	}

	public void ToggleMenuActive(string menuName)
	{
		MenuManager.instance.ToggleMenuEnabledState(menuName);
		MarkEventAsDone();
	}

    // Will tell daughter to stop checking for other states
    // Might have problems as this will retain state that it was in
    // Might want to set current state to idle?
    //
    // Also this might not give the desired effect
    // She wont follow once she hits the trigger even if you go the other way
    // Might just want to use the trigger as a do not go past this section
    //
    public void SetDaughterUpdateMachine(string doUpdate)
    {
        GameManager.instance.GetDaughterPuppet().GetComponent<AIContoller>().SetUpdateMachine(int.Parse(doUpdate) == 1);
        MarkEventAsDone();
    }
}
