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
    public LayerMask placeable;
    public LayerMask orbs;
    public ParticleSystem particles;
    public Color paintColour = new Color(1, 0, 0, 1);
    public int activeBlocks = 0;
    public int totalBlocks;
    public Renderer orbUp;
    public Renderer orbDown;
    public Renderer orbLeft;
    public Renderer orbRight;
    public Renderer orbForward;
    public Renderer orbBack;
    public bool moving;
    private bool canPlace;
    private bool placeCheck = true;
    private bool canBreak;
    private bool breakCheck = true;
    private Vector3 cubePoint = new Vector3();
    public GameObject[] blocks;
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
                    if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, 5, placeable))
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
                case 3:
                    if (moving == false)
                    {
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

                            orbUp.transform.position = new Vector3(hit.transform.position.x, hit.transform.position.y + 2f, hit.transform.position.z);
                            orbDown.transform.position = new Vector3(hit.transform.position.x, hit.transform.position.y - 2f, hit.transform.position.z);
                            orbLeft.transform.position = new Vector3(hit.transform.position.x - 2f, hit.transform.position.y, hit.transform.position.z);
                            orbRight.transform.position = new Vector3(hit.transform.position.x + 2f, hit.transform.position.y, hit.transform.position.z);
                            orbForward.transform.position = new Vector3(hit.transform.position.x, hit.transform.position.y, hit.transform.position.z + 2f);
                            orbBack.transform.position = new Vector3(hit.transform.position.x, hit.transform.position.y, hit.transform.position.z - 2f);

                            foreach (GameObject block in blocks)
                            {
                                if (block.GetComponent<Block>() != null)
                                {
                                    if (block.GetComponent<Block>().placed == true && block.GetComponent<Block>().selected == true)
                                    {
                                        block.GetComponent<Renderer>().material.SetFloat("_Metallic", 0f);
                                        block.GetComponent<Block>().selected = false;
                                    }
                                }
                            }

                            moving = true;

                            FindCubes(hit.transform);

                            foreach (GameObject block in blocks)
                            {
                                if (block.GetComponent<Block>() != null)
                                {
                                    if (block.GetComponent<Block>().placed == true && block.GetComponent<Block>().selected == true)
                                    {
                                        if (CheckDirection(block.transform, orbUp.gameObject.GetComponent<Orb>().direction))
                                        {
                                            orbUp.gameObject.transform.position = new Vector3(1000, 1000, 1000);
                                        }
                                        if (CheckDirection(block.transform, orbDown.gameObject.GetComponent<Orb>().direction))
                                        {
                                            orbDown.gameObject.transform.position = new Vector3(1000, 1000, 1000);
                                        }
                                        if (CheckDirection(block.transform, orbLeft.gameObject.GetComponent<Orb>().direction))
                                        {
                                            orbLeft.gameObject.transform.position = new Vector3(1000, 1000, 1000);
                                        }
                                        if (CheckDirection(block.transform, orbRight.gameObject.GetComponent<Orb>().direction))
                                        {
                                            orbRight.gameObject.transform.position = new Vector3(1000, 1000, 1000);
                                        }
                                        if (CheckDirection(block.transform, orbForward.gameObject.GetComponent<Orb>().direction))
                                        {
                                            orbForward.gameObject.transform.position = new Vector3(1000, 1000, 1000);
                                        }
                                        if (CheckDirection(block.transform, orbBack.gameObject.GetComponent<Orb>().direction))
                                        {
                                            orbBack.gameObject.transform.position = new Vector3(1000, 1000, 1000);
                                        }

                                        block.GetComponent<Renderer>().material.SetFloat("_Metallic", 1f);
                                    }
                                }
                            }
                            breakCheck = false;
                        }
                    }
                    else
                    {
                        orbUp.material.color = new Color(1, 0, 1, 1);
                        orbDown.material.color = new Color(1, 0, 1, 1);
                        orbLeft.material.color = new Color(1, 0, 1, 1);
                        orbRight.material.color = new Color(1, 0, 1, 1);
                        orbForward.material.color = new Color(1, 0, 1, 1);
                        orbBack.material.color = new Color(1, 0, 1, 1);

                        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, 100, orbs))
                        {
                            hit.transform.gameObject.GetComponent<Renderer>().material.color = new Color(0, 1, 1, 1);
                            canBreak = true;
                        }
                        else
                        {
                            canBreak = false;
                        }

                        if (canBreak == true && Input.GetMouseButtonDown(0) && breakCheck == true)
                        {
                            foreach (GameObject block in blocks)
                            {
                                if (block.GetComponent<Block>() != null)
                                {
                                    if (block.GetComponent<Block>().placed == true && block.GetComponent<Block>().selected == true)
                                    {
                                        block.gameObject.transform.position = block.gameObject.transform.position + hit.transform.gameObject.GetComponent<Orb>().direction;
                                    }
                                }
                            }
                            breakCheck = false;
                            wireFrameCube.transform.position = wireFrameCube.transform.position + hit.transform.gameObject.GetComponent<Orb>().direction;
                            orbUp.transform.position = new Vector3(wireFrameCube.transform.position.x, wireFrameCube.transform.position.y + 2f, wireFrameCube.transform.position.z);
                            orbDown.transform.position = new Vector3(wireFrameCube.transform.position.x, wireFrameCube.transform.position.y - 2f, wireFrameCube.transform.position.z);
                            orbLeft.transform.position = new Vector3(wireFrameCube.transform.position.x - 2f, wireFrameCube.transform.position.y, wireFrameCube.transform.position.z);
                            orbRight.transform.position = new Vector3(wireFrameCube.transform.position.x + 2f, wireFrameCube.transform.position.y, wireFrameCube.transform.position.z);
                            orbForward.transform.position = new Vector3(wireFrameCube.transform.position.x, wireFrameCube.transform.position.y, wireFrameCube.transform.position.z + 2f);
                            orbBack.transform.position = new Vector3(wireFrameCube.transform.position.x, wireFrameCube.transform.position.y, wireFrameCube.transform.position.z - 2f);

                            foreach (GameObject block in blocks)
                            {
                                if (block.GetComponent<Block>() != null)
                                {
                                    if (block.GetComponent<Block>().placed == true && block.GetComponent<Block>().selected == true)
                                    {
                                        if (CheckDirection(block.transform, orbUp.gameObject.GetComponent<Orb>().direction))
                                        {
                                            orbUp.gameObject.transform.position = new Vector3(1000, 1000, 1000);
                                        }
                                        if (CheckDirection(block.transform, orbDown.gameObject.GetComponent<Orb>().direction))
                                        {
                                            orbDown.gameObject.transform.position = new Vector3(1000, 1000, 1000);
                                        }
                                        if (CheckDirection(block.transform, orbLeft.gameObject.GetComponent<Orb>().direction))
                                        {
                                            orbLeft.gameObject.transform.position = new Vector3(1000, 1000, 1000);
                                        }
                                        if (CheckDirection(block.transform, orbRight.gameObject.GetComponent<Orb>().direction))
                                        {
                                            orbRight.gameObject.transform.position = new Vector3(1000, 1000, 1000);
                                        }
                                        if (CheckDirection(block.transform, orbForward.gameObject.GetComponent<Orb>().direction))
                                        {
                                            orbForward.gameObject.transform.position = new Vector3(1000, 1000, 1000);
                                        }
                                        if (CheckDirection(block.transform, orbBack.gameObject.GetComponent<Orb>().direction))
                                        {
                                            orbBack.gameObject.transform.position = new Vector3(1000, 1000, 1000);
                                        }
                                    }
                                }
                            }

                        }
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

    bool CheckDirection(Transform point, Vector3 direction)
    {
        if(direction.x == 1)
        {
            if (Physics.Raycast(point.position, point.right, out hit, 0.6f, breakAble))
            {
                if (hit.transform.gameObject.GetComponent<Block>().selected == false)
                {
                    return true;
                }
                
            }
        }
        else if(direction.x == -1)
        {
            if (Physics.Raycast(point.position, point.right * -1, out hit, 0.6f, breakAble))
            {
                if (hit.transform.gameObject.GetComponent<Block>().selected == false)
                {
                    return true;
                }

            }
        }
        else if (direction.y == 1)
        {
            if (Physics.Raycast(point.position, point.up, out hit, 0.6f, breakAble))
            {
                if (hit.transform.gameObject.GetComponent<Block>().selected == false)
                {
                    return true;
                }

            }
        }
        else if (direction.y == -1)
        {
            if (Physics.Raycast(point.position, point.up * -1, out hit, 0.6f, breakAble))
            {
                if (hit.transform.gameObject.GetComponent<Block>().selected == false)
                {
                    return true;
                }

            }
        }
        else if (direction.z == 1)
        {
            if (Physics.Raycast(point.position, point.forward, out hit, 0.6f, breakAble))
            {
                if (hit.transform.gameObject.GetComponent<Block>().selected == false)
                {
                    return true;
                }

            }
        }
        else if (direction.z == -1)
        {
            if (Physics.Raycast(point.position, point.forward * -1, out hit, 0.6f, breakAble))
            {
                if (hit.transform.gameObject.GetComponent<Block>().selected == false)
                {
                    return true;
                }

            }
        }

        return false;
    }

    void FindCubes(Transform point)
    {
        point.gameObject.GetComponent<Block>().selected = true;

        if (Physics.Raycast(point.position, point.forward, out hit, 0.6f, breakAble))
        {
            if(hit.transform.gameObject.GetComponent<Block>().selected == false)
            {
                hit.transform.gameObject.GetComponent<Block>().selected = true;
                FindCubes(hit.transform);
            }
        }

        if (Physics.Raycast(point.position, point.right, out hit, 0.6f, breakAble))
        {
            if (hit.transform.gameObject.GetComponent<Block>().selected == false)
            {
                hit.transform.gameObject.GetComponent<Block>().selected = true;
                FindCubes(hit.transform);
            }
        }

        if (Physics.Raycast(point.position, point.up, out hit, 0.6f, breakAble))
        {
            if (hit.transform.gameObject.GetComponent<Block>().selected == false)
            {
                hit.transform.gameObject.GetComponent<Block>().selected = true;
                FindCubes(hit.transform);
            }
        }

        if (Physics.Raycast(point.position, point.forward * -1, out hit, 0.6f, breakAble))
        {
            if (hit.transform.gameObject.GetComponent<Block>().selected == false)
            {
                hit.transform.gameObject.GetComponent<Block>().selected = true;
                FindCubes(hit.transform);
            }
        }

        if (Physics.Raycast(point.position, point.right * -1, out hit, 0.6f, breakAble))
        {
            if (hit.transform.gameObject.GetComponent<Block>().selected == false)
            {
                hit.transform.gameObject.GetComponent<Block>().selected = true;
                FindCubes(hit.transform);
            }
        }

        if (Physics.Raycast(point.position, point.up * -1, out hit, 0.6f, breakAble))
        {
            if (hit.transform.gameObject.GetComponent<Block>().selected == false)
            {
                hit.transform.gameObject.GetComponent<Block>().selected = true;
                FindCubes(hit.transform);
            }
        }
    }
}
