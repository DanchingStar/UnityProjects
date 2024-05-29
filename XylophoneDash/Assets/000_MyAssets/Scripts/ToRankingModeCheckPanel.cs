using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToRankingModeCheckPanel : MonoBehaviour
{
    [SerializeField] private Image insideImage;

    [SerializeField] private TextMeshProUGUI sheetMusicTitleText;
    [SerializeField] private TextMeshProUGUI stageNumberText;
    [SerializeField] private TextMeshProUGUI levelText;

    [SerializeField] private TextMeshProUGUI recordScoreStringText;
    [SerializeField] private TextMeshProUGUI recordScoreText;

    [SerializeField] private TextMeshProUGUI unlockConditionStringText;
    [SerializeField] private TextMeshProUGUI unlockConditionText;

    [SerializeField] private GameObject objectToPlayButton;

    [SerializeField] private GameObject showRankingPanelPrefab;

    private bool canPlayFlg;

    private PlayerInformationManager.GameLevel level;
    private int stageNumber;

    private string sheetMusicTitle;
    private int unlockCondition;
    private string rankingKey;

    public void SettingForCanPlay(PlayerInformationManager.GameLevel _level, int _stageNumber)
    {
        canPlayFlg = true;

        level = _level;
        stageNumber = _stageNumber;

        LoadValues();
        DisplaySetting();
    }

    public void SettingForCannotPlay(PlayerInformationManager.GameLevel _level, int _stageNumber)
    {
        canPlayFlg = false;

        level = _level;
        stageNumber = _stageNumber;

        LoadValues();
        DisplaySetting();
    }

    private void LoadValues()
    {
        sheetMusicTitle = PlayerInformationManager.Instance.
            GetRankingModeListGenerator().GetSheetMusicName(stageNumber);

        unlockCondition = PlayerInformationManager.Instance.
            GetRankingModeListGenerator().GetUnlockStageNumber(stageNumber);

        rankingKey = PlayerInformationManager.Instance.
            GetRankingModeListGenerator().GetKey(stageNumber);


    }

    private void DisplaySetting()
    {
        insideImage.color = PlayerInformationManager.Instance.GetRankingModeColor();

        recordScoreStringText.gameObject.SetActive(canPlayFlg);
        recordScoreText.gameObject.SetActive(canPlayFlg);
        unlockConditionStringText.gameObject.SetActive(!canPlayFlg);
        unlockConditionText.gameObject.SetActive(!canPlayFlg);
        objectToPlayButton.SetActive(canPlayFlg);

        sheetMusicTitleText.text = sheetMusicTitle;
        stageNumberText.text = (stageNumber+1).ToString();
        levelText.text = level.ToString();
        levelText.color = PlayerInformationManager.Instance.GetLevelColor(level);

        recordScoreStringText.text = $"マイベストスコア";
        float recordTime = PlayerInformationManager.Instance.rankingTimeRecord[stageNumber];
        if (recordTime > 0f)
        {
            recordScoreText.text = recordTime.ToString($"F2");
        }
        else
        {
            recordScoreText.text = " - - -";
        }
        unlockConditionStringText.text = $"[解放条件]";
        unlockConditionText.text = $"ステージクリアモード\n{level} ステージ{unlockCondition+1}で、\nスターを1個以上 獲得する。";
    }


    public void PushShowRankingButton()
    {
        ShowRankingPanelPrefab panel = Instantiate(showRankingPanelPrefab, MenuSceneManager.Instance.GetCanvasTransform()).GetComponent<ShowRankingPanelPrefab>();
        panel.SetMenuScene(rankingKey);
    }

    public void PushMyPlayButton()
    {
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Ok);

        PlayerInformationManager.PlayGameInformation info;

        info.mode = PlayerInformationManager.GameMode.Ranking;
        info.level = level;
        info.stageNumber = stageNumber;
        info.sheetMusicName = sheetMusicTitle;
        info.targetTime = 0;
        info.playerCharacterName = PlayerInformationManager.Instance.GetPlayerCharacterName();
        info.skipForExampleFlg = false;
        info.rankingKey = rankingKey;

        PlayerInformationManager.Instance.SetNextPlayGameInformation(info);
        MenuSceneManager.Instance.ToGameScene();
    }

    public void PushMyBackButton()
    {
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.No);
        Destroy(gameObject);
    }

}
