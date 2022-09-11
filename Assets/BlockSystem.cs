using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSystem : MonoBehaviour
{
    public MoveLook mainCamera;
    public GameObject wireFrameCube;
    public UI ui;
    public LayerMask breakAble;
    public ParticleSystem particles;
    private bool canPlace;
    private bool placeCheck = true;
    private bool canBreak;
    private bool breakCheck = true;
    private Vector3 cubePoint = new Vector3();
    private GameObject[] blocks;
    private RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {
        blocks = GameObject.FindGameObjectsWithTag("Block");
    }

    // Update is called once per frame
    void Update()
    {
        switch (ui.mode)
        {
            case 0:
                if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, 5))
                {
                    cubePoint.x = Mathf.RoundToInt(hit.point.x);
                    cubePoint.y = Mathf.RoundToInt(hit.point.y);
                    cubePoint.z = Mathf.RoundToInt(hit.point.z);
                    wireFrameCube.transform.position = cubePoint;
                    canPlace = true;
                }
                else
                {
                    wireFrameCube.transform.position = new Vector3(1000, 1000, 1000);
                    canPlace = false;
                }

                if (canPlace == true && Input.GetMouseButtonDown(0) && placeCheck == true)
                {
                    foreach (GameObject block in blocks)
                    {
                        if (block.GetComponent<Block>() != null)
                        {
                            if (block.GetComponent<Block>().placed == false && placeCheck == true)
                            {
                                block.transform.position = cubePoint;
                                block.GetComponent<Block>().placed = true;
                                placeCheck = false;
                            }
                        }
                    }
                }
                break;
            case 1:
                if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, 5, breakAble))
                {
                    cubePoint = hit.transform.position;
                    wireFrameCube.transform.position = cubePoint;
                    canBreak = true;
                }
                else
                {
                    wireFrameCube.transform.position = new Vector3(1000, 1000, 1000);
                    canBreak = false;
                }

                if (canBreak == true && Input.GetMouseButtonDown(0) && breakCheck == true)
                {

                    hit.transform.gameObject.GetComponent<Renderer>().material.color = new Color(1, 0, 0);
                    breakCheck = false;
                }
                break;
            case 2:
                if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, 5, breakAble))
                {
                    cubePoint = hit.transform.position;
                    wireFrameCube.transform.position = cubePoint;
                    canBreak = true;
                }
                else
                {
                    wireFrameCube.transform.position = new Vector3(1000, 1000, 1000);
                    canBreak = false;
                }

                if (canBreak == true && Input.GetMouseButtonDown(0) && breakCheck == true)
                {
                    particles.transform.position = cubePoint;
                    particles.startColor = hit.transform.gameObject.GetComponent<Renderer>().material.color;
                    particles.Play();
                    hit.transform.gameObject.GetComponent<Renderer>().material.color = new Color(1, 1, 1);
                    hit.transform.gameObject.GetComponent<Block>().placed = false;
                    hit.transform.position = new Vector3(1000, 1000, 1000);
                    breakCheck = false;
                }
                break;
        } 


        if (Input.GetMouseButtonUp(0))
        {
            placeCheck = true;
            breakCheck = true;
        }
    }
}
