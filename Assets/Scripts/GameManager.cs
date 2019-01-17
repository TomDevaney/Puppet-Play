﻿using System.Collections;
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

    // Even though it's a part of the livingBeings array, I want direct access to it sometimes
    Puppet playerPuppet;

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
        playerPuppet = GetComponentInChildren<Puppet>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EndGame()
    {
        // TODO: pull curtains in

        // Respawn everything that was killed
        RespawnAllDead();

        // TODO: Because I'm only respawning dead, might have to handle special logic for puppet that is still alive
    }

    void RespawnAllDead()
    {
        for (int i = 0; i < livingBeings.Length; ++i)
        {
            if (livingBeings[i].IsDead())
            {
                livingBeings[i].Respawn();
            }
        }
    }

    /* Getters */
    public Checkpoint GetRecentCheckpoint()
    {
        return recentCheckpoint;
    }

    public Puppet GetPlayerPuppet()
    {
        return playerPuppet;
    }

    /* Setters */
    public void SetRecentCheckpoint(Checkpoint checkpoint)
    {
        recentCheckpoint = checkpoint;
    }
}