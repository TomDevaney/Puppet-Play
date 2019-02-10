using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{

    // Singleton so classes can reference without reference
    public static MenuManager instance = null;

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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MainMenuSetActive( bool active)
    {
        GameObject.Find("MainMenuCanvas").GetComponent<Canvas>().enabled = active;
    }
}
