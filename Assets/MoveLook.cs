using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLook : MonoBehaviour
{
    public float mouseSensitivity = 150f;
    private Vector2 angle = new Vector2();
    public Transform playerTransform;
    private float xRotation = 0f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        angle.x = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        angle.y = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        xRotation -= angle.y;
        if(xRotation >= 90) { xRotation = 90f; }
        if (xRotation <= -90) { xRotation = -90f; }
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerTransform.Rotate(Vector3.up * angle.x);

    }
}
