using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToStageClearModeCheckPanel : MonoBehaviour
{
    [SerializeField] private Image insideImage;

    [SerializeField] private TextMeshProUGUI sheetMusicTitleText;
    [SerializeField] private TextMeshProUGUI stageNumberText;

    [SerializeField] private Image imageStarA;
    [SerializeField] private Image imageStarB;
    [SerializeField] private Image imageStarC;

    [SerializeField] private TextMeshProUGUI targetTimeStringText;
    [SerializeField] private TextMeshProUGUI targetTimeText;
    [SerializeField] private TextMeshProUGUI recordTimeStringText;
    [SerializeField] private TextMeshProUGUI recordTimeText;

    [SerializeField] private TextMeshProUGUI unlockConditionStringText;
    [SerializeField] private TextMeshProUGUI unlockConditionText;

    [SerializeField] private GameObject objectToPlayButton;

    private bool canPlayFlg;

    private PlayerInformationManager.GameLevel level;
    private int stageNumber;

    private string sheetMusicTitle;
    private float targetTime;
    private float recordTime;
    private int unlockCondition;

    public void SettingForCanPlay(PlayerInformationManager.GameLevel _level,int _stageNumber
        , float _recordTime, Sprite _starA, Sprite _starB, Sprite _starC)
    {
        canPlayFlg = true;

        level = _level;
        stageNumber = _stageNumber;
        recordTime = _recordTime; 

        SpriteSetting(_starA, _starB, _starC);
        LoadValues();
        DisplaySetting();
    }

    public void SettingForCannotPlay(PlayerInformationManager.GameLevel _level, int _stageNumber
        , Sprite _starA, Sprite _starB, Sprite _starC)
    {
        canPlayFlg = false;

        level = _level;
        stageNumber = _stageNumber;
        recordTime = 999.9f;

        SpriteSetting(_starA, _starB, _starC);
        LoadValues();
        DisplaySetting();
    }

    private void SpriteSetting(Sprite _starA, Sprite _starB, Sprite _starC)
    {
        insideImage.color = PlayerInformationManager.Instance.GetLevelColor(level);
        imageStarA.sprite = _starA;
        imageStarB.sprite = _starB;
        imageStarC.sprite = _starC;
    }

    private void LoadValues()
    {
        targetTime = PlayerInformationManager.Instance.
            GetStageClearModeListGenerator().GetTargetTime(level, stageNumber);

        sheetMusicTitle = PlayerInformationManager.Instance.
            GetStageClearModeListGenerator().GetSheetMusicName(level, stageNumber);

        unlockCondition = PlayerInformationManager.Instance.
            GetStageClearModeListGenerator().GetUnlockCondition(level, stageNumber);


    }

    private void DisplaySetting()
    {
        targetTimeStringText.gameObject.SetActive(canPlayFlg);
        targetTimeText.gameObject.SetActive(canPlayFlg);
        recordTimeStringText.gameObject.SetActive(canPlayFlg);
        recordTimeText.gameObject.SetActive(canPlayFlg);
        unlockConditionStringText.gameObject.SetActive(!canPlayFlg);
        unlockConditionText.gameObject.SetActive(!canPlayFlg);
        objectToPlayButton.SetActive(canPlayFlg);

        sheetMusicTitleText.text = sheetMusicTitle;
        stageNumberText.text = (stageNumber+1).ToString();

        targetTimeStringText.text = "目標タイム : ";
        targetTimeText.text = targetTime.ToString($"F1") + " 秒";

        recordTimeStringText.text = "マイベストタイム : ";
        if (recordTime > 0f)
        {
            recordTimeText.text = recordTime.ToString($"F1") + " 秒";
        }
        else
        {
            recordTimeText.text = " - - -";
        }

        int haveStars = PlayerInformationManager.Instance.GetHaveStarsForStageClearMode(level);
        unlockConditionStringText.text = $"[解放条件]";
        unlockConditionText.text = $"難易度{level}で、\nスターを {unlockCondition}個 獲得する。\n(現在の獲得数 : {haveStars}個)" ;
    }

    public void PushMyPlayButton()
    {
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Ok);

        PlayerInformationManager.PlayGameInformation info;

        info.mode = PlayerInformationManager.GameMode.StageClear;
        info.level = level;
        info.stageNumber = stageNumber;
        info.sheetMusicName = sheetMusicTitle;
        info.targetTime = targetTime;
        info.playerCharacterName = PlayerInformationManager.Instance.GetPlayerCharacterName();
        info.skipForExampleFlg = false;
        info.rankingKey = "";

        PlayerInformationManager.Instance.SetNextPlayGameInformation(info);
        MenuSceneManager.Instance.ToGameScene();
    }

    public void PushMyBackButton()
    {
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.No);
        Destroy(gameObject);
    }

}
