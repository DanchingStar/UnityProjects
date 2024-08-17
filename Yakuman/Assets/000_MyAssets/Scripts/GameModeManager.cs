using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeManager : MonoBehaviour
{
    [SerializeField] private TimeAttackContents timeAttackContents;
    [SerializeField] private PuzzleContents puzzleContents;

    [SerializeField] private CpuParent[] cpuAll;
    private CpuParent[] cpuThisGame;

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
        public List<MahjongManager.PaiKinds> yama;
    }

    public class StatusForFree
    {
        public bool supportFlg;
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

    private void ResetScore()
    {
        scoreTimer = 0f;
        scoreClearCount = 0;
    }

    private void ResetCpu()
    {
        cpuThisGame = new CpuParent[3];
        cpuThisGame[0] = cpuAll[2];
        cpuThisGame[1] = cpuAll[1];
        cpuThisGame[2] = cpuAll[0];
    }

    private void SetGameMode(GameMode _gameMode)
    {
        gameMode = _gameMode;
    }

    public void SetGameModeTimeAttack(bool _supportFlg, int _clearCount)
    {
        ResetValues();
        SetGameMode(GameMode.TimeAttack);
        statusForTimeAttack.supportFlg = _supportFlg;
        statusForTimeAttack.needClearCount = _clearCount;
    }

    public void SetGameModePuzzle(int _stageNumber)
    {
        ResetValues();
        SetGameMode(GameMode.Puzzle);
        statusForPuzzle.stageNumber = _stageNumber;
    }

    public void SetGameModeFree(bool _supportFlg)
    {
        ResetValues();
        SetGameMode(GameMode.Free);
        statusForFree.supportFlg = _supportFlg;
    }

    public void SetGameModeDebug(int _haipai, int _yama, int _rinshan)
    {
        ResetValues();
        SetGameMode(GameMode.Debug);
        statusForDebug.haipai = _haipai;
        statusForDebug.yama = _yama;
        statusForDebug.rinshan = _rinshan;
    }

    public GameMode GetGameMode()
    {
        return gameMode;
    }

    public StatusForTimeAttack GetStatusForTimeAttack()
    {
        return statusForTimeAttack;
    }

    public StatusForPuzzle GetStatusForPuzzle()
    {
        return statusForPuzzle;
    }

    public StatusForFree GetStatusForFree()
    {
        return statusForFree;
    }

    public StatusForDebug GetStatusForDebug()
    {
        return statusForDebug;
    }

    public List<MahjongManager.PaiKinds> GetSupportList(MahjongManager.SupportSetForTimeAttack _supportSetForTimeAttack)
    {
        List<MahjongManager.PaiKinds> resultList = new List<MahjongManager.PaiKinds>();

        resultList = timeAttackContents.GetSupportList(_supportSetForTimeAttack);

        return resultList;
    }

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

    public void ReceptionGameStart()
    {
        if(gameMode == GameMode.TimeAttack)
        {
            ResetScore();
        }
    }

    public void ReceptionGameContinueOrPause(bool _continueFlg)
    {
        if (gameMode == GameMode.TimeAttack)
        {
            timerMovingFlg = _continueFlg;
        }
    }

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

}
