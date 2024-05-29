using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectStageButtonForStageClearMode : MonoBehaviour
{
    [SerializeField] private GameObject toStageClearModeCheckPanelPrefab;

    [SerializeField] private Image imageBackground;
    [SerializeField] private Image imageStarA;
    [SerializeField] private Image imageStarB;
    [SerializeField] private Image imageStarC;
    [SerializeField] private Image imageLockBackground;
    [SerializeField] private Image imageLock;
    [SerializeField] private TextMeshProUGUI textStageNumber;

    private PlayerInformationManager.GameLevel level;
    private int stageNumber;
    private bool lockPlayFlg;
    private float bestTime;

    /// <summary>
    /// 自身の状態をセットする(Prefab生成時に必ず呼ぶこと)
    /// </summary>
    /// <param name="_level"></param>
    /// <param name="_stageNumber"></param>
    public void SettingMyStatus(PlayerInformationManager.GameLevel _level,int _stageNumber)
    {
        level = _level;
        stageNumber = _stageNumber;
        textStageNumber.text = (_stageNumber+1).ToString();
        imageBackground.color = PlayerInformationManager.Instance.GetLevelColor(level);
        if (_level == PlayerInformationManager.GameLevel.Normal)
        {
            bestTime = PlayerInformationManager.Instance.stageClearStarFlgsForNormal[_stageNumber].bestTime;
            if (PlayerInformationManager.Instance.stageClearStarFlgsForNormal[_stageNumber].starAll)
            {
                imageStarA.sprite = PlayerInformationManager.Instance.GetStarSprite(2);
                imageStarB.sprite = PlayerInformationManager.Instance.GetStarSprite(2);
                imageStarC.sprite = PlayerInformationManager.Instance.GetStarSprite(2);
            }
            else
            {
                imageStarA.sprite = PlayerInformationManager.Instance.stageClearStarFlgsForNormal[_stageNumber].star1 ?
                    PlayerInformationManager.Instance.GetStarSprite(1) : PlayerInformationManager.Instance.GetStarSprite(0);
                imageStarB.sprite = PlayerInformationManager.Instance.stageClearStarFlgsForNormal[_stageNumber].star2 ?
                    PlayerInformationManager.Instance.GetStarSprite(1) : PlayerInformationManager.Instance.GetStarSprite(0);
                imageStarC.sprite = PlayerInformationManager.Instance.stageClearStarFlgsForNormal[_stageNumber].star3 ?
                    PlayerInformationManager.Instance.GetStarSprite(1) : PlayerInformationManager.Instance.GetStarSprite(0);
            }
        }
        else if (_level == PlayerInformationManager.GameLevel.Hard)
        {
            bestTime = PlayerInformationManager.Instance.stageClearStarFlgsForHard[_stageNumber].bestTime;
            if (PlayerInformationManager.Instance.stageClearStarFlgsForHard[_stageNumber].starAll)
            {
                imageStarA.sprite = PlayerInformationManager.Instance.GetStarSprite(2);
                imageStarB.sprite = PlayerInformationManager.Instance.GetStarSprite(2);
                imageStarC.sprite = PlayerInformationManager.Instance.GetStarSprite(2);
            }
            else
            {
                imageStarA.sprite = PlayerInformationManager.Instance.stageClearStarFlgsForHard[_stageNumber].star1 ?
                    PlayerInformationManager.Instance.GetStarSprite(1) : PlayerInformationManager.Instance.GetStarSprite(0);
                imageStarB.sprite = PlayerInformationManager.Instance.stageClearStarFlgsForHard[_stageNumber].star2 ?
                    PlayerInformationManager.Instance.GetStarSprite(1) : PlayerInformationManager.Instance.GetStarSprite(0);
                imageStarC.sprite = PlayerInformationManager.Instance.stageClearStarFlgsForHard[_stageNumber].star3 ?
                    PlayerInformationManager.Instance.GetStarSprite(1) : PlayerInformationManager.Instance.GetStarSprite(0);
            }
        }
        else if (_level == PlayerInformationManager.GameLevel.VeryHard)
        {
            bestTime = PlayerInformationManager.Instance.stageClearStarFlgsForVeryHard[_stageNumber].bestTime;
            if (PlayerInformationManager.Instance.stageClearStarFlgsForVeryHard[_stageNumber].starAll)
            {
                imageStarA.sprite = PlayerInformationManager.Instance.GetStarSprite(2);
                imageStarB.sprite = PlayerInformationManager.Instance.GetStarSprite(2);
                imageStarC.sprite = PlayerInformationManager.Instance.GetStarSprite(2);
            }
            else
            {
                imageStarA.sprite = PlayerInformationManager.Instance.stageClearStarFlgsForVeryHard[_stageNumber].star1 ?
                    PlayerInformationManager.Instance.GetStarSprite(1) : PlayerInformationManager.Instance.GetStarSprite(0);
                imageStarB.sprite = PlayerInformationManager.Instance.stageClearStarFlgsForVeryHard[_stageNumber].star2 ?
                    PlayerInformationManager.Instance.GetStarSprite(1) : PlayerInformationManager.Instance.GetStarSprite(0);
                imageStarC.sprite = PlayerInformationManager.Instance.stageClearStarFlgsForVeryHard[_stageNumber].star3 ?
                    PlayerInformationManager.Instance.GetStarSprite(1) : PlayerInformationManager.Instance.GetStarSprite(0);
            }
        }
        else
        {
            Debug.LogError($"SelectStageButton.SettingMyStatus : Level Error -> {_level}");
            imageBackground.color = Color.white;
        }

        int unlockCondition = PlayerInformationManager.Instance.
            GetStageClearModeListGenerator().GetUnlockCondition(level, stageNumber);
        int haveStar = PlayerInformationManager.Instance.GetHaveStarsForStageClearMode(level);
        lockPlayFlg = haveStar >= unlockCondition ? false : true;

        imageLockBackground.gameObject.SetActive(lockPlayFlg);
        imageLock.gameObject.SetActive(lockPlayFlg);

    }

    /// <summary>
    /// 自身のボタンを押したとき
    /// </summary>
    public void PushMyButton()
    {
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes);

        ToStageClearModeCheckPanel checkPanel = 
            Instantiate(toStageClearModeCheckPanelPrefab, MenuSceneManager.Instance.GetCanvasTransform())
            .GetComponent<ToStageClearModeCheckPanel>();

        if (lockPlayFlg)
        {
            checkPanel.SettingForCannotPlay(level, stageNumber
                , imageStarA.sprite, imageStarB.sprite, imageStarC.sprite);
        }
        else
        {
            checkPanel.SettingForCanPlay(level, stageNumber, bestTime
                , imageStarA.sprite, imageStarB.sprite, imageStarC.sprite);
        }
    }


}
