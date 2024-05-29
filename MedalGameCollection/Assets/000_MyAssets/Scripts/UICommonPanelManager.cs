using DropBalls;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UICommonPanelManager : MonoBehaviour
{
    [SerializeField] private Text haveMedalText;
    [SerializeField] private GameObject menuPamel;
    [SerializeField] private GameObject howToPlayPamel;
    [SerializeField] private GameObject medalChargePamel;
    [SerializeField] private GameObject soundSettingPamel;
    [SerializeField] private GameObject checkToMenuPanel;

    [SerializeField] private ScrollRect howToPlayScrollView;
    [SerializeField] private Slider myBGMSlider;
    [SerializeField] private Slider mySESlider;
    //[SerializeField] private GameObject BGMObject;
    //[SerializeField] private GameObject SEObjectParent;

    [SerializeField] private GameObject rewardFinishPanel;
    [SerializeField] private Text rewardShowText1;
    [SerializeField] private Text rewardShowText2;

    [SerializeField] private UiSeManager uiSeManager;

    private const string BGM_VOLUME_KEY = "BGMVolume";
    private const string SE_VOLUME_KEY = "SEVolume";
    //private AudioSource myAudioBGM;
    //private Transform myAudioTF;
    private float BGM_Volume;
    private float SE_Volume;

    private bool isActiveSoundSettingPanel = false;

    private int haveMedal;

    private const int GET_REWARD_MEDALS = 25;

    private bool rewardFlg;

    public static UICommonPanelManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitSlider();
        UpdateHaveMedal();
        rewardFlg = false;
    }

    private void Update()
    {
        if (haveMedal != PlayerInformationManager.Instance.GetHaveMedal())
        {
            UpdateHaveMedal();
        }
    }

    /// <summary>
    /// 音量のスライダーの初期設定
    /// </summary>
    private void InitSlider()
    {
        //myAudioBGM = BGMObject.GetComponent<AudioSource>();
        //myAudioTF = SEObjectParent.GetComponent<Transform>();

        myBGMSlider.value = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, 0.5f);
        mySESlider.value = PlayerPrefs.GetFloat(SE_VOLUME_KEY, 0.5f);

        //myAudioBGM.volume = myBGMSlider.value;
        //foreach (Transform childTF in myAudioTF)
        //{
        //    childTF.gameObject.GetComponent<AudioSource>().volume = mySESlider.value;
        //}

        //myBGMSlider.onValueChanged.AddListener(value => myAudioBGM.volume = value);
        //foreach (Transform childTF in myAudioTF)
        //{
        //    mySESlider.onValueChanged.AddListener(value => childTF.gameObject.GetComponent<AudioSource>().volume = value);
        //}

        BGM_Volume = myBGMSlider.value;
        myBGMSlider.onValueChanged.AddListener(value => BGM_Volume = value);

        SE_Volume = mySESlider.value;
        mySESlider.onValueChanged.AddListener(value => SE_Volume = value);

    }

    /// <summary>
    /// 音量設定を保存する
    /// </summary>
    private void SaveSliderPrefs()
    {
        PlayerPrefs.SetFloat(BGM_VOLUME_KEY, myBGMSlider.value);
        PlayerPrefs.SetFloat(SE_VOLUME_KEY, mySESlider.value);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 持っているコインの表示枚数を更新する
    /// </summary>
    private void UpdateHaveMedal()
    {
        haveMedal = PlayerInformationManager.Instance.GetHaveMedal();
        if (haveMedal >= 10000000)
        {
            float num = Mathf.Floor((float)haveMedal / 100000f);
            haveMedalText.text = (num / 10f).ToString("F1") + "M";
        }
        else
        {
            haveMedalText.text = haveMedal.ToString("N0");
        }
    }

    /// <summary>
    /// 遊び方ボタン(or戻るボタン)を押したとき
    /// </summary>
    /// <param name="flg">true:表示する、false:非表示にする</param>
    public void PushHowToPlayButton(bool flg)
    {
        if (flg)
        {
            uiSeManager.PlaySE(UiSeManager.SoundSeType.SE_Yes);
            howToPlayPamel.SetActive(true);
        }
        else
        {
            uiSeManager.PlaySE(UiSeManager.SoundSeType.SE_No);
            howToPlayScrollView.content.anchoredPosition = Vector2.zero; // スクロール位置をTopに戻す
            howToPlayPamel.SetActive(false);
        }
    }
  
    /// <summary>
    /// メダルチャージボタン(or戻るボタン)を押したとき
    /// </summary>
    /// <param name="flg">true:表示する、false:非表示にする</param>
    public void PushMedalChargeButton(bool flg)
    {
        if (flg)
        {
            uiSeManager.PlaySE(UiSeManager.SoundSeType.SE_Yes);
            medalChargePamel.SetActive(true);
        }
        else
        {
            uiSeManager.PlaySE(UiSeManager.SoundSeType.SE_No);
            medalChargePamel.SetActive(false);
        }
    }

    /// <summary>
    /// メニューボタン(orゲームに戻るボタン)を押したとき
    /// </summary>
    /// <param name="flg">true:表示する、false:非表示にする</param>
    public void PushMenuButton(bool flg)
    {
        if (flg)
        {
            uiSeManager.PlaySE(UiSeManager.SoundSeType.SE_Yes);
            menuPamel.SetActive(true);
        }
        else
        {
            uiSeManager.PlaySE(UiSeManager.SoundSeType.SE_No);
            menuPamel.SetActive(false);
        }
    }

    /// <summary>
    /// 音量設定ボタン(or戻るボタン)を押したとき
    /// </summary>
    /// <param name="flg">true:表示する、false:非表示にする</param>
    public void PushSoundSettingButton(bool flg)
    {
        if (flg)
        {
            uiSeManager.PlaySE(UiSeManager.SoundSeType.SE_Yes);
            soundSettingPamel.SetActive(true);
        }
        else
        {
            uiSeManager.PlaySE(UiSeManager.SoundSeType.SE_No);
            SaveSliderPrefs();
            soundSettingPamel.SetActive(false);
        }
        isActiveSoundSettingPanel = flg;
    }

    /// <summary>
    /// 音量設定パネルが表示中かどうかを返す
    /// </summary>
    /// <returns>true:表示中、false:非表示</returns>
    public bool GetIsActiveSoundSettingPanel()
    {
        return isActiveSoundSettingPanel;
    }

    /// <summary>
    /// BGMのボリュームのゲッター
    /// </summary>
    /// <returns></returns>
    public float GetBGMVolume()
    {
        return BGM_Volume;
    }

    /// <summary>
    /// SEのボリュームのゲッター
    /// </summary>
    /// <returns></returns>
    public float GetSEVolume()
    {
        return SE_Volume;
    }

    /// <summary>
    /// メニューに戻るボタン(orいいえボタン)を押したとき
    /// </summary>
    /// <param name="flg">true:表示する、false:非表示にする</param>
    public void PushToMenuSceneButton(bool flg)
    {
        if (flg)
        {
            uiSeManager.PlaySE(UiSeManager.SoundSeType.SE_Yes);
            checkToMenuPanel.SetActive(true);
        }
        else
        {
            uiSeManager.PlaySE(UiSeManager.SoundSeType.SE_No);
            checkToMenuPanel.SetActive(false);
        }
    }

    /// <summary>
    /// メニューシーンに移動
    /// </summary>
    public void ToMenuScene()
    {
        uiSeManager.PlaySE(UiSeManager.SoundSeType.SE_OK);
        FadeManager.Instance.LoadScene("Menu", 0.3f);
    }

    /// <summary>
    /// メニューバーから広告を視聴するボタンを押したとき
    /// </summary>
    public void ShowReward()
    {
        AdMobManager.Instance.ShowReward();
    }

    /// <summary>
    /// リワード広告を見終えたとき
    /// </summary>
    /// <param name="flg">true : 広告を最後まで観た , false ; 広告を呼び出した</param>
    public void FinishReward(bool flg)
    {
        if (rewardFlg) return;

        if (!rewardFinishPanel.activeSelf)
        {
            DisplayRewardShowPanel(true);
        }

        uiSeManager.PlaySE(UiSeManager.SoundSeType.SE_OK);

        if (flg)
        {
            rewardFlg = true;

            PlayerInformationManager.Instance.AcquisitionMedal(GET_REWARD_MEDALS);

            rewardShowText1.text = "報酬を獲得しました";
            rewardShowText2.text = "所持メダル : " + PlayerInformationManager.Instance.GetHaveMedal().ToString();
        }
        else
        {
            rewardShowText1.text = "広告の視聴に失敗しました";
            rewardShowText2.text = "所持メダル : " + PlayerInformationManager.Instance.GetHaveMedal().ToString();
        }
    }

    /// <summary>
    /// リワード広告視聴後のパネルを表示・非表示にする
    /// </summary>
    /// <param name="flg"></param>
    public void DisplayRewardShowPanel(bool flg)
    {
        rewardFinishPanel.SetActive(flg);

        if (!flg)
        {
            rewardFlg = false;
            PushMedalChargeButton(false);
        }
    }


}
