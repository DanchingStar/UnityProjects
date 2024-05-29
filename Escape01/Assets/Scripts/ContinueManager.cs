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
    /// Menu�V�[���̎n�߂ɌĂ΂��
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
    /// Game�V�[���̎n�߂ɌĂ΂��
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
    /// Result�V�[���̎n�߂ɌĂ΂��
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
    /// �A�C�e���ƃM�~�b�N�ƂÂ�����̃Z�[�u�f�[�^�𖕏�����
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
    /// �M�~�b�N�N���A�̏����Z�[�u����
    /// </summary>
    /// <param name="gimmickName">���̃I�u�W�F�N�g�̃M�~�b�N��</param>
    /// <param name="value">�N���A�Ȃ�1�A���N���A�Ȃ�0</param>
    public void SaveGimmickClear(GimmickName.Type gimmickName, int value)
    {
        PlayerPrefs.SetInt(Enum.GetName(typeof(GimmickName.Type), gimmickName), value);
        PlayerPrefs.SetInt("IsContinue", 1);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// �A�C�e���̊l���󋵂��Z�[�u����
    /// </summary>
    /// <param name="itemType">���̃A�C�e���̖��O</param>
    /// <param name="value">0:������,1:����,2:�g�p�ς�</param>
    public void SaveItemStatus(Item.Type itemType, int value)
    {
        PlayerPrefs.SetInt(Enum.GetName(typeof(Item.Type), itemType), value);
        PlayerPrefs.SetInt("IsContinue", 1);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// �Â����炪�ł���̂ɂ͂��߂���Q�[����I�������Ƃ�
    /// </summary>
    public void StartFromTheBeginning()
    {
        PrefsReset();
    }

    /// <summary>
    /// �Â�����̃f�[�^�����݂��邩�ǂ�����Ԃ�
    /// </summary>
    /// <returns>true�Ȃ�Â�����̃f�[�^������</returns>
    public bool GetIsContinue()
    {
        return isContinue;
    }

}
