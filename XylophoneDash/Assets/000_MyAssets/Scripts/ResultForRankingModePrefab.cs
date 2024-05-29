using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultForRankingModePrefab : MonoBehaviour
{
    [SerializeField] private Button showRankingButton;
    [SerializeField] private Button goToMenuSceneButton;
    [SerializeField] private Button resetButton;

    [SerializeField] private Image insideImage;
    [SerializeField] private TextMeshProUGUI sheetMusicTitleText;
    [SerializeField] private TextMeshProUGUI stageNumberText;

    [SerializeField] private TextMeshProUGUI thisTimeText;
    [SerializeField] private TextMeshProUGUI thisMissCountText;
    [SerializeField] private TextMeshProUGUI thisScoreText;
    [SerializeField] private TextMeshProUGUI recordScoreText;

    [SerializeField] private GameObject showRankingPanelPrefab;

    private float clearTime;
    private int clearMissCount;

    private float thisScore;
    private float recordScore;

    private const float DEFAULT_RECORD_TIME = 99999.9f;

    private bool disableButtonFlg;

    private bool testFlg;

    private PlayerInformationManager.PlayGameInformation gameInfo;

    private void Start()
    {
        InitValue();
        ChangeEnableButtons(false);

        CheckClearResult();

        SettingDisplay();

        StartCoroutine(ResultStagingCoroutine());
    }


    /// <summary>
    /// 変数の初期化
    /// </summary>
    private void InitValue()
    {
        testFlg = PlayerInformationManager.Instance != null ? false : true;

        disableButtonFlg = false;

       
    }

    /// <summary>
    /// 画面情報を設定
    /// </summary>
    private void SettingDisplay()
    {
        if (!testFlg)
        {
            insideImage.color = PlayerInformationManager.Instance.GetRankingModeColor();

            sheetMusicTitleText.text = gameInfo.sheetMusicName;
            stageNumberText.text = (gameInfo.stageNumber+1).ToString();

            int displayTime1 = (int)(clearTime * 100f);
            float displayTime2 = (float)displayTime1 / 100f;
            
            thisTimeText.text = displayTime2.ToString($"F2") + " 秒";
            thisMissCountText.text = clearMissCount.ToString() + " 回";
            thisScoreText.text = thisScore.ToString($"F2");

            if (recordScore == DEFAULT_RECORD_TIME)
            {
                recordScoreText.text = " - - -";
            }
            else
            {
                recordScoreText.text = recordScore.ToString($"F2");
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
            recordScore = PlayerInformationManager.Instance.rankingTimeRecord[gameInfo.stageNumber];

            if (recordScore <= 0f)
            {
                recordScore = DEFAULT_RECORD_TIME;
            }

            // ランキングを送信
            thisScore = PlayFabManager.Instance.RankingSubmitScore(gameInfo.rankingKey, clearTime, clearMissCount);

            if(thisScore < recordScore)
            {
                PlayerInformationManager.Instance.UpdateRankingRecordScore(gameInfo.stageNumber, thisScore);
            }
        }
    }

    /// <summary>
    /// ランキングを見るボタンを押したとき
    /// </summary>
    public void PushDisplayRankingButton()
    {
        if (disableButtonFlg) return;
        ChangeEnableButtons(false);

        //ランキングを見る処理
        ShowRankingPanelPrefab panel = Instantiate(showRankingPanelPrefab, GameSceneManager.Instance.GetCanvasTransform())
            .GetComponent<ShowRankingPanelPrefab>();
        panel.SetGameScene(gameInfo.rankingKey,gameObject.GetComponent<ResultForRankingModePrefab>());
    }

    /// <summary>
    /// ShowRankingPanelPrefabから、閉じたことを受信する
    /// </summary>
    public void ReceptionBackFromShowRankingPanelPrefab()
    {
        ChangeEnableButtons(true);
    }

    /// <summary>
    /// リトライボタンを押したとき
    /// </summary>
    public void PushRetryButton()
    {
        if (disableButtonFlg) return;
        ChangeEnableButtons(false);

        GameSceneManager.Instance.ReceptionRetry();
    }

    /// <summary>
    /// メニューシーンへ移行するボタンを押したとき
    /// </summary>
    public void PushGoToMenuSceneButton()
    {
        if (disableButtonFlg) return;
        ChangeEnableButtons(false);

        GameSceneManager.Instance.ReceptionGoToMenuScene();
    }

    /// <summary>
    /// ボタンの押下可能かを切り替える
    /// </summary>
    /// <param name="flg"></param>
    private void ChangeEnableButtons(bool flg)
    {
        disableButtonFlg = !flg;

        showRankingButton.interactable = flg;
        goToMenuSceneButton.interactable = flg;
        resetButton.interactable = flg;
    }

    /// <summary>
    /// 結果画面の演出を司るコルーチン
    /// </summary>
    /// <returns></returns>
    private IEnumerator ResultStagingCoroutine()
    {
        //if (!testFlg)
        //{
        //    for (int i = 0; i < 4; i++)
        //    {
        //        if (clearFlgAfter[i] && !clearFlgBefore[i])
        //        {
        //            StartCoroutine(StarGetStagingCoroutine(i));
        yield return new WaitForSeconds(0.5f);
        //        }
        //    }

        //}

        ChangeEnableButtons(true);
    }



}
