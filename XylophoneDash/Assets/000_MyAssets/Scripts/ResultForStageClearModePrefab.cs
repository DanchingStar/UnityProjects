using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultForStageClearModePrefab : MonoBehaviour
{
    [SerializeField] private Button goToMenuSceneButton;
    [SerializeField] private Button resetButton;

    [SerializeField] private Image insideImage;
    [SerializeField] private TextMeshProUGUI sheetMusicTitleText;
    [SerializeField] private TextMeshProUGUI stageNumberText;
    [SerializeField] private Image[] starImages;
    [SerializeField] private TextMeshProUGUI targetTimeText;
    [SerializeField] private TextMeshProUGUI thisTimeText;
    [SerializeField] private TextMeshProUGUI recordTimeText;

    private const float DEFAULT_RECORD_TIME = 99999.9f;

    private float recordTime;

    private float clearTime;
    private int clearMissCount;

    private bool[] clearFlgBefore;
    private bool[] clearFlgThisTime;

    private bool disableButtonFlg;

    private bool testFlg;

    private PlayerInformationManager.PlayGameInformation gameInfo;

    private void Start()
    {
        InitValue();
        ChangeEnableButtons(false);

        CheckClearResult();

        SettingDisplay();

        SaveResult();

        StartCoroutine(ResultStagingCoroutine());
    }


    /// <summary>
    /// 変数の初期化
    /// </summary>
    private void InitValue()
    {
        testFlg = PlayerInformationManager.Instance != null ? false : true;

        disableButtonFlg = false;

        clearFlgBefore = new bool[4];
        clearFlgThisTime = new bool[4];

    }

    /// <summary>
    /// 画面情報を設定
    /// </summary>
    private void SettingDisplay()
    {
        if(!testFlg)
        {
            if (clearFlgBefore[3])
            {
                starImages[0].sprite = PlayerInformationManager.Instance.GetStarSprite(2);
                starImages[1].sprite = PlayerInformationManager.Instance.GetStarSprite(2);
                starImages[2].sprite = PlayerInformationManager.Instance.GetStarSprite(2);
            }
            else
            {
                starImages[0].sprite = clearFlgBefore[0] ?
                    PlayerInformationManager.Instance.GetStarSprite(1) : PlayerInformationManager.Instance.GetStarSprite(0);
                starImages[1].sprite = clearFlgBefore[1] ?
                    PlayerInformationManager.Instance.GetStarSprite(1) : PlayerInformationManager.Instance.GetStarSprite(0);
                starImages[2].sprite = clearFlgBefore[2] ?
                    PlayerInformationManager.Instance.GetStarSprite(1) : PlayerInformationManager.Instance.GetStarSprite(0);
            }

            insideImage.color = PlayerInformationManager.Instance.GetLevelColor(gameInfo.level);
            sheetMusicTitleText.text = gameInfo.sheetMusicName;
            stageNumberText.text = (gameInfo.stageNumber+1).ToString();
            targetTimeText.text = gameInfo.targetTime.ToString($"F1") + " s";
            thisTimeText.text = clearTime.ToString($"F1") + " s";

            if (recordTime == DEFAULT_RECORD_TIME)
            {
                recordTimeText.text = " - - -";
            }
            else
            {
                recordTimeText.text = recordTime.ToString($"F1") + " s";
            }
        }
    }

    /// <summary>
    /// ゲームクリア後の結果を調べる
    /// </summary>
    private void CheckClearResult()
    {
        clearTime = GameSceneManager.Instance.GetPlayTime();
        clearMissCount = GameSceneManager.Instance.GetMissCount();

        if (!testFlg)
        {
            gameInfo = new PlayerInformationManager.PlayGameInformation();
            gameInfo = PlayerInformationManager.Instance.GetNextPlayGameInformation();

            if (gameInfo.level == PlayerInformationManager.GameLevel.Normal)
            {
                recordTime = PlayerInformationManager.Instance.stageClearStarFlgsForNormal[gameInfo.stageNumber].bestTime;
                clearFlgBefore[0] = PlayerInformationManager.Instance.stageClearStarFlgsForNormal[gameInfo.stageNumber].star1;
                clearFlgBefore[1] = PlayerInformationManager.Instance.stageClearStarFlgsForNormal[gameInfo.stageNumber].star2;
                clearFlgBefore[2] = PlayerInformationManager.Instance.stageClearStarFlgsForNormal[gameInfo.stageNumber].star3;
                clearFlgBefore[3] = PlayerInformationManager.Instance.stageClearStarFlgsForNormal[gameInfo.stageNumber].starAll;
            }
            else if (gameInfo.level == PlayerInformationManager.GameLevel.Hard)
            {
                recordTime = PlayerInformationManager.Instance.stageClearStarFlgsForHard[gameInfo.stageNumber].bestTime;
                clearFlgBefore[0] = PlayerInformationManager.Instance.stageClearStarFlgsForHard[gameInfo.stageNumber].star1;
                clearFlgBefore[1] = PlayerInformationManager.Instance.stageClearStarFlgsForHard[gameInfo.stageNumber].star2;
                clearFlgBefore[2] = PlayerInformationManager.Instance.stageClearStarFlgsForHard[gameInfo.stageNumber].star3;
                clearFlgBefore[3] = PlayerInformationManager.Instance.stageClearStarFlgsForHard[gameInfo.stageNumber].starAll;
            }
            else if (gameInfo.level == PlayerInformationManager.GameLevel.VeryHard)
            {
                recordTime = PlayerInformationManager.Instance.stageClearStarFlgsForVeryHard[gameInfo.stageNumber].bestTime;
                clearFlgBefore[0] = PlayerInformationManager.Instance.stageClearStarFlgsForVeryHard[gameInfo.stageNumber].star1;
                clearFlgBefore[1] = PlayerInformationManager.Instance.stageClearStarFlgsForVeryHard[gameInfo.stageNumber].star2;
                clearFlgBefore[2] = PlayerInformationManager.Instance.stageClearStarFlgsForVeryHard[gameInfo.stageNumber].star3;
                clearFlgBefore[3] = PlayerInformationManager.Instance.stageClearStarFlgsForVeryHard[gameInfo.stageNumber].starAll;
            }

            if(recordTime <= 0f)
            {
                recordTime = DEFAULT_RECORD_TIME;
            }

            bool clearFlg2 = false;
            if (clearMissCount == 0)
            {
                clearFlg2 = true;
            }
            bool clearFlg3 = false;
            if (gameInfo.targetTime > clearTime)
            {
                clearFlg3 = true;
            }

            if (clearFlg2 && clearFlg3)
            {
                clearFlgThisTime[0] = true;
                clearFlgThisTime[1] = true;
                clearFlgThisTime[2] = true;
                clearFlgThisTime[3] = true;
            }
            else if (clearFlg2)
            {
                clearFlgThisTime[0] = true;
                clearFlgThisTime[1] = true;
                clearFlgThisTime[2] = false;
                clearFlgThisTime[3] = false;
            }
            else if (clearFlg3)
            {
                clearFlgThisTime[0] = true;
                clearFlgThisTime[1] = false;
                clearFlgThisTime[2] = true;
                clearFlgThisTime[3] = false;
            }
            else
            {
                clearFlgThisTime[0] = true;
                clearFlgThisTime[1] = false;
                clearFlgThisTime[2] = false;
                clearFlgThisTime[3] = false;
            }

        }
    }

    /// <summary>
    /// リトライボタンを押したとき
    /// </summary>
    public void PushRetryButton()
    {
        if (disableButtonFlg) return;
        disableButtonFlg = true;

        GameSceneManager.Instance.ReceptionRetry();
    }

    /// <summary>
    /// メニューシーンへ移行するボタンを押したとき
    /// </summary>
    public void PushGoToMenuSceneButton()
    {
        if (disableButtonFlg) return;
        disableButtonFlg = true;

        GameSceneManager.Instance.ReceptionGoToMenuScene();
    }

    /// <summary>
    /// ボタンの押下可能かを切り替える
    /// </summary>
    /// <param name="flg"></param>
    private void ChangeEnableButtons(bool flg)
    {
        goToMenuSceneButton.interactable = flg;
        resetButton.interactable = flg;
    }

    /// <summary>
    /// 結果画面の演出を司るコルーチン
    /// </summary>
    /// <returns></returns>
    private IEnumerator ResultStagingCoroutine()
    {
        if (!testFlg)
        {
            for (int i = 0; i < 4; i++)
            {
                if (clearFlgThisTime[i] && !clearFlgBefore[i])
                {
                    StartCoroutine(StarGetStagingCoroutine(i));
                    yield return new WaitForSeconds(0.5f);
                }
            }

        }

        ChangeEnableButtons(true);
    }

    /// <summary>
    /// スター獲得時の演出を司るコルーチン
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private IEnumerator StarGetStagingCoroutine(int index)
    {
        if(index == 3)
        {
            yield return new WaitForSeconds(0.25f);
            starImages[0].sprite = PlayerInformationManager.Instance.GetStarSprite(2);
            starImages[1].sprite = PlayerInformationManager.Instance.GetStarSprite(2);
            starImages[2].sprite = PlayerInformationManager.Instance.GetStarSprite(2);
        }
        else
        {
            yield return new WaitForSeconds(0.25f);
            starImages[index].sprite = PlayerInformationManager.Instance.GetStarSprite(1);
        }


    }

    /// <summary>
    /// 結果が更新された場合、保存する
    /// </summary>
    private void SaveResult()
    {
        bool updateFlg = false;

        for (int i = 0; i < 4; i++)
        {
            if (!clearFlgBefore[i] && clearFlgThisTime[i])
            {
                updateFlg = true;
                break;
            }
        }
        if (!updateFlg && recordTime > clearTime)
        {
            updateFlg = true;
        }

        if (updateFlg)
        {
            bool[] clearFlgAfter = new bool[4];
            for (int i = 0; i < clearFlgAfter.Length; i++) clearFlgAfter[i] = clearFlgThisTime[i] || clearFlgBefore[i];

            if (gameInfo.level == PlayerInformationManager.GameLevel.Normal)
            {
                PlayerInformationManager.Instance.stageClearStarFlgsForNormal[gameInfo.stageNumber].star1 = clearFlgAfter[0];
                PlayerInformationManager.Instance.stageClearStarFlgsForNormal[gameInfo.stageNumber].star2 = clearFlgAfter[1];
                PlayerInformationManager.Instance.stageClearStarFlgsForNormal[gameInfo.stageNumber].star3 = clearFlgAfter[2];
                PlayerInformationManager.Instance.stageClearStarFlgsForNormal[gameInfo.stageNumber].starAll = clearFlgAfter[3];
                PlayerInformationManager.Instance.stageClearStarFlgsForNormal[gameInfo.stageNumber].bestTime =
                    recordTime < clearTime ? recordTime : clearTime;
            }
            else if (gameInfo.level == PlayerInformationManager.GameLevel.Hard)
            {
                PlayerInformationManager.Instance.stageClearStarFlgsForHard[gameInfo.stageNumber].star1 = clearFlgAfter[0];
                PlayerInformationManager.Instance.stageClearStarFlgsForHard[gameInfo.stageNumber].star2 = clearFlgAfter[1];
                PlayerInformationManager.Instance.stageClearStarFlgsForHard[gameInfo.stageNumber].star3 = clearFlgAfter[2];
                PlayerInformationManager.Instance.stageClearStarFlgsForHard[gameInfo.stageNumber].starAll = clearFlgAfter[3];
                PlayerInformationManager.Instance.stageClearStarFlgsForHard[gameInfo.stageNumber].bestTime =
                    recordTime < clearTime ? recordTime : clearTime;
            }
            else if (gameInfo.level == PlayerInformationManager.GameLevel.VeryHard)
            {
                PlayerInformationManager.Instance.stageClearStarFlgsForVeryHard[gameInfo.stageNumber].star1 = clearFlgAfter[0];
                PlayerInformationManager.Instance.stageClearStarFlgsForVeryHard[gameInfo.stageNumber].star2 = clearFlgAfter[1];
                PlayerInformationManager.Instance.stageClearStarFlgsForVeryHard[gameInfo.stageNumber].star3 = clearFlgAfter[2];
                PlayerInformationManager.Instance.stageClearStarFlgsForVeryHard[gameInfo.stageNumber].starAll = clearFlgAfter[3];
                PlayerInformationManager.Instance.stageClearStarFlgsForVeryHard[gameInfo.stageNumber].bestTime =
                    recordTime < clearTime ? recordTime : clearTime;
            }

            PlayerInformationManager.Instance.UpdateSaveDataForStageClearMode();

            Debug.Log("ResultForStageClearModePrefab.SaveResult : Saved");
        }
        else
        {
            Debug.Log("ResultForStageClearModePrefab.SaveResult : Didn't Save");
        }
    }


}
