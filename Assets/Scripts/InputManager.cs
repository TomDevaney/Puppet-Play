using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    // Singleton so classes can reference without reference
    public static InputManager instance = null;

    // I am just using a delegate event to save some speed. I don't need this in editor as of right now
    // If I wanted in editor, change this to UnityEvent to sacrifice some speed
    // Event for left mouse released
    public delegate void LeftMouseReleased();
    public static event LeftMouseReleased OnLeftMouseReleased;

    //
    bool disablePlayerActions;

    // 
    bool canPlayerMove;

    //
    bool canPlayerJump;

    //
    bool canPlayerAttack;

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

    void Start()
    {
        // Initialize variables
        canPlayerMove = true;
        canPlayerJump = true;
        canPlayerAttack = true;
        disablePlayerActions = false;
    }

    void Update()
    {
        // Handle left mouse released event
        if (OnLeftMouseReleased != null)
        {
            if (Input.GetMouseButtonUp(0))
            {
                OnLeftMouseReleased();
            }
        }
    }

    public void DisablePlayerActions()
    {
        canPlayerAttack = false;
        canPlayerJump = false;
        canPlayerMove = false;
    }

    public void EnablePlayerActions()
    {
        canPlayerAttack = true;
        canPlayerJump = true;
        canPlayerMove = true;
    }

    /* Getters */
    public bool CanPlayerMove()
    {
        return canPlayerMove;
    }

    public bool CanPlayerJump()
    {
        return canPlayerJump;
    }

    public bool CanPlayerAttack()
    {
        return canPlayerAttack;
    }

    /* Setters */
    public void SetCanPlayerMove(bool canMove)
    {
        canPlayerMove = canMove;
    }
}
