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

        private const string DEFAULT_FIRST_NAME = "�V�v���C���[";

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
        /// �^�C�g����ʂŃ^�b�`�p�p�l�����������Ƃ�
        /// </summary>
        public void TouchTitle()
        {
            if (GetTitleStatus() != TitleStatus.BeforeTap) return;

            SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes);

            ChangeTitleStatus(TitleStatus.LogIn);

            PlayFabManager.Instance.Login();

        }

        /// <summary>
        /// ���O�C��������������Ɏ��s����
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
        /// �������O�C���̌�Ɏ��s����
        /// </summary>
        public void AfterRepairLogin()
        {
            CloseAllMenuPanel();

            PlayFabManager.Instance.SaveNewCustomID(beforeRepairCode);

            ChangeTitleStatus(TitleStatus.Repair);
        }

        /// <summary>
        /// �����f�[�^��DL�̌�Ɏ��s����
        /// </summary>
        /// <param name="repeaiSaveData"></param>
        public async void AfterRepairSaveData(SaveData repeaiSaveData)
        {
            await PlayerInformationManager.Instance.SaveRepairJson(repeaiSaveData);

            StartCoroutine(RestartApplication(0f));
        }

        /// <summary>
        /// �������s��
        /// </summary>
        public void RepairFailure()
        {
            menuPanel.DisplayRepairFailureText(true);
        }

        /// <summary>
        /// ���[�U�[�f�[�^�̃A�b�v���[�h����������Ɏ��s����
        /// </summary>
        public void AfterUpdateUserData()
        {
            //ChangeTitleStatus(TitleStatus.Finish);

            //MoveMenuScene();

            Debug.LogError($"TitleManager.AfterUpdateUserData");
        }

        /// <summary>
        /// newNamePanel��\���E��\���ɂ���
        /// </summary>
        /// <param name="flg">true : �\�� , false : ��\��</param>
        public void DisplayNewPlayerPanel(bool flg)
        {
            newNamePanel.SetActive(flg);

            if (flg)
            {
                inputNameTmp.text = DEFAULT_FIRST_NAME;
            }
        }

        /// <summary>
        /// ���O���͊����̃{�^�����������Ƃ�
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
        /// ���j���[�V�[���ֈڂ�
        /// </summary>
        public void MoveMenuScene()
        {

            FadeManager.Instance.LoadScene("Menu", 0.3f);
        }

        /// <summary>
        /// �^�C�g����ʂ̃��j���{�^��(or���j����ʂ���߂�{�^��)���������Ƃ�
        /// </summary>
        /// <param name="flg">true : ���j���{�^�� , false : ���j����ʂ���߂�{�^�� </param>
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
        /// ���݂̃X�e�[�^�X�i���[�h�j��ύX����
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
                informationTextPanel.SetInformationText("�f�[�^������");
            }
            else if (ts == TitleStatus.Reboot)
            {
                informationTextPanel.ActiveInformationTextPanel(true);
                informationTextPanel.SetInformationText("�A�v����\n�ċN����");
            }
            else if (ts == TitleStatus.LogIn)
            {
                informationTextPanel.ActiveInformationTextPanel(true);
                informationTextPanel.SetInformationText("���O�C����");
            }
            else if (ts == TitleStatus.NameEdit)
            {
                informationTextPanel.ActiveInformationTextPanel(false);
                informationTextPanel.SetInformationText("NameEdit");
            }
            else if (ts == TitleStatus.FailureLogIn)
            {
                informationTextPanel.ActiveInformationTextPanel(true);
                informationTextPanel.SetInformationText("���O�C���Ɏ��s");
            }
            else if (ts == TitleStatus.Finish)
            {
                informationTextPanel.ActiveInformationTextPanel(true);
                informationTextPanel.SetInformationText("����");
            }
        }

        /// <summary>
        /// TitleStatus�̏�Ԃ�Ԃ��Q�b�^�[
        /// </summary>
        /// <returns></returns>
        public TitleStatus GetTitleStatus()
        {
            return titleStatus;
        }

        /// <summary>
        /// Menu�̃f�[�^���������s����{�^�����������Ƃ�
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
        /// �S�Ă�Menu�̃p�l�������
        /// </summary>
        public void CloseAllMenuPanel()
        {
            menuPanel.CloseAllPanel();
        }

        /// <summary>
        /// �A�v�����ċN������
        /// </summary>
        /// <param name="delayTime">�ċN���܂ł̑ҋ@����</param>
        /// <returns></returns>
        public IEnumerator RestartApplication(float delayTime)
        {
            yield return new WaitForSeconds(delayTime);

            ChangeTitleStatus(TitleStatus.Reboot);

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
    }

}
