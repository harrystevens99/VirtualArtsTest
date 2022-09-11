using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public CharacterController controller;
    public float MovementSpeed = 10f;
    public float gravity = -9.81f;
    public Transform feet;
    public float groundCheckRadius = 0.4f;
    public LayerMask ground;
    public float jumpForce;

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
        grounded = Physics.CheckSphere(feet.position, groundCheckRadius, ground);

        if(grounded == true && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        inputVector.x = Input.GetAxis("Horizontal");
        inputVector.z = Input.GetAxis("Vertical");

        moveVector = transform.right * inputVector.x + transform.forward * inputVector.z;

        controller.Move(moveVector * MovementSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && grounded == true)
        {
            velocity.y = jumpForce;
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }
}
