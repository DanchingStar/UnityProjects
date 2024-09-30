using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameModeManager : MonoBehaviour
{
    [SerializeField] private bool debugTehaiOpenGame;

    [SerializeField] private TimeAttackContents timeAttackContents;
    [SerializeField] private PuzzleContents puzzleContents;

    [SerializeField] private CpuContentGenerator cpuContentGenerator;

    private CpuParent[] cpuThisGame;
    private bool[] cpuNakiActiveFlg;

    private GameMode gameMode;

    private StatusForTimeAttack statusForTimeAttack;
    private StatusForPuzzle statusForPuzzle;
    private StatusForFree statusForFree;
    private StatusForDebug statusForDebug;


    private float scoreTimer;
    private int scoreClearCount;

    private bool timerMovingFlg;

    private bool startFinifhFlg = false;

    public enum GameMode
    {
        None,
        TimeAttack,
        Puzzle,
        Free,
        Debug,
    }

    public class StatusForTimeAttack
    {
        public bool supportFlg;
        public int needClearCount;
    }

    public class StatusForPuzzle
    {
        public int stageNumber;

        public MahjongManager.PlayerKind oya;
        public CpuParent[] cpu;
        public bool[] nakiActiveFlg;
        public List<MahjongManager.PaiKinds> yama;
    }

    public class StatusForFree
    {
        public bool supportFlg;
        public bool tehaiOpenFlg;
    }

    public class StatusForDebug
    {
        public int haipai;
        public int yama;
        public int rinshan;
    }

    public static GameModeManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーン遷移しても破棄されないようにする
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (startFinifhFlg) return;
        startFinifhFlg = true;

        ResetValues();
        ResetCpu();
    }

    private void Update()
    {
        if(gameMode == GameMode.TimeAttack && timerMovingFlg)
        {
            scoreTimer += Time.deltaTime;
        }
    }

    /// <summary>
    /// 値の初期化
    /// </summary>
    private void ResetValues()
    {
        Start();

        gameMode = GameMode.None;
        statusForTimeAttack = new StatusForTimeAttack();
        statusForPuzzle = new StatusForPuzzle();
        statusForFree = new StatusForFree();
        statusForDebug = new StatusForDebug();

        timerMovingFlg = false;

        ResetScore();
    }

    /// <summary>
    /// スコアのリセット
    /// </summary>
    private void ResetScore()
    {
        scoreTimer = 0f;
        scoreClearCount = 0;
    }

    /// <summary>
    /// CPUの設定をリセット
    /// </summary>
    private void ResetCpu()
    {
        cpuThisGame = new CpuParent[3];
        cpuThisGame[0] = cpuContentGenerator.GetCpuChild(0);
        cpuThisGame[1] = cpuContentGenerator.GetCpuChild(0);
        cpuThisGame[2] = cpuContentGenerator.GetCpuChild(0);
        cpuNakiActiveFlg = new bool[3];
        cpuNakiActiveFlg[0] = false;
        cpuNakiActiveFlg[1] = false;
        cpuNakiActiveFlg[2] = false;
    }

    /// <summary>
    /// CPUの打ち方を設定する
    /// </summary>
    private void SetCpu()
    {
        if(GetGameMode() == GameMode.Puzzle)
        {
            cpuThisGame[0] = statusForPuzzle.cpu[0];
            cpuThisGame[1] = statusForPuzzle.cpu[1];
            cpuThisGame[2] = statusForPuzzle.cpu[2];
            cpuNakiActiveFlg[0] = statusForPuzzle.nakiActiveFlg[0];
            cpuNakiActiveFlg[1] = statusForPuzzle.nakiActiveFlg[1];
            cpuNakiActiveFlg[2] = statusForPuzzle.nakiActiveFlg[2];
        }
        else
        {
            // ToDo 適当に設定
            cpuThisGame[0] = cpuContentGenerator.GetCpuChild(5);
            cpuThisGame[1] = cpuContentGenerator.GetCpuChild(5);
            cpuThisGame[2] = cpuContentGenerator.GetCpuChild(5);
            //cpuThisGame[0] = cpuContentGenerator.GetCpuChild(1);
            //cpuThisGame[1] = cpuContentGenerator.GetCpuChild(1);
            //cpuThisGame[2] = cpuContentGenerator.GetCpuChild(1);
            cpuNakiActiveFlg[0] = true;
            cpuNakiActiveFlg[1] = true;
            cpuNakiActiveFlg[2] = true;
        }
    }

    /// <summary>
    /// ゲームモードのセッター
    /// </summary>
    /// <param name="_gameMode"></param>
    private void SetGameMode(GameMode _gameMode)
    {
        gameMode = _gameMode;
    }

    /// <summary>
    /// ゲームモードをタイムアタックにして設定する
    /// </summary>
    /// <param name="_supportFlg"></param>
    /// <param name="_clearCount"></param>
    public void SetGameModeTimeAttack(bool _supportFlg, int _clearCount)
    {
        ResetValues();
        SetGameMode(GameMode.TimeAttack);
        statusForTimeAttack.supportFlg = _supportFlg;
        statusForTimeAttack.needClearCount = _clearCount;
    }

    /// <summary>
    /// ゲームモードをパズルにして設定する
    /// </summary>
    /// <param name="_stageNumber"></param>
    public bool SetGameModePuzzle(int _stageNumber)
    {
        ResetValues();
        SetGameMode(GameMode.Puzzle);
        statusForPuzzle.stageNumber = _stageNumber;

        statusForPuzzle.cpu = new CpuParent[3];
        statusForPuzzle.nakiActiveFlg = new bool[3];
        statusForPuzzle.yama = new List<MahjongManager.PaiKinds>();

        return LoadPuzzle();
    }

    /// <summary>
    /// ゲームモードをフリーにして設定する
    /// </summary>
    /// <param name="_supportFlg"></param>
    public void SetGameModeFree(bool _supportFlg ,bool _tehaiOpenFlg)
    {
        ResetValues();
        SetGameMode(GameMode.Free);
        statusForFree.supportFlg = _supportFlg;
        statusForFree.tehaiOpenFlg = _tehaiOpenFlg;
    }

    /// <summary>
    /// ゲームモードをデバッグにして設定する
    /// </summary>
    /// <param name="_haipai"></param>
    /// <param name="_yama"></param>
    /// <param name="_rinshan"></param>
    public void SetGameModeDebug(int _haipai, int _yama, int _rinshan)
    {
        ResetValues();
        SetGameMode(GameMode.Debug);
        statusForDebug.haipai = _haipai;
        statusForDebug.yama = _yama;
        statusForDebug.rinshan = _rinshan;
    }

    /// <summary>
    /// パズルモードの内容をテキストから読み込む
    /// </summary>
    public bool LoadPuzzle()
    {
        //Debug.Log($"LoadPuzzle : Called , Now Scene = {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");

        TextAsset textfile = puzzleContents.GetPuzzleStageGenerator().GetStageTextFile(statusForPuzzle.stageNumber);

        StringReader stackLevelCostReader = new StringReader(textfile.text);
        string text = stackLevelCostReader.ReadToEnd();

        string[] allText = text.Split(';');

        bool errorFlg = false;
        int i = 0;
        foreach (var item in allText)
        {
            // 改行をカットする
            string itemStr = CutEnter(item);

            if (errorFlg)
            {
                break;
            }
            else if (string.IsNullOrEmpty(itemStr))
            {
                // 何もしない
            }
            else if (itemStr.StartsWith("#"))
            {
                // 何もしない
            }
            else
            {
                if (i == 0) // 親
                {
                    if (Enum.TryParse(itemStr, out MahjongManager.PlayerKind oyaKouho))
                    {
                        if (MahjongManager.PlayerKind.Player <= oyaKouho && oyaKouho <= MahjongManager.PlayerKind.Kamicha)
                        {
                            statusForPuzzle.oya = oyaKouho;
                        }
                        else
                        {
                            Debug.LogError($"LoadPuzzle : Error , i = {i}");
                        }
                    }
                }
                else if (i == 1) // CPUの名前
                {
                    string[] str = itemStr.Split(',');

                    int counter = 0;
                    foreach (var value in str)
                    {
                        if (string.IsNullOrEmpty(value))
                        {
                            continue;
                        }
                        else
                        {
                            bool errorFlgFor1 = true;
                            for (int j = 0; j < cpuContentGenerator.GetLength(); j++)
                            {
                                CpuParent cpuChild = cpuContentGenerator.GetCpuChild(j);
                                if (cpuChild.GetName() == value)
                                {
                                    statusForPuzzle.cpu[counter] = cpuChild;
                                    errorFlgFor1 = false;
                                    break;
                                }
                            }

                            if (errorFlgFor1)
                            {
                                Debug.LogError($"LoadPuzzle : Error , i = {i}\ncounter = {counter} , value = {value}");
                                errorFlg = true;
                                break;
                            }
                            else
                            {
                                counter++;
                            }
                        }
                    }
                    if (counter != 3)
                    {
                        Debug.LogError($"LoadPuzzle : Error , i = {i}\ncounter = {counter}");
                        errorFlg = true;
                        break;
                    }
                }
                else if (i == 2) // CPUが鳴くか
                {
                    string[] str = itemStr.Split(',');

                    int counter = 0;
                    foreach (var value in str)
                    {
                        if (string.IsNullOrEmpty(value))
                        {
                            continue;
                        }
                        else
                        {
                            bool result;
                            if (Boolean.TryParse(value, out result))
                            {
                                statusForPuzzle.nakiActiveFlg[counter] = result;
                                counter++;
                            }
                            else
                            {
                                Debug.LogError($"LoadPuzzle : Error , i = {i}\value = {value}");
                            }
                        }
                    }

                    if(counter != 3)
                    {
                        Debug.LogError($"LoadPuzzle : Error , i = {i}\ncounter = {counter}");
                    }
                }
                else if(i == 3) // 山
                {
                    //if (itemStr == "END") break;

                    List<MahjongManager.PaiKinds> loadList = new List<MahjongManager.PaiKinds>();

                    string[] str = itemStr.Split(',');

                    foreach (var value in str)
                    {
                        if (Enum.TryParse(value, out MahjongManager.PaiKinds auth))
                        {
                            if(auth == MahjongManager.PaiKinds.None_00)
                            {
                                Debug.LogError($"LoadPuzzle : Load PaiKind None_00");
                                errorFlg = true;
                                break;
                            }
                            else
                            {
                                loadList.Add(auth);
                            }
                        }
                    }

                    if (errorFlg)
                    {
                        break;
                    }
                    else if (loadList.Count == 136)
                    {
                        statusForPuzzle.yama = new List<MahjongManager.PaiKinds>(loadList);
                    }
                    else
                    {
                        Debug.LogError($"LoadPuzzle : Error , i = {i}\nloadList.Count = {loadList.Count}");
                        errorFlg = true;
                        break;
                    }
                }
                else
                {
                    break;
                }
                i++;
            }
        }

        if (errorFlg)
        {
            Debug.LogError($"LoadPuzzle : Load is Canceled");
        }
        else
        {
            Debug.Log($"LoadPuzzle : Load Success !\nOya = {statusForPuzzle.oya}\n" +
                $"CPU[0] = {statusForPuzzle.cpu[0]} , CPU[1] = {statusForPuzzle.cpu[1]} , CPU[2] = {statusForPuzzle.cpu[2]}\n" +
                $"Naki[0] = {statusForPuzzle.nakiActiveFlg[0]} , Naki[1] = {statusForPuzzle.nakiActiveFlg[1]} , Naki[2] = {statusForPuzzle.nakiActiveFlg[2]}\n" +
                $"Yama = {string.Join(", ", statusForPuzzle.yama)}");
        }

        return !errorFlg;
    }

    /// <summary>
    /// ゲームモードのゲッター
    /// </summary>
    /// <returns></returns>
    public GameMode GetGameMode()
    {
        return gameMode;
    }

    /// <summary>
    /// タイムアタックのステータスのゲッター
    /// </summary>
    /// <returns></returns>
    public StatusForTimeAttack GetStatusForTimeAttack()
    {
        return statusForTimeAttack;
    }

    /// <summary>
    /// パズルのステータスのゲッター
    /// </summary>
    /// <returns></returns>
    public StatusForPuzzle GetStatusForPuzzle()
    {
        return statusForPuzzle;
    }

    /// <summary>
    /// フリーのステータスのゲッター
    /// </summary>
    /// <returns></returns>
    public StatusForFree GetStatusForFree()
    {
        return statusForFree;
    }

    /// <summary>
    /// デバッグのステータスのゲッター
    /// </summary>
    /// <returns></returns>
    public StatusForDebug GetStatusForDebug()
    {
        return statusForDebug;
    }

    /// <summary>
    /// 配牌サポートのリストのゲッター
    /// </summary>
    /// <param name="_supportSetForTimeAttack"></param>
    /// <returns></returns>
    public List<MahjongManager.PaiKinds> GetSupportList(MahjongManager.SupportSetForTimeAttack _supportSetForTimeAttack)
    {
        List<MahjongManager.PaiKinds> resultList = new List<MahjongManager.PaiKinds>();

        resultList = timeAttackContents.GetSupportList(_supportSetForTimeAttack);

        return resultList;
    }

    /// <summary>
    /// CPUの思考するクラスのゲッター
    /// </summary>
    /// <param name="_playerKind"></param>
    /// <returns></returns>
    public CpuParent GetCpu(MahjongManager.PlayerKind _playerKind)
    {
        if(_playerKind <= MahjongManager.PlayerKind.Player)
        {
            return null;
        }
        else
        {
            return cpuThisGame[(int)_playerKind - 2];
        }
    }

    /// <summary>
    /// CpuContentGeneratorを返すゲッター
    /// </summary>
    /// <returns></returns>
    public CpuContentGenerator GetCpuContentGenerator()
    {
        return cpuContentGenerator;
    }

    /// <summary>
    /// CPUの鳴きを有効にしているかのゲッター
    /// </summary>
    /// <param name="_playerKind"></param>
    /// <returns></returns>
    public bool GetCpuNakiActive(MahjongManager.PlayerKind _playerKind)
    {
        if (_playerKind <= MahjongManager.PlayerKind.Player)
        {
            return false;
        }
        else
        {
            return cpuNakiActiveFlg[(int)_playerKind - 2];
        }
    }

    /// <summary>
    /// タイムアタックモードの時の時間を返すゲッター
    /// </summary>
    /// <returns></returns>
    public float GetTimeAttackScoreTimer()
    {
        return scoreTimer;
    }

    /// <summary>
    /// 手牌を開いて行うゲームであるかを返すゲッター
    /// </summary>
    /// <returns></returns>
    public bool GetTehaiOpenGame()
    {
        if (debugTehaiOpenGame)
        {
            return true;
        }
        else if (gameMode == GameMode.Free)
        {
            if (statusForFree.tehaiOpenFlg == true)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// PuzzleStageGeneratorを返すゲッター
    /// </summary>
    /// <returns></returns>
    public PuzzleStageGenerator GetPuzzleStageGenerator()
    {
        return puzzleContents.GetPuzzleStageGenerator();
    }

    /// <summary>
    /// ゲームスタートを受信したときの動作
    /// </summary>
    public void ReceptionGameStart()
    {
        SetCpu();

        if(gameMode == GameMode.TimeAttack)
        {
            ResetScore();
            ReceptionGameContinueOrPause(true);
        }
    }

    /// <summary>
    /// ゲームをポーズした(または再開した)ことを受信したときの動作
    /// </summary>
    /// <param name="_continueFlg">true : 再開する , false : とめる</param>
    public void ReceptionGameContinueOrPause(bool _continueFlg)
    {
        if (gameMode == GameMode.TimeAttack)
        {
            timerMovingFlg = _continueFlg;
        }
    }

    /// <summary>
    /// 役満の和了1回分を受信したときの動作
    /// </summary>
    public void ReceptionAgariYakumanOneTime()
    {
        if (gameMode == GameMode.TimeAttack)
        {
            scoreClearCount++;
            if (statusForTimeAttack.needClearCount <= scoreClearCount)
            {
                // クリア
            }

        }
        else if (gameMode == GameMode.Puzzle)
        {
            // クリア
        }


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
