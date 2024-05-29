using UnityEngine;
using GoogleMobileAds.Api;
using System;
public class AdMobInterstitial : MonoBehaviour
{
    private string adUnitId;
    private InterstitialAd interstitialAd;

    private int counter;

    void Start()
    {
#if UNITY_ANDROID
        adUnitId = "ca-app-pub-7223826824285484/2675394174";  //�{��
        //adUnitId = "ca-app-pub-3940256099942544/1033173712";  //�e�X�g
#elif UNITY_IOS
        //adUnitId = "�L�����j�b�gID���R�s�y�iiOS�j";  //�{��
        adUnitId = "ca-app-pub-3940256099942544/4411468910";  //�e�X�g
#endif
        this.interstitialAd = new InterstitialAd(adUnitId); //InterstitialAd���C���X�^���X��
        this.interstitialAd.OnAdClosed += HandleOnAdClosed; //���悪����ꂽ�Ƃ��ɁuHandleOnAdClosed�v���Ăяo�����
        AdRequest request = new AdRequest.Builder().Build(); //���request���쐬
        interstitialAd.LoadAd(request); //�C���^�[�X�e�B�V�����L�������[�h

        counter = PlayerPrefs.GetInt("ISCounter",0);
    }

    /// <summary>
    /// �C���^�[�X�e�B�V�����L����\�����郁�\�b�h
    /// </summary>
    /// <param name="num">�C���^�[�X�e�B�V�����L����\������m��</param>
    public void RequestInterstitial()
    {
        if (counter < 2)
        {
            counter++;
            PlayerPrefs.SetInt("ISCounter", counter);
            PlayerPrefs.Save();

            return; //�L�����o�����Ƀ��^�[��
        }

        if (this.interstitialAd.IsLoaded()) //�C���^�[�X�e�B�V�����L�������[�h����Ă��邩�ǂ������m�F
        {
            counter = 0;
            PlayerPrefs.SetInt("ISCounter", counter);
            PlayerPrefs.Save();

            this.interstitialAd.Show(); //�C���^�[�X�e�B�V�����L���̕\��
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