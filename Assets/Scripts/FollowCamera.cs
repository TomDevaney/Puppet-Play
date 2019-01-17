using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{

    public Puppet PlayerPuppet;

    public float LevelBoundLeft;

    public float LevelBoundRight;

    public float TargetY;


    // Start is called before the first frame update
    void Start()
    {
        PlayerPuppet = GameObject.Find("Player").GetComponent<Puppet>();
    }

    // Update is called once per frame
    void Update()
    {
        float nextX = LevelBoundLeft;


        nextX = Mathf.Lerp(transform.position.x, PlayerPuppet.transform.position.x, Time.deltaTime);
        transform.position = new Vector3(nextX, TargetY, transform.position.z);
    }

    public void OnRespawn()
    {
        PlayerPuppet = GameObject.Find("Player").GetComponent<Puppet>();
        transform.position = new Vector3(PlayerPuppet.transform.position.x, TargetY, transform.position.z);
    }
}
