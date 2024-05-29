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
        adUnitId = "ca-app-pub-7223826824285484/2675394174";  //本番
        //adUnitId = "ca-app-pub-3940256099942544/1033173712";  //テスト
#elif UNITY_IOS
        //adUnitId = "広告ユニットIDをコピペ（iOS）";  //本番
        adUnitId = "ca-app-pub-3940256099942544/4411468910";  //テスト
#endif
        this.interstitialAd = new InterstitialAd(adUnitId); //InterstitialAdをインスタンス化
        this.interstitialAd.OnAdClosed += HandleOnAdClosed; //動画が閉じられたときに「HandleOnAdClosed」が呼び出される
        AdRequest request = new AdRequest.Builder().Build(); //空のrequestを作成
        interstitialAd.LoadAd(request); //インタースティシャル広告をロード

        counter = PlayerPrefs.GetInt("ISCounter",0);
    }

    /// <summary>
    /// インタースティシャル広告を表示するメソッド
    /// </summary>
    /// <param name="num">インタースティシャル広告を表示する確率</param>
    public void RequestInterstitial()
    {
        if (counter < 2)
        {
            counter++;
            PlayerPrefs.SetInt("ISCounter", counter);
            PlayerPrefs.Save();

            return; //広告を出さずにリターン
        }

        if (this.interstitialAd.IsLoaded()) //インタースティシャル広告がロードされているかどうかを確認
        {
            counter = 0;
            PlayerPrefs.SetInt("ISCounter", counter);
            PlayerPrefs.Save();

            this.interstitialAd.Show(); //インタースティシャル広告の表示
        }
    }

    /// <summary>
    /// 広告が閉じられたときに行いたい処理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        interstitialAd.Destroy(); //メモリリーク阻止！
    }
}