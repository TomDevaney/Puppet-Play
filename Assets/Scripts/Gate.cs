using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void Open()
	{
		// TODO: Play animation

		// TODO: Play sound

		// Temp solution
		Destroy(gameObject);

		EventManager.instance.MarkEventAsDone();
	}
}
