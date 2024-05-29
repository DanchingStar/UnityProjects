using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitleSceneManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI versionText;

    [SerializeField] private TitleSceneInformationTextPanel informationTextPanel;

    [SerializeField] private TitleSceneMenuPanel menuPanel;

    [SerializeField] private AudioClip stageMusicClip;

    [SerializeField] private Image imageTitle;
    [SerializeField] private Image imageXylophone;
    [SerializeField] private TextMeshProUGUI tapScreenText;

    private GameObject fadeManagerObject;

    private string beforeRepairCode;

    private TitleStatus titleStatus;

    private float imageTimer;
    private float textTimer;

    private Color defaultTapStringColor;

    private const float COLOR_SPEED = 1.0f;
    private const float COLOR_RANGE = 1.2f;
    private const float COLOR_A_MIN = 0.2f;
    private const float COLOR_A_MAX = 1.0f;

    private const float SPEED_IMAGE_TITLE = 1.0f;
    private const float SPEED_IMAGE_XYLOPHONE = 0.5f;
    private const float ANGLE_IMAGE_XYLOPHONE = 60f;

    public enum TitleStatus
    {
        BeforeTap,
        Menu,
        Repair,
        Reboot,
        LogIn,
        SaveCheck,
        FailureLogIn,
        Finish,
    }

    public static TitleSceneManager Instance;
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
        imageTimer = 0f;

        beforeRepairCode = "";
        versionText.text = $"Ver.{Application.version}";

        fadeManagerObject = FadeManager.Instance.gameObject;

        defaultTapStringColor = tapScreenText.color;

        InitStageMusic();

        ChangeTitleStatus(TitleStatus.BeforeTap);
    }

    private void Update()
    {
        UpdateImages();

        if (titleStatus == TitleStatus.BeforeTap)
        {
            textTimer += Time.deltaTime;
            float value = Mathf.PingPong(textTimer * COLOR_SPEED, COLOR_RANGE - COLOR_A_MIN) + COLOR_A_MIN;
            if (value > COLOR_A_MAX) value = COLOR_A_MAX;
            tapScreenText.color = new Color(defaultTapStringColor.r, defaultTapStringColor.g, defaultTapStringColor.b, value);
        }
    }

    /// <summary>
    /// タイトルシーンのBGMを設定して流す
    /// </summary>
    private void InitStageMusic()
    {
        if (stageMusicClip == null) return;

        SoundManager.Instance.SetStageMusic(stageMusicClip);
        SoundManager.Instance.PlayStageMusic();
    }

    /// <summary>
    /// 画像を動かす
    /// </summary>
    private void UpdateImages()
    {
        imageTimer += Time.deltaTime;

        // 0 <= value <= 1
        float valueTitle = (Mathf.Sin(imageTimer * SPEED_IMAGE_TITLE) + 1) / 2;
        float valueXylophone = (Mathf.Sin(imageTimer * SPEED_IMAGE_XYLOPHONE) + 1) / 2;

        imageTitle.transform.localScale = Vector3.one * (valueTitle * 0.2f + 0.8f);
        imageXylophone.transform.rotation = Quaternion.Euler(valueXylophone * ANGLE_IMAGE_XYLOPHONE, 0, 0);

    }

    /// <summary>
    /// タイトル画面でタッチ用パネルを押したとき
    /// </summary>
    public void TouchTitle()
    {
        if (GetTitleStatus() != TitleStatus.BeforeTap) return;

        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Ok);

        ChangeTitleStatus(TitleStatus.LogIn);

        PlayFabManager.Instance.Login();

    }

    /// <summary>
    /// ログインが完了した後に実行する
    /// </summary>
    public void AfterLogIn(bool newPlayerFlg)
    {
        //Debug.Log($"TitleManager.AfterLogIn : newPlayerFlg = {newPlayerFlg}");

        // if (PlayerInformationManager.Instance.GetNewPlayerFlg())
        if (newPlayerFlg)
        {
            //ChangeTitleStatus(TitleStatus.NameEdit);

            //DisplayNewPlayerPanel(true);

            AfterLogIn(false);
        }
        else
        {
            //ChangeTitleStatus(TitleStatus.UpdateSaveFile);
            //PlayFabManager.Instance.UpdateUserData();

            ChangeTitleStatus(TitleStatus.SaveCheck);
            LoadSavedata();

            // Debug.Log($"TitleManager.TouchTitle : False");
        }
    }

    /// <summary>
    /// セーブデータをロードする
    /// </summary>
    private void LoadSavedata()
    {
        PlayerInformationManager.Instance.InitAndSettingPlayerInformation();
    }

    /// <summary>
    /// セーブデータをロードした後に呼ばれる
    /// </summary>
    public void AfterLoadSaveData(bool nullFlg)
    {
        if (nullFlg)
        {
            AfterLogIn(false);
        }
        else
        {
            ChangeTitleStatus(TitleStatus.Finish);
            MoveMenuScene();
        }
    }

    /// <summary>
    /// ログイン失敗時に呼ばれる
    /// </summary>
    public void FailureLogIn()
    {
        ChangeTitleStatus(TitleStatus.FailureLogIn);
        StartCoroutine(RestartApplication(2f));
    }

    /// <summary>
    /// 復旧ログインの後に実行する
    /// </summary>
    public void AfterRepairLogin()
    {
        CloseAllMenuPanel();

        PlayFabManager.Instance.SaveNewCustomID(beforeRepairCode);

        AfterRepair();
    }

    /// <summary>
    /// 復旧データのDLの後に実行する
    /// </summary>
    /// <param name="repeaiSaveData"></param>
    public void AfterRepair()
    {
        StartCoroutine(RestartApplication(0f));
    }

    /// <summary>
    /// 復旧失敗時
    /// </summary>
    public void RepairFailure()
    {
        menuPanel.DisplayRepairFailureText(true);

        informationTextPanel.ActiveInformationTextPanel(false);
        titleStatus = TitleStatus.Menu;
    }

    /// <summary>
    /// ユーザーデータのアップロード完了した後に実行する
    /// </summary>
    public void AfterUpdateUserData()
    {
        Debug.Log($"TitleManager.AfterUpdateUserData");
    }

    /// <summary>
    /// メニューシーンへ移る
    /// </summary>
    public void MoveMenuScene()
    {

        FadeManager.Instance.LoadScene("Menu", 0.3f);
    }

    /// <summary>
    /// タイトル画面のメニュボタン(orメニュ画面から戻るボタン)を押したとき
    /// </summary>
    /// <param name="flg">true : メニュボタン , false : メニュ画面から戻るボタン </param>
    public void PushMenuButton(bool flg)
    {
        if (flg)
        {
            //SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes);

            ChangeTitleStatus(TitleStatus.Menu);
        }
        else
        {
            //SoundManager.Instance.PlaySE(SoundManager.SoundSeType.No);

            ChangeTitleStatus(TitleStatus.BeforeTap);
        }
    }

    /// <summary>
    /// 現在のステータス（モード）を変更する
    /// </summary>
    /// <param name="ts"></param>
    public void ChangeTitleStatus(TitleStatus ts)
    {
        titleStatus = ts;

        if (ts == TitleStatus.BeforeTap)
        {
            textTimer = 0f;
            informationTextPanel.ActiveInformationTextPanel(false);
            informationTextPanel.SetInformationText("BeforeTap");
            if (menuPanel.GetActiveFlg())
            {
                menuPanel.ActiveMenuPanel(false);
            }
        }
        else if (ts == TitleStatus.Menu)
        {
            informationTextPanel.ActiveInformationTextPanel(false);
            informationTextPanel.SetInformationText("Menu");
            menuPanel.ActiveMenuPanel(true);
        }
        else if (ts == TitleStatus.Repair)
        {
            informationTextPanel.ActiveInformationTextPanel(true);
            informationTextPanel.SetInformationText("データ復旧中");
        }
        else if (ts == TitleStatus.Reboot)
        {
            informationTextPanel.ActiveInformationTextPanel(true);
            informationTextPanel.SetInformationText("アプリを\n再起動中");
        }
        else if (ts == TitleStatus.LogIn)
        {
            informationTextPanel.ActiveInformationTextPanel(true);
            informationTextPanel.SetInformationText("ログイン中");
        }
        else if (ts == TitleStatus.SaveCheck)
        {
            informationTextPanel.ActiveInformationTextPanel(true);
            informationTextPanel.SetInformationText("データ読み込み中");
        }
        else if (ts == TitleStatus.FailureLogIn)
        {
            informationTextPanel.ActiveInformationTextPanel(true);
            informationTextPanel.SetInformationText("ログインに失敗");
        }
        else if (ts == TitleStatus.Finish)
        {
            informationTextPanel.ActiveInformationTextPanel(true);
            informationTextPanel.SetInformationText("完了");
        }
    }

    /// <summary>
    /// TitleStatusの状態を返すゲッター
    /// </summary>
    /// <returns></returns>
    public TitleStatus GetTitleStatus()
    {
        return titleStatus;
    }

    /// <summary>
    /// Menuのデータ復旧を実行するボタンを押したとき
    /// </summary>
    public void PushRepairButton()
    {
        string code = menuPanel.GetRepairCode();

        if (beforeRepairCode == code)
        {
            SoundManager.Instance.PlaySE(SoundManager.SoundSeType.No);
            return;
        }

        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes);

        ChangeTitleStatus(TitleStatus.Repair);

        beforeRepairCode = code;

        PlayFabManager.Instance.RepairLogin(code);
    }

    /// <summary>
    /// 全てのMenuのパネルを閉じる
    /// </summary>
    public void CloseAllMenuPanel()
    {
        menuPanel.CloseAllPanel();
    }

    /// <summary>
    /// アプリを再起動する
    /// </summary>
    /// <param name="delayTime">再起動までの待機時間</param>
    /// <returns></returns>
    public IEnumerator RestartApplication(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        ChangeTitleStatus(TitleStatus.Reboot);

        // DontDestroyOnLoadで守られているオブジェクトを破棄する
        var objects = FindObjectsOfType<MonoBehaviour>();

        yield return new WaitForSeconds(0.5f);

        foreach (var obj in objects)
        {
            if (obj.gameObject != gameObject && obj.gameObject != fadeManagerObject)
            {
                Destroy(obj.gameObject);
            }
        }

        yield return new WaitForSeconds(0.5f);

        // 初めからスタートするシーンを読み込む
        FadeManager.Instance.LoadScene("Title", 0.5f);
    }

}
