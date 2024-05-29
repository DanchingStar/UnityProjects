using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ContinueManager : MonoBehaviour
{
    private string sceneName;
    private bool isContinue = false;

    public static ContinueManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "Menu")
        {
            MenuStart();
        }
        else if (sceneName == "Game")
        {
            if (PlayerPrefs.GetInt("IsContinue", 0) == 0) return;
            GameStart();
        }
        else if (sceneName == "Result")
        {
            ResultStart();
        }
        else
        {
            Debug.LogError("ContinueManager : Scene Name is not define");
        }
    }

    /// <summary>
    /// Menuシーンの始めに呼ばれる
    /// </summary>
    private void MenuStart()
    {
        int num = PlayerPrefs.GetInt("IsContinue", 0);
        if (num != 0)
        {
            isContinue = true;
        }
        else
        {
            isContinue = false;
        }
    }

    /// <summary>
    /// Gameシーンの始めに呼ばれる
    /// </summary>
    private void GameStart()
    {
        foreach (Item.Type value in Enum.GetValues(typeof(Item.Type)))
        {
            int prefsNum = PlayerPrefs.GetInt(Enum.GetName(typeof(Item.Type), value), 0);
            if (prefsNum == 1)
            {
                ItemBox.instance.SetItem(ItemGenerater.instance.Spawn(value));
            }
        }
    }

    /// <summary>
    /// Resultシーンの始めに呼ばれる
    /// </summary>
    private void ResultStart()
    {
        int num = PlayerPrefs.GetInt("NumOfClear", 0);
        num++;
        PlayerPrefs.SetInt("NumOfClear", num);
        PrefsReset();
        PlayerPrefs.Save();
    }

    /// <summary>
    /// アイテムとギミックとつづきからのセーブデータを抹消する
    /// </summary>
    private void PrefsReset()
    {
        PlayerPrefs.DeleteKey("IsContinue");
        foreach (GimmickName.Type value in Enum.GetValues(typeof(GimmickName.Type)))
        {
            PlayerPrefs.DeleteKey(Enum.GetName(typeof(GimmickName.Type), value));
        }
        foreach (Item.Type value in Enum.GetValues(typeof(Item.Type)))
        {
            PlayerPrefs.DeleteKey(Enum.GetName(typeof(Item.Type), value));
        }
    }

    /// <summary>
    /// ギミッククリアの情報をセーブする
    /// </summary>
    /// <param name="gimmickName">そのオブジェクトのギミック名</param>
    /// <param name="value">クリアなら1、未クリアなら0</param>
    public void SaveGimmickClear(GimmickName.Type gimmickName, int value)
    {
        PlayerPrefs.SetInt(Enum.GetName(typeof(GimmickName.Type), gimmickName), value);
        PlayerPrefs.SetInt("IsContinue", 1);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// アイテムの獲得状況をセーブする
    /// </summary>
    /// <param name="itemType">そのアイテムの名前</param>
    /// <param name="value">0:未所持,1:所持,2:使用済み</param>
    public void SaveItemStatus(Item.Type itemType, int value)
    {
        PlayerPrefs.SetInt(Enum.GetName(typeof(Item.Type), itemType), value);
        PlayerPrefs.SetInt("IsContinue", 1);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// つづきからができるのにはじめからゲームを選択したとき
    /// </summary>
    public void StartFromTheBeginning()
    {
        PrefsReset();
    }

    /// <summary>
    /// つづきからのデータが存在するかどうかを返す
    /// </summary>
    /// <returns>trueならつづきからのデータがある</returns>
    public bool GetIsContinue()
    {
        return isContinue;
    }

}
