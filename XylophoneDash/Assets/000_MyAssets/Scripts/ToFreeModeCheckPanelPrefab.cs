using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Cinemachine.DocumentationSortingAttribute;

public class ToFreeModeCheckPanelPrefab : MonoBehaviour
{
    [SerializeField] private Image insideImage;
    [SerializeField] private TextMeshProUGUI levelText;

    private PlayerInformationManager.GameLevel myLevel;

    public void SettingMyStatus(PlayerInformationManager.GameLevel level)
    {
        myLevel = level;

        if (level == PlayerInformationManager.GameLevel.None)
        {
            levelText.text = "Error";
        }
        else
        {
            insideImage.color = PlayerInformationManager.Instance.GetLevelColor(level);
            levelText.text = "“ïˆÕ“x : " + level.ToString();
        }

    }

    public void PushMyPlayButton()
    {
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Ok);

        PlayerInformationManager.PlayGameInformation info;

        info.mode = PlayerInformationManager.GameMode.FreeStyle;
        info.level = myLevel;
        info.stageNumber = 0;
        info.sheetMusicName = "";
        info.targetTime = 0;
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
