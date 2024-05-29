using GoogleMobileAds.Api;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEditor;
using UnityEngine;

public class ManagerParent : MonoBehaviour
{
    protected int maxStage = 12; //�쐬���Ă���X�e�[�W���������Őݒ肷��

    protected bool debugMuteMode = false; //�f�o�b�O���ASoundManager����������Ƃ���true�ɂ��Ă���

    protected bool buttonPushFlag =false; //�{�^���̓�x������h�����߂̃t���O
    
    public AdMobBanner myBanner = null; //�o�i�[�L��������

    public AdMobInterstitial myInterstitial = null; //�C���^�[�X�e�B�V�����L��������

    private void Awake()
    {
        buttonPushFlag = false;
    }

    /// <summary>
    /// �V�[����ύX����֐�
    /// </summary>
    /// <param name="name">�V�[����</param>
    protected void ChangeScene(string name)
    {
        if(myBanner)
        {
            myBanner.DestroyBanner();
        }
        FadeManager.Instance.LoadScene(name, 0.3f);
    }

    /// <summary>
    /// �V�[����ύX����֐�
    /// </summary>
    /// <param name="name">�V�[����</param>
    /// <param name="num">�C���^�[�X�e�B�V�����L����\������m��(��)</param>
    protected void ChangeScene(string name,int num)
    {
        myInterstitial.RequestInterstitial(num);
        if (myBanner)
        {
            myBanner.DestroyBanner();
        }
        FadeManager.Instance.LoadScene(name, 0.3f);
    }

    /// <summary>
    /// �T�E���h���Đ�����֐�(BGM)
    /// </summary>
    /// <param name="type"></param>
    public void OnSoundPlay(SoundManager.BGM_Type type)
    {
        if (!debugMuteMode)
        {
            SoundManager.instance.PlayBGM(type);
        }
    }

    /// <summary>
    /// �T�E���h���Đ�����֐�(SE)
    /// </summary>
    /// <param name="type"></param>
    public void OnSoundPlay(SoundManager.SE_Type type)
    {
        if (!debugMuteMode)
        {
            SoundManager.instance.PlaySE(type);
        }
    }


}
