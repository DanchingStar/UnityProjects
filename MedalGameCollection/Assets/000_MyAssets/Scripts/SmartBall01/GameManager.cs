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

        /// <summary> タイムスケールのスピード </summary>
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

        /// <summary> 穴ごとのステータス </summary>
        private PointHole[] pointHoles;
        /// <summary> ボールが発射できる状態でないかを示すフラグ </summary>
        private bool disableBallStartFlg = false;

        /// <summary> クリアしているBINGOラインのフラグ </summary>
        private bool[] clearBingoLineFlgs = new bool[MAX_BINGO];
        /// <summary> リーチ状態のholeのフラグ </summary>
        private bool[] reachHoleFlgs;
        /// <summary> クリアしているBINGOライン数 </summary>
        private int clearBingoLineNum;
        /// <summary> 現在のゲームモード </summary>
        private GameMode gameMode;
        /// <summary> 現在のゲームモード </summary>
        private int haveBall;

        /// <summary> 結果画面が出ているかのフラグ </summary>
        private bool displayResultFlg = false;

        /// <summary> ゲームが終了しているかのフラグ </summary>
        private bool gameFinishFlg = false;

        /// <summary> タイマー(リーチ時の色を変えるため) </summary>
        private float timer;
        /// <summary> 前回のタイマー(リーチ時の色を変えるため) </summary>
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
        /// ボールがスタートできない状態のフラグのセッター
        /// </summary>
        /// <param name="b">正ならボールが打てない</param>
        public void SetDisableBallStartFlg(bool b)
        {
            disableBallStartFlg = b;

            if (!b)
            {
                CurrentBall = null;
            }
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
        /// 何番の穴に入ったか受け取る
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
        /// マテリアルをすべて引数にする
        /// </summary>
        /// <param name="mtr">したいマテリアル</param>
        private void AllMaterialToAny(Material mtr)
        {
            for (int i = 0; i < pointHoles.Length; i++)
            {
                pointHoles[i].material = mtr;
            }
        }

        /// <summary>
        /// 指定したオブジェクトのマテリアルを更新し、表示する
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="mtr"></param>
        private void DisplayMaterialAndChange(ref PointHole obj,Material mtr)
        {
            obj.material = mtr;
            obj.circle.GetComponent<MeshRenderer>().material = obj.material;
        }

        /// <summary>
        /// 全てのオブジェクトをマテリアルの情報をもとに表示しなおす
        /// </summary>
        private void DisplayMaterialAll()
        {
            for (int i = 0; i < pointHoles.Length; i++)
            {
                pointHoles[i].circle.GetComponent<MeshRenderer>().material = pointHoles[i].material;
            }
        }

        /// <summary>
        /// ボールをスポーンさせる
        /// </summary>
        public void SpawnBall()
        {
            if (GetDisableBallStartFlg()) return;

            if (GetHaveBall() <= 0) //ボールがなくなったとき
            {
                GameFinish();
            }
            else if (clearBingoLineNum >= MAX_BINGO) //ボールがある状態で、パーフェクト達成したとき
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
        /// 情報をリセットする
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
        /// ゲームをリセットする
        /// </summary>
        public void ReStart()
        {
            StartCoroutine(ReStartCoroutine());
        }

        /// <summary>
        /// ReStartのコルーチン
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
        /// 全てのBINGOラインをチェックする
        /// </summary>
        /// <returns>完成しているBINGOの数</returns>
        private void CheckClearBingoLine()
        {
            int num = 0;

            // 縦のラインの判定
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

            // 横のラインの判定
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

            // 斜めのラインの判定
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

            // BINGO数をカウント
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
        /// 引数の穴がリーチ状態かを受け取る
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

            // 縦のラインの判定
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

            // 横のラインの判定
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

            // 斜めのラインの判定
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

            // BINGO数と比較
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
        /// リーチの穴の色を変える
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
        /// ボールとリスポーンのオブジェクトとの当たり判定の有無を変更する
        /// </summary>
        /// <param name="col">ボールのコライダー</param>
        /// <param name="flg">trueなら当たり判定をなくす</param>
        public void ChangeRespawnIgnoreCollision(Collider col, bool flg)
        {
            BoxCollider colB = respawnEntranceObject.GetComponent<BoxCollider>();
            Physics.IgnoreCollision(col, colB, flg);
        }

        /// <summary>
        /// ゲームスタートのボタンを押したとき
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
        /// ギブアップボタンを押したとき
        /// </summary>
        public void PushGiveUpButton()
        {
            if (gameMode == GameMode.None) return;
            if (gameFinishFlg) return;

            GameFinish();
        }

        /// <summary>
        /// 現在のゲームを終える
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
        /// haveBallのゲッター
        /// </summary>
        /// <returns>持ち球数</returns>
        public int GetHaveBall()
        {
            return haveBall;
        }

        /// <summary>
        /// haveBallのセッター
        /// </summary>
        /// <param name="num">持ち球数</param>
        private void SetHaveBall(int num)
        {
            haveBall = num;
            UpdateHaveBallText();
        }

        /// <summary>
        /// haveBallの数を1減らす
        /// </summary>
        private void DecrementHaveBall()
        {
            haveBall--;
            UpdateHaveBallText();
        }

        /// <summary>
        /// 持ち球数のテキストを更新
        /// </summary>
        private void UpdateHaveBallText()
        {
            haveBallText.text = haveBall.ToString("00");
        }

        /// <summary>
        /// 紙吹雪演出を実行する
        /// </summary>
        /// <returns></returns>
        private IEnumerator PlayKamiFubuki()
        {
            kamifubukiParticle.Play();

            yield return new WaitForSeconds(3);

            kamifubukiParticle.Stop();
        }

        /// <summary>
        /// 結果画面を表示する
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
        /// 結果画面の表示・非表示を切り替える
        /// </summary>
        /// <param name="flg">trueが表示、falseが非表示
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
        /// gameFinishFlgのゲッター
        /// </summary>
        /// <returns>gameFinishFlg</returns>
        public bool GetFinishFlg()
        {
            return gameFinishFlg;
        }

        /// <summary>
        /// デバッグ用、ボールワープボタン
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

