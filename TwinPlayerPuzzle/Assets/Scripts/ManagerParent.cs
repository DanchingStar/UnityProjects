using GoogleMobileAds.Api;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEditor;
using UnityEngine;

public class ManagerParent : MonoBehaviour
{
    protected int maxStage = 12; //作成しているステージ数をここで設定する

    protected bool debugMuteMode = false; //デバッグ中、SoundManagerが悪さするときにtrueにしておく

    protected bool buttonPushFlag =false; //ボタンの二度押しを防ぐためのフラグ
    
    public AdMobBanner myBanner = null; //バナー広告を扱う

    public AdMobInterstitial myInterstitial = null; //インタースティシャル広告を扱う

    private void Awake()
    {
        buttonPushFlag = false;
    }

    /// <summary>
    /// シーンを変更する関数
    /// </summary>
    /// <param name="name">シーン名</param>
    protected void ChangeScene(string name)
    {
        if(myBanner)
        {
            myBanner.DestroyBanner();
        }
        FadeManager.Instance.LoadScene(name, 0.3f);
    }

    /// <summary>
    /// シーンを変更する関数
    /// </summary>
    /// <param name="name">シーン名</param>
    /// <param name="num">インタースティシャル広告を表示する確率(％)</param>
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
    /// サウンドを再生する関数(BGM)
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
    /// サウンドを再生する関数(SE)
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
