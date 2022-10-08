using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    //Class to control player movement.

    [SerializeField] private PauseMenu menu;
    [SerializeField] private CharacterController controller;
    [SerializeField] private float movementSpeed = 10f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private Transform feet;
    [SerializeField] private float groundCheckRadius = 0.4f;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float jumpForce;
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

        if (menu.actived == false)
        {
            inputVector.x = Input.GetAxis("Horizontal");
            inputVector.z = Input.GetAxis("Vertical");

            moveVector = transform.right * inputVector.x + transform.forward * inputVector.z;
            controller.Move(moveVector * movementSpeed * Time.deltaTime);

            //Different controls for flying and grounded movement
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
}
