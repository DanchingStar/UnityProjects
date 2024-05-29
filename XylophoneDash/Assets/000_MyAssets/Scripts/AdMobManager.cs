using GoogleMobileAds.Api;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdMobManager : MonoBehaviour
{
    /// <summary> �f�o�b�O���ɂ�True��Banner/Interstitial��\�� </summary>
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
    private string adBannerUnitId = "ca-app-pub-7223826824285484/5711925826";  // �{��
    //private string adBannerUnitId = "ca-app-pub-3940256099942544/6300978111"; // �e�X�g

    private string adIntersUnitId = "ca-app-pub-7223826824285484/1265414351"; // �{��
    //private string adIntersUnitId = "ca-app-pub-3940256099942544/1033173712"; // �e�X�g

    private string adRewardUnitId = "ca-app-pub-7223826824285484/5044877266";  // �{��
    //private string adRewardUnitId = "ca-app-pub-3940256099942544/5224354917";  // �e�X�g
#elif UNITY_IPHONE
    //private string adBannerUnitId = "�L�����j�b�gID���R�s�y�iiOS�j";  // �{��
    private string adBannerUnitId = "ca-app-pub-7689051089863147/2788662322"; // �e�X�g

    private string adIntersUnitId = "ca-app-pub-3940256099942544/4411468910"; // �e�X�g

    //private string adRewardUnitId = "�L�����j�b�gID���R�s�y�iiOS�j";  // �{��
    private string adRewardUnitId = "ca-app-pub-3940256099942544/1712485313";  //�e�X�g
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
            DontDestroyOnLoad(gameObject); // �V�[���J�ڂ��Ă��j������Ȃ��悤�ɂ���

            Initialize();
        }
        else
        {
            // ��d�ŋN������Ȃ��悤�ɂ���
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        CheckItemPlayfab();
    }

    /// <summary>
    /// �A�C�e��������PlayFabManager�Ɋm�F����
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
    /// Playfab����A�C�e���̏����󋵂���M����
    /// </summary>
    /// <param name="flg">�����Ă�����true</param>
    public void ReceptionPlayFabForItem(bool flg)
    {
        isHaveItem = flg;
        AllAdRequest();
    }

    /// <summary>
    /// �������A��x�͌Ăяo���Ȃ���΂Ȃ�Ȃ�
    /// </summary>
    private static void Initialize()
    {
        // Google AdMob Initial
        MobileAds.Initialize(initStatus => { });
    }

    /// <summary>
    /// �S�Ă̍L����v������
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
    /// �L����\�����Ȃ��ꍇ�̃t���O��Ԃ�
    /// </summary>
    /// <returns>��\���Ȃ�True</returns>
    public bool GetNoAdFlg()
    {
        return isDebug || isHaveItem;
    }

    /// <summary>
    /// �f�o�b�O���[�h���̃t���O��Ԃ��Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public bool GetDebugFlg()
    {
        return isDebug;
    }

#region Banner

    /// <summary>
    /// �o�i�[�L�����Ăяo��
    /// </summary>
    public void BannerRequest()
    {
        BannerDestroy();

        AdSize adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);

        // Create a 320x50 banner at the bottom of the screen.
        //bannerView = new BannerView(adBannerUnitId, AdSize.Banner, AdPosition.Top);

        //�o�i�[�𐶐� new BannerView(�o�i�[ID,�o�i�[�T�C�Y,�o�i�[�\���ʒu)
        bannerView = new BannerView(adBannerUnitId, adaptiveSize, AdPosition.Top);

        //BannerView�^�̕ϐ� bannerView�̊e���� �Ɋ֐���o�^
        bannerView.OnAdLoaded += HandleBannerLoaded;//bannerView�̏�Ԃ� �o�i�[�\������ �ƂȂ������ɋN������֐�(�֐���HandleAdLoaded)��o�^
        bannerView.OnAdFailedToLoad += HandleBannerFailedToLoad;//bannerView�̏�Ԃ� �o�i�[�ǂݍ��ݎ��s �ƂȂ������ɋN������֐�(�֐���HandleAdFailedToLoad)��o�^

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        bannerView.LoadAd(request);

    }

    /// <summary>
    /// �o�i�[�\�������ƂȂ������ɋN������n���h��
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void HandleBannerLoaded(object sender, EventArgs args)
    {
        Debug.Log("AdMobManager.HandleBannerLoaded : �o�i�[�\�� ����");
    }

    /// <summary>
    /// �o�i�[�ǂݍ��ݎ��s�ƂȂ������ɋN������n���h��
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

        Debug.Log("AdMobManager.HandleBannerFailedToLoad : �o�i�[�ǂݍ��� ���s\n" + args.LoadAdError);//args.LoadAdError:�G���[���e 
    }

    /// <summary>
    /// �o�i�[����
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
    /// �C���^�[�X�e�B�V�����L�����Ăяo��
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
    /// �C���^�[�X�e�B�V�����L����\������
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
                Debug.LogError("AdMobManager.ShowInterstitialAd : �܂��ǂݍ��݂��ł��Ă��Ȃ�");
            }
        }

        if (SceneManager.GetActiveScene().name == GAME_SCENE_NAME)
        {
            GameSceneManager.Instance.ReceptionAdMobManagerForInterstitial(showFlg);
        }
    }

    /// <summary>
    /// �C���^�[�X�e�B�V�����L����j�󂷂�
    /// </summary>
    public void DestroyInterstitialAd()
    {
        interstitial.Destroy();
    }

    /// <summary>
    /// �L���̓ǂݍ��݊������ɌĂ΂��n���h��
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        Debug.Log("AdMobManager.HandleOnAdLoaded : event received");
    }

    /// <summary>
    /// �L�����f�o�C�X�̉�ʂ����ς��ɕ\�����ꂽ�Ƃ��ɌĂ΂��n���h��
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        Debug.Log("AdMobManager.HandleOnAdOpened : event received");
    }

    /// <summary>
    /// �L���̓ǂݍ��ݎ��s���ɌĂ΂��n���h��
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        Debug.Log("AdMobManager.HandleOnAdFailedToLoad : event received");
    }

    /// <summary>
    /// �L��������ꂽ�Ƃ��ɌĂ΂��n���h��
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
    /// �����[�h�L���̏�����
    /// </summary>
    private void RewardInit()
    {
        this.rewardedAd = new RewardedAd(adRewardUnitId);

        // Load�������Ɏ��s����֐��̓o�^
        //this.rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        // Load���s���Ɏ��s����֐��̓o�^
        //this.rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        // �\�����Ɏ��s����֐��̓o�^
        //this.rewardedAd.OnAdOpening += HandleRewardedAdOpening;
        // �\�����s���Ɏ��s����֐��̓o�^
        //this.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        // ��V�󂯎�莞�Ɏ��s����֐��̓o�^
        this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        // �L������鎞�Ɏ��s����֐��̓o�^
        this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;

        RequestReward(); //�L�������[�h
    }

    /// <summary>
    /// Reward�L�������[�h
    /// </summary>
    private void RequestReward()
    {
        AdRequest request = new AdRequest.Builder().Build();
        this.rewardedAd.LoadAd(request);
    }

    /// <summary>
    /// �L���Ƃ̃C���^���N�V�����Ń��[�U�[�ɕ�V��^����ׂ��Ƃ��ɌĂяo�����n���h��
    /// </summary>
    public void HandleUserEarnedReward(object sender, Reward args)
    {
        RewardContent();
    }

    /// <summary>
    /// �L��������ꂽ�Ƃ��ɌĂяo�����n���h��
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
    /// ������ĂׂΓ��悪�����i�Ⴆ�΃{�^���������Ȃǁj
    /// </summary>
    public void ShowReward()
    {
        if (this.rewardedAd.IsLoaded())
        {
            this.rewardedAd.Show();
        }
    }

    /// <summary>
    /// Reward��V�̓��e
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
