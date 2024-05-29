using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectStageButtonForRankingMode : MonoBehaviour
{
    [SerializeField] private GameObject toGameSceneCheckPanelPrefab;

    [SerializeField] private Image imageBackground;
    [SerializeField] private Image imageLevelColorBackground;
    [SerializeField] private Image imageLockBackground;
    [SerializeField] private Image imageLock;
    [SerializeField] private TextMeshProUGUI textStageNumber;
    
    private PlayerInformationManager.GameLevel level;
    private int stageNumber;
    private bool lockPlayFlg;

    /// <summary>
    /// 自身の状態をセットする(Prefab生成時に必ず呼ぶこと)
    /// </summary>
    /// <param name="_level"></param>
    /// <param name="_stageNumber"></param>
    public void SettingMyStatus(int _stageNumber)
    {
        stageNumber = _stageNumber;

        level = PlayerInformationManager.Instance.
            GetRankingModeListGenerator().GetGameLevel(stageNumber);
        int unlockCondition = PlayerInformationManager.Instance.
            GetRankingModeListGenerator().GetUnlockStageNumber(stageNumber);

        textStageNumber.text = (_stageNumber+1).ToString();
        imageBackground.color = PlayerInformationManager.Instance.GetRankingModeColor();
        imageLevelColorBackground.color = PlayerInformationManager.Instance.GetLevelColor(level);

        lockPlayFlg = ! PlayerInformationManager.Instance.GetAlreadyStageClearFlg(level, unlockCondition);

        imageLockBackground.gameObject.SetActive(lockPlayFlg);
        imageLock.gameObject.SetActive(lockPlayFlg);

    }

    /// <summary>
    /// 自身のボタンを押したとき
    /// </summary>
    public void PushMyButton()
    {
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes);

        ToRankingModeCheckPanel checkPanel =
            Instantiate(toGameSceneCheckPanelPrefab, MenuSceneManager.Instance.GetCanvasTransform())
            .GetComponent<ToRankingModeCheckPanel>();

        if (lockPlayFlg)
        {
            checkPanel.SettingForCannotPlay(level, stageNumber);
        }
        else
        {
            checkPanel.SettingForCanPlay(level, stageNumber);
        }
    }

}
