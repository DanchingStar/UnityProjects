using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : ManagerParent
{
    #region MyDeclare

    public enum TodayDate
    {
        None,
        Dec31,
        Jan1,
        Jan2,
        Jan3,
        Jan4,
        Jan5,
        Jan6,
        Jan7,
        Jan8,
        Jan9,
        Jan10,
    }
    public enum NowTime
    {
        None,
        Morning,
        Noon,
        AfterNoon,
        Evening,
        Night,
        Midnight,
    }

    [SerializeField] TodayDate todayDate;
    [SerializeField] NowTime nowTime;

    [SerializeField] private GameObject myUIObject;
    [SerializeField] private GameObject myOptionPanel;

    [SerializeField] private Text haveMoneyText;
    [SerializeField] private Text TodayDateText;
    [SerializeField] private Text NowTimeText;

    [SerializeField] private Slider myBGMSlider;
    [SerializeField] private Slider mySESlider;
    [SerializeField] private Slider mySpeedSlider;
    [SerializeField] private GameObject BGMObject;
    [SerializeField] private GameObject SEObjectParent;

    private AudioSource myAudioBGM;
    private Transform myAudioTF;

    private const string COMMAND_MYREAD = "_myread";
    private const string COMMAND_MYWRITE = "_mywrite";
    private const string COMMAND_MYPLUS = "_myplus";

    private bool skipButtonHoldFlg = false;

    private string nextSceneName;

    private int[] myParameter = new int[Enum.GetNames(typeof(MyParameterEnum)).Length];
    private int[] myItemFlags = new int[Enum.GetNames(typeof(MyItemFlagsEnum)).Length];

    #endregion 

    #region DefaultDeclare

    private const char SEPARATE_SUBSCENE = '#';
    private const char SEPARATE_PAGE = '&';
    private const char SEPARATE_COMMAND = '!';
    private const char SEPARATE_MAIN_START = '「';
    private const char SEPARATE_MAIN_END = '」';
    private const char OUTPUT_PARAM = '"';
    private const char OUTPUT_GLOBAL_PARAM = '$';

    private const char COMMAND_SEPARATE_PARAM = '=';
    private const char COMMAND_SEPARATE_ANIM = '%';
    private const string COMMAND_BACKGROUND = "background";
    private const string COMMAND_FOREGROUND = "foreground";
    private const string COMMAND_CHARACTER_IMAGE = "charaimg";
    private const string COMMAND_BGM = "bgm";
    private const string COMMAND_SE = "se";
    private const string COMMAND_JUMP = "jump_to";
    private const string COMMAND_SELECT = "select";
    private const string COMMAND_WAIT_TIME = "wait";
    private const string COMMAND_CHANGE_SCENE = "scene";
    private const string COMMAND_BACK_LOG = "backlog";
    private const string COMMAND_PARAM = "param";
    private const string COMMAND_GLOBAL_PARAM = "globalp";
    private const string COMMAND_BRANCH = "branch";
    private const string COMMAND_TEXT = "_text";
    private const string COMMAND_SPRITE = "_sprite";
    private const string COMMAND_COLOR = "_color";
    private const string COMMAND_SIZE = "_size";
    private const string COMMAND_POSITION = "_pos";
    private const string COMMAND_ROTATION = "_rotate";
    private const string COMMAND_ACTIVE = "_active";
    private const string COMMAND_DELETE = "_delete";
    private const string COMMAND_ANIM = "_anim";
    private const string COMMAND_PLAY = "_play";
    private const string COMMAND_MUTE = "_mute";
    private const string COMMAND_SOUND = "_sound";
    private const string COMMAND_VOLUME = "_volume";
    private const string COMMAND_PRIORITY = "_priority";
    private const string COMMAND_LOOP = "_loop";
    private const string COMMAND_FADE = "_fade";
    private const string COMMAND_WRITE = "_write";
    private const string COMMAND_CALC = "_calc";

    private const string CHARACTER_IMAGE_PREFAB = "CharacterImage";
    private const string SELECT_BUTTON_PREFAB = "SelectButton";
    private const string SE_AUDIOSOURCE_PREFAB = "SEAudioSource";

    [SerializeField]
    private Image backgroundImage;
    [SerializeField]
    private Image foregroundImage;
    [SerializeField]
    private GameObject characterImages;
    [SerializeField]
    private GameObject selectButtons;
    [SerializeField]
    private Text mainText;
    [SerializeField]
    private Text nameText;
    [SerializeField]
    private ScrollRect backLog;
    [SerializeField]
    private GameObject nextPageIcon;
    [SerializeField]
    private AudioSource bgmAudioSource;
    [SerializeField]
    private GameObject seAudioSources;
    [SerializeField]
    private string spritesDirectory = "Sprites/";
    [SerializeField]
    private string prefabsDirectory = "Prefabs/";
    [SerializeField]
    private string audioClipsDirectory = "AudioClips/";
    [SerializeField]
    private string textFile = "Texts/Scenario";
    [SerializeField]
    private string animationsDirectory = "Animations/";
    [SerializeField]
    private string overrideAnimationClipName = "Clip";
    [SerializeField]
    private float captionSpeed = 0.2f;
    [SerializeField]
    private KeyCode skipKey = KeyCode.S;
    [SerializeField]
    private KeyCode alreadyReadSkipKey = KeyCode.R;
    [SerializeField]
    private float skipSpeed = 0.05f;
    [SerializeField]
    private Color FirstReadCaptionColor;
    [SerializeField]
    private Color alreadyReadCaptionColor;

    private float _waitTime = 0;
    private string _text = "";
    private bool _isSkip = false;
    private bool isStop { get { return !mainText.transform.parent.gameObject.activeSelf || backLog.gameObject.activeSelf; } }
    private Dictionary<string, List<string>> _subScenes = new Dictionary<string, List<string>>();
    private Queue<string> _pageQueue;
    private Queue<char> _charQueue;
    private List<Image> _charaImageList = new List<Image>();
    private List<Button> _selectButtonList = new List<Button>();
    private List<AudioSource> _seList = new List<AudioSource>();
    private Dictionary<string, object> _params = new Dictionary<string, object>();
    private static Dictionary<string, object> _globalparams;
    private static Dictionary<string, Dictionary<string, bool>> _alreadyReadFlags;
    private static string _readingSubSceneName;
    private DataTable _dt = new DataTable();

    #endregion

    private void Start()
    {
        myBanner.RequestBanner();

        if (string.Compare(SceneManager.GetActiveScene().name, "Game_Prologue") == 0)
        {
            MakeMyParameter();
        }
        else
        {
            LoadMyParameter();
        }

        Init();

        MyInit();

    }

    private void Update()
    {
        //PushSkipButton();
        OnSkipButtonHold();
        if (_isSkip) return;

#if UNITY_EDITOR
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
#else 
    if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) {
        return;
    }
#endif

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0)) OnClick();
#else
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0); // タッチ情報の取得

            if (touch.phase == TouchPhase.Began)
            {
                OnClick();
            }
        }
#endif 

        //if (Input.GetMouseButtonDown(1)) OnClickRight();
        //MouseScroll();

    }

    #region MyMethod

    /// <summary>
    /// 自作の初期化処理
    /// </summary>
    private void MyInit()
    {
        DisplayTexts();

        myAudioBGM = BGMObject.GetComponent<AudioSource>();
        myAudioTF = SEObjectParent.GetComponent<Transform>();

        myBGMSlider.value = PlayerPrefs.GetFloat("BGMVolume", 0.5f);
        mySESlider.value = PlayerPrefs.GetFloat("SEVolume", 0.5f);
        mySpeedSlider.value = PlayerPrefs.GetFloat("TextSpeed", 0.1f);

        myAudioBGM.volume = myBGMSlider.value;
        foreach (Transform childTF in myAudioTF)
        {
            childTF.gameObject.GetComponent<AudioSource>().volume = mySESlider.value;
        }
        captionSpeed = mySpeedSlider.value;

        myBGMSlider.onValueChanged.AddListener(value => myAudioBGM.volume = value);
        foreach (Transform childTF in myAudioTF)
        {
            mySESlider.onValueChanged.AddListener(value => childTF.gameObject.GetComponent<AudioSource>().volume = value);
        }
        mySpeedSlider.onValueChanged.AddListener(value => captionSpeed = value);

    }

    public void OnOptionButton()
    {
        myOptionPanel.SetActive(true);
        nextPageIcon.SetActive(false);
        myUIObject.SetActive(false);
    }

    public void OnOptionBackButton()
    {
        myOptionPanel.SetActive(false);
        nextPageIcon.SetActive(true);
        myUIObject.SetActive(true);
        SetSliderPrefs();
    }

    private void SetSliderPrefs()
    {
        PlayerPrefs.SetFloat("BGMVolume", myBGMSlider.value);
        PlayerPrefs.SetFloat("SEVolume", mySESlider.value);
        PlayerPrefs.SetFloat("TextSpeed", mySpeedSlider.value);
        PlayerPrefs.Save();
    }

    public void OnToTitleButton()
    {
        FadeManager.Instance.LoadScene("Menu", 0.3f);
    }

    public void OnLogButton()
    {
        //StopAllAnimation(true);
        backLog.gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(backLog.verticalScrollbar.gameObject);
        backLog.verticalNormalizedPosition = 0f;

        nextPageIcon.SetActive(false);
        myUIObject.SetActive(false);
    }

    public void OnLogBackButton()
    {
        //StopAllAnimation(false);
        backLog.gameObject.SetActive(false);

        nextPageIcon.SetActive(true);
        myUIObject.SetActive(true);
    }

    public void OnSkipButtonDown()
    {
        skipButtonHoldFlg = true;
    }

    public void OnSkipButtonUp()
    {
        skipButtonHoldFlg = false;
        _isSkip = false;
    }

    public void OnSkipButtonHold()
    {
        if (!_isSkip && skipButtonHoldFlg)
        {
            _isSkip = true;
            StartCoroutine(Skip(skipSpeed));
        }
    }

    /// <summary>
    /// エンディングの種類を決める
    /// </summary>
    private void DecideEnding()
    {
        if (myParameter[(int)MyParameterEnum.FlgHorseClear] == 1) //競馬が当たっていた場合
        {
            nextSceneName = "Game_Ending_Another";
        }
        else if (myParameter[(int)MyParameterEnum.HaveMoney] >= 30000) //所持金が3万円以上の場合
        {
            nextSceneName = "Game_Ending_Normal";
        }
        else if (myParameter[(int)MyParameterEnum.MotherMoney] >= 80000) //預けたお金が8万円以上の場合
        {
            nextSceneName = "Game_Ending_Good";
        }
        else //その他
        {
            nextSceneName = "Game_Ending_Bad";
        }
    }


    /// <summary>
    /// 特別エンディングの種類を決める
    /// </summary>
    private void DecideSPEnding()
    {
        int itemCount = 0;
        foreach (var i in Enum.GetValues(typeof(MyItemFlagsEnum)))
        {
            if (myItemFlags[(int)i] == 1)
            {
                itemCount++; //アイテム数を数える
            }
        }

        if (itemCount >= 6) //アイテムが6個以上の場合
        {
            nextSceneName = "Game_Ending_SP1";
        }
        else if (myParameter[(int)MyParameterEnum.HeartfulPoint] >= 6) //ハートフルポイントが6以上の場合
        {
            nextSceneName = "Game_Ending_SP2";
        }
        else //その他
        {
            nextSceneName = "Result";
        }
    }

    /// <summary>
    /// シーン移動する前に行う処理
    /// </summary>
    private void ChangeNextSceneBefore()
    {
        if (string.Compare(nextSceneName, "Ending") == 0)
        {
            DecideEnding();
        }
        else if (string.Compare(nextSceneName, "SPEnding") == 0)
        {
            DecideSPEnding();
        }
        myInterstitial.RequestInterstitial(PlayerPrefs.GetInt("InterstitialCounter", 0));
        SaveMyParameter();
    }

    /// <summary>
    /// テキストファイルからパラメータを読み込む関数
    /// </summary>
    /// <param name="parameter">変数名（Enumの要素名）</param>
    /// <returns>読み込む値（Int or Bool）</returns>
    private object MyParameterRead(string parameter)
    {
        parameter = GetParameterForCommand(parameter);

        foreach (var i in Enum.GetValues(typeof(MyParameterEnum)))
        {
            if (string.Compare(parameter, i.ToString()) == 0)
            {
                return myParameter[(int)i];
            }
        }
        foreach (var i in Enum.GetValues(typeof(MyItemFlagsEnum)))
        {
            if (string.Compare(parameter, i.ToString()) == 0)
            {
                return myItemFlags[(int)i];
            }
        }
        if (string.Compare(parameter, "Enter") == 0) //改行コードの設定
        {
            return "\n";
        }

        Debug.LogError("MyParameterRead"); //変数名が該当なし

        return -1;
    }

    /// <summary>
    /// テキストファイルからパラメータを書き換える関数
    /// </summary>
    /// <param name="parameter">変数名（Enumの要素名）</param>
    /// <param name="num">書き込む値（Int or Bool）</param>
    private void MyParameterWrite(string parameter,object num)
    {
        parameter = GetParameterForCommand(parameter);

        foreach (var i in Enum.GetValues(typeof(MyParameterEnum)))
        {
            if (string.Compare(parameter, i.ToString()) == 0)
            {
                myParameter[(int)i] = Convert.ToInt32(num);
                return;
            }
        }
        foreach (var i in Enum.GetValues(typeof(MyItemFlagsEnum)))
        {
            if (string.Compare(parameter, i.ToString()) == 0)
            {
                myItemFlags[(int)i] = Convert.ToInt32(num);
                return;
            }
        }

        Debug.LogError($"MyParameterWrite Error! parameter : {parameter}"); //変数名が該当なし

        return;
    }

    /// <summary>
    /// 確率でTrueになるか抽選する関数
    /// </summary>
    /// <param name="probability">確率（％）</param>
    /// <returns>True:1 , False:0</returns>
    private int LotteryParameter(int probability)
    {
        int rand = Random.Range(0, 100);

        if (rand < probability)
            return 1;
        else
            return 0;
    }

    /// <summary>
    /// ゲーム「はじめから」のときに、パラメータを抽選して作る関数
    /// </summary>
    private void MakeMyParameter()
    {
        PlayerPrefs.DeleteKey("NextSceneName"); //中断データを消す

        foreach (var i in Enum.GetValues(typeof(MyItemFlagsEnum)))
        {
            myItemFlags[(int)i] = 0; //全てのアイテムのフラグを初期化
        }

        foreach (var i in Enum.GetValues(typeof(MyParameterEnum)))
        {
            myParameter[(int)i] = 0; //全てのパラメータを初期化
        }

        myParameter[(int)MyParameterEnum.HaveMoney] = 0; //所持金
        myParameter[(int)MyParameterEnum.MotherMoney] = 0; //母に預けたお金
        myParameter[(int)MyParameterEnum.HeartfulPoint] = 0; //ハートフルポイント

        myParameter[((int)MyParameterEnum.IsUncle)] = LotteryParameter(60); //伯父の出現確率
        myParameter[((int)MyParameterEnum.IsAunt)] = LotteryParameter(60); //叔母の出現確率
        myParameter[((int)MyParameterEnum.IsBrother)] = LotteryParameter(60); //兄の出現確率
        myParameter[((int)MyParameterEnum.IsSister)] = LotteryParameter(60); //姉の出現確率

        if (myParameter[((int)MyParameterEnum.IsUncle)  ] == 1 || myParameter[((int)MyParameterEnum.IsAunt)  ] == 1 ||
            myParameter[((int)MyParameterEnum.IsBrother)] == 1 || myParameter[((int)MyParameterEnum.IsSister)] == 1)
        {
            myParameter[((int)MyParameterEnum.IsCat)] = 0; //三毛猫が出現しない
        }
        else
        {
            myParameter[((int)MyParameterEnum.IsCat)] = 1; //三毛猫が出現する
        }

        myParameter[(int)MyParameterEnum.HatsuYume] = Random.Range(0, 5); //初夢の抽選

        if (myParameter[((int)MyParameterEnum.IsUncle)]   == 1 && myParameter[((int)MyParameterEnum.IsAunt)]   == 1 &&
            myParameter[((int)MyParameterEnum.IsBrother)] == 1 && myParameter[((int)MyParameterEnum.IsSister)] == 1)
        {
            myParameter[(int)MyParameterEnum.Asobi] = 0; //すごろく
        }
        else if (myParameter[((int)MyParameterEnum.IsBrother)] == 1 && myParameter[((int)MyParameterEnum.IsSister)] == 1)
        {
            myParameter[(int)MyParameterEnum.Asobi] = 1; //羽根つき
        }
        else if (myParameter[((int)MyParameterEnum.IsUncle)] == 1 && myParameter[((int)MyParameterEnum.IsAunt)] == 1)
        {
            myParameter[(int)MyParameterEnum.Asobi] = 2; //福笑い
        }
        else if (myParameter[((int)MyParameterEnum.IsBrother)] == 1 && myParameter[((int)MyParameterEnum.IsUncle)] == 1)
        {
            myParameter[(int)MyParameterEnum.Asobi] = 3; //凧あげ
        }
        else if (myParameter[((int)MyParameterEnum.IsAunt)] == 1 && myParameter[((int)MyParameterEnum.IsSister)] == 1)
        {
            myParameter[(int)MyParameterEnum.Asobi] = 4; //百人一首
        }
        else
        {
            myParameter[(int)MyParameterEnum.Asobi] = 5; //駅伝観戦
        }

        myParameter[((int)MyParameterEnum.IsHorseLuck)] = LotteryParameter(20); //競馬の判定
        myParameter[((int)MyParameterEnum.IsGrandPaLuck)] = LotteryParameter(20); //祖父の倍チャレの判定
        myParameter[((int)MyParameterEnum.IsGrandMaLuck)] = LotteryParameter(20); //祖母の倍チャレの判定

    }

    /// <summary>
    /// パラメータをロードする関数
    /// </summary>
    private void LoadMyParameter()
    {
        foreach (var i in Enum.GetValues(typeof(MyParameterEnum)))
        {
            myParameter[(int)i] = PlayerPrefs.GetInt(i.ToString(), -1);
        }

        foreach (var i in Enum.GetValues(typeof(MyItemFlagsEnum)))
        {
            myItemFlags[(int)i] = PlayerPrefs.GetInt(i.ToString(), -1);
        }
    }

    /// <summary>
    /// パラメータをセーブする関数
    /// </summary>
    private void SaveMyParameter()
    {
        foreach (var i in Enum.GetValues(typeof(MyParameterEnum)))
        {
            PlayerPrefs.SetInt(i.ToString(), myParameter[(int)i]);
        }
        foreach (var i in Enum.GetValues(typeof(MyItemFlagsEnum)))
        {
            PlayerPrefs.SetInt(i.ToString(), myItemFlags[(int)i]);
        }
        PlayerPrefs.SetString("NextSceneName", nextSceneName);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// スコアをアニメーションさせる
    /// </summary>
    /// <param name="addScore">加算するスコア</param>
    /// <param name="time">加算する時間</param>
    /// <returns></returns>
    IEnumerator ScoreAnimation(string parameter, float time)
    {
        parameter = GetParameterForCommand(parameter);

        int addScore = Convert.ToInt32(parameter);

        //前回のスコア
        int befor = myParameter[(int)MyParameterEnum.HaveMoney];
        //今回のスコア
        int after = myParameter[(int)MyParameterEnum.HaveMoney] + addScore;
        //得点加算
        myParameter[(int)MyParameterEnum.HaveMoney] += addScore;
        //0fを経過時間にする
        float elapsedTime = 0.0f;

        //timeが０になるまでループさせる
        while (elapsedTime < time)
        {
            float rate = elapsedTime / time;
            // テキストの更新
            haveMoneyText.text = (befor + (after - befor) * rate).ToString("N0");

            elapsedTime += Time.deltaTime;
            // 0.01秒待つ
            yield return new WaitForSeconds(0.01f);
        }
        // 最終的な着地のスコア
        haveMoneyText.text = after.ToString("N0");
    }

    /// <summary>
    /// Textの内容を初期化して表示させる
    /// </summary>
    private void DisplayTexts()
    {
        haveMoneyText.text = myParameter[(int)MyParameterEnum.HaveMoney].ToString("N0");

        string str = "";

        switch (todayDate)
        {
            case TodayDate.Dec31:
                str = "12月31日";
                break;
            case TodayDate.Jan1:
                str = "1月1日";
                break;
            case TodayDate.Jan2:
                str = "1月2日";
                break;
            case TodayDate.Jan3:
                str = "1月3日";
                break;
            case TodayDate.Jan4:
                str = "1月4日";
                break;
            case TodayDate.Jan5:
                str = "1月5日";
                break;
            case TodayDate.Jan6:
                str = "1月6日";
                break;
            case TodayDate.Jan7:
                str = "1月7日";
                break;
            case TodayDate.Jan8:
                str = "1月8日";
                break;
            case TodayDate.Jan9:
                str = "1月9日";
                break;
            case TodayDate.Jan10:
                str = "1月10日";
                break;
            default:
                str = "None";
                Debug.LogError("TodayDate None");
                break;
        }
        TodayDateText.text = str;

        switch (nowTime)
        {
            case NowTime.Morning:
                str = "朝";
                break;
            case NowTime.Noon:
                str = "昼";
                break;
            case NowTime.AfterNoon:
                str = "昼すぎ";
                break;
            case NowTime.Evening:
                str = "夕方";
                break;
            case NowTime.Night:
                str = "夜";
                break;
            case NowTime.Midnight:
                str = "深夜";
                break;
            default:
                str = "None";
                Debug.LogError("NowTime None");
                break;
        }
        NowTimeText.text = str;
    }

#endregion

#region DefaultMethod

    /**
     * 初期化する
     */
    private void Init()
    {
        if (_globalparams == null)
            _globalparams = new Dictionary<string, object>();
        if (_alreadyReadFlags == null)
            _alreadyReadFlags = new Dictionary<string, Dictionary<string, bool>>();
        Dictionary<string, bool> alreadyReadFlag = new Dictionary<string, bool>();
        _text = LoadTextFile(textFile);
        Queue<string> subScenes = SeparateString(_text, SEPARATE_SUBSCENE);
        foreach (string subScene in subScenes)
        {
            if (subScene.Equals("")) continue;
            Queue<string> pages = SeparateString(subScene, SEPARATE_PAGE);
            string subSceneName = pages.Dequeue();
            _subScenes[subSceneName] = pages.ToList<string>();
            alreadyReadFlag[subSceneName] = false;
        }
        string sceneName = SceneManager.GetActiveScene().name;
        if (!_alreadyReadFlags.ContainsKey(sceneName))
            _alreadyReadFlags[sceneName] = alreadyReadFlag;
        _readingSubSceneName = _subScenes.First().Key;
        _pageQueue = ListToQueue(_subScenes.First().Value);
        ChangeMainTextColor();
        ShowNextPage();
    }

    /**
     * 終了時の処理
     */
    private void Exit()
    {
        CheckAlreadyRead("");
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }

    /**
     * 文字列のリストをキューに変更する
     */
    private Queue<string> ListToQueue(List<string> strs)
    {
        Queue<string> queue = new Queue<string>();
        foreach (string l in strs) queue.Enqueue(l);
        return queue;
    }

    /**
     * クリックしたときの処理
     */
    private void OnClick()
    {
        if (isStop)
        {
            OnClickRight();
        }
        else
        {
            if (_charQueue.Count > 0)
                OutputAllChar();
            else
            {
                if (_selectButtonList.Count > 0)
                    return;
                if (!ShowNextPage())
                {
                    Exit();
                }
            }
        }
    }

    /**
     * 右クリックした時の処理
     */
    public void OnClickRight()
    {
        myUIObject.SetActive(!myUIObject.activeSelf);
        selectButtons.SetActive (!selectButtons.activeSelf);

        GameObject mainWindow = mainText.transform.parent.gameObject;
        GameObject nameWindow = nameText.transform.parent.gameObject;
        mainWindow.SetActive(!mainWindow.activeSelf);
        if (nameText.text.Length > 0)
            nameWindow.SetActive(mainWindow.activeSelf);
        if (_charQueue.Count <= 0)
            nextPageIcon.SetActive(mainWindow.activeSelf);
    }

    /**
     * スクロールした時の処理
     */
    private void MouseScroll()
    {
        if (backLog.gameObject.activeSelf && Input.mouseScrollDelta.y > 0 /* && backLog.verticalNormalizedPosition <= 0 */)
        {
            StopAllAnimation(false);
            backLog.gameObject.SetActive(false);
        }
        if (!backLog.gameObject.activeSelf && Input.mouseScrollDelta.y < 0)
        {
            StopAllAnimation(true);
            backLog.verticalNormalizedPosition = 0;
            backLog.gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(backLog.verticalScrollbar.gameObject);
        }
    }

    /**
     * スキップ用のキー(ボタン)を押した時の処理
     */
    private void PushSkipButton()
    {


        if (!_isSkip && Input.GetKey(skipKey))
        {
            _isSkip = true;
            StartCoroutine(Skip(skipSpeed));
        }
        if (!_isSkip && Input.GetKey(alreadyReadSkipKey))
        {
            if (IsAlreadyRead(_readingSubSceneName))
            {
                _isSkip = true;
                StartCoroutine(Skip(skipSpeed));
            }
        }
        if (Input.GetKeyUp(skipKey) || Input.GetKeyUp(alreadyReadSkipKey))
            _isSkip = false;
    }

    /**
     * スキップの処理
     */
    private IEnumerator Skip(float wait)
    {
        while (true)
        {
            if (isStop || !_isSkip || _waitTime > 0 || _selectButtonList.Count > 0)
                break;
            OutputAllChar();
            yield return new WaitForSeconds(wait);
            if (!ShowNextPage())
            {
                Exit();
            }
        }
        _isSkip = false;
        yield break;
    }

    /**
     * 文字送りするコルーチン
     */
    private IEnumerator ShowChars(float wait)
    {
        while (true)
        {
            if (!isStop)
            {
                if (!OutputChar()) break;
            }
            yield return new WaitForSeconds(wait);
        }
        yield break;
    }

    /**
     * 次の読み込みを待機するコルーチン
     */
    private IEnumerator WaitForCommand()
    {
        float time = 0;
        while (time < _waitTime)
        {
            if (!isStop) time += Time.deltaTime;
            yield return null;
        }
        _waitTime = 0;
        ShowNextPage();
        yield break;
    }

    /**
     * 音のフェードを行うコルーチン
     */
    private IEnumerator FadeSound(AudioSource audio, float time, float volume)
    {
        float vo = (volume - audio.volume) / (time / Time.deltaTime);
        bool isOut = audio.volume > volume;
        while ((!isOut && audio.volume < volume) || (isOut && audio.volume > volume))
        {
            audio.volume += vo;
            yield return null;
        }
        audio.volume = volume;
    }

    /**
     * 1文字を出力する
     */
    private bool OutputChar()
    {
        if (_charQueue.Count <= 0)
        {
            nextPageIcon.SetActive(true);
            return false;
        }
        mainText.text += _charQueue.Dequeue();
        return true;
    }

    /**
     * 全文を表示する
     */
    private void OutputAllChar()
    {
        StopCoroutine(ShowChars(captionSpeed));
        while (OutputChar()) ;
        _waitTime = 0;
        nextPageIcon.SetActive(true);
    }

    /**
     * 既読かどうかを返す
     */
    private bool IsAlreadyRead(string subSceneName)
    {
        return _alreadyReadFlags[SceneManager.GetActiveScene().name][subSceneName];
    }

    /**
     * 既読の時は色を変える
     */
    private void ChangeMainTextColor()
    {
        if (IsAlreadyRead(_readingSubSceneName))
            mainText.color = alreadyReadCaptionColor;
        else
            mainText.color = FirstReadCaptionColor;
    }

    /**
     * 既読チェックと次に読み込むラベル名の保存
     */
    private void CheckAlreadyRead(string nextSubSceneName)
    {
        _alreadyReadFlags[SceneManager.GetActiveScene().name][_readingSubSceneName] = true;
        _readingSubSceneName = nextSubSceneName;
    }

    /**
     * 次のページを表示する
     */
    private bool ShowNextPage()
    {
        if (_pageQueue.Count <= 0) return false;
        nextPageIcon.SetActive(false);
        ReadLine(_pageQueue.Dequeue());
        return true;
    }

    /**
     * 文字列を指定した区切り文字ごとに区切り、キューに格納したものを返す
     */
    private Queue<string> SeparateString(string str, char sep)
    {
        string[] strs = str.Split(sep);
        Queue<string> queue = new Queue<string>();
        foreach (string l in strs) queue.Enqueue(l);
        return queue;
    }

    /**
     * 文を1文字ごとに区切り、キューに格納したものを返す
     */
    private Queue<char> SeparateString(string str)
    {
        char[] chars = str.ToCharArray();
        Queue<char> charQueue = new Queue<char>();
        foreach (char c in chars) charQueue.Enqueue(c);
        return charQueue;
    }

    /**
     * 1行を読み出す
     */
    private void ReadLine(string text)
    {
        if (text[0].Equals(SEPARATE_COMMAND))
        {
            ReadCommand(text);
            if (_selectButtonList.Count > 0) return;
            if (_waitTime > 0)
            {
                StartCoroutine(WaitForCommand());
                return;
            }
            ShowNextPage();
            return;
        }
        text = ReplaceParameterForGame(text);
        string[] ts = text.Split(SEPARATE_MAIN_START);
        string name = ts[0];
        string main = ts[1].Remove(ts[1].LastIndexOf(SEPARATE_MAIN_END));

        nameText.text = name;
        if (name.Equals(""))
        {
            nameText.transform.parent.gameObject.SetActive(false);
            backLog.content.GetComponentInChildren<Text>().text += main;
        }
        else
        {
            nameText.transform.parent.gameObject.SetActive(true);
            backLog.content.GetComponentInChildren<Text>().text += text;
        }
        mainText.text = "";
        _charQueue = SeparateString(main);
        StartCoroutine(ShowChars(captionSpeed));
        backLog.content.GetComponentInChildren<Text>().text +=
            Environment.NewLine + Environment.NewLine;
    }

    /**
     * ゲーム中パラメーターの置き換え
     */
    private string ReplaceParameterForGame(string line)
    {
        string[] lines = line.Split(OUTPUT_PARAM);
        for (int i = 1; i < lines.Length; i += 2)
        {
            if (lines[i][0] == OUTPUT_GLOBAL_PARAM)
                lines[i] = _globalparams[lines[i].Remove(0, 1)].ToString();
            else
                lines[i] = _params[lines[i]].ToString();
        }
        return String.Join("", lines);
    }

    /**
     * コマンドの読み出し
     */
    private void ReadCommand(string cmdLine)
    {
        cmdLine = cmdLine.Remove(0, 1);
        Queue<string> cmdQueue = SeparateString(cmdLine, SEPARATE_COMMAND);
        foreach (string cmd in cmdQueue)
        {
            string[] cmds = cmd.Split((COMMAND_SEPARATE_PARAM + "").ToCharArray(), count: 3);
            if (cmds[0].Contains(COMMAND_BACKGROUND))
                SetBackgroundImage(cmds[0], cmds[1]);
            if (cmds[0].Contains(COMMAND_FOREGROUND))
                SetForegroundImage(cmds[0], cmds[1]);
            if (cmds[0].Contains(COMMAND_CHARACTER_IMAGE))
                SetCharacterImage(cmds[1], cmds[0], cmds[2]);
            if (cmds[0].Contains(COMMAND_JUMP))
                JumpTo(cmds[1]);
            if (cmds[0].Contains(COMMAND_SELECT))
                SetSelectButton(cmds[1], cmds[0], cmds[2]);
            if (cmds[0].Contains(COMMAND_WAIT_TIME))
                SetWaitTime(cmds[1]);
            if (cmds[0].Contains(COMMAND_BGM))
                SetBackgroundMusic(cmds[0], cmds[1]);
            if (cmds[0].Contains(COMMAND_SE))
                SetSoundEffect(cmds[1], cmds[0], cmds[2]);
            if (cmds[0].Contains(COMMAND_CHANGE_SCENE))
                ChangeNextScene(cmds[1]);
            if (cmds[0].Contains(COMMAND_BACK_LOG))
                WriteBackLog(cmds[1]);
            if (cmds[0].Contains(COMMAND_PARAM))
                SetParameterForGame(cmds[1], cmds[0].Replace(COMMAND_PARAM, ""), cmds[2], _params);
            if (cmds[0].Contains(COMMAND_GLOBAL_PARAM))
                SetParameterForGame(cmds[1], cmds[0].Replace(COMMAND_GLOBAL_PARAM, ""), cmds[2], _globalparams);
            if (cmds[0].Contains(COMMAND_BRANCH))
                CompareJumpTo(cmds[1], cmds[2]);
        }
    }

    /**
     * 対応するシーンに切り替える
     */
    private void ChangeNextScene(string parameter)
    {
        parameter = GetParameterForCommand(parameter);

        /***************************************** 自作 *****************************************/

        nextSceneName = parameter;
        ChangeNextSceneBefore();
        FadeManager.Instance.LoadScene(nextSceneName, 0.3f);

        /****************************************************************************************/

        //SceneManager.LoadSceneAsync(parameter);
    }

    /**
     * 対応するラベルまでジャンプする
     */
    private void JumpTo(string parameter)
    {
        parameter = GetParameterForCommand(parameter);
        CheckAlreadyRead(parameter);
        ChangeMainTextColor();
        _pageQueue = ListToQueue(_subScenes[parameter]);
    }

    /**
     * 比較式がtrueだったら対応するラベルまでジャンプする
     */
    private void CompareJumpTo(string method, string parameter)
    {
        if ((bool)CalcParameterForGame(parameter)) JumpTo(method); //自作で変更(methodとparameterの位置)
    }

    /**
     * 待機時間を設定する
     */
    private void SetWaitTime(string parameter)
    {
        parameter = GetParameterForCommand(parameter);
        _waitTime = float.Parse(parameter);
    }

    /**
     * バックログに記述する
     */
    private void WriteBackLog(string parameter)
    {
        parameter = GetParameterForCommand(parameter);
        backLog.content.GetComponentInChildren<Text>().text += parameter + Environment.NewLine + Environment.NewLine;
    }

    /**
     * 背景の設定
     */
    private void SetBackgroundImage(string cmd, string parameter)
    {
        cmd = cmd.Replace(COMMAND_BACKGROUND, "");
        SetImage(cmd, parameter, backgroundImage);
    }

    /**
     * 前景の設定
     */
    private void SetForegroundImage(string cmd, string parameter)
    {
        cmd = cmd.Replace(COMMAND_FOREGROUND, "");
        SetImage(cmd, parameter, foregroundImage);
    }

    /**
     * 立ち絵の設定
     */
    private void SetCharacterImage(string name, string cmd, string parameter)
    {
        cmd = cmd.Replace(COMMAND_CHARACTER_IMAGE, "");
        name = GetParameterForCommand(name);
        Image image = _charaImageList.Find(n => n.name == name);
        if (image == null)
        {
            image = Instantiate(Resources.Load<Image>(prefabsDirectory + CHARACTER_IMAGE_PREFAB), characterImages.transform);
            image.name = name;
            _charaImageList.Add(image);
        }
        SetImage(cmd, parameter, image);
    }

    /**
     * 選択肢の設定
     */
    private void SetSelectButton(string name, string cmd, string parameter)
    {
        cmd = cmd.Replace(COMMAND_SELECT, "");
        name = GetParameterForCommand(name);
        Button button = _selectButtonList.Find(n => n.name == name);
        if (button == null)
        {
            button = Instantiate(Resources.Load<Button>(prefabsDirectory + SELECT_BUTTON_PREFAB), selectButtons.transform);
            button.name = name;
            button.onClick.AddListener(() => SelectButtonOnClick(name));
            _selectButtonList.Add(button);
        }
        SetImage(cmd, parameter, button.image);
    }

    /**
     * 選択肢がクリックされた
     */
    private void SelectButtonOnClick(string label)
    {
        foreach (Button button in _selectButtonList) Destroy(button.gameObject);
        _selectButtonList.Clear();
        JumpTo('"' + label + '"');
        ShowNextPage();
    }

    /**
     * 画像の設定
     */
    private void SetImage(string cmd, string parameter, Image image)
    {
        cmd = cmd.Replace(" ", "");
        parameter = GetParameterForCommand(parameter);
        switch (cmd)
        {
            case COMMAND_TEXT:
                image.GetComponentInChildren<Text>().text = parameter;
                break;
            case COMMAND_SPRITE:
                image.sprite = LoadSprite(parameter);
                break;
            case COMMAND_COLOR:
                image.color = ParameterToColor(parameter);
                break;
            case COMMAND_SIZE:
                image.GetComponent<RectTransform>().sizeDelta = ParameterToVector3(parameter);
                break;
            case COMMAND_POSITION:
                image.GetComponent<RectTransform>().anchoredPosition = ParameterToVector3(parameter);
                break;
            case COMMAND_ROTATION:
                image.GetComponent<RectTransform>().eulerAngles = ParameterToVector3(parameter);
                break;
            case COMMAND_ACTIVE:
                image.gameObject.SetActive(ParameterToBool(parameter));
                break;
            case COMMAND_DELETE:
                _charaImageList.Remove(image);
                Destroy(image.gameObject);
                break;
            case COMMAND_ANIM:
                ImageSetAnimation(image, parameter);
                break;
        }
    }

    /**
     * スプライトをファイルから読み出し、インスタンス化する
     */
    private Sprite LoadSprite(string name)
    {
        return Instantiate(Resources.Load<Sprite>(spritesDirectory + name));
    }

    /**
     * パラメーターから色を作成する
     */
    private Color ParameterToColor(string parameter)
    {
        string[] ps = parameter.Replace(" ", "").Split(',');
        if (ps.Length > 3)
            return new Color32(byte.Parse(ps[0]), byte.Parse(ps[1]),
                                            byte.Parse(ps[2]), byte.Parse(ps[3]));
        else
            return new Color32(byte.Parse(ps[0]), byte.Parse(ps[1]),
                                            byte.Parse(ps[2]), 255);
    }

    /**
     * パラメーターからベクトルを取得する
     */
    private Vector3 ParameterToVector3(string parameter)
    {
        string[] ps = parameter.Replace(" ", "").Split(',');
        return new Vector3(float.Parse(ps[0]), float.Parse(ps[1]), float.Parse(ps[2]));
    }

    /**
     * パラメーターからboolを取得する
     */
    private bool ParameterToBool(string parameter)
    {
        string p = parameter.Replace(" ", "");
        return p.Equals("true") || p.Equals("TRUE");
    }

    /**
     * アニメーションを画像に設定する
     */
    private void ImageSetAnimation(Image image, string parameter)
    {
        Animator animator = image.GetComponent<Animator>();
        AnimationClip clip = ParameterToAnimationClip(image, parameter.Split(COMMAND_SEPARATE_ANIM));
        AnimatorOverrideController overrideController;
        if (animator.runtimeAnimatorController is AnimatorOverrideController)
            overrideController = (AnimatorOverrideController)animator.runtimeAnimatorController;
        else
        {
            overrideController = new AnimatorOverrideController();
            overrideController.runtimeAnimatorController = animator.runtimeAnimatorController;
            animator.runtimeAnimatorController = overrideController;
        }
        overrideController[overrideAnimationClipName] = clip;
        animator.Update(0.0f);
        animator.Play(overrideAnimationClipName, 0);
    }

    /**
     * パラメーターからアニメーションクリップを生成する
     */
    private AnimationClip ParameterToAnimationClip(Image image, string[] parameters)
    {
        string[] ps = parameters[0].Replace(" ", "").Split(',');
        string path = animationsDirectory + SceneManager.GetActiveScene().name + "/" + image.name;
        AnimationClip prevAnimation = Resources.Load<AnimationClip>(path + "/" + ps[0]);
        AnimationClip animation = null;
#if UNITY_EDITOR
        if (ps[3].Equals("Replay") && prevAnimation != null)
            return Instantiate(prevAnimation);
        animation = new AnimationClip();
        Color startcolor = image.color;
        Vector3[] start = new Vector3[3];
        start[0] = image.GetComponent<RectTransform>().sizeDelta;
        start[1] = image.GetComponent<RectTransform>().anchoredPosition;
        Color endcolor = startcolor;
        if (parameters[1] != "") endcolor = ParameterToColor(parameters[1]);
        Vector3[] end = new Vector3[3];
        for (int i = 0; i < 2; i++)
        {
            if (parameters[i + 2] != "")
                end[i] = ParameterToVector3(parameters[i + 2]);
            else end[i] = start[i];
        }
        AnimationCurve[,] curves = new AnimationCurve[4, 4];
        if (ps[3].Equals("EaseInOut"))
        {
            curves[0, 0] = AnimationCurve.EaseInOut(float.Parse(ps[1]), startcolor.r, float.Parse(ps[2]), endcolor.r);
            curves[0, 1] = AnimationCurve.EaseInOut(float.Parse(ps[1]), startcolor.g, float.Parse(ps[2]), endcolor.g);
            curves[0, 2] = AnimationCurve.EaseInOut(float.Parse(ps[1]), startcolor.b, float.Parse(ps[2]), endcolor.b);
            curves[0, 3] = AnimationCurve.EaseInOut(float.Parse(ps[1]), startcolor.a, float.Parse(ps[2]), endcolor.a);
            for (int i = 0; i < 2; i++)
            {
                curves[i + 1, 0] = AnimationCurve.EaseInOut(float.Parse(ps[1]), start[i].x, float.Parse(ps[2]), end[i].x);
                curves[i + 1, 1] = AnimationCurve.EaseInOut(float.Parse(ps[1]), start[i].y, float.Parse(ps[2]), end[i].y);
                curves[i + 1, 2] = AnimationCurve.EaseInOut(float.Parse(ps[1]), start[i].z, float.Parse(ps[2]), end[i].z);
            }
        }
        else
        {
            curves[0, 0] = AnimationCurve.Linear(float.Parse(ps[1]), startcolor.r, float.Parse(ps[2]), endcolor.r);
            curves[0, 1] = AnimationCurve.Linear(float.Parse(ps[1]), startcolor.g, float.Parse(ps[2]), endcolor.g);
            curves[0, 2] = AnimationCurve.Linear(float.Parse(ps[1]), startcolor.b, float.Parse(ps[2]), endcolor.b);
            curves[0, 3] = AnimationCurve.Linear(float.Parse(ps[1]), startcolor.a, float.Parse(ps[2]), endcolor.a);
            for (int i = 0; i < 2; i++)
            {
                curves[i + 1, 0] = AnimationCurve.Linear(float.Parse(ps[1]), start[i].x, float.Parse(ps[2]), end[i].x);
                curves[i + 1, 1] = AnimationCurve.Linear(float.Parse(ps[1]), start[i].y, float.Parse(ps[2]), end[i].y);
                curves[i + 1, 2] = AnimationCurve.Linear(float.Parse(ps[1]), start[i].z, float.Parse(ps[2]), end[i].z);
            }
        }
        string[] b1 = { "r", "g", "b", "a" };
        for (int i = 0; i < 4; i++)
        {
            AnimationUtility.SetEditorCurve(
                animation,
                EditorCurveBinding.FloatCurve("", typeof(Image), "m_Color." + b1[i]),
                curves[0, i]
            );
        }
        string[] a = { "m_SizeDelta", "m_AnchoredPosition" };
        string[] b2 = { "x", "y", "z" };
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                AnimationUtility.SetEditorCurve(
                    animation,
                    EditorCurveBinding.FloatCurve("", typeof(RectTransform), a[i] + "." + b2[j]),
                    curves[i + 1, j]
                );
            }
        }
        if (!Directory.Exists("Assets/Resources/" + path))
            Directory.CreateDirectory("Assets/Resources/" + path);
        AssetDatabase.CreateAsset(animation, "Assets/Resources/" + path + "/" + ps[0] + ".anim");
        AssetDatabase.ImportAsset("Assets/Resources/" + path + "/" + ps[0] + ".anim");
//#elif UNITY_STANDALONE
//           animation = prevAnimation;
#else //実機だとアニメーションが再生されず、つくってみた
        animation = prevAnimation;
#endif
        return Instantiate(animation);
    }

    /**
     * 全てのアニメーションを一時停止・再開する
     */
    private void StopAllAnimation(bool isStop)
    {
        StopAnimation(isStop, foregroundImage);
        foreach (Image image in _charaImageList)
            StopAnimation(isStop, image);
        foreach (Button button in _selectButtonList)
            StopAnimation(isStop, button.image);
    }

    /**
     * アニメーションを一時停止・再開する
     */
    private void StopAnimation(bool isStop, Image image)
    {
        Animator animator = image.GetComponent<Animator>();
        if (isStop) animator.speed = 0;
        else animator.speed = 1;
    }

    /**
     * BGMの設定
     */
    private void SetBackgroundMusic(string cmd, string parameter)
    {
        cmd = cmd.Replace(COMMAND_BGM, "");
        SetAudioSource(cmd, parameter, bgmAudioSource);
    }

    /**
     * 効果音の設定
     */
    private void SetSoundEffect(string name, string cmd, string parameter)
    {
        cmd = cmd.Replace(COMMAND_SE, "");
        name = GetParameterForCommand(name);
        AudioSource audio = _seList.Find(n => n.name == name);
        if (audio == null)
        {
            audio = Instantiate(Resources.Load<AudioSource>(prefabsDirectory + SE_AUDIOSOURCE_PREFAB), seAudioSources.transform);
            audio.name = name;
            _seList.Add(audio);
        }
        SetAudioSource(cmd, parameter, audio);
    }

    /**
     * 音声の設定
     */
    private void SetAudioSource(string cmd, string parameter, AudioSource audio)
    {
        cmd = cmd.Replace(" ", "");
        parameter = GetParameterForCommand(parameter);
        switch (cmd)
        {
            case COMMAND_PLAY:
                audio.Play();
                break;
            case COMMAND_MUTE:
                audio.mute = ParameterToBool(parameter);
                break;
            case COMMAND_SOUND:
                audio.clip = LoadAudioClip(parameter);
                break;
            case COMMAND_VOLUME:
                audio.volume = float.Parse(parameter);
                break;
            case COMMAND_PRIORITY:
                audio.priority = int.Parse(parameter);
                break;
            case COMMAND_LOOP:
                audio.loop = ParameterToBool(parameter);
                break;
            case COMMAND_FADE:
                FadeSound(audio, parameter);
                break;
            case COMMAND_ACTIVE:
                audio.gameObject.SetActive(ParameterToBool(parameter));
                break;
            case COMMAND_DELETE:
                _seList.Remove(audio);
                Destroy(audio.gameObject);
                break;
        }
    }

    /**
     * 音声ファイルを読み出し、インスタンス化する
     */
    private AudioClip LoadAudioClip(string name)
    {
        return Instantiate(Resources.Load<AudioClip>(audioClipsDirectory + name));
    }

    /**
     * 音声にフェードをかける
     */
    private void FadeSound(AudioSource audio, string parameter)
    {
        string[] ps = parameter.Replace(" ", "").Split(',');
        StartCoroutine(FadeSound(audio, int.Parse(ps[0]), int.Parse(ps[1])));
    }

    /**
     * ゲーム中パラメーターの設定
     */
    private void SetParameterForGame(string name, string cmd, string parameter, Dictionary<string, object> _params)
    {
        cmd = cmd.Replace(" ", "");
        name = GetParameterForCommand(name);

        switch (cmd)
        {
            case COMMAND_WRITE:
                _params[name] = GetParameterForCommand(parameter);
                break;
            case COMMAND_CALC:
                _params[name] = CalcParameterForGame(parameter);
                break;

         /***************************************** 自作 *****************************************/

            case COMMAND_MYREAD:
                _params[name] = MyParameterRead(parameter);
                break;
            case COMMAND_MYWRITE:
                MyParameterWrite(parameter, _params[name]);
                break;
            case COMMAND_MYPLUS:
                StartCoroutine(ScoreAnimation(parameter, 0.75f));
                break;

         /******************************************************************************************/
        }
    }

    /**
     * ゲーム中パラメーターの演算処理
     */
    private object CalcParameterForGame(string parameter)
    {
        parameter = GetParameterForCommand(parameter);
        object result = _dt.Compute(parameter, "");
        return result;
    }

    /**
     * コマンド中の「""」に挟まれた値を取り出す
     */
    private string GetParameterForCommand(string parameter)
    {
        parameter = parameter.Substring(parameter.IndexOf('"') + 1, parameter.LastIndexOf('"') - parameter.IndexOf('"') - 1);
        return ReplaceParameterForGame(parameter.Replace(" ", ""));
    }


    /**
     * テキストファイルを読み込む
     */
    private string LoadTextFile(string fname)
    {
        TextAsset textasset = Resources.Load<TextAsset>(fname);
        return textasset.text.Replace("\n", "").Replace("\r", "");
    }

#endregion
}