using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLook : MonoBehaviour
{
    //Class to control camera movement.

    public float mouseSensitivity = 150f;
    private Vector2 angle = new Vector2();
    public Transform playerTransform;
    public PauseMenu menu;
    private float xRotation = 0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (menu.actived == false)
        {
            angle.x = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            angle.y = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
            xRotation -= angle.y;
            if (xRotation >= 90) { xRotation = 90f; }
            if (xRotation <= -90) { xRotation = -90f; }
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            playerTransform.Rotate(Vector3.up * angle.x);
        }
    }
}
