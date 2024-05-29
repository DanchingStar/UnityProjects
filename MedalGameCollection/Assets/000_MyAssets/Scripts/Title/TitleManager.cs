using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Title
{
    public class TitleManager : MonoBehaviour
    {
        [SerializeField] private GameObject touchEventPanel;
        [SerializeField] private GameObject newNamePanel;

        [SerializeField] private TextMeshProUGUI versionText;

        [SerializeField] private TMP_InputField inputNameTmp;
        [SerializeField] private InformationTextPanel informationTextPanel;

        [SerializeField] private MenuPanel menuPanel;

        private GameObject fadeManagerObject;

        private string playerNameId;

        private string beforeRepairCode;

        private TitleStatus titleStatus;

        private const string DEFAULT_FIRST_NAME = "新プレイヤー";

        public enum TitleStatus
        {
            BeforeTap,
            Menu,
            Repair,
            Reboot,
            LogIn,
            NameEdit,
            FailureLogIn,
            Finish,
        }

        public static TitleManager Instance;
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
            playerNameId = PlayerInformationManager.Instance.GetPlayerName();
            beforeRepairCode = "";
            versionText.text = $"Ver.{Application.version}";

            fadeManagerObject = FadeManager.Instance.gameObject;

            ChangeTitleStatus(TitleStatus.BeforeTap);
        }

        /// <summary>
        /// タイトル画面でタッチ用パネルを押したとき
        /// </summary>
        public void TouchTitle()
        {
            if (GetTitleStatus() != TitleStatus.BeforeTap) return;

            SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes);

            ChangeTitleStatus(TitleStatus.LogIn);

            PlayFabManager.Instance.Login();

        }

        /// <summary>
        /// ログインが完了した後に実行する
        /// </summary>
        public void AfterLogIn(bool newPlayerFlg)
        {
            Debug.Log($"TitleManager.AfterLogIn : newPlayerFlg = {newPlayerFlg}");

            // if (PlayerInformationManager.Instance.GetNewPlayerFlg())
            if (newPlayerFlg)
            {
                ChangeTitleStatus(TitleStatus.NameEdit);

                DisplayNewPlayerPanel(true);
            }
            else
            {
                //ChangeTitleStatus(TitleStatus.UpdateSaveFile);
                //PlayFabManager.Instance.UpdateUserData();

                ChangeTitleStatus(TitleStatus.Finish);
                MoveMenuScene();

                // Debug.Log($"TitleManager.TouchTitle : False");
            }
        }

        public void FailureLogIn()
        {
            ChangeTitleStatus(TitleStatus.FailureLogIn);
            StartCoroutine(RestartApplication(2f));
        }

        /// <summary>
        /// 復旧ログインの後に実行する
        /// </summary>
        public void AfterRepairLogin()
        {
            CloseAllMenuPanel();

            PlayFabManager.Instance.SaveNewCustomID(beforeRepairCode);

            ChangeTitleStatus(TitleStatus.Repair);
        }

        /// <summary>
        /// 復旧データのDLの後に実行する
        /// </summary>
        /// <param name="repeaiSaveData"></param>
        public async void AfterRepairSaveData(SaveData repeaiSaveData)
        {
            await PlayerInformationManager.Instance.SaveRepairJson(repeaiSaveData);

            StartCoroutine(RestartApplication(0f));
        }

        /// <summary>
        /// 復旧失敗時
        /// </summary>
        public void RepairFailure()
        {
            menuPanel.DisplayRepairFailureText(true);
        }

        /// <summary>
        /// ユーザーデータのアップロード完了した後に実行する
        /// </summary>
        public void AfterUpdateUserData()
        {
            //ChangeTitleStatus(TitleStatus.Finish);

            //MoveMenuScene();

            Debug.LogError($"TitleManager.AfterUpdateUserData");
        }

        /// <summary>
        /// newNamePanelを表示・非表示にする
        /// </summary>
        /// <param name="flg">true : 表示 , false : 非表示</param>
        public void DisplayNewPlayerPanel(bool flg)
        {
            newNamePanel.SetActive(flg);

            if (flg)
            {
                inputNameTmp.text = DEFAULT_FIRST_NAME;
            }
        }

        /// <summary>
        /// 名前入力完了のボタンを押したとき
        /// </summary>
        public void PushInputNameButton()
        {
            if (PlayFabManager.Instance.IsValidName(inputNameTmp))
            {
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes);

                playerNameId = inputNameTmp.text;

                PlayerInformationManager.Instance.SavePlayerName(playerNameId);

                DisplayNewPlayerPanel(false);

                AfterLogIn(false);
            }
            else
            {
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.No);
            }
        }

        /// <summary>
        /// メニューシーンへ移る
        /// </summary>
        public void MoveMenuScene()
        {

            FadeManager.Instance.LoadScene("Menu", 0.3f);
        }

        /// <summary>
        /// タイトル画面のメニュボタン(orメニュ画面から戻るボタン)を押したとき
        /// </summary>
        /// <param name="flg">true : メニュボタン , false : メニュ画面から戻るボタン </param>
        public void PushMenuButton(bool flg)
        {
            if (flg)
            {
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes);

                ChangeTitleStatus(TitleStatus.Menu);
            }
            else
            {
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.No);

                ChangeTitleStatus(TitleStatus.BeforeTap);
            }
        }

        /// <summary>
        /// 現在のステータス（モード）を変更する
        /// </summary>
        /// <param name="ts"></param>
        public void ChangeTitleStatus(TitleStatus ts)
        {
            titleStatus = ts;

            if (ts == TitleStatus.BeforeTap)
            {
                informationTextPanel.ActiveInformationTextPanel(false);
                informationTextPanel.SetInformationText("BeforeTap");
                if (menuPanel.GetActiveFlg())
                {
                    menuPanel.ActiveMenuPanel(false);
                }
            }
            else if (ts == TitleStatus.Menu)
            {
                informationTextPanel.ActiveInformationTextPanel(false);
                informationTextPanel.SetInformationText("Menu");
                menuPanel.ActiveMenuPanel(true);
            }
            else if (ts == TitleStatus.Repair)
            {
                informationTextPanel.ActiveInformationTextPanel(true);
                informationTextPanel.SetInformationText("データ復旧中");
            }
            else if (ts == TitleStatus.Reboot)
            {
                informationTextPanel.ActiveInformationTextPanel(true);
                informationTextPanel.SetInformationText("アプリを\n再起動中");
            }
            else if (ts == TitleStatus.LogIn)
            {
                informationTextPanel.ActiveInformationTextPanel(true);
                informationTextPanel.SetInformationText("ログイン中");
            }
            else if (ts == TitleStatus.NameEdit)
            {
                informationTextPanel.ActiveInformationTextPanel(false);
                informationTextPanel.SetInformationText("NameEdit");
            }
            else if (ts == TitleStatus.FailureLogIn)
            {
                informationTextPanel.ActiveInformationTextPanel(true);
                informationTextPanel.SetInformationText("ログインに失敗");
            }
            else if (ts == TitleStatus.Finish)
            {
                informationTextPanel.ActiveInformationTextPanel(true);
                informationTextPanel.SetInformationText("完了");
            }
        }

        /// <summary>
        /// TitleStatusの状態を返すゲッター
        /// </summary>
        /// <returns></returns>
        public TitleStatus GetTitleStatus()
        {
            return titleStatus;
        }

        /// <summary>
        /// Menuのデータ復旧を実行するボタンを押したとき
        /// </summary>
        public void PushRepairButton()
        {
            string code = menuPanel.GetRepairCode();

            if (beforeRepairCode == code)
            {
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.No);
                return;
            }

            SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes);

            beforeRepairCode = code;

            PlayFabManager.Instance.RepairLogin(code);
        }

        /// <summary>
        /// 全てのMenuのパネルを閉じる
        /// </summary>
        public void CloseAllMenuPanel()
        {
            menuPanel.CloseAllPanel();
        }

        /// <summary>
        /// アプリを再起動する
        /// </summary>
        /// <param name="delayTime">再起動までの待機時間</param>
        /// <returns></returns>
        public IEnumerator RestartApplication(float delayTime)
        {
            yield return new WaitForSeconds(delayTime);

            ChangeTitleStatus(TitleStatus.Reboot);

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
    }

}
