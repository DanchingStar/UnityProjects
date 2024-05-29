using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdMobBanner: MonoBehaviour
{
    /// <summary> デバッグ時にはTrueでBanner非表示 </summary>
    private bool isDebug = false;

    private BannerView bannerView;

#if UNITY_ANDROID
    private string adUnitId = "ca-app-pub-7223826824285484/9056931043";  //本番
    //private string adUnitId = "ca-app-pub-3940256099942544/6300978111"; // テスト
#elif UNITY_IPHONE
    private string adUnitId = "ca-app-pub-7689051089863147/2788662322"; // テスト用広告ユニットID
#else
    private string adUnitId = "unexpected_platform";
#endif

    private void Start()
    {
        if (isDebug) return;

        RequestBanner();
    }

    public void RequestBanner()
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
        }

        // Create a 320x50 banner at the bottom of the screen.
        this.bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        bannerView.LoadAd(request);

    }

    public void DestroyBanner()
    {
        if (this.bannerView != null) 
        {
            this.bannerView.Destroy();
        }
    }

    //private void OnDestroy()
    //{
    //    this.bannerView.Destroy();
    //}
}