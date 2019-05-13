using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
