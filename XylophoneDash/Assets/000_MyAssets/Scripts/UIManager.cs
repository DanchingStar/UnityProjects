using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject gameMenuPrefab;
    [SerializeField] private GameObject resultForStageClearModePrefab;
    [SerializeField] private GameObject resultForRankingModePrefab;
    [SerializeField] private GameObject longTimeBackGroundPrefab;

    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI missCountText;
    [SerializeField] private TextMeshProUGUI next1Text;
    [SerializeField] private TextMeshProUGUI next2Text;
    [SerializeField] private TextMeshProUGUI restNotesText;

    [SerializeField] private TextMeshProUGUI centerText;

    /// <summary> 真ん中の文字のデフォルトカラー </summary>
    private readonly Color CENTER_COLOR_DEFAULT = new Color32(255, 0, 0, 255);
    /// <summary> 真ん中の文字の消えるカラー </summary>
    private readonly Color CENTER_COLOR_TRANSPARENT = new Color32(255, 0, 0, 0);
    /// <summary> 0から1をとり、大きくなるにつれ消えていく </summary>
    private float changeColorLeap;
    /// <summary> 色が元の色へと変化するスピード </summary>
    private const float CHANGE_COLOR_SPEED = 0.3f;

    [SerializeField] private TextMeshProUGUI testText1;
    [SerializeField] private TextMeshProUGUI testText2;
    [SerializeField] private TextMeshProUGUI testText3;

    [SerializeField] private GameObject skipButtonObject;
    private Button skipButton;
    [SerializeField] private GameObject menuButtonObject;
    private Button menuButton;

    private void Start()
    {
        centerText.color = CENTER_COLOR_TRANSPARENT;
        changeColorLeap = 1f;

        skipButton = skipButtonObject.GetComponent<Button>();
        menuButton = menuButtonObject.GetComponent<Button>();
    }

    private void Update()
    {
        UpdateCenterTextColor();
        SettingButtons();
    }

    /// <summary>
    /// デバッグ用テストボタンの処理
    /// </summary>
    /// <param name="num"></param>
    public void PushTestButton(int num)
    {
        switch (num)
        {
            case 1:
                //
                break;
            case 2:
                //
                break;
            case 3:
                //
                break;
            case 4:
                //
                break;
            case 5:
                //
                break;
            case 6:
                //
                break;
            case 7:
                //
                break;
            case 8:
                // 定点カメラ
                GameSceneManager.Instance.ChangeFixedPointCamera(true);
                break;
            case 9:
                // 追従カメラ
                GameSceneManager.Instance.ChangeFixedPointCamera(false);
                break;
            case 10:
                // 定点カメラと追従カメラを切り替える
                GameSceneManager.Instance.SwitchFixedPointCamera();
                break;
            case 11:
                // 

                break;
            case 12:
                // 

                break;
            case 13:
                // 

                break;
            case 14:
                // 

                break;
            default:
                Debug.Log("GameSceneManager.PushTestButton : No Test Number");
                break;
        }
    }

    /// <summary>
    /// デバッグ用テキストを変更する
    /// </summary>
    /// <param name="text"></param>
    /// <param name="number"></param>
    public void SetTestText(string text, int number)
    {
        switch (number)
        {
            case 1:
                testText1.text = text;
                break;
            case 2:
                testText2.text = text;
                break;
            case 3:
                testText3.text = text;
                break;
            default:
                Debug.LogError($"UIManager.SetTestText : Number Error -> {number}");
                break;
        }
    }

    /// <summary>
    /// 時間のテキストを変更する
    /// </summary>
    /// <param name="num"></param>
    public void SetTimerText(float num)
    {
        timerText.text = "タイム : " + num.ToString("000.0");
    }

    /// <summary>
    /// 時間のテキストを変更する
    /// </summary>
    /// <param name="str"></param>
    public void SetTimerText(string str)
    {
        timerText.text = str;
    }

    /// <summary>
    /// 残りノーツ数のテキストを変更する
    /// </summary>
    /// <param name="num"></param>
    public void SetRestNotesText(int num)
    {
        restNotesText.text = $"ノーツ数 : {num}";
    }

    /// <summary>
    /// 残りノーツ数のテキストを変更する
    /// </summary>
    /// <param name="str"></param>
    public void SetRestNotesText(string str)
    {
        restNotesText.text = str;
    }

    /// <summary>
    /// ミスカウントのテキストを変更する
    /// </summary>
    /// <param name="text"></param>
    public void SetMissCountText(int num)
    {
        missCountText.text = $"ミス : {num}";
    }

    /// <summary>
    /// ミスカウントのテキストを変更する
    /// </summary>
    /// <param name="str"></param>
    public void SetMissCountText(string str)
    {
        missCountText.text = str;
    }

    /// <summary>
    /// ネクストのテキストを変更する
    /// </summary>
    /// <param name="text"></param>
    public void SetNext1Text(XylophoneManager.Notes notes)
    {
        if (notes == XylophoneManager.Notes.None)
        {
            next1Text.text = "";
        }
        else
        {
            next1Text.text = XylophoneManager.Instance.GetNotesName(notes);
        }
    }

    /// <summary>
    /// ネクネクのテキストを変更する
    /// </summary>
    /// <param name="text"></param>
    public void SetNext2Text(XylophoneManager.Notes notes)
    {
        if (notes == XylophoneManager.Notes.None)
        {
            next2Text.text = "";
        }
        else
        {
            next2Text.text = XylophoneManager.Instance.GetNotesName(notes);
        }
    }

    /// <summary>
    /// 真ん中のテキストの文字を設定する
    /// </summary>
    /// <param name="text"></param>
    public void SetCenterText(string text)
    {
        centerText.text = text;
        changeColorLeap = 0;
    }

    /// <summary>
    /// フリーモード時のテキスト変更
    /// </summary>
    /// <param name="level"></param>
    public void SetFreeMode(PlayerInformationManager.GameLevel level)
    {
        SetTimerText("フリーモード");
        SetMissCountText("難易度  : ");
        SetRestNotesText(level.ToString());
        SetNext1Text(XylophoneManager.Notes.None);
        SetNext2Text(XylophoneManager.Notes.None);

        missCountText.alignment = TextAlignmentOptions.Right;
        restNotesText.alignment = TextAlignmentOptions.Left;
    }

    /// <summary>
    /// 真ん中のテキストの色を更新する
    /// </summary>
    private void UpdateCenterTextColor()
    {
        if (changeColorLeap > 1f) return;

        changeColorLeap += Time.deltaTime * CHANGE_COLOR_SPEED;
        centerText.color = Color.Lerp(CENTER_COLOR_DEFAULT, CENTER_COLOR_TRANSPARENT, changeColorLeap);

    }

    /// <summary>
    /// Menuボタンを押したとき
    /// </summary>
    public void PushMenuButton()
    {
        if (GameSceneManager.Instance.GetPauseFlg()) return;

        GameSceneManager.Instance.ChangePauseFlg(true);
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.GameMenuOpen);
        Instantiate(gameMenuPrefab, this.transform);
    }

    /// <summary>
    /// 結果画面を表示する
    /// </summary>
    public void DisplayResultUI()
    {
        var gameMode = GameSceneManager.Instance.GetGameInformation().mode;

        if (gameMode == PlayerInformationManager.GameMode.StageClear)
        {
            Instantiate(resultForStageClearModePrefab, this.transform);
        }
        else if (gameMode == PlayerInformationManager.GameMode.Ranking)
        {
            Instantiate(resultForRankingModePrefab, this.transform);
        }
        else
        {
            Debug.LogError($"UIManager.DisplayResultUI : Mode Error -> {gameMode}");
        }
    }

    /// <summary>
    /// Skipボタンを押したとき
    /// </summary>
    public void PushSkipButton()
    {
        GameSceneManager.Instance.ReceptionSkipForExample();
        skipButton.interactable = false;
    }

    /// <summary>
    /// ボタンの表示などの設定
    /// </summary>
    private void SettingButtons()
    {
        if (GameSceneManager.Instance.GetNowPhase() == GameSceneManager.NowPhase.ForExample && skipButtonObject.activeSelf == false)
        {
            skipButtonObject.SetActive(true);
            skipButton.interactable = true;
        }
        else if (GameSceneManager.Instance.GetNowPhase() != GameSceneManager.NowPhase.ForExample && skipButtonObject.activeSelf == true)
        {
            skipButton.interactable = false;
            skipButtonObject.SetActive(false);
        }

        if (GameSceneManager.Instance.GetNowPhase() == GameSceneManager.NowPhase.Play && menuButton.interactable == false)
        {
            menuButton.interactable = true;
        }
        else if (GameSceneManager.Instance.GetNowPhase() != GameSceneManager.NowPhase.Play && menuButton.interactable == true)
        {
            menuButton.interactable = false;
        }
    }

    /// <summary>
    /// LongTimeBackGroundPrefabを生成する
    /// </summary>
    public void InstantiateLongTimeBackGroundPrefab()
    {
        Instantiate(longTimeBackGroundPrefab, transform);
    }

}
