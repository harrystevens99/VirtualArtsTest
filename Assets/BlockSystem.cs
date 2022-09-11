using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSystem : MonoBehaviour
{
    public MoveLook mainCamera;
    public GameObject test;
    private Vector3 cubePoint = new Vector3();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, 5))
        {
            cubePoint.x = Mathf.RoundToInt(hit.point.x);
            cubePoint.y = Mathf.RoundToInt(hit.point.y);
            cubePoint.z = Mathf.RoundToInt(hit.point.z);
            test.transform.position = cubePoint;
        }
        else
        {
            test.transform.position = new Vector3(1000, 1000, 1000);
        }
    }
}
