using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdMobManager : MonoBehaviour
{
    /// <summary> �f�o�b�O���ɂ�True��Banner��\�� </summary>
    private bool isDebug = false;

    private BannerView bannerView;
    private RewardedAd rewardedAd;

    private const string MENU_SCENE_NAME = "Menu";

#if UNITY_ANDROID
    private string adBannerUnitId = "ca-app-pub-7223826824285484/4714229844";  // �{��
    //private string adBannerUnitId = "ca-app-pub-3940256099942544/6300978111"; // �e�X�g

    private string adRewardUnitId = "ca-app-pub-7223826824285484/1803875917";  // �{��
    //private string adRewardUnitId = "ca-app-pub-3940256099942544/5224354917";  // �e�X�g
#elif UNITY_IPHONE
    //private string adBannerUnitId = "�L�����j�b�gID���R�s�y�iiOS�j";  // �{��
    private string adBannerUnitId = "ca-app-pub-7689051089863147/2788662322"; // �e�X�g

    //private string adRewardUnitId = "�L�����j�b�gID���R�s�y�iiOS�j";  // �{��
    private string adRewardUnitId = "ca-app-pub-3940256099942544/1712485313";  //�e�X�g
#else
    private string adBannerUnitId = "unexpected_platform";
    private string adRewardUnitId = "unexpected_platform";
#endif

    public static AdMobManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            // �V�[���J�ڂ��Ă��j������Ȃ��悤�ɂ���
            Instance = this;
            DontDestroyOnLoad(gameObject);

            Initialize();
        }
        else
        {
            // ��d�ŋN������Ȃ��悤�ɂ���
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// �������A��x�͌Ăяo���Ȃ���΂Ȃ�Ȃ�
    /// </summary>
    static void Initialize()
    {
        // Google AdMob Initial
        MobileAds.Initialize(initStatus => { });
    }

    /// <summary>
    /// Start�֐�
    /// </summary>
    private void Start()
    {
        if (isDebug) return;

        BannerRequest();
        RewardInit();
    }

    /// <summary>
    /// �o�i�[�L�����Ăяo��
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
    /// �o�i�[����
    /// </summary>
    public void BannerDestroy()
    {
        if (this.bannerView != null)
        {
            this.bannerView.Destroy();
        }
    }

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
            Menu.MenuSceneManager.Instance.FinishReward(false);
        }
        else
        {
            UICommonPanelManager.Instance.FinishReward(false);
            //Debug.LogError($"AdMobManager.HandleRewardedAdClosed : Scene Name is {SceneManager.GetActiveScene().name}");
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
            Menu.MenuSceneManager.Instance.FinishReward(true);
        }
        else
        {
            UICommonPanelManager.Instance.FinishReward(true);
        }

        PlayerInformationManager.Instance.UpdateRewardTimes();
    }

    /// <summary>
    /// RewardPanel�����Ƃ�
    /// </summary>
    public void CloseRewardPanel()
    {

    }


}
