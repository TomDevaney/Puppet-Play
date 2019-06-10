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

	bool[] isLivingDead;

	// So the event won't keep on firing off when it's been completed
	bool eventDone = false;

	// Start is called before the first frame update
	public virtual void Start()
	{
		isLivingDead = new bool[allLiving.Count];

		for (int i = 0; i < isLivingDead.Length; ++i)
		{
			isLivingDead[i] = false;
		}
	}

	public void NotifyOfDeath(Living living)
	{
		if (!eventDone)
		{
			// Mark living as dead if we are keeping track of it
			int livingIndex = allLiving.IndexOf(living);
			if (livingIndex != -1)
			{
				isLivingDead[livingIndex] = true;
			}

			// Check if all are dead
			bool allLivingAreDead = true;
			for (int i = 0; i < isLivingDead.Length; ++i)
			{
				if (!isLivingDead[i])
				{
					allLivingAreDead = false;
					break;
				}
			}

			if (allLivingAreDead)
			{
				OnTrigger.Invoke();
				eventDone = true;
			}
		}
	}

	public void ResetEvent()
	{
		eventDone = false;

		for (int i = 0; i < isLivingDead.Length; ++i)
		{
			isLivingDead[i] = false;
		}
	}
}
