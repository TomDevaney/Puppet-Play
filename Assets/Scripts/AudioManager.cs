using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class allows two things. A gameobject doesn't have to worry about having an audio source in order to play a sound
// and because of this it allows a gameobject to play as many sounds as it wants without having to worry about if one is already playing
// The problem with having one audiosource and then switching out the clips is that the audiosource could still be playing a clip already
// So the solution would be to have N amount of audiosource for N amount of audio clips
// This manager gets rid of that problem
public class AudioManager : MonoBehaviour
{
    // Singleton so classes can reference without reference
    public static AudioManager instance = null;

    List<GameObject> audioObjects = null;

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
        audioObjects = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        // Start at end of loop so destroying doesn't mess with loop
        for (int i = audioObjects.Count - 1; i >= 0; --i)
        {
            GameObject curObject = audioObjects[i];

            if (curObject.GetComponent<AudioSource>().isPlaying != true)
            {
                // Remove before it's destroyed
                audioObjects.Remove(curObject);

                Destroy(curObject);
            }
        }
    }

	// TODO: in the future make it where every single PlaySound function ultimately calls this
	// That way we can have specific functionss for pitch and position and 3d but reuse functionality
	// As really the playing, the clip setting, the game object management is all the same across the board
    public void PlaySoundFX(AudioClip clip)
    {
        if (clip == null)
            return;

        GameObject gameObject = new GameObject();
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.clip = clip;
        audioSource.Play();

        audioObjects.Add(gameObject);
    }

	public void PlaySoundFXLooped(AudioClip clip)
	{
		if (clip == null)
			return;

		GameObject gameObject = new GameObject();
		AudioSource audioSource = gameObject.AddComponent<AudioSource>();

		audioSource.clip = clip;
		audioSource.loop = true;
		audioSource.Play();

		audioObjects.Add(gameObject);
	}

	public void PlaySoundFXAtPosition(AudioClip clip, Vector3 position, float pitch = 1.0f)
    {
        if (clip == null)
            return;

        GameObject gameObject = new GameObject();
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();

        gameObject.transform.position = position;
        audioSource.clip = clip;
		audioSource.pitch = pitch;
        audioSource.Play();

        audioObjects.Add(gameObject);
    }

	// I am using a curve that will have sound with 25 units but with 20 units will be the loudest
	// This will handle the case for the sound that I care about, but this is obviously very static and not dynamic
	public void Play3DSoundFX(AudioClip clip, Vector3 position)
	{
		GameObject gameObject = new GameObject();
		AudioSource audioSource = gameObject.AddComponent<AudioSource>();

		// Set up sound
		gameObject.transform.position = position;
		audioSource.clip = clip;

		// Set up 3D settings
		AnimationCurve curve = new AnimationCurve();
		curve.AddKey(10.0f, 1.0f);
		curve.AddKey(17.5f, 0.15f);
		curve.AddKey(25.0f, 0.0f);
		audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, curve);

		audioSource.minDistance = 10.0f;
		audioSource.maxDistance = 25.0f;
		audioSource.spatialBlend = 1.0f;
		audioSource.rolloffMode = AudioRolloffMode.Custom;

		// Play sound
		audioSource.Play();

		// Add to managed objects list
        audioObjects.Add(gameObject);
	}

	public void StopSoundFXByClip(AudioClip clip)
	{
		foreach (GameObject gameObject in audioObjects)
		{
			if (gameObject.GetComponent<AudioSource>().clip == clip)
			{
				// Remove before it's destroyed
				audioObjects.Remove(gameObject);

				Destroy(gameObject);

				// This function won't work if there are multiple sounds currently for that clip
				// As of right now, I don't have a need for that functionality though, so the break is fine
				break;
			}
		}
	}

	public void ChangeAudioVolumeByClip(float newVolume, AudioClip clip)
	{
		foreach (GameObject gameObject in audioObjects)
		{
			if (gameObject.GetComponent<AudioSource>().clip == clip)
			{
				gameObject.GetComponent<AudioSource>().volume = newVolume;

				// This function won't work if there are multiple sounds currently for that clip
				// As of right now, I don't have a need for that functionality though, so the break is fine
				break;
			}
		}
	}
}
