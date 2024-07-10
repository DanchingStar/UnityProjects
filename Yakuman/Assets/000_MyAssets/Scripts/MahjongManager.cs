//#define Yokogamen

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MahjongManager : MonoBehaviour
{
    #region _Region【宣言(Enum,Class)】

    /// <summary>牌の種類</summary>
    public enum PaiKinds
    {
        None_00,
        M1,
        M2,
        M3,
        M4,
        M5,
        M6,
        M7,
        M8,
        M9,
        None_10,
        P1,
        P2,
        P3,
        P4,
        P5,
        P6,
        P7,
        P8,
        P9,
        None_20,
        S1,
        S2,
        S3,
        S4,
        S5,
        S6,
        S7,
        S8,
        S9,
        None_30,
        J1,
        J2,
        J3,
        J4,
        J5,
        J6,
        J7,
    }

    /// <summary>鳴きの種類</summary>
    public enum NakiKinds
    {
        None,
        Menzen,
        Ankan,
        MinKan,
        DaiminkanFromShimocha,
        DaiminkanFromToimen,
        DaiminkanFromKamicha,
        KakanFromShimocha,
        KakanFromToimen,
        KakanFromKamicha,
        Pon,
        PonFromShimocha,
        PonFromToimen,
        PonFromKamicha,
        Chi,
        ChiNumSmall,
        ChiNumMiddle,
        ChiNumBig,
        Ron,
    }

    /// <summary>色(萬子とか)の種類</summary>
    public enum IroKinds
    {
        None,
        Manzu,
        Pinzu,
        Souzu,
        Jihai,
    }

    /// <summary>メンツの種類</summary>
    public enum MentsuKinds
    {
        None,
        Juntsu,
        Kootsu,
        Kantsu,
    }

    /// <summary>ゲーム進行のターンの種類</summary>
    public enum GameTurn
    {
        Ready,
        Ton_Tsumo,
        Ton_Sute,
        Nan_Tsumo,
        Nan_Sute,
        Sha_Tsumo,
        Sha_Sute,
        Pe_Tsumo,
        Pe_Sute,
        Finish_Ryuukyoku,
        Finish_Agari,
    }

    /// <summary>局の種類</summary>
    public enum Kyoku
    {
        None,
        Ton1,
        Ton2,
        Ton3,
        Ton4,
        Nan1,
        Nan2,
        Nan3,
        Nan4,
    }

    /// <summary>プレイヤーの種類</summary>
    public enum PlayerKind
    {
        Other,
        Player,
        Shimocha,
        Toimen,
        Kamicha,
    }

    /// <summary>プレイヤーが自ターンにできるアクションの種類</summary>
    public enum TurnActionKind
    {
        None,
        Tsumo,
        Ryuukyoku,
        Reach,
        Ankan,
        Kakan,
    }

    /// <summary>途中流局の種類</summary>
    public enum RyuukyokuOfTochuu
    {
        None,
        Kyuusyu,
        SuuKantsu,
        SuuFuuRenda,
        YoninReach,
    }

    /// <summary>役満の種類</summary>
    public enum YakumanKind
    {
        None,
        TenHo,
        SuAnKo,
        SuKanTsu,
        DaiSanGen,
        SyouSuuShi,
        DaiSuuShi,
        TsuIiSo,
        ChinRouTo,
        RyuIiSo,
        KokuShiMuSou,
        ChuRenPoTo,
    }

    /// <summary>ローカル役満の種類</summary>
    public enum YakumanOfLocalKind
    {
        None,
        SuRenKo,
        DaiShaRin,
        BeniKujaku,
        HyakuManGoku,
        DaiChiShin,
    }

    /// <summary>完成メンツの属性</summary>
    public class MentsuStatus
    {
        public PaiKinds minimumPai;
        public bool tanyao;
        public MentsuKinds mentsuKind;
        public int fu;
        public NakiKinds nakiKinds;
        public IroKinds iroKinds;
        public bool midori;

        public MentsuStatus(PaiKinds _minimumPai, bool _tanyao, MentsuKinds _mentsuKind, int _fu, NakiKinds _nakiKinds, IroKinds _iroKinds, bool _midori)
        {
            minimumPai = _minimumPai;
            tanyao = _tanyao;
            mentsuKind = _mentsuKind;
            fu = _fu;
            nakiKinds = _nakiKinds;
            iroKinds = _iroKinds;
            midori = _midori;
        }

        public void DataCopy(MentsuStatus _mentsuStatus)
        {
            minimumPai = _mentsuStatus.minimumPai;
            tanyao = _mentsuStatus.tanyao;
            mentsuKind = _mentsuStatus.mentsuKind;
            fu = _mentsuStatus.fu;
            nakiKinds = _mentsuStatus.nakiKinds;
            iroKinds = _mentsuStatus.iroKinds;
            midori = _mentsuStatus.midori;
        }
    }

    /// <summary>麻雀牌(物)の属性</summary>
    public class PaiStatus
    {
        public PaiKinds thisKind;
        public int totalNumber;

        public PaiStatus(PaiKinds _thisKind, int _totalNumber)
        {
            thisKind = _thisKind;
            totalNumber = _totalNumber;
        }
    }


    #endregion

    #region _Region【宣言(const,readonly)】

    private const int DEFAULT_MAISUU_TEHAI = 13;
    private const float TIME_TURN_INTERVAL = 0.5f;
    private const int COUNT_FINISH_TSUMO = 70;
    public const int INDEX_ERROR = -1;
    public const int INDEX_NONE = -20;

#if Yokogamen // 横画面

    private readonly Vector3 DEFAULT_POSITION_TEHAI_PLAYER = new Vector3(-13.82f, 11.5f, -9.5f);
    private readonly Vector3 DEFAULT_POSITION_TEHAI_SHIMOCHA = new Vector3(25, 1.5f, -13f);
    private readonly Vector3 DEFAULT_POSITION_TEHAI_TOIMEN = new Vector3(13, 1.5f, 14f);
    private readonly Vector3 DEFAULT_POSITION_TEHAI_KAMICHA = new Vector3(-25, 1.5f, 13f);

    private readonly Vector3 DEFAULT_POSITION_KAWA_PLAYER = new Vector3(-5f, 0.9f, -4.25f);
    private readonly Vector3 DEFAULT_POSITION_KAWA_SHIMOCHA = new Vector3(9.25f, 0.9f, -5f);
    private readonly Vector3 DEFAULT_POSITION_KAWA_TOIMEN = new Vector3(5f, 0.9f, 4.25f);
    private readonly Vector3 DEFAULT_POSITION_KAWA_KAMICHA = new Vector3(-9.25f, 0.9f, 5f);

    private readonly Vector3 DEFAULT_POSITION_DORA = new Vector3(-4f, 2.4f, 0f);

#else //縦画面

    private readonly Vector3 DEFAULT_POSITION_TEHAI_PLAYER = new Vector3(-13.48f, 9.2f, -22f);
    private readonly Vector3 DEFAULT_POSITION_TEHAI_SHIMOCHA = new Vector3(16, 1.5f, -13f);
    private readonly Vector3 DEFAULT_POSITION_TEHAI_TOIMEN = new Vector3(13.48f, 1.5f, 20f);
    private readonly Vector3 DEFAULT_POSITION_TEHAI_KAMICHA = new Vector3(-16, 1.5f, 13f);

    private readonly Vector3 DEFAULT_POSITION_NAKI_PLAYER = new Vector3(14.5f, 9.2f, -25f);
    private readonly Vector3 DEFAULT_POSITION_NAKI_SHIMOCHA = new Vector3(13.66f, 1.5f, -13f);
    private readonly Vector3 DEFAULT_POSITION_NAKI_TOIMEN = new Vector3(-11.14f, 1.5f, 20f);
    private readonly Vector3 DEFAULT_POSITION_NAKI_KAMICHA = new Vector3(-13.66f, 1.5f, 13f);

    private readonly Vector3 DEFAULT_POSITION_KAWA_PLAYER = new Vector3(-5f, 0.9f, -9.8f);
    private readonly Vector3 DEFAULT_POSITION_KAWA_SHIMOCHA = new Vector3(7.3f, 0.9f, -5f);
    private readonly Vector3 DEFAULT_POSITION_KAWA_TOIMEN = new Vector3(5f, 0.9f, 8.4f);
    private readonly Vector3 DEFAULT_POSITION_KAWA_KAMICHA = new Vector3(-7.3f, 0.9f, 5f);

    private readonly Vector3 DEFAULT_POSITION_DORA = new Vector3(-4f, 2.4f, 3.2f);

#endif

    private readonly Vector3 DEFAULT_ROTATE_PLAYER = new Vector3(270, 180, 0);
    private readonly Vector3 DEFAULT_ROTATE_SHIMOCHA_TEHAI = new Vector3(0, 90, 0);
    private readonly Vector3 DEFAULT_ROTATE_SHIMOCHA_KAWA = new Vector3(270, 0, 90);
    private readonly Vector3 DEFAULT_ROTATE_TOIMEN_TEHAI = new Vector3(0, 0, 0);
    private readonly Vector3 DEFAULT_ROTATE_TOIMEN_KAWA = new Vector3(270, 0, 0);
    private readonly Vector3 DEFAULT_ROTATE_KAMICHA_TEHAI = new Vector3(0, 270, 0);
    private readonly Vector3 DEFAULT_ROTATE_KAMICHA_KAWA = new Vector3(270, 0, 270);
    private readonly Vector3 DEFAULT_ROTATE_URA = new Vector3(90, 0, 0);

    private readonly MentsuStatus MenzenM123 = new MentsuStatus(PaiKinds.M1, false, MentsuKinds.Juntsu, 0, NakiKinds.Menzen, IroKinds.Manzu, false);
    private readonly MentsuStatus MenzenM234 = new MentsuStatus(PaiKinds.M2, true, MentsuKinds.Juntsu, 0, NakiKinds.Menzen, IroKinds.Manzu, false);
    private readonly MentsuStatus MenzenM345 = new MentsuStatus(PaiKinds.M3, true, MentsuKinds.Juntsu, 0, NakiKinds.Menzen, IroKinds.Manzu, false);
    private readonly MentsuStatus MenzenM456 = new MentsuStatus(PaiKinds.M4, true, MentsuKinds.Juntsu, 0, NakiKinds.Menzen, IroKinds.Manzu, false);
    private readonly MentsuStatus MenzenM567 = new MentsuStatus(PaiKinds.M5, true, MentsuKinds.Juntsu, 0, NakiKinds.Menzen, IroKinds.Manzu, false);
    private readonly MentsuStatus MenzenM678 = new MentsuStatus(PaiKinds.M6, true, MentsuKinds.Juntsu, 0, NakiKinds.Menzen, IroKinds.Manzu, false);
    private readonly MentsuStatus MenzenM789 = new MentsuStatus(PaiKinds.M7, false, MentsuKinds.Juntsu, 0, NakiKinds.Menzen, IroKinds.Manzu, false);

    private readonly MentsuStatus MenzenP123 = new MentsuStatus(PaiKinds.P1, false, MentsuKinds.Juntsu, 0, NakiKinds.Menzen, IroKinds.Pinzu, false);
    private readonly MentsuStatus MenzenP234 = new MentsuStatus(PaiKinds.P2, true, MentsuKinds.Juntsu, 0, NakiKinds.Menzen, IroKinds.Pinzu, false);
    private readonly MentsuStatus MenzenP345 = new MentsuStatus(PaiKinds.P3, true, MentsuKinds.Juntsu, 0, NakiKinds.Menzen, IroKinds.Pinzu, false);
    private readonly MentsuStatus MenzenP456 = new MentsuStatus(PaiKinds.P4, true, MentsuKinds.Juntsu, 0, NakiKinds.Menzen, IroKinds.Pinzu, false);
    private readonly MentsuStatus MenzenP567 = new MentsuStatus(PaiKinds.P5, true, MentsuKinds.Juntsu, 0, NakiKinds.Menzen, IroKinds.Pinzu, false);
    private readonly MentsuStatus MenzenP678 = new MentsuStatus(PaiKinds.P6, true, MentsuKinds.Juntsu, 0, NakiKinds.Menzen, IroKinds.Pinzu, false);
    private readonly MentsuStatus MenzenP789 = new MentsuStatus(PaiKinds.P7, false, MentsuKinds.Juntsu, 0, NakiKinds.Menzen, IroKinds.Pinzu, false);

    private readonly MentsuStatus MenzenS123 = new MentsuStatus(PaiKinds.S1, false, MentsuKinds.Juntsu, 0, NakiKinds.Menzen, IroKinds.Souzu, false);
    private readonly MentsuStatus MenzenS234 = new MentsuStatus(PaiKinds.S2, true, MentsuKinds.Juntsu, 0, NakiKinds.Menzen, IroKinds.Souzu, true);
    private readonly MentsuStatus MenzenS345 = new MentsuStatus(PaiKinds.S3, true, MentsuKinds.Juntsu, 0, NakiKinds.Menzen, IroKinds.Souzu, false);
    private readonly MentsuStatus MenzenS456 = new MentsuStatus(PaiKinds.S4, true, MentsuKinds.Juntsu, 0, NakiKinds.Menzen, IroKinds.Souzu, false);
    private readonly MentsuStatus MenzenS567 = new MentsuStatus(PaiKinds.S5, true, MentsuKinds.Juntsu, 0, NakiKinds.Menzen, IroKinds.Souzu, false);
    private readonly MentsuStatus MenzenS678 = new MentsuStatus(PaiKinds.S6, true, MentsuKinds.Juntsu, 0, NakiKinds.Menzen, IroKinds.Souzu, false);
    private readonly MentsuStatus MenzenS789 = new MentsuStatus(PaiKinds.S7, false, MentsuKinds.Juntsu, 0, NakiKinds.Menzen, IroKinds.Souzu, false);

    private readonly MentsuStatus MenzenM111 = new MentsuStatus(PaiKinds.M1, false, MentsuKinds.Kootsu, 8, NakiKinds.Menzen, IroKinds.Manzu, false);
    private readonly MentsuStatus MenzenM222 = new MentsuStatus(PaiKinds.M2, true, MentsuKinds.Kootsu, 4, NakiKinds.Menzen, IroKinds.Manzu, false);
    private readonly MentsuStatus MenzenM333 = new MentsuStatus(PaiKinds.M3, true, MentsuKinds.Kootsu, 4, NakiKinds.Menzen, IroKinds.Manzu, false);
    private readonly MentsuStatus MenzenM444 = new MentsuStatus(PaiKinds.M4, true, MentsuKinds.Kootsu, 4, NakiKinds.Menzen, IroKinds.Manzu, false);
    private readonly MentsuStatus MenzenM555 = new MentsuStatus(PaiKinds.M5, true, MentsuKinds.Kootsu, 4, NakiKinds.Menzen, IroKinds.Manzu, false);
    private readonly MentsuStatus MenzenM666 = new MentsuStatus(PaiKinds.M6, true, MentsuKinds.Kootsu, 4, NakiKinds.Menzen, IroKinds.Manzu, false);
    private readonly MentsuStatus MenzenM777 = new MentsuStatus(PaiKinds.M7, true, MentsuKinds.Kootsu, 4, NakiKinds.Menzen, IroKinds.Manzu, false);
    private readonly MentsuStatus MenzenM888 = new MentsuStatus(PaiKinds.M8, true, MentsuKinds.Kootsu, 4, NakiKinds.Menzen, IroKinds.Manzu, false);
    private readonly MentsuStatus MenzenM999 = new MentsuStatus(PaiKinds.M9, false, MentsuKinds.Kootsu, 8, NakiKinds.Menzen, IroKinds.Manzu, false);

    private readonly MentsuStatus MenzenP111 = new MentsuStatus(PaiKinds.P1, false, MentsuKinds.Kootsu, 8, NakiKinds.Menzen, IroKinds.Pinzu, false);
    private readonly MentsuStatus MenzenP222 = new MentsuStatus(PaiKinds.P2, true, MentsuKinds.Kootsu, 4, NakiKinds.Menzen, IroKinds.Pinzu, false);
    private readonly MentsuStatus MenzenP333 = new MentsuStatus(PaiKinds.P3, true, MentsuKinds.Kootsu, 4, NakiKinds.Menzen, IroKinds.Pinzu, false);
    private readonly MentsuStatus MenzenP444 = new MentsuStatus(PaiKinds.P4, true, MentsuKinds.Kootsu, 4, NakiKinds.Menzen, IroKinds.Pinzu, false);
    private readonly MentsuStatus MenzenP555 = new MentsuStatus(PaiKinds.P5, true, MentsuKinds.Kootsu, 4, NakiKinds.Menzen, IroKinds.Pinzu, false);
    private readonly MentsuStatus MenzenP666 = new MentsuStatus(PaiKinds.P6, true, MentsuKinds.Kootsu, 4, NakiKinds.Menzen, IroKinds.Pinzu, false);
    private readonly MentsuStatus MenzenP777 = new MentsuStatus(PaiKinds.P7, true, MentsuKinds.Kootsu, 4, NakiKinds.Menzen, IroKinds.Pinzu, false);
    private readonly MentsuStatus MenzenP888 = new MentsuStatus(PaiKinds.P8, true, MentsuKinds.Kootsu, 4, NakiKinds.Menzen, IroKinds.Pinzu, false);
    private readonly MentsuStatus MenzenP999 = new MentsuStatus(PaiKinds.P9, false, MentsuKinds.Kootsu, 8, NakiKinds.Menzen, IroKinds.Pinzu, false);

    private readonly MentsuStatus MenzenS111 = new MentsuStatus(PaiKinds.S1, false, MentsuKinds.Kootsu, 8, NakiKinds.Menzen, IroKinds.Souzu, false);
    private readonly MentsuStatus MenzenS222 = new MentsuStatus(PaiKinds.S2, true, MentsuKinds.Kootsu, 4, NakiKinds.Menzen, IroKinds.Souzu, true);
    private readonly MentsuStatus MenzenS333 = new MentsuStatus(PaiKinds.S3, true, MentsuKinds.Kootsu, 4, NakiKinds.Menzen, IroKinds.Souzu, true);
    private readonly MentsuStatus MenzenS444 = new MentsuStatus(PaiKinds.S4, true, MentsuKinds.Kootsu, 4, NakiKinds.Menzen, IroKinds.Souzu, true);
    private readonly MentsuStatus MenzenS555 = new MentsuStatus(PaiKinds.S5, true, MentsuKinds.Kootsu, 4, NakiKinds.Menzen, IroKinds.Souzu, false);
    private readonly MentsuStatus MenzenS666 = new MentsuStatus(PaiKinds.S6, true, MentsuKinds.Kootsu, 4, NakiKinds.Menzen, IroKinds.Souzu, true);
    private readonly MentsuStatus MenzenS777 = new MentsuStatus(PaiKinds.S7, true, MentsuKinds.Kootsu, 4, NakiKinds.Menzen, IroKinds.Souzu, false);
    private readonly MentsuStatus MenzenS888 = new MentsuStatus(PaiKinds.S8, true, MentsuKinds.Kootsu, 4, NakiKinds.Menzen, IroKinds.Souzu, true);
    private readonly MentsuStatus MenzenS999 = new MentsuStatus(PaiKinds.S9, false, MentsuKinds.Kootsu, 8, NakiKinds.Menzen, IroKinds.Souzu, false);

    private readonly MentsuStatus MenzenJ111 = new MentsuStatus(PaiKinds.J1, false, MentsuKinds.Kootsu, 8, NakiKinds.Menzen, IroKinds.Jihai, false);
    private readonly MentsuStatus MenzenJ222 = new MentsuStatus(PaiKinds.J2, false, MentsuKinds.Kootsu, 8, NakiKinds.Menzen, IroKinds.Jihai, false);
    private readonly MentsuStatus MenzenJ333 = new MentsuStatus(PaiKinds.J3, false, MentsuKinds.Kootsu, 8, NakiKinds.Menzen, IroKinds.Jihai, false);
    private readonly MentsuStatus MenzenJ444 = new MentsuStatus(PaiKinds.J4, false, MentsuKinds.Kootsu, 8, NakiKinds.Menzen, IroKinds.Jihai, false);
    private readonly MentsuStatus MenzenJ555 = new MentsuStatus(PaiKinds.J5, false, MentsuKinds.Kootsu, 8, NakiKinds.Menzen, IroKinds.Jihai, false);
    private readonly MentsuStatus MenzenJ666 = new MentsuStatus(PaiKinds.J6, false, MentsuKinds.Kootsu, 8, NakiKinds.Menzen, IroKinds.Jihai, true);
    private readonly MentsuStatus MenzenJ777 = new MentsuStatus(PaiKinds.J7, false, MentsuKinds.Kootsu, 8, NakiKinds.Menzen, IroKinds.Jihai, false);

    #endregion

    #region _Region【宣言(変数)】

    [SerializeField] private UiManager uiManager;
    [SerializeField] private Transform paiParentTransform;
    [SerializeField] private GameObject paiPrefab;
    [SerializeField] private GameObject[] nakiPrefab;
    [SerializeField] private GameObject ankanPrefab;
    [SerializeField] private Material[] garaMaterials;
    [SerializeField] private Sprite[] garaSprites;
    [SerializeField] private PlayerTehai[] playerTehais;
    [SerializeField] private PlayerKawa[] playerKawas;
    [SerializeField] private Wanpai wanpai;

    public List<PaiStatus> paiyama;

    private Kyoku nowKyoku;
    private int nowHonba;
    private int nowPaiIndex;
    private GameTurn nowGameTurn;
    private int nowTsumoCount;

    private bool nakiWaitFlg;
    private TurnActionKind turnActionFlg;

    private float gameTimer;

    #endregion

    public static MahjongManager Instance;
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

    }

    private void Update()
    {
        UpdateFunction();
    }

    /// <summary>
    /// Updateで走る関数
    /// </summary>
    private void UpdateFunction()
    {
        if (nowGameTurn == GameTurn.Finish_Ryuukyoku)
        {
            //何もせず、待ち
        }
        else if (nowGameTurn == GameTurn.Ton_Sute)
        {
            NextGameTurn();
        }
        else if (nowGameTurn != GameTurn.Ready && nowGameTurn != GameTurn.Ton_Tsumo)
        {
            gameTimer += Time.deltaTime;

            if (gameTimer > TIME_TURN_INTERVAL)
            {
                if (GetNowTurnTsumo() && nowGameTurn != GameTurn.Ton_Tsumo)
                {
                    int playerIndex = (int)nowGameTurn / 2;

                    bool thinkFlg = true; // CPUが思考してほしいならTrue
                    if (thinkFlg)
                    {
                        playerTehais[playerIndex].SuteThink(); // 評価値を計算して捨てる
                    }
                    else
                    {
                        int paiIndex = 0; // この値で捨てる牌が決まる
                        playerTehais[playerIndex].SuteMandatory(paiIndex, false); // 指定した牌を捨てる
                    }
                }
                else
                {
                    if (!nakiWaitFlg)
                    {
                        NextGameTurn();
                    }
                    else
                    {
                        return;
                    }
                }

                gameTimer = 0f;
            }
        }
    }

    /// <summary>
    /// 牌山を作る
    /// </summary>
    public void MakePaiYama()
    {
        ResetTransformObjects();
        ResetPlayersTehaisAndKawas();
        uiManager.ResetUi();

        paiyama = new List<PaiStatus>();
        nowPaiIndex = 0;

        // 牌山を生成する
        int numberIndex = 0;
        for (int i = 0; i < System.Enum.GetValues(typeof(PaiKinds)).Length; i++) 
        {
            if (i % 10 != 0)
            {
                for (int j = 0; j < 4; j++)
                {
                    paiyama.Add(new PaiStatus((PaiKinds)i, numberIndex));
                    numberIndex++;
                }
            }
        }
        
        // イカサマする。イカサマした牌の数が返り値
        int ikasamaCounter = IkasamaContents();

        // イカサマした牌以外を混ぜる
        for (int i = ikasamaCounter; i < paiyama.Count; i++)
        {
            var j = Random.Range(ikasamaCounter, paiyama.Count - ikasamaCounter);
            var temp = paiyama[i];
            paiyama[i] = paiyama[j];
            paiyama[j] = temp;
        }

        // イカサマした場合、手牌を混ぜる
        if(ikasamaCounter > 0)
        {
            for (int i = 0; i < 13; i++)
            {
                var j = Random.Range(0, 13);
                var temp = paiyama[i];
                paiyama[i] = paiyama[j];
                paiyama[j] = temp;
            }
        }

        // 混ぜた後に山などを変えたければここ
        IkasamaAfterVersion();


        // 牌山のログ
        string paiyamaString = $"牌数 : {paiyama.Count}\n";
        foreach(var item in paiyama)
        {
            paiyamaString += $"{item.thisKind}({item.totalNumber}),";
        }
        Debug.Log(paiyamaString);

    }

    /// <summary>
    /// ゲームをはじめからスタートする
    /// </summary>
    private void GameStart()
    {
        nowKyoku = Kyoku.Ton1;
        nowHonba = 0;

        UiManagerChangeKyokuText();

        MakePaiYama();

        ResetKyoku();
    }

    /// <summary>
    /// 局単位の変数をリセットする
    /// </summary>
    private void ResetKyoku()
    {
        nowPaiIndex = 0;
        nowGameTurn = GameTurn.Ready;
        nowTsumoCount = 0;
        wanpai.ResetWanpai();

        nakiWaitFlg = false;
        turnActionFlg = TurnActionKind.None;

        gameTimer = 0;

        UiManagerChangeNokoriText();
    }

    /// <summary>
    /// イカサマするならここ
    /// </summary>
    /// <returns></returns>
    private int IkasamaContents()
    {
        int counter = 0;
        int num = 13; //この値を変更して好みのイカサマを実施
        switch (num)
        {
            case 0: //大四喜
                {
                    counter = IkasamaTehai(counter, PaiKinds.J1);
                    counter = IkasamaTehai(counter, PaiKinds.J1);
                    counter = IkasamaTehai(counter, PaiKinds.J1);
                    counter = IkasamaTehai(counter, PaiKinds.J2);
                    counter = IkasamaTehai(counter, PaiKinds.J2);
                    counter = IkasamaTehai(counter, PaiKinds.J2);
                    counter = IkasamaTehai(counter, PaiKinds.J3);
                    counter = IkasamaTehai(counter, PaiKinds.J3);
                    counter = IkasamaTehai(counter, PaiKinds.J3);
                    counter = IkasamaTehai(counter, PaiKinds.J4);
                    counter = IkasamaTehai(counter, PaiKinds.J4);
                    counter = IkasamaTehai(counter, PaiKinds.J4);
                }
                break;
            case 1: //チーのテスト1
                {
                    counter = IkasamaTehai(counter, PaiKinds.M2);
                    counter = IkasamaTehai(counter, PaiKinds.M3);
                    counter = IkasamaTehai(counter, PaiKinds.M4);
                    counter = IkasamaTehai(counter, PaiKinds.M5);
                    counter = IkasamaTehai(counter, PaiKinds.M6);
                    counter = IkasamaTehai(counter, PaiKinds.M7);
                    counter = IkasamaTehai(counter, PaiKinds.M8);
                    counter = IkasamaTehai(counter, PaiKinds.P5);
                    counter = IkasamaTehai(counter, PaiKinds.P5);
                    counter = IkasamaTehai(counter, PaiKinds.S4);
                    counter = IkasamaTehai(counter, PaiKinds.S5);
                    counter = IkasamaTehai(counter, PaiKinds.S5);
                    counter = IkasamaTehai(counter, PaiKinds.S6);
                }
                break;
            case 2: //チーのテスト2
                {
                    counter = IkasamaTehai(counter, PaiKinds.J1);
                    counter = IkasamaTehai(counter, PaiKinds.J2);
                    counter = IkasamaTehai(counter, PaiKinds.J3);
                    counter = IkasamaTehai(counter, PaiKinds.J4);
                    counter = IkasamaTehai(counter, PaiKinds.J5);
                    counter = IkasamaTehai(counter, PaiKinds.J6);
                    counter = IkasamaTehai(counter, PaiKinds.J7);
                    counter = IkasamaTehai(counter, PaiKinds.M1);
                    counter = IkasamaTehai(counter, PaiKinds.M9);
                    counter = IkasamaTehai(counter, PaiKinds.P1);
                    counter = IkasamaTehai(counter, PaiKinds.P9);
                }
                break;
            case 3: //字一色七対子
                {
                    counter = IkasamaTehai(counter, PaiKinds.J1);
                    counter = IkasamaTehai(counter, PaiKinds.J2);
                    counter = IkasamaTehai(counter, PaiKinds.J3);
                    counter = IkasamaTehai(counter, PaiKinds.J4);
                    counter = IkasamaTehai(counter, PaiKinds.J5);
                    counter = IkasamaTehai(counter, PaiKinds.J6);
                    counter = IkasamaTehai(counter, PaiKinds.J7);
                    counter = IkasamaTehai(counter, PaiKinds.J1);
                    counter = IkasamaTehai(counter, PaiKinds.J2);
                    counter = IkasamaTehai(counter, PaiKinds.J3);
                    counter = IkasamaTehai(counter, PaiKinds.J4);
                    counter = IkasamaTehai(counter, PaiKinds.J5);
                    counter = IkasamaTehai(counter, PaiKinds.J6);
                }
                break;
            case 4: //複雑な形の聴牌
                {
                    counter = IkasamaTehai(counter, PaiKinds.P4);
                    counter = IkasamaTehai(counter, PaiKinds.P4);
                    counter = IkasamaTehai(counter, PaiKinds.P4);
                    counter = IkasamaTehai(counter, PaiKinds.S2);
                    counter = IkasamaTehai(counter, PaiKinds.S3);
                    counter = IkasamaTehai(counter, PaiKinds.S4);
                    counter = IkasamaTehai(counter, PaiKinds.S5);
                    counter = IkasamaTehai(counter, PaiKinds.S5);
                    counter = IkasamaTehai(counter, PaiKinds.S5);
                    counter = IkasamaTehai(counter, PaiKinds.S6);
                    counter = IkasamaTehai(counter, PaiKinds.S7);
                    counter = IkasamaTehai(counter, PaiKinds.M8);
                    counter = IkasamaTehai(counter, PaiKinds.M8);
                }
                break;
            case 5: //九連宝灯
                {
                    counter = IkasamaTehai(counter, PaiKinds.M1);
                    counter = IkasamaTehai(counter, PaiKinds.M1);
                    counter = IkasamaTehai(counter, PaiKinds.M1);
                    counter = IkasamaTehai(counter, PaiKinds.M2);
                    counter = IkasamaTehai(counter, PaiKinds.M3);
                    counter = IkasamaTehai(counter, PaiKinds.M4);
                    counter = IkasamaTehai(counter, PaiKinds.M5);
                    counter = IkasamaTehai(counter, PaiKinds.M6);
                    counter = IkasamaTehai(counter, PaiKinds.M7);
                    counter = IkasamaTehai(counter, PaiKinds.M8);
                    counter = IkasamaTehai(counter, PaiKinds.M9);
                    counter = IkasamaTehai(counter, PaiKinds.M9);
                    counter = IkasamaTehai(counter, PaiKinds.M9);
                }
                break;
            case 6: //8面待ち
                {
                    counter = IkasamaTehai(counter, PaiKinds.P1);
                    counter = IkasamaTehai(counter, PaiKinds.P1);
                    counter = IkasamaTehai(counter, PaiKinds.P1);
                    counter = IkasamaTehai(counter, PaiKinds.P3);
                    counter = IkasamaTehai(counter, PaiKinds.P3);
                    counter = IkasamaTehai(counter, PaiKinds.P3);
                    counter = IkasamaTehai(counter, PaiKinds.P4);
                    counter = IkasamaTehai(counter, PaiKinds.P5);
                    counter = IkasamaTehai(counter, PaiKinds.P6);
                    counter = IkasamaTehai(counter, PaiKinds.P7);
                    counter = IkasamaTehai(counter, PaiKinds.P8);
                    counter = IkasamaTehai(counter, PaiKinds.P8);
                    counter = IkasamaTehai(counter, PaiKinds.P8);
                }
                break;
            case 7: //国士十三面待ち
                {
                    counter = IkasamaTehai(counter, PaiKinds.M1);
                    counter = IkasamaTehai(counter, PaiKinds.M9);
                    counter = IkasamaTehai(counter, PaiKinds.P1);
                    counter = IkasamaTehai(counter, PaiKinds.P9);
                    counter = IkasamaTehai(counter, PaiKinds.S1);
                    counter = IkasamaTehai(counter, PaiKinds.S9);
                    counter = IkasamaTehai(counter, PaiKinds.J1);
                    counter = IkasamaTehai(counter, PaiKinds.J2);
                    counter = IkasamaTehai(counter, PaiKinds.J3);
                    counter = IkasamaTehai(counter, PaiKinds.J4);
                    counter = IkasamaTehai(counter, PaiKinds.J5);
                    counter = IkasamaTehai(counter, PaiKinds.J6);
                    counter = IkasamaTehai(counter, PaiKinds.J7);
                }
                break;
            case 8: //二盃口
                {
                    counter = IkasamaTehai(counter, PaiKinds.M1);
                    counter = IkasamaTehai(counter, PaiKinds.M1);
                    counter = IkasamaTehai(counter, PaiKinds.M2);
                    counter = IkasamaTehai(counter, PaiKinds.M2);
                    counter = IkasamaTehai(counter, PaiKinds.M3);
                    counter = IkasamaTehai(counter, PaiKinds.M3);
                    counter = IkasamaTehai(counter, PaiKinds.P1);
                    counter = IkasamaTehai(counter, PaiKinds.P2);
                    counter = IkasamaTehai(counter, PaiKinds.P2);
                    counter = IkasamaTehai(counter, PaiKinds.P3);
                    counter = IkasamaTehai(counter, PaiKinds.P3);
                    counter = IkasamaTehai(counter, PaiKinds.S9);
                    counter = IkasamaTehai(counter, PaiKinds.S9);
                }
                break;
            case 9: //鳴いて速そうな好配牌
                {
                    counter = IkasamaTehai(counter, PaiKinds.M1);
                    counter = IkasamaTehai(counter, PaiKinds.M7);
                    counter = IkasamaTehai(counter, PaiKinds.M8);
                    counter = IkasamaTehai(counter, PaiKinds.P2);
                    counter = IkasamaTehai(counter, PaiKinds.P3);
                    counter = IkasamaTehai(counter, PaiKinds.P4);
                    counter = IkasamaTehai(counter, PaiKinds.P5);
                    counter = IkasamaTehai(counter, PaiKinds.P6);
                    counter = IkasamaTehai(counter, PaiKinds.P9);
                    counter = IkasamaTehai(counter, PaiKinds.P9);
                    counter = IkasamaTehai(counter, PaiKinds.J6);
                    counter = IkasamaTehai(counter, PaiKinds.J6);
                    counter = IkasamaTehai(counter, PaiKinds.J7);
                }
                break;
            case 10: //面前でマッハな好配牌
                {
                    counter = IkasamaTehai(counter, PaiKinds.M7);
                    counter = IkasamaTehai(counter, PaiKinds.M8);
                    counter = IkasamaTehai(counter, PaiKinds.M9);
                    counter = IkasamaTehai(counter, PaiKinds.M9);
                    counter = IkasamaTehai(counter, PaiKinds.M9);
                    counter = IkasamaTehai(counter, PaiKinds.P2);
                    counter = IkasamaTehai(counter, PaiKinds.P3);
                    counter = IkasamaTehai(counter, PaiKinds.P4);
                    counter = IkasamaTehai(counter, PaiKinds.P5);
                    counter = IkasamaTehai(counter, PaiKinds.P6);
                    counter = IkasamaTehai(counter, PaiKinds.P6);
                    counter = IkasamaTehai(counter, PaiKinds.P6);
                }
                break;
            case 11: //リーチ時の暗槓のテストに使う
                {
                    counter = IkasamaTehai(counter, PaiKinds.M7);
                    counter = IkasamaTehai(counter, PaiKinds.M7);
                    counter = IkasamaTehai(counter, PaiKinds.M7);
                    counter = IkasamaTehai(counter, PaiKinds.M9);
                    counter = IkasamaTehai(counter, PaiKinds.M9);
                    counter = IkasamaTehai(counter, PaiKinds.M9);
                    counter = IkasamaTehai(counter, PaiKinds.P2);
                    counter = IkasamaTehai(counter, PaiKinds.P3);
                    counter = IkasamaTehai(counter, PaiKinds.P4);
                    counter = IkasamaTehai(counter, PaiKinds.P5);
                    counter = IkasamaTehai(counter, PaiKinds.P6);
                    counter = IkasamaTehai(counter, PaiKinds.P6);
                    counter = IkasamaTehai(counter, PaiKinds.P6);
                }
                break;
            case 12: //門前の高目緑一色の両面待ち
                {
                    counter = IkasamaTehai(counter, PaiKinds.S2);
                    counter = IkasamaTehai(counter, PaiKinds.S3);
                    counter = IkasamaTehai(counter, PaiKinds.S3);
                    counter = IkasamaTehai(counter, PaiKinds.S4);
                    counter = IkasamaTehai(counter, PaiKinds.S4);
                    counter = IkasamaTehai(counter, PaiKinds.S6);
                    counter = IkasamaTehai(counter, PaiKinds.S6);
                    counter = IkasamaTehai(counter, PaiKinds.S6);
                    counter = IkasamaTehai(counter, PaiKinds.S8);
                    counter = IkasamaTehai(counter, PaiKinds.S8);
                    counter = IkasamaTehai(counter, PaiKinds.S8);
                    counter = IkasamaTehai(counter, PaiKinds.J6);
                    counter = IkasamaTehai(counter, PaiKinds.J6);
                }
                break;
            case 13: //門前の字一色のシャボ待ち
                {
                    counter = IkasamaTehai(counter, PaiKinds.J1);
                    counter = IkasamaTehai(counter, PaiKinds.J1);
                    counter = IkasamaTehai(counter, PaiKinds.J1);
                    counter = IkasamaTehai(counter, PaiKinds.J2);
                    counter = IkasamaTehai(counter, PaiKinds.J2);
                    counter = IkasamaTehai(counter, PaiKinds.J2);
                    counter = IkasamaTehai(counter, PaiKinds.J3);
                    counter = IkasamaTehai(counter, PaiKinds.J3);
                    counter = IkasamaTehai(counter, PaiKinds.J3);
                    counter = IkasamaTehai(counter, PaiKinds.J5);
                    counter = IkasamaTehai(counter, PaiKinds.J5);
                    counter = IkasamaTehai(counter, PaiKinds.J6);
                    counter = IkasamaTehai(counter, PaiKinds.J6);
                }
                break;
            default:
                break;
        }
        return counter;
    }

    /// <summary>
    /// 混ぜた後に山などをイカサマで変えたければここ
    /// </summary>
    private void IkasamaAfterVersion()
    {
        int num = -1;
        switch (num)
        {
            case 0: // イカサマNo.11[リーチ時の暗槓のテストに使う] のとき
                {
                    IkasamaTehai(52, PaiKinds.J1);
                    IkasamaTehai(56, PaiKinds.J1);
                    IkasamaTehai(60, PaiKinds.M7);
                    IkasamaTehai(64, PaiKinds.P6);
                    IkasamaTehai(68, PaiKinds.M9);
                    IkasamaTehai(72, PaiKinds.P2);
                }
                break;
            case 1: // イカサマNo.12[門前の緑一色の両面待ち] のとき
                {
                    IkasamaTehai(52, PaiKinds.J1);
                    IkasamaTehai(56, PaiKinds.S5);
                    IkasamaTehai(60, PaiKinds.S2);
                }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 手牌を指定の牌と入れ替えるイカサマを実行する
    /// </summary>
    /// <param name="_tehaiIndex">手牌のインデックス(0から順に入れていく)</param>
    /// <param name="_paiKind">入れ替えたい牌種</param>
    /// <returns>インデックス+1の値</returns>
    private int IkasamaTehai(int _tehaiIndex, PaiKinds _paiKind)
    {
        bool findFlg = false;

        if (paiyama[_tehaiIndex].thisKind != _paiKind)
        {
            for (int i = _tehaiIndex + 1; i < paiyama.Count; i++)
            {
                if (paiyama[i].thisKind == _paiKind)
                {
                    var temp = paiyama[i];
                    paiyama[i] = paiyama[_tehaiIndex];
                    paiyama[_tehaiIndex] = temp;
                    findFlg = true;
                    break;
                }
            }
        }

        int target = 4;
        while (!findFlg) //普通に探して見つからなかったとき
        {
            int counter = 0;
            for (int i = 0; i < paiyama.Count; i++)
            {
                if (paiyama[i].thisKind == _paiKind)
                {
                    counter++;
                    if (counter == target)
                    {
                        var temp = paiyama[i];
                        paiyama[i] = paiyama[_tehaiIndex];
                        paiyama[_tehaiIndex] = temp;
                        findFlg = true;
                        Debug.Log($"IkasamaTehai : Find Difficult , _paiKind = {_paiKind} , target counter = {target}");
                        break;
                    }
                }
            }
            target--;
        }

        return _tehaiIndex + 1;
    }

    /// <summary>
    /// ゲーム進行を進める
    /// </summary>
    private void NextGameTurn()
    {
        bool suteFlg = false;
        if (nowGameTurn == GameTurn.Ton_Sute) suteFlg = true;
        else if (nowGameTurn == GameTurn.Nan_Sute) suteFlg = true;
        else if (nowGameTurn == GameTurn.Sha_Sute) suteFlg = true;
        else if (nowGameTurn == GameTurn.Pe_Sute) suteFlg = true;

        if (nowTsumoCount >= COUNT_FINISH_TSUMO && suteFlg)
        {
            KyokuFinishOfRyuukyoku(RyuukyokuOfTochuu.None);
        }
        else if (nowGameTurn == GameTurn.Pe_Sute)
        {
            nowGameTurn = GameTurn.Ton_Tsumo;
        }
        else
        {
            nowGameTurn++;
        }

        //Debug.Log($"NextGameTurn : nowGameTurn = {nowGameTurn}");

        ActionTurn(false, false, PlayerKind.Other);
    }

    /// <summary>
    /// ターンに応じてアクションを実行する
    /// </summary>
    /// <param name="_nakiFlg">鳴きから回ってきたときはTrue</param>
    /// <param name="_kanFlg">カンから回ってきたときはTrue</param>
    private void ActionTurn(bool _nakiFlg , bool _kanFlg , PlayerKind _kanPlayerKind) 
    {
        if (GetNowTurnTsumo())
        {
            if (_kanFlg)
            {
                int paiIndex = wanpai.GetAndAddRinshanIndex();
                playerTehais[(int)_kanPlayerKind - 1].Tsumo(paiyama[paiIndex], paiIndex);
            }
            else
            {
                int playerIndex = (int)nowGameTurn / 2;
                playerTehais[playerIndex].Tsumo(paiyama[nowPaiIndex], nowPaiIndex);
                nowPaiIndex++;
            }
            nowTsumoCount++;

            UiManagerChangeNokoriText();

            if (nowGameTurn == GameTurn.Ton_Tsumo)
            {
                if (_nakiFlg) //鳴きで回ってきたとき
                {
                    //なし
                }
                else
                {
                    bool flgTsumoAgari = playerTehais[0].GetAbleTsumoAgari();

                    if (playerTehais[0].GetReachTurn() == INDEX_NONE) // 立直していないとき
                    {
                        bool flgRyuukyoku = playerTehais[0].GetAbleRyuukyoku();
                        bool flgAnkan = playerTehais[0].GetAbleAnkan(false);
                        bool flgKakan = playerTehais[0].GetAbleKakan();

                        List<PaiKinds> reachList = new List<PaiKinds>();
                        List<PaiKinds> ankanList = new List<PaiKinds>();
                        List<PaiKinds> kakanList = new List<PaiKinds>();

                        reachList = playerTehais[0].GetAbleReach();

                        bool flgReach = reachList.Count > 0 ? true : false;

                        if (flgAnkan)
                        {
                            for (int i = 0; i < System.Enum.GetValues(typeof(PaiKinds)).Length; i++)
                            {
                                if (playerTehais[0].GetAbleAnkanForPaiKind((PaiKinds)i))
                                {
                                    ankanList.Add((PaiKinds)i);
                                }
                            }
                        }

                        if (flgKakan)
                        {
                            for (int i = 0; i < System.Enum.GetValues(typeof(PaiKinds)).Length; i++)
                            {
                                if (playerTehais[0].GetAbleKakanForPaiKind((PaiKinds)i))
                                {
                                    kakanList.Add((PaiKinds)i);
                                }
                            }
                        }

                        if (flgTsumoAgari || flgRyuukyoku || flgReach || flgAnkan || flgKakan)
                        {
                            uiManager.DisplayTurnAction(flgTsumoAgari, flgRyuukyoku, reachList, ankanList, kakanList);
                        }

                    }
                    else // 立直しているとき
                    {
                        bool flgAnkan = playerTehais[0].GetAbleAnkan(true);
                        List<PaiKinds> ankanList = new List<PaiKinds>();
                        if (flgAnkan)
                        {
                            ankanList.Add(playerTehais[0].GetPaiStatusTehaiOfRight().thisKind);
                        }

                        if (flgTsumoAgari || flgAnkan)
                        {
                            uiManager.DisplayTurnAction(flgTsumoAgari, false, new List<PaiKinds>(), ankanList, new List<PaiKinds>());
                        }
                        else
                        {
                            ReceptionPaiPrefab(playerTehais[0].GetPaiStatusTehaiOfRight().totalNumber);
                        }
                    }

                    if (flgTsumoAgari)
                    {
                        // 役満のときの条件
                        YakumanKind yakuman = CheckYakuman(playerTehais[0].GetTehais(), playerTehais[0].GetNakis(), PaiKinds.None_00);
                        if(yakuman != YakumanKind.None)
                        {
                            //Debug.Log($"ActionTurn : Yakuman Tsumo");
                            playerTehais[0].SetTsumoPaiFire(true);
                        }
                    }
                }
            }
        }
        else if (GetNowTurnSute() && nowGameTurn != GameTurn.Ton_Sute)
        {
            int playerIndex = (int)nowGameTurn / 2;

            var sutePai = playerKawas[playerIndex - 1].GetLastSutePai().myPaiStatus.thisKind;

            bool flgRon;
            bool flgFuriten;

            if (playerTehais[0].GetReachTurn() == INDEX_NONE) // 立直していないとき
            {
                bool flgChiL = playerTehais[0].GetAbleChiLow(sutePai, (PlayerKind)playerIndex);
                bool flgChiM = playerTehais[0].GetAbleChiMid(sutePai, (PlayerKind)playerIndex);
                bool flgChiH = playerTehais[0].GetAbleChiHigh(sutePai, (PlayerKind)playerIndex);
                bool flgPon = playerTehais[0].GetAblePon(sutePai);
                bool flgKan = playerTehais[0].GetAbleDaiminkan(sutePai);
                flgRon = playerTehais[0].GetAbleRon(sutePai);
                flgFuriten = flgRon ? playerKawas[0].CheckFuriten(playerTehais[0].GetTehaiInformation(), (PlayerKind)playerIndex) : false;

                if (flgChiL || flgChiM || flgChiH || flgPon || flgKan || flgRon)
                {
                    uiManager.DisplayNaki(flgChiL, flgChiM, flgChiH, flgPon, flgKan, flgRon, flgFuriten, sutePai);
                }
            }
            else // 立直しているとき
            {
                flgRon = playerTehais[0].GetAbleRon(sutePai);
                flgFuriten = playerTehais[0].GetMinogashiAfterReachFlg();

                if (!flgFuriten)
                {
                    flgFuriten = flgRon ? playerKawas[0].CheckFuriten(playerTehais[0].GetTehaiInformation(), (PlayerKind)playerIndex) : false;
                }

                if (flgRon)
                {
                    uiManager.DisplayNaki(false, false, false, false, false, flgRon, flgFuriten, sutePai);
                }
            }

            if (flgRon && !flgFuriten)
            {
                // 役満のときの条件
                YakumanKind yakuman = CheckYakuman(playerTehais[0].GetTehais(), playerTehais[0].GetNakis(), sutePai);
                if (yakuman != YakumanKind.None)
                {
                    //Debug.Log($"ActionTurn : Yakuman Ron");
                    playerKawas[playerIndex - 1].SetRonPaiAura(true);
                }
            }
        }
    }

    /// <summary>
    /// UiManagerのChangeKyokuTextを実行する
    /// </summary>
    private void UiManagerChangeKyokuText()
    {
        uiManager.ChangeKyokuText(nowKyoku, nowHonba);
    }

    /// <summary>
    /// UiManagerのChangeNokoriTextを実行する
    /// </summary>
    private void UiManagerChangeNokoriText()
    {
        uiManager.ChangeNokoriText(COUNT_FINISH_TSUMO - nowTsumoCount);
    }

    /// <summary>
    /// 物理的に牌を指定したTransformに作る
    /// </summary>
    /// <param name="_thisKind"></param>
    /// <param name="_totalNumber"></param>
    /// <param name="_position"></param>
    public void InstantiatePai(PaiKinds _thisKind, int _totalNumber, int _arrayNumber, Vector3 _position, Vector3 _rotation)
    {
        var pai = Instantiate(paiPrefab,paiParentTransform).GetComponent<PaiPrefab>();
        pai.SetStatus(_thisKind, _totalNumber, _arrayNumber, PlayerKind.Other);
        pai.ChangeTransformPosition(_position);
        pai.ChangeTransformRotate(_rotation);
    }

    /// <summary>
    /// 牌のオブジェクト(表示されているやつ)を削除(リセット)する
    /// </summary>
    private void ResetTransformObjects()
    {
        foreach (Transform item in paiParentTransform)
        {
            Destroy(item.gameObject);
        }
    }

    /// <summary>
    /// それぞれのプレイヤーの手牌情報をリセットする
    /// </summary>
    private void ResetPlayersTehaisAndKawas()
    {
        foreach (var item in playerTehais)
        {
            item.ResetTehai();
        }
        foreach (var item in playerKawas)
        {
            item.ResetKawa();
        }
        wanpai.ResetWanpai();
    }

    /// <summary>
    /// 配牌を各プレイヤーに配る
    /// </summary>
    private void DealHaiPai()
    {
        for(int j = 0; j < 4; j++)
        {
            for (int i = 0; i < DEFAULT_MAISUU_TEHAI; i++)
            {
                playerTehais[j].AddTehai(paiyama[nowPaiIndex], nowPaiIndex);
                nowPaiIndex++;
            }
        }
    }

    /// <summary>
    /// カンをした時のアクション
    /// </summary>
    /// <param name="_playerKind">カンしたプレイヤー</param>
    /// <param name="_nakiFlg">大明槓した場合True</param>
    private void ActionKan(PlayerKind _playerKind , bool _nakiFlg)
    {
        StartCoroutine(ActionKanCoroutine(_playerKind, _nakiFlg));
    }

    /// <summary>
    /// カンをした時のアクションのコルーチン
    /// </summary>
    /// <param name="_playerKind"></param>
    /// <param name="_nakiFlg"></param>
    /// <returns></returns>
    private IEnumerator ActionKanCoroutine(PlayerKind _playerKind, bool _nakiFlg)
    {
        yield return new WaitForSeconds(0.4f);
        ActionTurn(_nakiFlg, true, _playerKind);
    }

    /// <summary>
    /// 聴牌かどうかを調べ、その可否を返す
    /// </summary>
    /// <param name="_tehaiInformationList"></param>
    /// <returns></returns>
    public bool CheckTenpai(int[] _tehaiInformationList)
    {
        if (_tehaiInformationList.Length != System.Enum.GetValues(typeof(PaiKinds)).Length)
        {
            Debug.LogWarning($"CheckTenpai : Length Error , _tehaiInformationList.Length = {_tehaiInformationList.Length}");
            return false;
        }

        for (int i = 0; i < System.Enum.GetValues(typeof(PaiKinds)).Length; i++)
        {
            if (i % 10 != 0)
            {
                int[] copyTehaiInformation = new int[System.Enum.GetValues(typeof(PaiKinds)).Length];

                System.Array.Copy(_tehaiInformationList, copyTehaiInformation, _tehaiInformationList.Length);

                copyTehaiInformation[i]++;

                bool result = CheckAgariAll(copyTehaiInformation);

                if (result) return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 全ての和了形のアガリが可能かを調べ、その可否を返す
    /// </summary>
    /// <param name="_tehaiInformationList"></param>
    /// <returns></returns>
    public bool CheckAgariAll(int[] _tehaiInformationList)
    {
        if(_tehaiInformationList.Length != System.Enum.GetValues(typeof(PaiKinds)).Length)
        {
            Debug.LogWarning($"CheckAgari : Length Error , _tehaiInformationList.Length = {_tehaiInformationList.Length}");
            return false;
        }

        int counter = 0;
        List<int> toitsuList = new List<int>();
        List<int> kootsuList = new List<int>();
        for (int i = 0; i < _tehaiInformationList.Length; i++)
        {
            if (i % 10 == 0 && _tehaiInformationList[i] != 0)
            {
                Debug.LogWarning($"CheckAgari : Data Error , _tehaiInformationList[i] = {_tehaiInformationList[i]}");
                return false;
            }

            counter += _tehaiInformationList[i];
            if(_tehaiInformationList[i] >= 2)
            {
                toitsuList.Add(i);
                if (_tehaiInformationList[i] >= 3)
                {
                    kootsuList.Add(i);
                    if(_tehaiInformationList[i] >= 5)
                    {
                        return false;
                    }
                }
            }
        }

        if (counter % 3 != 2)
        {
            Debug.LogWarning($"CheckAgari : Count Error , counter = {counter}");
            return false;
        }

        int needMentsuNum = (counter - 2) / 3;

        bool flgKokusimusou = CheckAgariKokushimusou(_tehaiInformationList, counter);
        if (flgKokusimusou)
        {
            return true;
        }

        bool flg7Toitsu = CheckAgari7Toitsu(_tehaiInformationList, toitsuList);
        if (flg7Toitsu)
        {
            return true;
        }

        bool flgMentsuTe = CheckAgariMentsuTe(_tehaiInformationList, needMentsuNum, toitsuList, kootsuList);
        if (flgMentsuTe)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 面子手が和了れるか調べる
    /// </summary>
    /// <param name="_tehaiInformationList"></param>
    /// <param name="_needMentsuNum"></param>
    /// <param name="_toitsuList"></param>
    /// <param name="_kootsuList"></param>
    /// <returns></returns>
    private bool CheckAgariMentsuTe(int[] _tehaiInformationList , int _needMentsuNum, List<int> _toitsuList, List<int> _kootsuList)
    {
        // 鳴いてない手牌が対々和or四暗刻の形である場合を調べる(鳴き面子に順子が含まれる場合も該当する)
        if (_needMentsuNum == _kootsuList.Count && _needMentsuNum == (_toitsuList.Count - 1))
        {
            //Debug.Log($"CheckAgariMentsuTe : Agari For ToiToi");
            return true;
        }

        // 字牌が1or4枚のときは省く
        for (int i = 31; i < 7; i++)
        {
            if (_tehaiInformationList[i] == 1 || _tehaiInformationList[i] == 4)
            {
                return false;
            }
        }

        // 上記で該当しないので、ちゃんと調べる
        foreach(var itemToitsuIndex in _toitsuList) // 対子候補すべてで確認する
        {
            int[] copyTehaiInformationList = new int[_tehaiInformationList.Length];
            System.Array.Copy(_tehaiInformationList, copyTehaiInformationList, _tehaiInformationList.Length); //手牌リストをコピー

            List<int> copyKootsuList = new List<int>(_kootsuList); //刻子リストをコピー

            copyTehaiInformationList[itemToitsuIndex] -= 2; //コピーリストから対子を抜き去り、面子候補だけを残す
            foreach(var itemKootsuIndex in copyKootsuList)
            {
                if(itemKootsuIndex == itemToitsuIndex)
                {
                    copyKootsuList.Remove(itemKootsuIndex);
                    break;
                }
            }

            bool mentsuteFlg = CheckMentsu(copyTehaiInformationList, copyKootsuList);

            if (mentsuteFlg)
            {
                //Debug.Log($"CheckAgariMentsuTe : Agari For Not ToiToi");
                //Debug.Log($"CheckAgariMentsuTe : itemToitsuIndex = {(PaiKinds)itemToitsuIndex}");

                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 面子が揃っているかを調べて返す
    /// </summary>
    /// <param name="_tehaiInformationListCutToitsu">手牌のリスト(対子をカット)</param>
    /// <param name="_needMentsuNum">必要な面子数</param>
    /// <param name="_kootsuList">刻子のリスト</param>
    /// <returns></returns>
    private bool CheckMentsu(int[] _tehaiInformationListCutToitsu, List<int> _kootsuList)
    {
        bool resultFlg = false;

        int testKootsuCount = -1;

        if (_kootsuList.Count >= 4)
        {
            Debug.LogError($"CheckMentsu : Over Kootsu Count = {_kootsuList.Count}");
            return false;
        }
        else if (_kootsuList.Count == 3)
        {
            for (int i = 0; i < 8; i++)
            {
                int[] copyTehaiInformationList = new int[_tehaiInformationListCutToitsu.Length];
                System.Array.Copy(_tehaiInformationListCutToitsu, copyTehaiInformationList, _tehaiInformationListCutToitsu.Length); //手牌リストをコピー

                bool x = i % 2 == 0 ? true : false;
                bool y = (i / 2) % 2 == 0 ? true : false;
                bool z = i / 4 == 0 ? true : false;

                if (x) copyTehaiInformationList[_kootsuList[0]] -= 3;
                if (y) copyTehaiInformationList[_kootsuList[1]] -= 3;
                if (z) copyTehaiInformationList[_kootsuList[2]] -= 3;

                bool result = CheckJuntsu(copyTehaiInformationList);

                if (result)
                {
                    testKootsuCount = _kootsuList.Count;
                    resultFlg = true;
                    break;
                }
            }
        }
        else if (_kootsuList.Count == 2)
        {
            for (int i = 0; i < 4; i++)
            {
                int[] copyTehaiInformationList = new int[_tehaiInformationListCutToitsu.Length];
                System.Array.Copy(_tehaiInformationListCutToitsu, copyTehaiInformationList, _tehaiInformationListCutToitsu.Length); //手牌リストをコピー

                bool x = i % 2 == 0 ? true : false;
                bool y = i / 2 == 0 ? true : false;

                if (x) copyTehaiInformationList[_kootsuList[0]] -= 3;
                if (y) copyTehaiInformationList[_kootsuList[1]] -= 3;

                bool result = CheckJuntsu(copyTehaiInformationList);

                if (result)
                {
                    testKootsuCount = _kootsuList.Count;
                    resultFlg = true;
                    break;
                }
            }
        }
        else if (_kootsuList.Count == 1)
        {
            for (int i = 0; i < 2; i++)
            {
                int[] copyTehaiInformationList = new int[_tehaiInformationListCutToitsu.Length];
                System.Array.Copy(_tehaiInformationListCutToitsu, copyTehaiInformationList, _tehaiInformationListCutToitsu.Length); //手牌リストをコピー

                bool x = i % 2 == 0 ? true : false;

                if (x) copyTehaiInformationList[_kootsuList[0]] -= 3;

                bool result = CheckJuntsu(copyTehaiInformationList);

                if (result)
                {
                    testKootsuCount = _kootsuList.Count;
                    resultFlg = true;
                    break;
                }
            }
        }
        else if (_kootsuList.Count == 0)
        {
            int[] copyTehaiInformationList = new int[_tehaiInformationListCutToitsu.Length];
            System.Array.Copy(_tehaiInformationListCutToitsu, copyTehaiInformationList, _tehaiInformationListCutToitsu.Length); //手牌リストをコピー

            bool result = CheckJuntsu(copyTehaiInformationList);

            if (result)
            {
                testKootsuCount = _kootsuList.Count;
                resultFlg = true;
            }
        }


        if (resultFlg)
        {
            string testStr = "";
            for (int i = 0; i < _tehaiInformationListCutToitsu.Length; i++)
            {
                if (i % 10 == 0)
                {
                    if (i == 0) testStr += "\nM : ";
                    else if (i == 10) testStr += "\nP : ";
                    else if (i == 20) testStr += "\nS : ";
                    else if (i == 30) testStr += "\nJ : ";
                }
                else
                {
                    testStr += $"{_tehaiInformationListCutToitsu[i]}, ";
                }
            }
            //Debug.LogWarning($"CheckMentsu : Clear!!\nkootsu count = {testKootsuCount}{testStr}");
        }

        return resultFlg;
    }

    /// <summary>
    /// 順子をチェックして、リスト内全てで完成したらTrueを返す
    /// </summary>
    /// <param name="_tehaiInformationListCutKootsuAndToitsu">手牌のリスト(対子・刻子をカット)</param>
    /// <returns></returns>
    private bool CheckJuntsu(int[] _tehaiInformationListCutKootsuAndToitsu)
    {
        int count = 0;
        foreach (var item in _tehaiInformationListCutKootsuAndToitsu) count += item;
        if (count % 3 != 0)
        {
            Debug.LogError($"CheckJuntsu : Error , _tehaiInformationListCutKootsuAndToitsu Count = {count}");
            return false;
        }

        //List<int> testPaiKindNum = new List<int>(); // Test

        int needJuntsu = count / 3;
        int juntsuCount = 0;

        int[] copyTehaiInformationList = new int[_tehaiInformationListCutKootsuAndToitsu.Length];
        System.Array.Copy(_tehaiInformationListCutKootsuAndToitsu, copyTehaiInformationList, _tehaiInformationListCutKootsuAndToitsu.Length); //手牌リストをコピー
        for (int i = 0; i < _tehaiInformationListCutKootsuAndToitsu.Length - 10; i++)
        {
            if (i % 10 == 0 || i % 10 >= 8) continue;
            while (copyTehaiInformationList[i] > 0)
            {
                if (copyTehaiInformationList[i + 1] > 0 && copyTehaiInformationList[i + 2] > 0)
                {
                    copyTehaiInformationList[i]--;
                    copyTehaiInformationList[i + 1]--;
                    copyTehaiInformationList[i + 2]--;
                    juntsuCount++;
                    //testPaiKindNum.Add(i); // Test
                }
                else
                {
                    return false;
                }
            }
        }

        if (needJuntsu == juntsuCount)
        {
            //string testStr = "";
            //foreach (var item in testPaiKindNum) testStr += $"{(PaiKinds)item}, ";
            //Debug.Log($"CheckJuntsu : Juntsu Count = {juntsuCount} , Need Juntsu = {needJuntsu}\nPaikind = {testStr}");

            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 国士無双が和了れるか調べる
    /// </summary>
    /// <param name="_tehaiInformationList"></param>
    /// <param name="_counter"></param>
    /// <returns></returns>
    private bool CheckAgariKokushimusou(int[] _tehaiInformationList , int _counter)
    {
        if (_counter != 14) return false;

        if (_tehaiInformationList[1] <= 0) return false;
        if (_tehaiInformationList[9] <= 0) return false;
        if (_tehaiInformationList[11] <= 0) return false;
        if (_tehaiInformationList[19] <= 0) return false;
        if (_tehaiInformationList[21] <= 0) return false;
        if (_tehaiInformationList[29] <= 0) return false;
        if (_tehaiInformationList[31] <= 0) return false;
        if (_tehaiInformationList[32] <= 0) return false;
        if (_tehaiInformationList[33] <= 0) return false;
        if (_tehaiInformationList[34] <= 0) return false;
        if (_tehaiInformationList[35] <= 0) return false;
        if (_tehaiInformationList[36] <= 0) return false;
        if (_tehaiInformationList[37] <= 0) return false;

        if (_tehaiInformationList[2] > 0) return false;
        if (_tehaiInformationList[3] > 0) return false;
        if (_tehaiInformationList[4] > 0) return false;
        if (_tehaiInformationList[5] > 0) return false;
        if (_tehaiInformationList[6] > 0) return false;
        if (_tehaiInformationList[7] > 0) return false;
        if (_tehaiInformationList[8] > 0) return false;
        if (_tehaiInformationList[12] > 0) return false;
        if (_tehaiInformationList[13] > 0) return false;
        if (_tehaiInformationList[14] > 0) return false;
        if (_tehaiInformationList[15] > 0) return false;
        if (_tehaiInformationList[16] > 0) return false;
        if (_tehaiInformationList[17] > 0) return false;
        if (_tehaiInformationList[18] > 0) return false;
        if (_tehaiInformationList[22] > 0) return false;
        if (_tehaiInformationList[23] > 0) return false;
        if (_tehaiInformationList[24] > 0) return false;
        if (_tehaiInformationList[25] > 0) return false;
        if (_tehaiInformationList[26] > 0) return false;
        if (_tehaiInformationList[27] > 0) return false;
        if (_tehaiInformationList[28] > 0) return false;

        //Debug.Log("Kokushi : True");
        return true;
    }

    /// <summary>
    /// 七対子が和了れるか調べる
    /// </summary>
    /// <param name="_tehaiInformationList"></param>
    /// <param name="_toitsuList"></param>
    /// <returns></returns>
    private bool CheckAgari7Toitsu(int[] _tehaiInformationList, List<int> _toitsuList)
    {
        if (_toitsuList.Count != 7) return false;

        return true;
    }

    /// <summary>
    /// 和了った手牌から役満をチェックする
    /// </summary>
    /// <param name="_tehaiList"></param>
    /// <param name="_nakiList"></param>
    /// <param name="_ronPai"></param>
    /// <returns></returns>
    private YakumanKind CheckYakuman(List<PaiStatus> _tehaiList, List<MentsuStatus> _nakiList , PaiKinds _ronPai)
    {
        YakumanKind result = YakumanKind.None;

        int[] _tehaiInformation = new int[System.Enum.GetValues(typeof(PaiKinds)).Length];

        foreach (var item in _tehaiList) //手牌のリストを作成
        {
            _tehaiInformation[(int)item.thisKind]++;
        }
        if (_ronPai != PaiKinds.None_00)
        {
            _tehaiInformation[(int)_ronPai]++;
        }

        if (CheckYakuTenho(_nakiList, _ronPai)) result = YakumanKind.TenHo;
        else if (CheckYakuSuAnKo(_tehaiInformation, _nakiList, _ronPai)) result = YakumanKind.SuAnKo;
        else if (CheckYakuSuKanTsu(_nakiList)) result = YakumanKind.SuKanTsu;
        else if (CheckYakuDaiSanGen(_tehaiInformation, _nakiList)) result = YakumanKind.DaiSanGen;
        else if (CheckYakuSyouSuuShi(_tehaiInformation, _nakiList)) result = YakumanKind.SyouSuuShi;
        else if (CheckYakuDaiSuuShi(_tehaiInformation, _nakiList)) result = YakumanKind.DaiSuuShi;
        else if (CheckYakuTsuIiSo(_tehaiInformation, _nakiList)) result = YakumanKind.TsuIiSo;
        else if (CheckYakuChinRouTo(_tehaiInformation, _nakiList)) result = YakumanKind.ChinRouTo;
        else if (CheckYakuRyuIiSo(_tehaiInformation, _nakiList)) result = YakumanKind.RyuIiSo;
        else if (CheckYakuKokuShiMuSou(_tehaiInformation, _nakiList)) result = YakumanKind.KokuShiMuSou;
        else if (CheckYakuChuRenPoTo(_tehaiInformation, _nakiList)) result = YakumanKind.ChuRenPoTo;

        return result;
    }

    /// <summary>
    /// 和了った役に天和が含まれるか確認する
    /// </summary>
    /// <param name="_nakiList"></param>
    /// <param name="_ronPai"></param>
    /// <returns></returns>
    private bool CheckYakuTenho(List<MentsuStatus> _nakiList, PaiKinds _ronPai)
    {
        //初手専用
        if (nowPaiIndex > 1) return false;
        //面前のとき専用
        if (_nakiList.Count != 0) return false;
        //ツモのとき専用
        if (_ronPai != PaiKinds.None_00) return false;

        return true;
    }

    /// <summary>
    /// 和了った役に四暗刻が含まれるか確認する
    /// </summary>
    /// <param name="_tehaiList"></param>
    /// <param name="_nakiList"></param>
    /// <param name="_ronPai"></param>
    /// <returns></returns>
    private bool CheckYakuSuAnKo(int[] _tehaiInformation, List<MentsuStatus> _nakiList, PaiKinds _ronPai)
    {
        if (_nakiList.Count != 0) return false; //面前のとき専用

        List<PaiKinds> ankoList = new List<PaiKinds>(); //暗刻のリストを作成
        foreach (var item in _tehaiInformation)
        {
            if (item == 3)
            {
                ankoList.Add((PaiKinds)item);
            }
        }

        if (ankoList.Count != 4) return false; //暗刻(出和了りの明刻も含む)が4個出ないときをはじく

        if (_ronPai == PaiKinds.None_00) //ツモのとき
        {
            return true;
        }
        else //ロンのとき
        {
            foreach (var item in ankoList) //ロン牌が明刻でなかったか確認する
            {
                if (item == _ronPai)
                {
                    return false;
                }
            }
            return true;
        }
    }

    /// <summary>
    /// 和了った役に四槓子が含まれるか確認する
    /// </summary>
    /// <param name="_nakiList"></param>
    /// <returns></returns>
    private bool CheckYakuSuKanTsu(List<MentsuStatus> _nakiList)
    {
        if (_nakiList.Count != 4) return false; //裸単騎のとき専用

        foreach (var item in _nakiList)
        {
            if (item.mentsuKind != MentsuKinds.Kantsu)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 和了った役に大三元が含まれるか確認する
    /// </summary>
    /// <returns></returns>
    private bool CheckYakuDaiSanGen(int[] _tehaiInformation, List<MentsuStatus> _nakiList)
    {
        bool haku = false;
        bool hatsu = false;
        bool chun = false;

        if (_tehaiInformation[(int)PaiKinds.J5] == 3) haku = true;
        if (_tehaiInformation[(int)PaiKinds.J6] == 3) hatsu = true;
        if (_tehaiInformation[(int)PaiKinds.J7] == 3) chun = true;

        foreach(var item in _nakiList)
        {
            if (item.minimumPai == PaiKinds.J5) haku = true;
            if (item.minimumPai == PaiKinds.J6) hatsu = true;
            if (item.minimumPai == PaiKinds.J7) chun = true;
        }

        if (haku && hatsu && chun)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 和了った役に小四喜が含まれるか確認する
    /// </summary>
    /// <param name="_tehaiInformation"></param>
    /// <param name="_nakiList"></param>
    /// <returns></returns>
    private bool CheckYakuSyouSuuShi(int[] _tehaiInformation, List<MentsuStatus> _nakiList)
    {
        bool ton = false;
        bool nan = false;
        bool sha = false;
        bool pe = false;

        if (_tehaiInformation[(int)PaiKinds.J1] == 3) ton = true;
        if (_tehaiInformation[(int)PaiKinds.J2] == 3) nan = true;
        if (_tehaiInformation[(int)PaiKinds.J3] == 3) sha = true;
        if (_tehaiInformation[(int)PaiKinds.J4] == 3) pe = true;

        foreach (var item in _nakiList)
        {
            if (item.minimumPai == PaiKinds.J1) ton = true;
            if (item.minimumPai == PaiKinds.J2) nan = true;
            if (item.minimumPai == PaiKinds.J3) sha = true;
            if (item.minimumPai == PaiKinds.J4) pe = true;
        }

        int counter = 0;
        if (ton) counter++;
        if (nan) counter++;
        if (sha) counter++;
        if (pe) counter++;

        if (counter == 3)
        {
            if (!ton) { if (_tehaiInformation[(int)PaiKinds.J1] == 2) return true; }
            else if (!nan) { if (_tehaiInformation[(int)PaiKinds.J2] == 2) return true; }
            else if (!sha) { if (_tehaiInformation[(int)PaiKinds.J3] == 2) return true; }
            else if (!pe) { if (_tehaiInformation[(int)PaiKinds.J4] == 2) return true; }

            return false;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 和了った役に大四喜が含まれるか確認する
    /// </summary>
    /// <param name="_tehaiInformation"></param>
    /// <param name="_nakiList"></param>
    /// <returns></returns>
    private bool CheckYakuDaiSuuShi(int[] _tehaiInformation, List<MentsuStatus> _nakiList)
    {
        bool ton = false;
        bool nan = false;
        bool sha = false;
        bool pe = false;

        if (_tehaiInformation[(int)PaiKinds.J1] == 3) ton = true;
        if (_tehaiInformation[(int)PaiKinds.J2] == 3) nan = true;
        if (_tehaiInformation[(int)PaiKinds.J3] == 3) sha = true;
        if (_tehaiInformation[(int)PaiKinds.J4] == 3) pe = true;

        foreach (var item in _nakiList)
        {
            if (item.minimumPai == PaiKinds.J1) ton = true;
            if (item.minimumPai == PaiKinds.J2) nan = true;
            if (item.minimumPai == PaiKinds.J3) sha = true;
            if (item.minimumPai == PaiKinds.J4) pe = true;
        }

        if (ton && nan && sha && pe)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 和了った役に字一色が含まれるか確認する
    /// </summary>
    /// <returns></returns>
    private bool CheckYakuTsuIiSo(int[] _tehaiInformation, List<MentsuStatus> _nakiList)
    {
        foreach (var item in _nakiList)
        {
            if (item.iroKinds != IroKinds.Jihai) return false;
        }

        for (int i = 0; i < _tehaiInformation.Length; i++)
        {
            if (_tehaiInformation[i] > 0)
            {
                if (i < (int)PaiKinds.J1) return false;
            }

        }

        return true;
    }

    /// <summary>
    /// 和了った役に清老頭が含まれるか確認する
    /// </summary>
    /// <param name="_tehaiInformation"></param>
    /// <param name="_nakiList"></param>
    /// <returns></returns>
    private bool CheckYakuChinRouTo(int[] _tehaiInformation, List<MentsuStatus> _nakiList)
    {
        foreach (var item in _nakiList)
        {
            if(item.mentsuKind == MentsuKinds.Juntsu)
            {
                return false;
            }
            else if(item.iroKinds == IroKinds.Jihai)
            {
                return false;
            }
            else
            {
                int num = (int)item.mentsuKind % 10;
                if (1 != num && 9 != num)
                {
                    return false;
                }
            }
        }

        for (int i = 0; i < _tehaiInformation.Length; i++)
        {
            if (_tehaiInformation[i] > 0)
            {
                int num = _tehaiInformation[i] % 10;
                if (1 != num && 9 != num)
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// 和了った役に緑一色が含まれるか確認する
    /// </summary>
    /// <param name="_tehaiInformation"></param>
    /// <param name="_nakiList"></param>
    /// <returns></returns>
    private bool CheckYakuRyuIiSo(int[] _tehaiInformation, List<MentsuStatus> _nakiList)
    {
        List<PaiKinds> greenList = new List<PaiKinds>();
        greenList.Add(PaiKinds.S2);
        greenList.Add(PaiKinds.S3);
        greenList.Add(PaiKinds.S4);
        greenList.Add(PaiKinds.S6);
        greenList.Add(PaiKinds.S8);
        greenList.Add(PaiKinds.J6);

        foreach (var item in _nakiList)
        {
            if (!item.midori) return false;
        }

        for (int i = 0; i < _tehaiInformation.Length; i++)
        {
            if (_tehaiInformation[i] > 0)
            {
                bool okFlg = false;
                foreach (var item in greenList)
                {
                    if (item == (PaiKinds)i)
                    {
                        okFlg = true;
                        break;
                    }
                }
                if (!okFlg) return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 和了った役に国士無双が含まれるか確認する
    /// </summary>
    /// <param name="_tehaiInformation"></param>
    /// <param name="_nakiList"></param>
    /// <returns></returns>
    private bool CheckYakuKokuShiMuSou(int[] _tehaiInformation, List<MentsuStatus> _nakiList)
    {
        if (_nakiList.Count != 0) return false; //面前のとき専用

        return CheckAgariKokushimusou(_tehaiInformation, 14);
    }

    /// <summary>
    /// 和了った役に九連宝灯が含まれるか確認する
    /// </summary>
    /// <param name="_tehaiInformation"></param>
    /// <param name="_nakiList"></param>
    /// <returns></returns>
    private bool CheckYakuChuRenPoTo(int[] _tehaiInformation, List<MentsuStatus> _nakiList)
    {
        if (_nakiList.Count != 0) return false; //面前のとき専用

        IroKinds iro = CheckYakuChinIiSo(_tehaiInformation, _nakiList); //清一色のとき限定
        if (iro == IroKinds.None)
        {
            return false;
        }
        else
        {
            for (int i = 1; i < 10; i++)
            {
                int index = (int)(iro - 1) * 10 + i;
                if (i == 1 || i == 9)
                {
                    if (_tehaiInformation[index] < 3) return false;
                }
                else
                {
                    if (_tehaiInformation[index] < 1) return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// 和了った役に清一色が含まれるか確認し、その色を返す
    /// </summary>
    /// <param name="_tehaiInformation"></param>
    /// <param name="_nakiList"></param>
    /// <returns></returns>
    private IroKinds CheckYakuChinIiSo(int[] _tehaiInformation, List<MentsuStatus> _nakiList)
    {
        IroKinds kouho = IroKinds.None;

        for (int i = 0; i < _tehaiInformation.Length; i++)
        {
            if(kouho == IroKinds.None)
            {
                kouho = (IroKinds)((i + 10) / 10);
            }
            else
            {
                if (kouho != (IroKinds)((i + 10) / 10))
                {
                    return IroKinds.None;
                }
            }
        }

        foreach (var item in _nakiList)
        {
            if (item.iroKinds != kouho) return IroKinds.None;
        }

        if(kouho == IroKinds.Jihai || kouho == IroKinds.None)
        {
            return IroKinds.None;
        }
        else
        {
            return kouho;
        }
    }

    /// <summary>
    /// 局が和了で終わったとき
    /// </summary>
    /// <param name="_ronPai">ツモったときはnull</param>
    private void KyokuFinishOfAgari(PaiStatus _ronPai)
    {
        nowGameTurn = GameTurn.Finish_Agari;

        if (_ronPai == null) //ツモ和了
        {
            uiManager.ReceptionMahjongManagerForAgari(PlayerKind.Player, null);
        }
        else ////ロン和了
        {
            uiManager.ReceptionMahjongManagerForAgari(PlayerKind.Player, _ronPai);
        }
    }

    /// <summary>
    /// 局が流局で終わったとき
    /// </summary>
    /// <param name="_kind">通常の流局はNone</param>
    private void KyokuFinishOfRyuukyoku(RyuukyokuOfTochuu _kind)
    {
        nowGameTurn = GameTurn.Finish_Ryuukyoku;
        uiManager.ReceptionMahjongManagerForRyuukyoku(_kind);
    }

#region 受信

    /// <summary>
    /// PlayerTehaiから牌を河に切り終えたことを受信したとき
    /// </summary>
    public void ReceptionPlayerTehaiForTurnEnd()
    {
        NextGameTurn();
    }

    /// <summary>
    /// PlayerTehaiから鳴きの処理を終えたことを受信したとき
    /// </summary>
    public void ReceptionPlayerTehaiForNaki(PlayerKind _playerKind, bool _kanFlg)
    {
        switch (_playerKind)
        {
            case PlayerKind.Player:
                nowGameTurn = GameTurn.Ton_Tsumo;
                break;
            case PlayerKind.Shimocha:
                nowGameTurn = GameTurn.Nan_Tsumo;
                break;
            case PlayerKind.Toimen:
                nowGameTurn = GameTurn.Sha_Tsumo;
                break;
            case PlayerKind.Kamicha:
                nowGameTurn = GameTurn.Pe_Tsumo;
                break;
        }

        if (_kanFlg)
        {
            ActionKan(_playerKind, true);
        }
    }

    /// <summary>
    /// 自身のターンにカンをして、処理を終えたことを受信したとき
    /// </summary>
    /// <param name="_playerKind"></param>
    public void ReceptionPlayerTehaiForKanOfMyTurn(PlayerKind _playerKind)
    {
        ActionKan(_playerKind, false);
    }

    /// <summary>
    /// 牌のオブジェクトからタップを受信したとき
    /// </summary>
    /// <param name="_totalNumber"></param>
    public void ReceptionPaiPrefab(int _totalNumber)
    {
        if (nowGameTurn != GameTurn.Ton_Tsumo) return;

        bool errorFlg = false;
        int index = playerTehais[0].CheckHavePai(_totalNumber);
        if (turnActionFlg == TurnActionKind.None) //普通に牌を捨てるとき
        {
            if (index != INDEX_ERROR || !playerTehais[0].CheckAbleSute())
            {
                playerTehais[0].SuteMandatory(index, false);
                uiManager.ReceptionPlayerSute();
            }
            else
            {
                errorFlg = true;
            }
        }
        else //リーチやカンする牌を選ぶとき
        {
            if (turnActionFlg == TurnActionKind.Reach)
            {
                if (index != INDEX_ERROR || !playerTehais[0].CheckAbleSute())
                {
                    playerTehais[0].SuteMandatory(index, true);
                    uiManager.ReceptionPlayerSute();
                    ReceptionUiTurnActionForCancel();
                }
                else
                {
                    errorFlg = true;
                }
            }
            else if(turnActionFlg == TurnActionKind.Ankan || turnActionFlg == TurnActionKind.Kakan)
            {
                if (index != INDEX_ERROR || !playerTehais[0].CheckAbleSute())
                {
                    var pai = GetPaiStatusFromTotalNumber(_totalNumber).thisKind;

                    if (playerTehais[0].GetHavePaiKindCount(pai) == 4)
                    {
                        playerTehais[0].Ankan(pai);
                    }
                    else
                    {
                        playerTehais[0].Kakan(pai);
                    }
                }
                else
                {
                    errorFlg = true;
                }
            }
        }

        if (errorFlg)
        {
            Debug.Log($"ReceptionPaiPrefab : Not Tehai Pai , _totalNumber = {_totalNumber}");
        }
    }

    /// <summary>
    /// UiNakiPrefabからnakiWaitFlgの変更を受け取ったとき
    /// </summary>
    /// <param name="flg"></param>
    public void ReceptionUiNakiPrefabForChangeNakiWaitFlg(bool flg)
    {
        nakiWaitFlg = flg;

        if (playerTehais[0].GetReachTurn() != INDEX_NONE) //立直していたとき
        {
            playerTehais[0].SetMinogashiAfterReachFlg();
        }

        foreach(var item in playerKawas) //演出をキャンセルする
        {
            item.SetRonPaiAura(false);
        }
    }

    /// <summary>
    /// UiTurnActionPrefabからCancelを受け取ったとき
    /// </summary>
    public void ReceptionUiTurnActionForCancel()
    {
        turnActionFlg = TurnActionKind.None;
        playerTehais[0].ChangeTehaiChangeInteractableTap(null);
    }

    /// <summary>
    /// UiTurnActionPrefabからアクションを受信したとき
    /// </summary>
    /// <param name="_nakiKind"></param>
    public void ReceptionUiTurnActionPrefab(TurnActionKind _actionKind , PaiKinds _paiKind)
    {
        if (_actionKind == TurnActionKind.Tsumo)
        {
            KyokuFinishOfAgari(null);
        }
        else if (_actionKind == TurnActionKind.Ryuukyoku)
        {
            KyokuFinishOfRyuukyoku(RyuukyokuOfTochuu.Kyuusyu);
        }
        else if (_actionKind == TurnActionKind.Reach)
        {
            turnActionFlg = TurnActionKind.Reach;

            playerTehais[0].ChangeTehaiChangeInteractableTap(uiManager.GetActiveUiTurnActionPrefab().GetReachAbleList());            
        }
        else if (_actionKind == TurnActionKind.Ankan)
        {
            if(_paiKind == PaiKinds.None_00)
            {
                // 加槓と同じなのでそちらに任せる
                ReceptionUiTurnActionPrefab(TurnActionKind.Kakan, _paiKind);
            }
            else
            {
                playerTehais[0].Ankan(_paiKind);
            }
        }
        else if (_actionKind == TurnActionKind.Kakan)
        {
            if (_paiKind == PaiKinds.None_00) // 候補が複数あるとき
            {
                turnActionFlg = TurnActionKind.Kakan;

                playerTehais[0].ChangeTehaiChangeInteractableTap(uiManager.GetActiveUiTurnActionPrefab().GetKanAbleList());
            }
            else // 候補が1択のとき
            {
                playerTehais[0].Kakan(_paiKind);
            }
        }
    }

    /// <summary>
    /// UiNakiPrefabから鳴きを受信したとき
    /// </summary>
    /// <param name="_nakiKind"></param>
    public void ReceptionUiNakiPrefabForNaki(NakiKinds _nakiKind)
    {
        int playerIndex = (int)nowGameTurn / 2;
        var sutePai = playerKawas[playerIndex - 1].GetLastSutePai();

        if (_nakiKind == NakiKinds.Ron)
        {
            KyokuFinishOfAgari(sutePai.myPaiStatus);
        }
        else
        {
            playerKawas[playerIndex - 1].RemoveKawaLast();

            if (_nakiKind == NakiKinds.Pon)
            {
                playerTehais[0].Pon((NakiPrefab.NakiPlace)(playerIndex - 1), sutePai.myPaiStatus, sutePai.masterArrayNumber);
            }
            else if (_nakiKind == NakiKinds.ChiNumSmall)
            {
                playerTehais[0].Chi(NakiPrefab.PaiOfChi.Low, sutePai.myPaiStatus, sutePai.masterArrayNumber);
            }
            else if (_nakiKind == NakiKinds.ChiNumMiddle)
            {
                playerTehais[0].Chi(NakiPrefab.PaiOfChi.Mid, sutePai.myPaiStatus, sutePai.masterArrayNumber);
            }
            else if (_nakiKind == NakiKinds.ChiNumBig)
            {
                playerTehais[0].Chi(NakiPrefab.PaiOfChi.High, sutePai.myPaiStatus, sutePai.masterArrayNumber);
            }
            else if (_nakiKind == NakiKinds.MinKan)
            {
                playerTehais[0].Daiminkan((NakiPrefab.NakiPlace)(playerIndex - 1), sutePai.myPaiStatus, sutePai.masterArrayNumber);
            }

            ReceptionUiNakiPrefabForChangeNakiWaitFlg(false);
        }
    }


#endregion

#region ゲッター

    /// <summary>
    /// 麻雀牌の柄のマテリアルを返すゲッター
    /// </summary>
    /// <param name="paiKind"></param>
    /// <returns></returns>
    public Material GetGaraMaterial(PaiKinds paiKind)
    {
        bool errorFlg = false;
        if (paiKind == PaiKinds.None_00) errorFlg = true;
        else if (paiKind == PaiKinds.None_10) errorFlg = true;
        else if (paiKind == PaiKinds.None_20) errorFlg = true;
        else if (paiKind == PaiKinds.None_30) errorFlg = true;

        if (errorFlg)
        {
            Debug.LogError($"MahjongManager.GetGaraMaterial : Error , Pai Kind = {paiKind}");
            return garaMaterials[0];
        }
        else
        {
            return garaMaterials[(int)paiKind];
        }
    }

    /// <summary>
    /// 麻雀牌の柄のスプライトを返すゲッター
    /// </summary>
    /// <param name="paiKind"></param>
    /// <returns></returns>
    public Sprite GetGaraSprite(PaiKinds paiKind)
    {
        bool errorFlg = false;
        if (paiKind == PaiKinds.None_00) errorFlg = true;
        else if (paiKind == PaiKinds.None_10) errorFlg = true;
        else if (paiKind == PaiKinds.None_20) errorFlg = true;
        else if (paiKind == PaiKinds.None_30) errorFlg = true;

        if (errorFlg)
        {
            Debug.LogError($"MahjongManager.GetGaraSprite : Error , Pai Kind = {paiKind}");
            return garaSprites[0];
        }
        else
        {
            return garaSprites[(int)paiKind];
        }
    }

    /// <summary>
    /// 手牌の位置を返すゲッター
    /// </summary>
    /// <param name="_playerKind"></param>
    /// <param name="_positionNumber"></param>
    /// <returns></returns>
    public Vector3 GetPositionTehai(PlayerKind _playerKind,int _positionNumber,bool _tsumoFlg)
    {
        Vector3 result;
        if (_positionNumber < 0 || 13 < _positionNumber)
        {
            Debug.LogWarning($"GetPositionTehai : Error , _positionNumber = {_positionNumber}");
            result = Vector3.zero;
        }
        else if (_playerKind == PlayerKind.Player)
        {
            result = DEFAULT_POSITION_TEHAI_PLAYER + new Vector3(2 * _positionNumber, 0, 0);
            if (_tsumoFlg) result.x += 1;
        }
        else if (_playerKind == PlayerKind.Shimocha)
        {
            result = DEFAULT_POSITION_TEHAI_SHIMOCHA + new Vector3(0, 0, 2 * _positionNumber);
            if (_tsumoFlg) result.z += 1;
        }
        else if (_playerKind == PlayerKind.Toimen)
        {
            result = DEFAULT_POSITION_TEHAI_TOIMEN + new Vector3(-2 * _positionNumber, 0, 0);
            if (_tsumoFlg) result.x -= 1;
        }
        else if (_playerKind == PlayerKind.Kamicha)
        {
            result = DEFAULT_POSITION_TEHAI_KAMICHA + new Vector3(0, 0, -2 * _positionNumber);
            if (_tsumoFlg) result.z -= 1;
        }
        else
        {
            Debug.LogWarning($"GetPositionTehai : Error , _playerKind = {_playerKind}");
            result = Vector3.zero;
        }

        return result;
    }

    /// <summary>
    /// 鳴きメンツの位置を返すゲッター
    /// </summary>
    /// <returns></returns>
    public Vector3 GetPositionNaki(PlayerKind _playerKind, int _nakiCount)
    {
        Vector3 result;
        if (_nakiCount < 0 || 3 < _nakiCount)
        {
            Debug.LogWarning($"GetPositionNaki : Error , _nakiCount = {_nakiCount}");
            result = Vector3.zero;
        }
        else if (_playerKind == PlayerKind.Player)
        {
            int i = _nakiCount / 2;
            int j = _nakiCount % 2;

            result = DEFAULT_POSITION_NAKI_PLAYER + new Vector3(-9.0f * i, 0, 4.5f * j);
        }
        else if (_playerKind == PlayerKind.Shimocha)
        {
            result = DEFAULT_POSITION_NAKI_SHIMOCHA + new Vector3(0, 0, -6.5f * _nakiCount);
        }
        else if (_playerKind == PlayerKind.Toimen)
        {
            result = DEFAULT_POSITION_NAKI_TOIMEN + new Vector3(6.5f * _nakiCount, 0, 0);
        }
        else if (_playerKind == PlayerKind.Kamicha)
        {
            result = DEFAULT_POSITION_NAKI_KAMICHA + new Vector3(0, 0, 6.5f * _nakiCount);
        }
        else
        {
            Debug.LogWarning($"GetPositionNaki : Error , _playerKind = {_playerKind}");
            result = Vector3.zero;
        }

        return result;
    }

    /// <summary>
    /// 王牌の位置を返すゲッター
    /// </summary>
    /// <param name="_count"></param>
    /// <returns></returns>
    public Vector3 GetPositionWanpai(int _count)
    {
        int i = _count / 2;
        int j = _count % 2;

        return DEFAULT_POSITION_DORA + new Vector3(2f * i, -1.5f * j, 0f);
    }

    /// <summary>
    /// 河の位置を返すゲッター
    /// </summary>
    /// <param name="_playerKind"></param>
    /// <param name="_positionNumber"></param>
    /// <returns></returns>
    public Vector3 GetPositionKawa(PlayerKind _playerKind, int _positionNumber ,int _reachTurn)
    {
        Vector3 result;

        const int NUM_KUGIRI = 6;

        int _gyou = _positionNumber % NUM_KUGIRI;
        int _retsu = _positionNumber / NUM_KUGIRI;

        int _reachCoefficient;
        if (_reachTurn == playerKawas[(int)(_playerKind - 1)].GetKawaLength())
        {
            _reachCoefficient = 1;
        }
        else if ((_reachTurn - 1) / 6 == (playerKawas[(int)(_playerKind - 1)].GetKawaLength() - 1) / 6)
        {
            _reachCoefficient = 2;
        }
        else
        {
            _reachCoefficient = 0;
        }


        if (_positionNumber < 0 || 23 < _positionNumber)
        {
            Debug.LogWarning($"GetPositionKawa : Error , _positionNumber = {_positionNumber}");
            result = Vector3.zero;
        }

#if Yokogamen

        else if (_playerKind == PlayerKind.Player)
        {
            while (_retsu > 2)
            {
                _retsu--;
                _gyou += NUM_KUGIRI;
            }
            result = DEFAULT_POSITION_KAWA_PLAYER + new Vector3(2 * _gyou, 0f, - 2.75f * _retsu);
        }
        else if (_playerKind == PlayerKind.Shimocha)
        {
            result = DEFAULT_POSITION_KAWA_SHIMOCHA + new Vector3(2.75f * _retsu, 0f, 2 * _gyou);
        }
        else if (_playerKind == PlayerKind.Toimen)
        {
            while (_retsu > 2)
            {
                _retsu--;
                _gyou += NUM_KUGIRI;
            }
            result = DEFAULT_POSITION_KAWA_TOIMEN + new Vector3(- 2 * _gyou, 0f, 2.75f * _retsu);
        }
        else if (_playerKind == PlayerKind.Kamicha)
        {
            result = DEFAULT_POSITION_KAWA_KAMICHA + new Vector3(- 2.75f * _retsu, 0.9f, - 2 * _gyou);
        }

#else

        else if (_playerKind == PlayerKind.Player)
        {
            result = DEFAULT_POSITION_KAWA_PLAYER + new Vector3(2 * _gyou + (_reachCoefficient * 0.3f), 0f, - 2.75f * _retsu);
        }
        else if (_playerKind == PlayerKind.Shimocha)
        {
            while (_retsu > 2)
            {
                _retsu--;
                _gyou += NUM_KUGIRI;
            }
            result = DEFAULT_POSITION_KAWA_SHIMOCHA + new Vector3(2.75f * _retsu, 0f, 2 * _gyou + (_reachCoefficient * 0.3f));
        }
        else if (_playerKind == PlayerKind.Toimen)
        {
            result = DEFAULT_POSITION_KAWA_TOIMEN + new Vector3(- 2 * _gyou - (_reachCoefficient * 0.3f), 0f, 2.75f * _retsu);
        }
        else if (_playerKind == PlayerKind.Kamicha)
        {
            while (_retsu > 2)
            {
                _retsu--;
                _gyou += NUM_KUGIRI;
            }
            result = DEFAULT_POSITION_KAWA_KAMICHA + new Vector3(- 2.75f * _retsu, 0.9f, - 2 * _gyou - (_reachCoefficient * 0.3f));
        }

#endif

        else
        {
            Debug.LogWarning($"GetPositionKawa : Error , _playerKind = {_playerKind}");
            result = Vector3.zero;
        }

        return result;
    }

    /// <summary>
    /// 牌の回転角を返すゲッター
    /// </summary>
    /// <param name="_playerKind"></param>
    /// <param name="_tehaiFlg">手牌ならTrue</param>
    /// <returns></returns>
    public Vector3 GetRotation(PlayerKind _playerKind, bool _tehaiFlg, bool _reachFlg)
    {
        Vector3 result;

        if (_tehaiFlg)
        {
            if(_playerKind == PlayerKind.Player)
            {
                result = DEFAULT_ROTATE_PLAYER;
            }
            else if (_playerKind == PlayerKind.Shimocha)
            {
                result = DEFAULT_ROTATE_SHIMOCHA_TEHAI;
            }
            else if (_playerKind == PlayerKind.Toimen)
            {
                result = DEFAULT_ROTATE_TOIMEN_TEHAI;
            }
            else if (_playerKind == PlayerKind.Kamicha)
            {
                result = DEFAULT_ROTATE_KAMICHA_TEHAI;
            }
            else
            {
                Debug.LogWarning($"GetRotation : Error , _playerKind = {_playerKind}");
                result = Vector3.zero;
            }
        }
        else
        {
            if (_playerKind == PlayerKind.Player || _playerKind == PlayerKind.Other)
            {
                result = !_reachFlg ? DEFAULT_ROTATE_PLAYER : DEFAULT_ROTATE_SHIMOCHA_KAWA;
            }
            else if (_playerKind == PlayerKind.Shimocha)
            {
                result = !_reachFlg ? DEFAULT_ROTATE_SHIMOCHA_KAWA : DEFAULT_ROTATE_TOIMEN_KAWA;
            }
            else if (_playerKind == PlayerKind.Toimen)
            {
                result = !_reachFlg ? DEFAULT_ROTATE_TOIMEN_KAWA : DEFAULT_ROTATE_KAMICHA_KAWA;
            }
            else if (_playerKind == PlayerKind.Kamicha)
            {
                result = !_reachFlg ? DEFAULT_ROTATE_KAMICHA_KAWA : DEFAULT_ROTATE_PLAYER;
            }
            else
            {
                Debug.LogWarning($"GetRotation : Error , _playerKind = {_playerKind}");
                result = Vector3.zero;
            }
        }

        return result;
    }

    /// <summary>
    /// 裏向きの回転角を返すゲッター
    /// </summary>
    /// <returns></returns>
    public Vector3 GetRotationUra()
    {
        return DEFAULT_ROTATE_URA;
    }

    /// <summary>
    /// 指定したインデックスのPaiStatusを返すゲッター
    /// </summary>
    /// <param name="_index"></param>
    /// <returns></returns>
    public PaiStatus GetPaiyama(int _index)
    {
        return paiyama[_index];
    }

    /// <summary>
    /// 牌のプレファブを返すゲッター
    /// </summary>
    /// <returns></returns>
    public GameObject GetPaiPrefab()
    {
        return paiPrefab;
    }

    /// <summary>
    /// 鳴き形のプレファブを返すゲッター
    /// </summary>
    /// <param name="nakiPlace"></param>
    /// <returns></returns>
    public GameObject GetNakiPrefab(NakiPrefab.NakiPlace nakiPlace)
    {
        if (nakiPlace == NakiPrefab.NakiPlace.Left) return nakiPrefab[0];
        else if (nakiPlace == NakiPrefab.NakiPlace.Center) return nakiPrefab[1];
        else if (nakiPlace == NakiPrefab.NakiPlace.Right) return nakiPrefab[2];
        else return null;
    }

    /// <summary>
    /// 暗槓のプレファブを返すゲッター
    /// </summary>
    /// <returns></returns>
    public GameObject GetAnkanPrefab()
    {
        return ankanPrefab;
    }

    /// <summary>
    /// 牌オブジェクトを生成する親Transformを返すゲッター
    /// </summary>
    /// <returns></returns>
    public Transform GetpaiParentTransform()
    {
        return paiParentTransform;
    }

    /// <summary>
    /// 対応するPlayerTehaiコンポーネントを返すゲッター
    /// </summary>
    /// <param name="_playerKind"></param>
    /// <returns></returns>
    public PlayerTehai GetPlayerTehaiComponent(PlayerKind _playerKind)
    {
        PlayerTehai result;
        if (_playerKind == PlayerKind.Player)
        {
            result = playerTehais[0];
        }
        else if (_playerKind == PlayerKind.Shimocha)
        {
            result = playerTehais[1];
        }
        else if (_playerKind == PlayerKind.Toimen)
        {
            result = playerTehais[2];
        }
        else if (_playerKind == PlayerKind.Kamicha)
        {
            result = playerTehais[3];
        }
        else
        {
            Debug.LogWarning($"GetPlayerTehaiComponent : Error , _playerKind = {_playerKind}");
            result = null;
        }

        return result;
    }

    /// <summary>
    /// 対応するPlayerKawaコンポーネントを返すゲッター
    /// </summary>
    /// <param name="_playerKind"></param>
    /// <returns></returns>
    public PlayerKawa GetPlayerKawaComponent(PlayerKind _playerKind)
    {
        PlayerKawa result;
        if (_playerKind == PlayerKind.Player)
        {
            result = playerKawas[0];
        }
        else if (_playerKind == PlayerKind.Shimocha)
        {
            result = playerKawas[1];
        }
        else if (_playerKind == PlayerKind.Toimen)
        {
            result = playerKawas[2];
        }
        else if (_playerKind == PlayerKind.Kamicha)
        {
            result = playerKawas[3];
        }
        else
        {
            Debug.LogWarning($"GetPlayerKawaComponent : Error , _playerKind = {_playerKind}");
            result = null;
        }

        return result;
    }

    /// <summary>
    /// ツモ回数と最後の回数が同じかを返すゲッター
    /// </summary>
    /// <returns></returns>
    public bool GetFlgTsumoCountEqualFinishCount()
    {
        return nowTsumoCount == COUNT_FINISH_TSUMO;
    }

    /// <summary>
    /// 今が牌を捨てたタイミングかを返すゲッター
    /// </summary>
    /// <returns></returns>
    public bool GetNowTurnSute()
    {
        return nowGameTurn == GameTurn.Ton_Sute || nowGameTurn == GameTurn.Nan_Sute ||
            nowGameTurn == GameTurn.Sha_Sute || nowGameTurn == GameTurn.Pe_Sute;
    }

    /// <summary>
    /// 今が牌をツモって考えているタイミングかを返すゲッター
    /// </summary>
    /// <returns></returns>
    public bool GetNowTurnTsumo()
    {
        return nowGameTurn == GameTurn.Ton_Tsumo || nowGameTurn == GameTurn.Nan_Tsumo ||
            nowGameTurn == GameTurn.Sha_Tsumo || nowGameTurn == GameTurn.Pe_Tsumo;
    }

    /// <summary>
    /// トータルナンバーからPaiStatusを返すゲッター
    /// </summary>
    /// <param name="_totalNumber"></param>
    /// <returns></returns>
    public PaiStatus GetPaiStatusFromTotalNumber(int _totalNumber)
    {
        foreach(var item in paiyama)
        {
            if(item.totalNumber== _totalNumber)
            {
                return item;
            }
        }
        return null;
    }

    /// <summary>
    /// 完成面子の情報を返すゲッター
    /// </summary>
    /// <param name="_mentsuKind"></param>
    /// <param name="_lowPai"></param>
    /// <param name="nakiFlg"></param>
    /// <returns></returns>
    public MentsuStatus GetMentsuStatus(MentsuKinds _mentsuKind , PaiKinds _lowPai , bool nakiFlg)
    {
        MentsuStatus result = new MentsuStatus(PaiKinds.None_00, false, MentsuKinds.None, 0, NakiKinds.None, IroKinds.None, false);
        if (_mentsuKind == MentsuKinds.Juntsu)
        {
            if (_lowPai == PaiKinds.M1) result.DataCopy(MenzenM123);
            else if (_lowPai == PaiKinds.M2) result.DataCopy(MenzenM234);
            else if (_lowPai == PaiKinds.M3) result.DataCopy(MenzenM345);
            else if (_lowPai == PaiKinds.M4) result.DataCopy(MenzenM456);
            else if (_lowPai == PaiKinds.M5) result.DataCopy(MenzenM567);
            else if (_lowPai == PaiKinds.M6) result.DataCopy(MenzenM678);
            else if (_lowPai == PaiKinds.M7) result.DataCopy(MenzenM789);
            else if (_lowPai == PaiKinds.P1) result.DataCopy(MenzenP123);
            else if (_lowPai == PaiKinds.P2) result.DataCopy(MenzenP234);
            else if (_lowPai == PaiKinds.P3) result.DataCopy(MenzenP345);
            else if (_lowPai == PaiKinds.P4) result.DataCopy(MenzenP456);
            else if (_lowPai == PaiKinds.P5) result.DataCopy(MenzenP567);
            else if (_lowPai == PaiKinds.P6) result.DataCopy(MenzenP678);
            else if (_lowPai == PaiKinds.P7) result.DataCopy(MenzenP789);
            else if (_lowPai == PaiKinds.S1) result.DataCopy(MenzenS123);
            else if (_lowPai == PaiKinds.S2) result.DataCopy(MenzenS234);
            else if (_lowPai == PaiKinds.S3) result.DataCopy(MenzenS345);
            else if (_lowPai == PaiKinds.S4) result.DataCopy(MenzenS456);
            else if (_lowPai == PaiKinds.S5) result.DataCopy(MenzenS567);
            else if (_lowPai == PaiKinds.S6) result.DataCopy(MenzenS678);
            else if (_lowPai == PaiKinds.S7) result.DataCopy(MenzenS789);
            else Debug.LogWarning($"GetMentsuStatus : Error , Juntsu _lowPai = {_lowPai}");
        }
        else if (_mentsuKind == MentsuKinds.Kootsu || _mentsuKind == MentsuKinds.Kantsu)
        {
            if (_lowPai == PaiKinds.M1) result.DataCopy(MenzenM111);
            else if (_lowPai == PaiKinds.M2) result.DataCopy(MenzenM222);
            else if (_lowPai == PaiKinds.M3) result.DataCopy(MenzenM333);
            else if (_lowPai == PaiKinds.M4) result.DataCopy(MenzenM444);
            else if (_lowPai == PaiKinds.M5) result.DataCopy(MenzenM555);
            else if (_lowPai == PaiKinds.M6) result.DataCopy(MenzenM666);
            else if (_lowPai == PaiKinds.M7) result.DataCopy(MenzenM777);
            else if (_lowPai == PaiKinds.M8) result.DataCopy(MenzenM888);
            else if (_lowPai == PaiKinds.M9) result.DataCopy(MenzenM999);
            else if (_lowPai == PaiKinds.P1) result.DataCopy(MenzenP111);
            else if (_lowPai == PaiKinds.P2) result.DataCopy(MenzenP222);
            else if (_lowPai == PaiKinds.P3) result.DataCopy(MenzenP333);
            else if (_lowPai == PaiKinds.P4) result.DataCopy(MenzenP444);
            else if (_lowPai == PaiKinds.P5) result.DataCopy(MenzenP555);
            else if (_lowPai == PaiKinds.P6) result.DataCopy(MenzenP666);
            else if (_lowPai == PaiKinds.P7) result.DataCopy(MenzenP777);
            else if (_lowPai == PaiKinds.P8) result.DataCopy(MenzenP888);
            else if (_lowPai == PaiKinds.P9) result.DataCopy(MenzenP999);
            else if (_lowPai == PaiKinds.S1) result.DataCopy(MenzenS111);
            else if (_lowPai == PaiKinds.S2) result.DataCopy(MenzenS222);
            else if (_lowPai == PaiKinds.S3) result.DataCopy(MenzenS333);
            else if (_lowPai == PaiKinds.S4) result.DataCopy(MenzenS444);
            else if (_lowPai == PaiKinds.S5) result.DataCopy(MenzenS555);
            else if (_lowPai == PaiKinds.S6) result.DataCopy(MenzenS666);
            else if (_lowPai == PaiKinds.S7) result.DataCopy(MenzenS777);
            else if (_lowPai == PaiKinds.S8) result.DataCopy(MenzenS888);
            else if (_lowPai == PaiKinds.S9) result.DataCopy(MenzenS999);
            else if (_lowPai == PaiKinds.J1) result.DataCopy(MenzenJ111);
            else if (_lowPai == PaiKinds.J2) result.DataCopy(MenzenJ222);
            else if (_lowPai == PaiKinds.J3) result.DataCopy(MenzenJ333);
            else if (_lowPai == PaiKinds.J4) result.DataCopy(MenzenJ444);
            else if (_lowPai == PaiKinds.J5) result.DataCopy(MenzenJ555);
            else if (_lowPai == PaiKinds.J6) result.DataCopy(MenzenJ666);
            else if (_lowPai == PaiKinds.J7) result.DataCopy(MenzenJ777);
            else Debug.LogWarning($"GetMentsuStatus : Error , Kootsu(Kantsu) _lowPai = {_lowPai}");

            if (_mentsuKind == MentsuKinds.Kantsu) result.mentsuKind = MentsuKinds.Kantsu;
        }

        if (nakiFlg)
        {
            result.fu /= 2;
        }

        if(_mentsuKind == MentsuKinds.Kantsu)
        {
            result.fu *= 4;
            if (nakiFlg)
            {
                result.nakiKinds = NakiKinds.MinKan;
            }
            else
            {
                result.nakiKinds = NakiKinds.Ankan;
            }
        }
        else if (_mentsuKind == MentsuKinds.Kootsu)
        {
            if (nakiFlg)
            {
                result.nakiKinds = NakiKinds.Pon;
            }
        }
        else if (_mentsuKind == MentsuKinds.Juntsu)
        {
            if (nakiFlg)
            {
                result.nakiKinds = NakiKinds.Chi;
            }
        }

        //Debug.Log($"{_mentsuKind} , {_lowPai} , {nakiFlg}\n{result.minimumPai} , {result.fu} , {result.nakiKinds} , {result.mentsuKind}");
        return result;
    }


#endregion

#region テスト関数

    /// <summary>
    /// テスト関数(デバッグ用)
    /// </summary>
    /// <param name="number"></param>
    public void PushTestButton(int number)
    {
        switch (number)
        {
            case 1: // 牌山をキレイに並べる
                {
                    ResetTransformObjects();

                    int iMax = 8;
                    int jMax = 17;
                    for (int i = 0; i < iMax; i++)
                    {
                        for (int j = 0; j < jMax; j++)
                        {
                            int num = j + (i * jMax);
                            InstantiatePai(paiyama[num].thisKind, paiyama[num].totalNumber, num,
                                new Vector3(j * 2 - 16, 1.5f, 12.5f - i * 2.5f), DEFAULT_ROTATE_PLAYER);
                        }
                    }
                }
                break;
            case 2: // 牌山をそれっぽい位置に配置する
                {
                    ResetTransformObjects();
                    ResetPlayersTehaisAndKawas();

                    int index = 0;

                    for (int i = 0; i < DEFAULT_MAISUU_TEHAI; i++)
                    {
                        InstantiatePai(paiyama[index].thisKind, paiyama[index].totalNumber, index,
                            GetPositionTehai(PlayerKind.Player, i, false), DEFAULT_ROTATE_PLAYER);
                        index++;
                    }
                    for (int i = 0; i < DEFAULT_MAISUU_TEHAI; i++)
                    {
                        InstantiatePai(paiyama[index].thisKind, paiyama[index].totalNumber, index, 
                            GetPositionTehai(PlayerKind.Shimocha, i, false), DEFAULT_ROTATE_SHIMOCHA_TEHAI);
                        index++;
                    }
                    for (int i = 0; i < DEFAULT_MAISUU_TEHAI; i++)
                    {
                        InstantiatePai(paiyama[index].thisKind, paiyama[index].totalNumber, index,
                            GetPositionTehai(PlayerKind.Toimen, i, false), DEFAULT_ROTATE_TOIMEN_TEHAI);
                        index++;
                    }
                    for (int i = 0; i < DEFAULT_MAISUU_TEHAI; i++)
                    {
                        InstantiatePai(paiyama[index].thisKind, paiyama[index].totalNumber, index,
                            GetPositionTehai(PlayerKind.Kamicha, i, false), DEFAULT_ROTATE_KAMICHA_TEHAI);
                        index++;
                    }

                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < 6; j++)
                        {
                            InstantiatePai(paiyama[index].thisKind, paiyama[index].totalNumber, index,
                                GetPositionKawa(PlayerKind.Player, i * 6 + j, INDEX_NONE), DEFAULT_ROTATE_PLAYER);
                            index++;
                            if (i == 3 && j > 1) break;
                        }
                    }
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < 6; j++)
                        {
                            InstantiatePai(paiyama[index].thisKind, paiyama[index].totalNumber, index,
                                GetPositionKawa(PlayerKind.Shimocha, i * 6 + j, INDEX_NONE), DEFAULT_ROTATE_SHIMOCHA_KAWA);
                            index++;
                            if (i == 3 && j > 1) break;
                        }
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 6; j++)
                        {
                            InstantiatePai(paiyama[index].thisKind, paiyama[index].totalNumber, index,
                                GetPositionKawa(PlayerKind.Toimen, i * 6 + j, INDEX_NONE), DEFAULT_ROTATE_TOIMEN_KAWA);
                            index++;
                            if (i == 2 && j > 3) break;
                        }
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 6; j++)
                        {
                            InstantiatePai(paiyama[index].thisKind, paiyama[index].totalNumber, index,
                                GetPositionKawa(PlayerKind.Kamicha, i * 6 + j, INDEX_NONE), DEFAULT_ROTATE_KAMICHA_KAWA);
                            index++;
                            if (i == 2 && j > 3) break;
                        }
                    }

                    InstantiatePai(paiyama[index].thisKind, paiyama[index].totalNumber, index,
                            GetPositionTehai(PlayerKind.Player, 13, true), DEFAULT_ROTATE_PLAYER);
                    index++;

                    for(int i = 0; i < 10; i++)
                    {
                        if (i == 0)
                        {
                            int reIndex = paiyama.Count - 1 - (i + 4);
                            InstantiatePai(paiyama[reIndex].thisKind, paiyama[reIndex].totalNumber, reIndex,
                                    GetPositionWanpai(i), DEFAULT_ROTATE_PLAYER);
                        }
                        else
                        {
                            int reIndex = paiyama.Count - 1 - (i + 4);
                            InstantiatePai(paiyama[reIndex].thisKind, paiyama[reIndex].totalNumber, reIndex,
                                    GetPositionWanpai(i), DEFAULT_ROTATE_URA);
                        }
                    }
                }
                break;
            case 3: // それっぽい局を開始する
                {
                    ResetTransformObjects();
                    ResetPlayersTehaisAndKawas();
                    uiManager.ResetUi();

                    nowKyoku = Kyoku.Ton1;
                    nowHonba = 0;

                    UiManagerChangeKyokuText();

                    ResetKyoku();

                    DealHaiPai();

                    wanpai.MakeDoraHyouji();

                    StartCoroutine(TestCoroutine(1));

                }
                break;
            case 4: // なにで和了れるかを出力する
                {
                    int[] copyTehaiInformationList = new int[playerTehais[0].GetTehaiInformation().Length];
                    System.Array.Copy(playerTehais[0].GetTehaiInformation(), copyTehaiInformationList, playerTehais[0].GetTehaiInformation().Length);

                    if (nowGameTurn == GameTurn.Ton_Tsumo) copyTehaiInformationList[(int)paiyama[nowPaiIndex-1].thisKind]--;

                    string str = "Tenpai : ";
                    for (int i = 0; i < copyTehaiInformationList.Length; i++)
                    {
                        if (i % 10 == 0) continue;
                        int[] copyCopyTehaiInformationList = new int[copyTehaiInformationList.Length];
                        System.Array.Copy(copyTehaiInformationList, copyCopyTehaiInformationList, copyTehaiInformationList.Length);

                        copyCopyTehaiInformationList[i]++;
                        bool result = CheckAgariAll(copyCopyTehaiInformationList);
                        if (result)
                        {
                            str += $"{(PaiKinds)i}, ";
                        }
                    }

                    Debug.Log($"{str}");
                }
                break;
            default:
                Debug.LogWarning($"PushTestButton : Error , Number = {number}");
                break;
        }
    }

    /// <summary>
    /// テスト関数Verコルーチン(デバッグ用)
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    private IEnumerator TestCoroutine(int number)
    {
        yield return null;
        switch (number)
        {
            case 1:
                {
                    foreach (var item in playerTehais)
                    {
                        item.MakeAllTehaiObjects();
                    }
                    yield return new WaitForSeconds(1f);
                    foreach (var item in playerTehais)
                    {
                        item.RiiPai();
                    }
                    yield return new WaitForSeconds(1f);
                    NextGameTurn();
                }
                break;
            case 2:
                {

                }
                break;
            default:
                Debug.LogWarning($"TestCoroutine : Error , Number = {number}");
                break;
        }
    }

#endregion
}
