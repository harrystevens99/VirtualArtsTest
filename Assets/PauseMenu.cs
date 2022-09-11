using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{

    public bool actived = false;
    public Movement movement;
    public BlockSystem blockSystem;
    public GameObject panel;
    public GameObject paintText;
    public GameObject flyText;
    public GameObject flyToggle;
    public GameObject blockLimit;
    public Text blockLimitText;
    public GameObject exitGame;
    public Image colourBox;
    public GameObject save;
    public Text saveText;
    public GameObject load;
    public Text loadText;
    public Slider r;
    public Slider g;
    public Slider b;
    public Slider a;
    public GameObject error;
    public Text errorText;

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
        

        foreach (GameObject obj in blocks)
        {
            if(obj.GetComponent<Block>() != null)
            {
                if (obj.GetComponent<Block>().placed == true)
                {
                    saveFileLines[lineCount] = "" + obj.transform.position.x + "," + obj.transform.position.y + "," + obj.transform.position.z
                        + "," + obj.GetComponent<Renderer>().material.color.r + "," + obj.GetComponent<Renderer>().material.color.g + "," + obj.GetComponent<Renderer>().material.color.b
                        + "," + obj.GetComponent<Renderer>().material.color.a;
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
                foreach (GameObject obj in blocks)
                {
                    if (obj.GetComponent<Block>() != null)
                    {
                        if (obj.GetComponent<Block>().placed == true)
                        {
                            obj.GetComponent<Renderer>().material.color = new Color(1, 1, 1);
                            obj.GetComponent<Block>().placed = false;
                            obj.transform.gameObject.GetComponent<Renderer>().enabled = false;
                            obj.transform.gameObject.GetComponent<BoxCollider>().enabled = false;
                            obj.transform.position = new Vector3(1000, 1000, 1000);
                        }
                    }
                }


                for (int i = 0; i < lines.Length; i++)
                {
                    if (blocks[i].GetComponent<Block>() != null)
                    {
                        string[] str = lines[i].Split(',');
                        blocks[i].GetComponent<Renderer>().enabled = true;
                        blocks[i].GetComponent<BoxCollider>().enabled = true;
                        blocks[i].GetComponent<Block>().placed = true;
                        blocks[i].transform.position = new Vector3(int.Parse(str[0]), int.Parse(str[1]), int.Parse(str[2]));
                        blocks[i].GetComponent<Renderer>().material.color = new Color(float.Parse(str[3]), float.Parse(str[4]), float.Parse(str[5]), float.Parse(str[6]));
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

}
