using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdMobManager : MonoBehaviour
{
    /// <summary> �f�o�b�O���ɂ�True��Banner/Interstitial��\�� </summary>
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
    private string adBannerUnitId = "ca-app-pub-7223826824285484/7575971522";  // �{��
    //private string adBannerUnitId = "ca-app-pub-3940256099942544/6300978111"; // �e�X�g

    private string adRewardUnitId = "ca-app-pub-7223826824285484/2492865928";  // �{��
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
        //Debug.Log("AdMobManager.Awake : In");
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
        //Debug.Log("AdMobManager.Awake : Out");
    }

    private void Start()
    {
        AllAdRequest();
    }


    /// <summary>
    /// �������A��x�͌Ăяo���Ȃ���΂Ȃ�Ȃ�
    /// </summary>
    private static void Initialize()
    {
        // �C�x���g�����C���X���b�h�Ɠ���������
        MobileAds.RaiseAdEventsOnUnityMainThread = true;

        // Google AdMob Initial
        MobileAds.Initialize(initStatus => { Debug.Log("AdMobManager.Initialize : complete !"); });
    }

    /// <summary>
    /// �S�Ă̍L����v������
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

        //�o�i�[�𐶐� new BannerView(�o�i�[ID,�o�i�[�T�C�Y,�o�i�[�\���ʒu)
        bannerView = new BannerView(adBannerUnitId, adaptiveSize, AdPosition.Top);

        //BannerView�^�̕ϐ� bannerView�̊e���� �Ɋ֐���o�^
        bannerView.OnBannerAdLoaded += () => HandleBannerLoaded();
        bannerView.OnBannerAdLoadFailed += (LoadAdError error) => HandleBannerFailedToLoad(error);

        // Create an empty ad request.
        AdRequest request = new AdRequest();

        // Load the banner with the request.
        bannerView.LoadAd(request);

    }

    /// <summary>
    /// �o�i�[�\�������ƂȂ������ɋN������n���h��
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void HandleBannerLoaded()
    {
        Debug.Log("AdMobManager.HandleBannerLoaded : �o�i�[�\�� ����");
    }

    /// <summary>
    /// �o�i�[�ǂݍ��ݎ��s�ƂȂ������ɋN������n���h��
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

        Debug.Log("AdMobManager.HandleBannerFailedToLoad : �o�i�[�ǂݍ��� ���s\n" + error);
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

#region Reward

    /// <summary>
    /// �����[�h�L���̏�����
    /// </summary>
    private void RewardInit()
    {
        RequestReward(); //�L�������[�h
    }

    /// <summary>
    /// Reward�L�������[�h
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

        // �L������鎞�Ɏ��s����֐��̓o�^
        rewardedAd.OnAdFullScreenContentClosed += () => HandleRewardedAdClosed();

    }

    /// <summary>
    /// �L���Ƃ̃C���^���N�V�����Ń��[�U�[�ɕ�V��^����ׂ��Ƃ��ɌĂяo�����n���h��
    /// </summary>
    public void HandleUserEarnedReward(Reward args)
    {
        RewardContent();
        Debug.Log("AdMobManager.HandleUserEarnedReward : Get Reward!");
    }

    /// <summary>
    /// �L��������ꂽ�Ƃ��ɌĂяo�����n���h��
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
    /// Reward�Ăяo��(�V�~�����[�V�������s)
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
    /// Reward�Ăяo��(�}�C�Z�b�g�̘g�ǉ�)
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
    /// Reward��V�̓��e
    /// </summary>
    private void RewardContent()
    {
        StartCoroutine(EarnedReward());
    }

    /// <summary>
    /// Reward��̃t���[������(�N���b�V���΍�)
    /// </summary>
    private IEnumerator EarnedReward()
    {
        // ��F�҂��ƂŃN���b�V������� , 1F�ł����������ǔO�ɂ͔O����2�`3F���x�҂��Ƃ��Έ��S�����H
        yield return new WaitForSeconds(0.1f);
        yield return null;

        // �d�������[�h�t�^����
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