using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSystem : MonoBehaviour
{
    //Class for block system, which contains code for the different builder modes (placing blocks, moving blocks etc).

    [SerializeField] private MoveLook mainCamera;
    [SerializeField] private PauseMenu menu;
    [SerializeField] private GameObject wireFrameCube;
    [SerializeField] private UI ui;
    [SerializeField] private LayerMask breakAble;
    [SerializeField] private LayerMask placeable;
    [SerializeField] private LayerMask orbs;
    [SerializeField] private ParticleSystem particles;
    public Color paintColour = new Color(1, 0, 0, 1);
    public int activeBlocks = 0;
    public int totalBlocks;
    public Mesh sphere;
    public Mesh cube;
    public bool moving;
    private bool canPlace;
    private bool placeCheck = true;
    private bool canBreak;
    private bool breakCheck = true;
    private Vector3 cubePoint = new Vector3();
    public GameObject[] blocks;
    public Block[] blockScripts;
    public Renderer[] blockRenderers;
    public MeshFilter[] blockMeshFilters;
    public Collider[] blockColliders;
    public int blockType = 0;
    private RaycastHit hit;
    private Vector3[] directions;
    private Orb[] orbScripts;
    private Renderer[] orbRenderers;
    public GameObject[] orbObjects;
    private int orbIndex = 0;
    public bool segmentStartFlag = false;
    public GameObject segmentPlaceWireFrame;
    private Vector3 segmentStart;
    private Vector3 segmentEnd;
    [SerializeField] private GameObject segmentOutline;
    

    // Start is called before the first frame update
    void Start()
    {
        blocks = GameObject.FindGameObjectsWithTag("Block");
        totalBlocks = blocks.Length;

        //Cache components to avoid .GetComponent calls

        blockScripts = new Block[blocks.Length];
        blockRenderers = new Renderer[blocks.Length];
        blockMeshFilters = new MeshFilter[blocks.Length];
        blockColliders = new BoxCollider[blocks.Length];

        for (int i = 0; i < blocks.Length; i++)
        {
            blockScripts[i] = blocks[i].GetComponent<Block>();
            blockRenderers[i] = blocks[i].GetComponent<Renderer>();
            blockMeshFilters[i] = blocks[i].GetComponent<MeshFilter>();
            blockColliders[i] = blocks[i].GetComponent<BoxCollider>();
        }

        orbScripts = new Orb[orbObjects.Length];
        orbRenderers = new Renderer[orbObjects.Length];

        for(int i = 0; i < orbObjects.Length; i++)
        {
            orbRenderers[i] = orbObjects[i].GetComponent<Renderer>();
            orbScripts[i] = orbObjects[i].GetComponent<Orb>();
        }

        directions = new Vector3[6];
        directions[0] = blocks[0].transform.forward;
        directions[1] = blocks[0].transform.right;
        directions[2] = blocks[0].transform.up;
        directions[3] = blocks[0].transform.forward * -1;
        directions[4] = blocks[0].transform.right * -1;
        directions[5] = blocks[0].transform.up * -1;
    }

    // Update is called once per frame
    void Update()
    {
        if (menu.actived == false)
        {
            switch (ui.mode)
            {
                case 0:
                    BlockPlacement();
                    break;
                case 1:
                    //Paint cube
                    PlaceWireFrameCube();
                    PaintBlock();
                    break;
                case 2:
                    //Remove cube
                    PlaceWireFrameCube();
                    RemoveBlock();
                    break;
                case 3:
                    //Move cube
                    MoveBlock();
                    break;
                case 4:
                    SegmentPlace();
                    break;
            }


            if (Input.GetMouseButtonUp(0))
            {
                placeCheck = true;
                breakCheck = true;
            }

            if(segmentStartFlag == false)
            {
                segmentPlaceWireFrame.transform.position = new Vector3(1000, 1000, 1000);
                segmentOutline.transform.position = new Vector3(1000, 1000, 1000);
                segmentOutline.transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    void SegmentPlace()
    {
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, 5, placeable))
        {
            cubePoint.x = Mathf.RoundToInt(hit.point.x);
            cubePoint.y = Mathf.RoundToInt(hit.point.y);
            cubePoint.z = Mathf.RoundToInt(hit.point.z);
            wireFrameCube.transform.position = cubePoint;
            canBreak = true;
        }
        else
        {
            wireFrameCube.transform.position = new Vector3(1000, 1000, 1000);
            canBreak = false;
        }

        if (segmentStartFlag == false)
        {
            if (canBreak == true && Input.GetMouseButtonDown(0) && breakCheck == true)
            {
                segmentStartFlag = true;
                segmentPlaceWireFrame.transform.position = cubePoint;
                segmentStart = cubePoint;
                breakCheck = false;
            }
        }
        else
        {
            int zVec = 1; int yVec = 1; int xVec = 1;

            if (canBreak == false)
            {
                segmentOutline.transform.localScale = new Vector3(1, 1, 1);
                segmentOutline.transform.position = new Vector3(1000, 1000, 1000);
            }
            else
            {
                //Difference in location on each axis dictates how each loop iterates from the start point to the end point
                if ((segmentStart.z - segmentEnd.z) > 0) { zVec *= -1; }
                if ((segmentStart.y - segmentEnd.y) > 0) { yVec *= -1; }
                if ((segmentStart.x - segmentEnd.x) > 0) { xVec *= -1; }

                segmentEnd = cubePoint;
                segmentOutline.transform.localScale = new Vector3(segmentStart.x - segmentEnd.x - xVec, segmentStart.y - segmentEnd.y - yVec, segmentStart.z - segmentEnd.z - zVec);
                segmentOutline.transform.position = (segmentStart + segmentEnd) / 2;

                if (Input.GetMouseButtonDown(0) && breakCheck == true)
                {
                    for (int y = (int)segmentStart.y; y != (int)segmentEnd.y + yVec; y += yVec)
                    {
                        //each y-axis column filled with cubes
                        for (int z = (int)segmentStart.z; z != (int)segmentEnd.z + zVec; z += zVec)
                        {
                            //each z-axis column filled with cubes
                            for (int x = (int)segmentStart.x; x != (int)segmentEnd.x + xVec; x += xVec)
                            {
                                //each x-axis column filled with cubes
                                Vector3 pos = new Vector3(x, y, z);
                                if (!Physics.CheckSphere(pos, 0.1f, breakAble))
                                {
                                    for (int i = 0; i < blockScripts.Length; i++)
                                    {
                                        if (blockScripts[i] != null)
                                        {
                                            if (blockScripts[i].placed == false && placeCheck == true)
                                            {
                                                activeBlocks++;
                                                blockScripts[i].type = blockType;
                                                switch (blockType)
                                                {
                                                    case 0:
                                                        blockMeshFilters[i].mesh = cube;
                                                        break;
                                                    case 1:
                                                        blockMeshFilters[i].mesh = sphere;
                                                        break;
                                                }
                                                blockRenderers[i].enabled = true;
                                                blockColliders[i].enabled = true;
                                                blocks[i].transform.position = pos;
                                                blockScripts[i].placed = true;
                                                placeCheck = false;
                                            }
                                        }
                                    }
                                }
                                placeCheck = true;
                            }
                        }
                    }
                    segmentStartFlag = false;
                }
            }
        }
    }

    void PlaceWireFrameCube()
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
    }

    void BlockPlacement()
    {
        //Cube placement
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, 5, placeable))
        {
            //Snap cube to grid
            cubePoint.x = Mathf.RoundToInt(hit.point.x);
            cubePoint.y = Mathf.RoundToInt(hit.point.y);
            cubePoint.z = Mathf.RoundToInt(hit.point.z);
            wireFrameCube.transform.position = cubePoint;
            canPlace = true;
        }
        else
        {
            //If surface is non-placeable or out of range, target cube is moved out of bounds
            wireFrameCube.transform.position = new Vector3(1000, 1000, 1000);
            canPlace = false;
        }

        //Place cube
        if (canPlace == true && Input.GetMouseButtonDown(0) && placeCheck == true)
        {
            for(int i = 0; i < blockScripts.Length; i++)
            {
                if(blockScripts[i] != null)
                {
                    if(blockScripts[i].placed == false && placeCheck == true)
                    {
                        activeBlocks++;
                        blockScripts[i].type = blockType;
                        switch (blockType)
                        {
                            case 0:
                                blockMeshFilters[i].mesh = cube;
                                break;
                            case 1:
                                blockMeshFilters[i].mesh = sphere;
                                break;
                        }
                        blockRenderers[i].enabled = true;
                        blockColliders[i].enabled = true;
                        blocks[i].transform.position = cubePoint;
                        blockScripts[i].placed = true;
                        placeCheck = false;
                    }
                }
            }
        }
    }

    void PaintBlock()
    {
        if (canBreak == true && Input.GetMouseButtonDown(0) && breakCheck == true)
        {
            int index = System.Array.IndexOf(blocks, hit.transform.gameObject);
            blockRenderers[index].material.color = paintColour;
            breakCheck = false;
        }
    }

    void RemoveBlock()
    {
        if (canBreak == true && Input.GetMouseButtonDown(0) && breakCheck == true)
        {
            int index = System.Array.IndexOf(blocks, hit.transform.gameObject);
            particles.transform.position = cubePoint;
            particles.startColor = blockRenderers[index].material.color;
            //Alpha channel set to 1 as transparent blocks would otherwise create invisible particles
            particles.startColor = new Color(particles.startColor.r, particles.startColor.g, particles.startColor.b, 1);
            particles.Play();
            blockRenderers[index].material.color = new Color(1, 1, 1);
            blockScripts[index].placed = false;
            blockRenderers[index].enabled = false;
            blockColliders[index].enabled = false;
            blockScripts[index].type = 0;
            blockMeshFilters[index].mesh = cube;
            hit.transform.position = new Vector3(1000, 1000, 1000);
            activeBlocks--;
            breakCheck = false;
        }
    }

    void MoveBlock()
    {
        if (moving == false)
        {
            PlaceWireFrameCube();

            if (canBreak == true && Input.GetMouseButtonDown(0) && breakCheck == true)
            {
                PlaceOrbs();

                //All cubes un-selected first

                for(int i = 0; i < blocks.Length; i++)
                {
                    if(blockScripts[i] != null)
                    {
                        if(blockScripts[i].placed == true && blockScripts[i].selected == true)
                        {
                            blockRenderers[i].material.SetFloat("_Metallic", 0f);
                            blockScripts[i].selected = false;
                        }
                    }
                }
                moving = true;

                //Find cubes to be selected
                FindCubes(hit.transform);
                breakCheck = false;
            }
        }
        else
        {
            for(int i = 0; i < orbRenderers.Length; i++)
            {
                orbRenderers[i].material.color = new Color(1, 0, 1, 1);
            }

            if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, 100, orbs))
            {
                //Direction selection, stored in "hit"
                orbIndex = System.Array.IndexOf(orbObjects, hit.transform.gameObject);
                orbRenderers[orbIndex].material.color = new Color(0, 1, 1, 1);
                canBreak = true;
            }
            else
            {
                canBreak = false;
            }

            if (canBreak == true && Input.GetMouseButtonDown(0) && breakCheck == true)
            {
                //Move selected blocks in given direction

                for(int i = 0; i < blocks.Length; i++)
                {
                    if(blockScripts[i] != null)
                    {
                        if(blockScripts[i].placed == true && blockScripts[i].selected == true)
                        {
                            blocks[i].transform.position += orbScripts[orbIndex].direction;
                        }
                    }
                }

                breakCheck = false;
                wireFrameCube.transform.position = wireFrameCube.transform.position + orbScripts[orbIndex].direction;
                PlaceOrbs();

                for (int i = 0; i < blocks.Length; i++)
                {
                    if(blockScripts[i] != null)
                    {
                        if(blockScripts[i].placed == true && blockScripts[i].selected == true)
                        {
                            RemoveOrbs(blocks[i].transform);
                        }
                    }
                }
            }
        }
    }

    void PlaceOrbs()
    {
        orbObjects[0].transform.position = new Vector3(wireFrameCube.transform.position.x, wireFrameCube.transform.position.y + 2f, wireFrameCube.transform.position.z);
        orbObjects[1].transform.position = new Vector3(wireFrameCube.transform.position.x, wireFrameCube.transform.position.y - 2f, wireFrameCube.transform.position.z);
        orbObjects[2].transform.position = new Vector3(wireFrameCube.transform.position.x - 2f, wireFrameCube.transform.position.y, wireFrameCube.transform.position.z);
        orbObjects[3].transform.position = new Vector3(wireFrameCube.transform.position.x + 2f, wireFrameCube.transform.position.y, wireFrameCube.transform.position.z);
        orbObjects[4].transform.position = new Vector3(wireFrameCube.transform.position.x, wireFrameCube.transform.position.y, wireFrameCube.transform.position.z + 2f);
        orbObjects[5].transform.position = new Vector3(wireFrameCube.transform.position.x, wireFrameCube.transform.position.y, wireFrameCube.transform.position.z - 2f);
    }

    void RemoveOrbs(Transform pos)
    {
        for(int i = 0; i < orbScripts.Length; i++)
        {
            if (CheckDirection(pos, orbScripts[i].direction))
            {
                orbScripts[i].gameObject.transform.position = new Vector3(1000, 1000, 1000);
            }
        }
    }

    //Given a block and a direction, check if there is an unselected block in that direction
    bool CheckDirection(Transform point, Vector3 direction)
    {
        if (Physics.Raycast(point.position, direction, out hit, 0.6f, breakAble))
        {
            int index = System.Array.IndexOf(blocks, hit.transform.gameObject);
            if (blockScripts[index].selected == false)
            {
                return true;
            }

        }

        return false;
    }

    //Recursive function to select all cubes which are adjacent to the initially selected cube
    void FindCubes(Transform point)
    {
        int index = System.Array.IndexOf(blocks, point.gameObject);
        blockScripts[index].selected = true;
        blockRenderers[index].material.SetFloat("_Metallic", 0.5f);

        for (int i = 0; i < directions.Length; i++)
        {
            if (Physics.Raycast(point.position, directions[i], out hit, 0.6f, breakAble))
            {
                int hitIndex = System.Array.IndexOf(blocks, hit.transform.gameObject);
                if (blockScripts[hitIndex].selected == false)
                {
                    blockScripts[hitIndex].selected = true;
                    blockRenderers[hitIndex].material.SetFloat("_Metallic", 0.5f);
                    FindCubes(hit.transform);
                }
            }
        }
    }
}
