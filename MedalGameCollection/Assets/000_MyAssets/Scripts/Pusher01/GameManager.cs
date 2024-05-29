using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Pusher01
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private MedalSpawn medalSpawn;
        [SerializeField] private MedalPayFall medalPayFall;
        [SerializeField] private UIAnimationPanel uIAnimationPanel;
        [SerializeField] private HoryuuLamp[] horyuuLamp;
        [SerializeField] private Lille[] Lilles;
        [SerializeField] private ParticleSystem backgroundParticle;

        [SerializeField] private GameObject settingLightPanel;
        [SerializeField] private GameObject pointLightsObject;
        [SerializeField] private GameObject directionalLightObject;
        [SerializeField] private Text nowQualityText;

        /// <summary> 保留の最大値(抽選中の保留を含む) </summary>
        private const int MAX_HORYUU = 10;
        /// <summary> 一度の発射で使うメダルの枚数 </summary>
        private const int ONCE_BET_MEDALS = 1;
        /// <summary> スーパー当選時のメダル払い出し枚数 </summary>
        private const int PAY_MEDALS_SUPER = 200;
        /// <summary> 確変当選時のメダル払い出し枚数 </summary>
        private const int PAY_MEDALS_KAKUHEN = 100;
        /// <summary> 通常当選時のメダル払い出し枚数 </summary>
        private const int PAY_MEDALS_TUUJOU = 100;

        /// <summary> メダルが発射できないかのフラグ </summary>
        private bool disableMedalStartFlg = false;
        /// <summary> 保留に入ったときに抽選する当たりなどのデータ </summary>
        private List<SlotData> slotData;
        /// <summary> スロットが回転できるかのフラグ </summary>
        private bool waitSlot = false;
        /// <summary> リーチ状態のフラグ </summary>
        private bool reachFlg = false;
        /// <summary> 現在のモード、保留はこれをもとに抽選する </summary>
        private Atari naibuMode;
        /// <summary> 現在のモード、背景やBGMはこれをもとに決める </summary>
        private Atari mitameMode;
        /// <summary> 現在の光の設定 true:高品質 , false:低品質 </summary>
        private bool lightQualityFlg;

        private UiSeManager uiSeManager;

        private const string LIGHT_QUALITY_KEY = "SmartBall01_LightQuality";
        private const int LIGHT_QUALITY_HIGH = 1;
        private const int LIGHT_QUALITY_LOW = 0;

        private const int RANDOM_RANGE = 10000;


        /// <summary>
        /// 抽選する内容
        /// </summary>
        public struct SlotData
        {
            public LilleNumber lilleLeft;
            public LilleNumber lilleCenter;
            public LilleNumber lilleRight;
            public HoryuuColor horyuuColor;
            public Atari atari;
        }

        /// <summary>
        /// リールの数字
        /// </summary>
        public enum LilleNumber
        {
            Zero,
            One,
            Two,
            Three,
            Four,
            Five,
            Six,
            Seven,
            Eight,
            Nine,
        }

        /// <summary>
        /// 保留の色
        /// </summary>
        public enum HoryuuColor
        {
            Normal,
            Blue,
            Green,
            Purple,
            Red,
            None,
        }

        /// <summary>
        /// 当たりかどうかと当たりの種類
        /// </summary>
        public enum Atari
        {
            Hazure,
            Reach,
            Tuujou,
            Kakuhen,
            Super,
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
            Physics.gravity = new Vector3(0, -20f, 0);

            uiSeManager = GameObject.Find("UiSeManager").GetComponent<UiSeManager>();
            InitSettingLight();

            slotData = new List<SlotData>();
            SetNaibuMode(Atari.Tuujou);
            SetMitameMode(Atari.Tuujou);
            UpdateHoryuuColors();
        }

        private void Update()
        {
            RotateSlot();
        }

        /// <summary>
        /// メダルがスタートできない状態のフラグのセッター
        /// </summary>
        /// <param name="b">正ならボールが打てない</param>
        public void SetDisableMedalStartFlg(bool b)
        {
            disableMedalStartFlg = b;
        }

        /// <summary>
        /// メダルがスタートできない状態のフラグのゲッター
        /// </summary>
        /// <returns>正ならボールが打てない</returns>
        public bool GetDisableMedalStartFlg()
        {
            return disableMedalStartFlg;
        }

        /// <summary>
        /// メダル発射ボタンを押したとき
        /// </summary>
        public void PushMedalSpawn()
        {
            if (PlayerInformationManager.Instance.ConsumptionMedal(ONCE_BET_MEDALS))
            {
                medalSpawn.SpawnMedal();
                PlayerInformationManager.Instance.IncrementField("Pusher01Data", "betMedalTotal");
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.MedalIn);
            }
            else
            {

            }
        }

        /// <summary>
        /// 保留数を1増やす
        /// </summary>
        /// <param name="cheatFlg">チートモード : true -> 確定大当たり</param>
        public void IncrementHoryuu(bool cheatFlg)
        {
            SlotData addData = new SlotData();
            int count = slotData.Count;
            if (count < MAX_HORYUU)
            {
                addData = LotterySlotData(cheatFlg);

                slotData.Add(addData);
                if (!(addData.atari == Atari.Hazure || addData.atari == Atari.Reach))
                {
                    if (naibuMode != addData.atari)
                    {
                        // 現在の確率の内部モードを変更する
                        SetNaibuMode(addData.atari);
                    }
                }
                horyuuLamp[count].ChangeHoryuuColor(slotData[count].horyuuColor);
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.HoryuuIn);
            }
        }

        /// <summary>
        /// 当たりや保留などを抽選する
        /// </summary>
        /// /// <param name="cheatFlg">チートモード : true -> 確定大当たり</param>
        /// <returns>当たりなどのデータ</returns>
        private SlotData LotterySlotData(bool cheatFlg)
        {
            SlotData nowData = new SlotData();

            int randomNum = Random.Range(0, RANDOM_RANGE);
            int subRandomNum = Random.Range(0, RANDOM_RANGE);

            // 大当たりの抽選
            nowData.atari = DecideAtari(naibuMode, randomNum, cheatFlg);

            // リールの数字を決定する
            nowData = DecideLilleNumbers(nowData, subRandomNum);

            // 保留の色を決める
            nowData.horyuuColor = DecideHoryuuColor(nowData, subRandomNum);

            return nowData;
        }

        /// <summary>
        /// 大当たりを抽選する
        /// </summary>
        /// <param name="nowMode"></param>
        /// <param name="randomNum"></param>
        /// <param name="cheatFlg"></param>
        /// <returns></returns>
        private Atari DecideAtari(Atari nowMode, int randomNum, bool cheatFlg)
        {
            //返り値
            Atari decide = Atari.Hazure;

            //デバッグ用チートフラグが正のとき
            if (cheatFlg)
            {
                if (naibuMode == Atari.Tuujou)
                {
                    randomNum /= 100;
                }
                else
                {
                    randomNum /= 10;
                }
            }

            // 現在のモードに合わせて当たりを抽選する
            if (naibuMode == Atari.Tuujou)
            {
                if (randomNum < 100) //大当たりのときの振り分け
                {
                    if (randomNum < 5)
                    {
                        decide = Atari.Super;
                    }
                    else if (randomNum < 40)
                    {
                        decide = Atari.Kakuhen;
                    }
                    else
                    {
                        decide = Atari.Tuujou;
                    }
                }
                else //ハズレのときの振り分け
                {
                    if (randomNum < 1000)
                    {
                        decide = Atari.Reach;
                    }
                    else
                    {
                        decide = Atari.Hazure;
                    }
                }
            }
            else if (naibuMode == Atari.Kakuhen)
            {
                if (randomNum < 1000) //大当たりのときの振り分け
                {
                    if (randomNum < 100)
                    {
                        decide = Atari.Super;
                    }
                    else if (randomNum < 750)
                    {
                        decide = Atari.Kakuhen;
                    }
                    else
                    {
                        decide = Atari.Tuujou;
                    }
                }
                else //ハズレのときの振り分け
                {
                    if (randomNum < 2000)
                    {
                        decide = Atari.Reach;
                    }
                    else
                    {
                        decide = Atari.Hazure;
                    }
                }
            }
            else if (naibuMode == Atari.Super)
            {
                if (randomNum < 1000) //大当たりのときの振り分け
                {
                    if (randomNum < 500)
                    {
                        decide = Atari.Super;
                    }
                    else
                    {
                        decide = Atari.Kakuhen;
                    }
                }
                else //ハズレのときの振り分け
                {
                    if (randomNum < 2000)
                    {
                        decide = Atari.Reach;
                    }
                    else
                    {
                        decide = Atari.Hazure;
                    }
                }
            }

            return decide;
        }

        /// <summary>
        /// 3つのリールの数字を決める
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private SlotData DecideLilleNumbers(SlotData data,int subRandomNum)
        {
            if (data.atari == Atari.Hazure)
            {
                int leftNum = subRandomNum % 10;
                int centerNum = Random.Range(0, 10);
                int rightNum = leftNum + Random.Range(1, 10);
                if (rightNum >= 10)
                {
                    rightNum -= 10;
                }

                data.lilleLeft = (LilleNumber)leftNum;
                data.lilleCenter = (LilleNumber)centerNum;
                data.lilleRight = (LilleNumber)rightNum;
            }
            else if (data.atari == Atari.Reach)
            {
                int leftNum = subRandomNum % 10;
                if (leftNum == 7)
                {
                    leftNum += Random.Range(1, 10);
                    if (leftNum >= 10)
                    {
                        leftNum -= 10;
                    }
                }
                int rightNum = leftNum;

                int centerNum = leftNum + 1;
                if (Random.Range(0, 2) == 0)
                {
                    centerNum = leftNum + 9;
                }

                if (centerNum >= 10)
                {
                    centerNum -= 10;
                }

                data.lilleLeft = (LilleNumber)leftNum;
                data.lilleCenter = (LilleNumber)centerNum;
                data.lilleRight = (LilleNumber)rightNum;
            }
            else if (data.atari == Atari.Tuujou)
            {
                int leftNum = subRandomNum % 10;
                if (leftNum % 2 == 1)
                {
                    leftNum--;
                }
                int centerNum = leftNum;
                int rightNum = leftNum;

                data.lilleLeft = (LilleNumber)leftNum;
                data.lilleCenter = (LilleNumber)centerNum;
                data.lilleRight = (LilleNumber)rightNum;
            }
            else if (data.atari == Atari.Kakuhen)
            {
                int num = subRandomNum % 4;
                int leftNum;
                switch (num)
                {
                    case 0: leftNum = 1; break;
                    case 1: leftNum = 3; break;
                    case 2: leftNum = 5; break;
                    case 3: leftNum = 9; break;
                    default: leftNum = -1; break;
                }
                int centerNum = leftNum;
                int rightNum = leftNum;

                data.lilleLeft = (LilleNumber)leftNum;
                data.lilleCenter = (LilleNumber)centerNum;
                data.lilleRight = (LilleNumber)rightNum;
            }
            else if (data.atari == Atari.Super)
            {
                int leftNum = 7;
                int centerNum = leftNum;
                int rightNum = leftNum;

                data.lilleLeft = (LilleNumber)leftNum;
                data.lilleCenter = (LilleNumber)centerNum;
                data.lilleRight = (LilleNumber)rightNum;

            }

            return data;
        }

        /// <summary>
        /// 保留の色を決定する
        /// </summary>
        /// <param name="nowData"></param>
        /// <param name="subRandomNum"></param>
        /// <param name="leftNum"></param>
        /// <returns></returns>
        private HoryuuColor DecideHoryuuColor(SlotData nowData,int subRandomNum)
        {
            HoryuuColor color = HoryuuColor.Normal;
            bool errorFlg = false;

            if (naibuMode == Atari.Tuujou)
            {
                if (nowData.atari == Atari.Hazure)
                {
                    if (subRandomNum < (RANDOM_RANGE / 50))
                    {
                        color = HoryuuColor.Blue;
                    }
                    else
                    {
                        color = HoryuuColor.Normal;
                    }
                }
                else if (nowData.atari == Atari.Reach)
                {
                    if (subRandomNum < (RANDOM_RANGE / 60))
                    {
                        color = HoryuuColor.Purple;
                    }
                    else if (subRandomNum < (RANDOM_RANGE / 30))
                    {
                        color = HoryuuColor.Green;
                    }
                    else if (subRandomNum < (RANDOM_RANGE / 10))
                    {
                        color = HoryuuColor.Blue;
                    }
                    else
                    {
                        color = HoryuuColor.Normal;
                    }
                }
                else if(nowData.atari == Atari.Tuujou)
                {
                    if (subRandomNum < (RANDOM_RANGE / 50))
                    {
                        color = HoryuuColor.Red;
                    }
                    else if (subRandomNum < (RANDOM_RANGE / 30))
                    {
                        color = HoryuuColor.Purple;
                    }
                    else if (subRandomNum < (RANDOM_RANGE / 10))
                    {
                        color = HoryuuColor.Green;
                    }
                    else if (subRandomNum < (RANDOM_RANGE / 5))
                    {
                        color = HoryuuColor.Blue;
                    }
                    else
                    {
                        color = HoryuuColor.Normal;
                    }
                }
                else if(nowData.atari == Atari.Kakuhen)
                {
                    if (subRandomNum < (RANDOM_RANGE / 30))
                    {
                        color = HoryuuColor.Red;
                    }
                    else if (subRandomNum < (RANDOM_RANGE / 15))
                    {
                        color = HoryuuColor.Purple;
                    }
                    else if (subRandomNum < (RANDOM_RANGE / 5))
                    {
                        color = HoryuuColor.Green;
                    }
                    else if (subRandomNum < (RANDOM_RANGE / 3))
                    {
                        color = HoryuuColor.Blue;
                    }
                    else
                    {
                        color = HoryuuColor.Normal;
                    }
                }
                else if (nowData.atari == Atari.Super)
                {
                    if (subRandomNum < (RANDOM_RANGE / 20))
                    {
                        color = HoryuuColor.Red;
                    }
                    else if (subRandomNum < (RANDOM_RANGE / 10))
                    {
                        color = HoryuuColor.Purple;
                    }
                    else if (subRandomNum < (RANDOM_RANGE / 4))
                    {
                        color = HoryuuColor.Green;
                    }
                    else if (subRandomNum < (RANDOM_RANGE / 2))
                    {
                        color = HoryuuColor.Blue;
                    }
                    else
                    {
                        color = HoryuuColor.Normal;
                    }
                }
                else
                {
                    errorFlg = true;
                }
            }
            else if (naibuMode == Atari.Kakuhen)
            {
                if (nowData.atari == Atari.Hazure)
                {
                    color = HoryuuColor.Normal;
                }
                else if (nowData.atari == Atari.Reach)
                {
                    if (0 == (int)nowData.lilleLeft % 2)
                    {
                        color = HoryuuColor.Normal;
                    }
                    else
                    {
                        if (subRandomNum < (RANDOM_RANGE / 50))
                        {
                            color = HoryuuColor.Purple;
                        }
                        else if (subRandomNum < (RANDOM_RANGE / 20))
                        {
                            color = HoryuuColor.Green;
                        }
                        else if (subRandomNum < (RANDOM_RANGE / 5))
                        {
                            color = HoryuuColor.Blue;
                        }
                        else
                        {
                            color = HoryuuColor.Normal;
                        }
                    }
                }
                else if (nowData.atari == Atari.Tuujou)
                {
                    color = HoryuuColor.Normal;
                }
                else if (nowData.atari == Atari.Kakuhen)
                {
                    if (subRandomNum < (RANDOM_RANGE / 20))
                    {
                        color = HoryuuColor.Red;
                    }
                    else if (subRandomNum < (RANDOM_RANGE / 10))
                    {
                        color = HoryuuColor.Purple;
                    }
                    else if (subRandomNum < (RANDOM_RANGE / 5))
                    {
                        color = HoryuuColor.Green;
                    }
                    else if (subRandomNum < (RANDOM_RANGE / 3))
                    {
                        color = HoryuuColor.Blue;
                    }
                    else
                    {
                        color = HoryuuColor.Normal;
                    }
                }
                else if (nowData.atari == Atari.Super)
                {
                    if (subRandomNum < (RANDOM_RANGE / 10))
                    {
                        color = HoryuuColor.Red;
                    }
                    else if (subRandomNum < (RANDOM_RANGE / 8))
                    {
                        color = HoryuuColor.Purple;
                    }
                    else if (subRandomNum < (RANDOM_RANGE / 4))
                    {
                        color = HoryuuColor.Green;
                    }
                    else if (subRandomNum < (RANDOM_RANGE / 2))
                    {
                        color = HoryuuColor.Blue;
                    }
                    else
                    {
                        color = HoryuuColor.Normal;
                    }
                }
                else
                {
                    errorFlg = true;
                }
            }
            else if(naibuMode == Atari.Super)
            {
                if (nowData.atari == Atari.Super)
                {
                    if (subRandomNum < (RANDOM_RANGE / 10))
                    {
                        color = HoryuuColor.Red;
                    }
                    else
                    {
                        color = HoryuuColor.Normal;
                    }
                }
                else
                {
                    color = HoryuuColor.Normal;
                }

            }
            else
            {
                errorFlg = true;
            }

            if (errorFlg)
            {
                Debug.LogError($"GameManager.DecideHoryuuColor : naibuMode is {naibuMode} , nowData is {nowData}");
            }

            return color;
        }

        /// <summary>
        /// スロットを回す
        /// </summary>
        private void RotateSlot()
        {
            if (slotData.Count <= 0) return;
            if (waitSlot == true) return;

            StartCoroutine(RotateSlotCoroutine());
        }

        /// <summary>
        /// スロットのコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator RotateSlotCoroutine()
        {
            waitSlot = true;
            reachFlg = (slotData[0].lilleLeft == slotData[0].lilleRight) ? true : false;

            UpdateHoryuuColors();

            // リールの回転を開始する
            Lilles[0].StartSpin();
            Lilles[1].StartSpin();
            Lilles[2].StartSpin();
            SoundManager.Instance.PlaySE(SoundManager.SoundSeType.SlotRotate);

            if (slotData.Count > 6)
            {
                yield return new WaitForSeconds(0.5f);
            }
            else if (slotData.Count > 3)
            {
                yield return new WaitForSeconds(2);
            }
            else
            {
                yield return new WaitForSeconds(4);
            }

            // 左リールを止める
            Lilles[0].StopNumber((int)slotData[0].lilleLeft, false, (int)slotData[0].lilleLeft);

            while (!Lilles[0].GetFinishFlg())
            {
                yield return null;
            }

            // 右リールを止める
            Lilles[2].StopNumber((int)slotData[0].lilleRight, false, (int)slotData[0].lilleLeft);

            while (!Lilles[2].GetFinishFlg())
            {
                yield return null;
            }

            bool myReachFlg = reachFlg;

            //リーチ演出
            if (reachFlg)
            {
                //リーチ演出用のスクリプトへ演出を依頼する
                uIAnimationPanel.ReachAnimation();

                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Reach);

                while (reachFlg)
                {
                    yield return null;
                }
            }

            // 中リールを止める
            Lilles[1].StopNumber((int)slotData[0].lilleCenter, myReachFlg, (int)slotData[0].lilleLeft);

            while (!Lilles[1].GetFinishFlg())
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.5f);

            // ハズれたとき
            if (slotData[0].atari == Atari.Hazure || slotData[0].atari == Atari.Reach)
            {
                horyuuLamp[0].ChangeHoryuuColor(HoryuuColor.None);
            }
            // 当たったとき
            else
            {
                yield return new WaitForSeconds(1.0f);

                if (mitameMode != slotData[0].atari)
                {
                    // 現在の確率の見た目のモードを変更する
                    SetMitameMode(slotData[0].atari);
                }

                // 当たりの種類によって払い出し枚数を決めて伝える
                if(slotData[0].atari == Atari.Super)
                {
                    uIAnimationPanel.AcquisitionMedalAnimation(PAY_MEDALS_SUPER);
                    medalPayFall.AddFallMedals(PAY_MEDALS_SUPER);
                    SoundManager.Instance.PlaySE(SoundManager.SoundSeType.GetTuujou);
                }
                else if (slotData[0].atari == Atari.Kakuhen)
                {
                    uIAnimationPanel.AcquisitionMedalAnimation(PAY_MEDALS_KAKUHEN);
                    medalPayFall.AddFallMedals(PAY_MEDALS_KAKUHEN);
                    SoundManager.Instance.PlaySE(SoundManager.SoundSeType.GetKakuhen);
                }
                else if (slotData[0].atari == Atari.Tuujou)
                {
                    uIAnimationPanel.AcquisitionMedalAnimation(PAY_MEDALS_TUUJOU);
                    medalPayFall.AddFallMedals(PAY_MEDALS_TUUJOU);
                    SoundManager.Instance.PlaySE(SoundManager.SoundSeType.GetSuper);
                }

                PlayerInformationManager.Instance.IncrementField("Pusher01Data", "bonusTimes");

                yield return new WaitForSeconds(0.5f);
                horyuuLamp[0].ChangeHoryuuColor(HoryuuColor.None);
            }

            yield return new WaitForSeconds(1.0f);

            slotData.RemoveAt(0);
            waitSlot = false;

        }

        /// <summary>
        /// 保留の色をすべて更新する
        /// </summary>
        private void UpdateHoryuuColors()
        {
            for (int i = 0; i < horyuuLamp.Length; i++) 
            {
                if (i < slotData.Count)
                {
                    horyuuLamp[i].ChangeHoryuuColor(slotData[i].horyuuColor);
                }
                else
                {
                    horyuuLamp[i].ChangeHoryuuColor(HoryuuColor.None);
                }
            }
        }

        /// <summary>
        /// reachFlgのセッター
        /// </summary>
        /// <param name="flg">変えたいフラグ</param>
        public void SetReachFlg(bool flg)
        {
            reachFlg = flg;
        }

        /// <summary>
        /// 見た目モードを変更する
        /// </summary>
        /// <param name="mode"></param>
        private void SetMitameMode(Atari mode)
        {
            mitameMode = mode;
            Debug.Log($"見た目モード変更 : {mitameMode}");

            // BGMの変更
            if (mode == Atari.Tuujou)
            {
                SoundManager.Instance.ChangeAndPlayBgm(SoundManager.SoundBgmType.Tuujou);
            }
            else if (mode == Atari.Kakuhen)
            {
                SoundManager.Instance.ChangeAndPlayBgm(SoundManager.SoundBgmType.Kakuhen);
            }
            else if (mode == Atari.Super)
            {
                SoundManager.Instance.ChangeAndPlayBgm(SoundManager.SoundBgmType.Super);
            }

            //変更する背景色を決定
            Color color = Color.white;
            if (mode == Atari.Tuujou)
            {
                color = Color.blue;
            }
            else if (mode == Atari.Kakuhen)
            {
                color = Color.red;
            }
            else if (mode == Atari.Super)
            {
                color = Color.yellow;
            }

            //グラデーションの作成
            Gradient grad = new Gradient();
            grad.SetKeys(
                new GradientColorKey[] { new GradientColorKey(color, 0.0f) },
                new GradientAlphaKey[]
                {
                    new GradientAlphaKey(0.0f, 0.0f),
                    new GradientAlphaKey(1.0f, 0.2f),
                    new GradientAlphaKey(1.0f, 0.8f),
                    new GradientAlphaKey(0.0f, 1.0f)
                });

            //ColorOverLifetimeを上書き
            var colt = backgroundParticle.colorOverLifetime;
            colt.color = new ParticleSystem.MinMaxGradient(grad);
        }

        /// <summary>
        /// 内部モードを変更する
        /// </summary>
        /// <param name="mode"></param>
        private void SetNaibuMode(Atari mode)
        {
            naibuMode = mode;
            Debug.Log($"内部モード変更 : {naibuMode}");
        }

        /// <summary>
        /// 光の設定ボタンを押したとき
        /// </summary>
        public void PushSettingLightButton()
        {
            uiSeManager.PlaySE(UiSeManager.SoundSeType.SE_Yes);

            if (lightQualityFlg)
            {
                nowQualityText.text = "現在の設定\n[高品質]";
            }
            else
            {
                nowQualityText.text = "現在の設定\n[低品質]";
            }

            settingLightPanel.SetActive(true);
        }

        /// <summary>
        /// 光の品質を選ぶボタンを押したとき
        /// </summary>
        /// <param name="flg">true:高品質 , false:低品質</param>
        public void PushLightQualityButton(bool flg)
        {
            if (flg)
            {
                lightQualityFlg = true;
                pointLightsObject.SetActive(true);
                directionalLightObject.SetActive(false);
                PlayerPrefs.SetInt(LIGHT_QUALITY_KEY, LIGHT_QUALITY_HIGH);
            }
            else
            {
                lightQualityFlg = false;
                pointLightsObject.SetActive(false);
                directionalLightObject.SetActive(true);
                PlayerPrefs.SetInt(LIGHT_QUALITY_KEY, LIGHT_QUALITY_LOW);
            }

            uiSeManager.PlaySE(UiSeManager.SoundSeType.SE_OK);
            settingLightPanel.SetActive(false);
        }

        /// <summary>
        /// 光の設定の初期化
        /// </summary>
        private void InitSettingLight()
        {
            int flgInt = PlayerPrefs.GetInt(LIGHT_QUALITY_KEY, LIGHT_QUALITY_LOW);

            if (flgInt == LIGHT_QUALITY_HIGH)
            {
                lightQualityFlg = true;
                pointLightsObject.SetActive(true);
                directionalLightObject.SetActive(false);
            }
            else
            {
                lightQualityFlg = false;
                pointLightsObject.SetActive(false);
                directionalLightObject.SetActive(true);
            }

            settingLightPanel.SetActive(false);
        }

    }
}

