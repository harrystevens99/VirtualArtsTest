using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    //Class which controls the UI hot-bar.

    public int mode = 0;
    public Image cube;
    public Image paintBrush;
    public Image picAxe;
    public Image move;
    public Color selectedColour;
    public Color deselectedColour;
    public PauseMenu menu;
    public BlockSystem blockSystem;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (menu.actived == false)
        {
            //When moving out of "move mode" de-select all blocks
            if (Input.GetKeyDown("q") && mode > 0)
            {
                if (mode == 3)
                {
                    blockSystem.orbUp.gameObject.transform.position = new Vector3(1000, 1000, 1000);
                    blockSystem.orbDown.gameObject.transform.position = new Vector3(1000, 1000, 1000);
                    blockSystem.orbLeft.gameObject.transform.position = new Vector3(1000, 1000, 1000);
                    blockSystem.orbRight.gameObject.transform.position = new Vector3(1000, 1000, 1000);
                    blockSystem.orbForward.gameObject.transform.position = new Vector3(1000, 1000, 1000);
                    blockSystem.orbBack.gameObject.transform.position = new Vector3(1000, 1000, 1000);

                    blockSystem.moving = false;

                    foreach (GameObject block in blockSystem.blocks)
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
                }

                mode--;
            }

            if (Input.GetKeyDown("e") && mode < 3)
            {
                mode++;
            }

            switch (mode)
            {
                case 0:
                    cube.color = selectedColour;
                    paintBrush.color = deselectedColour;
                    picAxe.color = deselectedColour;
                    move.color = deselectedColour;
                    break;
                case 1:
                    cube.color = deselectedColour;
                    paintBrush.color = selectedColour;
                    picAxe.color = deselectedColour;
                    move.color = deselectedColour;
                    break;
                case 2:
                    cube.color = deselectedColour;
                    paintBrush.color = deselectedColour;
                    picAxe.color = selectedColour;
                    move.color = deselectedColour;
                    break;
                case 3:
                    cube.color = deselectedColour;
                    paintBrush.color = deselectedColour;
                    picAxe.color = deselectedColour;
                    move.color = selectedColour;
                    break;
            }
        }
    }
}
