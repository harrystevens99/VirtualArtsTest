using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    //Class which controls the UI hot-bar.

    public int mode = 0;
    [SerializeField] private Image cube;
    [SerializeField] private Image paintBrush;
    [SerializeField] private Image picAxe;
    [SerializeField] private Image move;
    [SerializeField] private Image blockSelect;
    [SerializeField] private Color selectedColour;
    [SerializeField] private Color deselectedColour;
    [SerializeField] private PauseMenu menu;
    [SerializeField] private BlockSystem blockSystem;

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
            if (mode == 3 && (Input.GetKeyDown("q") || Input.GetKeyDown("e")))
            {
                for (int i = 0; i < blockSystem.orbObjects.Length; i++)
                {
                    blockSystem.orbObjects[i].gameObject.transform.position = new Vector3(1000, 1000, 1000);
                }

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
            }

            //When moving out of "block place mode"
            if (mode == 4 && Input.GetKeyDown("q"))
            {
                blockSystem.segmentStartFlag = false;
            }



            if (Input.GetKeyDown("q") && mode > 0)
            {
                mode--;
            }

            if (Input.GetKeyDown("e") && mode < 4)
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
                    blockSelect.color = deselectedColour;
                    break;
                case 1:
                    cube.color = deselectedColour;
                    paintBrush.color = selectedColour;
                    picAxe.color = deselectedColour;
                    move.color = deselectedColour;
                    blockSelect.color = deselectedColour;
                    break;
                case 2:
                    cube.color = deselectedColour;
                    paintBrush.color = deselectedColour;
                    picAxe.color = selectedColour;
                    move.color = deselectedColour;
                    blockSelect.color = deselectedColour;
                    break;
                case 3:
                    cube.color = deselectedColour;
                    paintBrush.color = deselectedColour;
                    picAxe.color = deselectedColour;
                    move.color = selectedColour;
                    blockSelect.color = deselectedColour;
                    break;
                case 4:
                    cube.color = deselectedColour;
                    paintBrush.color = deselectedColour;
                    picAxe.color = deselectedColour;
                    move.color = deselectedColour;
                    blockSelect.color = selectedColour;
                    break;
            }
        }
    }
}
