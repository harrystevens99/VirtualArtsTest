using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSystem : MonoBehaviour
{
    public MoveLook mainCamera;
    public PauseMenu menu;
    public GameObject wireFrameCube;
    public UI ui;
    public LayerMask breakAble;
    public ParticleSystem particles;
    public Color paintColour = new Color(1, 0, 0, 1);
    public int activeBlocks = 0;
    public int totalBlocks;
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
        totalBlocks = blocks.Length;
    }

    // Update is called once per frame
    void Update()
    {
        if (menu.actived == false)
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
                                    activeBlocks++;
                                    block.transform.gameObject.GetComponent<Renderer>().enabled = true;
                                    block.transform.gameObject.GetComponent<BoxCollider>().enabled = true;
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

                        hit.transform.gameObject.GetComponent<Renderer>().material.color = paintColour;
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
                        hit.transform.gameObject.GetComponent<Renderer>().enabled = false;
                        hit.transform.gameObject.GetComponent<BoxCollider>().enabled = false;
                        hit.transform.position = new Vector3(1000, 1000, 1000);
                        activeBlocks--;
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
}
