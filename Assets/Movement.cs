using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public CharacterController controller;
    public float MovementSpeed = 10f;
    private Vector3 inputVector = new Vector3();
    //??
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

        controller.Move(moveVector * MovementSpeed * Time.deltaTime);
    }
}
