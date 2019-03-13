using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{

    // Singleton so classes can reference without reference
    public static MenuManager instance = null;

	SortedDictionary<string, int> canvasesDictionary;
	Canvas[] canvases;

    void Awake()
    {
        // Set up singleton
        if (!instance)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
		// Fill in array of canvases regardless of them being enabled or not
		canvasesDictionary = new SortedDictionary<string, int>();
		canvases = GetComponentsInChildren<Canvas>(true);

		for (int i = 0; i < canvases.Length; ++i)
		{
			canvasesDictionary.Add(canvases[i].name, i);
		}
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void ToggleMenuEnabledState(string menuName)
	{
		int menuIndex = 0;

		if (canvasesDictionary.TryGetValue(menuName, out menuIndex))
		{
			canvases[menuIndex].gameObject.SetActive(!canvases[menuIndex].isActiveAndEnabled);
		}
		else
		{
			print("MenuManager.ToggleMenuEnabledState failed due to specified menu not existing");
		}
	}
}
