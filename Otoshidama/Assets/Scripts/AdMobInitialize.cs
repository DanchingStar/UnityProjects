using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdMobInitialize : MonoBehaviour
{
    // �Q�[���J�n�O�ɌĂяo��
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        // Google AdMob Initial
        MobileAds.Initialize(initStatus => { });
    }
}