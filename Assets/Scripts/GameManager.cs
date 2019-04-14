using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton so classes can reference without reference
    public static GameManager instance = null;

    // Keeps track of the most recent checkpoint for respawning puppets to
    Checkpoint recentCheckpoint;

    Living[] livingBeings;

    // Even though it's a part of the livingBeings array, I want direct access to both puppets
    Puppet playerPuppet;
    Puppet daughterPuppet;

	CameraFSM cameraFSM;
	//FollowCamera followCamera;

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

        Puppet[] puppets = GetComponentsInChildren<Puppet>();
        playerPuppet = puppets[0];
        daughterPuppet = puppets[1];

		//followCamera = FindObjectOfType<Camera>().GetComponent<FollowCamera>();
		cameraFSM = FindObjectOfType<Camera>().GetComponent<CameraFSM>();
	}

	// Start is called before the first frame update
	void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GameOver()
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

	//public FollowCamera GetFollowCamera()
	//{
	//	return followCamera;
	//}

	/* Setters */
	public void SetRecentCheckpoint(Checkpoint checkpoint)
    {
        recentCheckpoint = checkpoint;
    }
}
