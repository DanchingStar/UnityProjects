using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleManager : ManagerParent
{
    private void Start()
    {
        OnSoundPlay(SoundManager.BGM_Type.MenuBGM);
    }
    void Update()
    {
        PushEnter();
    }

    void PushEnter()
    {
        if (buttonPushFlag)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButton(0))
        {
            buttonPushFlag = true;

            OnSoundPlay(SoundManager.SE_Type.Yes);
            ChangeScene("Menu");
        }
    }
}
