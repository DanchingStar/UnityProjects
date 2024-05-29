using UnityEngine;
using GoogleMobileAds.Api;
using System;
public class AdMobInterstitial : MonoBehaviour
{
    private string adUnitId;
    private InterstitialAd interstitialAd;

    void Start()
    {
#if UNITY_ANDROID
        adUnitId = "ca-app-pub-7223826824285484/3625739106";  //�{��
        //adUnitId = "ca-app-pub-3940256099942544/1033173712";  //�e�X�g
#elif UNITY_IOS
        //adUnitId = "�L�����j�b�gID���R�s�y�iiOS�j";  //�{��
        adUnitId = "ca-app-pub-3940256099942544/4411468910";  //�e�X�g
#endif
        this.interstitialAd = new InterstitialAd(adUnitId); //InterstitialAd���C���X�^���X��
        this.interstitialAd.OnAdClosed += HandleOnAdClosed; //���悪����ꂽ�Ƃ��ɁuHandleOnAdClosed�v���Ăяo�����
        AdRequest request = new AdRequest.Builder().Build(); //���request���쐬
        interstitialAd.LoadAd(request); //�C���^�[�X�e�B�V�����L�������[�h
    }

    /// <summary>
    /// �C���^�[�X�e�B�V�����L����\�����郁�\�b�h
    /// </summary>
    public void RequestInterstitial(int counter)
    {
        if (counter < 2)
        {
            counter++;
            PlayerPrefs.SetInt("InterstitialCounter", counter);
            PlayerPrefs.Save();

            return; //�L�����o�����Ƀ��^�[��
        }

        if (this.interstitialAd.IsLoaded()) //�C���^�[�X�e�B�V�����L�������[�h����Ă��邩�ǂ������m�F
        {
            this.interstitialAd.Show(); //�C���^�[�X�e�B�V�����L���̕\��

            Start();

            PlayerPrefs.SetInt("InterstitialCounter", 0);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// �L��������ꂽ�Ƃ��ɍs����������
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        interstitialAd.Destroy(); //���������[�N�j�~�I
    }
}