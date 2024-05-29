using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    public class Achievement : MonoBehaviour
    {
        [SerializeField] private GameObject achievementDetailPanelPrefab;
        [SerializeField] private Button[] gameKindsButtons;
        [SerializeField] private Button commonButton;
        [SerializeField] private TextMeshProUGUI totalExpText;

        private TextMeshProUGUI[] titleTexts;
        private TextMeshProUGUI[] levelTexts;
        private TextMeshProUGUI loginDaysText;

        private const int BASE_EXP = 100;
        private const int MAX_LEVEL = 10;
        private const int GAME_NUM = 4;
        private const int KINDS_NUM = 3;
        private const int ONE_LEVEL_EXP = 100;

        private const int EXP_COMMON = 10;
        private const int EXP_GAMES = 30;

        private GameSet[] gameSets;

        private int myTotalExp;
        private int[,] gamesScore;

        public struct GameSet
        {
            public int[] useMedals;
            public int[] payMedals;
            public int[] winNumber;
        }

        /// <summary>
        /// ������
        /// </summary>
        private void Init()
        {
            gameSets = new GameSet[GAME_NUM];

            gamesScore = new int[GAME_NUM, KINDS_NUM];

            //for (int i = 0; i < GAME_NUM; i++)
            //{
            //    gameSets[i].useMedals = new int[MAX_LEVEL];
            //    gameSets[i].payMedals = new int[MAX_LEVEL];
            //    gameSets[i].winNumber = new int[MAX_LEVEL];
            //}

            titleTexts = new TextMeshProUGUI[GAME_NUM];
            levelTexts = new TextMeshProUGUI[GAME_NUM];

            for (int i = 0; i < GAME_NUM; i++) 
            {
                int num = i;
                gameKindsButtons[i].onClick.AddListener(() => PushGameKindsButtons(num));
                titleTexts[i] = gameKindsButtons[i].gameObject.transform.Find("TitleText").gameObject.GetComponent<TextMeshProUGUI>();
                levelTexts[i] = gameKindsButtons[i].gameObject.transform.Find("LevelText").gameObject.GetComponent<TextMeshProUGUI>();
            }
            commonButton.onClick.AddListener(() => PushCommonButtons());
            loginDaysText = commonButton.gameObject.transform.Find("LoginDaysText").gameObject.GetComponent<TextMeshProUGUI>();

            SettingSomeArray();
        }

        /// <summary>
        /// �B���x�̃��x���̒l��ݒ肷��
        /// </summary>
        private void SettingSomeArray()
        {
            // DropBall
            gameSets[0].useMedals = new int[] { 1, 10, 50, 100, 250, 500, 1000, 2000, 5000, 10000 };
            gameSets[0].payMedals = new int[] { 1, 30, 100, 500, 1000, 3000, 6000, 10000, 20000, 30000 };
            gameSets[0].winNumber = new int[] { 1, 3, 6, 10, 20, 50, 100, 200, 500, 1000 };

            // JanKen
            gameSets[1].useMedals = new int[] { 1, 10, 50, 100, 250, 500, 1000, 2000, 5000, 10000 };
            gameSets[1].payMedals = new int[] { 1, 10, 50, 100, 250, 500, 1000, 2000, 5000, 10000 };
            gameSets[1].winNumber = new int[] { 1, 5, 10, 20, 50, 100, 200, 500, 1000, 3000 };

            // Pusher01
            gameSets[2].useMedals = new int[] { 1, 30, 100, 500, 1000, 3000, 6000, 10000, 50000, 99999 };
            gameSets[2].payMedals = new int[] { 1, 30, 100, 500, 1000, 3000, 6000, 10000, 50000, 99999 };
            gameSets[2].winNumber = new int[] { 1, 3, 6, 10, 20, 50, 100, 200, 500, 1000 };

            // SmartBall01
            gameSets[3].useMedals = new int[] { 20, 100, 300, 600, 1000, 2000, 3000, 6000, 10000, 20000 };
            gameSets[3].payMedals = new int[] { 20, 300, 600, 1000, 2000, 3000, 6000, 10000, 20000, 30000 };
            gameSets[3].winNumber = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        }

        /// <summary>
        /// �w�肵�����ڂ̃��x����Ԃ�
        /// </summary>
        /// <param name="score">�w�肵�����ڂ̌��݂̃X�R�A</param>
        /// <param name="gameNumber">�Q�[���̎�ޔԍ� 0:DropBall , 1:JanKen , 2:Pusher01 , 3:SmartBall01</param>
        /// <param name="kindNumber">�B���x�̔ԍ� 0:useMedals , 1:payMedals , 2:winNumber</param>
        /// <returns></returns>
        private int GetAchievementLevel(int score , int gameNumber , int kindNumber)
        {
            int[] array;

            if (gameNumber > GAME_NUM || gameNumber < 0)
            {
                Debug.LogError($"Achievement.GetExpValue : Error gameNumber = {gameNumber}");
                return -1;
            }

            switch (kindNumber)
            {
                case 0:
                    array = gameSets[gameNumber].useMedals;
                    break;
                case 1:
                    array = gameSets[gameNumber].payMedals;
                    break;
                case 2:
                    array = gameSets[gameNumber].winNumber;
                    break;
                default:
                    Debug.LogError($"Achievement.GetExpValue : Error kindNumber = {kindNumber}");
                    return -1;
            }

            if (array.Length != MAX_LEVEL)
            {
                Debug.LogError($"Achievement.GetExpValue : Array Error , Length = {array.Length}");
                return -1;
            }

            for (int i = 0; i < array.Length; i++)
            {
                if (score < array[i])
                {
                    return i;
                }
            }

            return MAX_LEVEL;
        }

        /// <summary>
        /// �o���l���v�Z����
        /// </summary>
        private void CalcMyTotalExp()
        {
            myTotalExp = BASE_EXP;

            // ���O�C��
            myTotalExp += PlayerInformationManager.Instance.GetLoginDays() * EXP_COMMON;

            // �S�Q�[��
            for (int i = 0; i < GAME_NUM; i++) 
            {
                // �S����
                for(int j = 0; j < KINDS_NUM; j++)
                {
                    gamesScore[i,j] = PlayerInformationManager.Instance.GetAchievementScore(i, j);

                    int thisExp = GetAchievementLevel(gamesScore[i, j], i, j) * EXP_GAMES;
                    myTotalExp += thisExp;

                    //Debug.Log($"Achievement.CalcMyTotalExp : {i},{j} -> thisExp = {thisExp}");
                }
            }

            Debug.Log($"Achievement.CalcMyTotalExp : MyTotalExp = {myTotalExp}");
        }

        /// <summary>
        /// �B���x�̏�����
        /// </summary>
        public void InitAchievement()
        {
            Init();
            CalcMyTotalExp();
        }

        /// <summary>
        /// �g�[�^���̌o���l��Ԃ��Q�b�^�[
        /// </summary>
        /// <returns>myTotalExp</returns>
        public int GetMyTotalExp()
        {
            return myTotalExp;
        }

        /// <summary>
        /// ���݂̃��x����Ԃ��Q�b�^�[
        /// </summary>
        /// <returns></returns>
        public int GetMyLevel()
        {
            return myTotalExp / ONE_LEVEL_EXP;
        }

        /// <summary>
        /// ���݂̌o���l(���x����������)��Ԃ��Q�b�^�[
        /// </summary>
        /// <returns></returns>
        public int GetOnlyExp()
        {
            return myTotalExp % ONE_LEVEL_EXP;
        }

        /// <summary>
        /// �Q�[�����Ƃ̒B���x���x���̕\�����X�V����
        /// </summary>
        public void UpdateAchievementOfGameKinds()
        {
            for (int i = 0; i < GAME_NUM; i++)
            {
                int wa = 0;
                for(int j = 0; j< KINDS_NUM; j++)
                {
                    wa += GetAchievementLevel(gamesScore[i, j], i, j);
                }

                levelTexts[i].text = wa.ToString();
                titleTexts[i].text = TextListGenerater.instance.StageToStageNameText((TextList.Stage)(i + 1));
            }
            loginDaysText.text = PlayerInformationManager.Instance.GetLoginDays().ToString();
            totalExpText.text = $"���o���l : {myTotalExp} Exp";
        }

        /// <summary>
        /// �Q�[�����Ƃ̏ڍ׃{�^�����������Ƃ�
        /// </summary>
        /// <param name="gameNum"></param>
        private void PushGameKindsButtons(int gameNum)
        {
            SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes1);

            int[] scores = new int[KINDS_NUM];
            int[] levels = new int[KINDS_NUM];

            for (int i = 0;i < KINDS_NUM; i++)
            {
                scores[i] = gamesScore[gameNum, i];
                levels[i] = GetAchievementLevel(scores[i], gameNum, i);
            }

            GameObject canvas = GameObject.FindGameObjectWithTag("Point");

            GameObject obj = Instantiate(achievementDetailPanelPrefab, canvas.transform);

            obj.GetComponent<AchievementDetailPanelPrefab>().SettingMyStatusForGameDetail(gameNum, scores, levels);

        }

        /// <summary>
        /// ���O�C�����̃{�^�����������Ƃ�
        /// </summary>
        private void PushCommonButtons()
        {
            SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes1);

            GameObject canvas = GameObject.FindGameObjectWithTag("Point");

            GameObject obj = Instantiate(achievementDetailPanelPrefab, canvas.transform);

            obj.GetComponent<AchievementDetailPanelPrefab>().SettingMyStatusForCommonDetail();
        }

        /// <summary>
        /// �w�肵��Mission�̖ڕW�X�R�A�̔z���Ԃ�
        /// </summary>
        /// <param name="gameNumber">�Q�[���̎�ޔԍ� 0:DropBall , 1:JanKen , 2:Pusher01 , 3:SmartBall01</param>
        /// <param name="kindNumber">�B���x�̔ԍ� 0:useMedals , 1:payMedals , 2:winNumber</param>
        /// <returns></returns>
        public int[] GetTargetScoreArray(int gameNumber, int kindNumber)
        {
            int[] array = new int[MAX_LEVEL];

            for (int i = 0; i < MAX_LEVEL; i++) 
            {
                if (kindNumber == 0)
                {
                    array[i] = gameSets[gameNumber].useMedals[i];
                }
                else if (kindNumber == 1)
                {
                    array[i] = gameSets[gameNumber].payMedals[i];
                }
                else if (kindNumber == 2)
                {
                    array[i] = gameSets[gameNumber].winNumber[i];
                }
                else
                {
                    break;
                }
            }

            return array;
        }

    }
}
