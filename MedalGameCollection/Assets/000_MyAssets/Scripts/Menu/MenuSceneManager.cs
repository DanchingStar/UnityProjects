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
        /// メニューの内容たち
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

            if (PlayFabManager.Instance.GetIsLogined()) //ログイン状況を確認
            {
                AfterCheckPlayFabLogin(true);
            }
            else
            {
                PlayFabManager.Instance.Login(); //ログインする
            }
        }

        private void Update()
        {
            UpdateForContents();
        }

        /// <summary>
        /// Testボタンの処理(デバッグ用)
        /// </summary>
        public void TestButton()
        {
            Debug.Log($"currentContents : {currentContents}");
            // Debug.Log($"NumberOfPanels : {stageSelect.NumberOfPanels}");
        }

        /// <summary>
        /// 文字列をカットインに表示する(テスト用)
        /// </summary>
        /// <param name="text"></param>
        public void PushTestString(string text)
        {
            SendInformationText(text);
        }

        /// <summary>
        /// Startで呼び出される初期化
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
        /// 表示に合わせてUpdateの内容を変える
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
        /// メニューのボタンを押したとき
        /// </summary>
        /// <param name="toContent">押したボタンの内容</param>
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
        /// Panelの表示を入れ替える演出をする
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
        /// 戻るボタンを押したとき
        /// </summary>
        public void OnTapBackButton()
        {
            if (isDisableTapButton) return;

            SoundManager.Instance.PlaySE(SoundManager.SoundSeType.No1);
            StartCoroutine(ChangeDisplayContentsBack());
        }

        /// <summary>
        /// Panelの表示を前のPanelに戻る演出をする
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
        /// MenuContentsText(戻るボタンの横の文字列)を更新する
        /// </summary>
        private void UpdateMenuContentsText()
        {
            switch (currentContents)
            {
                case MenuContents.TopMenu:
                    menuContentsText.text = "メニュー";
                    break;
                case MenuContents.Game:
                    menuContentsText.text = "ゲーム";
                    break;
                case MenuContents.Shop:
                    menuContentsText.text = "ショップ";
                    break;
                case MenuContents.Achievement:
                    menuContentsText.text = "達成度";
                    break;
                case MenuContents.Friend:
                    menuContentsText.text = "フレンド";
                    break;
                case MenuContents.Setting:
                    menuContentsText.text = "設定";
                    break;
                case MenuContents.Shop_Exchange:
                    menuContentsText.text = "交換所";
                    break;
                case MenuContents.Shop_Gacha:
                    menuContentsText.text = "ガチャ";
                    break;
                case MenuContents.Shop_Advertisement:
                    menuContentsText.text = "広告を見る";
                    break;
                case MenuContents.Shop_SNS_Share:
                    menuContentsText.text = "SNSでシェア";
                    break;
                case MenuContents.Setting_Volume:
                    menuContentsText.text = "音量";
                    break;
                case MenuContents.Setting_Profile:
                    menuContentsText.text = "プロフィール画像";
                    break;
                case MenuContents.Setting_PlayerNameEdit:
                    menuContentsText.text = "プレイヤー名の変更";
                    break;
                case MenuContents.Setting_Credit:
                    menuContentsText.text = "クレジット";
                    break;
                case MenuContents.Setting_CustomCode:
                    menuContentsText.text = "復旧(引継ぎ)コード";
                    break;
                case MenuContents.Setting_ToTitle:
                    menuContentsText.text = "タイトルへ";
                    break;
                case MenuContents.Setting_Delete:
                    menuContentsText.text = "データ削除";
                    break;
                case MenuContents.Setting_DeleteFinal:
                    menuContentsText.text = "データ削除(最終確認)";
                    break;
                default:
                    Debug.LogError($"MenuSceneManager.UpdateMenuContentsText : currentContents is None");
                    break;
            }
        }

        /// <summary>
        /// 戻るボタンの表示非表示を更新する
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
        /// 持っているメダルとSPコインとプレイヤー名のテキストを更新する
        /// </summary>
        public void UpdateHaveMedalsAndSPCoinsAndPlayerInformation()
        {
            myPlayerInformation.UpdateDisplayInformationText();
            haveMedalsText.text = $"メダル : {PlayerInformationManager.Instance.GetHaveMedal().ToString("N0")} 枚";
            haveSPCoinsText.text = $"SPコイン : {PlayerInformationManager.Instance.GetSPCoin().ToString("N0")} 枚";
        }

        /// <summary>
        /// PlayFabManagerからセーブの更新が成功/失敗したことを受信したとき
        /// </summary>
        /// <param name="isSuccess">true : 成功 , false : 失敗</param>
        public void ReceptionUpdatePlayFab(bool isSuccess)
        {
            if (isSuccess)
            {
                if (rebootFlg)
                {
                    toTitleText.text = "データをサーバに保存しました。\nタイトルへ戻ります。";
                    StartCoroutine(RestartApplication());
                }
                else
                {
                    SendInformationText("サーバにデータを\n保存しました");

                }
            }
            else
            {
                if (rebootFlg)
                {
                    rebootFlg = false;
                    toTitleText.text = "データをサーバに保存できませんでした。\nタイトルへ戻るのを自重します。";
                }
                else
                {
                    SendInformationText("サーバにデータを\n保存できませんでした");
                }
            }
        }

        /// <summary>
        /// カットインで表示したいテキストを送る
        /// </summary>
        /// <param name="text"></param>
        public void SendInformationText(string text)
        {
            informationCutInPanel.AddInformationText(text);
        }

        /// <summary>
        /// ログイン成功をチェックした後に実行
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
        /// ログインの日付をチェックする
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
        /// ログインボーナスを付与する
        /// </summary>
        private void LoginBonus()
        {
            GameObject canvas = GameObject.FindGameObjectWithTag("Point");

            GameObject obj = Instantiate(loginBonusPanelPrefab, canvas.transform);

            obj.GetComponent<LoginBonusPanelPrefab>().SetMyStatus();
        }

        /// <summary>
        /// PlayFabのデータを更新する
        /// </summary>
        /// <returns></returns>
        private IEnumerator UpdatePlayFabData()
        {
            PlayFabManager.Instance.UpdateUserData();

            yield return null;
        }

        /// <summary>
        /// メイン以外のアクティブなパネルを全て非アクティブにする(Startで呼ぶ)
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
        /// パネルがアクティブなら非アクティブにする
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
        /// 選択されているステージの番号を返す
        /// </summary>
        /// <returns>ステージの番号(最小値は0、最大値は選択肢にあるステージ数-1)</returns>
        public int GetCenteredPanel()
        {
            return stageSelect.CenteredPanel;
        }

        /// <summary>
        /// ステージ選択でタップしたとき
        /// </summary>
        /// <param name="stageNumber">ステージ番号</param>
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
        /// ステージセレクトのテキストの情報を更新する
        /// </summary>
        public void UpdateStageSelectTexts()
        {
            stageNameText.text = TextListGenerater.instance.StageToStageNameText(currentStage);
            stageIntroductionText.text = TextListGenerater.instance.StageToStageIntroductionText(currentStage);
        }

        /// <summary>
        /// ステージ移動ダイアログを表示する
        /// </summary>
        public void DisplayDialog()
        {
            stageMoveDialogPanel.SetActive(true);
            dialogStageNameText.text = stageNameText.text;
        }

        /// <summary>
        /// ステージ移動ダイアログでYesのとき
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
                // シーン名がセッティングされていないとき
                Debug.Log($"MenuSceneManager.DialogYes : {str} is Nothing");
                DialogNo();
            }
        }

        /// <summary>
        /// ステージ移動ダイアログでNoのとき
        /// </summary>
        public void DialogNo()
        {
            SoundManager.Instance.PlaySE(SoundManager.SoundSeType.No1);
            stageMoveDialogPanel.SetActive(false);
        }

        /// <summary>
        /// ヨコのボタンを押したとき(画面移動はアセットの別関数)
        /// </summary>
        public void PushMoveButton()
        {
            SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Move);
        }

#endregion


#region Shop

        /// <summary>
        /// リワード広告視聴後のパネルを表示・非表示にする
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
        /// Reward広告を視聴するボタンを押したとき
        /// </summary>
        public void PushShowRewardAd()
        {
            AdMobManager.Instance.ShowReward();
        }

        /// <summary>
        /// リワード広告を観た後に呼ばれる
        /// </summary>
        /// <param name="flg">true : 広告を最後まで観た , false ; 広告を呼び出した</param>
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

                rewardShowText1.text = "報酬を獲得しました";
                rewardShowText2.text = "所持SPコイン : " + PlayerInformationManager.Instance.GetSPCoin().ToString();
            }
            else
            {
                rewardShowText1.text = "広告の視聴に失敗しました";
                rewardShowText2.text = "所持SPコイン : " + PlayerInformationManager.Instance.GetSPCoin().ToString();
            }

            Debug.Log($"MenuSceneManager.FinishReward : {flg}");
        }

        /// <summary>
        /// ガチャチケット枚数の表示を更新する
        /// </summary>
        public void UpdateGachaTicketText()
        {
            haveGachaTicketText.text = $"ガチャチケット : {PlayFabManager.Instance.GetHaveGachaTicket().ToString("N0")} 枚";
        }

        /// <summary>
        /// ガチャのストア情報を取得する
        /// </summary>
        public void LoadStoreGacha()
        {
            if(PlayFabManager.Instance.CatalogItems == null)
            {
                PlayFabManager.Instance.GetGachaCatalogData();
            }
        }

        /// <summary>
        /// ガチャ結果を受信する
        /// </summary>
        /// <param name="list"></param>
        public void ReceptionGachaResult(List<string> list)
        {
            gachaGenerator.ReceptionGachaResultAndLotteryGachaAll(list);
        }

        /// <summary>
        /// ガチャ失敗を受け取ったとき
        /// </summary>
        /// <param name="str">表示するエラーメッセージ</param>
        public void ReceptionGachaFailure(string str)
        {
            gachaGenerator.ReceptionGachaFailure(str);
        }

        /// <summary>
        /// 単発ガチャを実行する
        /// </summary>
        public void ShoppingGacha1()
        {
            if (waitingGachaResultFlg) return;

            waitingGachaResultFlg = true;
            PlayFabManager.Instance.PurchaseGacha("Gacha1", 1);
            StartCoroutine(GachaStaging());
        }

        /// <summary>
        /// 11連ガチャを実行する
        /// </summary>
        public void ShoppingGacha11()
        {
            if (waitingGachaResultFlg) return;

            waitingGachaResultFlg = true;
            PlayFabManager.Instance.PurchaseGacha("Gacha11", 10);
            StartCoroutine(GachaStaging());
        }

        /// <summary>
        /// ガチャの演出
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
        /// ガチャの注意事項ボタンを押したとき
        /// </summary>
        public void PushGachaCautionButton()
        {
            GameObject canvas = GameObject.FindGameObjectWithTag("Point");

            GameObject obj = Instantiate(gachaCautionPanelPrefab, canvas.transform);

            obj.GetComponent<GachaCautionPanelPrefab>().SetMyText();
        }

        /// <summary>
        /// waitingGachaResultFlgをfalseにする
        /// </summary>
        public void SetFalseWaitingGachaResultFlg()
        {
            waitingGachaResultFlg = false;
        }

        /// <summary>
        /// ショップの内容を表示するPrefabを生成する
        /// </summary>
        /// <param name="shopContents"></param>
        public void OpenShopContentsPanelPrefab(int shopContents)
        {
            GameObject canvas = GameObject.FindGameObjectWithTag("Point");

            GameObject obj = Instantiate(shopContentsPanelPrefab, canvas.transform);

            obj.GetComponent<ShopContentsPanelPrefab>().SetMyStatus((ShopContentsPanelPrefab.ShopContents)shopContents);

        }

        /// <summary>
        /// SNSシェアのボタンを押したとき
        /// </summary>
        public void PushShareButton()
        {
            string url = "";
            //string image_path = "";
            string str1 = "楽しくも懐かしいメダルゲームはいかが？\n";

            //string str2 = "閉じ込められた部屋から抜け出したぞ！！\n";

            string str3 = "さあ、みんなもやってみよう！！\n";

            //string str5 = "#脱出ゲーム ";

            string str6 = "#メダルゲームコレクション ";

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
                //text = text + "その他の機種\n";
                Debug.Log("Other OS");
            }

            SocialConnector.SocialConnector.Share(text, url); //第1引数:テキスト,第2引数:URL,第3引数:画像

            if (PlayerInformationManager.Instance.GetTodaySNSShareFlg() == false && flg == true)
            {
                PlayerInformationManager.Instance.UpdateAndSetTodaySNSShareFlgTrue();
                PlayerInformationManager.Instance.AcquisitionSPCoin(GET_SP_COIN_NUM);
                SendInformationText($"SPコインを{GET_SP_COIN_NUM}枚\n獲得しました。");
            }

        }

        /// <summary>
        /// シェア済みかどうかのテキストを選ぶ
        /// </summary>
        private void SelectShareText()
        {
            if (PlayerInformationManager.Instance.GetTodaySNSShareFlg())
            {
                snsShareText.text = "今日はシェア済みです。";
            }
            else
            {
                snsShareText.text = "今日はまだシェアしていません。";
            }
        }

#endregion


#region Setting

        /// <summary>
        /// プロフィール画像の設定を押したとき
        /// </summary>
        public void PushProfileEditButton()
        {
            SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes1);
            string str = "MakeSprite";
            FadeManager.Instance.LoadScene(str, FADE_MANAGER_TIME);
        }

        /// <summary>
        /// デバッグボタンを押したとき
        /// </summary>
        public void PushDebugButton()
        {
            PlayerInformationManager.Instance.PushDebugButton();
        }

        /// <summary>
        /// カスタムコードを表示するボタンを押したとき
        /// </summary>
        public void PushDisplayCustomCode()
        {
            SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes2);

            string customCode = PlayerPrefs.GetString(PlayFabManager.CUSTOM_ID_SAVE_KEY, "");
            GUIUtility.systemCopyBuffer = customCode;

            displayCustomCodeText1.text = customCode;
            displayCustomCodeText2.text = "復旧(引継ぎ)コードをコピーしました";
        }

        /// <summary>
        /// カスタムコード画面のテキストを初期化する
        /// </summary>
        public void InitDisplayCustomCodeTexts()
        {
            displayCustomCodeText1.text = "";
            displayCustomCodeText2.text = "";
        }

        /// <summary>
        /// クレジットテキストを読み込んで、文字列に代入する
        /// </summary>
        public void SetCreditText()
        {
            string resourcePath = "Credit/CreditText";

            TextAsset textAsset = Resources.Load<TextAsset>(resourcePath);

            string fileContents = textAsset.text;

            setting_CreditText.text = fileContents;
        }

        /// <summary>
        /// タイトルへボタンを押したとき
        /// </summary>
        public void PushToTitle()
        {
            SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes2);
            rebootFlg = true;
            PlayFabManager.Instance.UpdateUserData();
        }

        /// <summary>
        /// タイトルへ画面のテキストを初期化する
        /// </summary>
        public void InitToTitleTexts()
        {
            toTitleText.text = "";
        }

        /// <summary>
        /// アプリを再起動する
        /// </summary>
        public IEnumerator RestartApplication()
        {
            // DontDestroyOnLoadで守られているオブジェクトを破棄する
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

            // 初めからスタートするシーンを読み込む
            FadeManager.Instance.LoadScene("Title", 0.5f);
        }

        /// <summary>
        /// プレイヤー名入力完了のボタンを押したとき
        /// </summary>
        public void PushNameUpdateButton()
        {
            if (PlayFabManager.Instance.IsValidName(setting_NameEditInputField))
            {
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes2);

                string playerNameId = setting_NameEditInputField.text;

                PlayerInformationManager.Instance.SavePlayerName(playerNameId);

                setting_NameEditText.text = "プレイヤー名を\n変更しました。";
            }
            else
            {
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.No2);
                setting_NameEditText.text = "新しい名前が条件を\n満たしていません。";
            }
        }

        /// <summary>
        /// プレイヤー名を変更画面のテキストを初期化する
        /// </summary>
        public void InitNameEditTexts()
        {
            setting_NameEditText.text = "";
            setting_NameEditInputField.text = PlayerInformationManager.Instance.GetPlayerName();
        }

        /// <summary>
        /// データ削除ボタンを押したとき
        /// </summary>
        public void PushDeleteButton()
        {
            PlayFabManager.Instance.DeletePlayerData();
        }

        /// <summary>
        /// PlayFabからデータ削除の結果を受信する
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
        /// データ削除後に再起動する
        /// </summary>
        public void ReceptionAfterDelete()
        {
            StartCoroutine(RestartApplication());
        }


#endregion


#region Achievement

        /// <summary>
        /// 達成度の設定の初期化
        /// </summary>
        private void AchievementInit()
        {
            achievement.InitAchievement();
        }

        /// <summary>
        /// 達成度を更新する
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

                    SendInformationText($"レベルが{sa}上がりました!\nSPコインを{sa}枚プレゼント!!");

                    UpdateHaveMedalsAndSPCoinsAndPlayerInformation();
                }

                PlayerInformationManager.Instance.UpdateLevelAndExp(getLevel, getExp);
            }
        }

        /// <summary>
        /// ゲームごとの達成度レベルの表示を更新する
        /// </summary>
        private void UpdateAchievementOfGameKinds()
        {
            achievement.UpdateAchievementOfGameKinds();
        }

        /// <summary>
        /// Achievementのオブジェクトを返す
        /// </summary>
        /// <returns></returns>
        public Achievement GetAchievementObject()
        {
            return achievement;
        }

#endregion


    }
}

