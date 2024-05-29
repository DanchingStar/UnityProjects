using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdMobInitialize : MonoBehaviour
{
    private BannerView bannerView;

    public void Start()
    {
        // Google AdMob Initial
        MobileAds.Initialize(initStatus => { });
    }
}