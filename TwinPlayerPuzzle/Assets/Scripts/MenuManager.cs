using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : ManagerParent
{
    public GameObject exitCanvas;

    int clearStage = 0;

    private void Start()
    {
        if (myBanner)
        {
            myBanner.RequestBanner(); //バナー広告を呼び出す
        }

        clearStage = PlayerPrefs.GetInt("ClearStage", 0);
    }

    public void PushGameStart()
    {
        if (buttonPushFlag)
        {
            return;
        }
        buttonPushFlag = true;
        
        OnSoundPlay(SoundManager.SE_Type.Yes);
        int n = clearStage + 1;
        if(maxStage  < n)
        {
            n = maxStage ;
        }
        string text = "Game" + n;
        ChangeScene(text);
    }
    public void PushStageSelect()
    {
        if (buttonPushFlag)
        {
            return;
        }
        buttonPushFlag = true;

        OnSoundPlay(SoundManager.SE_Type.Yes);
        ChangeScene("StageSelect");
    }
    public void PushExit()
    {
        if (buttonPushFlag)
        {
            return;
        }

        OnSoundPlay(SoundManager.SE_Type.Yes);
        exitCanvas.SetActive(true);
    }
    public void PushExitYes()
    {
        OnSoundPlay(SoundManager.SE_Type.No);
        Application.Quit();
    }
    public void PushExitNo()
    {
        OnSoundPlay(SoundManager.SE_Type.Yes);
        exitCanvas.SetActive(false);
    }
}
