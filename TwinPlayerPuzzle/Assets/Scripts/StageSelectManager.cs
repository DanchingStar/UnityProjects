using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectManager : ManagerParent 
{
    int clearStage = 0; //�N���A�X�e�[�W�����i�[����ϐ�

    public Button[] stageSelectButton; //���ׂẴ{�^�����Ⴂ���ɐݒ肵�Ă���

    void Start()
    {
        if (myBanner)
        {
            myBanner.RequestBanner(); //�o�i�[�L�����Ăяo��
        }

        clearStage = PlayerPrefs.GetInt("ClearStage", 0); //�N���A�X�e�[�W����ǂݍ���

        int num = 1;
        foreach (var button in stageSelectButton)
        {
            if (num <= clearStage) //�{�^���i���o�[���N���A�X�e�[�W����菭�Ȃ��ꍇ
            {
                button.interactable = true; //�{�^����L����
            }
            else
            {
                button.interactable = false; //�{�^���𖳌���
            }

            num++; //num�̃C���N�������g
        }
    }

    public void PushMenu() //���j���[�{�^�����������Ƃ�
    {
        if (buttonPushFlag)
        {
            return;
        }
        buttonPushFlag = true;

        OnSoundPlay(SoundManager.SE_Type.No);
        ChangeScene("Menu");
    }
    public void PushStage(int num) //�X�e�[�W�ԍ��̃{�^�����������Ƃ�
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
