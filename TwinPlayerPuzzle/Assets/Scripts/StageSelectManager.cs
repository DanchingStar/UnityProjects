using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectManager : ManagerParent 
{
    int clearStage = 0; //クリアステージ数を格納する変数

    public Button[] stageSelectButton; //すべてのボタンを若い順に設定しておく

    void Start()
    {
        if (myBanner)
        {
            myBanner.RequestBanner(); //バナー広告を呼び出す
        }

        clearStage = PlayerPrefs.GetInt("ClearStage", 0); //クリアステージ数を読み込む

        int num = 1;
        foreach (var button in stageSelectButton)
        {
            if (num <= clearStage) //ボタンナンバーがクリアステージ数より少ない場合
            {
                button.interactable = true; //ボタンを有効化
            }
            else
            {
                button.interactable = false; //ボタンを無効化
            }

            num++; //numのインクリメント
        }
    }

    public void PushMenu() //メニューボタンを押したとき
    {
        if (buttonPushFlag)
        {
            return;
        }
        buttonPushFlag = true;

        OnSoundPlay(SoundManager.SE_Type.No);
        ChangeScene("Menu");
    }
    public void PushStage(int num) //ステージ番号のボタンを押したとき
    {
        if (buttonPushFlag)
        {
            return;
        }
        buttonPushFlag = true;

        OnSoundPlay(SoundManager.SE_Type.Yes);
        string str = "Game" + num.ToString();
        ChangeScene(str,30);
    }
}
