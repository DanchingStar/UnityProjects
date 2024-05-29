using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SmartBall01
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Material colorRed;
        [SerializeField] private Material colorBlue;
        [SerializeField] private Material colorGreen;
        [SerializeField] private Material colorYellow;

        [SerializeField] private GameObject[] HoleObjects;
        [SerializeField] private GameObject ballSpawn;
        [SerializeField] private GameObject[] ballPrefab;
        [SerializeField] private GameObject holesStopper;
        [SerializeField] private GameObject respawnEntranceObject;

        [SerializeField] private TextMeshPro haveBallText;
        [SerializeField] private ResultPanel resultPanel;

        [SerializeField] private ParticleSystem kamifubukiParticle;

        /// <summary> �^�C���X�P�[���̃X�s�[�h </summary>
        [HideInInspector] public const float TIME_SPEED = 2f;

        public struct PointHole
        {
            public GameObject holeObject;
            public GameObject circle;
            public Material material;
            public bool isClear;
        }

        public enum GameMode
        {
            None,
            AllFire20,
            Fast1Line20,
        }

        private const int BET_MEDAL_ALLFIRE20 = 20;
        private const float CLEAR_ODS = 1.5f;
        private const int MAX_BINGO = 10;

        /// <summary> �����Ƃ̃X�e�[�^�X </summary>
        private PointHole[] pointHoles;
        /// <summary> �{�[�������˂ł����ԂłȂ����������t���O </summary>
        private bool disableBallStartFlg = false;

        /// <summary> �N���A���Ă���BINGO���C���̃t���O </summary>
        private bool[] clearBingoLineFlgs = new bool[MAX_BINGO];
        /// <summary> ���[�`��Ԃ�hole�̃t���O </summary>
        private bool[] reachHoleFlgs;
        /// <summary> �N���A���Ă���BINGO���C���� </summary>
        private int clearBingoLineNum;
        /// <summary> ���݂̃Q�[�����[�h </summary>
        private GameMode gameMode;
        /// <summary> ���݂̃Q�[�����[�h </summary>
        private int haveBall;

        /// <summary> ���ʉ�ʂ��o�Ă��邩�̃t���O </summary>
        private bool displayResultFlg = false;

        /// <summary> �Q�[�����I�����Ă��邩�̃t���O </summary>
        private bool gameFinishFlg = false;

        /// <summary> �^�C�}�[(���[�`���̐F��ς��邽��) </summary>
        private float timer;
        /// <summary> �O��̃^�C�}�[(���[�`���̐F��ς��邽��) </summary>
        private int oldTimer;

        private GameObject CurrentBall;

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
            Time.timeScale = TIME_SPEED;

            Physics.gravity = new Vector3(0, -20f, 0);

            pointHoles = new PointHole[HoleObjects.Length];
            reachHoleFlgs = new bool[HoleObjects.Length];
            for (int i = 0; i < HoleObjects.Length; i++) 
            {
                pointHoles[i].holeObject = HoleObjects[i];
                pointHoles[i].circle = HoleObjects[i].transform.Find("Circle").gameObject;
                // pointHoles[i].material = colorBlue;
                // pointHoles[i].isClear = false;
            }

            ResetStatus();
        }

        private void Update()
        {
            if (gameMode == GameMode.None) return;

            ReachHoleColorChange();

            if (gameFinishFlg) return;

            SpawnBall();
        }

        /// <summary>
        /// �{�[�����X�^�[�g�ł��Ȃ���Ԃ̃t���O�̃Z�b�^�[
        /// </summary>
        /// <param name="b">���Ȃ�{�[�����łĂȂ�</param>
        public void SetDisableBallStartFlg(bool b)
        {
            disableBallStartFlg = b;

            if (!b)
            {
                CurrentBall = null;
            }
        }

        /// <summary>
        /// �{�[�����X�^�[�g�ł��Ȃ���Ԃ̃t���O�̃Q�b�^�[
        /// </summary>
        /// <returns>���Ȃ�{�[�����łĂȂ�</returns>
        public bool GetDisableBallStartFlg()
        {
            return disableBallStartFlg;
        }

        /// <summary>
        /// ���Ԃ̌��ɓ��������󂯎��
        /// </summary>
        /// <param name="number"></param>
        public void SetPointNumber(int number)
        {
            pointHoles[number].isClear = true;
            DisplayMaterialAndChange(ref pointHoles[number], colorRed);

            CheckClearBingoLine();
            for (int i = 0;i < HoleObjects.Length;i++)
            {
                reachHoleFlgs[i] = isReachHole(i);
                //Debug.Log($"Hole {i} = {reachHoleFlgs[i]}");
            }
        }

        /// <summary>
        /// �}�e���A�������ׂĈ����ɂ���
        /// </summary>
        /// <param name="mtr">�������}�e���A��</param>
        private void AllMaterialToAny(Material mtr)
        {
            for (int i = 0; i < pointHoles.Length; i++)
            {
                pointHoles[i].material = mtr;
            }
        }

        /// <summary>
        /// �w�肵���I�u�W�F�N�g�̃}�e���A�����X�V���A�\������
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="mtr"></param>
        private void DisplayMaterialAndChange(ref PointHole obj,Material mtr)
        {
            obj.material = mtr;
            obj.circle.GetComponent<MeshRenderer>().material = obj.material;
        }

        /// <summary>
        /// �S�ẴI�u�W�F�N�g���}�e���A���̏������Ƃɕ\�����Ȃ���
        /// </summary>
        private void DisplayMaterialAll()
        {
            for (int i = 0; i < pointHoles.Length; i++)
            {
                pointHoles[i].circle.GetComponent<MeshRenderer>().material = pointHoles[i].material;
            }
        }

        /// <summary>
        /// �{�[�����X�|�[��������
        /// </summary>
        public void SpawnBall()
        {
            if (GetDisableBallStartFlg()) return;

            if (GetHaveBall() <= 0) //�{�[�����Ȃ��Ȃ����Ƃ�
            {
                GameFinish();
            }
            else if (clearBingoLineNum >= MAX_BINGO) //�{�[���������ԂŁA�p�[�t�F�N�g�B�������Ƃ�
            {
                GameFinish();
            }
            else
            {
                GameObject myGameobject = Instantiate(ballPrefab[Random.Range(0, ballPrefab.Length)],
                    ballSpawn.transform.position, Quaternion.identity, this.transform);

                if (myGameobject != null)
                {
                    DecrementHaveBall();
                    SetDisableBallStartFlg(true);
                    CurrentBall = myGameobject;
                }
            }
        }

        /// <summary>
        /// �������Z�b�g����
        /// </summary>
        private void ResetStatus()
        {
            for (int i = 0; i < pointHoles.Length; i++)
            {
                pointHoles[i].material = colorBlue;
                pointHoles[i].isClear = false;
                reachHoleFlgs[i] = false;
            }

            for (int i = 0; i < MAX_BINGO; i++) 
            {
                clearBingoLineFlgs[i] = false;
            }
            clearBingoLineNum = 0;

            timer = 0f;
            oldTimer = 0;

            gameMode = GameMode.None;
            SetHaveBall(0);

            DisplayMaterialAll();
        }

        /// <summary>
        /// �Q�[�������Z�b�g����
        /// </summary>
        public void ReStart()
        {
            StartCoroutine(ReStartCoroutine());
        }

        /// <summary>
        /// ReStart�̃R���[�`��
        /// </summary>
        /// <returns></returns>
        private IEnumerator ReStartCoroutine()
        {
            SoundManager.Instance.PlaySE(SoundManager.SoundSeType.GameReset);

            SetDisableBallStartFlg(true);
            holesStopper.SetActive(false);

            yield return new WaitForSeconds(1);

            ResetStatus();

            yield return new WaitForSeconds(1);

            SetDisableBallStartFlg(false);
            holesStopper.SetActive(true);
            gameFinishFlg = false;
        }

        /// <summary>
        /// �S�Ă�BINGO���C�����`�F�b�N����
        /// </summary>
        /// <returns>�������Ă���BINGO�̐�</returns>
        private void CheckClearBingoLine()
        {
            int num = 0;

            // �c�̃��C���̔���
            for (int i = 0; i < 4; i++)
            {
                bool isBingo = true;
                for (int j = 0; j < 4; j++)
                {
                    if (!pointHoles[i + j * 4].isClear)
                    {
                        isBingo = false;
                        break;
                    }
                }
                clearBingoLineFlgs[i] = isBingo;
            }

            // ���̃��C���̔���
            for (int i = 0; i < 4; i++)
            {
                bool isBingo = true;
                for (int j = 0; j < 4; j++)
                {
                    if (!pointHoles[i * 4 + j].isClear)
                    {
                        isBingo = false;
                        break;
                    }
                }
                clearBingoLineFlgs[i + 4] = isBingo;

            }

            // �΂߂̃��C���̔���
            bool diagonalBingo1 = true;
            bool diagonalBingo2 = true;
            for (int i = 0; i < 4; i++)
            {
                if (!pointHoles[i * 4 + i].isClear)
                {
                    diagonalBingo1 = false;
                }

                if (!pointHoles[i * 4 + (3 - i)].isClear)
                {
                    diagonalBingo2 = false;
                }
            }
            clearBingoLineFlgs[8] = diagonalBingo1;
            clearBingoLineFlgs[9] = diagonalBingo2;

            // BINGO�����J�E���g
            for (int i = 0; i < MAX_BINGO; i++) 
            {
                if (clearBingoLineFlgs[i])
                {
                    num++;
                }
            }

            if (num > clearBingoLineNum)
            {
                StartCoroutine(PlayKamiFubuki());
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Bingo);
            }
            else
            {
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.BallIn);
            }

            clearBingoLineNum = num;
        }

        /// <summary>
        /// �����̌������[�`��Ԃ����󂯎��
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        private bool isReachHole(int num)
        {
            if (pointHoles[num].isClear) return false;

            int ifBingoLine = 0;
            bool[] isClearHoles = new bool[pointHoles.Length];

            for (int i = 0; i < pointHoles.Length; i++)
            {
                isClearHoles[i] = pointHoles[i].isClear;
            }
            isClearHoles[num] = true;

            // �c�̃��C���̔���
            for (int i = 0; i < 4; i++)
            {
                bool isBingo = true;
                for (int j = 0; j < 4; j++)
                {
                    if (!isClearHoles[i + j * 4])
                    {
                        isBingo = false;
                        break;
                    }
                }
                if (isBingo)
                {
                    ifBingoLine++;
                }
            }

            // ���̃��C���̔���
            for (int i = 0; i < 4; i++)
            {
                bool isBingo = true;
                for (int j = 0; j < 4; j++)
                {
                    if (!isClearHoles[i * 4 + j])
                    {
                        isBingo = false;
                        break;
                    }
                }
                if (isBingo)
                {
                    ifBingoLine++;
                }
            }

            // �΂߂̃��C���̔���
            bool diagonalBingo1 = true;
            bool diagonalBingo2 = true;
            for (int i = 0; i < 4; i++)
            {
                if (!isClearHoles[i * 4 + i])
                {
                    diagonalBingo1 = false;
                }

                if (!isClearHoles[i * 4 + (3 - i)])
                {
                    diagonalBingo2 = false;
                }
            }
            if (diagonalBingo1)
            {
                ifBingoLine++;
            }
            if (diagonalBingo2)
            {
                ifBingoLine++;
            }

            //Debug.Log($"{clearBingoLineNum} < {ifBingoLine}");

            // BINGO���Ɣ�r
            if (clearBingoLineNum < ifBingoLine)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// ���[�`�̌��̐F��ς���
        /// </summary>
        private void ReachHoleColorChange()
        {
            const int BASE_TIME = 1;

            timer += Time.deltaTime;

            //Debug.Log($"{timer} : {oldTimer}");

            if (timer - oldTimer > BASE_TIME)
            {
                oldTimer += BASE_TIME;
                //Debug.Log($"oldTimer = {oldTimer}");

                for (int i = 0; i < HoleObjects.Length; i++)
                {
                    if (reachHoleFlgs[i])
                    {
                        if (oldTimer % (BASE_TIME * 2) == 0) 
                        {
                            pointHoles[i].material = colorBlue;
                        }
                        else
                        {
                            pointHoles[i].material = colorYellow;
                        }
                        //Debug.Log($"{i} = {reachHoleFlgs[i]} : {pointHoles[i].material}");
                    }
                }
                DisplayMaterialAll();
            }
        }

        /// <summary>
        /// �{�[���ƃ��X�|�[���̃I�u�W�F�N�g�Ƃ̓����蔻��̗L����ύX����
        /// </summary>
        /// <param name="col">�{�[���̃R���C�_�[</param>
        /// <param name="flg">true�Ȃ瓖���蔻����Ȃ���</param>
        public void ChangeRespawnIgnoreCollision(Collider col, bool flg)
        {
            BoxCollider colB = respawnEntranceObject.GetComponent<BoxCollider>();
            Physics.IgnoreCollision(col, colB, flg);
        }

        /// <summary>
        /// �Q�[���X�^�[�g�̃{�^�����������Ƃ�
        /// </summary>
        public void PushGameStartButton()
        {
            if (gameMode != GameMode.None) return;
            if (gameFinishFlg) return;

            if (PlayerInformationManager.Instance.ConsumptionMedal(BET_MEDAL_ALLFIRE20))
            {
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.GameStart);
                PlayerInformationManager.Instance.AddField("SmartBall01Data", "betMedalTotal", BET_MEDAL_ALLFIRE20);
                gameMode = GameMode.AllFire20;
                SetHaveBall(20);
                SpawnBall();
            }
            else
            {

            }
        }

        /// <summary>
        /// �M�u�A�b�v�{�^�����������Ƃ�
        /// </summary>
        public void PushGiveUpButton()
        {
            if (gameMode == GameMode.None) return;
            if (gameFinishFlg) return;

            GameFinish();
        }

        /// <summary>
        /// ���݂̃Q�[�����I����
        /// </summary>
        public void GameFinish()
        {
            gameFinishFlg = true;

            if (CurrentBall != null)
            {
                Destroy(CurrentBall);
            }

            SoundManager.Instance.PlaySE(SoundManager.SoundSeType.GameFinish);
            DisplayResult(true);
        }

        /// <summary>
        /// haveBall�̃Q�b�^�[
        /// </summary>
        /// <returns>��������</returns>
        public int GetHaveBall()
        {
            return haveBall;
        }

        /// <summary>
        /// haveBall�̃Z�b�^�[
        /// </summary>
        /// <param name="num">��������</param>
        private void SetHaveBall(int num)
        {
            haveBall = num;
            UpdateHaveBallText();
        }

        /// <summary>
        /// haveBall�̐���1���炷
        /// </summary>
        private void DecrementHaveBall()
        {
            haveBall--;
            UpdateHaveBallText();
        }

        /// <summary>
        /// ���������̃e�L�X�g���X�V
        /// </summary>
        private void UpdateHaveBallText()
        {
            haveBallText.text = haveBall.ToString("00");
        }

        /// <summary>
        /// �����቉�o�����s����
        /// </summary>
        /// <returns></returns>
        private IEnumerator PlayKamiFubuki()
        {
            kamifubukiParticle.Play();

            yield return new WaitForSeconds(3);

            kamifubukiParticle.Stop();
        }

        /// <summary>
        /// ���ʉ�ʂ�\������
        /// </summary>
        private IEnumerator DisplayResult()
        {
            float getMedals = BET_MEDAL_ALLFIRE20 * clearBingoLineNum * CLEAR_ODS;
            resultPanel.DisplayAndUpdateThisPanel(BET_MEDAL_ALLFIRE20, clearBingoLineNum, CLEAR_ODS);

            PlayerInformationManager.Instance.UpdateField("SmartBall01Data", "maxLines", clearBingoLineNum);
            PlayerInformationManager.Instance.AddField("SmartBall01Data", "getMedalTotal", (int)getMedals);
            PayOut.Instance.SpawnMedals((int)getMedals);

            while (displayResultFlg)
            {
                yield return null;
            }

            ReStart();
        }

        /// <summary>
        /// ���ʉ�ʂ̕\���E��\����؂�ւ���
        /// </summary>
        /// <param name="flg">true���\���Afalse����\��
        public void DisplayResult(bool flg)
        {
            if (flg)
            {
                displayResultFlg = flg;
                resultPanel.gameObject.SetActive(flg);
                StartCoroutine(DisplayResult());
            }
            else
            {
                resultPanel.ResetTexts();
                displayResultFlg = flg;
                resultPanel.gameObject.SetActive(flg);
            }

        }

        /// <summary>
        /// gameFinishFlg�̃Q�b�^�[
        /// </summary>
        /// <returns>gameFinishFlg</returns>
        public bool GetFinishFlg()
        {
            return gameFinishFlg;
        }

        /// <summary>
        /// �f�o�b�O�p�A�{�[�����[�v�{�^��
        /// </summary>
        /// <param name="num"></param>
        public void PushDebagMoveButton(int num)
        {
            if (gameMode == GameMode.None) return;

            const float  DEFAULT_POSITION_X = -9f;
            const float  DEFAULT_POSITION_Y = -2.72f;
            const float  DEFAULT_POSITION_Z = -6.6f;

            const float MOVE_POSITION_X = 6f;
            float MOVE_POSITION_Y = MOVE_POSITION_X * Mathf.Sin(Mathf.Deg2Rad * 30);
            float MOVE_POSITION_Z = MOVE_POSITION_X * Mathf.Cos(Mathf.Deg2Rad * 30);

            int x = num % 4;
            int z = num / 4;

            CurrentBall.transform.position = new Vector3(
                DEFAULT_POSITION_X + MOVE_POSITION_X * x,
                DEFAULT_POSITION_Y + MOVE_POSITION_Y * z,
                DEFAULT_POSITION_Z + MOVE_POSITION_Z * z );

            CurrentBall.GetComponent<Rigidbody>().velocity = Vector3.zero;



        }

    }
}

