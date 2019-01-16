using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton so classes can reference without reference
    public static GameManager instance = null;

    // Keeps track of the most recent checkpoint for respawning puppets to
    Checkpoint recentCheckpoint;

    //
    Living[] livingBeings;

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
        // Retrieve references for everything game manager cares about
        livingBeings = GetComponentsInChildren<Living>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EndGame()
    {
        // TODO: pull curtains in

        // Respawn everything that was killed
        RespawnAllLiving();
    }

    void RespawnAllLiving()
    {
        for (int i = 0; i < livingBeings.Length; ++i)
        {
            livingBeings[i].Respawn();
        }
    }

    /* Getters */
    public Checkpoint GetRecentCheckpoint()
    {
        return recentCheckpoint;
    }

    /* Setters */
    public void SetRecentCheckpoint(Checkpoint checkpoint)
    {
        recentCheckpoint = checkpoint;
    }
}
