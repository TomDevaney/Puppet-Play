using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Windows;
using UnityEngine;


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
    // Every piece of dialogue needed for the game
    // If it ever became too big, or to slow to load in all the dialogue at once, there could be multiple dimensions and/or multiple divisions of dialogue based on chapters
    List<Dialogue> mAllDialogue;

    // Indicates which dialogue to currently display
    int mCurrentDialogueIndex;

    // Relative path to the dialogue file
    const string mDialogueFilePath = "/Files/Test Dialogue.txt";

    // List of all the people who do dialogue
    Person[] mPeople = new Person[]
    {
        new Person("Daughter", new Color(255 / 255.0f, 255 / 255.0f, 0 / 255.0f)),
        new Person("Dad", new Color(0 / 255.0f, 102 / 255.0f, 255 / 255.0f)),
        new Person("Princess", new Color(255 / 255.0f, 51 / 255.0f, 153 / 255.0f)),
        new Person("Knight", new Color(255 / 255.0f, 0 / 255.0f, 0 / 255.0f)),
    };

    // Start is called before the first frame update
    void Start()
    {
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

        // TODO: need to add something to tell dialogue when to stop. I think the actions will be useful for that.

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
    }

    // Update is called once per frame
    void Update()
    {
        // Start displaying the text to the screen one character at a time
        // Todo: just set color one time in the function that determines index of dialogue
        //GUIText text = FindObjectOfType<GUIText>();
        //text.color = mAllDialogue[mCurrentDialogueIndex].GetPerson().GetColor();
    }

    public void 
}