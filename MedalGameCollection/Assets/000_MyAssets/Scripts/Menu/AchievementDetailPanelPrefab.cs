using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    public class AchievementDetailPanelPrefab : MonoBehaviour
    {
        private bool isCommonDetailFlg;
        private int myGameNumber;

        [SerializeField] private TextMeshProUGUI titleTexts;

        [SerializeField] private GameObject[] missionFields;
        [SerializeField] private GameObject loginField;
        [SerializeField] private GameObject initField;
        [SerializeField] private TextMeshProUGUI totalNumberText;
        [SerializeField] private TextMeshProUGUI totalExpText;

        private TextMeshProUGUI[] missionTexts;
        private TextMeshProUGUI[] levelTexts;
        private Button[] informationButtons;

        private const int MISSION_NUM = 3;
        private const int MAX_LEVEL = 10;

        [SerializeField] private GameObject informationPanel;
        [SerializeField] private TextMeshProUGUI informationTitleText;
        [SerializeField] private TextMeshProUGUI informationMissionNameText;
        [SerializeField] private TextMeshProUGUI informationListText;
        [SerializeField] private TextMeshProUGUI informationNowScoreText;
        [SerializeField] private TextMeshProUGUI informationNowLevelText;

        /// <summary>
        /// �����̏����Z�b�g����(�Q�[���p)
        /// </summary>
        public void SettingMyStatusForGameDetail(int gameNumber, int[] scores, int[] levels)
        {
            isCommonDetailFlg = false;
            myGameNumber = gameNumber;

            InitObjects();

            InputValueForGame(scores, levels);
        }

        /// <summary>
        /// �����̏����Z�b�g����(���ʗp)
        /// </summary>
        public void SettingMyStatusForCommonDetail()
        {
            isCommonDetailFlg = true;
            myGameNumber = -1;

            InitObjects();

            InputValueForCommon();
        }

        /// <summary>
        /// �I�u�W�F�N�g�̏�����
        /// </summary>
        private void InitObjects()
        {
            ResetInformationPanel();

            if (isCommonDetailFlg)
            {
                foreach (var item in missionFields)
                {
                    item.SetActive(false);
                }
                loginField.SetActive(true);
                initField.SetActive(true);

                levelTexts = new TextMeshProUGUI[1];
                levelTexts[0]= loginField.transform.Find("LevelText").gameObject.GetComponent<TextMeshProUGUI>();
            }
            else
            {
                missionTexts = new TextMeshProUGUI[MISSION_NUM];
                levelTexts = new TextMeshProUGUI[MISSION_NUM];
                informationButtons = new Button[MISSION_NUM];

                for (int i = 0; i < MISSION_NUM; i++) 
                {
                    missionFields[i].SetActive(true);
                    missionTexts[i] = missionFields[i].transform.Find("MissionText").gameObject.GetComponent<TextMeshProUGUI>();
                    levelTexts[i] = missionFields[i].transform.Find("LevelText").gameObject.GetComponent<TextMeshProUGUI>();
                    informationButtons[i] = missionFields[i].transform.Find("InformationButton").gameObject.GetComponent<Button>();
                }
                loginField.SetActive(false);
                initField.SetActive(false);
            }
        }

        /// <summary>
        /// �K�v�Ȓl��������(�Q�[���p)
        /// </summary>
        /// <param name="scores"></param>
        /// <param name="levels"></param>
        private void InputValueForGame(int[] scores, int[] levels)
        {
            bool errorFlg = false;

            switch (myGameNumber)
            {
                case 0:
                    for (int i = 0; i < MISSION_NUM; i++)
                    {
                        switch (i)
                        {
                            case 0: missionTexts[i].text = "�{�[���𔭎˂�����"; break;
                            case 1: missionTexts[i].text = "�l�����_������"; break;
                            case 2: missionTexts[i].text = "�p�[�t�F�N�g�B����"; break;
                            default: errorFlg = true; break;
                        }
                        levelTexts[i].text = levels[i].ToString();
                        int num = i;
                        informationButtons[i].onClick.AddListener(() => PushInformationButton(scores[num], levels[num], num));
                    }
                    break;
                case 1:
                    for (int i = 0; i < MISSION_NUM; i++)
                    {
                        switch (i)
                        {
                            case 0: missionTexts[i].text = "�o�g��������"; break;
                            case 1: missionTexts[i].text = "�l�����_������"; break;
                            case 2: missionTexts[i].text = "������"; break;
                            default: errorFlg = true; break;
                        }
                        levelTexts[i].text = levels[i].ToString();
                        int num = i;
                        informationButtons[i].onClick.AddListener(() => PushInformationButton(scores[num], levels[num], num));
                    }
                    break;
                case 2:
                    for (int i = 0; i < MISSION_NUM; i++)
                    {
                        switch (i)
                        {
                            case 0: missionTexts[i].text = "�{�[���𔭎˂�����"; break;
                            case 1: missionTexts[i].text = "�l�����_������"; break;
                            case 2: missionTexts[i].text = "�哖�����"; break;
                            default: errorFlg = true; break;
                        }
                        levelTexts[i].text = levels[i].ToString();
                        int num = i;
                        informationButtons[i].onClick.AddListener(() => PushInformationButton(scores[num], levels[num], num));
                    }
                    break;
                case 3:
                    for (int i = 0; i < MISSION_NUM; i++)
                    {
                        switch (i)
                        {
                            case 0: missionTexts[i].text = "�g�������_���̖���"; break;
                            case 1: missionTexts[i].text = "�l�����_������"; break;
                            case 2: missionTexts[i].text = "�ő�BINGO���C����"; break;
                            default: errorFlg = true; break;
                        }
                        levelTexts[i].text = levels[i].ToString();
                        int num = i;
                        informationButtons[i].onClick.AddListener(() => PushInformationButton(scores[num], levels[num], num));
                    }
                    break;
                default:
                    errorFlg = true;
                    break;
            }

            if (errorFlg)
            {
                Debug.LogError("AchievementDetailPanelPrefab.InputValueForGame : Error");
                return;
            }

            int totalLevel = 0;
            foreach (int i in levels)
            {
                totalLevel += i;
            }
            totalNumberText.text = $"���v : {totalLevel}";
            totalExpText.text = $"�o���l(���v�~30) : {totalLevel*30} Exp";
            titleTexts.text = TextListGenerater.instance.StageToStageNameText((TextList.Stage)(myGameNumber + 1));
        }

        /// <summary>
        /// �K�v�Ȓl��������(���ʗp)
        /// </summary>
        private void InputValueForCommon()
        {
            int loginDay = PlayerInformationManager.Instance.GetLoginDays();
            levelTexts[0].text = loginDay.ToString();
            totalNumberText.text = $"���O�C���o���l(���O�C�������~10)\n �� {loginDay*10} Exp";
            totalExpText.text = $"���v�o���l(���O�C�� + ����)\n �� {loginDay*10+100} Exp";
            titleTexts.text = "����";
        }

        /// <summary>
        /// �ڍ׃{�^�����������Ƃ�
        /// </summary>
        /// <param name="score"></param>
        /// <param name="level"></param>
        /// <param name="kind"></param>
        private void PushInformationButton(int score, int level, int kind)
        {
            SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes1);

            Achievement achievement = MenuSceneManager.Instance.GetAchievementObject();
            int[] array = new int[MAX_LEVEL];
            array = achievement.GetTargetScoreArray(myGameNumber, kind);

            informationTitleText.text = titleTexts.text;
            informationMissionNameText.text = missionTexts[kind].text;
            informationListText.text = "";
            informationNowScoreText.text = $"���݂̃X�R�A : {score}";
            informationNowLevelText.text = $"���݂̃��x�� : {level}";
            informationPanel.SetActive(true);

            for(int i=0;i<MAX_LEVEL;i++)
            {
                string str = (i + 1).ToString().PadLeft(2) + " : " + array[i] + "\n";
                informationListText.text += str;
            }
        }

        /// <summary>
        /// Close�{�^�����������Ƃ�
        /// </summary>
        public void PushCloseButton()
        {
            SoundManager.Instance.PlaySE(SoundManager.SoundSeType.No1);

            Destroy(this.gameObject);
        }

        /// <summary>
        /// �ڍ׃p�l���̕�����������Ƃ�
        /// </summary>
        public void PushInformationPanelCloseButton()
        {
            SoundManager.Instance.PlaySE(SoundManager.SoundSeType.No1);

            ResetInformationPanel();
        }

        /// <summary>
        /// �ڍ׃p�l���̒��g��S�ă��Z�b�g����
        /// </summary>
        private void ResetInformationPanel()
        {
            informationPanel.SetActive(false);
            informationTitleText.text = "";
            informationMissionNameText.text = "";
            informationListText.text = "";
            informationNowScoreText.text = "";
            informationNowLevelText.text = "";
        }

    }
}
