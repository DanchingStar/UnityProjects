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
        /// 自分の情報をセットする(ゲーム用)
        /// </summary>
        public void SettingMyStatusForGameDetail(int gameNumber, int[] scores, int[] levels)
        {
            isCommonDetailFlg = false;
            myGameNumber = gameNumber;

            InitObjects();

            InputValueForGame(scores, levels);
        }

        /// <summary>
        /// 自分の情報をセットする(共通用)
        /// </summary>
        public void SettingMyStatusForCommonDetail()
        {
            isCommonDetailFlg = true;
            myGameNumber = -1;

            InitObjects();

            InputValueForCommon();
        }

        /// <summary>
        /// オブジェクトの初期化
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
        /// 必要な値を代入する(ゲーム用)
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
                            case 0: missionTexts[i].text = "ボールを発射した回数"; break;
                            case 1: missionTexts[i].text = "獲得メダル枚数"; break;
                            case 2: missionTexts[i].text = "パーフェクト達成回数"; break;
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
                            case 0: missionTexts[i].text = "バトルした回数"; break;
                            case 1: missionTexts[i].text = "獲得メダル枚数"; break;
                            case 2: missionTexts[i].text = "勝利回数"; break;
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
                            case 0: missionTexts[i].text = "ボールを発射した回数"; break;
                            case 1: missionTexts[i].text = "獲得メダル枚数"; break;
                            case 2: missionTexts[i].text = "大当たり回数"; break;
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
                            case 0: missionTexts[i].text = "使ったメダルの枚数"; break;
                            case 1: missionTexts[i].text = "獲得メダル枚数"; break;
                            case 2: missionTexts[i].text = "最大BINGOライン数"; break;
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
            totalNumberText.text = $"合計 : {totalLevel}";
            totalExpText.text = $"経験値(合計×30) : {totalLevel*30} Exp";
            titleTexts.text = TextListGenerater.instance.StageToStageNameText((TextList.Stage)(myGameNumber + 1));
        }

        /// <summary>
        /// 必要な値を代入する(共通用)
        /// </summary>
        private void InputValueForCommon()
        {
            int loginDay = PlayerInformationManager.Instance.GetLoginDays();
            levelTexts[0].text = loginDay.ToString();
            totalNumberText.text = $"ログイン経験値(ログイン日数×10)\n → {loginDay*10} Exp";
            totalExpText.text = $"合計経験値(ログイン + 初期)\n → {loginDay*10+100} Exp";
            titleTexts.text = "共通";
        }

        /// <summary>
        /// 詳細ボタンを押したとき
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
            informationNowScoreText.text = $"現在のスコア : {score}";
            informationNowLevelText.text = $"現在のレベル : {level}";
            informationPanel.SetActive(true);

            for(int i=0;i<MAX_LEVEL;i++)
            {
                string str = (i + 1).ToString().PadLeft(2) + " : " + array[i] + "\n";
                informationListText.text += str;
            }
        }

        /// <summary>
        /// Closeボタンを押したとき
        /// </summary>
        public void PushCloseButton()
        {
            SoundManager.Instance.PlaySE(SoundManager.SoundSeType.No1);

            Destroy(this.gameObject);
        }

        /// <summary>
        /// 詳細パネルの閉じるを押したとき
        /// </summary>
        public void PushInformationPanelCloseButton()
        {
            SoundManager.Instance.PlaySE(SoundManager.SoundSeType.No1);

            ResetInformationPanel();
        }

        /// <summary>
        /// 詳細パネルの中身を全てリセットする
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
