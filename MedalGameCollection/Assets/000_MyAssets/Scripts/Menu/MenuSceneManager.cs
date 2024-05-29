using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DanielLochner.Assets.SimpleScrollSnap;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;

namespace Menu
{
    public class MenuSceneManager : MonoBehaviour
    {
        [SerializeField] private GameObject topMenuPanel;
        [SerializeField] private GameObject gamePanel;
        [SerializeField] private GameObject shopPanel;
        [SerializeField] private GameObject achievementPanel;
        [SerializeField] private GameObject friendPanel;
        [SerializeField] private FriendPanel myFriendPanel;
        [SerializeField] private GameObject settingPanel;
        [SerializeField] private GameObject shop_ExchangePanel;
        [SerializeField] private GameObject shop_GachaPanel;
        [SerializeField] private GameObject shop_AdvertisementPanel;
        [SerializeField] private GameObject shop_SNS_Share;
        [SerializeField] private GameObject shop_RewardShowPanel;
        [SerializeField] private Text snsShareText;
        [SerializeField] private Text rewardShowText1;
        [SerializeField] private Text rewardShowText2;
        [SerializeField] private GameObject setting_VolumePanel;
        [SerializeField] private GameObject setting_ProfilePanel;
        [SerializeField] private GameObject setting_PlayerNameEditPanel;
        [SerializeField] private TMP_InputField setting_NameEditInputField;
        [SerializeField] private Text setting_NameEditText;
        [SerializeField] private GameObject setting_CreditPanel;
        [SerializeField] private TextMeshProUGUI setting_CreditText;
        [SerializeField] private GameObject setting_CustomCodePanel;
        [SerializeField] private Text displayCustomCodeText1;
        [SerializeField] private Text displayCustomCodeText2;
        [SerializeField] private GameObject setting_ToTitlePanel;
        [SerializeField] private Text toTitleText;
        [SerializeField] private GameObject safetyPanel;
        [SerializeField] private GameObject setting_DeletePanel;
        [SerializeField] private GameObject setting_DeleteFinalPanel;

        [SerializeField] private PlayerInformation myPlayerInformation;
        [SerializeField] private GachaGenerator gachaGenerator;

        [SerializeField] private InformationCutInPanel informationCutInPanel;

        [SerializeField] private Button backButton;
        [SerializeField] private Text menuContentsText;

        [SerializeField] private SimpleScrollSnap stageSelect;
        [SerializeField] private Text stageNameText;
        [SerializeField] private Text stageIntroductionText;
        [SerializeField] private Text dialogStageNameText;
        [SerializeField] private Text haveMedalsText;
        [SerializeField] private Text haveSPCoinsText;
        [SerializeField] private Text haveGachaTicketText;

        [SerializeField] private GameObject stageMoveDialogPanel;

        [SerializeField] private GameObject loginBonusPanelPrefab;
        [SerializeField] private GameObject shopContentsPanelPrefab;
        [SerializeField] private GameObject gachaCautionPanelPrefab;
        [SerializeField] private GameObject deleteInformationPrefab;

        private Achievement achievement;

        private MenuContents currentContents = MenuContents.None;
        private bool isDisableTapButton = false;

        private Vector3 defaultPanelPosition;
        private float panelDistance = 1500f;
        private float panelMoveSpeed = 1.8f;

        private int currentStageNum = -1;
        private TextList.Stage currentStage = TextList.Stage.None;

        private const int GET_SP_COIN_NUM = 1;

        private GameObject fadeManagerObject;
        private const float FADE_MANAGER_TIME = 0.3f;

        private bool waitingGachaResultFlg;

        private bool rebootFlg;

        private bool rewardFlg;

        /// <summary>
        /// ���j���[�̓��e����
        /// </summary>
        public enum MenuContents
        {
            None,

            TopMenu,
            Game,
            Shop,
            Achievement,
            Friend,
            Setting,

            Shop_Exchange,
            Shop_Gacha,
            Shop_Advertisement,
            Shop_SNS_Share,

            Setting_Volume,
            Setting_Profile,
            Setting_PlayerNameEdit,
            Setting_Credit,
            Setting_CustomCode,
            Setting_ToTitle,
            Setting_Delete,
            Setting_DeleteFinal,
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
            safetyPanel.SetActive(true);

            Time.timeScale = 1f;

            achievement = this.gameObject.GetComponent<Achievement>();

            if (PlayFabManager.Instance.GetIsLogined()) //���O�C���󋵂��m�F
            {
                AfterCheckPlayFabLogin(true);
            }
            else
            {
                PlayFabManager.Instance.Login(); //���O�C������
            }
        }

        private void Update()
        {
            UpdateForContents();
        }

        /// <summary>
        /// Test�{�^���̏���(�f�o�b�O�p)
        /// </summary>
        public void TestButton()
        {
            Debug.Log($"currentContents : {currentContents}");
            // Debug.Log($"NumberOfPanels : {stageSelect.NumberOfPanels}");
        }

        /// <summary>
        /// ��������J�b�g�C���ɕ\������(�e�X�g�p)
        /// </summary>
        /// <param name="text"></param>
        public void PushTestString(string text)
        {
            SendInformationText(text);
        }

        /// <summary>
        /// Start�ŌĂяo����鏉����
        /// </summary>
        private void InitStart()
        {
            defaultPanelPosition = topMenuPanel.GetComponent<Transform>().position;
            currentContents = MenuContents.TopMenu;
            UpdateDisplayBackButton();
            UpdateMenuContentsText();

            //waitingGachaResultFlg = false;

            fadeManagerObject = FadeManager.Instance.gameObject;
            rebootFlg = false;
            rewardFlg = false;
        }

        /// <summary>
        /// �\���ɍ��킹��Update�̓��e��ς���
        /// </summary>
        private void UpdateForContents()
        {
            if (currentContents == MenuContents.TopMenu)
            {

            }
            else if (currentContents == MenuContents.Game)
            {
                if (GetCenteredPanel() != currentStageNum)
                {
                    currentStageNum = GetCenteredPanel();
                    currentStage = (TextList.Stage)(currentStageNum + 1);
                    UpdateStageSelectTexts();
                }
            }
        }

        /// <summary>
        /// ���j���[�̃{�^�����������Ƃ�
        /// </summary>
        /// <param name="toContent">�������{�^���̓��e</param>
        [com.llamagod.EnumAction(typeof(MenuContents))]
        public void OnTapMenuButton(int toContent)
        {
            if (isDisableTapButton) return;

            MenuContents content = (MenuContents)toContent;

            if (content == MenuContents.TopMenu)
            {
                // BackButton.gameObject.SetActive(false);
            }
            else
            {
                if (content == MenuContents.None)
                {
                    Debug.LogError($"MenuSceneManager.OnTapMenuButton : content is None");
                }
                else
                {
                    SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes1);
                    StartCoroutine(ChangeDisplayContents(content));
                }
            }
        }

        /// <summary>
        /// Panel�̕\�������ւ��鉉�o������
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private IEnumerator ChangeDisplayContents(MenuContents content)
        {
            GameObject currentPanel = null;
            GameObject previousPanel = null;

            switch (content)
            {
                case MenuContents.Game:
                    currentPanel = gamePanel;
                    currentContents = MenuContents.Game;
                    previousPanel = topMenuPanel;
                    break;
                case MenuContents.Shop:
                    currentPanel = shopPanel;
                    currentContents = MenuContents.Shop;
                    previousPanel = topMenuPanel;
                    LoadStoreGacha();
                    break;
                case MenuContents.Achievement:
                    currentPanel = achievementPanel;
                    currentContents = MenuContents.Achievement;
                    previousPanel = topMenuPanel;
                    UpdateAchievementOfGameKinds();
                    break;
                case MenuContents.Friend:
                    currentPanel = friendPanel;
                    currentContents = MenuContents.Friend;
                    previousPanel = topMenuPanel;
                    myFriendPanel.ResetFriendPanel();
                    break;
                case MenuContents.Setting:
                    currentPanel = settingPanel;
                    currentContents = MenuContents.Setting;
                    previousPanel = topMenuPanel;
                    break;
                case MenuContents.Shop_Exchange:
                    currentPanel = shop_ExchangePanel;
                    currentContents = MenuContents.Shop_Exchange;
                    previousPanel = shopPanel;
                    break;
                case MenuContents.Shop_Gacha:
                    currentPanel = shop_GachaPanel;
                    currentContents = MenuContents.Shop_Gacha;
                    previousPanel = shopPanel;
                    UpdateGachaTicketText();
                    break;
                case MenuContents.Shop_Advertisement:
                    currentPanel = shop_AdvertisementPanel;
                    currentContents = MenuContents.Shop_Advertisement;
                    previousPanel = shopPanel;
                    break;
                case MenuContents.Shop_SNS_Share:
                    currentPanel = shop_SNS_Share;
                    currentContents = MenuContents.Shop_SNS_Share;
                    previousPanel = shopPanel;
                    SelectShareText();
                    break;
                case MenuContents.Setting_Volume:
                    currentPanel = setting_VolumePanel;
                    currentContents = MenuContents.Setting_Volume;
                    previousPanel = settingPanel;
                    break;
                case MenuContents.Setting_Profile:
                    currentPanel = setting_ProfilePanel;
                    currentContents = MenuContents.Setting_Profile;
                    previousPanel = settingPanel;
                    break;
                case MenuContents.Setting_PlayerNameEdit:
                    currentPanel = setting_PlayerNameEditPanel;
                    currentContents = MenuContents.Setting_PlayerNameEdit;
                    previousPanel = settingPanel;
                    InitNameEditTexts();
                    break;
                case MenuContents.Setting_Credit:
                    currentPanel = setting_CreditPanel;
                    currentContents = MenuContents.Setting_Credit;
                    previousPanel = settingPanel;
                    SetCreditText();
                    break;
                case MenuContents.Setting_CustomCode:
                    currentPanel = setting_CustomCodePanel;
                    currentContents = MenuContents.Setting_CustomCode;
                    previousPanel = settingPanel;
                    InitDisplayCustomCodeTexts();
                    break;
                case MenuContents.Setting_ToTitle:
                    currentPanel = setting_ToTitlePanel;
                    currentContents = MenuContents.Setting_ToTitle;
                    previousPanel = settingPanel;
                    InitToTitleTexts();
                    break;
                case MenuContents.Setting_Delete:
                    currentPanel = setting_DeletePanel;
                    currentContents = MenuContents.Setting_Delete;
                    previousPanel = settingPanel;
                    InitToTitleTexts();
                    break;
                case MenuContents.Setting_DeleteFinal:
                    currentPanel = setting_DeleteFinalPanel;
                    currentContents = MenuContents.Setting_DeleteFinal;
                    previousPanel = setting_DeletePanel;
                    InitToTitleTexts();
                    break;
                default:
                    Debug.LogError($"MenuSceneManager.ChangeDisplayContents : content is None");
                    yield break;
            }

            isDisableTapButton = true;

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

            UpdateDisplayBackButton();
            UpdateMenuContentsText();

            isDisableTapButton = false;
        }

        /// <summary>
        /// �߂�{�^�����������Ƃ�
        /// </summary>
        public void OnTapBackButton()
        {
            if (isDisableTapButton) return;

            SoundManager.Instance.PlaySE(SoundManager.SoundSeType.No1);
            StartCoroutine(ChangeDisplayContentsBack());
        }

        /// <summary>
        /// Panel�̕\����O��Panel�ɖ߂鉉�o������
        /// </summary>
        /// <returns></returns>
        private IEnumerator ChangeDisplayContentsBack()
        {
            GameObject currentPanel = null;
            GameObject previousPanel = null;

            switch (currentContents)
            {
                case MenuContents.Game:
                    currentPanel = gamePanel;
                    previousPanel = topMenuPanel;
                    currentContents = MenuContents.TopMenu;
                    break;
                case MenuContents.Shop:
                    currentPanel = shopPanel;
                    previousPanel = topMenuPanel;
                    currentContents = MenuContents.TopMenu;
                    break;
                case MenuContents.Achievement:
                    currentPanel = achievementPanel;
                    previousPanel = topMenuPanel;
                    currentContents = MenuContents.TopMenu;
                    break;
                case MenuContents.Friend:
                    currentPanel = friendPanel;
                    previousPanel = topMenuPanel;
                    currentContents = MenuContents.TopMenu;
                    break;
                case MenuContents.Setting:
                    currentPanel = settingPanel;
                    previousPanel = topMenuPanel;
                    currentContents = MenuContents.TopMenu;
                    break;
                case MenuContents.Shop_Exchange:
                    currentPanel = shop_ExchangePanel;
                    previousPanel = shopPanel;
                    currentContents = MenuContents.Shop;
                    break;
                case MenuContents.Shop_Gacha:
                    currentPanel = shop_GachaPanel;
                    previousPanel = shopPanel;
                    currentContents = MenuContents.Shop;
                    break;
                case MenuContents.Shop_Advertisement:
                    currentPanel = shop_AdvertisementPanel;
                    previousPanel = shopPanel;
                    currentContents = MenuContents.Shop;
                    break;
                case MenuContents.Shop_SNS_Share:
                    currentPanel = shop_SNS_Share;
                    previousPanel = shopPanel;
                    currentContents = MenuContents.Shop;
                    break;
                case MenuContents.Setting_Volume:
                    currentPanel = setting_VolumePanel;
                    previousPanel = settingPanel;
                    currentContents = MenuContents.Setting;
                    SoundManager.Instance.SaveSliderPrefs();
                    break;
                case MenuContents.Setting_Profile:
                    currentPanel = setting_ProfilePanel;
                    previousPanel = settingPanel;
                    currentContents = MenuContents.Setting;
                    break;
                case MenuContents.Setting_PlayerNameEdit:
                    currentPanel = setting_PlayerNameEditPanel;
                    previousPanel = settingPanel;
                    currentContents = MenuContents.Setting;
                    break;
                case MenuContents.Setting_Credit:
                    currentPanel = setting_CreditPanel;
                    previousPanel = settingPanel;
                    currentContents = MenuContents.Setting;
                    break;
                case MenuContents.Setting_CustomCode:
                    currentPanel = setting_CustomCodePanel;
                    previousPanel = settingPanel;
                    currentContents = MenuContents.Setting;
                    break;
                case MenuContents.Setting_ToTitle:
                    currentPanel = setting_ToTitlePanel;
                    previousPanel = settingPanel;
                    currentContents = MenuContents.Setting;
                    break;
                case MenuContents.Setting_Delete:
                    currentPanel = setting_DeletePanel;
                    previousPanel = settingPanel;
                    currentContents = MenuContents.Setting;
                    break;
                case MenuContents.Setting_DeleteFinal:
                    currentPanel = setting_DeleteFinalPanel;
                    previousPanel = settingPanel;
                    currentContents = MenuContents.Setting;
                    break;
                default:
                    Debug.LogError($"MenuSceneManager.ChangeDisplayContentsToTopMenu : currentContents is None");
                    yield break;
            }

            isDisableTapButton = true;

            if (currentContents == MenuContents.TopMenu)
            {
                UpdateHaveMedalsAndSPCoinsAndPlayerInformation();
            }

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

            UpdateDisplayBackButton();
            UpdateMenuContentsText();

            isDisableTapButton = false;
        }

        /// <summary>
        /// MenuContentsText(�߂�{�^���̉��̕�����)���X�V����
        /// </summary>
        private void UpdateMenuContentsText()
        {
            switch (currentContents)
            {
                case MenuContents.TopMenu:
                    menuContentsText.text = "���j���[";
                    break;
                case MenuContents.Game:
                    menuContentsText.text = "�Q�[��";
                    break;
                case MenuContents.Shop:
                    menuContentsText.text = "�V���b�v";
                    break;
                case MenuContents.Achievement:
                    menuContentsText.text = "�B���x";
                    break;
                case MenuContents.Friend:
                    menuContentsText.text = "�t�����h";
                    break;
                case MenuContents.Setting:
                    menuContentsText.text = "�ݒ�";
                    break;
                case MenuContents.Shop_Exchange:
                    menuContentsText.text = "������";
                    break;
                case MenuContents.Shop_Gacha:
                    menuContentsText.text = "�K�`��";
                    break;
                case MenuContents.Shop_Advertisement:
                    menuContentsText.text = "�L��������";
                    break;
                case MenuContents.Shop_SNS_Share:
                    menuContentsText.text = "SNS�ŃV�F�A";
                    break;
                case MenuContents.Setting_Volume:
                    menuContentsText.text = "����";
                    break;
                case MenuContents.Setting_Profile:
                    menuContentsText.text = "�v���t�B�[���摜";
                    break;
                case MenuContents.Setting_PlayerNameEdit:
                    menuContentsText.text = "�v���C���[���̕ύX";
                    break;
                case MenuContents.Setting_Credit:
                    menuContentsText.text = "�N���W�b�g";
                    break;
                case MenuContents.Setting_CustomCode:
                    menuContentsText.text = "����(���p��)�R�[�h";
                    break;
                case MenuContents.Setting_ToTitle:
                    menuContentsText.text = "�^�C�g����";
                    break;
                case MenuContents.Setting_Delete:
                    menuContentsText.text = "�f�[�^�폜";
                    break;
                case MenuContents.Setting_DeleteFinal:
                    menuContentsText.text = "�f�[�^�폜(�ŏI�m�F)";
                    break;
                default:
                    Debug.LogError($"MenuSceneManager.UpdateMenuContentsText : currentContents is None");
                    break;
            }
        }

        /// <summary>
        /// �߂�{�^���̕\����\�����X�V����
        /// </summary>
        private void UpdateDisplayBackButton()
        {
            if (currentContents == MenuContents.TopMenu)
            {
                backButton.gameObject.SetActive(false);
            }
            else
            {
                backButton.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// �����Ă��郁�_����SP�R�C���ƃv���C���[���̃e�L�X�g���X�V����
        /// </summary>
        public void UpdateHaveMedalsAndSPCoinsAndPlayerInformation()
        {
            myPlayerInformation.UpdateDisplayInformationText();
            haveMedalsText.text = $"���_�� : {PlayerInformationManager.Instance.GetHaveMedal().ToString("N0")} ��";
            haveSPCoinsText.text = $"SP�R�C�� : {PlayerInformationManager.Instance.GetSPCoin().ToString("N0")} ��";
        }

        /// <summary>
        /// PlayFabManager����Z�[�u�̍X�V������/���s�������Ƃ���M�����Ƃ�
        /// </summary>
        /// <param name="isSuccess">true : ���� , false : ���s</param>
        public void ReceptionUpdatePlayFab(bool isSuccess)
        {
            if (isSuccess)
            {
                if (rebootFlg)
                {
                    toTitleText.text = "�f�[�^���T�[�o�ɕۑ����܂����B\n�^�C�g���֖߂�܂��B";
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
                    toTitleText.text = "�f�[�^���T�[�o�ɕۑ��ł��܂���ł����B\n�^�C�g���֖߂�̂����d���܂��B";
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
        /// ���O�C���������`�F�b�N������Ɏ��s
        /// </summary>
        public void AfterCheckPlayFabLogin(bool flg)
        {
            if (flg)
            {
                PlayerInformationManager.Instance.InitLoad();
                InitStart();
                UpdateHaveMedalsAndSPCoinsAndPlayerInformation();

                CheckLoginDay();

                StartCoroutine(UpdatePlayFabData());

                CloseAllOtherPanel();

                AchievementInit();

                UpdateAchievement();

                safetyPanel.SetActive(false);
            }
            else
            {
                Debug.LogError("AfterCheckPlayFabLogin : Error");
                AfterCheckPlayFabLogin(true);
            }
        }

        /// <summary>
        /// ���O�C���̓��t���`�F�b�N����
        /// </summary>
        private void CheckLoginDay()
        {
            DateTime today = DateTime.Today;

            //Debug.Log($"MenuSceneManager.CheckLoginDay : " +
            //    $"{PlayerInformationManager.Instance.GetLastLoginDay()} , {today}");

            if ((PlayerInformationManager.Instance.GetLastLoginDay() - today).TotalDays != 0) 
            {
                PlayerInformationManager.Instance.UpdateLoginDays();
                LoginBonus();
            }
            else
            {

            }
        }

        /// <summary>
        /// ���O�C���{�[�i�X��t�^����
        /// </summary>
        private void LoginBonus()
        {
            GameObject canvas = GameObject.FindGameObjectWithTag("Point");

            GameObject obj = Instantiate(loginBonusPanelPrefab, canvas.transform);

            obj.GetComponent<LoginBonusPanelPrefab>().SetMyStatus();
        }

        /// <summary>
        /// PlayFab�̃f�[�^���X�V����
        /// </summary>
        /// <returns></returns>
        private IEnumerator UpdatePlayFabData()
        {
            PlayFabManager.Instance.UpdateUserData();

            yield return null;
        }

        /// <summary>
        /// ���C���ȊO�̃A�N�e�B�u�ȃp�l����S�Ĕ�A�N�e�B�u�ɂ���(Start�ŌĂ�)
        /// </summary>
        private void CloseAllOtherPanel()
        {
            ClosePanel(gamePanel);
            ClosePanel(shopPanel);
            ClosePanel(achievementPanel);
            ClosePanel(friendPanel); 
            ClosePanel(settingPanel);

            ClosePanel(shop_ExchangePanel);
            ClosePanel(shop_GachaPanel);
            ClosePanel(shop_AdvertisementPanel);
            ClosePanel(shop_SNS_Share);

            ClosePanel(setting_VolumePanel);
            ClosePanel(setting_ProfilePanel);
            ClosePanel(setting_PlayerNameEditPanel);
            ClosePanel(setting_CreditPanel);
            ClosePanel(setting_CustomCodePanel);
            ClosePanel(setting_ToTitlePanel);
            ClosePanel(setting_DeletePanel);
            ClosePanel(setting_DeleteFinalPanel);
        }

        /// <summary>
        /// �p�l�����A�N�e�B�u�Ȃ��A�N�e�B�u�ɂ���
        /// </summary>
        /// <param name="panel">panel</param>
        private void ClosePanel(GameObject panel)
        {
            if (panel.activeSelf)
            {
                panel.SetActive(false);
            }
        }


#region SelectGame

        /// <summary>
        /// �I������Ă���X�e�[�W�̔ԍ���Ԃ�
        /// </summary>
        /// <returns>�X�e�[�W�̔ԍ�(�ŏ��l��0�A�ő�l�͑I�����ɂ���X�e�[�W��-1)</returns>
        public int GetCenteredPanel()
        {
            return stageSelect.CenteredPanel;
        }

        /// <summary>
        /// �X�e�[�W�I���Ń^�b�v�����Ƃ�
        /// </summary>
        /// <param name="stageNumber">�X�e�[�W�ԍ�</param>
        [com.llamagod.EnumAction(typeof(TextList.Stage))]
        public void OnTapStageImage(int stageNumber)
        {
            if (isDisableTapButton) return;

            TextList.Stage stage = (TextList.Stage)stageNumber;

            if (stage == currentStage)
            {
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes1);
                DisplayDialog();
            }
            else
            {
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Move);
                stageSelect.GoToPanel(stageNumber - 1);
            }
        }

        /// <summary>
        /// �X�e�[�W�Z���N�g�̃e�L�X�g�̏����X�V����
        /// </summary>
        public void UpdateStageSelectTexts()
        {
            stageNameText.text = TextListGenerater.instance.StageToStageNameText(currentStage);
            stageIntroductionText.text = TextListGenerater.instance.StageToStageIntroductionText(currentStage);
        }

        /// <summary>
        /// �X�e�[�W�ړ��_�C�A���O��\������
        /// </summary>
        public void DisplayDialog()
        {
            stageMoveDialogPanel.SetActive(true);
            dialogStageNameText.text = stageNameText.text;
        }

        /// <summary>
        /// �X�e�[�W�ړ��_�C�A���O��Yes�̂Ƃ�
        /// </summary>
        public void DialogYes()
        {
            string str = currentStage.ToString();
            int sceneIndex = SceneUtility.GetBuildIndexByScenePath("000_MyAssets/Scenes/" + str);
            if (sceneIndex >= 0)
            {
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes2);
                FadeManager.Instance.LoadScene(str, FADE_MANAGER_TIME);
            }
            else
            {
                // �V�[�������Z�b�e�B���O����Ă��Ȃ��Ƃ�
                Debug.Log($"MenuSceneManager.DialogYes : {str} is Nothing");
                DialogNo();
            }
        }

        /// <summary>
        /// �X�e�[�W�ړ��_�C�A���O��No�̂Ƃ�
        /// </summary>
        public void DialogNo()
        {
            SoundManager.Instance.PlaySE(SoundManager.SoundSeType.No1);
            stageMoveDialogPanel.SetActive(false);
        }

        /// <summary>
        /// ���R�̃{�^�����������Ƃ�(��ʈړ��̓A�Z�b�g�̕ʊ֐�)
        /// </summary>
        public void PushMoveButton()
        {
            SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Move);
        }

#endregion


#region Shop

        /// <summary>
        /// �����[�h�L��������̃p�l����\���E��\���ɂ���
        /// </summary>
        /// <param name="flg"></param>
        public void DisplayRewardShowPanel(bool flg)
        {
            shop_RewardShowPanel.SetActive(flg);

            if (!flg)
            {
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes1);
                rewardFlg = false;
            }
        }

        /// <summary>
        /// Reward�L������������{�^�����������Ƃ�
        /// </summary>
        public void PushShowRewardAd()
        {
            AdMobManager.Instance.ShowReward();
        }

        /// <summary>
        /// �����[�h�L�����ς���ɌĂ΂��
        /// </summary>
        /// <param name="flg">true : �L�����Ō�܂Ŋς� , false ; �L�����Ăяo����</param>
        public void FinishReward(bool flg)
        {
            if (rewardFlg) return;

            if (!shop_RewardShowPanel.activeSelf)
            {
                DisplayRewardShowPanel(true);
            }

            if (flg)
            {
                rewardFlg = true;

                PlayerInformationManager.Instance.AcquisitionSPCoin(GET_SP_COIN_NUM);
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes2);

                rewardShowText1.text = "��V���l�����܂���";
                rewardShowText2.text = "����SP�R�C�� : " + PlayerInformationManager.Instance.GetSPCoin().ToString();
            }
            else
            {
                rewardShowText1.text = "�L���̎����Ɏ��s���܂���";
                rewardShowText2.text = "����SP�R�C�� : " + PlayerInformationManager.Instance.GetSPCoin().ToString();
            }

            Debug.Log($"MenuSceneManager.FinishReward : {flg}");
        }

        /// <summary>
        /// �K�`���`�P�b�g�����̕\�����X�V����
        /// </summary>
        public void UpdateGachaTicketText()
        {
            haveGachaTicketText.text = $"�K�`���`�P�b�g : {PlayFabManager.Instance.GetHaveGachaTicket().ToString("N0")} ��";
        }

        /// <summary>
        /// �K�`���̃X�g�A�����擾����
        /// </summary>
        public void LoadStoreGacha()
        {
            if(PlayFabManager.Instance.CatalogItems == null)
            {
                PlayFabManager.Instance.GetGachaCatalogData();
            }
        }

        /// <summary>
        /// �K�`�����ʂ���M����
        /// </summary>
        /// <param name="list"></param>
        public void ReceptionGachaResult(List<string> list)
        {
            gachaGenerator.ReceptionGachaResultAndLotteryGachaAll(list);
        }

        /// <summary>
        /// �K�`�����s���󂯎�����Ƃ�
        /// </summary>
        /// <param name="str">�\������G���[���b�Z�[�W</param>
        public void ReceptionGachaFailure(string str)
        {
            gachaGenerator.ReceptionGachaFailure(str);
        }

        /// <summary>
        /// �P���K�`�������s����
        /// </summary>
        public void ShoppingGacha1()
        {
            if (waitingGachaResultFlg) return;

            waitingGachaResultFlg = true;
            PlayFabManager.Instance.PurchaseGacha("Gacha1", 1);
            StartCoroutine(GachaStaging());
        }

        /// <summary>
        /// 11�A�K�`�������s����
        /// </summary>
        public void ShoppingGacha11()
        {
            if (waitingGachaResultFlg) return;

            waitingGachaResultFlg = true;
            PlayFabManager.Instance.PurchaseGacha("Gacha11", 10);
            StartCoroutine(GachaStaging());
        }

        /// <summary>
        /// �K�`���̉��o
        /// </summary>
        /// <returns></returns>
        private IEnumerator GachaStaging()
        {
            gachaGenerator.DisplayGachaPanel(true);

            //while (waitingGachaResultFlg)
            //{
            //    yield return null;
            //}

            yield return null;
        }

        /// <summary>
        /// �K�`���̒��ӎ����{�^�����������Ƃ�
        /// </summary>
        public void PushGachaCautionButton()
        {
            GameObject canvas = GameObject.FindGameObjectWithTag("Point");

            GameObject obj = Instantiate(gachaCautionPanelPrefab, canvas.transform);

            obj.GetComponent<GachaCautionPanelPrefab>().SetMyText();
        }

        /// <summary>
        /// waitingGachaResultFlg��false�ɂ���
        /// </summary>
        public void SetFalseWaitingGachaResultFlg()
        {
            waitingGachaResultFlg = false;
        }

        /// <summary>
        /// �V���b�v�̓��e��\������Prefab�𐶐�����
        /// </summary>
        /// <param name="shopContents"></param>
        public void OpenShopContentsPanelPrefab(int shopContents)
        {
            GameObject canvas = GameObject.FindGameObjectWithTag("Point");

            GameObject obj = Instantiate(shopContentsPanelPrefab, canvas.transform);

            obj.GetComponent<ShopContentsPanelPrefab>().SetMyStatus((ShopContentsPanelPrefab.ShopContents)shopContents);

        }

        /// <summary>
        /// SNS�V�F�A�̃{�^�����������Ƃ�
        /// </summary>
        public void PushShareButton()
        {
            string url = "";
            //string image_path = "";
            string str1 = "�y�����������������_���Q�[���͂������H\n";

            //string str2 = "�����߂�ꂽ�������甲���o�������I�I\n";

            string str3 = "�����A�݂�Ȃ�����Ă݂悤�I�I\n";

            //string str5 = "#�E�o�Q�[�� ";

            string str6 = "#���_���Q�[���R���N�V���� ";

            string text = str1 + str3 + str6;

            bool flg = false;
            if (Application.platform == RuntimePlatform.Android)
            {
                url = "https://play.google.com/store/apps/details?id=com.DanchingStar.MedalGameCollection";
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
                PlayerInformationManager.Instance.AcquisitionSPCoin(GET_SP_COIN_NUM);
                SendInformationText($"SP�R�C����{GET_SP_COIN_NUM}��\n�l�����܂����B");
            }

        }

        /// <summary>
        /// �V�F�A�ς݂��ǂ����̃e�L�X�g��I��
        /// </summary>
        private void SelectShareText()
        {
            if (PlayerInformationManager.Instance.GetTodaySNSShareFlg())
            {
                snsShareText.text = "�����̓V�F�A�ς݂ł��B";
            }
            else
            {
                snsShareText.text = "�����͂܂��V�F�A���Ă��܂���B";
            }
        }

#endregion


#region Setting

        /// <summary>
        /// �v���t�B�[���摜�̐ݒ���������Ƃ�
        /// </summary>
        public void PushProfileEditButton()
        {
            SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes1);
            string str = "MakeSprite";
            FadeManager.Instance.LoadScene(str, FADE_MANAGER_TIME);
        }

        /// <summary>
        /// �f�o�b�O�{�^�����������Ƃ�
        /// </summary>
        public void PushDebugButton()
        {
            PlayerInformationManager.Instance.PushDebugButton();
        }

        /// <summary>
        /// �J�X�^���R�[�h��\������{�^�����������Ƃ�
        /// </summary>
        public void PushDisplayCustomCode()
        {
            SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes2);

            string customCode = PlayerPrefs.GetString(PlayFabManager.CUSTOM_ID_SAVE_KEY, "");
            GUIUtility.systemCopyBuffer = customCode;

            displayCustomCodeText1.text = customCode;
            displayCustomCodeText2.text = "����(���p��)�R�[�h���R�s�[���܂���";
        }

        /// <summary>
        /// �J�X�^���R�[�h��ʂ̃e�L�X�g������������
        /// </summary>
        public void InitDisplayCustomCodeTexts()
        {
            displayCustomCodeText1.text = "";
            displayCustomCodeText2.text = "";
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
            SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes2);
            rebootFlg = true;
            PlayFabManager.Instance.UpdateUserData();
        }

        /// <summary>
        /// �^�C�g���։�ʂ̃e�L�X�g������������
        /// </summary>
        public void InitToTitleTexts()
        {
            toTitleText.text = "";
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
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes2);

                string playerNameId = setting_NameEditInputField.text;

                PlayerInformationManager.Instance.SavePlayerName(playerNameId);

                setting_NameEditText.text = "�v���C���[����\n�ύX���܂����B";
            }
            else
            {
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.No2);
                setting_NameEditText.text = "�V�������O��������\n�������Ă��܂���B";
            }
        }

        /// <summary>
        /// �v���C���[����ύX��ʂ̃e�L�X�g������������
        /// </summary>
        public void InitNameEditTexts()
        {
            setting_NameEditText.text = "";
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
                PlayerInformationManager.Instance.DeleteSaveFile();
            }

            GameObject canvas = GameObject.FindGameObjectWithTag("Point");
            GameObject obj = Instantiate(deleteInformationPrefab, canvas.transform);
            obj.GetComponent<DeleteInformationPrefab>().SetMyStatus(isSuccess);
        }

        /// <summary>
        /// �f�[�^�폜��ɍċN������
        /// </summary>
        public void ReceptionAfterDelete()
        {
            StartCoroutine(RestartApplication());
        }


#endregion


#region Achievement

        /// <summary>
        /// �B���x�̐ݒ�̏�����
        /// </summary>
        private void AchievementInit()
        {
            achievement.InitAchievement();
        }

        /// <summary>
        /// �B���x���X�V����
        /// </summary>
        private void UpdateAchievement()
        {
            int getLevel = achievement.GetMyLevel();
            int getExp = achievement.GetOnlyExp();

            int nowLevel = PlayerInformationManager.Instance.GetPlayerLevel();
            int nowExp = PlayerInformationManager.Instance.GetPlayerExperience();

            if (getLevel != nowLevel || getExp != nowExp)
            {
                if (getLevel > nowLevel)
                {
                    int sa = getLevel - nowLevel;

                    PlayerInformationManager.Instance.AcquisitionSPCoin(sa);

                    SendInformationText($"���x����{sa}�オ��܂���!\nSP�R�C����{sa}���v���[���g!!");

                    UpdateHaveMedalsAndSPCoinsAndPlayerInformation();
                }

                PlayerInformationManager.Instance.UpdateLevelAndExp(getLevel, getExp);
            }
        }

        /// <summary>
        /// �Q�[�����Ƃ̒B���x���x���̕\�����X�V����
        /// </summary>
        private void UpdateAchievementOfGameKinds()
        {
            achievement.UpdateAchievementOfGameKinds();
        }

        /// <summary>
        /// Achievement�̃I�u�W�F�N�g��Ԃ�
        /// </summary>
        /// <returns></returns>
        public Achievement GetAchievementObject()
        {
            return achievement;
        }

#endregion


    }
}

