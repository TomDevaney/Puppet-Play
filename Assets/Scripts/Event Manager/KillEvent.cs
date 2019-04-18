using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

// A kill event is an event that is triggered when certain Living objects have been killed

public class KillEvent : MonoBehaviour
{
	// An event that can be assigned to by other classes and through the editor
	public UnityEvent OnTrigger;

	// List of living that need to be killed for event to be triggered
	public List<Living> allLiving;

	// So the event won't keep on firing off when it's been completed
	bool eventDone = false;

	// Start is called before the first frame update
	public virtual void Start()
	{

	}

	public void NotifyOfDeath(Living living)
	{
		if (!eventDone)
		{
			allLiving.Remove(living);

			if (allLiving.Count == 0)
			{
				OnTrigger.Invoke();
				eventDone = true;
			}
		}
	}
}
