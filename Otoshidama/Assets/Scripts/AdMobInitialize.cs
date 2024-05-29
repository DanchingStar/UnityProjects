using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdMobInitialize : MonoBehaviour
{
    // ゲーム開始前に呼び出す
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        // Google AdMob Initial
        MobileAds.Initialize(initStatus => { });
    }
}