using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton so classes can reference without reference
    public static GameManager instance = null;

	public AudioClip audienceTalkingAudioClip;
	public AudioClip creditsAudioClip;
	public AudioClip shhAudioClip;
	public AudioClip backgroundAudioClip;

	// Keeps track of the most recent checkpoint for respawning puppets to
	Checkpoint recentCheckpoint;
	Checkpoint[] checkpoints;

	Gate[] gates;
    Living[] livingBeings;

    // Even though it's a part of the livingBeings array, I want direct access to both puppets
    Puppet playerPuppet;
    Puppet daughterPuppet;

	CameraFSM cameraFSM;
	StageController stageController;

	// Indicates the player is in the game and not at the main menu still
	bool gameIsStarted = false;

	// Indicates whether  paused
	bool gameIsPaused = false;

	// Shows that they got to the end of the game
	bool beatGame = false;

	// Is a cutscene happening?
	[HideInInspector]
	public bool cutsceneIsPlaying;

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
		stageController = FindObjectOfType<StageController>();
		gates = FindObjectsOfType<Gate>();

		Puppet[] puppets = GetComponentsInChildren<Puppet>();
        playerPuppet = puppets[0];
        daughterPuppet = puppets[1];
	}

	// Start is called before the first frame update
	void Start()
    {
		AudioManager.instance.PlaySoundFXLooped(audienceTalkingAudioClip);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void StartGame()
	{
		// Mark as started
		gameIsStarted = true;

		// Stop audience talking sound effect
		AudioManager.instance.StopSoundFXByClip(audienceTalkingAudioClip);

		// Play shush sound
		AudioManager.instance.PlaySoundFX(shhAudioClip);

		// Invoke curtains to open after sound is done playing and then some
		Invoke("RealStartGame", shhAudioClip.length);
	}

	// Separate private function so it can be invoked
	// But do everything needed after the shh audio clip is played
	void RealStartGame()
	{
		AudioManager.instance.PlaySoundFXLooped(GameManager.instance.backgroundAudioClip);

		// Turn volume down as cutscene starts right away
		// And we don't want background music loud during cutscenes
		AudioManager.instance.ChangeAudioVolumeByClip(0.5f, GameManager.instance.backgroundAudioClip);

		EventManager.instance.OpenCurtains("");
	}

	// Either the game was beat or the player exited to the main menu
	// So reset everything so the player starts in a fresh new world
	public void EndGame(bool gameWasBeat)
	{
		// Mark as not started
		gameIsStarted = false;

		beatGame = gameWasBeat;

		InputManager.instance.DisablePlayerActions();

		// Stop game music
		AudioManager.instance.StopSoundFXByClip(backgroundAudioClip);

		// Play epic music!
		if (gameWasBeat)
		{
			AudioManager.instance.PlaySoundFXLooped(creditsAudioClip);
		}

		// Do other logic for end of game
		EventManager.instance.CloseCurtains("");

		// Subscribe to curtains delegate
		stageController.OnCurtainsDoneMoving += RealEndGame;
	}

	// Contains all the end game functionality needed to be called after the curtains are done closing
	void RealEndGame()
	{
		// Unsubscribe from curtains delegate
		stageController.OnCurtainsDoneMoving -= RealEndGame;

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
		recentCheckpoint = checkpoints[0];

		foreach (Checkpoint checkpoint in checkpoints)
		{
			checkpoint.ResetCheckpoint();
		}

		// Reset triggers
		EventManager.instance.ResetTriggerEvents();

		// Reset gates
		for (int i = 0; i < gates.Length; ++i)
		{
			gates[i].ResetGate();
		}

		// Respawn everything that was killed
		for (int i = 0; i < livingBeings.Length; ++i)
		{
			if (livingBeings[i].IsDead())
			{
				livingBeings[i].Respawn();
			}
		}

		// Bring up credits if they came from beating the game
		if (beatGame)
		{
			MenuManager.instance.ToggleMenuEnabledState("CreditsCanvas");
		}
		// Bring up main menu if came from pause menu
		else
		{
			MenuManager.instance.ToggleMenuEnabledState("MainMenuCanvas");

			// Main menu has the audience talking, so play that
			AudioManager.instance.PlaySoundFXLooped(audienceTalkingAudioClip);
		}

		// Reset dialogue manager
		// Do at end as this function tells event manager that event is done
		DialogueManager.instance.ResetDialogueManager();

		// Stupid fix
		// Marking event as done calls StopCutscene() which enables player actions
		// Instead of fixing that, just disable again
		InputManager.instance.DisablePlayerActions();
	}

	// The player was killed so it's game over
	public void GameOver()
    {
        // Respawn everything that was killed
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

	// Called from pause menu to set the player back to the most recent checkpoint
	public void Retry()
	{
		// Kill player so no shenanigans can happen while curtains close
		// And it'll also take care of respawning player and doing game over stuff
		playerPuppet.TakeDamage(1);
	}

	// Close the application
	public void ExitGame()
	{
		Application.Quit();
	}

    void RespawnAllDead()
    {
    }

	public void TogglePauseGame()
	{
		gameIsPaused = !gameIsPaused;

		if (gameIsPaused)
		{
			Time.timeScale = 0.0f;
			InputManager.instance.DisablePlayerActions();
			AudioManager.instance.ChangeAudioVolumeByClip(0.5f, GameManager.instance.backgroundAudioClip);
		}
		else
		{
			Time.timeScale = 1.0f;
			InputManager.instance.EnablePlayerActions();
			AudioManager.instance.ChangeAudioVolumeByClip(1.0f, GameManager.instance.backgroundAudioClip);
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

	public StageController GetStageController()
	{
		return stageController;
	}

	public bool IsGameStarted()
	{
		return gameIsStarted;
	}

	// Don't let player pause the game if the game isn't started, if a cutscene is playing, or the curtains are moving
	// These scenarios can break the game
	public bool CanPauseGame()
	{
		return (gameIsStarted && !cutsceneIsPlaying && !stageController.IsCurtainMoving());
	}

	/* Setters */
	public void SetRecentCheckpoint(Checkpoint checkpoint)
    {
        recentCheckpoint = checkpoint;
    }
}
