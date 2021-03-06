﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Windows;
using UnityEngine.UI;

// A complete piece of dialogue. Won't need anything else in order to display
public class Dialogue
{
    // Who said the dialogue
    Person mPerson;

    // What that person said
    string mWords;

    public Dialogue(Person person, string words)
    {
        mPerson = person;
        mWords = words;
    }

    /* Getters */
    public Person GetPerson()
    {
        return mPerson;
    }

    public string GetWords()
    {
        return mWords;
    }
}

// Everything needed to identify the person in question
public class Person
{
    string mName;

	// No longer used. Used to change color of text on a per person basis
    Color mColor;

    public Person(string name, Color color)
    {
        mName = name;
        mColor = color;
    }

    /* Getters */
    public string GetName()
    {
        return mName;
    }

    public Color GetColor()
    {
        return mColor;
    }
}

/* How the dialogue parser works */
// Add the name of the person on the first line
// On the second line add the words you want them to say
// Add a newline to indicate a new dialogue is next
// There is a limit to how many words can be displayed so break it into small chunks

// Add "//" after the chunk of dialogue you want to display back to back in a cutscene
// This comment will indicate the end of a current block of dialogue

public class DialogueManager : MonoBehaviour
{
    // Singleton so classes can reference without reference
    public static DialogueManager instance = null;

    // Every piece of dialogue needed for the game
    // If it ever became too big, or too slow to load in all the dialogue at once, there could be multiple dimensions and/or multiple divisions of dialogue based on chapters
    List<List<Dialogue>> mAllDialogueChunks;

    // Indicates which chunk of dialogue is being read from
    int mCurrentDialogueChunkIndex;
	List<Dialogue> currentDialogueChunk;

	// Indicates which piece of dialogue in a chunk should be said
	int mCurrentDialogueIndex;
	Dialogue currentDialogue;

	// Relative path to the dialogue file
	const string mDialogueFileName = "Dialogue";

    // Used to keep track what char needs to be output for the dialogue
    int currentCharIndexForDialogue;

    // Used to notify Update when to display dialogue
    bool doDialogue;

    // Used to take every nth frame off for display text
    int currentNumberOfFrames;

	// How many frames that need to be passed for text crawl
	public float numberOfFramesNeeded = 2.5f;

	// List of all the people who do dialogue
	Person[] mPeople = new Person[]
    {
        new Person("Daughter", new Color(255 / 255.0f, 255 / 255.0f, 0 / 255.0f)),
        new Person("Dad", new Color(0 / 255.0f, 102 / 255.0f, 255 / 255.0f)),
        new Person("Princess", new Color(255 / 255.0f, 51 / 255.0f, 153 / 255.0f)),
        new Person("Knight", new Color(255 / 255.0f, 0 / 255.0f, 0 / 255.0f)),
    };

	// Index map for people
	Dictionary<string, int> personDictionary;

	// Text label to be filled up with dialogue
	Text nameText;

	// Text label to be filled up with dialogue
	Text dialogueText;

    // Canvas containing all the dialogue HUD
    Canvas canvas;

	// The stamp on the name label has a different color for different people
	public Sprite[] nameLabels;
	Image nameLabel;

	// I'm not going to use this currently as I would need 2D art
	// Instead, I'm doing a name instead
	// Image of the person's avatar
	Image avatar;

	// Image for input prompt
	Image inputPrompt;

    // Audio source that is used for mumbling sound
    AudioSource audioSource;

    // The voices of every person
    [SerializeField]
    List<AudioClip> mumblingClips = null;

    // TODO: Check that this is disabled for real bbuild
    [SerializeField]
    bool skipDialogue = true;

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
		// Initialize variables
		mCurrentDialogueChunkIndex = 0;
		mCurrentDialogueIndex = 0;
        currentCharIndexForDialogue = 0;
        currentNumberOfFrames = 0;
        doDialogue = false;

        // Initialize all dialogue list
        mAllDialogueChunks = new List<List<Dialogue>>();

        // Create dictionary for quick access to the respective person
        personDictionary = new Dictionary<string, int>();

        for (int i = 0; i < mPeople.Length; ++i)
        {
            personDictionary.Add(mPeople[i].GetName(), i);
        }

        // Load txt file into Unity Text Asset then convert to string array
		TextAsset textAsset = (TextAsset)Resources.Load(mDialogueFileName, typeof(TextAsset));
		string [] splitCharacters = new string[] {"\r\n", "\r", "\n"};

		string[] allText = textAsset.text.Split(splitCharacters, System.StringSplitOptions.None);
		int currentTextIndex = 0;

		// Transfer the dialogue to the list of dialogue
		List <Dialogue> dialogueChunk = new List<Dialogue>();

		while (true)
        {
			string currentLine = allText[currentTextIndex++];

			// Is this the end of the current dialogue chunk?
			if (currentLine[0] == '/' && currentLine[1] == '/')
			{
				// Add chunk and clear it for next chunk to be filled
				mAllDialogueChunks.Add(new List<Dialogue>(dialogueChunk));
				
				dialogueChunk.Clear();

				// Get rid of blank line after comment
				++currentTextIndex;
			}
			else
			{
				// Find out who said the dialogue
				string personName = currentLine;
				string personsWords = allText[currentTextIndex++];
				int whichPerson = personDictionary[personName];

				// Set the person
				Person person = mPeople[whichPerson];

				// Construct dialogue
				Dialogue dialogue = new Dialogue(person, personsWords);

				// Add to current chunk of dialogue
				dialogueChunk.Add(dialogue);

				// Every dialogue has an empty line after it so clear that
				++currentTextIndex;
			}

			// Check if everything has been read
			if (allText.Length == currentTextIndex)
			{
				break;
			}
		}

        // Get disabled canvas
        canvas = GetComponentInChildren<Canvas>(true);

		// Set text labels
		nameText = canvas.GetComponentsInChildren<Text>()[0];
		dialogueText = canvas.GetComponentsInChildren<Text>()[1];
		//dialogueText = canvas.GetComponentsInChildren<Text>()[0];

		// Set avatar and input prompt
		Image[] images = canvas.GetComponentsInChildren<Image>();

		// images[0] is the background so offset by 1
		nameLabel = images[1];
		avatar = images[2];
		inputPrompt = images[3];

        // Disable input prompt image
        inputPrompt.enabled = false;

        // Set audio source
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // Only do if it's been told to
        if (doDialogue)
        {
            // Play the talking sound until all the words have been displayed
            if (!audioSource.isPlaying)
            {
                audioSource.pitch = Random.Range(0.75f, 1.25f);
                audioSource.Play();
            }

			// Only do if all the text isn't there
			if (dialogueText.text.Length != currentDialogue.GetWords().Length)
            {
				// Is it the nth frame?
				if (currentNumberOfFrames++ >= numberOfFramesNeeded * currentCharIndexForDialogue)
				{
					dialogueText.text += currentDialogue.GetWords()[currentCharIndexForDialogue];

                    ++currentCharIndexForDialogue;
                }
            }
            else
            {
                // Enable input prompt image
                inputPrompt.enabled = true;
				inputPrompt.GetComponent<Animator>().SetBool("Bounce", true);

                // Stop dialog from updating
                doDialogue = false;

                // Update overall dialogue index
                ++mCurrentDialogueIndex;

                // Have we done all the dialogue in the chunk
                if (mCurrentDialogueIndex < currentDialogueChunk.Count)
                {
                    // Then subscribe to input manager for left mouse click event using NextDialogue
                    InputManager.OnLeftMouseReleased += NextDialogue;
					InputManager.OnReturnUp += NextDialogue;
					InputManager.OnSpaceUp += NextDialogue;
				}
                else
                {
                    InputManager.OnLeftMouseReleased += EndDialogue;
					InputManager.OnReturnUp += EndDialogue;
					InputManager.OnSpaceUp += EndDialogue;
				}
            }

        }
    }

    public void StartDialogue()
    {
        // TODO: Do cool animation to bring up canvas

        // Set enabled so it is visible
        //canvas.enabled = true;
		canvas.gameObject.SetActive(true);

		// Init some variables needed
		DialogueInitialization();
    }

    // Start the next piece of dialogue
    public void NextDialogue()
    {
        // Unsubscribe from event
        InputManager.OnLeftMouseReleased -= NextDialogue;
		InputManager.OnReturnUp -= NextDialogue;
		InputManager.OnSpaceUp -= NextDialogue;

		// Reset char index
		currentCharIndexForDialogue = 0;

        // Init some variables needed
        DialogueInitialization();
    }

    void EndDialogue()
    {
        // TODO: Do cool animation to put away canvas

        // Unsubscribe from event
        InputManager.OnLeftMouseReleased -= EndDialogue;
		InputManager.OnReturnUp -= EndDialogue;
		InputManager.OnSpaceUp -= EndDialogue;

		// Set enabled so it isn't visible
		//canvas.enabled = false;
		canvas.gameObject.SetActive(false);

		// Clear text. In the future, clearing at the beginning of start could work better depending on HUD animations
		dialogueText.text = "";

        // Disable input prompt image
        inputPrompt.enabled = false;

		// Increment chunk index
		++mCurrentDialogueChunkIndex;

		// Reinitialize variables
		doDialogue = false;
        currentCharIndexForDialogue = 0;
		mCurrentDialogueIndex = 0;

		// Tell event manager all the dialogue is done
		EventManager.instance.MarkEventAsDone();
    }

    // Some basic initialization that both StartDialogue and NextDialogue use
    void DialogueInitialization()
    {
		// Set current dialogue variables
		currentDialogueChunk = mAllDialogueChunks[mCurrentDialogueChunkIndex];
		currentDialogue = currentDialogueChunk[mCurrentDialogueIndex];

		// Disable input prompt image
		inputPrompt.enabled = false;
		inputPrompt.GetComponent<Animator>().SetBool("Bounce", false);

		// Change name label's sprite
		nameLabel.sprite = nameLabels[personDictionary[currentDialogue.GetPerson().GetName()]];

		// Clear text
		dialogueText.text = "";

		// Reset frames
		currentNumberOfFrames = 0;

		// Set name text label
		nameText.text = currentDialogue.GetPerson().GetName();

		// Set audio clip to the correct person
		audioSource.clip = mumblingClips[personDictionary[currentDialogue.GetPerson().GetName()]];

        // Play the audio
        audioSource.Play();

        // Spit out dialogue time
        doDialogue = true;

        // Development tool to skip dialogue
        if (skipDialogue)
        {
			dialogueText.text = currentDialogue.GetWords();
            currentCharIndexForDialogue = dialogueText.text.Length;
        }
    }

	// Reset dialogue manager to initial state
	public void ResetDialogueManager()
	{
		// Just in case they were in the middle of dialogue and quit
		EndDialogue();

		// Reset back to first dialogue
		mCurrentDialogueChunkIndex = 0;
		mCurrentDialogueIndex = 0;
	}
}