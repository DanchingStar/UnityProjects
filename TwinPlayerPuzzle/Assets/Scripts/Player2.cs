using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2 : Player
{
    void Start()
    {
        DoStart(); //�����ݒ�Ȃ�
    }

    void Update()
    {
        if (myGameManager.PauseCheck()) //�|�[�Y���̏ꍇ
        {
            return; //�������Ȃ�
        }

        if (walkToHole == 0) //���Ƃ͖����̂Ƃ�
        {
            if (myGameManager.GameClearCheck() == false) //�܂��Q�[���N���A���Ă��Ȃ��ꍇ
            {
                if (isGetKeyOK == true) //�L�[���󂯕t���Ă���Ƃ�
                {
                    if (waitFlag == true) //waitFlag��true�̂Ƃ�
                    {
                        waitFlag = false;
                        return; //�������Ȃ�
                    }
                    DoIsGetKeyTrue(); //���͂����L�[�ŖړI�n�����߂�
                }
            }
        }

        if (walkToHole == 0 || walkToHole == 1) //���Ɩ����̂Ƃ�,�܂��͌��֌������Ă���Ƃ�
        {
            if (isGetKeyOK == false)  //�L�[���󂯕t���Ȃ��Ƃ�
            {
                DoIsGetKeyFalse(); //�w�肳�ꂽ�}�X�܂ŕ���
                if (waitFlag == false) //waitFlag��false�̂Ƃ�
                {
                    waitFlag = true;
                }
            }
        }
        else //���ɓ��B���Ă���Ƃ�
        {
            FallToAbyss(); //�ޗ��ւƗ�������
        }

        if (myGameManager.GameClearCheck() == true) //�Q�[���N���A�����Ƃ�
        {
            GameClearAction(); //�o���U�C�̃|�[�Y���Ƃ�
        }
    }
}
