using DropBalls;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace JanKen
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameObject enemy1;
        [SerializeField] private GameObject enemy2;
        [SerializeField] private GameObject enemy3;
        [SerializeField] private GameObject enemy4;

        [SerializeField] private GameObject enemyGuu;
        [SerializeField] private GameObject enemyChoki;
        [SerializeField] private GameObject enemyPaa;

        [SerializeField] private GameObject playerGuu;
        [SerializeField] private GameObject playerChoki;
        [SerializeField] private GameObject playerPaa;

        /// <summary>0,1,2:�������A3,4,5:�ԁA����G,C,P</summary>
        [SerializeField] private Material[] playerMaterials;

        private MeshRenderer[] playerMRs;

        private int numberOfUseMedals = 1;

        /// <summary>0����14�A���͍��ォ��</summary>
        [SerializeField] private GameObject[] pointDisplay;

        /// <summary>0����14:�������A15����29:�F�t���A����64,32,16,8,4,2,1</summary>
        [SerializeField] private Material[] pointMaterials;

        private MeshRenderer[] pointMRs;

        /// <summary>�ݒ�l(1�`5)�A5�������₷���A1�������₷��</summary>
        private int settingNum = 0;

        private int probabilityWin = 0;
        private int probabilityDraw = 0;
        private int probabilityLose = 0;

        private int[,] probabilityData;
        private int probabilityLCM = 0;

        private bool ableStartFlg = false;
        private bool ableDecideFlg = false;

        private int battleResult = 0;

        public enum JankenHand
        {
            None,
            Guu,
            Choki,
            Paa,
        }

        public static GameManager Instance;
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
            MakeProbabilityData();
            settingNum = MakeSetting();
            InitProbability();
            InitMeshRenderer();

            ResetBattle();
            Debug.Log($"GameManager.Start ( �ݒ�l ) = ( {settingNum} )");
        }

        /// <summary>
        /// �ݒ�l����t���猈�߂�
        /// </summary>
        /// <returns>�ݒ�l�F1�`5</returns>
        private int MakeSetting()
        {
            DateTime baseDay = new DateTime(2023, 4, 1);
            DateTime today = DateTime.Today;

            uint someday = (uint)(today - baseDay).TotalDays + 1;

            Unity.Mathematics.Random rand = new Unity.Mathematics.Random(someday);

            for (int i = 0; i < 10; i++)
            {
                rand.NextInt(0, 100); // �Ȃ�������͂������Ȓl�ɂȂ�̂�10��قǋ�ł����Ă���
            }
           
            int x = rand.NextInt(0, 100);

            Debug.Log($"GameManager.MakeSetting (����,�l) = ( {someday}��,{x} )");

            if (x < 5)
            {
                return 5;
            }
            else if(x < 20)
            {
                return 4;
            }
            else if(x < 40)
            {
                return 3;
            }
            else if(x < 70)
            {
                return 2;
            }
            else
            {
                return 1;
            }
        }

        /// <summary>
        /// �������Ƃ��̕����o�����������߂�
        /// </summary>
        /// <returns>�����o������</returns>
        private int DecidePayOut()
        {
            float n = Random.Range(0f, 100f);

            if(n < 1)
            {
                return 64;
            }
            else if(n < 3)
            {
                return 32;
            }
            else if(n < 6)
            {
                return 16;
            }
            else if(n < 16)
            {
                return 8;
            }
            else if(n < 52)
            {
                return 4;
            }
            else if(n < 88)
            {
                return 2;
            }
            else
            {
                return 1;
            }
        }

        /// <summary>
        /// ����񂯂�̏����f�[�^
        /// </summary>
        private void MakeProbabilityData()
        {
            probabilityData = new int[5, 3] { { 100, 202, 398 }, { 100, 208, 392 }, { 100, 216, 384 }, { 100, 230, 370 }, { 100, 250, 350 } };
        }

        /// <summary>
        /// �ݒ�Ɋ�Â��Ă���񂯂�̏��������߂�
        /// </summary>
        private void InitProbability()
        {
            probabilityWin = probabilityData[settingNum - 1, 0];
            probabilityDraw = probabilityData[settingNum - 1, 1];
            probabilityLose = probabilityData[settingNum - 1, 2];

            probabilityLCM = probabilityWin + probabilityDraw + probabilityLose;
        }

        /// <summary>
        /// �v���C���[�n���h�Ɠ��_�̃}�e���A���̂��߂�MeshRenderer�̏�����
        /// </summary>
        private void InitMeshRenderer()
        {
            playerMRs = new MeshRenderer[3] { playerGuu.GetComponent<MeshRenderer>(), 
                playerChoki.GetComponent<MeshRenderer>(), playerPaa.GetComponent<MeshRenderer>() };

            pointMRs = new MeshRenderer[pointDisplay.Length];
            for (int i = 0; i < pointDisplay.Length; i++) 
            {
                pointMRs[i] = pointDisplay[i].GetComponent<MeshRenderer>();
            }
        }

        /// <summary>
        /// ����񂯂���J�n����
        /// </summary>
        /// <param name="isDraw">true:���������o���Ƃ��Afalse�F���������o�Ă��Ȃ��Ƃ�</param>
        private void StartBattle(bool isDraw)
        {
            if (!isDraw)
            {
                if (!ableStartFlg)
                {
                    return;
                }
                ableStartFlg = false;

                PlayerInformationManager.Instance.IncrementField("JanKenData", "battleTimes");

                // SE(����񂯂�)
                SoundManager.Instance.PlaySEOnlyOne(SoundManager.SoundType.BattleStart);
            }
            else
            {
                // SE(��������)
                SoundManager.Instance.PlaySEOnlyOne(SoundManager.SoundType.BattleReStart);
            }

            battleResult = Random.Range(0, probabilityLCM);
            ableDecideFlg = true;

            StartCoroutine(DecideWaiting());

        }

        /// <summary>
        /// ����񂯂�ɏ������Ƃ�
        /// </summary>
        private void WinBattle()
        {
            // SE(����)
            SoundManager.Instance.PlaySEOnlyOne(SoundManager.SoundType.Win);

            StartCoroutine(WinBattleCoroutine());
        }

        /// <summary>
        /// ����񂯂�ɏ������Ƃ��̉��o�ƕ����o��
        /// </summary>
        /// <returns></returns>
        private IEnumerator WinBattleCoroutine()
        {
            int payout = DecidePayOut();
            int length = pointDisplay.Length;

            int backNum = -1;
            for (int i = 0; i < Random.Range(30, 50); i++) 
            {
                int num = Random.Range(0, length);
                if (num == backNum)
                {
                    num += 8;
                    if (num >= length)
                    {
                        num -= length;
                    }
                }
                backNum = num;

                PointDisplayReset();
                ChangePointDisplayOn(num);
                yield return new WaitForSeconds(0.05f);
            }

            PointDisplayReset();
            ChangePointDisplayOn(DicidePointDisplayNumber(payout));

            yield return new WaitForSeconds(1);

            AcquisitionPayOutMedals(payout);
            PayOut.Instance.SpawnMedals(payout);

            yield return new WaitForSeconds(2);

            // Debug.Log($"Win : {payout}");

            ResetBattle();

        }

        /// <summary>
        /// ����񂯂�ɕ������Ƃ�
        /// </summary>
        private void LoseBattle()
        {
            // SE(����)
            SoundManager.Instance.PlaySEOnlyOne(SoundManager.SoundType.Lose);

            ResetBattle();
        }

        /// <summary>
        /// ����񂯂�����Z�b�g���ăX�^�[�g�ł����Ԃɂ���
        /// </summary>
        private void ResetBattle()
        {
            StartCoroutine(ResetBattleCoroutine());             
        }

        /// <summary>
        /// ResetBattle�̒��g
        /// </summary>
        /// <returns></returns>
        private IEnumerator ResetBattleCoroutine()
        {
            while (PayOut.Instance.GetIsGameWait()) 
            {
                yield return null;
            }
            ableStartFlg = true;
            PointDisplayReset();
            StartCoroutine(StartWaiting());
        }

        /// <summary>
        /// ����񂯂�̏o��������߂āA���肵���Ƃ�
        /// </summary>
        /// <param name="hand">����񂯂�ŏo����</param>
        [com.llamagod.EnumAction(typeof(GameManager.JankenHand))]
        public void DecideHand(int handType)
        {
            if (!ableDecideFlg) return;

            ableDecideFlg = false;

            // SE(�ق����I)
            SoundManager.Instance.PlaySEOnlyOne(SoundManager.SoundType.Decide);

            HandReset();

            StartCoroutine(BattleStaging((JankenHand)handType));
        }

        /// <summary>
        /// ����񂯂�̉��o�ƁAbattleResult�ɂ�鏟�s�̌���
        /// </summary>
        /// <param name="hand"></param>
        /// <returns></returns>
        private IEnumerator BattleStaging(JankenHand hand)
        {
            float waitTime = 1;
            if (hand == JankenHand.Guu)
            {
                playerMRs[0].material = playerMaterials[0+3];
                if (battleResult < probabilityWin) //����
                {
                    enemyChoki.SetActive(true);
                    yield return new WaitForSeconds(waitTime);
                    WinBattle();
                }
                else if (battleResult < probabilityWin + probabilityDraw) //������
                {
                    enemyGuu.SetActive(true);
                    yield return new WaitForSeconds(waitTime);
                    StartBattle(true);
                }
                else //����
                {
                    enemyPaa.SetActive(true);
                    yield return new WaitForSeconds(waitTime);
                    LoseBattle();
                }
            }
            else if (hand == JankenHand.Choki)
            {
                playerMRs[1].material = playerMaterials[1 + 3];
                if (battleResult < probabilityWin) //����
                {
                    enemyPaa.SetActive(true);
                    yield return new WaitForSeconds(waitTime);
                    WinBattle();
                }
                else if (battleResult < probabilityWin + probabilityDraw) //������
                {
                    enemyChoki.SetActive(true);
                    yield return new WaitForSeconds(waitTime);
                    StartBattle(true);
                }
                else //����
                {
                    enemyGuu.SetActive(true);
                    yield return new WaitForSeconds(waitTime);
                    LoseBattle();
                }

            }
            else if (hand == JankenHand.Paa)
            {
                playerMRs[2].material = playerMaterials[2 + 3];
                if (battleResult < probabilityWin) //����
                {
                    enemyGuu.SetActive(true);
                    yield return new WaitForSeconds(waitTime);
                    WinBattle();
                }
                else if (battleResult < probabilityWin + probabilityDraw) //������
                {
                    enemyPaa.SetActive(true);
                    yield return new WaitForSeconds(waitTime);
                    StartBattle(true);
                }
                else //����
                {
                    enemyChoki.SetActive(true);
                    yield return new WaitForSeconds(waitTime);
                    LoseBattle();
                }

            }
            else
            {
                Debug.Log("GameManager.DecideHand Error");
            }

        }

        /// <summary>
        /// ����񂯂�Ō��߂�܂ő҂��Ă���Ԃ̉��o
        /// </summary>
        /// <returns></returns>
        private IEnumerator DecideWaiting()
        {
            int i = 0;
            while (ableDecideFlg)
            {
                HandReset();

                if (i == 0)
                {
                    enemyGuu.SetActive(true);
                    i = 1;
                }
                else if (i == 1)
                {
                    enemyChoki.SetActive(true);
                    i = 2;
                }
                else if (i == 2)
                {
                    enemyPaa.SetActive(true);
                    i = 0;
                }
                else
                {
                    Debug.LogError("GameManager.BattleWaiting Error");
                }
                yield return new WaitForSeconds(0.05f);
            }
        }

        /// <summary>
        /// ����񂯂�X�^�[�g�܂ő҂��Ă���Ԃ̉��o
        /// </summary>
        /// <returns></returns>
        private IEnumerator StartWaiting()
        {
            int i = 0;
            while (ableStartFlg)
            {
                HandReset();

                if (i == 0)
                {
                    enemy1.SetActive(true);
                    i = 1;
                }
                else if(i == 1)
                {
                    enemy2.SetActive(true);
                    i = 2;
                }
                else if( i == 2)
                {
                    enemy3.SetActive(true);
                    i = RareFaceLottery();
                }
                else if(i == 3)
                {
                    enemy4.SetActive(true);
                    i = 0;
                }
                else
                {
                    Debug.Log("GameManager.StartWaiting Error");
                }
                yield return new WaitForSeconds(1f);
            }

        }

        /// <summary>
        /// �G�̊�̃��A�\��̏o���𒊑I����
        /// </summary>
        /// <returns>0:�ʏ�A3:Rare�ȕ\��</returns>
        private int RareFaceLottery()
        {
            if (settingNum == 1) return 0;

            int num = Random.Range(0, 100);

            if (settingNum == 2)
            {
                if (num < 1)
                {
                    return 3;
                }
            }
            else if (settingNum == 3)
            {
                if (num < 2)
                {
                    return 3;
                }
            }
            else if (settingNum == 4)
            {
                if (num < 5)
                {
                    return 3;
                }
            }
            else if (settingNum == 5)
            {
                if (num < 10)
                {
                    return 3;
                }
            }

            return 0;
        }

        /// <summary>
        /// �v���C���[�ƓG�̎�̕\�������Z�b�g
        /// </summary>
        private void HandReset()
        {
            enemy1.SetActive(false);
            enemy2.SetActive(false);
            enemy3.SetActive(false);
            enemy4.SetActive(false);

            enemyGuu.SetActive(false);
            enemyChoki.SetActive(false);
            enemyPaa.SetActive(false);

            playerMRs[0].material = playerMaterials[0];
            playerMRs[1].material = playerMaterials[1];
            playerMRs[2].material = playerMaterials[2];
        }

        /// <summary>
        /// ���_�ł̕\�������Z�b�g
        /// </summary>
        private void PointDisplayReset()
        {
            pointMRs[0].material = pointMaterials[3];
            pointMRs[1].material = pointMaterials[5];
            pointMRs[2].material = pointMaterials[4];
            pointMRs[3].material = pointMaterials[5];
            pointMRs[4].material = pointMaterials[2];
            pointMRs[5].material = pointMaterials[6];
            pointMRs[6].material = pointMaterials[4];
            pointMRs[7].material = pointMaterials[0];
            pointMRs[8].material = pointMaterials[4];
            pointMRs[9].material = pointMaterials[6];
            pointMRs[10].material = pointMaterials[1];
            pointMRs[11].material = pointMaterials[5];
            pointMRs[12].material = pointMaterials[4];
            pointMRs[13].material = pointMaterials[5];
            pointMRs[14].material = pointMaterials[3];
        }

        /// <summary>
        /// �l�����_���ɉ����āA�_��������ׂ����_�ł�Ԃ�
        /// </summary>
        /// <param name="getMedal">�l�����_����</param>
        /// <returns>�_�����ׂ����_��</returns>
        private int DicidePointDisplayNumber(int getMedal)
        {
            if(getMedal == 64)
            {
                return 7;
            }
            else if(getMedal == 32)
            {
                return 10;
            }
            else if(getMedal == 16)
            {
                return 4;
            }
            else if (getMedal == 8)
            {
                int num = Random.Range(0, 2);
                if (num == 0)
                {
                    return 0;
                }
                else if (num == 1)
                {
                    return 14;
                }
                else
                {
                    Debug.Log("GameManager.DicidePointDisplayNumber Error");
                    return -1;
                }
            }
            else if (getMedal == 4)
            {
                int num = Random.Range(0, 4);
                if (num == 0)
                {
                    return 2;
                }
                else if (num == 1)
                {
                    return 6;
                }
                else if (num == 2)
                {
                    return 8;
                }
                else if (num == 3)
                {
                    return 12;
                }
                else
                {
                    Debug.Log("GameManager.DicidePointDisplayNumber Error");
                    return -1;
                }
            }
            else if (getMedal == 2)
            {
                int num = Random.Range(0, 4);
                if (num == 0)
                {
                    return 1;
                }
                else if (num == 1)
                {
                    return 3;
                }
                else if (num == 2)
                {
                    return 11;
                }
                else if (num == 3)
                {
                    return 13;
                }
                else
                {
                    Debug.Log("GameManager.DicidePointDisplayNumber Error");
                    return -1;
                }
            }
            else if (getMedal == 1)
            {
                int num = Random.Range(0, 2);
                if (num == 0)
                {
                    return 5;
                }
                else if (num == 1)
                {
                    return 9;
                }
                else
                {
                    Debug.Log("GameManager.DicidePointDisplayNumber Error");
                    return -1;
                }
            }
            else
            {
                Debug.Log("GameManager.DicidePointDisplayNumber Error");
                return -1;
            }
        }

        /// <summary>
        /// �w�肵�����_�ł�_��������
        /// </summary>
        /// <param name="num">�_�������������_�ł̔ԍ�(0����14)</param>
        private void ChangePointDisplayOn(int num)
        {
            switch (num)
            {
                case 0: pointMRs[num].material = pointMaterials[10]; break;
                case 1: pointMRs[num].material = pointMaterials[12]; break;
                case 2: pointMRs[num].material = pointMaterials[11]; break;
                case 3: pointMRs[num].material = pointMaterials[12]; break;
                case 4: pointMRs[num].material = pointMaterials[9]; break;
                case 5: pointMRs[num].material = pointMaterials[13]; break;
                case 6: pointMRs[num].material = pointMaterials[11]; break;
                case 7: pointMRs[num].material = pointMaterials[7]; break;
                case 8: pointMRs[num].material = pointMaterials[11]; break;
                case 9: pointMRs[num].material = pointMaterials[13]; break;
                case 10: pointMRs[num].material = pointMaterials[8]; break;
                case 11: pointMRs[num].material = pointMaterials[12]; break;
                case 12: pointMRs[num].material = pointMaterials[11]; break;
                case 13: pointMRs[num].material = pointMaterials[12]; break;
                case 14: pointMRs[num].material = pointMaterials[10]; break;
                default:Debug.Log("GameManager.ChangePointDisplayOn Error");break;
            }
        }

        /// <summary>
        /// ableStartFlg�̃Q�b�^�[
        /// </summary>
        /// <returns></returns>
        public bool GetAbleStartFlg()
        {
            return ableStartFlg;
        }

        /// <summary>
        /// �X�^�[�g�{�^�����������Ƃ�
        /// </summary>
        public void PushStartButton()
        {
            if (!ableStartFlg) return;

            if (PlayerInformationManager.Instance.ConsumptionMedal(numberOfUseMedals))
            {
                StartBattle(false);

            }
            else
            {

            }
        }

        /// <summary>
        /// ���_�����l������
        /// </summary>
        /// <param name="num">�l������</param>
        private void AcquisitionPayOutMedals(int num)
        {
            if (num <= 0) return;

            PlayerInformationManager.Instance.AddField("JanKenData", "getMedalsTotal", num);
            PlayerInformationManager.Instance.IncrementField("JanKenData", "winTimes");

            PlayerInformationManager.Instance.AcquisitionMedal(num);
        }
    }
}
