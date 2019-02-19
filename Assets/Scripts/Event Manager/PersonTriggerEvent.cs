using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Can only be triggered by a specific object
public class PersonTriggerEvent : TriggerEvent
{
    public GameObject person = null;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Return: returns true if the event was triggered
    public virtual bool OnTriggerEnter(Collider other)
    {
        bool result = false;

        // Only do if a person is set and it's the correct one
        if (person)
        {
            if (person == other.gameObject)
            {
                if (base.OnTriggerEnter(other))
                {
                    result = true;
                }
            }
        }

        return result;
    }
}
