using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdMobManager : MonoBehaviour
{
    /// <summary> デバッグ時にはTrueでBanner/Interstitial非表示 </summary>
    private bool isDebug = false;

    private BannerView bannerView;
    private InterstitialAd interstitial;
    private RewardedAd rewardedAd;

    private const string MENU_SCENE_NAME = "Menu";
    private const string GAME_SCENE_NAME = "Game";

    private const int TRY_TIMES_RE_CONNECT = 3;
    private const float COOL_TIME_RE_CONNECT = 3f;

    private int bannerMissCount = 0;

    private EditDiceMenuPanelPrefab receptionEditDiceMenuPanelPrefab;
    private SimulationPanelPrefab receptionSimulationPanelPrefab;

#if UNITY_ANDROID
    private string adBannerUnitId = "ca-app-pub-7223826824285484/7575971522";  // 本番
    //private string adBannerUnitId = "ca-app-pub-3940256099942544/6300978111"; // テスト

    private string adRewardUnitId = "ca-app-pub-7223826824285484/2492865928";  // 本番
    //private string adRewardUnitId = "ca-app-pub-3940256099942544/5224354917";  // テスト
#elif UNITY_IPHONE
    //private string adBannerUnitId = "広告ユニットIDをコピペ（iOS）";  // 本番
    private string adBannerUnitId = "ca-app-pub-7689051089863147/2788662322"; // テスト

    //private string adRewardUnitId = "広告ユニットIDをコピペ（iOS）";  // 本番
    private string adRewardUnitId = "ca-app-pub-3940256099942544/1712485313";  //テスト
#else
    private string adBannerUnitId = "unexpected_platform";
    private string adRewardUnitId = "unexpected_platform";
#endif

    public static AdMobManager Instance;
    private void Awake()
    {
        //Debug.Log("AdMobManager.Awake : In");
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーン遷移しても破棄されないようにする

            Initialize();
        }
        else
        {
            // 二重で起動されないようにする
            Destroy(gameObject);
        }
        //Debug.Log("AdMobManager.Awake : Out");
    }

    private void Start()
    {
        AllAdRequest();
    }


    /// <summary>
    /// 初期化、一度は呼び出さなければならない
    /// </summary>
    private static void Initialize()
    {
        // イベントをメインスレッドと同期させる
        MobileAds.RaiseAdEventsOnUnityMainThread = true;

        // Google AdMob Initial
        MobileAds.Initialize(initStatus => { Debug.Log("AdMobManager.Initialize : complete !"); });
    }

    /// <summary>
    /// 全ての広告を要求する
    /// </summary>
    public void AllAdRequest()
    {
        if (!GetDebugFlg())
        {
            BannerRequest();
        }

        RewardInit();
    }

    /// <summary>
    /// デバッグモードかのフラグを返すゲッター
    /// </summary>
    /// <returns></returns>
    public bool GetDebugFlg()
    {
        return isDebug;
    }

#region Banner

    /// <summary>
    /// バナー広告を呼び出す
    /// </summary>
    public void BannerRequest()
    {
        BannerDestroy();

        AdSize adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);

        //バナーを生成 new BannerView(バナーID,バナーサイズ,バナー表示位置)
        bannerView = new BannerView(adBannerUnitId, adaptiveSize, AdPosition.Top);

        //BannerView型の変数 bannerViewの各種状態 に関数を登録
        bannerView.OnBannerAdLoaded += () => HandleBannerLoaded();
        bannerView.OnBannerAdLoadFailed += (LoadAdError error) => HandleBannerFailedToLoad(error);

        // Create an empty ad request.
        AdRequest request = new AdRequest();

        // Load the banner with the request.
        bannerView.LoadAd(request);

    }

    /// <summary>
    /// バナー表示完了となった時に起動するハンドル
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void HandleBannerLoaded()
    {
        Debug.Log("AdMobManager.HandleBannerLoaded : バナー表示 成功");
    }

    /// <summary>
    /// バナー読み込み失敗となった時に起動するハンドル
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void HandleBannerFailedToLoad(LoadAdError error)
    {
        bannerMissCount++;

        if (bannerMissCount < TRY_TIMES_RE_CONNECT)
        {
            Invoke(nameof(BannerRequest), COOL_TIME_RE_CONNECT);
        }

        Debug.Log("AdMobManager.HandleBannerFailedToLoad : バナー読み込み 失敗\n" + error);
    }

    /// <summary>
    /// バナーを壊す
    /// </summary>
    public void BannerDestroy()
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
        }
    }

#endregion

#region Reward

    /// <summary>
    /// リワード広告の初期化
    /// </summary>
    private void RewardInit()
    {
        RequestReward(); //広告をロード
    }

    /// <summary>
    /// Reward広告をロード
    /// </summary>
    private void RequestReward()
    {
        if (rewardedAd != null)
        {

            rewardedAd.Destroy();
            rewardedAd = null;
        }

        AdRequest request = new AdRequest();

        RewardedAd.Load(adRewardUnitId, request,
            (RewardedAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError("Rewarded ad failed to load an ad " + "with error : " + error);
                    return;
                }

                Debug.Log("Rewarded ad loaded with response : " + ad.GetResponseInfo());

                rewardedAd = ad;
            });

        // 広告を閉じる時に実行する関数の登録
        rewardedAd.OnAdFullScreenContentClosed += () => HandleRewardedAdClosed();

    }

    /// <summary>
    /// 広告とのインタラクションでユーザーに報酬を与えるべきときに呼び出されるハンドル
    /// </summary>
    public void HandleUserEarnedReward(Reward args)
    {
        RewardContent();
        Debug.Log("AdMobManager.HandleUserEarnedReward : Get Reward!");
    }

    /// <summary>
    /// 広告が閉じられたときに呼び出されるハンドル
    /// </summary>
    public void HandleRewardedAdClosed()
    {
        RequestReward();

        if (SceneManager.GetActiveScene().name == MENU_SCENE_NAME)
        {
            receptionEditDiceMenuPanelPrefab.ReceptionAdMobReward(false);
        }
        else if (SceneManager.GetActiveScene().name == GAME_SCENE_NAME)
        {
            receptionSimulationPanelPrefab.ReceptionAdMobReward(false);
        }
        else
        {
            Debug.LogError($"AdMobManager.HandleRewardedAdClosed : Scene Name is {SceneManager.GetActiveScene().name}");
        }
    }

    /// <summary>
    /// Reward呼び出し(シミュレーション実行)
    /// </summary>
    public void ShowRewardForSimulation(SimulationPanelPrefab _simulationPanelPrefab)
    {
        receptionSimulationPanelPrefab = _simulationPanelPrefab;

        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) => HandleUserEarnedReward(reward));
        }
    }

    /// <summary>
    /// Reward呼び出し(マイセットの枠追加)
    /// </summary>
    public void ShowRewardForAddMySet(EditDiceMenuPanelPrefab _editDiceMenuPanelPrefab)
    {
        receptionEditDiceMenuPanelPrefab = _editDiceMenuPanelPrefab;

        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) => HandleUserEarnedReward(reward));
        }
    }

    /// <summary>
    /// Reward報酬の内容
    /// </summary>
    private void RewardContent()
    {
        StartCoroutine(EarnedReward());
    }

    /// <summary>
    /// Reward後のフレーム消費(クラッシュ対策)
    /// </summary>
    private IEnumerator EarnedReward()
    {
        // 数F待つことでクラッシュを回避 , 1Fでも動いたけど念には念をで2～3F程度待っとけば安全かも？
        yield return new WaitForSeconds(0.1f);
        yield return null;

        // 重いリワード付与処理
        if (SceneManager.GetActiveScene().name == MENU_SCENE_NAME)
        {
            receptionEditDiceMenuPanelPrefab.ReceptionAdMobReward(true);
        }
        else if (SceneManager.GetActiveScene().name == GAME_SCENE_NAME)
        {
            receptionSimulationPanelPrefab.ReceptionAdMobReward(true);
        }
        else
        {
            Debug.LogError($"AdMobManager.RewardContent : Scene Name is {SceneManager.GetActiveScene().name}");
        }

        RequestReward();
    }

    #endregion

}