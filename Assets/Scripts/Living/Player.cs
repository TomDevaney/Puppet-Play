using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.instance.CanPlayerMove())
        {
            float xAxis = Input.GetAxis("Horizontal");

            //transform.Translate(new Vector2(xAxis * 5.0f, 0.0f));
            transform.Translate(new Vector2(xAxis * 0.01f, 0.0f));
        }
    }
}
