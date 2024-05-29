using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public Player1 myPlayer1;
    public Player2 myPlayer2;

    bool inputUp = false;
    bool inputDown = false;
    bool inputLeft = false;
    bool inputRight = false;
    
    bool inputBottonUp = false;
    bool inputBottonDown = false;
    bool inputBottonLeft = false;
    bool inputBottonRight = false;

    // Update is called once per frame
    void Update()
    {
        inputUp = false;
        inputDown = false;
        inputLeft = false;
        inputRight = false;

        if (Input.GetKeyDown(KeyCode.UpArrow) || inputBottonUp == true)
        {
            inputUp = true;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || inputBottonDown == true)
        {
            inputDown = true;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || inputBottonLeft == true)
        {
            inputLeft = true;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || inputBottonRight == true)
        {
            inputRight = true;
        }

        if (myPlayer1.IsGetKeyOK() && myPlayer2.IsGetKeyOK())
        {
            myPlayer1.inputManagerFlag = true;
            myPlayer2.inputManagerFlag = true;
        }

        inputBottonUp = false;
        inputBottonDown = false;
        inputBottonLeft = false;
        inputBottonRight = false;
    }


    public void ButtonUpInput()
    {
        inputBottonUp = true;
    }
    public void ButtonDownInput()
    {
        inputBottonDown = true;
    }
    public void ButtonLeftInput()
    {
        inputBottonLeft = true;
    }
    public void ButtonRightInput()
    {
        inputBottonRight = true;
    }

    public bool CheckInputUp()
    {
        if (myPlayer1.inputManagerFlag == true || myPlayer2.inputManagerFlag == true) 
        {
            return inputUp;
        }
        else
        {
            return false;
        }
    }
    public bool CheckInputDown()
    {
        if (myPlayer1.inputManagerFlag == true || myPlayer2.inputManagerFlag == true)
        {
            return inputDown;
        }
        else
        {
            return false;
        }
    }
    public bool CheckInputLeft()
    {
        if (myPlayer1.inputManagerFlag == true || myPlayer2.inputManagerFlag == true)
        {
            return inputLeft;
        }
        else
        {
            return false;
        }
    }
    public bool CheckInputRight()
    {
        if (myPlayer1.inputManagerFlag == true || myPlayer2.inputManagerFlag == true)
        {
            return inputRight;
        }
        else
        {
            return false;
        }
    }
}
