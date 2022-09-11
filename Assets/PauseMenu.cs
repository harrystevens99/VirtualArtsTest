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
    public GameObject load;
    public Slider r;
    public Slider g;
    public Slider b;
    public Slider a;

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
        }
        else
        {
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
        foreach (GameObject obj in blocks)
        {
            if(obj.GetComponent<Block>() != null)
            {
                if (obj.GetComponent<Block>().placed == true)
                {

                }
            }
        }

        string[] lines =
{
            "First line", "Second line", "Third line"
        };

        File.CreateText("WriteLines.txt");
        File.WriteAllLines("WriteLines.txt", lines);
        actived = false;
    }

    public void Load()
    {
        actived = false;
    }

}
