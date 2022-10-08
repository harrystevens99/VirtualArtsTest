using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    //Class for the pause menu, which includes different options and saving/loading

    public bool actived = false;
    [SerializeField] private Movement movement;
    [SerializeField] private BlockSystem blockSystem;
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject paintText;
    [SerializeField] private GameObject flyText;
    [SerializeField] private GameObject flyToggle;
    [SerializeField] private GameObject blockLimit;
    [SerializeField] private Text blockLimitText;
    [SerializeField] private GameObject exitGame;
    [SerializeField] private Image colourBox;
    [SerializeField] private GameObject save;
    [SerializeField] private Text saveText;
    [SerializeField] private GameObject load;
    [SerializeField] private Text loadText;
    [SerializeField] private GameObject blockTypeText;
    [SerializeField] private Dropdown blockType;
    [SerializeField] private Slider r;
    [SerializeField] private Slider g;
    [SerializeField] private Slider b;
    [SerializeField] private Slider a;
    [SerializeField] private GameObject error;
    [SerializeField] private Text errorText;
    [SerializeField] private UI ui;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private GameObject fullscreen;

    private string[] saveFileLines;
    private GameObject[] blocks;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            actived = !actived;
            //When activated, exit move mode. This avoids conflicts when choosing to load structures
            if(actived == true)
            {
                if (ui.mode == 3)
                {
                    blockSystem.orbUp.gameObject.transform.position = new Vector3(1000, 1000, 1000);
                    blockSystem.orbDown.gameObject.transform.position = new Vector3(1000, 1000, 1000);
                    blockSystem.orbLeft.gameObject.transform.position = new Vector3(1000, 1000, 1000);
                    blockSystem.orbRight.gameObject.transform.position = new Vector3(1000, 1000, 1000);
                    blockSystem.orbForward.gameObject.transform.position = new Vector3(1000, 1000, 1000);
                    blockSystem.orbBack.gameObject.transform.position = new Vector3(1000, 1000, 1000);

                    blockSystem.moving = false;

                    for (int i = 0; i < blockSystem.blocks.Length; i++)
                    {
                        if (blockSystem.blocks[i] != null)
                        {
                            if (blockSystem.blockScripts[i].placed == true && blockSystem.blockScripts[i].selected == true)
                            {
                                blockSystem.blockRenderers[i].material.SetFloat("_Metallic", 0f);
                                blockSystem.blockScripts[i].selected = false;
                            }
                        }
                    }

                    ui.mode--;
                }
            }
        }

        if(actived == false)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            panel.SetActive(false);
            paintText.SetActive(false);
            flyText.SetActive(false);
            flyToggle.SetActive(false);
            blockLimit.SetActive(false);
            exitGame.SetActive(false);
            load.SetActive(false);
            save.SetActive(false);
            error.SetActive(false);
            blockTypeText.SetActive(false);
            fullscreen.SetActive(false);
        }
        else
        {
            saveFileLines = new string[blockSystem.activeBlocks];
            blockLimitText.text = ("Blocks Remaining: " + (blockSystem.totalBlocks - blockSystem.activeBlocks));
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            panel.SetActive(true);
            paintText.SetActive(true);
            flyText.SetActive(true);
            flyToggle.SetActive(true);
            blockLimit.SetActive(true);
            exitGame.SetActive(true);
            load.SetActive(true);
            save.SetActive(true);
            error.SetActive(true);
            blockTypeText.SetActive(true);
            fullscreen.SetActive(true);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ToggleFlight()
    {
        movement.flying = !movement.flying;
    }

    public void UpdateColour()
    {
        colourBox.color = new Color(r.value, g.value, b.value, a.value);
        blockSystem.paintColour = new Color(r.value, g.value, b.value, a.value);
    }

    public void Save()
    {
        blocks = GameObject.FindGameObjectsWithTag("Block");
        int lineCount = 0;
        

        for(int i = 0; i < blockSystem.blocks.Length; i++)
        {
            if(blockSystem.blockScripts[i] != null)
            {
                if(blockSystem.blockScripts[i].placed == true)
                {
                    //File structure for saving
                    saveFileLines[lineCount] = "" + blockSystem.blocks[i].transform.position.x + "," + blockSystem.blocks[i].transform.position.y + "," + blockSystem.blocks[i].transform.position.z
                        + "," + blockSystem.blockRenderers[i].material.color.r + "," + blockSystem.blockRenderers[i].material.color.g + "," + blockSystem.blockRenderers[i].material.color.b
                        + "," + blockSystem.blockRenderers[i].material.color.a + "," + blockSystem.blockScripts[i].type;
                    lineCount++;
                }
            }
        }

        if (saveText.text != "")
        {
            File.WriteAllLines(saveText.text + ".txt", saveFileLines);
        }
        else
        {
            File.WriteAllLines("MyWorld.txt", saveFileLines);
        }
        actived = false;
    }

    public void Load()
    {
        string[] lines;
        blocks = GameObject.FindGameObjectsWithTag("Block");
        try
        {
            lines = File.ReadAllLines(loadText.text);

            if (blocks.Length >= lines.Length)
            {
                //Begin by removing all blocks

                for(int i = 0; i < blockSystem.blocks.Length; i++)
                {
                    if(blockSystem.blockScripts[i] != null)
                    {
                        if(blockSystem.blockScripts[i].placed == true)
                        {
                            blockSystem.blockRenderers[i].material.color = new Color(1, 1, 1);
                            blockSystem.blockScripts[i].placed = false;
                            blockSystem.blockRenderers[i].enabled = false;
                            blockSystem.blockColliders[i].enabled = false;
                            blockSystem.blocks[i].transform.position = new Vector3(1000, 1000, 1000);
                        }
                    }
                }

                //place each block as described in file
                for (int i = 0; i < lines.Length; i++)
                {
                    if (blockSystem.blockScripts[i] != null)
                    {
                        string[] str = lines[i].Split(',');
                        blockSystem.blockRenderers[i].enabled = true;
                        blockSystem.blockColliders[i].enabled = true;
                        blockSystem.blockScripts[i].placed = true;
                        blockSystem.blocks[i].transform.position = new Vector3(int.Parse(str[0]), int.Parse(str[1]), int.Parse(str[2]));
                        blockSystem.blockRenderers[i].material.color = new Color(float.Parse(str[3]), float.Parse(str[4]), float.Parse(str[5]), float.Parse(str[6]));
                        blockSystem.blockScripts[i].type = int.Parse(str[7]);
                        switch (int.Parse(str[7]))
                        {
                            case 0:
                                blockSystem.blockMeshFilters[i].mesh = blockSystem.cube;
                                break;
                            case 1:
                                blockSystem.blockMeshFilters[i].mesh = blockSystem.sphere;
                                break;
                        }
                    }
                }
                blockSystem.activeBlocks = lines.Length;
                actived = false;
            }
            else
            {
                errorText.text = "Error: Not Enough Blocks!";
            }
        }
        catch(FileNotFoundException e)
        {
            errorText.text = "Error: Could Not Find File";
        }
    }

    public void ChangeType()
    {
        blockSystem.blockType = blockType.value;
    }

    public void Fullscreen()
    {
        if(fullscreenToggle.isOn == true)
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
        

    }

}
