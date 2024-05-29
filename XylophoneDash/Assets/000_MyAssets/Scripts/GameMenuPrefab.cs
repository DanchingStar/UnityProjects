using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Cinemachine.DocumentationSortingAttribute;

public class GameMenuPrefab : MonoBehaviour
{
    [SerializeField] private Slider sliderBgm;
    [SerializeField] private Slider sliderSe;
    [SerializeField] private Slider sliderKeyboard;

    [SerializeField] private GameObject soundSettingPanel;
    [SerializeField] private GameObject stageInformationPanel;
    [SerializeField] private Image insideImage;

    [SerializeField] private TextMeshProUGUI sheetMusicTitleText;
    [SerializeField] private TextMeshProUGUI stageNumberText;
    [SerializeField] private Image imageStarA;
    [SerializeField] private Image imageStarB;
    [SerializeField] private Image imageStarC;
    [SerializeField] private Image imageLevelForRanking;
    [SerializeField] private TextMeshProUGUI textLevelForRanking;
    [SerializeField] private TextMeshProUGUI targetTimeStringText;
    [SerializeField] private TextMeshProUGUI targetTimeText;
    [SerializeField] private TextMeshProUGUI recordTimeStringText;
    [SerializeField] private TextMeshProUGUI recordTimeText;

    [SerializeField] private Button stageInformationButton;

    private PlayerInformationManager.PlayGameInformation info;
    private StageClearStarFlgs stageClearStarFlgs;
    private float rankingModeRecordScore;

    private bool disableFlg;

    private void Start()
    {
        InitGameInformation();

        disableFlg = false;

        InitSlider();
    }

    /// <summary>
    /// �X���C�_�[�̏����ݒ�
    /// </summary>
    private void InitSlider()
    {
        sliderBgm.onValueChanged.AddListener(OnValueChangedForBGM);
        sliderSe.onValueChanged.AddListener(OnValueChangedForSE);
        sliderKeyboard.onValueChanged.AddListener(OnValueChangedForKeyboard);
    }

    /// <summary>
    /// ������
    /// </summary>
    private void InitGameInformation()
    {
        info = new PlayerInformationManager.PlayGameInformation();

        if (PlayerInformationManager.Instance != null)
        {
            info = PlayerInformationManager.Instance.GetNextPlayGameInformation();
        }
        else // �e�X�g�p
        {
            info.mode = PlayerInformationManager.GameMode.StageClear;
            info.level = PlayerInformationManager.GameLevel.Normal;
            info.stageNumber = 0;
            info.sheetMusicName = "Debug Mode";
            info.targetTime = 120f;
            info.playerCharacterName = "";
            info.skipForExampleFlg = false;
        }

        soundSettingPanel.SetActive(false);
        stageInformationPanel.SetActive(false);
        stageInformationButton.interactable = info.mode == PlayerInformationManager.GameMode.FreeStyle ? false : true;
    }

    /// <summary>
    /// ���j���[�����{�^�����������Ƃ�
    /// </summary>
    public void PushCloseButton()
    {
        if (disableFlg) return;
        disableFlg = true;

        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.GameMenuClose);
        GameSceneManager.Instance.ChangePauseFlg(false);
        Destroy(gameObject);
    }

    /// <summary>
    /// ���g���C�{�^�����������Ƃ�
    /// </summary>
    public void PushRetryButton()
    {
        if (disableFlg) return;
        disableFlg = true;

        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.GameMenuSceneMove);
        GameSceneManager.Instance.ReceptionRetry();
    }

    /// <summary>
    /// ���j���[�V�[���ֈڍs����{�^�����������Ƃ�
    /// </summary>
    public void PushGoToMenuSceneButton()
    {
        if (disableFlg) return;
        disableFlg = true;

        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.GameMenuSceneMove);
        GameSceneManager.Instance.ReceptionGoToMenuScene();
    }

    /// <summary>
    /// ���ʐݒ�̃{�^�����������Ƃ�
    /// </summary>
    /// <param name="flg">true:�\������ , false ��\���ɂ���</param>
    public void PushSoundSetting(bool flg)
    {
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.GameMenuSelect);
        if (flg)
        {
            sliderBgm.value = SoundManager.Instance.GetVolumeForBGM();
            sliderSe.value = SoundManager.Instance.GetVolumeForSE();
            sliderKeyboard.value = SoundManager.Instance.GetVolumeForKeyboard();
        }
        else
        {
            SoundManager.Instance.SavePlayerPrefs();
        }

        soundSettingPanel.SetActive(flg);
    }

    /// <summary>
    /// �X�e�[�W���̃{�^�����������Ƃ�
    /// </summary>
    /// <param name="flg">true:�\������ , false ��\���ɂ���</param>
    public void PushStageInformation(bool flg)
    {
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.GameMenuSelect);

        if (info.mode == PlayerInformationManager.GameMode.FreeStyle)
        {
            stageInformationButton.interactable = false;
            return;
        }
        else if (info.mode == PlayerInformationManager.GameMode.StageClear)
        {
            if (flg)
            {
                imageLevelForRanking.gameObject.SetActive(false);
                textLevelForRanking.gameObject.SetActive(false);

                Color col = Color.white;

                if (PlayerInformationManager.Instance != null)
                {
                    col = PlayerInformationManager.Instance.GetLevelColor(info.level);

                    stageClearStarFlgs = PlayerInformationManager.Instance.DEFAULT_STAGE_CLEAR_STAR_FLGS;
                    if (info.level == PlayerInformationManager.GameLevel.Normal)
                    {
                        stageClearStarFlgs = PlayerInformationManager.Instance.stageClearStarFlgsForNormal[info.stageNumber];
                    }
                    else if (info.level == PlayerInformationManager.GameLevel.Hard)
                    {
                        stageClearStarFlgs = PlayerInformationManager.Instance.stageClearStarFlgsForHard[info.stageNumber];
                    }
                    else if (info.level == PlayerInformationManager.GameLevel.VeryHard)
                    {
                        stageClearStarFlgs = PlayerInformationManager.Instance.stageClearStarFlgsForVeryHard[info.stageNumber];
                    }

                    if (stageClearStarFlgs.starAll)
                    {
                        imageStarA.sprite = PlayerInformationManager.Instance.GetStarSprite(2);
                        imageStarB.sprite = PlayerInformationManager.Instance.GetStarSprite(2);
                        imageStarC.sprite = PlayerInformationManager.Instance.GetStarSprite(2);
                    }
                    else
                    {
                        imageStarA.sprite = stageClearStarFlgs.star1 ?
                            PlayerInformationManager.Instance.GetStarSprite(1) : PlayerInformationManager.Instance.GetStarSprite(0);
                        imageStarB.sprite = stageClearStarFlgs.star2 ?
                            PlayerInformationManager.Instance.GetStarSprite(1) : PlayerInformationManager.Instance.GetStarSprite(0);
                        imageStarC.sprite = stageClearStarFlgs.star3 ?
                            PlayerInformationManager.Instance.GetStarSprite(1) : PlayerInformationManager.Instance.GetStarSprite(0);
                    }
                }
                else // �e�X�g�p
                {
                    stageClearStarFlgs = new StageClearStarFlgs(false, false, false, false, 0f);
                }

                insideImage.color = col;
                sheetMusicTitleText.text = info.sheetMusicName;
                stageNumberText.text = (info.stageNumber + 1).ToString();
                targetTimeText.text = info.targetTime.ToString($"F1") + " s";
                targetTimeStringText.text = "�ڕW�^�C�� : ";
                recordTimeStringText.text = "�}�C�x�X�g�^�C�� : ";
                if (stageClearStarFlgs.bestTime > 0f)
                {
                    recordTimeText.text = stageClearStarFlgs.bestTime.ToString($"F1") + " s";
                }
                else
                {
                    recordTimeText.text = " - - -";
                }
            }
            else
            {

            }
        }
        else if (info.mode == PlayerInformationManager.GameMode.Ranking)
        {
            if (flg)
            {
                targetTimeStringText.gameObject.SetActive(false);
                targetTimeText.gameObject.SetActive(false);
                imageStarA.gameObject.SetActive(false);
                imageStarB.gameObject.SetActive(false);
                imageStarC.gameObject.SetActive(false);

                Color col = Color.white;
                Color colLevelForRanking = Color.white;

                if (PlayerInformationManager.Instance != null)
                {
                    col = PlayerInformationManager.Instance.GetRankingModeColor();
                    rankingModeRecordScore = PlayerInformationManager.Instance.rankingTimeRecord[info.stageNumber];
                    colLevelForRanking = PlayerInformationManager.Instance.GetLevelColor(info.level);
                    textLevelForRanking.color = colLevelForRanking;
                    textLevelForRanking.text = info.level.ToString();
                }
                else // �e�X�g�p
                {
                    rankingModeRecordScore = 0f;
                }

                insideImage.color = col;
                stageNumberText.text = (info.stageNumber + 1).ToString();
                sheetMusicTitleText.text = info.sheetMusicName;
                recordTimeStringText.text = "�}�C�x�X�g�X�R�A : ";
                if (rankingModeRecordScore > 0f)
                {
                    recordTimeText.text = rankingModeRecordScore.ToString($"F2");
                }
                else
                {
                    recordTimeText.text = " - - -";
                }
            }

        }

        stageInformationPanel.SetActive(flg);
    }

    /// <summary>
    /// �V�ѕ��{�^�����������Ƃ�
    /// </summary>
    public void PushHowToPlayButton()
    {
        GameSceneManager.Instance.InstantiateHowToPlayPrefab();
    }

    /// <summary>
    /// BGM�X���C�_�[�̒l���ς�����Ƃ�
    /// </summary>
    /// <param name="value">�ς�����l</param>
    private void OnValueChangedForBGM(float value)
    {
        SoundManager.Instance.ChangeVolumeForBGM(value);
    }

    /// <summary>
    /// SE�X���C�_�[�̒l���ς�����Ƃ�
    /// </summary>
    /// <param name="value">�ς�����l</param>
    private void OnValueChangedForSE(float value)
    {
        SoundManager.Instance.ChangeVolumeForSE(value);
    }

    /// <summary>
    /// Keyboard�X���C�_�[�̒l���ς�����Ƃ�
    /// </summary>
    /// <param name="value">�ς�����l</param>
    private void OnValueChangedForKeyboard(float value)
    {
        SoundManager.Instance.ChangeVolumeForKeyboard(value);
    }


}
