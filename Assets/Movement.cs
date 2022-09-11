using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public CharacterController controller;
    public float movementSpeed = 10f;
    public float gravity = -9.81f;
    public Transform feet;
    public float groundCheckRadius = 0.4f;
    public LayerMask whatIsGround;
    public float jumpForce;
    public bool flying;
    private int flyDir = 0;
    private Vector3 flyVector = new Vector3();
    private bool grounded;
    private Vector3 inputVector = new Vector3();
    private Vector3 velocity = new Vector3();
    private Vector3 moveVector = new Vector3();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {


        inputVector.x = Input.GetAxis("Horizontal");
        inputVector.z = Input.GetAxis("Vertical");

        moveVector = transform.right * inputVector.x + transform.forward * inputVector.z;
        controller.Move(moveVector * movementSpeed * Time.deltaTime);

        if (flying == false)
        {
            grounded = Physics.CheckSphere(feet.position, groundCheckRadius, whatIsGround);

            if (grounded == true && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            if (Input.GetKeyDown("space") && grounded == true)
            {
                velocity.y = jumpForce;
            }

            velocity.y += gravity * Time.deltaTime;

            
        }
        else
        {
            if (Input.GetKey("space"))
            {
                flyDir = 1;
            }
            else if (Input.GetKey("left shift"))
            {
                flyDir = -1;
            }
            else
            {
                flyDir = 0;
            }

            flyVector = transform.up * flyDir;
            velocity = flyVector * movementSpeed;

        }


        controller.Move(velocity * Time.deltaTime);
    }
}
