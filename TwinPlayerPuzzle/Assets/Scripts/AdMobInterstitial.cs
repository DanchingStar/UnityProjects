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
        adUnitId = "ca-app-pub-7223826824285484/2348336599";  //本番
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
    /// <param name="num">インタースティシャル広告を表示する確率</param>
    public void RequestInterstitial(int num)
    {
        if (num < 0 || num > 100) 
        {
            num = 30;
        }
        int number = UnityEngine.Random.Range(1, 100); //1から100までの整数を生成
        if(number <= 100 - num) //num%の確率で
        {
            return; //広告を出さずにリターン
        }

        if (this.interstitialAd.IsLoaded()) //インタースティシャル広告がロードされているかどうかを確認
        {
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