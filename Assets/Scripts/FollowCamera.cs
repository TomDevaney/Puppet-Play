using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{

    public Puppet PlayerPuppet;

    public float LevelBoundLeft;
    public float LevelBoundRight;
	public float levelBoundUp;
	public float levelBoundDown;

	// Start is called before the first frame update
	void Start()
    {
        PlayerPuppet = GameObject.Find("Player").GetComponent<Puppet>();
    }

    // Update is called once per frame
    void Update()
    {
        float TargetX = LevelBoundLeft;
        if(PlayerPuppet.transform.position.x > LevelBoundRight)
        {
            TargetX = LevelBoundRight;
        }
        else if(PlayerPuppet.transform.position.x < LevelBoundLeft)
        {
            TargetX = LevelBoundLeft;
        }
        else
        {
            TargetX = PlayerPuppet.transform.position.x;
        }

		float targetY = 0;
		if (PlayerPuppet.transform.position.y > levelBoundUp)
		{
			targetY = levelBoundUp;
		}
		else if (PlayerPuppet.transform.position.y < levelBoundDown)
		{
			targetY = levelBoundDown;
		}
		else
		{
			targetY = PlayerPuppet.transform.position.y;
		}

		float nextX = Mathf.Lerp(transform.position.x, TargetX , Time.deltaTime);
        float nextY = Mathf.Lerp(transform.position.y, targetY, Time.deltaTime);
		//float nextY = Mathf.Lerp(transform.position.y, PlayerPuppet.transform.position.y + yOffset, Time.deltaTime);

		transform.position = new Vector3(nextX, nextY, transform.position.z);
    }

    public void OnRespawn()
    {
        PlayerPuppet = GameObject.Find("Player").GetComponent<Puppet>();
        transform.position = new Vector3(PlayerPuppet.transform.position.x, PlayerPuppet.transform.position.y, transform.position.z);
    }
}
