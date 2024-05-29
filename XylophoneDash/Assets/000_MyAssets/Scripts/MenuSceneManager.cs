using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject uiBasePanel;
    [SerializeField] private Transform canvasTransform;

    [SerializeField] private InformationCutInPanel informationCutInPanel;

    [SerializeField] private TextMeshProUGUI modeText;

    [SerializeField] private GameObject stageClearModePanel;
    [SerializeField] private GameObject rankingModePanel;
    [SerializeField] private GameObject freeModePanel;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject settingPanel;

    [SerializeField] private GameObject stageClearMode_SelectLevelImage;
    [SerializeField] private GameObject stageClearMode_SelectStageImage;
    [SerializeField] private Transform stageClearMode_SelectStageImage_ScrollContent;

    [SerializeField] private GameObject rankingMode_NoNameImage;
    [SerializeField] private GameObject rankingMode_SelectStageImage;
    [SerializeField] private Transform rankingMode_SelectStageImage_ScrollContent;

    [SerializeField] private GameObject shop_IndexImage;
    [SerializeField] private GameObject shop_ObtainSPCoinImage;
    [SerializeField] private GameObject shop_GachaImage;
    [SerializeField] private GameObject shop_AdvertisementImage;
    [SerializeField] private GameObject shop_SNS_Share_Image;

    [SerializeField] private TextMeshProUGUI shop_HaveSPCoinText;
    [SerializeField] private TextMeshProUGUI shop_ObtainText1;
    [SerializeField] private TextMeshProUGUI shop_ObtainText2;
    [SerializeField] private TextMeshProUGUI shop_ObtainText3;
    [SerializeField] private Button shop_ObtainButton;
    [SerializeField] private TextMeshProUGUI shop_SnsShareText;

    [SerializeField] private GameObject setting_IndexImage;
    [SerializeField] private GameObject setting_ProfileImage;
    [SerializeField] private GameObject setting_Profile_NameEditImage;
    [SerializeField] private GameObject setting_Profile_ItemSelectImage;
    [SerializeField] private GameObject setting_CustomCodeImage;
    [SerializeField] private GameObject setting_VolumeImage;
    [SerializeField] private GameObject setting_NotesLanguageImage;
    [SerializeField] private GameObject setting_CreditImage;
    [SerializeField] private GameObject setting_DataDeleteImage;
    [SerializeField] private GameObject setting_DataDeleteFinalImage;
    [SerializeField] private GameObject setting_ToTitleImage;

    [SerializeField] private TextMeshProUGUI setting_PlayerNameText;

    [SerializeField] private Image setting_ProfileItem_Image;
    [SerializeField] private Button setting_ChangeSelectButton;
    [SerializeField] private TextMeshProUGUI setting_ProfileItem_NameText;
    [SerializeField] private TextMeshProUGUI setting_ProfileItem_InformationText;
    [SerializeField] private Transform setting_ProfileItem_Content;

    [SerializeField] private TextMeshProUGUI setting_DisplayCustomCodeText1;
    [SerializeField] private TextMeshProUGUI setting_DisplayCustomCodeText2;
    [SerializeField] private TextMeshProUGUI setting_NotesLanguageStatus1;
    [SerializeField] private TextMeshProUGUI setting_NotesLanguageStatus2;
    [SerializeField] private TextMeshProUGUI setting_CreditText;
    [SerializeField] private TextMeshProUGUI setting_ToTitleText;
    [SerializeField] private TMP_InputField setting_NameEditInputField;
    [SerializeField] private TextMeshProUGUI setting_NameEditInformationText;

    [SerializeField] private Slider setting_SliderBgm;
    [SerializeField] private Slider setting_SliderSe;
    [SerializeField] private Slider setting_SliderKeyboard;

    [SerializeField] private IconPrefab setting_IconPrefabForSettingProfile;

    [SerializeField] private GameObject shop_GachaCautionPanelPrefab;
    [SerializeField] private GameObject shop_RewardShowPanelPrefab;
    [SerializeField] private GameObject shop_GachaConfirmPanelPrefab;
    [SerializeField] private GameObject shop_GachaResultPanelPrefab;

    [SerializeField] private GameObject setting_SelectStageButtonForStageClearModePrefab;
    [SerializeField] private GameObject setting_SelectStageButtonForRankingModePrefab;
    [SerializeField] private GameObject setting_ToFreeModeCheckPanelPrefab;
    [SerializeField] private GameObject setting_DeleteInformationPrefab;
    [SerializeField] private GameObject setting_SelectItemButtonPrefab;

    [SerializeField] private MainMenuBoard mainMenuBoard;
    [SerializeField] private AudioClip stageMusicClip;

    private SelectMode nowSelectMode;

    private CinemachineVirtualCamera vcamStart;
    private CinemachineVirtualCamera vcamMenu;
    private CinemachineVirtualCamera vcamUI;

    private const string NAME_OF_VCAM_START = "VcamStart";
    private const string NAME_OF_VCAM_MENU = "VcamFirstMenu";
    private const string NAME_OF_VCAM_UI = "VcamSelectUI";

    private float startTimer;
    private const float WAIT_START_TIME = 2.5f;

    private const int GET_REWARD_SP_COIN_NUM = 1;

    private Vector3 defaultPanelPosition;
    private float panelDistance = 1500f;
    private float panelMoveSpeed = 1.8f;

    private int isDisableTapButtonFlg = 0;

    private GameObject fadeManagerObject;
    private bool rebootFlg;
    private bool rewardFlg;
    private bool waitingGachaResultFlg;
    private bool profileChangedFlg;

    private float timerForMainMenuKeyObject;

    /// <summary> �V���b�v�̌��ʂ̃R���[�`��(Reward��SP�R�C���̒l�̍X�V�Ɏg��) </summary>
    private Coroutine forShopResultCoroutine;

    /// <summary> �A�C�e���ݒ�ɂāA���ݑI��ł�����e�̃��X�g�ԍ� </summary>
    private int setting_CurrentSelectListNumber;

    /// <summary> ���K���ݒ�ɂāA���ݑI��ł�����e </summary>
    private PlayerInformationManager.DisplayNotesKind currentNotesKind;
    /// <summary> ���ՃV�[���ݒ�ɂāA���ݑI��ł�����e </summary>
    private bool currentDisplayStickerFlg;

    /// <summary> ���ݕ\������Ă���RewardShowPanelPrefab </summary>
    private ShopResultPanelPrefab currentShopResultPanelPrefab;

    /// <summary> ���ݕ\������Ă���RewardShowPanelPrefab </summary>
    private GachaResultPanelPrefab currentGachaResultPanelPrefab;

    /// <summary> �K�`����NEW�e�L�X�g�̃J���[��_�ł��邽�߂̒l���v�Z���邽�߂̃^�C�}�[ </summary>
    private float shop_TimerForChangeColorLeapForGacha = 0;

    /// <summary> �K�`����NEW�e�L�X�g�̃J���[��_�ł��邽�߂̒l </summary>
    private float shop_ValueForChangeColorLeapForGacha = 0;


    /// <summary>
    /// ���j���[�̓��e����
    /// </summary>
    public enum SelectMode
    {
        None,

        Start,
        FirstMenu,

        StageClearMode,
        RankingMode,
        FreeMode,
        Shop,
        Setting,

        StageClearMode_Normal,
        StageClearMode_Hard,
        StageClearMode_Veryhard,

        Shop_ObtainSPCoin,
        Shop_Gacha,
        Shop_Advertisement,
        Shop_SnsShare,

        Setting_Profile,
        Setting_Profile_NameEdit,
        Setting_Profile_Character,
        Setting_Profile_IconBackground,
        Setting_Profile_IconFrame,
        Setting_CustomCode,
        Setting_Volume,
        Setting_NotesLanguage,
        Setting_Credit,
        Setting_DataDelete,
        Setting_DataDeleteFinal,
        Setting_ToTitle,
    }

    public static MenuSceneManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitVariable();
        InitSlider();
        InitLogIn();
        InitVcam();
        InitStageMusic();

        if (PlayerInformationManager.Instance.GetSaveDataReadyFlg())
        {
            PlayerInformationManager.Instance.UpdateHaveStarsForStageClearMode();
        }
        else
        {

        }
    }

    private void Update()
    {
        timerForMainMenuKeyObject += Time.deltaTime;

        UpdateModeStartToFirst();
        UpdateColorLerpValueForGachaNewText();
    }

    /// <summary>
    /// �ϐ��̏�����
    /// </summary>
    private void InitVariable()
    {
        ChangeSelectMode(SelectMode.None);
        startTimer = 0;

        timerForMainMenuKeyObject = 0;

        defaultPanelPosition = stageClearModePanel.GetComponent<Transform>().position;

        fadeManagerObject = FadeManager.Instance.gameObject;
        rebootFlg = false;

        mainMenuBoard.ChangeImage(0);
    }

    /// <summary>
    /// �͂��߂̃��O�C������
    /// </summary>
    private void InitLogIn()
    {
        if (PlayFabManager.Instance.GetIsLogined()) //���O�C���󋵂��m�F
        {
            Debug.Log("MenuSceneManager.InitLogIn : Already Login");
        }
        else
        {
            PlayFabManager.Instance.Login(); //���O�C������
        }
    }

    /// <summary>
    /// �o�[�`�����J�����̏�����
    /// </summary>
    private void InitVcam()
    {
        //cinemachineBrain = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CinemachineBrain>();
        vcamStart = transform.Find(NAME_OF_VCAM_START).gameObject.GetComponent<CinemachineVirtualCamera>();
        vcamMenu = transform.Find(NAME_OF_VCAM_MENU).gameObject.GetComponent<CinemachineVirtualCamera>();
        vcamUI = transform.Find(NAME_OF_VCAM_UI).gameObject.GetComponent<CinemachineVirtualCamera>();

        vcamStart.Priority = 20;
        vcamMenu.Priority = 10;
        vcamUI.Priority = 5;
    }

    /// <summary>
    /// ���j���[�V�[����BGM��ݒ肵�ė���
    /// </summary>
    private void InitStageMusic()
    {
        if (stageMusicClip == null) return;

        SoundManager.Instance.SetStageMusic(stageMusicClip);
        SoundManager.Instance.PlayStageMusic();
    }

    /// <summary>
    /// �J�����ʒu���X�^�[�g���烁�j���[�ֈړ����邽�߂̃R���[�`��
    /// </summary>
    /// <returns></returns>
    private IEnumerator SwitchCameraStart()
    {
        yield return new WaitForSeconds(0.5f); // �w�肵���x�����Ԃ����҂�

        vcamStart.Priority = 0;
        vcamUI.Priority = 5;
    }

    /// <summary>
    /// ���O�C���������`�F�b�N���čX�V����
    /// </summary>
    /// <returns></returns>
    private IEnumerator UpdateLoginDays()
    {
        PlayerInformationManager.Instance.UpdateLoginDays();
        yield return null;
    }

    /// <summary>
    /// ���[�hStart����FirstMenu�Ɉڍs����܂ł̏���
    /// </summary>
    private void UpdateModeStartToFirst()
    {
        if (nowSelectMode == SelectMode.None)
        {
            ChangeSelectMode(SelectMode.Start);
        }
        else if (nowSelectMode == SelectMode.Start)
        {
            startTimer += Time.deltaTime;
            if (startTimer >= WAIT_START_TIME)
            {
                ChangeSelectMode(SelectMode.FirstMenu);
            }
        }
    }

    /// <summary>
    /// �����ԃo�b�N�O���E���h�ɂ������Ƃ�PlayFabManager����󂯎��
    /// </summary>
    public void ReceptionBackgroundLongerFromPlayFab()
    {
        InitLogIn();
    }

    /// <summary>
    /// ���O�C�����`�F�b�N������Ɏ��s
    /// </summary>
    /// <param name="flg">���O�C���̐���or���s</param>
    public void AfterCheckPlayFabLogin(bool flg)
    {
        if (flg) // ���O�C��������
        {
            SendInformationText("���O�C���ɐ������܂���");
            PlayerInformationManager.Instance.InitAndSettingPlayerInformation();
        }
        else // ���O�C�����s��
        {
            SendInformationText("���O�C���Ɏ��s���܂���");
        }

    }

    /// <summary>
    /// PlayFabManager����Z�[�u�̍X�V������/���s�������Ƃ���M�����Ƃ�
    /// </summary>
    /// <param name="isSuccess">true : ���� , false : ���s</param>
    public void ReceptionUpdateSaveDataPlayFab(bool isSuccess)
    {
        if (isSuccess)
        {
            if (rebootFlg)
            {
                setting_ToTitleText.text = "�f�[�^���T�[�o�ɕۑ����܂����B\n�^�C�g���֖߂�܂��B";
                StartCoroutine(RestartApplication());
            }
            else
            {
                SendInformationText("�T�[�o�Ƀf�[�^��\n�ۑ����܂���");
            }
        }
        else
        {
            if (rebootFlg)
            {
                rebootFlg = false;
                setting_ToTitleText.text = "�f�[�^���T�[�o�ɕۑ��ł��܂���ł����B\n�^�C�g���֖߂�̂����d���܂��B";
            }
            else
            {
                SendInformationText("�T�[�o�Ƀf�[�^��\n�ۑ��ł��܂���ł���");
            }
        }
    }

    /// <summary>
    /// �J�b�g�C���ŕ\���������e�L�X�g�𑗂�
    /// </summary>
    /// <param name="text"></param>
    public void SendInformationText(string text)
    {
        informationCutInPanel.AddInformationText(text);
    }

    /// <summary>
    /// ���݂̑I�����[�h��ύX����
    /// </summary>
    /// <param name="mode"></param>
    private void ChangeSelectMode(SelectMode mode)
    {
        ChangesDisableTapButtonFlg(true);

        if (mode == SelectMode.None)
        {
            nowSelectMode = mode;
        }
        else
        {
            SelectMode beforeMode = nowSelectMode;
            nowSelectMode = mode;

            if (mode == SelectMode.Start)
            {
                if (beforeMode == SelectMode.None)
                {
                    DisplayMenuPanel(false);
                    StartCoroutine(SwitchCameraStart());
                    StartCoroutine(UpdateLoginDays());
                }
                else
                {
                    Debug.LogError($"MenuSceneManager.ChangeSelectMode : Cannot Change Mode {nowSelectMode} To Start !");
                }
            }
            else if (mode == SelectMode.FirstMenu)
            {
                if (beforeMode == SelectMode.Start)
                {

                }
                else
                {
                    vcamUI.Priority = 5;
                    DisplayMenuPanel(false);
                }
            }
            else
            {
                vcamUI.Priority = 15;

                if (mode == SelectMode.StageClearMode)
                {
                    if (beforeMode == SelectMode.FirstMenu)
                    {
                        stageClearMode_SelectLevelImage.SetActive(true);
                        stageClearMode_SelectStageImage.SetActive(false);
                        stageClearModePanel.SetActive(true);
                    }
                    else
                    {

                    }
                }
                else if (mode == SelectMode.RankingMode)
                {
                    if (beforeMode == SelectMode.FirstMenu)
                    {
                        if (PlayerInformationManager.Instance.GetPlayerName() == "") 
                        {
                            rankingMode_SelectStageImage.SetActive(false);
                            rankingMode_NoNameImage.SetActive(true);
                        }
                        else
                        {
                            AllDeleteForRankingModeScrollContent();
                            InstantiateRankingStageSelectPrefabs();
                            rankingMode_SelectStageImage.SetActive(true);
                            rankingMode_NoNameImage.SetActive(false);
                        }

                        rankingModePanel.SetActive(true);
                    }
                    else
                    {

                    }
                }
                else if (mode == SelectMode.FreeMode)
                {
                    if (beforeMode == SelectMode.FirstMenu)
                    {
                        freeModePanel.SetActive(true);
                    }
                    else
                    {

                    }
                }
                else if (mode == SelectMode.Shop)
                {
                    if (beforeMode == SelectMode.FirstMenu)
                    {
                        shop_GachaImage.SetActive(false);
                        shop_ObtainSPCoinImage.SetActive(false);
                        shop_AdvertisementImage.SetActive(false);
                        shop_SNS_Share_Image.SetActive(false);

                        shop_IndexImage.SetActive(true);
                        shopPanel.SetActive(true);
                    }
                    else
                    {

                    }
                }
                else if (mode == SelectMode.Setting)
                {
                    if (beforeMode == SelectMode.FirstMenu)
                    {
                        setting_ProfileImage.SetActive(false);
                        setting_Profile_NameEditImage.SetActive(false);
                        setting_Profile_ItemSelectImage.SetActive(false);
                        setting_CustomCodeImage.SetActive(false);
                        setting_VolumeImage.SetActive(false);
                        setting_NotesLanguageImage.SetActive(false);
                        setting_CreditImage.SetActive(false);
                        setting_DataDeleteImage.SetActive(false);
                        setting_DataDeleteFinalImage.SetActive(false);
                        setting_ToTitleImage.SetActive(false);

                        setting_IndexImage.SetActive(true);
                        settingPanel.SetActive(true);
                    }
                    else
                    {

                    }
                }

                if (mode == SelectMode.StageClearMode_Normal)
                {
                    AllDeleteForStageClearModeScrollContent();
                    InstantiateStageSelectPrefabs(PlayerInformationManager.GameLevel.Normal);
                }
                else if (mode == SelectMode.StageClearMode_Hard)
                {
                    AllDeleteForStageClearModeScrollContent();
                    InstantiateStageSelectPrefabs(PlayerInformationManager.GameLevel.Hard);
                }
                else if (mode == SelectMode.StageClearMode_Veryhard)
                {
                    AllDeleteForStageClearModeScrollContent();
                    InstantiateStageSelectPrefabs(PlayerInformationManager.GameLevel.VeryHard);
                }

                else if (mode == SelectMode.Shop_Gacha)
                {
                    UpdateGachaTicketText();
                    LoadStoreGacha();
                }
                else if (mode == SelectMode.Shop_SnsShare)
                {
                    SelectShareText();
                }
                else if (mode == SelectMode.Shop_ObtainSPCoin)
                {
                    UpdateObtainContents();
                }

                else if (mode == SelectMode.Setting_Profile)
                {
                    UpdateIconPrefabForSettingProfile();
                    UpdatePlayerNameText();
                    if (beforeMode == SelectMode.FirstMenu)
                    {
                        profileChangedFlg = false;
                    }
                }
                else if (mode == SelectMode.Setting_Profile_NameEdit)
                {
                    setting_NameEditInformationText.text = "";
                    setting_NameEditInputField.text = "";
                }
                else if (mode == SelectMode.Setting_Profile_Character)
                {
                    AllDeleteForSettingItemSelectScrollContent();
                    UpdateProfileContents();
                    ChangeActiveProfileItemSelectButton(false);
                }
                else if (mode == SelectMode.Setting_Profile_IconBackground)
                {
                    AllDeleteForSettingItemSelectScrollContent();
                    UpdateProfileContents();
                    ChangeActiveProfileItemSelectButton(false);
                }
                else if (mode == SelectMode.Setting_Profile_IconFrame)
                {
                    AllDeleteForSettingItemSelectScrollContent();
                    UpdateProfileContents();
                    ChangeActiveProfileItemSelectButton(false);
                }
                else if (mode == SelectMode.Setting_CustomCode)
                {
                    InitDisplayCustomCodeTexts();
                }
                else if (mode == SelectMode.Setting_NotesLanguage)
                {
                    SetNotesLanguageContents();
                }
                else if (mode == SelectMode.Setting_Credit)
                {
                    SetCreditText();
                }
                else if (mode == SelectMode.Setting_ToTitle)
                {
                    InitToTitleTexts();
                }




                else
                {
                    //Debug.Log($"MenuSceneManager.ChangeSelectMode : Else");
                }

                DisplayMenuPanel(true);
            }
        }

        // ���[�h�ύX�̊m�F�e�X�g
        //SendInformationText($" Mode \n{nowSelectMode}");
        //Debug.Log($"MenuSceneManager.ChangeSelectMode : Mode -> {nowSelectMode}");

        ChangesDisableTapButtonFlg(false);
    }

    /// <summary>
    /// Panel�̕\�������ւ��鉉�o������
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    private IEnumerator ChangeDisplayContents(SelectMode mode)
    {
        ChangesDisableTapButtonFlg(true);

        GameObject currentPanel = null;
        GameObject previousPanel = null;

        switch (mode)
        {
            case SelectMode.StageClearMode_Normal:
                currentPanel = stageClearMode_SelectStageImage;
                previousPanel = stageClearMode_SelectLevelImage;
                ChangeSelectMode(SelectMode.StageClearMode_Normal);
                break;
            case SelectMode.StageClearMode_Hard:
                currentPanel = stageClearMode_SelectStageImage;
                previousPanel = stageClearMode_SelectLevelImage;
                ChangeSelectMode(SelectMode.StageClearMode_Hard);
                break;
            case SelectMode.StageClearMode_Veryhard:
                currentPanel = stageClearMode_SelectStageImage;
                previousPanel = stageClearMode_SelectLevelImage;
                ChangeSelectMode(SelectMode.StageClearMode_Veryhard);
                break;

            case SelectMode.Shop_Gacha:
                currentPanel = shop_GachaImage;
                previousPanel = shop_IndexImage;
                ChangeSelectMode(SelectMode.Shop_Gacha);
                break;
            case SelectMode.Shop_ObtainSPCoin:
                currentPanel = shop_ObtainSPCoinImage;
                previousPanel = shop_IndexImage;
                ChangeSelectMode(SelectMode.Shop_ObtainSPCoin);
                break;
            case SelectMode.Shop_Advertisement:
                currentPanel = shop_AdvertisementImage;
                previousPanel = shop_IndexImage;
                ChangeSelectMode(SelectMode.Shop_Advertisement);
                break;
            case SelectMode.Shop_SnsShare:
                currentPanel = shop_SNS_Share_Image;
                previousPanel = shop_IndexImage;
                ChangeSelectMode(SelectMode.Shop_SnsShare);
                break;

            case SelectMode.Setting_Profile:
                currentPanel = setting_ProfileImage;
                previousPanel = setting_IndexImage;
                ChangeSelectMode(SelectMode.Setting_Profile);
                break;
            case SelectMode.Setting_Profile_NameEdit:
                currentPanel = setting_Profile_NameEditImage;
                previousPanel = setting_ProfileImage;
                ChangeSelectMode(SelectMode.Setting_Profile_NameEdit);
                break;
            case SelectMode.Setting_Profile_Character:
                currentPanel = setting_Profile_ItemSelectImage;
                //currentPanel = settingMode_Profile_CharacterImage;
                previousPanel = setting_ProfileImage;
                ChangeSelectMode(SelectMode.Setting_Profile_Character);
                break;
            case SelectMode.Setting_Profile_IconBackground:
                currentPanel = setting_Profile_ItemSelectImage;
                //currentPanel = settingMode_Profile_IconBackgroundImage;
                previousPanel = setting_ProfileImage;
                ChangeSelectMode(SelectMode.Setting_Profile_IconBackground);
                break;
            case SelectMode.Setting_Profile_IconFrame:
                currentPanel = setting_Profile_ItemSelectImage;
                //currentPanel = settingMode_Profile_IconFrameImage;
                previousPanel = setting_ProfileImage;
                ChangeSelectMode(SelectMode.Setting_Profile_IconFrame);
                break;
            case SelectMode.Setting_CustomCode:
                currentPanel = setting_CustomCodeImage;
                previousPanel = setting_IndexImage;
                ChangeSelectMode(SelectMode.Setting_CustomCode);
                break;
            case SelectMode.Setting_Volume:
                currentPanel = setting_VolumeImage;
                previousPanel = setting_IndexImage;
                ChangeSelectMode(SelectMode.Setting_Volume);
                break;
            case SelectMode.Setting_NotesLanguage:
                currentPanel = setting_NotesLanguageImage;
                previousPanel = setting_IndexImage;
                ChangeSelectMode(SelectMode.Setting_NotesLanguage);
                break;
            case SelectMode.Setting_Credit:
                currentPanel = setting_CreditImage;
                previousPanel = setting_IndexImage;
                ChangeSelectMode(SelectMode.Setting_Credit);
                break;
            case SelectMode.Setting_DataDelete:
                currentPanel = setting_DataDeleteImage;
                previousPanel = setting_IndexImage;
                ChangeSelectMode(SelectMode.Setting_DataDelete);
                break;
            case SelectMode.Setting_DataDeleteFinal:
                currentPanel = setting_DataDeleteFinalImage;
                previousPanel = setting_DataDeleteImage;
                ChangeSelectMode(SelectMode.Setting_DataDeleteFinal);
                break;
            case SelectMode.Setting_ToTitle:
                currentPanel = setting_ToTitleImage;
                previousPanel = setting_IndexImage;
                ChangeSelectMode(SelectMode.Setting_ToTitle);
                break;




            default:
                Debug.Log($"MenuSceneManager.ChangeDisplayContents : Switch default ->  {mode}");
                break;
        }

        Transform currentPanelTransform = currentPanel.GetComponent<Transform>();
        Transform previousPanelTransform = previousPanel.GetComponent<Transform>();

        Vector3 currentPanelBeforePosition = defaultPanelPosition + new Vector3(panelDistance, 0, 0);
        Vector3 currentPanelAfterPosition = defaultPanelPosition;
        Vector3 topMenutPanelBeforePosition = defaultPanelPosition;
        Vector3 topMenuPanelAfterPosition = defaultPanelPosition - new Vector3(panelDistance, 0, 0);

        currentPanelTransform.position = currentPanelBeforePosition;
        currentPanel.SetActive(true);

        float presentLocation = 0f;
        while (presentLocation < 1f)
        {
            presentLocation += Time.deltaTime * panelMoveSpeed;
            float curveValue = Mathf.Sin(presentLocation * Mathf.PI * 0.5f);

            currentPanelTransform.position = Vector3.Lerp(currentPanelBeforePosition, currentPanelAfterPosition, curveValue);
            previousPanelTransform.position = Vector3.Lerp(topMenutPanelBeforePosition, topMenuPanelAfterPosition, curveValue);

            yield return null;
        }

        previousPanel.SetActive(false);

        //UpdateDisplayBackButton();
        UpdateMenuContentsText();

        ChangesDisableTapButtonFlg(false);
    }

    /// <summary>
    /// ���j���[�̃{�^�����������Ƃ�
    /// </summary>
    /// <param name="toContent"></param>
    public void OnTapMenuButton(int toContent)
    {
        if (GetDisableTapButtonFlg()) return;

        ChangesDisableTapButtonFlg(true);

        SelectMode content = (SelectMode)toContent;

        if (content == SelectMode.FirstMenu)
        {
            // BackButton.gameObject.SetActive(false);
        }
        else
        {
            if (content == SelectMode.None)
            {
                Debug.LogError($"MenuSceneManager.OnTapMenuButton : content is None");
            }
            else
            {
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes);
                StartCoroutine(ChangeDisplayContents(content));
            }
        }

        ChangesDisableTapButtonFlg(false);
    }

    /// <summary>
    /// MenuContentsText(�߂�{�^���̉��̕�����)���X�V����
    /// </summary>
    private void UpdateMenuContentsText()
    {
        switch (nowSelectMode)
        {
            case SelectMode.StageClearMode:
                modeText.text = "�X�e�[�W�N���A���[�h";
                break;
            case SelectMode.RankingMode:
                modeText.text = "�����L���O���[�h";
                break;
            case SelectMode.FreeMode:
                modeText.text = "�t���[���[�h";
                break;
            case SelectMode.Shop:
                modeText.text = "�V���b�v";
                break;
            case SelectMode.Setting:
                modeText.text = "�ݒ�";
                break;

            case SelectMode.StageClearMode_Normal:
                modeText.text = "�X�e�[�W�N���A���[�h(Normal)";
                break;
            case SelectMode.StageClearMode_Hard:
                modeText.text = "�X�e�[�W�N���A���[�h(Hard)";
                break;
            case SelectMode.StageClearMode_Veryhard:
                modeText.text = "�X�e�[�W�N���A���[�h(Very Hard)";
                break;

            case SelectMode.Shop_Gacha:
                modeText.text = "�K�`��";
                break;
            case SelectMode.Shop_ObtainSPCoin:
                modeText.text = "SP�R�C�������炤";
                break;
            case SelectMode.Shop_Advertisement:
                modeText.text = "�L��������";
                break;
            case SelectMode.Shop_SnsShare:
                modeText.text = "SNS�V�F�A";
                break;

            case SelectMode.Setting_Profile:
                modeText.text = "�v���t�B�[��";
                break;
            case SelectMode.Setting_Profile_NameEdit:
                modeText.text = "���O �ύX";
                break;
            case SelectMode.Setting_Profile_Character:
                modeText.text = "�L�����N�^�[ �ύX";
                break;
            case SelectMode.Setting_Profile_IconBackground:
                modeText.text = "�A�C�R���w�i �ύX";
                break;
            case SelectMode.Setting_Profile_IconFrame:
                modeText.text = "�A�C�R���g �ύX";
                break;
            case SelectMode.Setting_CustomCode:
                modeText.text = "�����R�[�h";
                break;
            case SelectMode.Setting_Volume:
                modeText.text = "����";
                break;
            case SelectMode.Setting_NotesLanguage:
                modeText.text = "���K���̕\��";
                break;
            case SelectMode.Setting_Credit:
                modeText.text = "�N���W�b�g";
                break;
            case SelectMode.Setting_DataDelete:
                modeText.text = "�f�[�^�폜";
                break;
            case SelectMode.Setting_DataDeleteFinal:
                modeText.text = "�f�[�^�폜(�ŏI�m�F)";
                break;
            case SelectMode.Setting_ToTitle:
                modeText.text = "�^�C�g���֖߂�";
                break;

            default:
                Debug.LogError($"MenuSceneManager.UpdateMenuContentsText : currentContents is None -> {nowSelectMode}");
                break;
        }
    }

    /// <summary>
    /// �߂�{�^�����������Ƃ�
    /// </summary>
    public void PushBackButton()
    {
        if (GetDisableTapButtonFlg()) return;

        ChangesDisableTapButtonFlg(true);

        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.No);

        if (nowSelectMode == SelectMode.FirstMenu || nowSelectMode == SelectMode.Start)
        {
            Debug.LogError($"MenuSceneManager.PushBackButton : Error Now Mode -> {nowSelectMode}");
        }
        else if (nowSelectMode == SelectMode.StageClearMode)
        {
            ChangeSelectMode(SelectMode.FirstMenu);
        }
        else if (nowSelectMode == SelectMode.RankingMode)
        {
            ChangeSelectMode(SelectMode.FirstMenu);
        }
        else if (nowSelectMode == SelectMode.FreeMode)
        {
            ChangeSelectMode(SelectMode.FirstMenu);
        }
        else if (nowSelectMode == SelectMode.Shop)
        {
            ChangeSelectMode(SelectMode.FirstMenu);
        }
        else if (nowSelectMode == SelectMode.Setting)
        {
            ChangeSelectMode(SelectMode.FirstMenu);
        }
        else if (nowSelectMode == SelectMode.Setting_Profile)
        {
            SaveProfile();
            StartCoroutine(ChangeDisplayContentsBack());
        }
        else if (nowSelectMode == SelectMode.Setting_Volume)
        {
            SaveSoundSetting();
            StartCoroutine(ChangeDisplayContentsBack());
        }
        else if (nowSelectMode == SelectMode.Setting_NotesLanguage)
        {
            SaveNotesLanguageSetting();
            StartCoroutine(ChangeDisplayContentsBack());
        }
        else
        {
            StartCoroutine(ChangeDisplayContentsBack());
        }

        ChangesDisableTapButtonFlg(false);
    }

    /// <summary>
    /// Panel�̕\����O��Panel�ɖ߂鉉�o������
    /// </summary>
    /// <returns></returns>
    private IEnumerator ChangeDisplayContentsBack()
    {
        ChangesDisableTapButtonFlg(true);

        GameObject currentPanel = null;
        GameObject previousPanel = null;

        switch (nowSelectMode)
        {
            case SelectMode.StageClearMode_Normal:
                currentPanel = stageClearMode_SelectStageImage;
                previousPanel = stageClearMode_SelectLevelImage;
                ChangeSelectMode(SelectMode.StageClearMode);
                break;
            case SelectMode.StageClearMode_Hard:
                currentPanel = stageClearMode_SelectStageImage;
                previousPanel = stageClearMode_SelectLevelImage;
                ChangeSelectMode(SelectMode.StageClearMode);
                break;
            case SelectMode.StageClearMode_Veryhard:
                currentPanel = stageClearMode_SelectStageImage;
                previousPanel = stageClearMode_SelectLevelImage;
                ChangeSelectMode(SelectMode.StageClearMode);
                break;

            case SelectMode.Shop_Gacha:
                currentPanel = shop_GachaImage;
                previousPanel = shop_IndexImage;
                ChangeSelectMode(SelectMode.Shop);
                break;
            case SelectMode.Shop_ObtainSPCoin:
                currentPanel = shop_ObtainSPCoinImage;
                previousPanel = shop_IndexImage;
                ChangeSelectMode(SelectMode.Shop);
                break;
            case SelectMode.Shop_Advertisement:
                currentPanel = shop_AdvertisementImage;
                previousPanel = shop_IndexImage;
                ChangeSelectMode(SelectMode.Shop);
                break;
            case SelectMode.Shop_SnsShare:
                currentPanel = shop_SNS_Share_Image;
                previousPanel = shop_IndexImage;
                ChangeSelectMode(SelectMode.Shop);
                break;

            case SelectMode.Setting_Profile:
                currentPanel = setting_ProfileImage;
                previousPanel = setting_IndexImage;
                ChangeSelectMode(SelectMode.Setting);
                break;
            case SelectMode.Setting_Profile_NameEdit:
                currentPanel = setting_Profile_NameEditImage;
                previousPanel = setting_ProfileImage;
                ChangeSelectMode(SelectMode.Setting_Profile);
                break;
            case SelectMode.Setting_Profile_Character:
                currentPanel = setting_Profile_ItemSelectImage;
                previousPanel = setting_ProfileImage;
                ChangeSelectMode(SelectMode.Setting_Profile);
                break;
            case SelectMode.Setting_Profile_IconBackground:
                currentPanel = setting_Profile_ItemSelectImage;
                previousPanel = setting_ProfileImage;
                ChangeSelectMode(SelectMode.Setting_Profile);
                break;
            case SelectMode.Setting_Profile_IconFrame:
                currentPanel = setting_Profile_ItemSelectImage;
                previousPanel = setting_ProfileImage;
                ChangeSelectMode(SelectMode.Setting_Profile);
                break;
            case SelectMode.Setting_CustomCode:
                currentPanel = setting_CustomCodeImage;
                previousPanel = setting_IndexImage;
                ChangeSelectMode(SelectMode.Setting);
                break;
            case SelectMode.Setting_Volume:
                currentPanel = setting_VolumeImage;
                previousPanel = setting_IndexImage;
                ChangeSelectMode(SelectMode.Setting);
                break;
            case SelectMode.Setting_NotesLanguage:
                currentPanel = setting_NotesLanguageImage;
                previousPanel = setting_IndexImage;
                ChangeSelectMode(SelectMode.Setting);
                break;
            case SelectMode.Setting_Credit:
                currentPanel = setting_CreditImage;
                previousPanel = setting_IndexImage;
                ChangeSelectMode(SelectMode.Setting);
                break;
            case SelectMode.Setting_DataDelete:
                currentPanel = setting_DataDeleteImage;
                previousPanel = setting_IndexImage;
                ChangeSelectMode(SelectMode.Setting);
                break;
            case SelectMode.Setting_DataDeleteFinal:
                currentPanel = setting_DataDeleteFinalImage;
                previousPanel = setting_IndexImage;
                ChangeSelectMode(SelectMode.Setting);
                break;
            case SelectMode.Setting_ToTitle:
                currentPanel = setting_ToTitleImage;
                previousPanel = setting_IndexImage;
                ChangeSelectMode(SelectMode.Setting);
                break;

            default:
                Debug.Log($"MenuSceneManager.ChangeDisplayContentsBack : Switch default ->  {nowSelectMode}");
                break;
        }

        //if (currentContents == MenuContents.TopMenu)
        //{
        //    UpdateHaveMedalsAndSPCoinsAndPlayerInformation();
        //}

        Transform currentPanelTransform = currentPanel.GetComponent<Transform>();
        Transform previousPanelTransform = previousPanel.GetComponent<Transform>();

        Vector3 currentPanelBeforePosition = defaultPanelPosition;
        Vector3 currentPanelAfterPosition = defaultPanelPosition + new Vector3(panelDistance, 0, 0);
        Vector3 topMenutPanelBeforePosition = defaultPanelPosition - new Vector3(panelDistance, 0, 0);
        Vector3 topMenuPanelAfterPosition = defaultPanelPosition;

        currentPanelTransform.position = currentPanelBeforePosition;
        previousPanel.SetActive(true);

        float presentLocation = 0f;
        while (presentLocation < 1f)
        {
            presentLocation += Time.deltaTime * panelMoveSpeed;
            float curveValue = Mathf.Sin(presentLocation * Mathf.PI * 0.5f);

            currentPanelTransform.position = Vector3.Lerp(currentPanelBeforePosition, currentPanelAfterPosition, curveValue);
            previousPanelTransform.position = Vector3.Lerp(topMenutPanelBeforePosition, topMenuPanelAfterPosition, curveValue);

            yield return null;
        }

        currentPanel.SetActive(false);

        //UpdateDisplayBackButton();
        UpdateMenuContentsText();

        ChangesDisableTapButtonFlg(false);
    }

    /// <summary>
    /// �S�Ẵ��j���[�̃p�l�������
    /// </summary>
    private void CloseAllMenuPanel()
    {
        stageClearModePanel.SetActive(false);
        rankingModePanel.SetActive(false);
        freeModePanel.SetActive(false);
        shopPanel.SetActive(false);
        settingPanel.SetActive(false);

        //Debug.Log($"MenuSceneManager.CloseAllMenuPanel");
    }

    /// <summary>
    /// ���j���p�l���̕\����\����ύX����
    /// </summary>
    /// <param name="flg"></param>
    private void DisplayMenuPanel(bool flg)
    {
        StartCoroutine(DisplayMenuPanelCoroutine(flg));
    }

    /// <summary>
    /// ���j���p�l���̕\����\���̃R���[�`��
    /// </summary>
    /// <param name="flg"></param>
    /// <returns></returns>
    private IEnumerator DisplayMenuPanelCoroutine(bool flg)
    {
        ChangesDisableTapButtonFlg(true);

        float time = flg ? 1.0f : 0.0f;
        yield return new WaitForSeconds(time);
        uiBasePanel.SetActive(flg);
        if (flg) UpdateMenuContentsText();

        ChangesDisableTapButtonFlg(false);
    }

    /// <summary>
    /// �؋Ղ̌��Ղ�@������
    /// </summary>
    /// <param name="mode"></param>
    public void TapXylophone(SelectMode mode)
    {
        if (nowSelectMode != SelectMode.FirstMenu) return;

        SoundManager.Instance.PlayXylophoneSE(mode);
        CloseAllMenuPanel();
        ChangeSelectMode(mode);
        mainMenuBoard.ChangeImage((int)mode - 2);
    }

    /// <summary>
    /// ���݂̃��[�h��Ԃ��Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public SelectMode GetNowSelectMode()
    {
        return nowSelectMode;
    }

    /// <summary>
    /// Canvas��Transform��Ԃ��Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public Transform GetCanvasTransform()
    {
        return canvasTransform;
    }

    /// <summary>
    /// ���C���̖؋ՃI�u�W�F�N�g�̕�����_�ł����邽�߂̃^�C�}�[��Ԃ��Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public float GetTimerForMainMenuKeyObject()
    {
        return timerForMainMenuKeyObject;
    }

    /// <summary>
    /// �{�^���̉����\���ǂ����̃t���O��ς���
    /// </summary>
    /// <param name="flg">true : �����s�\ , false : �����\</param>
    private void ChangesDisableTapButtonFlg(bool flg)
    {
        if (flg)
        {
            isDisableTapButtonFlg++;
        }
        else
        {
            isDisableTapButtonFlg--;
        }

        if (isDisableTapButtonFlg < 0)
        {
            isDisableTapButtonFlg = 0;
            Debug.Log($"ChangesDisableTapButtonFlg : flg is minus");
        }
    }

    /// <summary>
    /// �{�^���������s�\����Ԃ�
    /// </summary>
    /// <returns>true : �����s�\ , false : �����\</returns>
    private bool GetDisableTapButtonFlg()
    {
        return isDisableTapButtonFlg <= 0 ? false : true;
    }

#region GameMode

    /// <summary>
    /// �X�e�[�W�N���A���[�h�̒��̃X�e�[�W�Z���N�gPrefab�𐶐�����
    /// </summary>
    /// <param name="level"></param>
    private void InstantiateStageSelectPrefabs(PlayerInformationManager.GameLevel level)
    {
        int length = PlayerInformationManager.Instance.GetStageClearModeListGenerator().GetListLength(level);

        for (int i = 0; i < length; i++)
        {
            SelectStageButtonForStageClearMode selectStageButton =
                Instantiate(setting_SelectStageButtonForStageClearModePrefab, stageClearMode_SelectStageImage_ScrollContent)
                .GetComponent<SelectStageButtonForStageClearMode>();

            selectStageButton.SettingMyStatus(level, i);
        }

        stageClearMode_SelectStageImage.transform.Find("Scroll View").GetComponent<ScrollRect>().verticalNormalizedPosition = 1f;
    }

    /// <summary>
    /// �X�e�[�W�N���A���[�h�̃X�e�[�W�Z���N�g�̃X�N���[�����̎q�I�u�W�F�N�g�����ׂ�Destroy����
    /// </summary>
    private void AllDeleteForStageClearModeScrollContent()
    {
        //�����̎q����S�Ē��ׂ�
        foreach (Transform child in stageClearMode_SelectStageImage_ScrollContent)
        {
            //�����̎q����Destroy����
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// �����L���O���[�h�̒��̃X�e�[�W�Z���N�gPrefab�𐶐�����
    /// </summary>
    /// <param name="level"></param>
    private void InstantiateRankingStageSelectPrefabs()
    {
        int length = PlayerInformationManager.Instance.GetRankingModeListGenerator().GetLength();

        for (int i = 0; i < length; i++)
        {
            SelectStageButtonForRankingMode selectStageButton =
                Instantiate(setting_SelectStageButtonForRankingModePrefab, rankingMode_SelectStageImage_ScrollContent)
                .GetComponent<SelectStageButtonForRankingMode>();

            selectStageButton.SettingMyStatus(i);
        }

        rankingMode_SelectStageImage.transform.Find("Scroll View").GetComponent<ScrollRect>().verticalNormalizedPosition = 1f;
    }

    /// <summary>
    /// �����L���O���[�h�̃X�e�[�W�Z���N�g�̃X�N���[�����̎q�I�u�W�F�N�g�����ׂ�Destroy����
    /// </summary>
    private void AllDeleteForRankingModeScrollContent()
    {
        //�����̎q����S�Ē��ׂ�
        foreach (Transform child in rankingMode_SelectStageImage_ScrollContent)
        {
            //�����̎q����Destroy����
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// �t���[���[�h�̃��x���{�^����I�����ĉ������Ƃ�
    /// </summary>
    /// <param name="level"></param>
    public void PushFreeMode(int level)
    {
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes);

        ToFreeModeCheckPanelPrefab checkPanel =
            Instantiate(setting_ToFreeModeCheckPanelPrefab, GetCanvasTransform()).GetComponent<ToFreeModeCheckPanelPrefab>();

        checkPanel.SettingMyStatus((PlayerInformationManager.GameLevel)level);
    }

    /// <summary>
    /// �Q�[���V�[���ֈړ�����
    /// </summary>
    public void ToGameScene()
    {
        FadeManager.Instance.LoadScene("Game", 0.5f);
    }


#endregion

#region Setting

    /// <summary>
    /// �X���C�_�[�̏����ݒ�
    /// </summary>
    private void InitSlider()
    {
        setting_SliderBgm.onValueChanged.AddListener(OnValueChangedForBGM);
        setting_SliderSe.onValueChanged.AddListener(OnValueChangedForSE);
        setting_SliderKeyboard.onValueChanged.AddListener(OnValueChangedForKeyboard);

        setting_SliderBgm.value = SoundManager.Instance.GetVolumeForBGM();
        setting_SliderSe.value = SoundManager.Instance.GetVolumeForSE();
        setting_SliderKeyboard.value = SoundManager.Instance.GetVolumeForKeyboard();
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

    /// <summary>
    /// ���ʐݒ�̕ύX��ۑ�����
    /// </summary>
    public void SaveSoundSetting()
    {
        SoundManager.Instance.SavePlayerPrefs();
    }

    /// <summary>
    /// �v���t�B�[���摜�̐ݒ���������Ƃ�
    /// </summary>
    public void PushProfileEditButton()
    {
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes);
        //string str = "MakeSprite";
        //FadeManager.Instance.LoadScene(str, FADE_MANAGER_TIME);
    }

    /// <summary>
    /// �v���t�B�[���ݒ�̃p�l���̒��̃A�C�R���̓��e���X�V����
    /// </summary>
    public void UpdateIconPrefabForSettingProfile()
    {
        string character = PlayerInformationManager.Instance.GetPlayerCharacterName();
        string background = PlayerInformationManager.Instance.GetSelectIconBackgroundName();
        string frame = PlayerInformationManager.Instance.GetSelectIconFrameName();

        setting_IconPrefabForSettingProfile.UpdateMySprites(
            PlayerInformationManager.Instance.GetIconBackListGenerator().GetSprite(background),
            PlayerInformationManager.Instance.GetIconFrameListGenerator().GetSprite(frame),
            PlayerInformationManager.Instance.GetCharacterListGenerator().GetCharacterSprite(character)
            );

        //Debug.Log($"MenuSceneManager.UpdateIconPrefabForSettingProfile : Update Icon Prefab");
    }

    /// <summary>
    /// �v���t�B�[���ݒ�̃v���C���[���̃e�L�X�g���X�V����
    /// </summary>
    public void UpdatePlayerNameText()
    {
        string name = PlayerInformationManager.Instance.GetPlayerName();

        if (name == "")
        {
            setting_PlayerNameText.text = "�i���ݒ�j";
            setting_PlayerNameText.color = Color.red;
        }
        else
        {
            setting_PlayerNameText.text = name;
            setting_PlayerNameText.color = Color.black;
        }

    }

    /// <summary>
    /// �A�C�e���ύX��ʂ��X�V����
    /// </summary>
    private void UpdateProfileContents()
    {
        if (nowSelectMode == SelectMode.Setting_Profile_Character)
        {
            setting_CurrentSelectListNumber = PlayerInformationManager.Instance.GetPlayerCharacterNumber();

            setting_ProfileItem_Image.sprite = PlayerInformationManager.Instance.GetCharacterListGenerator()
                .GetCharacterSprite(setting_CurrentSelectListNumber);
            setting_ProfileItem_NameText.text = PlayerInformationManager.Instance.GetPlayerCharacterName();
            setting_ProfileItem_InformationText.text = 
                "�@�Q�[���Ŏg�p����L�����N�^�[��ύX�ł��܂��B\n" +
                "�@�L�����N�^�[���Ƃɂ�鐫�\���͂���܂���B\n" +
                "�@�������̃L�����N�^�[�́A [�V���b�v] �� [�K�`��] ����m���œ���ł��܂��B";

            int length = PlayerInformationManager.Instance.GetCharacterListGenerator().GetLength();
            for (int i = 0; i < length; i++)
            {
                if (PlayerInformationManager.Instance.haveItemForCharacters[i] >= 1)
                {
                    SelectItemButtonPrefab selectItemButton =
                        Instantiate(setting_SelectItemButtonPrefab, setting_ProfileItem_Content)
                        .GetComponent<SelectItemButtonPrefab>();

                    selectItemButton.SetCategory(SelectItemButtonPrefab.Category.Character);
                    selectItemButton.SetListNumber(i);
                    selectItemButton.SetSprite();
                }
            }
        }
        else if (nowSelectMode == SelectMode.Setting_Profile_IconBackground)
        {
            setting_CurrentSelectListNumber = PlayerInformationManager.Instance.GetSelectIconBackgroundNumber();

            setting_ProfileItem_Image.sprite = PlayerInformationManager.Instance.GetIconBackListGenerator()
                .GetSprite(setting_CurrentSelectListNumber);
            setting_ProfileItem_NameText.text = PlayerInformationManager.Instance.GetSelectIconBackgroundName();
            setting_ProfileItem_InformationText.text = 
                "�@�A�C�R���̔w�i��ύX�ł��܂��B\n" +
                "�@�A�C�R���͎�Ƀ����L���O���[�h�ɂČ��J����܂��B\n" +
                "�@�������̃A�C�R���w�i�́A [�V���b�v] �� [�K�`��] ����m���œ���ł��܂��B";

            int length = PlayerInformationManager.Instance.GetIconBackListGenerator().GetLength();
            for (int i = 0; i < length; i++)
            {
                if (PlayerInformationManager.Instance.haveItemForBackgrounds[i] >= 1)
                {
                    SelectItemButtonPrefab selectItemButton =
                        Instantiate(setting_SelectItemButtonPrefab, setting_ProfileItem_Content)
                        .GetComponent<SelectItemButtonPrefab>();

                    selectItemButton.SetCategory(SelectItemButtonPrefab.Category.IconBackground);
                    selectItemButton.SetListNumber(i);
                    selectItemButton.SetSprite();
                }
            }
        }
        else if (nowSelectMode == SelectMode.Setting_Profile_IconFrame)
        {
            setting_CurrentSelectListNumber = PlayerInformationManager.Instance.GetSelectIconFrameNumber();

            setting_ProfileItem_Image.sprite = PlayerInformationManager.Instance.GetIconFrameListGenerator()
                .GetSprite(setting_CurrentSelectListNumber);
            setting_ProfileItem_NameText.text = PlayerInformationManager.Instance.GetSelectIconFrameName();
            setting_ProfileItem_InformationText.text = 
                "�@�A�C�R���̘g��ύX�ł��܂��B\n" +
                "�@�A�C�R���͎�Ƀ����L���O���[�h�ɂČ��J����܂��B\n" +
                "�@�������̃A�C�R���g�́A [�V���b�v] �� [�K�`��] ����m���œ���ł��܂��B";

            int length = PlayerInformationManager.Instance.GetIconFrameListGenerator().GetLength();
            for (int i = 0; i < length; i++)
            {
                if (PlayerInformationManager.Instance.haveItemForFlames[i] >= 1)
                {
                    SelectItemButtonPrefab selectItemButton =
                        Instantiate(setting_SelectItemButtonPrefab, setting_ProfileItem_Content)
                        .GetComponent<SelectItemButtonPrefab>();

                    selectItemButton.SetCategory(SelectItemButtonPrefab.Category.IconFrame);
                    selectItemButton.SetListNumber(i);
                    selectItemButton.SetSprite();
                }
            }
        }
        else
        {
            Debug.LogError($"MenuSceneManager.UpdateProfileContents : Now Mode Error -> {nowSelectMode}");
            return;
        }
        setting_Profile_ItemSelectImage.transform.Find("RightArea").Find("Scroll View").GetComponent<ScrollRect>().verticalNormalizedPosition = 1f;
    }

    /// <summary>
    /// �A�C�e���ύX��ʂ̃X�N���[�����̎q�I�u�W�F�N�g�����ׂ�Destroy����
    /// </summary>
    private void AllDeleteForSettingItemSelectScrollContent()
    {
        //�����̎q����S�Ē��ׂ�
        foreach (Transform child in setting_ProfileItem_Content)
        {
            //�����̎q����Destroy����
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// �v���t�B�[���̃A�C�e���ݒ��ʂŁA�A�C�e����ύX����{�^�����������Ƃ�
    /// </summary>
    public void PushChangeProfileItemSelectButton()
    {
        if (nowSelectMode == SelectMode.Setting_Profile_Character)
        {
            PlayerInformationManager.Instance.UpdateSelectCharacter(setting_CurrentSelectListNumber);
        }
        else if (nowSelectMode == SelectMode.Setting_Profile_IconBackground)
        {
            PlayerInformationManager.Instance.UpdateSelectIconBackground(setting_CurrentSelectListNumber);
        }
        else if (nowSelectMode == SelectMode.Setting_Profile_IconFrame)
        {
            PlayerInformationManager.Instance.UpdateSelectIconFrame(setting_CurrentSelectListNumber);
        }
        else
        {
            Debug.LogError($"MenuSceneManager.PushChangeProfileItemSelectButton : Now Mode Error -> {nowSelectMode}");
            return;
        }

        ChangeActiveProfileItemSelectButton(false);

        SendInformationText("�ݒ��ύX���܂���");

        //PlayFabManager.Instance.UpdateUserDataForIcon();
        profileChangedFlg = true;
    }

    /// <summary>
    /// �v���t�B�[���ݒ�̕ύX��ۑ�����
    /// </summary>
    public void SaveProfile()
    {
        if (!profileChangedFlg) return;

        PlayFabManager.Instance.UpdateUserDataForAll();
        profileChangedFlg = false;
    }

    /// <summary>
    /// PlayFabManager����A�C�R���ݒ�̍X�V������/���s�������Ƃ���M�����Ƃ�
    /// </summary>
    /// <param name="isSuccess">true : ���� , false : ���s</param>
    public void ReceptionUpdateIconSettingPlayFab(bool isSuccess)
    {
        if (isSuccess)
        {
            if (rebootFlg)
            {
                //setting_ToTitleText.text = "�f�[�^���T�[�o�ɕۑ����܂����B\n�^�C�g���֖߂�܂��B";
                //StartCoroutine(RestartApplication());
            }
            else
            {
                SendInformationText("�ݒ�̕ύX��\n�T�[�o�ɕۑ����܂����B");
            }
        }
        else
        {
            if (rebootFlg)
            {
                //rebootFlg = false;
                //setting_ToTitleText.text = "�f�[�^���T�[�o�ɕۑ��ł��܂���ł����B\n�^�C�g���֖߂�̂����d���܂��B";
            }
            else
            {
                SendInformationText("�ݒ�̕ύX���T�[�o��\n�ۑ��ł��܂���ł����B");
            }
        }
    }

    /// <summary>
    /// SelectItemButtonPrefab����A�{�^����������M����
    /// </summary>
    public void ReceptionSelectItemButtonPrefab(int listNumber)
    {
        if (nowSelectMode == SelectMode.Setting_Profile_Character)
        {
            setting_ProfileItem_Image.sprite = PlayerInformationManager.Instance.GetCharacterListGenerator()
                .GetCharacterSprite(listNumber);
            setting_ProfileItem_NameText.text = PlayerInformationManager.Instance.GetCharacterListGenerator()
                .GetCharacterName(listNumber);
        }
        else if (nowSelectMode == SelectMode.Setting_Profile_IconBackground)
        {
            setting_ProfileItem_Image.sprite = PlayerInformationManager.Instance.GetIconBackListGenerator()
                .GetSprite(listNumber);
            setting_ProfileItem_NameText.text = PlayerInformationManager.Instance.GetIconBackListGenerator()
                .GetName(listNumber);
        }
        else if (nowSelectMode == SelectMode.Setting_Profile_IconFrame)
        {
            setting_ProfileItem_Image.sprite = PlayerInformationManager.Instance.GetIconFrameListGenerator()
                .GetSprite(listNumber);
            setting_ProfileItem_NameText.text = PlayerInformationManager.Instance.GetIconFrameListGenerator()
                .GetName(listNumber);
        }
        else
        {
            Debug.LogError($"MenuSceneManager.ReceptionSelectItemButtonPrefab : Now Mode Error -> {nowSelectMode}");
            return;
        }

        setting_CurrentSelectListNumber = listNumber;

        ChangeActiveProfileItemSelectButton(true);
    }

    /// <summary>
    /// �ݒ��ύX����{�^���̉����\���ǂ�����ς���
    /// </summary>
    /// <param name="flg"></param>
    private void ChangeActiveProfileItemSelectButton(bool flg)
    {
        setting_ChangeSelectButton.interactable = flg;
    }

    /// <summary>
    /// �J�X�^���R�[�h��\������{�^�����������Ƃ�
    /// </summary>
    public void PushDisplayCustomCode()
    {
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.KiraKira);

        string customCode = PlayerPrefs.GetString(PlayFabManager.CUSTOM_ID_SAVE_KEY, "");
        GUIUtility.systemCopyBuffer = customCode;

        setting_DisplayCustomCodeText1.text = customCode;
        setting_DisplayCustomCodeText2.text = "����(���p��)�R�[�h���R�s�[���܂���";
    }

    /// <summary>
    /// �J�X�^���R�[�h��ʂ̃e�L�X�g������������
    /// </summary>
    public void InitDisplayCustomCodeTexts()
    {
        setting_DisplayCustomCodeText1.text = "";
        setting_DisplayCustomCodeText2.text = "";
    }

    /// <summary>
    /// ���K���\���ݒ�̏���������
    /// </summary>
    private void SetNotesLanguageContents()
    {
        currentNotesKind = PlayerInformationManager.Instance.GetSettingDisplayNotesKind();
        currentDisplayStickerFlg = PlayerInformationManager.Instance.GetDisplayXylophoneStickerFlg();
        setting_NotesLanguageStatus1.text = PlayerInformationManager.Instance.GetDisplayNotesKindToJapaneseString(currentNotesKind);
        setting_NotesLanguageStatus2.text = currentDisplayStickerFlg ? "�\������" : "�\�����Ȃ�";
    }

    /// <summary>
    /// [���K���̕\�����e]�̕ύX�{�^�����������Ƃ�
    /// </summary>
    public void PushChangeNotesLanguageButton()
    {
        int num = (int)currentNotesKind;
        num++;
        if (num >= Enum.GetValues(typeof(PlayerInformationManager.DisplayNotesKind)).Length) num = 1;
        currentNotesKind = (PlayerInformationManager.DisplayNotesKind)num;
        setting_NotesLanguageStatus1.text = PlayerInformationManager.Instance.GetDisplayNotesKindToJapaneseString(currentNotesKind);
    }

    /// <summary>
    /// [���Տ�̃V�[��]�̕ύX�{�^�����������Ƃ�
    /// </summary>
    public void PushChangeStickerFlgButton()
    {
        currentDisplayStickerFlg = !currentDisplayStickerFlg;
        setting_NotesLanguageStatus2.text = currentDisplayStickerFlg ? "�\������" : "�\�����Ȃ�";
    }

    /// <summary>
    /// [���K���̕\��]�Őݒ肵�����e��ۑ�����
    /// </summary>
    private void SaveNotesLanguageSetting()
    {
        PlayerInformationManager.Instance.SavePlayerPrefsForNotesLanguageAndStickerFlg(currentNotesKind, currentDisplayStickerFlg);
    }

    /// <summary>
    /// �N���W�b�g�e�L�X�g��ǂݍ���ŁA������ɑ������
    /// </summary>
    public void SetCreditText()
    {
        string resourcePath = "Credit/CreditText";

        TextAsset textAsset = Resources.Load<TextAsset>(resourcePath);

        string fileContents = textAsset.text;

        setting_CreditText.text = fileContents;
    }

    /// <summary>
    /// �^�C�g���փ{�^�����������Ƃ�
    /// </summary>
    public void PushToTitle()
    {
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes);
        rebootFlg = true;
        PlayFabManager.Instance.UpdateUserDataForSave();
    }

    /// <summary>
    /// �^�C�g���։�ʂ̃e�L�X�g������������
    /// </summary>
    public void InitToTitleTexts()
    {
        setting_ToTitleText.text = "";
    }

    /// <summary>
    /// �A�v�����ċN������
    /// </summary>
    public IEnumerator RestartApplication()
    {
        // DontDestroyOnLoad�Ŏ���Ă���I�u�W�F�N�g��j������
        var objects = FindObjectsOfType<MonoBehaviour>();

        yield return new WaitForSeconds(0.5f);

        foreach (var obj in objects)
        {
            if (obj.gameObject != gameObject && obj.gameObject != fadeManagerObject)
            {
                Destroy(obj.gameObject);
            }
        }

        yield return new WaitForSeconds(0.5f);

        // ���߂���X�^�[�g����V�[����ǂݍ���
        FadeManager.Instance.LoadScene("Title", 0.5f);
    }

    /// <summary>
    /// �v���C���[�����͊����̃{�^�����������Ƃ�
    /// </summary>
    public void PushNameUpdateButton()
    {
        if (PlayFabManager.Instance.IsValidName(setting_NameEditInputField))
        {
            SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes);

            PlayerInformationManager.Instance.SavePlayerName(setting_NameEditInputField.text);

            setting_NameEditInformationText.text = "�v���C���[����ύX���܂����B";
        }
        else
        {
            SoundManager.Instance.PlaySE(SoundManager.SoundSeType.No);
            setting_NameEditInformationText.text = "�V�������O�������𖞂����Ă��܂���B";
        }
    }

    /// <summary>
    /// �v���C���[����ύX��ʂ̃e�L�X�g������������
    /// </summary>
    public void InitNameEditTexts()
    {
        setting_NameEditInformationText.text = "";
        setting_NameEditInputField.text = PlayerInformationManager.Instance.GetPlayerName();
    }

    /// <summary>
    /// �f�[�^�폜�{�^�����������Ƃ�
    /// </summary>
    public void PushDeleteButton()
    {
        PlayFabManager.Instance.DeletePlayerData();
    }

    /// <summary>
    /// PlayFab����f�[�^�폜�̌��ʂ���M����
    /// </summary>
    /// <param name="isSuccess"></param>
    public void ReceptionDeletePlayFab(bool isSuccess)
    {
        if (isSuccess)
        {
            PlayerPrefs.DeleteAll();
            //PlayerInformationManager.Instance.DeleteSaveFile();
        }

        GameObject obj = Instantiate(setting_DeleteInformationPrefab, canvasTransform);
        obj.GetComponent<DeleteInformationPrefab>().SetMyStatus(isSuccess);
    }

    /// <summary>
    /// �f�[�^�폜��ɍċN������
    /// </summary>
    public void ReceptionAfterDelete()
    {
        StartCoroutine(RestartApplication());
    }

    /// <summary>
    /// �f�o�b�O�{�^�����������Ƃ�
    /// </summary>
    public void PushDebugButton()
    {
        PlayerInformationManager.Instance.PushDebugButton();
    }

#endregion

#region Shop

    /// <summary>
    /// �����[�h�L��������̃p�l����\������
    /// </summary>
    public void DisplayRewardShowPanel()
    {
        currentShopResultPanelPrefab = Instantiate(shop_RewardShowPanelPrefab, canvasTransform).GetComponent<ShopResultPanelPrefab>();

        currentShopResultPanelPrefab.SetMyContent(ShopResultPanelPrefab.Content.AdReward);
        currentShopResultPanelPrefab.SetMySub1Text("�������ʂ�҂��Ă��܂��B");
        currentShopResultPanelPrefab.SetMySub2Text("�����������҂����������B");
    }

    /// <summary>
    /// Reward�L������������{�^�����������Ƃ�
    /// </summary>
    public void PushShowRewardAd()
    {
        rewardFlg = false;
        DisplayRewardShowPanel();

        AdMobManager.Instance.ShowReward();
    }

    /// <summary>
    /// �����[�h�L�����ς���ɌĂ΂��
    /// </summary>
    /// <param name="flg">true : �L�����Ō�܂Ŋς� , false ; �L�����Ăяo����</param>
    public void FinishReward(bool flg)
    {
        if (rewardFlg) return;

        if (flg)
        {
            rewardFlg = true;

            int odds = 1;
            int rand = UnityEngine.Random.Range(0, 11);
            switch (rand)
            {
                case 0: odds = 1; break;
                case 1: odds = 1; break;
                case 2: odds = 1; break;
                case 3: odds = 1; break;
                case 4: odds = 2; break;
                case 5: odds = 2; break;
                case 6: odds = 2; break;
                case 7: odds = 2; break;
                case 8: odds = 3; break;
                case 9: odds = 3; break;
                case 10: odds = 3; break;
                default: odds = 1; break;
            }

            PlayFabManager.Instance.AddGachaTicket(GET_REWARD_SP_COIN_NUM * odds);
            SoundManager.Instance.PlaySE(SoundManager.SoundSeType.KiraKira);

            currentShopResultPanelPrefab.SetMySub1Text("��V���l�����܂����B");
            forShopResultCoroutine = StartCoroutine(UpdateRewardSpCoinCoroutine());
        }
        else
        {
            currentShopResultPanelPrefab.SetMySub1Text("�L���̎����Ɏ��s���܂����B");
            currentShopResultPanelPrefab.SetMySub2Text("����SP�R�C�� : " + PlayFabManager.Instance.GetHaveGachaTicket().ToString());
        }

        Debug.Log($"MenuSceneManager.FinishReward : flg = {flg}");
    }

    /// <summary>
    /// Reward�̌��ʕ\���̍ہASP�R�C���̖������X�V����Ȃ��Ƃ��̂��߂̃R���[�`��
    /// </summary>
    /// <returns></returns>
    private IEnumerator UpdateRewardSpCoinCoroutine()
    {
        int coinNum = PlayFabManager.Instance.GetHaveGachaTicket();
        float timer = 0;

        currentShopResultPanelPrefab.SetMySub2Text("����SP�R�C�� : " + coinNum.ToString());

        while (timer < 10f)
        {
            if (coinNum != PlayFabManager.Instance.GetHaveGachaTicket())
            {
                coinNum = PlayFabManager.Instance.GetHaveGachaTicket();
                break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        currentShopResultPanelPrefab.SetMySub2Text("����SP�R�C�� : " + coinNum.ToString());
    }

    /// <summary>
    /// currentShopResultPanelPrefab���������Ƃ���M�����Ƃ�
    /// </summary>
    public void ReceptionCloseFromShopResultPanelPrefab()
    {
        StopCoroutine(forShopResultCoroutine);
        currentShopResultPanelPrefab = null;
    }

    /// <summary>
    /// SP�R�C�������̕\�����X�V����
    /// </summary>
    public void UpdateGachaTicketText()
    {
        shop_HaveSPCoinText.text = $"SP�R�C�� : {PlayFabManager.Instance.GetHaveGachaTicket().ToString("N0")} ��";
    }

    /// <summary>
    /// �K�`���̃X�g�A�����擾����
    /// </summary>
    public void LoadStoreGacha()
    {
        if (PlayFabManager.Instance.CatalogItems == null)
        {
            PlayFabManager.Instance.GetGachaCatalogData();
        }
    }

    /// <summary>
    /// �K�`��������{�^�����������Ƃ�
    /// </summary>
    /// <param name="gachaTimes">�K�`����(1or11)</param>
    public void PushGachaButton(int gachaTimes)
    {
        Instantiate(shop_GachaConfirmPanelPrefab, canvasTransform).GetComponent<GachaConfirmPanelPrefab>().SetMyStatus(gachaTimes);
    }

    /// <summary>
    /// �P���K�`�������s����
    /// </summary>
    public void Old_ShoppingGacha1()
    {
        //if (waitingGachaResultFlg) return;

        //waitingGachaResultFlg = true;
        //currentGachaResultPanelPrefab = Instantiate(shop_GachaResultPanelPrefab, canvasTransform).GetComponent<GachaResultPanelPrefab>();
        //PlayFabManager.Instance.PurchaseGacha(1);
        ////StartCoroutine(GachaStaging());

        ShoppingGacha(1);
    }

    /// <summary>
    /// 11�A�K�`�������s����
    /// </summary>
    public void Old_ShoppingGacha11()
    {
        //if (waitingGachaResultFlg) return;

        //waitingGachaResultFlg = true;
        //currentGachaResultPanelPrefab = Instantiate(shop_GachaResultPanelPrefab, canvasTransform).GetComponent<GachaResultPanelPrefab>();
        //PlayFabManager.Instance.PurchaseGacha(11);
        ////StartCoroutine(GachaStaging());

        ShoppingGacha(11);
    }

    /// <summary>
    /// �K�`�������s����
    /// </summary>
    /// <param name="gachaTimes">�K�`����(1or11)</param>
    public void ShoppingGacha(int gachaTimes)
    {
        if (waitingGachaResultFlg) return;

        waitingGachaResultFlg = true;
        currentGachaResultPanelPrefab = Instantiate(shop_GachaResultPanelPrefab, canvasTransform).GetComponent<GachaResultPanelPrefab>();
        PlayFabManager.Instance.PurchaseGacha(gachaTimes);
        //StartCoroutine(GachaStaging());
    }

    /// <summary>
    /// �K�`�����o��New�e�L�X�g��_�ł����邽�߂̒l���v�Z����
    /// </summary>
    private void UpdateColorLerpValueForGachaNewText()
    {
        if (!waitingGachaResultFlg) return;

        float CHANGE_COLOR_SPEED = 0.8f;
        shop_TimerForChangeColorLeapForGacha += Time.deltaTime * CHANGE_COLOR_SPEED;
        shop_ValueForChangeColorLeapForGacha = (Mathf.Sin(shop_TimerForChangeColorLeapForGacha * 2f * Mathf.PI) + 1) / 2;
        //Debug.Log($"shop_ValueForChangeColorLeapForGacha = {shop_ValueForChangeColorLeapForGacha}");
    }

    /// <summary>
    /// �K�`�����o��New�e�L�X�g��_�ł����邽�߂̒l��Ԃ��Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public float GetColorLerpValueForGachaNewText()
    {
        return shop_ValueForChangeColorLeapForGacha;
    }

    /// <summary>
    /// �K�`�����s���󂯎�����Ƃ�
    /// </summary>
    /// <param name="str">�\������G���[���b�Z�[�W</param>
    public void ReceptionGachaFailure(string str)
    {
        currentGachaResultPanelPrefab.SpawnGachaFailurePrefab(str);
    }

    /// <summary>
    /// �K�`�����ʂ���M���āA���e�𐮗�����
    /// </summary>
    /// <param name="resultGachaList">�K�`�����ʂ̃��X�g</param>
    public void ReceptionGachaResult(List<string> resultGachaList)
    {
        List<PlayerInformationManager.GachaItemKind> resultItemKindsList = new List<PlayerInformationManager.GachaItemKind>();
        List<int> resultItemNumberList = new List<int>();

        bool errorFlg = false;

        foreach (string item in resultGachaList)
        {
            string[] arr = item.Split('_');

            switch (arr[0])
            {
                case "C":
                    resultItemKindsList.Add(PlayerInformationManager.GachaItemKind.Character);
                    break;
                case "B":
                    resultItemKindsList.Add(PlayerInformationManager.GachaItemKind.IconBackground);
                    break;
                case "F":
                    resultItemKindsList.Add(PlayerInformationManager.GachaItemKind.IconFrame);
                    break;
                default:
                    errorFlg = true;
                    break;
            }

            if (int.TryParse(arr[1], out int num)) 
            {
                resultItemNumberList.Add(num);
            }
            else
            {
                errorFlg = true;
            }

        }

        if (errorFlg)
        {
            Debug.LogError($"GachaGenerator.ReceptionGachaResultAndLotteryGachaAll : Error");
        }
        else
        {
            for (int i = 0; i < resultItemKindsList.Count; i++) 
            {
                // ���ʂ̕ۑ�
                bool newFlg = PlayerInformationManager.Instance.AcquisitionItemAndGetNewFlg(resultItemKindsList[i], resultItemNumberList[i]);

                // ���ʂ�\��
                currentGachaResultPanelPrefab.SpawnGachaResultPrefab(resultItemKindsList[i], resultItemNumberList[i], newFlg);
            }

            PlayerInformationManager.Instance.SaveJson();
        }
    }

    /// <summary>
    /// currentGachaResultPanelPrefab���������Ƃ���M�����Ƃ�
    /// </summary>
    public void ReceptionCloseFromGachaResultPanelPrefab()
    {
        waitingGachaResultFlg = false;
        currentGachaResultPanelPrefab = null;
    }

    /// <summary>
    /// �K�`���̉��o
    /// </summary>
    /// <returns></returns>
    private IEnumerator Old_GachaStaging()
    {
        //gachaGenerator.DisplayGachaPanel(true);

        yield return null;
    }

    /// <summary>
    /// �K�`���̒��ӎ����{�^�����������Ƃ�
    /// </summary>
    public void PushGachaCautionButton()
    {
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes);

        GameObject obj = Instantiate(shop_GachaCautionPanelPrefab, canvasTransform);

        obj.GetComponent<GachaCautionPanelPrefab>().SetMyText();
    }

    /// <summary>
    /// �uSP�R�C�������炤�v�̃e�L�X�g�ƃ{�^���̏�Ԃ��X�V����
    /// </summary>
    private void UpdateObtainContents()
    {
        int oldClearStarNum = PlayerInformationManager.Instance.GetClearStarNumForObtainSpCoin();
        int totalStarNum = PlayerInformationManager.Instance.GetTotalHaveStarsForStageClearMode();
        int getSpCoinNum = totalStarNum - oldClearStarNum;

        shop_ObtainText1.text = $"{oldClearStarNum} ��";
        shop_ObtainText2.text = $"{totalStarNum} ��";
        shop_ObtainText3.text = $"{getSpCoinNum} ��";

        shop_ObtainButton.interactable = getSpCoinNum > 0 ? true : false;
    }

    /// <summary>
    /// �uSP�R�C�������炤�v�{�^�����������Ƃ�
    /// </summary>
    public void PushObtainButton()
    {
        shop_ObtainButton.interactable = false;

        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Ok);

        int oldClearStarNum = PlayerInformationManager.Instance.GetClearStarNumForObtainSpCoin();
        int totalStarNum = PlayerInformationManager.Instance.GetTotalHaveStarsForStageClearMode();
        int getSpCoinNum = totalStarNum - oldClearStarNum;

        PlayerInformationManager.Instance.UpdateClearStarNumAndObtainSpCoin();

        PlayFabManager.Instance.AddGachaTicket(getSpCoinNum);
    }

    /// <summary>
    /// PlayFabManager����SP�R�C���̒ǉ��������ʂ��󂯎�����Ƃ�
    /// </summary>
    /// <param name="errorFlg">�G���[�Ȃ�True</param>
    /// <param name="addSpCoin">SP�R�C��������������</param>
    public void ReceptionAddSpCoin(bool errorFlg , int addSpCoin)
    {
        if (errorFlg)
        {
            SendInformationText($"SP�R�C���̒ǉ��Ɏ��s���܂����B");
        }
        else
        {
            SendInformationText($"SP�R�C���� {addSpCoin} ��\n�Q�b�g���܂����B");
        }
    }

    /// <summary>
    /// SNS�V�F�A�̃{�^�����������Ƃ�
    /// </summary>
    public void PushShareButton()
    {
        string url = "";
        //string image_path = "";
        string str1 = "�傫�Ȗ؋Ղ̏�𑖂��đt�ł�Q�[���I\n";

        string str3 = "�����A�݂�Ȃ�����Ă݂悤�I�I\n";

        string str6 = "#�؋Ճ_�b�V�� ";

        string text = str1 + str3 + str6;

        bool flg = false;
        if (Application.platform == RuntimePlatform.Android)
        {
            url = "https://play.google.com/store/apps/details?id=com.DanchingStar.XylophoneDash";
            //image_path = Application.persistentDataPath + "/SS.png";
            text = text + "#Android\n";
            Debug.Log("Android");
            flg = true;
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            url = "https://www.google.com";
            //image_path = Application.persistentDataPath + "/SS.png";
            text = text + "#iPhone\n";
            Debug.Log("iPhone");
            flg = true;
        }
        else
        {
            url = "https://www.google.com";
            //image_path = Application.persistentDataPath + "/SS.png";
            //text = text + "���̑��̋@��\n";
            Debug.Log("Other OS");
        }

        SocialConnector.SocialConnector.Share(text, url); //��1����:�e�L�X�g,��2����:URL,��3����:�摜

        if (PlayerInformationManager.Instance.GetTodaySNSShareFlg() == false && flg == true)
        {
            PlayerInformationManager.Instance.UpdateAndSetTodaySNSShareFlgTrue();
            PlayFabManager.Instance.AddGachaTicket(GET_REWARD_SP_COIN_NUM);
            SendInformationText($"SP�R�C����{GET_REWARD_SP_COIN_NUM}��\n�l�����܂����B");
        }

    }

    /// <summary>
    /// �V�F�A�ς݂��ǂ����̃e�L�X�g��I��
    /// </summary>
    private void SelectShareText()
    {
        if (PlayerInformationManager.Instance.GetTodaySNSShareFlg())
        {
            shop_SnsShareText.text = "�����̓V�F�A�ς݂ł��B";
        }
        else
        {
            shop_SnsShareText.text = "�����͂܂��V�F�A���Ă��܂���B";
        }
    }

#endregion




}
