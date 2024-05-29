using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInformationManager : MonoBehaviour
{
    [SerializeField] private Sprite[] starSprite;

    /// <summary> 変数の初期化が完了しているかのフラグ(falseなら準備できていない) </summary>
    private bool initReadyFlg = false;
    /// <summary> セーブデータが読み込めているかのフラグ(falseなら準備できていない) </summary>
    private bool saveReadyFlg = false;

    /// <summary> ユーザーの設定した名前 </summary>
    private string playerName;
    /// <summary> ログイン日数 </summary>
    private int logInDays;
    /// <summary> 前回のログイン日 </summary>
    private DateTime lastLoginDay;
    /// <summary> SNSで本日シェアしたかのフラグ </summary>
    private bool todaySNSShareFlg;
    /// <summary> リワード広告を獲得した回数 </summary>
    private int rewardTimes;
    /// <summary> SNSでシェアした日数 </summary>
    private int shareDays;

    /// <summary> 設定しているノーツの表示の仕方の種類 </summary>
    private DisplayNotesKind settingDisplayNotesKind;
    /// <summary> 音階名のシールを表示するかのフラグ </summary>
    private bool displayXylophoneStickerFlg;

    /// <summary> 設定したプレイヤーキャラクターの番号 </summary>
    private int selectCharacterNumber;
    /// <summary> 設定したアイコン背景の番号 </summary>
    private int selectIconBackgroundNumber;
    /// <summary> 設定したアイコン枠の番号 </summary>
    private int selectIconFrameNumber;
    /// <summary> 「SPコインをもらう」で前回もらったときのスター数 </summary>
    private int clearStarNumForObtainSpCoin;

    /// <summary> セーブデータをロードした変数 </summary>
    private SaveData save;

    /// <summary> 新規プレイヤーのフラグ </summary>
    private bool newPlayerFlg;

    /// <summary> ステージクリアモード(Normal)のクリア状況 </summary>
    [HideInInspector] public StageClearStarFlgs[] stageClearStarFlgsForNormal;
    /// <summary> ステージクリアモード(Hard)のクリア状況 </summary>
    [HideInInspector] public StageClearStarFlgs[] stageClearStarFlgsForHard;
    /// <summary> ステージクリアモード(Veryhard)のクリア状況 </summary>
    [HideInInspector] public StageClearStarFlgs[] stageClearStarFlgsForVeryHard;
    /// <summary> ランキングモードの自身の記録 </summary>
    [HideInInspector] public float[] rankingTimeRecord;
    /// <summary> 所持アイテム(キャラクター)のフラグ </summary>
    [HideInInspector] public int[] haveItemForCharacters;
    /// <summary> 所持アイテム(背景)のフラグ </summary>
    [HideInInspector] public int[] haveItemForBackgrounds;
    /// <summary> 所持アイテム(フレーム)のフラグ </summary>
    [HideInInspector] public int[] haveItemForFlames;

    /// <summary> ステージクリアモードの獲得スターの総数 (0:Normal , 1:Hard , 2:Veryhard) </summary>
    private int[] haveStarsForStageClearMode;

    /// <summary> ゲームシーン移行の際に必要な情報たち </summary>
    private PlayGameInformation nextPlayGameInformation;

    /// <summary> ステージクリアモードのリストを作成 </summary>
    private StageClearModeListGenerator stageClearModeListGenerator;
    /// <summary> StageClearModeListGeneratorのファイル名(文字列) </summary>
    private const string NAME_OF_STAGE_CLEAR_MODE_LIST_GENERATOR = "StageClearModeListGenerators";

    /// <summary> ランキングモードのリストを作成 </summary>
    private RankingModeListGenerator rankingModeListGenerator;
    /// <summary> RankingModeListGeneratorのファイル名(文字列) </summary>
    private const string NAME_OF_RANKING_MODE_LIST_GENERATOR = "RankingModeListGenerator";

    /// <summary> キャラクターのリストを作成 </summary>
    private PlayerGenerator characterListGenerator;
    /// <summary> キャラクターリスト生成のファイル名(文字列) </summary>
    private const string NAME_OF_CHARACTER_LIST_GENERATOR = "CharactersGenerator";
    /// <summary> アイコン(背景)リストを作成 </summary>
    private IconItemListGenerator iconBackListGenerator;
    /// <summary> アイコン(背景)リスト生成のファイル名(文字列) </summary>
    private const string NAME_OF_ICON_BACK_LIST_GENERATOR = "IconBackListGenerator";
    /// <summary> アイコン(枠)リストを作成 </summary>
    private IconItemListGenerator iconFrameListGenerator;
    /// <summary> アイコン(枠)リスト生成のファイル名(文字列) </summary>
    private const string NAME_OF_ICON_FRAME_LIST_GENERATOR = "IconFrameListGenerator";

    /// <summary> PlayerPrefsのキー(表示するノーツの言語) </summary>
    private const string KEY_PLAYERPREFS_DISPLAY_NOTES_LANGUAGE = "KEY_PLAYERPREFS_DISPLAY_NOTES_LANGUAGE";
    /// <summary> PlayerPrefsのキー(鍵盤のシールを表示するかのフラグ) </summary>
    private const string KEY_PLAYERPREFS_DISPLAY_STICKER_FLG = "KEY_PLAYERPREFS_DISPLAY_STICKER_FLG";

    /// <summary> StageClearStarFlgsのデフォルト値 </summary>
    public readonly StageClearStarFlgs DEFAULT_STAGE_CLEAR_STAR_FLGS = new StageClearStarFlgs(false, false, false, false, 0f);

    private readonly Color COLOR_NORMAL = new Color(1.0f, 1.0f, 0.0f);
    private readonly Color COLOR_HARD = new Color(0.0f, 1.0f, 0.0f);
    private readonly Color COLOR_VERYHARD = new Color(1.0f, 0.5f, 0.0f);
    private readonly Color COLOR_RANKING = new Color(1.0f, 0.0f, 1.0f);

    private const string SCENE_NAME_TITLE = "Title";
    private const string SCENE_NAME_MENU = "Menu";

    /// <summary>
    /// ゲームシーンで取り扱う内容
    /// </summary>
    public struct PlayGameInformation
    {
        /// <summary> ゲームモード </summary>
        public GameMode mode;
        /// <summary> ゲームの難易度 </summary>
        public GameLevel level;
        /// <summary> ステージ番号 </summary>
        public int stageNumber;
        /// <summary> 楽譜名 </summary>
        public string sheetMusicName;
        /// <summary> 目標タイム(ランキングモード時は使用しない) </summary>
        public float targetTime;
        /// <summary> キャラクター名 </summary>
        public string playerCharacterName;
        /// <summary> お手本スキップ機能のフラグ </summary>
        public bool skipForExampleFlg;
        /// <summary> ランキングモードのキー(ステージクリアモード時は使用しない) </summary>
        public string rankingKey;
    }

    /// <summary>
    /// ガチャアイテムの種類
    /// </summary>
    public enum GachaItemKind
    {
        None,
        Character,
        IconBackground,
        IconFrame,
    }

    /// <summary>
    /// ゲームモード
    /// </summary>
    public enum GameMode
    {
        None,
        StageClear,
        Ranking,
        FreeStyle,
    }

    /// <summary>
    /// ゲームの難易度(邪魔なオブジェクトが出てくる)
    /// </summary>
    public enum GameLevel
    {
        None,
        Normal,
        Hard,
        VeryHard,
    }

    /// <summary>
    /// ノーツの表示の仕方の種類
    /// </summary>
    public enum DisplayNotesKind
    {
        None,
        KeyboardNumber,
        Japanese,
        English,
        //Deutsch,
    }


    public static PlayerInformationManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            // シーン遷移しても破棄されないようにする
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 二重で起動されないようにする
            Destroy(gameObject);
        }

    }

    private void Start()
    {
        // スタックトレースを有効にしてNativeArrayのメモリリークを探す
        NativeLeakDetection.Mode = NativeLeakDetectionMode.EnabledWithStackTrace;

        InitValues();
    }

    /// <summary>
    /// 全てを読み込む
    /// </summary>
    public void InitAndSettingPlayerInformation()
    {
        InitValues();
        PlayFabManager.Instance.GetUserSaveData();
    }

    /// <summary>
    /// テストメソッド(デバッグ用)
    /// </summary>
    /// <param name="num"></param>
    private void TestMethod(int num)
    {
        bool defaultFlg = false;
        switch (num)
        {
            case 1: // データの書き込み
                //save.commonData.playerName = "Danchi";
                //save.commonData.logInDays = 3;
                //save.commonData.playerLevel = 12;
                //save.commonData.playerExperience = 26;
                //save.commonData.haveMedal = 1000;
                //save.commonData.haveSPCoin = 5;
                //save.dropBallsData.perfectClearTimes = 1;

                //save.profileData.myBackgroundNumber = 1;
                //save.profileData.myEyeVerticalPosition = 100;
                //save.profileData.myEyeHorizontalPosition = 50;

                //InitializeHavePartsForDefaultAll();

                //save.haveProfileParts.haveBackground[3] = true;

                //SaveJson();
                break;
            case 2: // データの読み込み
                //LoadJson();
                break;
            case 3: // ファイルを削除
                //DeleteSaveFile();
                break;
            case 4: // データを読み込んだ後、指定したデータのインクリメント
                //LoadJson();
                //IncrementField("dropBallsData", "perfectClearTimes");
                break;
            case 5: // ログを出力
                //Debug.Log($"testMethod[5] : {save.haveProfileParts.haveBackground[0]}");
                //Debug.Log($"testMethod[5] : {save.haveProfileParts.haveBackground[1]}");
                //Debug.Log($"testMethod[5] : {save.haveProfileParts.haveBackground[2]}");
                //Debug.Log($"testMethod[5] : {save.haveProfileParts.haveBackground[3]}");
                break;
            default:
                defaultFlg = true;
                break;
        }

        if (!defaultFlg)
        {
            Debug.Log($"PlayerInformationManager.testMethod : num {num}");
        }

        Debug.Log("PlayerInformationManager.TestMethod : Json File ↓\n" + JsonUtility.ToJson(save, true));
    }

    /// <summary>
    /// 変数の初期化
    /// </summary>
    private void InitValues()
    {
        if (initReadyFlg) return;

        initReadyFlg = true;

        haveStarsForStageClearMode = new int[Enum.GetValues(typeof(GameLevel)).Length];
        stageClearModeListGenerator = transform.Find(NAME_OF_STAGE_CLEAR_MODE_LIST_GENERATOR)
            .gameObject.GetComponent<StageClearModeListGenerator>();

        rankingModeListGenerator = transform.Find(NAME_OF_RANKING_MODE_LIST_GENERATOR)
            .gameObject.GetComponent<RankingModeListGenerator>();

        characterListGenerator = transform.Find(NAME_OF_CHARACTER_LIST_GENERATOR)
            .gameObject.GetComponent<PlayerGenerator>();
        iconBackListGenerator = transform.Find(NAME_OF_ICON_BACK_LIST_GENERATOR)
            .gameObject.GetComponent<IconItemListGenerator>();
        iconFrameListGenerator = transform.Find(NAME_OF_ICON_FRAME_LIST_GENERATOR)
            .gameObject.GetComponent<IconItemListGenerator>();


        stageClearStarFlgsForNormal = new StageClearStarFlgs[stageClearModeListGenerator.GetListLength(GameLevel.Normal)];
        stageClearStarFlgsForHard = new StageClearStarFlgs[stageClearModeListGenerator.GetListLength(GameLevel.Hard)];
        stageClearStarFlgsForVeryHard = new StageClearStarFlgs[stageClearModeListGenerator.GetListLength(GameLevel.VeryHard)];
        rankingTimeRecord = new float[rankingModeListGenerator.GetLength()];
        haveItemForCharacters = new int[characterListGenerator.GetLength()]; 
        haveItemForBackgrounds = new int[iconBackListGenerator.GetLength()]; 
        haveItemForFlames = new int[iconFrameListGenerator.GetLength()];

        LoadPlayerPrefsForNotesLanguageAndStickerFlg();
    }

    /// <summary>
    /// jsonファイルを読み込み、クラスの変数に値を代入する
    /// </summary>
    public void InitLoad(SaveData saveFromPlayfab)
    {
        bool nullFlg = false;
        save = new SaveData();

        save.InitializeStageClearStarFlgsForNormal(GetStageClearModeListGenerator().GetListLength(GameLevel.Normal));
        save.InitializeStageClearStarFlgsForHard(GetStageClearModeListGenerator().GetListLength(GameLevel.Hard));
        save.InitializeStageClearStarFlgsForVeryHard(GetStageClearModeListGenerator().GetListLength(GameLevel.VeryHard));
        save.InitializeRankingTime(GetRankingModeListGenerator().GetLength());
        save.InitializeHaveCharacters(GetCharacterListGenerator().GetLength());
        save.InitializeHaveBackgrounds(GetIconBackListGenerator().GetLength());
        save.InitializeHaveFlames(GetIconFrameListGenerator().GetLength());

        //Debug.Log($"PlayerInformationManager.InitLoad : Length List\n" +
        //    $"save.stageClear.normal.Length = {save.stageClear.normal.Length}\n" +
        //    $"save.stageClear.hard.Length = {save.stageClear.hard.Length}\n" +
        //    $"save.stageClear.veryhard.Length = {save.stageClear.veryhard.Length}\n" +
        //    $"save.rankingTime.Length = {save.rankingTime.Length}\n" +
        //    $"save.haveItems.characters.Length = {save.haveItems.characters.Length}\n" +
        //    $"save.haveItems.backgrounds.Length = {save.haveItems.backgrounds.Length}\n" +
        //    $"save.haveItems.flames.Length = {save.haveItems.flames.Length}");

        if (saveFromPlayfab != null) 
        {
            save = saveFromPlayfab;

            playerName = save.commonData.playerName;
            logInDays = save.commonData.logInDays;
            todaySNSShareFlg = save.commonData.todaySNSShareFlg;
            rewardTimes = save.commonData.rewardTimes;
            shareDays = save.commonData.shareDays;
            if (DateTime.TryParse(save.commonData.lastLoginDay, out DateTime dateTime))
            {
                lastLoginDay = DateTime.Parse(save.commonData.lastLoginDay);
            }
            else
            {
                // 変換失敗
                lastLoginDay = DateTime.MinValue;
            }
            selectCharacterNumber = save.commonData.selectCharacterNumber;
            selectIconBackgroundNumber = save.commonData.selectIconBackgroundNumber;
            selectIconFrameNumber = save.commonData.selectIconFrameNumber;
            clearStarNumForObtainSpCoin = save.commonData.clearStarNumForObtainSpCoin;

            //stageClearStarFlgsForNormal = new StageClearStarFlgs[stageClearModeListGenerator.GetListLength(GameLevel.Normal)];
            for (int i = 0; i < stageClearModeListGenerator.GetListLength(GameLevel.Normal); i++)
            {
                if (i < save.stageClear.normal.Length)
                {
                    stageClearStarFlgsForNormal[i] = save.stageClear.normal[i];
                }
                else
                {
                    stageClearStarFlgsForNormal[i] = DEFAULT_STAGE_CLEAR_STAR_FLGS;
                }
            }
            //stageClearStarFlgsForHard = new StageClearStarFlgs[stageClearModeListGenerator.GetListLength(GameLevel.Hard)];
            for (int i = 0; i < stageClearModeListGenerator.GetListLength(GameLevel.Hard); i++)
            {
                if (i < save.stageClear.hard.Length)
                {
                    stageClearStarFlgsForHard[i] = save.stageClear.hard[i];
                }
                else
                {
                    stageClearStarFlgsForHard[i] = DEFAULT_STAGE_CLEAR_STAR_FLGS;
                }
            }
            //stageClearStarFlgsForVeryHard = new StageClearStarFlgs[stageClearModeListGenerator.GetListLength(GameLevel.VeryHard)];
            for (int i = 0; i < stageClearModeListGenerator.GetListLength(GameLevel.VeryHard); i++)
            {
                if (i < save.stageClear.veryhard.Length)
                {
                    stageClearStarFlgsForVeryHard[i] = save.stageClear.veryhard[i];
                }
                else
                {
                    stageClearStarFlgsForVeryHard[i] = DEFAULT_STAGE_CLEAR_STAR_FLGS;
                }
            }

            //rankingTimeRecord = new float[save.rankingTime.Length]; // サイズをランキングモードのステージ数にする
            for (int i = 0; i < save.rankingTime.Length; i++)
            {
                rankingTimeRecord[i] = save.rankingTime[i];
            }
             
            //haveItemForCharacters = new bool[save.haveItems.characters.Length]; // サイズをキャラ数にする
            for (int i = 0; i < save.haveItems.characters.Length; i++)
            {
                haveItemForCharacters[i] = save.haveItems.characters[i];
            }
            //haveItemForBackgrounds = new bool[save.haveItems.backgrounds.Length]; // サイズを背景数にする
            for (int i = 0; i < save.haveItems.backgrounds.Length; i++)
            {
                haveItemForBackgrounds[i] = save.haveItems.backgrounds[i];
            }
            //haveItemForFlames = new bool[save.haveItems.flames.Length]; // サイズをフレーム数にする
            for (int i = 0; i < save.haveItems.flames.Length; i++)
            {
                haveItemForFlames[i] = save.haveItems.flames[i];
            }

            ArraySizeCheckAndChange();
        }
        else
        {
            nullFlg = true;
            Debug.Log($"PlayerInformationManager.InitLoad : Save is null");

            playerName = "";
            logInDays = 0;
            todaySNSShareFlg = false;
            rewardTimes = 0;
            shareDays = 0;
            lastLoginDay = DateTime.MinValue;
            selectCharacterNumber = 0;
            selectIconBackgroundNumber = 0;
            selectIconFrameNumber = 0;
            clearStarNumForObtainSpCoin = 0;

            haveItemForCharacters[0] = 1;
            haveItemForCharacters[1] = 1;
            haveItemForBackgrounds[0] = 1;
            haveItemForBackgrounds[1] = 1;
            haveItemForFlames[0] = 1;
            haveItemForFlames[1] = 1;

            save.commonData.playerName = playerName;
            save.commonData.logInDays = logInDays;
            save.commonData.todaySNSShareFlg = todaySNSShareFlg;
            save.commonData.rewardTimes = rewardTimes;
            save.commonData.shareDays = shareDays;
            save.commonData.lastLoginDay = lastLoginDay.ToString();
            save.commonData.selectCharacterNumber = selectCharacterNumber;
            save.commonData.selectIconBackgroundNumber = selectIconBackgroundNumber;
            save.commonData.selectIconFrameNumber = selectIconFrameNumber;
            save.commonData.clearStarNumForObtainSpCoin = clearStarNumForObtainSpCoin;

            save.haveItems.characters[0] = haveItemForCharacters[0];
            save.haveItems.characters[1] = haveItemForCharacters[1];
            save.haveItems.backgrounds[0] = haveItemForBackgrounds[0];
            save.haveItems.backgrounds[1] = haveItemForBackgrounds[1];
            save.haveItems.flames[0] = haveItemForFlames[0];
            save.haveItems.flames[1] = haveItemForFlames[1];

            PlayFabManager.Instance.UpdateUserDataForIcon();

            //InitHaveDefaultParts();
        }

        // test
        TestMethod(0);

        SaveJson();

        saveReadyFlg = true;

        if (SceneManager.GetActiveScene().name == SCENE_NAME_TITLE)
        {
            TitleSceneManager.Instance.AfterLoadSaveData(nullFlg);
        }
        else if (SceneManager.GetActiveScene().name == SCENE_NAME_MENU)
        {
            UpdateHaveStarsForStageClearMode();
        }
    }

    /// <summary>
    /// jsonファイルを保存する
    /// </summary>
    public void SaveJson()
    {
        PlayFabManager.Instance.UpdateUserDataForSave();

        Debug.Log($"PlayerInformationManager.SaveJson : Saved");
    }

    /// <summary>
    /// PlayFabからSaveデータを受信する
    /// </summary>
    /// <param name="saveData"></param>
    public void ReceptionSaveDataFromPlayFab(SaveData saveData)
    {
        InitLoad(saveData);
    }

    /// <summary>
    /// 配列のサイズに変更があった際、サイズをリサイズする
    /// </summary>
    private void ArraySizeCheckAndChange()
    {
    //    Debug.Log($"ArraySizeCheckAndChange : Length :\n" +
    //$"save.stageClear.normal.Length -> {save.stageClear.normal.Length}\n" +
    //$"stageClearStarFlgsForNormal.Length -> {stageClearStarFlgsForNormal.Length}" +
    //$"");

        if (save.stageClear.normal.Length != stageClearStarFlgsForNormal.Length)
        {
            Debug.Log($"ArraySizeCheckAndChange : Resize StageClearNormal " +
                $"{save.stageClear.normal.Length} -> {stageClearStarFlgsForNormal.Length}");

            Array.Resize(ref save.stageClear.normal, stageClearStarFlgsForNormal.Length);

            for (int i = save.stageClear.normal.Length; i < stageClearStarFlgsForNormal.Length; i++)
            {
                //save.stageClear.normal[i].star1 = false;
                //save.stageClear.normal[i].star2 = false;
                //save.stageClear.normal[i].star3 = false;
                //save.stageClear.normal[i].starAll = false;
                //save.stageClear.normal[i].bestTime = 0f;
                save.stageClear.normal[i] = DEFAULT_STAGE_CLEAR_STAR_FLGS;
            }
            Array.Copy(save.stageClear.normal, stageClearStarFlgsForNormal, stageClearStarFlgsForNormal.Length);
        }

        if (save.stageClear.hard.Length != stageClearStarFlgsForHard.Length)
        {
            Debug.Log($"ArraySizeCheckAndChange : Resize StageClearHard " +
                $"{save.stageClear.hard.Length} -> {stageClearStarFlgsForHard.Length}");

            Array.Resize(ref save.stageClear.hard, stageClearStarFlgsForHard.Length);

            for (int i = save.stageClear.hard.Length; i < stageClearStarFlgsForHard.Length; i++)
            {
                //save.stageClear.hard[i].star1 = false;
                //save.stageClear.hard[i].star2 = false;
                //save.stageClear.hard[i].star3 = false;
                //save.stageClear.hard[i].starAll = false;
                //save.stageClear.hard[i].bestTime = 0f;
                save.stageClear.hard[i] = DEFAULT_STAGE_CLEAR_STAR_FLGS;
            }
            Array.Copy(save.stageClear.hard, stageClearStarFlgsForHard, stageClearStarFlgsForHard.Length);
        }

        if (save.stageClear.veryhard.Length != stageClearStarFlgsForVeryHard.Length)
        {
            Debug.Log($"ArraySizeCheckAndChange : Resize StageClearVeryhard " +
                $"{save.stageClear.veryhard.Length} -> {stageClearStarFlgsForVeryHard.Length}");

            Array.Resize(ref save.stageClear.veryhard, stageClearStarFlgsForVeryHard.Length);

            for (int i = save.stageClear.veryhard.Length; i < stageClearStarFlgsForVeryHard.Length; i++)
            {
                //save.stageClear.veryhard[i].star1 = false;
                //save.stageClear.veryhard[i].star2 = false;
                //save.stageClear.veryhard[i].star3 = false;
                //save.stageClear.veryhard[i].starAll = false;
                //save.stageClear.veryhard[i].bestTime = 0f;
                save.stageClear.veryhard[i] = DEFAULT_STAGE_CLEAR_STAR_FLGS;
            }
            Array.Copy(save.stageClear.veryhard, stageClearStarFlgsForVeryHard, stageClearStarFlgsForVeryHard.Length);
        }

        if (save.rankingTime.Length != rankingTimeRecord.Length)
        {
            Debug.Log($"ArraySizeCheckAndChange : Resize RankingTimeRecord " +
                $"{save.rankingTime.Length} -> {rankingTimeRecord.Length}");

            Array.Resize(ref save.rankingTime, rankingTimeRecord.Length);

            for (int i = save.rankingTime.Length; i < rankingTimeRecord.Length; i++)
            {
                save.rankingTime[i] = 0f;
            }
        }

        if (save.haveItems.characters.Length != haveItemForCharacters.Length)
        {
            Debug.Log($"ArraySizeCheckAndChange : Resize HaveItemForCharacters " +
                $"{save.haveItems.characters.Length} -> {haveItemForCharacters.Length}");

            Array.Resize(ref save.haveItems.characters, haveItemForCharacters.Length);

            for (int i = save.haveItems.characters.Length; i < haveItemForCharacters.Length; i++)
            {
                save.haveItems.characters[i] = 0;
            }
        }

        if (save.haveItems.backgrounds.Length != haveItemForBackgrounds.Length)
        {
            Debug.Log($"ArraySizeCheckAndChange : Resize HaveItemForBackgrounds " +
                $"{save.haveItems.backgrounds.Length} -> {haveItemForBackgrounds.Length}");

            Array.Resize(ref save.haveItems.backgrounds, haveItemForBackgrounds.Length);

            for (int i = save.haveItems.backgrounds.Length; i < haveItemForBackgrounds.Length; i++)
            {
                save.haveItems.backgrounds[i] = 0;
            }
        }

        if (save.haveItems.flames.Length != haveItemForFlames.Length)
        {
            Debug.Log($"ArraySizeCheckAndChange : Resize HaveItemForFlames " +
                $"{save.haveItems.flames.Length} -> {haveItemForFlames.Length}");

            Array.Resize(ref save.haveItems.flames, haveItemForFlames.Length);

            for (int i = save.haveItems.flames.Length; i < haveItemForFlames.Length; i++)
            {
                save.haveItems.flames[i] = 0;
            }
        }
    }

    /// <summary>
    /// プレイヤー名を変更して、jsonファイルに保存する
    /// </summary>
    /// <param name="newName"></param>
    public void SavePlayerName(string newName)
    {
        save.commonData.playerName = newName;
        playerName = newName;
        SaveJson();
        PlayFabManager.Instance.SetPlayerDisplayName(newName);
    }

    /// <summary>
    /// セーブデータの準備ができているかを返すゲッター
    /// </summary>
    /// <returns></returns>
    public bool GetSaveDataReadyFlg()
    {
        return saveReadyFlg;
    }

    /// <summary>
    /// プレイヤーの名前を返すゲッター
    /// </summary>
    /// <returns></returns>
    public string GetPlayerName()
    {
        return playerName;
    }

    /// <summary>
    /// 新しいプレイヤーであるかのフラグを返すゲッター
    /// </summary>
    /// <returns></returns>
    public bool GetNewPlayerFlg()
    {
        return newPlayerFlg;
    }

    /// <summary>
    /// 最終ログイン日を返すゲッター
    /// </summary>
    /// <returns></returns>
    public DateTime GetLastLoginDay()
    {
        return lastLoginDay;
    }

    /// <summary>
    /// 本日のSNSシェア済みかのフラグを返すゲッター
    /// </summary>
    /// <returns></returns>
    public bool GetTodaySNSShareFlg()
    {
        return todaySNSShareFlg;
    }

    /// <summary>
    /// ログイン日数を返すゲッター
    /// </summary>
    /// <returns></returns>
    public int GetLoginDays()
    {
        return logInDays;
    }

    /// <summary>
    /// 「SPコインをもらう」で前回もらったときのスター数を返すゲッター
    /// </summary>
    /// <returns></returns>
    public int GetClearStarNumForObtainSpCoin()
    {
        return clearStarNumForObtainSpCoin;
    }

    /// <summary>
    /// 設定しているキャラクター名を返すゲッター
    /// </summary>
    /// <returns></returns>
    public string GetPlayerCharacterName()
    {
        string name = characterListGenerator.GetCharacterName(selectCharacterNumber);

        return name;
    }

    /// <summary>
    /// 設定しているキャラクターのリスト番号を返すゲッター
    /// </summary>
    /// <returns></returns>
    public int GetPlayerCharacterNumber()
    {
        return selectCharacterNumber;
    }

    /// <summary>
    /// 設定しているアイコン背景名を返すゲッター
    /// </summary>
    /// <returns></returns>
    public string GetSelectIconBackgroundName()
    {
        string name = iconBackListGenerator.GetName(selectIconBackgroundNumber);

        return name;
    }

    /// <summary>
    /// 設定しているアイコン背景のリスト番号を返すゲッター
    /// </summary>
    /// <returns></returns>
    public int GetSelectIconBackgroundNumber()
    {
        return selectIconBackgroundNumber;
    }

    /// <summary>
    /// 設定しているアイコン枠名を返すゲッター
    /// </summary>
    /// <returns></returns>
    public string GetSelectIconFrameName()
    {
        string name = iconFrameListGenerator.GetName(selectIconFrameNumber);

        return name;
    }

    /// <summary>
    /// 設定しているアイコン枠のリスト番号を返すゲッター
    /// </summary>
    /// <returns></returns>
    public int GetSelectIconFrameNumber()
    {
        return selectIconFrameNumber;
    }

    /// <summary>
    /// 指定したガチャアイテムを取得し、初めての入手アイテムかを返す
    /// </summary>
    /// <param name="itemKinds">アイテムの種類</param>
    /// <param name="itemNumber">アイテムの要素番号</param>
    /// <returns>初めて入手ならTrue</returns>
    public bool AcquisitionItemAndGetNewFlg(GachaItemKind itemKinds, int itemNumber)
    {
        bool errorFlg = false;
        bool newFlg = false;

        switch (itemKinds)
        {
            case GachaItemKind.Character:
                if (itemNumber > characterListGenerator.GetLength())
                {
                    errorFlg = true;
                }
                else if (itemNumber > save.haveItems.characters.Length)
                {
                    errorFlg = true;
                }
                haveItemForCharacters[itemNumber]++;
                save.haveItems.characters[itemNumber]++;
                newFlg = haveItemForCharacters[itemNumber] == 1 ? true : false;
                break;
            case GachaItemKind.IconBackground:
                if (itemNumber > GetIconBackListGenerator().GetLength())
                {
                    errorFlg = true;
                }
                else if (itemNumber > save.haveItems.backgrounds.Length)
                {
                    errorFlg = true;
                }
                haveItemForBackgrounds[itemNumber]++;
                save.haveItems.backgrounds[itemNumber]++;
                newFlg = haveItemForBackgrounds[itemNumber] == 1 ? true : false;
                break;
            case GachaItemKind.IconFrame:
                if (itemNumber > GetIconFrameListGenerator().GetLength())
                {
                    errorFlg = true;
                }
                else if (itemNumber > save.haveItems.flames.Length)
                {
                    errorFlg = true;
                }
                haveItemForFlames[itemNumber]++;
                save.haveItems.flames[itemNumber]++;
                newFlg = haveItemForFlames[itemNumber] == 1 ? true : false;
                break;
            default:
                errorFlg = true;
                break;
        }

        if (errorFlg)
        {
            Debug.LogError($"PlayerInformationManager.AcquisitionParts : Error");
        }
        else
        {
            //SaveJson();
        }

        return newFlg;
    }

    /// <summary>
    /// キャラクターとアイコン変更とその保存
    /// </summary>
    /// <param name="name"></param>
    public void UpdatePlayerCharacterAndIcon(int _characterNumber, int _backgroundNumber, int _frameNumber)
    {
        selectCharacterNumber = _characterNumber;
        save.commonData.selectCharacterNumber = _characterNumber;

        selectIconBackgroundNumber = _backgroundNumber;
        save.commonData.selectIconBackgroundNumber = _backgroundNumber;

        selectIconFrameNumber = _frameNumber;
        save.commonData.selectIconFrameNumber = _frameNumber;

        SaveJson();
    }

    /// <summary>
    /// ゲームモードの内容を返すゲッター
    /// </summary>
    /// <returns></returns>
    public PlayGameInformation GetNextPlayGameInformation()
    {
        return nextPlayGameInformation;
    }

    /// <summary>
    /// ゲームモードの内容を設定するセッター
    /// </summary>
    /// <param name="informations"></param>
    public void SetNextPlayGameInformation(PlayGameInformation informations)
    {
        nextPlayGameInformation = informations;
    }

    /// <summary>
    /// ログイン日数を更新し、最終ログイン日を今日にする
    /// </summary>
    public void UpdateLoginDays()
    {
        if (lastLoginDay == DateTime.Today) return;

        lastLoginDay = DateTime.Today;
        logInDays++;
        todaySNSShareFlg = false;

        save.commonData.lastLoginDay = lastLoginDay.ToString();
        save.commonData.logInDays = logInDays;
        save.commonData.todaySNSShareFlg = todaySNSShareFlg;

        Debug.Log($"UpdateLoginDays : Update Login Day\nlastLoginDay = {lastLoginDay} , logInDays = {logInDays}");

        //SaveJson();
    }

    /// <summary>
    /// 本日のSNSシェアを済みにし、更新して保存する
    /// </summary>
    public void UpdateAndSetTodaySNSShareFlgTrue()
    {
        todaySNSShareFlg = true;
        save.commonData.todaySNSShareFlg = todaySNSShareFlg;

        shareDays++;
        save.commonData.shareDays = shareDays;

        SaveJson();
    }

    /// <summary>
    /// Reward広告を見た回数をインクリメントして更新する
    /// </summary>
    public void UpdateRewardTimes()
    {
        rewardTimes++;
        save.commonData.rewardTimes = rewardTimes;
        SaveJson();
    }

    /// <summary>
    /// ステージクリアモードのクリア状況を更新してセーブする
    /// </summary>
    public void UpdateSaveDataForStageClearMode()
    {
        //Debug.Log($"Length :\n" +
        //   $"stageClearStarFlgsForNormal -> {stageClearStarFlgsForNormal.Length}\n" +
        //   $"save.stageClear.normal -> {save.stageClear.normal.Length}" +
        //   $"");

        Array.Copy(stageClearStarFlgsForNormal, save.stageClear.normal, stageClearStarFlgsForNormal.Length);
        Array.Copy(stageClearStarFlgsForHard, save.stageClear.hard, stageClearStarFlgsForHard.Length);
        Array.Copy(stageClearStarFlgsForVeryHard, save.stageClear.veryhard, stageClearStarFlgsForVeryHard.Length);

        SaveJson();
    }

    /// <summary>
    /// ランキングモードのスコアを更新してセーブする
    /// </summary>
    /// <param name="stageNumber"></param>
    /// <param name="score"></param>
    public void UpdateRankingRecordScore(int stageNumber,float score)
    {
        if (score < rankingTimeRecord[stageNumber] || rankingTimeRecord[stageNumber] <= 0f)
        {
            rankingTimeRecord[stageNumber] = score;
            save.rankingTime[stageNumber] = score;
            SaveJson();
        }
        else
        {
            Debug.LogError($"UpdateRankingRecordScore : Score Error!\n" +
                $"New Score = {score} , Old Score = {rankingTimeRecord[stageNumber]}");
        }
    }

    /// <summary>
    /// 「SPコインをもらう」で前回もらったときのスター数を更新し、セーブする
    /// </summary>
    public void UpdateClearStarNumAndObtainSpCoin()
    {
        int totalStar = GetTotalHaveStarsForStageClearMode();

        if (save.commonData.clearStarNumForObtainSpCoin < totalStar)
        {
            clearStarNumForObtainSpCoin = totalStar;
            save.commonData.clearStarNumForObtainSpCoin = totalStar;
            SaveJson();
        }
        else
        {
            Debug.LogError($"PlayerInformationManager.UpdateClearStarNumForObtainSpCoin : " +
                $"Error save.commonData.clearStarNumForObtainSpCoin >= totalStar\n" +
                $"{save.commonData.clearStarNumForObtainSpCoin} >= {totalStar}");

            return;
        }
    }

    /// <summary>
    /// 設定キャラクターを更新して保存する
    /// </summary>
    /// <param name="listNumber"></param>
    public void UpdateSelectCharacter(int listNumber)
    {
        selectCharacterNumber = listNumber;
        save.commonData.selectCharacterNumber = listNumber;
        //SaveJson();
    }

    /// <summary>
    /// 設定したアイコン背景を更新して保存する
    /// </summary>
    /// <param name="listNumber"></param>
    public void UpdateSelectIconBackground(int listNumber)
    {
        selectIconBackgroundNumber = listNumber;
        save.commonData.selectIconBackgroundNumber = listNumber;
        //SaveJson();
    }

    /// <summary>
    /// 設定したアイコン枠を更新して保存する
    /// </summary>
    /// <param name="listNumber"></param>
    public void UpdateSelectIconFrame(int listNumber)
    {
        selectIconFrameNumber = listNumber;
        save.commonData.selectIconFrameNumber = listNumber;
        //SaveJson();
    }

    /// <summary>
    /// 指定したクリアスターのスプライトを返すゲッター
    /// </summary>
    /// <param name="number">0,1,2</param>
    /// <returns></returns>
    public Sprite GetStarSprite(int number)
    {
        if (0 <= number && number < starSprite.Length)
        {
            return starSprite[number];
        }
        else
        {
            Debug.LogError($"PlayerInformationManager.GetStarSprite : Number Error -> {number}");
            return null;
        }
    }

    /// <summary>
    /// デバッグボタンを押したとき
    /// </summary>
    public void PushDebugButton()
    {
        TestMethod(1);
    }

    /// <summary>
    /// スター獲得数を計算して更新する
    /// </summary>
    public void UpdateHaveStarsForStageClearMode()
    {
        //Debug.Log($"PlayerInformationManager.UpdateHaveStarsForStageClearMode : \n" +
        //    $"stageClearStarFlgsForNormal.Length = {stageClearStarFlgsForNormal.Length}");

        int counter = 0;
        //int testCount = -1;
        foreach (var hoge in stageClearStarFlgsForNormal)
        {
            //testCount++;
            //Debug.Log($"hoge[{testCount}] : hoge.star1 = {hoge.star1}");
            if (hoge.star1) counter++;
            if (hoge.star2) counter++;
            if (hoge.star3) counter++;
        }
        haveStarsForStageClearMode[0] = counter;

        counter = 0;
        foreach (var hoge in stageClearStarFlgsForHard)
        {
            if (hoge.star1) counter++;
            if (hoge.star2) counter++;
            if (hoge.star3) counter++;
        }
        haveStarsForStageClearMode[1] = counter;

        counter = 0;
        foreach (var hoge in stageClearStarFlgsForVeryHard)
        {
            if (hoge.star1) counter++;
            if (hoge.star2) counter++;
            if (hoge.star3) counter++;
        }
        haveStarsForStageClearMode[2] = counter;

        //Debug.Log($"PlayerInformationManager.UpdateHaveStarsForStageClearMode : Have Stars ↓\n" +
        //    $"Normal = {haveStarsForStageClearMode[0]} , " +
        //    $"Hard  = {haveStarsForStageClearMode[1]} , " +
        //    $"VeryHard  = {haveStarsForStageClearMode[2]}");
    }

    /// <summary>
    /// 難易度ごとの獲得スターの総数を返すゲッター
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public int GetHaveStarsForStageClearMode(GameLevel level)
    {
        int index;
        switch (level)
        {
            case GameLevel.Normal: index = 0; break;
            case GameLevel.Hard: index = 1; break;
            case GameLevel.VeryHard: index = 2; break;
            default: Debug.LogError("MenuSceneManager.GetHaveStarsForStageClearMode : level Error"); return -1;
        }

        return haveStarsForStageClearMode[index];
    }

    /// <summary>
    /// 全難易度の獲得スターの総数を返すゲッター
    /// </summary>
    /// <returns></returns>
    public int GetTotalHaveStarsForStageClearMode()
    {
        int totalStarNum = 0;

        foreach(int item in haveStarsForStageClearMode)
        {
            totalStarNum += item;
        }

        return totalStarNum;
    }

    /// <summary>
    /// 指定した難易度のそのステージがクリア済み(スター1を取得済み)かを返すゲッター
    /// </summary>
    /// <param name="level"></param>
    /// <param name="stageNumber"></param>
    /// <returns></returns>
    public bool GetAlreadyStageClearFlg(GameLevel level,int stageNumber)
    {
        bool flg = false;
        switch (level)
        {
            case GameLevel.Normal: flg = stageClearStarFlgsForNormal[stageNumber].star1; break;
            case GameLevel.Hard: flg = stageClearStarFlgsForHard[stageNumber].star1; break;
            case GameLevel.VeryHard: flg = stageClearStarFlgsForVeryHard[stageNumber].star1; break;
            default: Debug.LogError("MenuSceneManager.GetAlreadyStageClearFlg : level Error"); return false;
        }

        return flg;
    }

    /// <summary>
    /// StageClearModeListGeneratorを返すゲッター
    /// </summary>
    /// <returns></returns>
    public StageClearModeListGenerator GetStageClearModeListGenerator()
    {
        return stageClearModeListGenerator;
    }

    /// <summary>
    /// RankingModeListGeneratorを返すゲッター
    /// </summary>
    /// <returns></returns>
    public RankingModeListGenerator GetRankingModeListGenerator()
    {
        return rankingModeListGenerator;
    }

    /// <summary>
    /// キャラクターリストのGeneratorを返すゲッター
    /// </summary>
    /// <returns></returns>
    public PlayerGenerator GetCharacterListGenerator()
    {
        return characterListGenerator;
    }

    /// <summary>
    /// アイコン(背景)のGeneratorを返すゲッター
    /// </summary>
    /// <returns></returns>
    public IconItemListGenerator GetIconBackListGenerator()
    {
        return iconBackListGenerator;
    }

    /// <summary>
    /// アイコン(枠)のGeneratorを返すゲッター
    /// </summary>
    /// <returns></returns>
    public IconItemListGenerator GetIconFrameListGenerator()
    {
        return iconFrameListGenerator;
    }

    /// <summary>
    /// 難易度のカラーを返すゲッター
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public Color GetLevelColor(GameLevel level)
    {
        Color col;
        if (level == GameLevel.Normal)
        {
            col = COLOR_NORMAL;
        }
        else if (level == GameLevel.Hard)
        {
            col = COLOR_HARD;
        }
        else if (level == GameLevel.VeryHard)
        {
            col = COLOR_VERYHARD;
        }
        else
        {
            Debug.LogError($"MenuSceneManager.GetLevelColor : Level Error {level}");
            col = Color.white;
        }
        return col;
    }

    /// <summary>
    /// ランキングモードのカラーを返すゲッター
    /// </summary>
    /// <returns></returns>
    public Color GetRankingModeColor()
    {
        return COLOR_RANKING;
    }

    /// <summary>
    /// saveのゲッター
    /// </summary>
    /// <returns></returns>
    public SaveData GetSaveData()
    {
        return save;
    }

    /// <summary>
    /// 設定している音階名の言語を返すゲッター
    /// </summary>
    /// <returns></returns>
    public DisplayNotesKind GetSettingDisplayNotesKind()
    {
        return settingDisplayNotesKind;
    }

    /// <summary>
    /// 鍵盤シールを表示するかのフラグを返すゲッター
    /// </summary>
    /// <returns></returns>
    public bool GetDisplayXylophoneStickerFlg()
    {
        return displayXylophoneStickerFlg;
    }

    /// <summary>
    /// ノーツ表示の日本語文字列を返す
    /// </summary>
    /// <param name="kind"></param>
    /// <returns></returns>
    public string GetDisplayNotesKindToJapaneseString(DisplayNotesKind kind)
    {
        string str;
        switch (kind)
        {
            case DisplayNotesKind.KeyboardNumber:
                str = "ピアノの鍵盤番号";
                break;
            case DisplayNotesKind.Japanese:
                str = "日本式(ドレミ…)";
                break;
            case DisplayNotesKind.English:
                str = "イギリス式(CDE…)";
                break;
            default:
                str = "";
                break;
        }

        return str;
    }

    /// <summary>
    /// ノーツの言語と鍵盤シールの表示を管理する値をPlayerPrefsに保存する
    /// </summary>
    /// <param name="kind">表示する言語</param>
    /// <param name="flg">true : 表示する</param>
    public void SavePlayerPrefsForNotesLanguageAndStickerFlg(DisplayNotesKind kind , bool flg)
    {
        settingDisplayNotesKind = kind;
        displayXylophoneStickerFlg = flg;
        string str = kind.ToString();
        int flgInt = flg ? 1 : 0;
        PlayerPrefs.SetString(KEY_PLAYERPREFS_DISPLAY_STICKER_FLG, str);
        PlayerPrefs.SetInt(KEY_PLAYERPREFS_DISPLAY_STICKER_FLG, flgInt);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// ノーツの言語と鍵盤シールの表示を管理する値をPlayerPrefsから読み込む
    /// </summary>
    public void LoadPlayerPrefsForNotesLanguageAndStickerFlg()
    {
        string str = PlayerPrefs.GetString(KEY_PLAYERPREFS_DISPLAY_STICKER_FLG, "Japanese");
        settingDisplayNotesKind = Enum.TryParse(str, out DisplayNotesKind auth) ? auth : DisplayNotesKind.Japanese;
        displayXylophoneStickerFlg = PlayerPrefs.GetInt(KEY_PLAYERPREFS_DISPLAY_STICKER_FLG, 1) == 1 ? true : false;
    }


}
