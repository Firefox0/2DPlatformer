using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public PlayerController controller;
    // -1 left; 0 nothing; 1 right
    float horizontal_direction = 0f;
    public float run_speed = 100f;
    bool jump = false;

    void Start()
    {
        
    }

    void Update()
    {
        horizontal_direction = Input.GetAxis("Horizontal");
        
        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }
    }

    void FixedUpdate()
    {
        controller.Move(horizontal_direction * run_speed * Time.fixedDeltaTime, false, jump);
        jump = false;
    }
}
