using System.Collections;
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

public class DialogueManager : MonoBehaviour
{
    // Singleton so classes can reference without reference
    public static DialogueManager instance = null;

    // Every piece of dialogue needed for the game
    // If it ever became too big, or too slow to load in all the dialogue at once, there could be multiple dimensions and/or multiple divisions of dialogue based on chapters
    List<Dialogue> mAllDialogue;

    // Indicates which dialogue to currently display
    int mCurrentDialogueIndex;

    // Relative path to the dialogue file
    const string mDialogueFilePath = "/Files/Test Dialogue.txt";

    // Used to display multiple dialogues per one event call
    int numberOfDialoguesToDisplay;

    // Used to keep track of number of dialogues displayed per one event call
    int numberOfDialoguesThatHaveBeenDisplayed;

    // Used to keep track what char needs to be output for the dialogue
    int currentCharIndexForDialogue;

    // Used to notify Update when to display dialogue
    bool doDialogue;

    // Used to take every nth frame off for display text
    int everyNthFrame;

    // List of all the people who do dialogue
    Person[] mPeople = new Person[]
    {
        new Person("Daughter", new Color(255 / 255.0f, 255 / 255.0f, 0 / 255.0f)),
        new Person("Dad", new Color(0 / 255.0f, 102 / 255.0f, 255 / 255.0f)),
        new Person("Princess", new Color(255 / 255.0f, 51 / 255.0f, 153 / 255.0f)),
        new Person("Knight", new Color(255 / 255.0f, 0 / 255.0f, 0 / 255.0f)),
    };

    // Text label to be filled up with dialogue
    Text text;

    // Canvas containing all the dialogue HUD
    Canvas canvas;

    // Image of the person's avatar
    Image avatar;

    // Image for input prompt
    Image inputPrompt;

    // Audio source that is used for mumbling sound
    AudioSource audioSource;

    // The voices of every person
    [SerializeField]
    List<AudioClip> mumblingClips = null;

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
        mCurrentDialogueIndex = 0;
        numberOfDialoguesToDisplay = 0;
        currentCharIndexForDialogue = 0;
        numberOfDialoguesThatHaveBeenDisplayed = 0;
        everyNthFrame = 0;
        doDialogue = false;

        // Initialize all dialogue list
        mAllDialogue = new List<Dialogue>();

        // Create dictionary for quick access to the respective person
        Dictionary<string, int> personDictionary = new Dictionary<string, int>();

        for (int i = 0; i < mPeople.Length; ++i)
        {
            personDictionary.Add(mPeople[i].GetName(), i);
        }

        // Set up stream reader to dialogue text file
        StreamReader reader = new StreamReader(Application.dataPath + mDialogueFilePath);

        // TODO: I think this isn't needed anymore... need to add something to tell dialogue when to stop. I think the actions will be useful for that.

        // TODO: See how long this file reading takes

        // Transfer the dialogue to the list of dialogue
        while (true)
        {
            // Find out who said the dialogue
            string personName = reader.ReadLine();
            string personsWords = reader.ReadLine();
            int whichPerson = personDictionary[personName];

            // Set the person
            Person person = mPeople[whichPerson];

            // Construct dialogue
            Dialogue dialogue = new Dialogue(person, personsWords);

            // Add to list of all dialogue
            mAllDialogue.Add(dialogue);

            // Every dialogue has an empty line so clear that
            reader.ReadLine();

            // Check if everything has been read
            if (reader.EndOfStream)
            {
                break;
            }
        }

        // Close stream reader given we're done reading
        reader.Close();

        // Set canvas
        canvas = GetComponentInChildren<Canvas>();
        canvas.enabled = false;

        // Set text label
        text = GetComponentInChildren<Text>();

        // Set avatar and input prompt
        Image[] images = GetComponentsInChildren<Image>();

        avatar = images[1];
        inputPrompt = images[2];

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
            // Number of frames til text can be added
            const int nFrames = 3;

            // Only do if all the text isn't there
            if (text.text.Length != mAllDialogue[mCurrentDialogueIndex].GetWords().Length)
            {
                // Is it the nth frame?
                if (everyNthFrame++ % nFrames == 0)
                {
                    text.text += mAllDialogue[mCurrentDialogueIndex].GetWords()[currentCharIndexForDialogue];

                    ++currentCharIndexForDialogue;
                }
            }
            else
            {
                // Only do if we haven't met requested number of dialogues
                if (numberOfDialoguesThatHaveBeenDisplayed + 1 < numberOfDialoguesToDisplay)
                {
                    // Then subscribe to input manager for left mouse click event using NextDialogue
                    InputManager.OnLeftMouseReleased += NextDialogue;

                    // Stop dialog from updating
                    doDialogue = false;
                }
                else
                {
                    InputManager.OnLeftMouseReleased += EndDialogue;
                }

                // Enable input prompt image
                inputPrompt.enabled = true;

                // Stop the mumbling
                audioSource.Stop();
            }

        }
    }

    public void StartDialogue(int numDialogues)
    {
        // Init some variables needed
        DialogueInitialization();

        // Store number of dialogues
        numberOfDialoguesToDisplay = numDialogues;

        // TODO: Do cool animation to bring up canvas

        // Set enabled so it is visible
        canvas.enabled = true;
    }

    // Start the next piece of dialogue
    public void NextDialogue()
    {
        // Init some variables needed
        DialogueInitialization();

        // Update number of dialogues that have been displayed in a row
        ++numberOfDialoguesThatHaveBeenDisplayed;

        // Update overall dialogue index
        ++mCurrentDialogueIndex;

        // Unsubscribe from event
        InputManager.OnLeftMouseReleased -= NextDialogue;

        // Reset char index
        currentCharIndexForDialogue = 0;
    }

    void EndDialogue()
    {
        // TODO: Do cool animation to put away canvas

        // Set enabled so it isn't visible
        canvas.enabled = false;

        // Clear text. In the future, clearing at the beginning of start could work better depending on HUD animations
        text.text = "";

        // Disable input prompt image
        inputPrompt.enabled = false;

        // Reinitialize variables
        doDialogue = false;
        numberOfDialoguesToDisplay = 0;
        currentCharIndexForDialogue = 0;
        numberOfDialoguesThatHaveBeenDisplayed = 0;

        // Tell event manager all the dialogue is done
        EventManager.instance.MarkEventAsDone();
    }

    // Some basic initialization that both StartDialogue and NextDialogue use
    void DialogueInitialization()
    {
        // Disable input prompt image
        inputPrompt.enabled = false;

        // Clear text
        text.text = "";

        // Set text to the respective color
        text.color = mAllDialogue[mCurrentDialogueIndex].GetPerson().GetColor();

        // Set audio clip to the correct person
        audioSource.clip = mumblingClips[0];

        // Play the audio
        audioSource.Play();

        // TODO: Check if it's a change in person. If so, change Avatar and whatever else needs to be changed. Maybe animation.

        // Spit out dialogue time
        doDialogue = true;
    }
}