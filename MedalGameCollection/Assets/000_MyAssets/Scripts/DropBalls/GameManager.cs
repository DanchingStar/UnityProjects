using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace DropBalls
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameObject underBlocker;
        [SerializeField] private TextMeshPro scoreTMP;
        [SerializeField] private TextMeshPro getTMP;

        [SerializeField] private PayOut payOut;
        [SerializeField] private BallSpawn ballSpawn;

        [SerializeField] private ParticleSystem kamifubukiParticle;

        private bool gameOverFlg = false;
        private bool gameReStartFlg = false;
        private bool disableBallStartFlg = false;

        private const int NUMBER_OF_USE_MEDALS = 1;
        private const int NUMBER_OF_MAX_BALLS = 5;

        private int onBallCount = 0;
        private int getMedal = 0;

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
            Physics.gravity = new Vector3(0, -20f, 0);

            SetReStart();
        }

        private void Update()
        {
            if(gameOverFlg == underBlocker.activeSelf)
            {
                underBlocker.SetActive(!gameOverFlg);
            }

            if (gameOverFlg == true && gameReStartFlg == false)
            {
                gameReStartFlg = true;
                Invoke("SetReStart", 3.0f);
            }
        }

        /// <summary>
        /// �Q�[���I�[�o�[�t���O�̃Z�b�^�[
        /// </summary>
        /// <param name="flg">�����̎���false�A�����o���̎���true</param>
        public void SetGameOverFlgTrue(bool flg)
        {
            gameOverFlg = true;
        }

        /// <summary>
        /// �Q�[���I�[�o�[�t���O�̃Q�b�^�[
        /// </summary>
        /// <returns></returns>
        public bool GetGameOverFlg()
        {
            return gameOverFlg;
        }

        /// <summary>
        /// ���X�^�[�g����
        /// </summary>
        public void SetReStart()
        {
            gameOverFlg = false;
            gameReStartFlg = false;
            onBallCount = 0;
            getMedal = GetMedal();
            UpdateSegment();
            SetDisableBallStartFlg(false);

            SoundManager.Instance.ChangeAndPlayBgm(SoundManager.SoundBgmType.Normal);
        }

        /// <summary>
        /// �L���ȃ{�[���̐���ǉ�����
        /// </summary>
        public void SetOnBall()
        {
            onBallCount++;
            getMedal = GetMedal();
            UpdateSegment();

            if (onBallCount == NUMBER_OF_MAX_BALLS)
            {
                StartCoroutine(PerfectAchievementEffect());
            }
            else if (onBallCount == NUMBER_OF_MAX_BALLS - 1)
            {
                SoundManager.Instance.ChangeAndPlayBgm(SoundManager.SoundBgmType.Chance);
            }
            else
            {
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.ScoreUp);
            }
        }

        /// <summary>
        /// �{�[���̗L�����ɉ����Ċl�����������߂�
        /// </summary>
        /// <returns>���_���̊l������</returns>
        private int GetMedal()
        {
            switch (onBallCount)
            {
                case 0: return 0;
                case 1: return 0;
                case 2: return 2;
                case 3: return 5;
                case 4: return 12;
                case 5: return 48;
                default: return 0;
            }
        }

        /// <summary>
        /// �����o�����郁�_�����𓾂�Q�b�^�[
        /// </summary>
        /// <returns>�l�����郁�_����</returns>
        public int GetPayoutMedals()
        {
            return getMedal;
        }

        /// <summary>
        /// �L���ȃ{�[���̐��̃Q�b�^�[
        /// </summary>
        /// <returns></returns>
        public int GetOnBallCount()
        {
            return onBallCount;
        }

        /// <summary>
        /// �{�[�����X�^�[�g�ł��Ȃ���Ԃ̃t���O�̃Z�b�^�[
        /// </summary>
        /// <param name="b">���Ȃ�{�[�����łĂȂ�</param>
        public void SetDisableBallStartFlg(bool b)
        {
            disableBallStartFlg = b;
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
        /// �Z�O�����g�I�ȕ\�����X�V����
        /// </summary>
        private void UpdateSegment()
        {
            scoreTMP.text = onBallCount.ToString();
            getTMP.text = getMedal.ToString().PadLeft(2, '0');
        }

        /// <summary>
        /// ���_�����l������
        /// </summary>
        private void AcquisitionPayOutMedals()
        {
            int num = GetPayoutMedals();
            if (num <= 0)
            {
                return;
            }

            PlayerInformationManager.Instance.AddField("DropBallsData", "getMedalsTotal", num);

            payOut.SpawnMedals(num);
        }

        /// <summary>
        /// �p�[�t�F�N�g�B�����̉��o�Ə���
        /// </summary>
        /// <returns></returns>
        private IEnumerator PerfectAchievementEffect()
        {
            SetDisableBallStartFlg(true);

            // �����Ŋ��������ȃW���O�����Đ�����
            SoundManager.Instance.ChangeAndPlayBgm(SoundManager.SoundBgmType.Success);
            StartCoroutine(PlayKamiFubuki());

            yield return new WaitForSeconds(3);

            AcquisitionPayOutMedals();

            PlayerInformationManager.Instance.IncrementField("DropBallsData", "perfectClearTimes");
        }

        /// <summary>
        /// �����቉�o�����s����
        /// </summary>
        /// <returns></returns>
        private IEnumerator PlayKamiFubuki()
        {
            kamifubukiParticle.Play();

            yield return new WaitForSeconds(7);

            kamifubukiParticle.Stop();
        }

        /// <summary>
        /// PayOut�{�^�����������Ƃ��̏���
        /// </summary>
        public void PushPayOutButton()
        {
            if (GetDisableBallStartFlg()) return;

            AcquisitionPayOutMedals();
        }

        /// <summary>
        /// Shot�{�^�����������Ƃ��̏���
        /// </summary>
        public void PushShotButton()
        {
            if (GetDisableBallStartFlg()) return;

            if (PlayerInformationManager.Instance.ConsumptionMedal(NUMBER_OF_USE_MEDALS))
            {
                ballSpawn.SpawnBall();
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.BallStart);
                PlayerInformationManager.Instance.IncrementField("DropBallsData", "shotTimes");
            }
            else
            {

            }
        }

    }
}