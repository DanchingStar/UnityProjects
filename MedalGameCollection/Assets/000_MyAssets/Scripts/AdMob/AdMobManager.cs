using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdMobManager : MonoBehaviour
{
    /// <summary> デバッグ時にはTrueでBanner非表示 </summary>
    private bool isDebug = false;

    private BannerView bannerView;
    private RewardedAd rewardedAd;

    private const string MENU_SCENE_NAME = "Menu";

#if UNITY_ANDROID
    private string adBannerUnitId = "ca-app-pub-7223826824285484/4714229844";  // 本番
    //private string adBannerUnitId = "ca-app-pub-3940256099942544/6300978111"; // テスト

    private string adRewardUnitId = "ca-app-pub-7223826824285484/1803875917";  // 本番
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
        if (Instance == null)
        {
            // シーン遷移しても破棄されないようにする
            Instance = this;
            DontDestroyOnLoad(gameObject);

            Initialize();
        }
        else
        {
            // 二重で起動されないようにする
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 初期化、一度は呼び出さなければならない
    /// </summary>
    static void Initialize()
    {
        // Google AdMob Initial
        MobileAds.Initialize(initStatus => { });
    }

    /// <summary>
    /// Start関数
    /// </summary>
    private void Start()
    {
        if (isDebug) return;

        BannerRequest();
        RewardInit();
    }

    /// <summary>
    /// バナー広告を呼び出す
    /// </summary>
    public void BannerRequest()
    {
        BannerDestroy();

        // Create a 320x50 banner at the bottom of the screen.
        this.bannerView = new BannerView(adBannerUnitId, AdSize.Banner, AdPosition.Bottom);

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        bannerView.LoadAd(request);

    }

    /// <summary>
    /// バナーを壊す
    /// </summary>
    public void BannerDestroy()
    {
        if (this.bannerView != null)
        {
            this.bannerView.Destroy();
        }
    }

    /// <summary>
    /// リワード広告の初期化
    /// </summary>
    private void RewardInit()
    {
        this.rewardedAd = new RewardedAd(adRewardUnitId);

        // Load成功時に実行する関数の登録
        //this.rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        // Load失敗時に実行する関数の登録
        //this.rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        // 表示時に実行する関数の登録
        //this.rewardedAd.OnAdOpening += HandleRewardedAdOpening;
        // 表示失敗時に実行する関数の登録
        //this.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        // 報酬受け取り時に実行する関数の登録
        this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        // 広告を閉じる時に実行する関数の登録
        this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;

        RequestReward(); //広告をロード

    }

    /// <summary>
    /// Reward広告をロード
    /// </summary>
    private void RequestReward()
    {
        AdRequest request = new AdRequest.Builder().Build();
        this.rewardedAd.LoadAd(request);
    }

    /// <summary>
    /// 広告とのインタラクションでユーザーに報酬を与えるべきときに呼び出されるハンドル
    /// </summary>
    public void HandleUserEarnedReward(object sender, Reward args)
    {
        RewardContent();
    }

    /// <summary>
    /// 広告が閉じられたときに呼び出されるハンドル
    /// </summary>
    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        RequestReward();

        if (SceneManager.GetActiveScene().name == MENU_SCENE_NAME)
        {
            Menu.MenuSceneManager.Instance.FinishReward(false);
        }
        else
        {
            UICommonPanelManager.Instance.FinishReward(false);
            //Debug.LogError($"AdMobManager.HandleRewardedAdClosed : Scene Name is {SceneManager.GetActiveScene().name}");
        }
    }

    /// <summary>
    /// これを呼べば動画が流れる（例えばボタン押下時など）
    /// </summary>
    public void ShowReward()
    {
        if (this.rewardedAd.IsLoaded())
        {
            this.rewardedAd.Show();
        }
    }

    /// <summary>
    /// Reward報酬の内容
    /// </summary>
    private void RewardContent()
    {
        if (SceneManager.GetActiveScene().name == MENU_SCENE_NAME)
        {
            Menu.MenuSceneManager.Instance.FinishReward(true);
        }
        else
        {
            UICommonPanelManager.Instance.FinishReward(true);
        }

        PlayerInformationManager.Instance.UpdateRewardTimes();
    }

    /// <summary>
    /// RewardPanelを閉じるとき
    /// </summary>
    public void CloseRewardPanel()
    {

    }


}
