using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Fungus;

public class GameMenu : ManagerParent 
{
    public GameObject gameMenuCanvas;
    public GameObject menuButtonCanvas;
    public GameManager myGameManager;
    public TextMeshProUGUI stageTextObject;

    bool menuPause = false;

    int nowStage = 0;

    private void Start()
    {
        nowStage = myGameManager.NowStageCheck();
        stageTextObject.text = "～ ステージ " + nowStage.ToString() + " ～";

        if(!myBanner)
        {
            myBanner = myGameManager.myBanner;
        }
    }

    private void Update()
    {
        if (myGameManager.GameOverCheck() == true || myGameManager.GameClearCheck() == true) //ゲームオーバー,ゲームクリアが確定したとき
        {
            this.gameObject.SetActive(false); //メニュボタンを非表示にする
        }
    }

    public void MenuBottonClick()
    {
        OnSoundPlay(SoundManager.SE_Type.Yes);
        gameMenuCanvas.SetActive(true);
        menuPause = true;
    }

    public void PushContinue()
    {
        if (buttonPushFlag)
        {
            return;
        }

        OnSoundPlay(SoundManager.SE_Type.Yes);
        gameMenuCanvas.SetActive(false);
        menuPause = false;
    }
    public void PushRetry()
    {
        if (buttonPushFlag)
        {
            return;
        }
        buttonPushFlag = true;

        OnSoundPlay(SoundManager.SE_Type.Yes);
        string str = SceneManager.GetActiveScene().name;
        ChangeScene(str,30);
    }
    public void PushTitle()
    {
        if (buttonPushFlag)
        {
            return;
        }
        buttonPushFlag = true;

        OnSoundPlay(SoundManager.SE_Type.No);
        ChangeScene("Title");
    }

    public bool MenuPauseCheck() //ポーズ中かチェックする関数
    {
        return menuPause;
    }
    public void TalkBottonClick()
    {
        menuPause = true;
        menuButtonCanvas.SetActive(false); 
        Flowchart.BroadcastFungusMessage("TalkStart");
    }
    public void TalkEnd()
    {
        menuPause = false;
        menuButtonCanvas.SetActive(true);
    }

}
