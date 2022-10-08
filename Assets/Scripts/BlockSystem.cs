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
    public Renderer orbUp;
    public Renderer orbDown;
    public Renderer orbLeft;
    public Renderer orbRight;
    public Renderer orbForward;
    public Renderer orbBack;
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
    [SerializeField] private GameObject[] orbObjects;
    private int orbIndex = 0;
    

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
            }


            if (Input.GetMouseButtonUp(0))
            {
                placeCheck = true;
                breakCheck = true;
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
                orbUp.transform.position = new Vector3(hit.transform.position.x, hit.transform.position.y + 2f, hit.transform.position.z);
                orbDown.transform.position = new Vector3(hit.transform.position.x, hit.transform.position.y - 2f, hit.transform.position.z);
                orbLeft.transform.position = new Vector3(hit.transform.position.x - 2f, hit.transform.position.y, hit.transform.position.z);
                orbRight.transform.position = new Vector3(hit.transform.position.x + 2f, hit.transform.position.y, hit.transform.position.z);
                orbForward.transform.position = new Vector3(hit.transform.position.x, hit.transform.position.y, hit.transform.position.z + 2f);
                orbBack.transform.position = new Vector3(hit.transform.position.x, hit.transform.position.y, hit.transform.position.z - 2f);

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
            orbUp.material.color = new Color(1, 0, 1, 1);
            orbDown.material.color = new Color(1, 0, 1, 1);
            orbLeft.material.color = new Color(1, 0, 1, 1);
            orbRight.material.color = new Color(1, 0, 1, 1);
            orbForward.material.color = new Color(1, 0, 1, 1);
            orbBack.material.color = new Color(1, 0, 1, 1);

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
                orbUp.transform.position = new Vector3(wireFrameCube.transform.position.x, wireFrameCube.transform.position.y + 2f, wireFrameCube.transform.position.z);
                orbDown.transform.position = new Vector3(wireFrameCube.transform.position.x, wireFrameCube.transform.position.y - 2f, wireFrameCube.transform.position.z);
                orbLeft.transform.position = new Vector3(wireFrameCube.transform.position.x - 2f, wireFrameCube.transform.position.y, wireFrameCube.transform.position.z);
                orbRight.transform.position = new Vector3(wireFrameCube.transform.position.x + 2f, wireFrameCube.transform.position.y, wireFrameCube.transform.position.z);
                orbForward.transform.position = new Vector3(wireFrameCube.transform.position.x, wireFrameCube.transform.position.y, wireFrameCube.transform.position.z + 2f);
                orbBack.transform.position = new Vector3(wireFrameCube.transform.position.x, wireFrameCube.transform.position.y, wireFrameCube.transform.position.z - 2f);

                for (int i = 0; i < blocks.Length; i++)
                {
                    if(blockScripts[i] != null)
                    {
                        if(blockScripts[i].placed == true && blockScripts[i].selected == true)
                        {
                            PlaceOrbs(blocks[i].transform);
                        }
                    }
                }
            }
        }
    }


    void PlaceOrbs(Transform pos)
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

        for(int i = 0; i < directions.Length; i++)
        {
            if (Physics.Raycast(point.position, directions[i], out hit, 0.6f, breakAble))
            {
                int hitIndex = System.Array.IndexOf(blocks, hit.transform.gameObject);
                if (blockScripts[hitIndex].selected == false)
                {
                    blockScripts[hitIndex].selected = true;
                    FindCubes(hit.transform);
                }
            }
        }
    }
}
