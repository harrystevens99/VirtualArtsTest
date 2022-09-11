using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    public int mode = 0;
    public Renderer cube;
    public Renderer paintBrush;
    public Renderer picAxe;
    public Color selectedColour;
    public Color deselectedColour;
    public PauseMenu menu;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (menu.actived == false)
        {
            if (Input.GetKeyDown("q") && mode > 0)
            {
                mode--;
            }

            if (Input.GetKeyDown("e") && mode < 2)
            {
                mode++;
            }

            switch (mode)
            {
                case 0:
                    cube.material.color = selectedColour;
                    paintBrush.material.color = deselectedColour;
                    picAxe.material.color = deselectedColour;
                    break;
                case 1:
                    cube.material.color = deselectedColour;
                    paintBrush.material.color = selectedColour;
                    picAxe.material.color = deselectedColour;
                    break;
                case 2:
                    cube.material.color = deselectedColour;
                    paintBrush.material.color = deselectedColour;
                    picAxe.material.color = selectedColour;
                    break;
            }
        }
    }
}
