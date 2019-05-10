using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton so classes can reference without reference
    public static GameManager instance = null;

    // Keeps track of the most recent checkpoint for respawning puppets to
    Checkpoint recentCheckpoint;
	Checkpoint[] checkpoints;

    Living[] livingBeings;

    // Even though it's a part of the livingBeings array, I want direct access to both puppets
    Puppet playerPuppet;
    Puppet daughterPuppet;

	CameraFSM cameraFSM;

    // Awake is called before Start
    void Awake()
    {
        // Set up singleton
        if (!instance)
        {
            instance = this;
        }

        // Sets this to not be destroyed when loading different scene... Not sure it's needed for this game
        //DontDestroyOnLoad(gameObject);

        // Retrieve references for everything game manager cares about
        livingBeings = GetComponentsInChildren<Living>();
		cameraFSM = FindObjectOfType<Camera>().GetComponent<CameraFSM>();
		checkpoints = FindObjectsOfType<Checkpoint>();

		Puppet[] puppets = GetComponentsInChildren<Puppet>();
        playerPuppet = puppets[0];
        daughterPuppet = puppets[1];
	}

	// Start is called before the first frame update
	void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

	// Either the game was beat or the player exited to the main menu
	// So reset everything so the player starts in a fresh new world
	public void EndGame()
	{
		RespawnAllDead();
		
		// Reset living to initial position in world
		for (int i = 0; i < livingBeings.Length; ++i)
		{
			livingBeings[i].ResetToInitialPosition();
		}

		// Set camera position at player's position
		Vector3 camPosition = GameManager.instance.GetCameraFSM().transform.position;
		camPosition.x = playerPuppet.transform.position.x;

		GameManager.instance.GetCameraFSM().transform.position = camPosition;

		// Reset checkpoints
		recentCheckpoint = null;

		foreach (Checkpoint checkpoint in checkpoints)
		{
			checkpoint.ResetCheckpoint();
		}

		// Reset dialogue manager
		DialogueManager.instance.ResetDialogueManager();

		// Reset triggers
		EventManager.instance.ResetTriggerEvents();
	}

	// The player was killed so it's game over
    public void GameOver()
    {
        // Respawn everything that was killed
        RespawnAllDead();
    }

    void RespawnAllDead()
    {
        for (int i = 0; i < livingBeings.Length; ++i)
        {
			// Only respawn enemies to the right of the checkpoint because it looked weird that enemies before the checkpoint were spawned
			if (livingBeings[i].transform.position.x > recentCheckpoint.transform.position.x)
			{
				if (livingBeings[i].IsDead())
				{
					livingBeings[i].Respawn();
				}
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

    public Puppet GetDaughterPuppet()
    {
        return daughterPuppet;
    }

	public CameraFSM GetCameraFSM()
	{
		return cameraFSM;
	}

	/* Setters */
	public void SetRecentCheckpoint(Checkpoint checkpoint)
    {
        recentCheckpoint = checkpoint;
    }
}
