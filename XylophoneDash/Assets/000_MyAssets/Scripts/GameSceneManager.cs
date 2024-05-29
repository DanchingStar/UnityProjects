using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Text.RegularExpressions;

public class GameSceneManager : MonoBehaviour
{
    /// <summary> UIManager </summary>
    [SerializeField] private UIManager uIManager;
    /// <summary> Player関連の親オブジェクト </summary>
    [SerializeField] private GameObject playerObjects;
    /// <summary> CanvasのTransform </summary>
    [SerializeField] private Transform canvasTransform;
    /// <summary> 遊び方Prefab </summary>
    [SerializeField] private GameObject howToPlayPrefab;

    [Space(10)]
    [Header("[Test Status]")]

    /// <summary> お手本をスキップするかのフラグ(trueでスキップ) </summary>
    [SerializeField] private bool skipForExampleFlg;
    /// <summary> ステージの難易度 </summary>
    [SerializeField] private PlayerInformationManager.GameLevel stageLevel;
    /// <summary> プレイヤーキャラの名前 </summary>
    [SerializeField] private string playerCharactorName;
    /// <summary> 楽譜の名前 </summary>
    [SerializeField] private string sheetMusicName;
    /// <summary> ステージBGM </summary>
    [SerializeField] private AudioClip stageBgmClip;

    /// <summary> PlayerInformationManagerから引っ張ってきたこのゲームの情報 </summary>
    private PlayerInformationManager.PlayGameInformation myGameInformation;

    /// <summary> 楽譜のジェネレーター </summary>
    private SheetMusicListGenerator sheetMusicListGenerator;
    /// <summary> 定点カメラ </summary>
    private CinemachineVirtualCamera fixedPointCamera;
    /// <summary> プレイヤー頭上のカメラ(ゲームオーバーのときに使う) </summary>
    private CinemachineVirtualCamera playerTopCamera;
    /// <summary> プレイヤーを追うカメラ </summary>
    private CinemachineVirtualCamera playerFollowCamera;

    private ParticleSystem kamifubuki;

    /// <summary> プレイヤーキャラのTransform </summary>
    private Transform playerArmatureTransform;
    /// <summary> プレイする楽譜のデータ </summary>
    private SheetMusic sheetMusic;

    /// <summary> 表示する音階の種類 </summary>
    private PlayerInformationManager.DisplayNotesKind displayNotesKind;

    /// <summary> 譜面演奏の進捗(ノーツ数到達でクリア) </summary>
    private int progress;
    /// <summary> プレイしている現在のタイム </summary>
    private float playTimer;
    /// <summary> ミスった回数 </summary>
    private int missCount;
    /// <summary> スタート前のカウントダウンの数字 </summary>
    private int countDownValue;

    /// <summary> お手本で見せるときに変えるカラー </summary>
    private readonly Color COLOR_EXAMPLE = Color.yellow;

    /// <summary> 定点カメラのフラグ </summary>
    private bool fixedPointCameraFlg;

    /// <summary> 現在のフェーズ </summary>
    private NowPhase nowPhase;

    /// <summary> ポーズ中かのフラグ </summary>
    private bool pauseFlg;

    /// <summary> お手本のコルーチン(Skipに使う) </summary>
    private Coroutine forExampleCoroutine;


    /// <summary>
    /// 読み込んだ楽譜を覚える構造体
    /// </summary>
    public struct SheetMusic
    {
        /// <summary> タイトル </summary>
        public string title;
        /// <summary> 拍子の分子 </summary>
        public float hyoushiBunshi;
        /// <summary> 拍子の分母 </summary>
        public float hyoushiBunbo;
        /// <summary> テンポ </summary>
        public float tempo;
        /// <summary> ノーツ数 </summary>
        public int notesNum;
        /// <summary> 音符たち </summary>
        public List<SingleNote> notes;
    }

    /// <summary>
    /// 音符ひとつごとの詳細
    /// </summary>
    public struct SingleNote
    {
        /// <summary> 鍵盤名 </summary>
        public XylophoneManager.Notes keyBoardName;
        /// <summary> 〇〇分音符など、〇〇の数字 </summary>
        public float bu;

        public SingleNote(XylophoneManager.Notes keyBoardName, float bu)
        {
            this.keyBoardName = keyBoardName;
            this.bu = bu;
        }
    }

    /// <summary>
    /// 現在のフェーズ
    /// </summary>
    public enum NowPhase
    {
        Start,
        HowToPlay,
        ForExample,
        StandBy,
        Play,
        GameOver,
        GameClear,
    }

    public static GameSceneManager Instance;
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
        LoadPlayGameInformation();

        InitVariable();
        InitChildrenComponent();
        InitPlayerPrefab();
        InitSheetMusic();
        InitSounds();

        ChangeFixedPointCamera(false);
        ChangeDisplayNotesKind();

        InitUI();

        SwitchOperatePlayerEnable(false);

        DecidePhaseForStart();
    }

    private void Update()
    {
        UpdateTimer();
    }

    /// <summary>
    /// 変数の初期化
    /// </summary>
    private void InitVariable()
    {
        ChangeNowPhase(NowPhase.Start, 0f);
        playTimer = 0f;
        missCount = 0;
        countDownValue = 100;
    }

    /// <summary>
    /// 子要素のコンポーネントを扱うための初期化
    /// </summary>
    private void InitChildrenComponent()
    {
        sheetMusicListGenerator = transform.Find("SheetMusicListGenerator").GetComponent<SheetMusicListGenerator>();

        fixedPointCamera = transform.Find("FixedPointCamera").GetComponent<CinemachineVirtualCamera>();
        playerTopCamera = transform.Find("PlayerTopCamera").GetComponent<CinemachineVirtualCamera>();
        playerFollowCamera = playerObjects.transform.Find("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();

        kamifubuki = transform.Find("KamifubukiParticle").GetComponent<ParticleSystem>();
        DisplayKamifubuki(false);
    }

    /// <summary>
    /// プレイヤーPrefabの初期化
    /// </summary>
    private void InitPlayerPrefab()
    {
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) // すでにPlayerが存在しているとき
        {
            playerArmatureTransform = playerObj.transform;

            Debug.LogError($"GameSceneManager.InitPlayerPrefab : Already Exist PlayerArmature ! ");
        }
        else
        {
            var prefab = PlayerInformationManager.Instance.GetCharacterListGenerator()
                .GetPlayerPrefab(myGameInformation.playerCharacterName);

            if (prefab != null)
            {
                playerObj = Instantiate(prefab, playerObjects.transform);

                playerArmatureTransform = playerObj.transform;
            }
            else
            {
                Debug.LogError($"GameSceneManager.InitPlayerPrefab : Prefab is Nothing ! ");
            }
        }

        if (playerArmatureTransform != null)
        {
            playerTopCamera.Follow = playerArmatureTransform;
            playerTopCamera.LookAt = playerArmatureTransform;
            playerFollowCamera.Follow = playerObj.transform.Find("PlayerCameraRoot");
        }
        else
        {
            Debug.LogError($"GameSceneManager.InitPlayerPrefab : PlayerArmatureTransform is Null ! ");
        }


    }

    /// <summary>
    /// 楽譜の読み込みや初期化
    /// </summary>
    private void InitSheetMusic()
    {
        if (myGameInformation.mode == PlayerInformationManager.GameMode.None ||
            myGameInformation.mode == PlayerInformationManager.GameMode.FreeStyle)
        {
            sheetMusic.notes = null;
            sheetMusic.title = "";
            sheetMusic.notesNum = 0;
            sheetMusic.tempo = 0;
            sheetMusic.hyoushiBunshi = 0;
            sheetMusic.hyoushiBunbo = 0;

            if (myGameInformation.mode == PlayerInformationManager.GameMode.None)
            {
                Debug.LogError($"GameSceneManager.InitSheetMusic : Mode is None !");
            }
        }
        else
        {
            sheetMusic.notes = new List<SingleNote>();

            TextAsset textfile = sheetMusicListGenerator.GetSheetMusicFile(myGameInformation.sheetMusicName);

            StringReader stackLevelCostReader = new StringReader(textfile.text);
            string text = stackLevelCostReader.ReadToEnd();

            string[] allText = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            int i = 0;
            foreach (var item in allText)
            {
                // 改行をカットする
                string itemStr = CutEnter(item);

                if (string.IsNullOrEmpty(itemStr))
                {
                    //break;
                }
                else
                {
                    if (i == 0)
                    {
                        sheetMusic.title = itemStr;
                    }
                    else if (i == 1)
                    {
                        string[] str = itemStr.Split('/');
                        sheetMusic.hyoushiBunshi = float.Parse(str[0]);
                        sheetMusic.hyoushiBunbo = float.Parse(str[1]);
                    }
                    else if (i == 2)
                    {
                        sheetMusic.tempo = float.Parse(itemStr);
                    }
                    else
                    {
                        if (itemStr == "END") break;

                        string[] str = itemStr.Split(',');

                        XylophoneManager.Notes notes;

                        if (Enum.TryParse(str[0], out XylophoneManager.Notes auth))
                        {
                            notes = auth;
                        }
                        else if (Enum.TryParse(str[0], out XylophoneManager.NotesAnother authAnother))
                        {
                            notes = (XylophoneManager.Notes)authAnother;
                        }
                        else
                        {
                            Debug.LogError($"GameSceneManager.InitSheetMusic : Not Find Notes Name -> str[0] = {str[0]} , item = {itemStr} , i = {i}");
                            notes = XylophoneManager.Notes.None;
                        }

                        if (float.TryParse(str[1], out float result))
                        {
                            sheetMusic.notes.Add(new SingleNote(notes, result));
                        }
                        else
                        {
                            Debug.LogError($"GameSceneManager.InitSheetMusic : Cannot Be Float -> str[1] = {str[1]} , item = {itemStr} , i = {i}");
                            sheetMusic.notes.Add(new SingleNote(notes, 0f));
                        }
                    }

                    i++;
                }
            }
            sheetMusic.notesNum = sheetMusic.notes.Count;


            // 楽譜ファイルのロードを確認するログ
            string debugText = $"タイトル : {sheetMusic.title}\n拍子 : {sheetMusic.hyoushiBunshi}/{sheetMusic.hyoushiBunbo}\n" +
                $"テンポ : {sheetMusic.tempo}\nノーツ数 : {sheetMusic.notesNum}\n";
            foreach (var item in sheetMusic.notes)
            {
                debugText += $"{item.keyBoardName}({item.bu})\n";
            }
            Debug.Log(debugText);
        }

    }

    /// <summary>
    /// BGMなどの初期設定
    /// </summary>
    private void InitSounds()
    {
        SoundManager.Instance.SetStageMusic(stageBgmClip);
        SoundManager.Instance.SetSuccessMusic(sheetMusicListGenerator.GetSuccessMusicClip(myGameInformation.sheetMusicName));
    }

    /// <summary>
    /// テキストなどのUIの初期化
    /// </summary>
    private void InitUI()
    {
        if (myGameInformation.mode == PlayerInformationManager.GameMode.FreeStyle)
        {
            uIManager.SetFreeMode(myGameInformation.level);
        }
        else
        {
            uIManager.SetTimerText(playTimer);
            uIManager.SetMissCountText(missCount);

            UpdateNextsText();
        }

    }

    /// <summary>
    /// 時間とそのUIを更新する
    /// </summary>
    private void UpdateTimer()
    {
        if (myGameInformation.mode == PlayerInformationManager.GameMode.None)
        {

        }
        else if (myGameInformation.mode == PlayerInformationManager.GameMode.FreeStyle)
        {
            if (nowPhase == NowPhase.Play)
            {
                if (!pauseFlg)
                {
                    playTimer += Time.deltaTime;
                }
            }
        }
        else
        {
            if (nowPhase == NowPhase.Play)
            {
                if (!pauseFlg)
                {
                    playTimer += Time.deltaTime;
                    uIManager.SetTimerText(playTimer);
                }
            }
        }
    }

    /// <summary>
    /// ゲーム情報を読み込む
    /// </summary>
    private void LoadPlayGameInformation()
    {
        bool errorFlg = false;

        if (PlayerInformationManager.Instance == null)
        {
            errorFlg = true;
            Debug.LogError($"LoadPlayGameInformation : PlayerInformationManager.Instance is Nothing !!");
        }
        else if (PlayerInformationManager.Instance.GetNextPlayGameInformation().mode == PlayerInformationManager.GameMode.None)
        {
            errorFlg = true;
            Debug.LogError($"LoadPlayGameInformation : GameMode is None !!");
        }
        else
        {
            myGameInformation = PlayerInformationManager.Instance.GetNextPlayGameInformation();
        }

        if (errorFlg)
        {
            myGameInformation.sheetMusicName = sheetMusicName;
            myGameInformation.skipForExampleFlg = skipForExampleFlg;
            myGameInformation.playerCharacterName = playerCharactorName;
            myGameInformation.level = stageLevel;

            myGameInformation.mode = PlayerInformationManager.GameMode.StageClear;
            myGameInformation.stageNumber = -99;
            myGameInformation.targetTime = 60f;
        }

    }

    /// <summary>
    /// 難易度に応じて敵を配置する
    /// </summary>
    private void SpawnEnemies()
    {
        if (myGameInformation.level == PlayerInformationManager.GameLevel.Normal)
        {

        }
        else if (myGameInformation.level == PlayerInformationManager.GameLevel.Hard)
        {
            SpawnEnemiesForHard();
        }
        else if (myGameInformation.level == PlayerInformationManager.GameLevel.VeryHard)
        {
            SpawnEnemiesForHard();
            SpawnEnemiesForVeryhard();
        }
        else
        {
            Debug.LogError($"GameSceneManager.SpawnEnemys : Level Error {myGameInformation.level}");
        }
    }

    /// <summary>
    /// Hard用の敵を配置する
    /// </summary>
    private void SpawnEnemiesForHard()
    {
        EnemyManager.Instance.SpawnFlyEnemy(0, 0.8f);
        EnemyManager.Instance.SpawnFlyEnemy(1, 0.5f);
        EnemyManager.Instance.SpawnFlyEnemy(2, 0.2f);
        EnemyManager.Instance.SpawnFlyEnemy(3, 0.5f);
        EnemyManager.Instance.SpawnFlyEnemy(4, 0.8f);
    }

    /// <summary>
    /// VeryHard用の敵を配置する
    /// </summary>
    private void SpawnEnemiesForVeryhard()
    {
        EnemyManager.Instance.SpawnWalkUnderEnemy(0, 0.2f);
        EnemyManager.Instance.SpawnWalkUnderEnemy(0, 0.8f);
        EnemyManager.Instance.SpawnWalkUnderEnemy(2, 0.2f);
        EnemyManager.Instance.SpawnWalkUnderEnemy(2, 0.8f);

        EnemyManager.Instance.SpawnWalkOverEnemy(0, 0.2f);
        EnemyManager.Instance.SpawnWalkOverEnemy(0, 0.8f);
        EnemyManager.Instance.SpawnWalkOverEnemy(2, 0.2f);
        EnemyManager.Instance.SpawnWalkOverEnemy(2, 0.8f);
    }

    /// <summary>
    /// Start内で、モードによってフェーズを決める
    /// </summary>
    private void DecidePhaseForStart()
    {
        if (myGameInformation.mode == PlayerInformationManager.GameMode.None)
        {
            Debug.LogError($"GameSceneManager.DecidePhaseForStart : GameMode is None ! ");
            ChangeNowPhase(NowPhase.Play, 0f);
        }
        else if (myGameInformation.mode == PlayerInformationManager.GameMode.FreeStyle)
        {
            SwitchOperatePlayerEnable(true);
            ChangeNowPhase(NowPhase.Play, 0f);
            SpawnEnemies();
        }
        else if (myGameInformation.mode == PlayerInformationManager.GameMode.StageClear && myGameInformation.skipForExampleFlg == false &&
            PlayerInformationManager.Instance.GetTotalHaveStarsForStageClearMode() <= 0)
        {
            ChangeNowPhase(NowPhase.HowToPlay, 0.5f);
        }
        else
        {
            ChangeNowPhase(NowPhase.ForExample, 0.5f);
        }
    }

    /// <summary>
    /// 鍵盤を踏んだ時に受信する
    /// </summary>
    /// <param name="notesKeyboard"></param>
    public void ReceptionStepKeyboard(NotesKeyboard notesKeyboard)
    {
        var stepNotes = notesKeyboard.GetMyNotes();

        XylophoneManager.Instance.PlayNotes(stepNotes);

        //Debug.Log($"GameSceneManager.StepKeyboard : Step {stepNotes} ");


        if (myGameInformation.mode == PlayerInformationManager.GameMode.None ||
            myGameInformation.mode == PlayerInformationManager.GameMode.FreeStyle)
        {
            notesKeyboard.ChangeMaterialColorAnother();
        }
        else
        {
            if (stepNotes == sheetMusic.notes[progress].keyBoardName)
            {
                notesKeyboard.ChangeMaterialColorSuccess();
                progress++;
                UpdateNextsText();
                if (progress == sheetMusic.notesNum)
                {
                    ChangeNowPhase(NowPhase.GameClear, 0f);
                    XylophoneManager.Instance.UpdateNextNotesFlg(XylophoneManager.Notes.None);
                }
                else
                {
                    XylophoneManager.Instance.UpdateNextNotesFlg(sheetMusic.notes[progress].keyBoardName);
                }
            }
            else
            {
                notesKeyboard.ChangeMaterialColorFailure();
                missCount++;
                uIManager.SetMissCountText(missCount);
            }
        }
    }

    /// <summary>
    /// 音名の表示モードを変える
    /// </summary>
    /// <param name="mode"></param>
    public void ChangeDisplayNotesKind()
    {
        displayNotesKind = PlayerInformationManager.Instance.GetSettingDisplayNotesKind();
    }

    /// <summary>
    /// カメラを定点カメラにするかを切り替える
    /// </summary>
    /// <param name="flg">true:定点カメラにする , false:しない</param>
    public void ChangeFixedPointCamera(bool flg)
    {
        fixedPointCameraFlg = flg;
        FixedPointCameraPriority(fixedPointCameraFlg);
    }

    /// <summary>
    /// 定点カメラと追跡カメラを切り替える
    /// </summary>
    public void SwitchFixedPointCamera()
    {
        fixedPointCameraFlg = !fixedPointCameraFlg;
        FixedPointCameraPriority(fixedPointCameraFlg);
    }

    /// <summary>
    /// 定点カメラのPriorityを切り替える
    /// </summary>
    /// <param name="flg">true:定点カメラにする , false:しない</param>
    private void FixedPointCameraPriority(bool flg)
    {
        if (flg)
        {
            fixedPointCamera.Priority = 15;
            playerArmatureTransform.gameObject.GetComponent<StarterAssets.ThirdPersonController>().LockCameraPosition = true;
        }
        else
        {
            playerArmatureTransform.gameObject.GetComponent<StarterAssets.ThirdPersonController>().LockCameraPosition = false;
            fixedPointCamera.Priority = 5;
        }
    }

    /// <summary>
    /// ゲームオーバーを受信する
    /// </summary>
    public void ReceptionGameOver()
    {
        if (myGameInformation.mode == PlayerInformationManager.GameMode.None)
        {
            PlayerFallforFreeStyleMode();
        }
        else if (myGameInformation.mode == PlayerInformationManager.GameMode.FreeStyle)
        {
            PlayerFallforFreeStyleMode();
        }
        else
        {
            ChangeNowPhase(NowPhase.GameOver, 0);
        }
    }

    /// <summary>
    /// フリースタイルモードでプレイヤー落下時の処理
    /// </summary>
    private void PlayerFallforFreeStyleMode()
    {
        //playerArmatureTransform.localPosition = new Vector3(0, 10, 0);

        ChangeNowPhase(NowPhase.GameOver, 0);



    }

    /// <summary>
    /// プレイヤーの操作をできるようにするかどうかを切り替える
    /// </summary>
    /// <param name="flg">true : 操作を受け付ける , false : 操作を受け付けない</param>
    public void SwitchOperatePlayerEnable(bool flg)
    {
        playerArmatureTransform.gameObject.GetComponent<UnityEngine.InputSystem.PlayerInput>().enabled = flg;
    }

    /// <summary>
    /// プレイヤーが全ての動きをできるようにするかどうかを切り替える
    /// </summary>
    /// <param name="flg">true : 動ける , false : 動けない</param>
    public void SwitchPlayerControllerEnable(bool flg)
    {
        playerArmatureTransform.gameObject.GetComponent<StarterAssets.ThirdPersonController>().enabled = flg;
    }

    /// <summary>
    /// お手本のコルーチン
    /// </summary>
    /// <returns></returns>
    private IEnumerator ExampleShowCoroutine()
    {
        float waitTime = 2f;

        uIManager.SetTestText($"{sheetMusic.title}\n(お手本)", 2);

        FixedPointCameraPriority(true);
        yield return new WaitForSeconds(waitTime);

        foreach (var item in sheetMusic.notes)
        {
            float timeSecond = sheetMusic.hyoushiBunbo * 60f / sheetMusic.tempo / (item.bu * sheetMusic.hyoushiBunbo / 4f);

            RingKeyboardNyGameManager(item.keyBoardName);
            yield return new WaitForSeconds(timeSecond);
        }

        FixedPointCameraPriority(false);
        yield return new WaitForSeconds(waitTime);

        ChangeNowPhase(NowPhase.StandBy, 0.5f);
    }

    /// <summary>
    /// ゲームスタート前のカウントダウンのコルーチン
    /// </summary>
    /// <returns></returns>
    private IEnumerator StandByCountDownCoroutine()
    {
        uIManager.SetTestText(sheetMusic.title, 2);

        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.GameCountDounReady);
        countDownValue = 3;
        uIManager.SetCenterText($"{countDownValue}...");
        SpawnEnemies();
        yield return new WaitForSeconds(1);

        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.GameCountDounReady);
        countDownValue--;
        uIManager.SetCenterText($"{countDownValue}...");
        XylophoneManager.Instance.UpdateNextNotesFlg(sheetMusic.notes[progress].keyBoardName);
        yield return new WaitForSeconds(1);

        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.GameCountDounReady);
        countDownValue--;
        uIManager.SetCenterText($"{countDownValue}...");
        yield return new WaitForSeconds(1);

        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.GameCountDounStart);
        countDownValue--;
        uIManager.SetCenterText("START !!");

        SwitchOperatePlayerEnable(true);
        ChangeNowPhase(NowPhase.Play, 0f);

        SoundManager.Instance.PlayStageMusic();
    }

    /// <summary>
    /// ゲームオーバー演出のコルーチン
    /// </summary>
    /// <param name="retryFlg">プレイヤーがリトライボタンを選択したときはtrue</param>
    /// <returns></returns>
    private IEnumerator GameOverCoroutine(bool retryFlg)
    {
        SwitchOperatePlayerEnable(false);

        if (!retryFlg)
        {
            SoundManager.Instance.PlaySE(SoundManager.SoundSeType.GameOver);
            uIManager.SetCenterText("GAME OVER");
            playerTopCamera.Priority = 100;

            yield return new WaitForSeconds(0.5f);
        }

        if (PlayerInformationManager.Instance != null)
        {
            var info = PlayerInformationManager.Instance.GetNextPlayGameInformation();
            if (!info.skipForExampleFlg)
            {
                info.skipForExampleFlg = true;
                PlayerInformationManager.Instance.SetNextPlayGameInformation(info);
            }
        }
        else
        {
            Debug.LogError($"GameSceneManager.GameOverCoroutine : PlayerInformationManager.Instance is Null ! ");
        }

        yield return new WaitForSeconds(0.5f);

        FadeManager.Instance.LoadScene("Game", 0.3f);

    }

    /// <summary>
    /// リトライを受信したとき
    /// </summary>
    public void ReceptionRetry()
    {
        StartCoroutine(GameOverCoroutine(true));
    }

    /// <summary>
    /// メニューシーンへ移行を受信したとき
    /// </summary>
    public void ReceptionGoToMenuScene()
    {
        if (AdMobManager.Instance.GetNoAdFlg())
        {
            ReceptionAdMobManagerForInterstitial(false);
        }
        else
        {
            AdMobManager.Instance.ShowInterstitialAd();
        }
    }

    /// <summary>
    /// 広告を表示するかの旨を受け取る
    /// </summary>
    /// <param name="adFlg">広告表示するならTrue , 表示しないor表示が終わったならFalse</param>
    public void ReceptionAdMobManagerForInterstitial(bool adFlg)
    {
        Debug.Log($"GameSceneManager.ReceptionAdMobManagerForInterstitial : AdFlg = {adFlg}");

        if (adFlg)
        {

        }
        else
        {
            FadeManager.Instance.LoadScene("Menu", 0.3f);
        }
    }

    /// <summary>
    /// ゲームクリア演出のコルーチン
    /// </summary>
    /// <returns></returns>
    private IEnumerator GameClearCoroutine()
    {
        SwitchOperatePlayerEnable(false);

        uIManager.SetCenterText("GAME CLEAR");

        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.GameClear);
        DisplayKamifubuki(true);

        yield return new WaitForSeconds(1);

        SoundManager.Instance.StopStageMusic();
        SoundManager.Instance.PlaySuccessMusic();

        uIManager.DisplayResultUI();
    }

    /// <summary>
    /// 現在のフェーズを変更する
    /// </summary>
    /// <param name="phase"></param>
    /// <param name="waitTime"></param>
    private void ChangeNowPhase(NowPhase phase, float waitTime)
    {
        if (nowPhase == NowPhase.GameClear)
        {
            Debug.LogError($"GameSceneManager.ChangeNowPhase : Already GameClear");
            return;
        }

        if (phase == NowPhase.Start)
        {
            nowPhase = phase;
        }
        else if (phase == NowPhase.ForExample && myGameInformation.skipForExampleFlg)
        {
            ChangeNowPhase(NowPhase.StandBy, waitTime);
        }
        else
        {
            StartCoroutine(ChangeNowPhaseCoroutine(phase, waitTime));
        }
    }

    /// <summary>
    /// フェーズ変更のコルーチン
    /// </summary>
    /// <param name="phase"></param>
    /// <param name="waitTime"></param>
    /// <returns></returns>
    private IEnumerator ChangeNowPhaseCoroutine(NowPhase phase, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        nowPhase = phase;

        if (phase == NowPhase.ForExample)
        {
            forExampleCoroutine = StartCoroutine(ExampleShowCoroutine());
        }
        else if (phase == NowPhase.HowToPlay)
        {
            InstantiateHowToPlayPrefab();
        }
        else if (phase == NowPhase.StandBy)
        {
            StartCoroutine(StandByCountDownCoroutine());
        }
        else if (phase == NowPhase.GameOver)
        {
            StartCoroutine(GameOverCoroutine(false));
        }
        else if (phase == NowPhase.GameClear)
        {
            StartCoroutine(GameClearCoroutine());
        }

        Debug.Log($"GameSceneManager.ChangeNowPhaseCoroutine : NowPhase = {nowPhase}");
    }

    /// <summary>
    /// 鍵盤をゲームマネージャーから鳴らす
    /// </summary>
    /// <param name="key"></param>
    private void RingKeyboardNyGameManager(XylophoneManager.Notes key)
    {
        XylophoneManager.Instance.PlayNotes(key);
        XylophoneManager.Instance.ChangeColorNotesKeyboad(key, COLOR_EXAMPLE);
    }

    /// <summary>
    /// 進捗に合わせてネクストとネクネクのテキストを更新する
    /// </summary>
    private void UpdateNextsText()
    {
        if (sheetMusic.notesNum == progress)
        {
            uIManager.SetNext1Text(XylophoneManager.Notes.None);
            uIManager.SetNext2Text(XylophoneManager.Notes.None);
        }
        else if (sheetMusic.notesNum == progress + 1)
        {
            uIManager.SetNext1Text(sheetMusic.notes[progress].keyBoardName);
            uIManager.SetNext2Text(XylophoneManager.Notes.None);
        }
        else
        {
            uIManager.SetNext1Text(sheetMusic.notes[progress].keyBoardName);
            uIManager.SetNext2Text(sheetMusic.notes[progress + 1].keyBoardName);
        }

        uIManager.SetRestNotesText(sheetMusic.notesNum - progress);
    }

    /// <summary>
    /// Skipボタンから受信
    /// </summary>
    public void ReceptionSkipForExample()
    {
        if (nowPhase == NowPhase.ForExample)
        {
            StopCoroutine(forExampleCoroutine);
            FixedPointCameraPriority(false);
            ChangeNowPhase(NowPhase.StandBy, 0.5f);
        }
    }

    /// <summary>
    /// 遊び方のPrefabから画面が閉じたことを受信する
    /// </summary>
    public void ReceptionHowToPlayPrefab()
    {
        if (nowPhase == NowPhase.HowToPlay) ChangeNowPhase(NowPhase.ForExample, 0.5f);
    }

    /// <summary>
    /// 遊び方Prefabを生成する
    /// </summary>
    public void InstantiateHowToPlayPrefab()
    {
        Instantiate(howToPlayPrefab, canvasTransform);
    }

    /// <summary>
    /// PlayFabManagerから長時間バックグラウンドにいたことを受信したとき
    /// </summary>
    public void ReceptionLongTimeBackGroundFromPlayFabManager()
    {
        ChangePauseFlg(true);
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.No);
        uIManager.InstantiateLongTimeBackGroundPrefab();
    }

    /// <summary>
    /// 現在のフェーズを返すゲッター
    /// </summary>
    /// <returns></returns>
    public NowPhase GetNowPhase()
    {
        return nowPhase;

    }

    /// <summary>
    /// ポーズにする
    /// </summary>
    /// <param name="flg"></param>
    public void ChangePauseFlg(bool flg)
    {
        pauseFlg = flg;
        SwitchPlayerControllerEnable(!flg);
        SwitchOperatePlayerEnable(!flg);
    }

    /// <summary>
    /// 現在のプレイタイムを返すゲッター
    /// </summary>
    /// <returns></returns>
    public float GetPlayTime()
    {
        return playTimer;
    }

    /// <summary>
    /// 現在のミスカウントを返すゲッター
    /// </summary>
    /// <returns></returns>
    public int GetMissCount()
    {
        return missCount;
    }

    /// <summary>
    /// ポーズのフラグを取得するゲッター
    /// </summary>
    /// <returns></returns>
    public bool GetPauseFlg()
    {
        return pauseFlg;
    }

    /// <summary>
    /// myGameInformationを返すゲッター
    /// </summary>
    /// <returns></returns>
    public PlayerInformationManager.PlayGameInformation GetGameInformation()
    {
        return myGameInformation;
    }

    /// <summary>
    /// ゲームスタート前のカウントダウンの値を返す
    /// </summary>
    /// <returns></returns>
    public int GetCountDownValue()
    {
        return countDownValue;
    }

    /// <summary>
    /// 紙吹雪演出の表示を切り替える
    /// </summary>
    /// <param name="flg"></param>
    private void DisplayKamifubuki(bool flg)
    {
        if (flg)
        {
            kamifubuki.Play();
        }
        else
        {
            kamifubuki.Stop();
        }
    }

    /// <summary>
    /// CanvasのTransformを返すゲッター
    /// </summary>
    /// <returns></returns>
    public Transform GetCanvasTransform()
    {
        return canvasTransform;
    }

    /// <summary>
    /// 文字列の改行を消す
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    private string CutEnter(string str)
    {
        return str.Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace("\v", "").Replace("\0", "").Replace(" ", "").Trim();
    }

}
