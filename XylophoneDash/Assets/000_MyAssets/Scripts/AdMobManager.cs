using GoogleMobileAds.Api;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdMobManager : MonoBehaviour
{
    /// <summary> デバッグ時にはTrueでBanner/Interstitial非表示 </summary>
    private bool isDebug = false;

    private bool isHaveItem = false; 

    private BannerView bannerView;
    private InterstitialAd interstitial;
    private RewardedAd rewardedAd;

    private const string MENU_SCENE_NAME = "Menu";
    private const string GAME_SCENE_NAME = "Game";

    private const int DISPLAY_INTERSTITIAL_COUNT = 2;
    private const int TRY_TIMES_RE_CONNECT = 3;
    private const float COOL_TIME_RE_CONNECT = 3f;

    private int playfabMissCount = 0;

    private int bannerMissCount = 0;

    private int interCounter = 0;


#if UNITY_ANDROID
    private string adBannerUnitId = "ca-app-pub-7223826824285484/5711925826";  // 本番
    //private string adBannerUnitId = "ca-app-pub-3940256099942544/6300978111"; // テスト

    private string adIntersUnitId = "ca-app-pub-7223826824285484/1265414351"; // 本番
    //private string adIntersUnitId = "ca-app-pub-3940256099942544/1033173712"; // テスト

    private string adRewardUnitId = "ca-app-pub-7223826824285484/5044877266";  // 本番
    //private string adRewardUnitId = "ca-app-pub-3940256099942544/5224354917";  // テスト
#elif UNITY_IPHONE
    //private string adBannerUnitId = "広告ユニットIDをコピペ（iOS）";  // 本番
    private string adBannerUnitId = "ca-app-pub-7689051089863147/2788662322"; // テスト

    private string adIntersUnitId = "ca-app-pub-3940256099942544/4411468910"; // テスト

    //private string adRewardUnitId = "広告ユニットIDをコピペ（iOS）";  // 本番
    private string adRewardUnitId = "ca-app-pub-3940256099942544/1712485313";  //テスト
#else
    private string adBannerUnitId = "unexpected_platform";
    private string adIntersUnitId = "unexpected_platform";
    private string adRewardUnitId = "unexpected_platform";
#endif

    public static AdMobManager Instance;
    private void Awake()
    {
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
    }

    private void Start()
    {
        CheckItemPlayfab();
    }

    /// <summary>
    /// アイテム所持をPlayFabManagerに確認する
    /// </summary>
    private void CheckItemPlayfab()
    {
        if (PlayFabManager.Instance.GetIsLogined())
        {
            PlayFabManager.Instance.CheckHaveItemNoAdMob("NoAdMob");
        }
        else
        {
            Debug.Log($"AdMobManager.CheckItemPlayfab : Playfab is not Log in");

            playfabMissCount++;
            if (playfabMissCount < TRY_TIMES_RE_CONNECT)
            {
                Invoke(nameof(CheckItemPlayfab), COOL_TIME_RE_CONNECT);
            }
        }
    }

    /// <summary>
    /// Playfabからアイテムの所持状況を受信する
    /// </summary>
    /// <param name="flg">持っていたらtrue</param>
    public void ReceptionPlayFabForItem(bool flg)
    {
        isHaveItem = flg;
        AllAdRequest();
    }

    /// <summary>
    /// 初期化、一度は呼び出さなければならない
    /// </summary>
    private static void Initialize()
    {
        // Google AdMob Initial
        MobileAds.Initialize(initStatus => { });
    }

    /// <summary>
    /// 全ての広告を要求する
    /// </summary>
    public void AllAdRequest()
    {
        if (!GetNoAdFlg())
        {
            BannerRequest();
            RequestInterstitial();
        }

        RewardInit();
    }

    /// <summary>
    /// 広告を表示しない場合のフラグを返す
    /// </summary>
    /// <returns>非表示ならTrue</returns>
    public bool GetNoAdFlg()
    {
        return isDebug || isHaveItem;
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

        // Create a 320x50 banner at the bottom of the screen.
        //bannerView = new BannerView(adBannerUnitId, AdSize.Banner, AdPosition.Top);

        //バナーを生成 new BannerView(バナーID,バナーサイズ,バナー表示位置)
        bannerView = new BannerView(adBannerUnitId, adaptiveSize, AdPosition.Top);

        //BannerView型の変数 bannerViewの各種状態 に関数を登録
        bannerView.OnAdLoaded += HandleBannerLoaded;//bannerViewの状態が バナー表示完了 となった時に起動する関数(関数名HandleAdLoaded)を登録
        bannerView.OnAdFailedToLoad += HandleBannerFailedToLoad;//bannerViewの状態が バナー読み込み失敗 となった時に起動する関数(関数名HandleAdFailedToLoad)を登録

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        bannerView.LoadAd(request);

    }

    /// <summary>
    /// バナー表示完了となった時に起動するハンドル
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void HandleBannerLoaded(object sender, EventArgs args)
    {
        Debug.Log("AdMobManager.HandleBannerLoaded : バナー表示 成功");
    }

    /// <summary>
    /// バナー読み込み失敗となった時に起動するハンドル
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void HandleBannerFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        bannerMissCount++;

        if (bannerMissCount < TRY_TIMES_RE_CONNECT)
        {
            Invoke(nameof(BannerRequest), COOL_TIME_RE_CONNECT);
        }

        Debug.Log("AdMobManager.HandleBannerFailedToLoad : バナー読み込み 失敗\n" + args.LoadAdError);//args.LoadAdError:エラー内容 
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

#region Interstitial

    /// <summary>
    /// インタースティシャル広告を呼び出す
    /// </summary>
    public void RequestInterstitial()
    {
        interstitial = new InterstitialAd(adIntersUnitId);
        DestroyInterstitialAd();

        interstitial.OnAdLoaded += HandleOnAdLoaded; // Called when an ad request has successfully loaded.
        interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad; // Called when an ad request failed to load.
        interstitial.OnAdOpening += HandleOnAdOpened; // Called when an ad is shown.
        interstitial.OnAdClosed += HandleOnAdClosed; // Called when the ad is closed.

        AdRequest request = new AdRequest.Builder().Build();
        interstitial.LoadAd(request);
    }

    /// <summary>
    /// インタースティシャル広告を表示する
    /// </summary>
    public void ShowInterstitialAd()
    {
        bool showFlg = false;
        interCounter++;

        if (interCounter >= DISPLAY_INTERSTITIAL_COUNT)
        {
            if (interstitial.IsLoaded())
            {
                showFlg = true;
                interstitial.Show();
            }
            else
            {
                Debug.LogError("AdMobManager.ShowInterstitialAd : まだ読み込みができていない");
            }
        }

        if (SceneManager.GetActiveScene().name == GAME_SCENE_NAME)
        {
            GameSceneManager.Instance.ReceptionAdMobManagerForInterstitial(showFlg);
        }
    }

    /// <summary>
    /// インタースティシャル広告を破壊する
    /// </summary>
    public void DestroyInterstitialAd()
    {
        interstitial.Destroy();
    }

    /// <summary>
    /// 広告の読み込み完了時に呼ばれるハンドル
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        Debug.Log("AdMobManager.HandleOnAdLoaded : event received");
    }

    /// <summary>
    /// 広告がデバイスの画面いっぱいに表示されたときに呼ばれるハンドル
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        Debug.Log("AdMobManager.HandleOnAdOpened : event received");
    }

    /// <summary>
    /// 広告の読み込み失敗時に呼ばれるハンドル
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        Debug.Log("AdMobManager.HandleOnAdFailedToLoad : event received");
    }

    /// <summary>
    /// 広告が閉じられたときに呼ばれるハンドル
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        Debug.Log("AdMobManager.HandleOnAdClosed : event received");

        interCounter = 0;

        RequestInterstitial();

        if (SceneManager.GetActiveScene().name == GAME_SCENE_NAME)
        {
            GameSceneManager.Instance.ReceptionAdMobManagerForInterstitial(false);
        }
    }

#endregion

#region Reward

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
            MenuSceneManager.Instance.FinishReward(false);
        }
        else
        {
            Debug.LogError($"AdMobManager.HandleRewardedAdClosed : Scene Name is {SceneManager.GetActiveScene().name}");
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
            MenuSceneManager.Instance.FinishReward(true);
        }
        else
        {
            Debug.LogError($"AdMobManager.RewardContent : Scene Name is {SceneManager.GetActiveScene().name}");
        }

        PlayerInformationManager.Instance.UpdateRewardTimes();
    }

#endregion

}
