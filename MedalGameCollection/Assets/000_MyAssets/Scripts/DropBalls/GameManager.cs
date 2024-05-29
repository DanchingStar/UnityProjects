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
        /// ゲームオーバーフラグのセッター
        /// </summary>
        /// <param name="flg">負けの時はfalse、払い出しの時はtrue</param>
        public void SetGameOverFlgTrue(bool flg)
        {
            gameOverFlg = true;
        }

        /// <summary>
        /// ゲームオーバーフラグのゲッター
        /// </summary>
        /// <returns></returns>
        public bool GetGameOverFlg()
        {
            return gameOverFlg;
        }

        /// <summary>
        /// リスタートする
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
        /// 有効なボールの数を追加する
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
        /// ボールの有効数に応じて獲得枚数を決める
        /// </summary>
        /// <returns>メダルの獲得枚数</returns>
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
        /// 払い出しするメダル数を得るゲッター
        /// </summary>
        /// <returns>獲得するメダル数</returns>
        public int GetPayoutMedals()
        {
            return getMedal;
        }

        /// <summary>
        /// 有効なボールの数のゲッター
        /// </summary>
        /// <returns></returns>
        public int GetOnBallCount()
        {
            return onBallCount;
        }

        /// <summary>
        /// ボールがスタートできない状態のフラグのセッター
        /// </summary>
        /// <param name="b">正ならボールが打てない</param>
        public void SetDisableBallStartFlg(bool b)
        {
            disableBallStartFlg = b;
        }

        /// <summary>
        /// ボールがスタートできない状態のフラグのゲッター
        /// </summary>
        /// <returns>正ならボールが打てない</returns>
        public bool GetDisableBallStartFlg()
        {
            return disableBallStartFlg;
        }

        /// <summary>
        /// セグメント的な表示を更新する
        /// </summary>
        private void UpdateSegment()
        {
            scoreTMP.text = onBallCount.ToString();
            getTMP.text = getMedal.ToString().PadLeft(2, '0');
        }

        /// <summary>
        /// メダルを獲得する
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
        /// パーフェクト達成時の演出と処理
        /// </summary>
        /// <returns></returns>
        private IEnumerator PerfectAchievementEffect()
        {
            SetDisableBallStartFlg(true);

            // ここで嬉しそうなジングルを再生する
            SoundManager.Instance.ChangeAndPlayBgm(SoundManager.SoundBgmType.Success);
            StartCoroutine(PlayKamiFubuki());

            yield return new WaitForSeconds(3);

            AcquisitionPayOutMedals();

            PlayerInformationManager.Instance.IncrementField("DropBallsData", "perfectClearTimes");
        }

        /// <summary>
        /// 紙吹雪演出を実行する
        /// </summary>
        /// <returns></returns>
        private IEnumerator PlayKamiFubuki()
        {
            kamifubukiParticle.Play();

            yield return new WaitForSeconds(7);

            kamifubukiParticle.Stop();
        }

        /// <summary>
        /// PayOutボタンを押したときの処理
        /// </summary>
        public void PushPayOutButton()
        {
            if (GetDisableBallStartFlg()) return;

            AcquisitionPayOutMedals();
        }

        /// <summary>
        /// Shotボタンを押したときの処理
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