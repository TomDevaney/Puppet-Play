using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{

    // Singleton so classes can reference without reference
    public static MenuManager instance = null;

	SortedDictionary<string, int> canvasesDictionary;
	Canvas[] canvases;
	int currentCanvasIndex = -1;

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
		// Do pause menu stuff on Esc and if they're not on main menu still
		if (Input.GetKeyUp(KeyCode.Escape) && GameManager.instance.IsGameStarted())
		{
			//if (canvasesDictionary["PauseMenuCanvas"] == currentCanvasIndex)
			//{
			//	GameManager.instance.TogglePauseGame();
			//	MenuManager.instance.ToggleMenuEnabledState("PauseMenuCanvas");
			//}
			if (canvasesDictionary["WarningMenuCanvas"] == currentCanvasIndex)
			{
				MenuManager.instance.ToggleMenuEnabledState("WarningMenuCanvas");
				MenuManager.instance.ToggleMenuEnabledState("PauseMenuCanvas");
			}
			else
			{
				GameManager.instance.TogglePauseGame();
				MenuManager.instance.ToggleMenuEnabledState("PauseMenuCanvas");
			}
		}
	}

	public void ToggleMenuEnabledState(string menuName)
	{
		int menuIndex = 0;

		if (canvasesDictionary.TryGetValue(menuName, out menuIndex))
		{
			canvases[menuIndex].gameObject.SetActive(!canvases[menuIndex].isActiveAndEnabled);

			if (canvases[menuIndex].isActiveAndEnabled)
			{
				currentCanvasIndex = menuIndex;
			}
			// Just because this one isn't active doesn't mean currentCanvasIndex isn't the active one
			// If currentCanvasIndex is equal to this menu though and it's not active, we know for sure there is no current canvass
			else if (menuIndex == currentCanvasIndex)
			{
				currentCanvasIndex = -1;
			}
		}
		else
		{
			print("MenuManager.ToggleMenuEnabledState failed due to specified menu not existing");
		}
	}
}
