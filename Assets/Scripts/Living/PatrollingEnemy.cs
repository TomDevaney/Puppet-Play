using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrollingEnemy : Enemy
{
    // Where the enemy will go back to after end position
    [SerializeField]
    float startPosition = 0.0f;

    // Where the enemy will go to after start position
    [SerializeField]
    float endPosition = 0.0f;

    // Whether start and end position are relative or absolute
    [SerializeField]
    bool doRelativePositioning = false;

    // Final positions (takes account into relative)
    float finalStartPosition;
    float finalEndPosition;

    // If true, it will go to the end position. If false, it will go to the false position
    bool goToEnd;

	// Lazy hack to have a stationary enemy
	bool dontMove;

    // Start is called before the first frame update
    public override void Start()
    {
        // Initialize parent variables
        base.Start();

		doFacing = false;
		SetMoveSpeed(0.04f);
        SetAttackDamage(1);

        // Initialize variables
        goToEnd = true;

        // Make final positions
        if (doRelativePositioning)
        {
            finalStartPosition = transform.position.x + startPosition;
            finalEndPosition = transform.position.x + endPosition;
        }
        else
        {
            finalStartPosition = startPosition;
            finalEndPosition = endPosition;
        }

		if (startPosition == endPosition)
		{
			dontMove = true;
		}
    }

    // Update is called once per frame
    public override void Update()
    {
		base.Update();

		if (dontMove)
		{
			return;
		}

        // -1 = go left. 1 = go right
        int direction;

        // Find out which direction to go in
        if (goToEnd)
        {
            // End position is to the right
            if (finalEndPosition - transform.position.x > 0)
            {
                direction = 1;
            }
            else
            {
                direction = -1;
            }
        }
        else
        {
            // startPosition position is to the right
            if (finalStartPosition - transform.position.x > 0)
            {
                direction = 1;
            }
            else
            {
                direction = -1;
            }
        }

        // Apply movement in the desired direction
        Move(direction);

        // TODO: I don't think this takes into consideration endPosition being on the left

        // Is final end position to the right?
        if (finalEndPosition > finalStartPosition)
        {
            // You passed the end Position
            if (transform.position.x >= finalEndPosition)
            {
                goToEnd = false;
            }
            else if (transform.position.x <= finalStartPosition)
            {
                goToEnd = true;
            }
        }
        else
        {
            // You passed the end Position
            if (transform.position.x <= finalEndPosition)
            {
                goToEnd = false;
            }
            else if (transform.position.x >= finalStartPosition)
            {
                goToEnd = true;
            }
        }
    }

    public override void Respawn()
    {
        base.Respawn();

        // Reinitialize parent variables
        SetHealthPoints(1);
    }
}
