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
        adUnitId = "ca-app-pub-7223826824285484/3625739106";  //本番
        //adUnitId = "ca-app-pub-3940256099942544/1033173712";  //テスト
#elif UNITY_IOS
        //adUnitId = "広告ユニットIDをコピペ（iOS）";  //本番
        adUnitId = "ca-app-pub-3940256099942544/4411468910";  //テスト
#endif
        this.interstitialAd = new InterstitialAd(adUnitId); //InterstitialAdをインスタンス化
        this.interstitialAd.OnAdClosed += HandleOnAdClosed; //動画が閉じられたときに「HandleOnAdClosed」が呼び出される
        AdRequest request = new AdRequest.Builder().Build(); //空のrequestを作成
        interstitialAd.LoadAd(request); //インタースティシャル広告をロード
    }

    /// <summary>
    /// インタースティシャル広告を表示するメソッド
    /// </summary>
    public void RequestInterstitial(int counter)
    {
        if (counter < 2)
        {
            counter++;
            PlayerPrefs.SetInt("InterstitialCounter", counter);
            PlayerPrefs.Save();

            return; //広告を出さずにリターン
        }

        if (this.interstitialAd.IsLoaded()) //インタースティシャル広告がロードされているかどうかを確認
        {
            this.interstitialAd.Show(); //インタースティシャル広告の表示

            Start();

            PlayerPrefs.SetInt("InterstitialCounter", 0);
            PlayerPrefs.Save();
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