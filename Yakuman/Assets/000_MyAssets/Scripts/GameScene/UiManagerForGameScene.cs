using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiManagerForGameScene : MonoBehaviour
{
    [SerializeField] private GameObject uiForBeforeGame;
    [SerializeField] private GameObject uiForPlayingGame;

    [SerializeField] private Button gameStartButton;

    [SerializeField] private TextMeshProUGUI gameModeText;
    [SerializeField] private TextMeshProUGUI gameModeInformationText;

    [SerializeField] private TextMeshProUGUI mainInformationMainText;
    [SerializeField] private TextMeshProUGUI kyokuText;
    [SerializeField] private TextMeshProUGUI honbaText;
    [SerializeField] private TextMeshProUGUI nokoriCountText;
    [SerializeField] private GameObject uiNakiPrefab;
    [SerializeField] private GameObject uiTurnActionPrefab;
    [SerializeField] private GameObject uiKyokuFinishPanelPrefab;
    [SerializeField] private GameObject uiNakiVoicePrefab;

    [SerializeField] private Transform nakiVoiceParentTf;

    private UiTurnActionPrefab activeUiTurnActionPrefab = null;

    private GameModeManager.GameMode gameMode;

    private void Start()
    {

    }

    private void Update()
    {
        if (gameMode == GameModeManager.GameMode.TimeAttack)
        {
            ChangeMainInformationMainText();
        }
    }

    /// <summary>
    /// UIの値や表示をリセットする
    /// </summary>
    public void ResetUi()
    {
        ChangeUiForPlayingGameSetActive(true);
        ReceptionPlayerSute();
        ChangeMenuArea();
        ChangeKyokuText("");
    }

    /// <summary>
    /// ゲーム中のUIを表示/非表示を変える
    /// </summary>
    /// <param name="_flg"></param>
    private void ChangeUiForPlayingGameSetActive(bool _flg)
    {
        uiForPlayingGame.SetActive(_flg);
        uiForBeforeGame.SetActive(!_flg);
    }

    /// <summary>
    /// ゲーム開始前のUIを表示する
    /// </summary>
    public void DisplayBeforeGame()
    {
        ChangeGameModeText(GameModeManager.Instance.GetGameMode());
        ChangeGameModeInformationText(GameModeManager.Instance.GetGameMode());

        ChangeUiForPlayingGameSetActive(false);
    }

    /// <summary>
    /// ゲームモードのテキストを変更する
    /// </summary>
    /// <param name="_str"></param>
    private void ChangeGameModeText(string _str)
    {
        gameModeText.text = _str;
    }

    /// <summary>
    /// ゲームモードのテキストを変更する
    /// </summary>
    /// <param name="_gameMode"></param>
    private void ChangeGameModeText(GameModeManager.GameMode _gameMode)
    {
        ChangeGameModeText(_gameMode.ToString());
    }

    /// <summary>
    /// メニューエリアのUIを変える
    /// </summary>
    private void ChangeMenuArea()
    {
        gameMode = GameModeManager.Instance.GetGameMode();
        ChangeMainInformationMainText();
    }

    /// <summary>
    /// メニューエリアのメインテキストを変える
    /// </summary>
    private void ChangeMainInformationMainText()
    {
        if (gameMode == GameModeManager.GameMode.TimeAttack)
        {
            float timeAttackTimer = GameModeManager.Instance.GetTimeAttackScoreTimer();
            System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(timeAttackTimer);
            //string formattedTime = string.Format("{0:D2}:{1:D2}.{2:D2}", timeSpan.Minutes, timeSpan.Seconds, (int)(timeSpan.Milliseconds / 10)); //少数点以下2ケタ
            string formattedTime = string.Format("{0:D2}:{1:D2}.{2}", timeSpan.Minutes, timeSpan.Seconds, (int)(timeSpan.Milliseconds / 100)); //少数点以下1ケタ
            mainInformationMainText.text = $"{formattedTime}";
        }
        else if (gameMode == GameModeManager.GameMode.Puzzle)
        {
            mainInformationMainText.text = $"ステージ {GameModeManager.Instance.GetStatusForPuzzle().stageNumber}";
        }
        else if (gameMode == GameModeManager.GameMode.Free)
        {
            mainInformationMainText.text = $"フリーモード";
        }
        else if (gameMode == GameModeManager.GameMode.Debug)
        {
            mainInformationMainText.text = $"デバッグ";
        }
        else
        {
            mainInformationMainText.text = $"? ? ?";
        }
    }

    /// <summary>
    /// ゲームモードの詳細情報のテキストを変更する
    /// </summary>
    /// <param name="_str"></param>
    private void ChangeGameModeInformationText(string _str)
    {
        gameModeInformationText.text = _str;
    }

    /// <summary>
    /// ゲームモードの詳細情報のテキストを変更する
    /// </summary>
    /// <param name="_str"></param>
    private void ChangeGameModeInformationText(GameModeManager.GameMode _gameMode)
    {
        string str = "";

        switch (_gameMode)
        {
            case GameModeManager.GameMode.TimeAttack:
                {
                    var status = GameModeManager.Instance.GetStatusForTimeAttack();
                    str += $"配牌サポート : ";
                    str += status.supportFlg ? "アリ\n" : "ナシ\n";
                    str += $"クリア必要回数 : {status.needClearCount}回";
                }
                break;
            case GameModeManager.GameMode.Puzzle:
                {
                    var status = GameModeManager.Instance.GetStatusForPuzzle();
                    str += $"ステージ : {status.stageNumber}";
                }
                break;
            case GameModeManager.GameMode.Free:
                {
                    var status = GameModeManager.Instance.GetStatusForFree();
                    str += $"配牌サポート : ";
                    str += status.supportFlg ? "アリ\n" : "ナシ\n";
                }
                break;
            case GameModeManager.GameMode.Debug:
                {
                    var status = GameModeManager.Instance.GetStatusForDebug();
                    str += $"haipai : No.{status.haipai}\nyama : No.{status.yama}\nrinshan : No.{status.rinshan}";
                }
                break;
        }

        ChangeGameModeInformationText(str);
    }

    /// <summary>
    /// 局のテキストを変更する
    /// </summary>
    /// <param name="_str"></param>
    public void ChangeKyokuText(string _str)
    {
        kyokuText.text = _str;
    }

    /// <summary>
    /// 局のテキストを変更する
    /// </summary>
    /// <param name="_str"></param>
    public void ChangeKyokuText(MahjongManager.Kyoku _kyoku, int _honba)
    {
        string str1 = "";
        string str2 = "";

        switch (_kyoku)
        {
            case MahjongManager.Kyoku.Ton1:
                {
                    str1 = "東1局";
                }
                break;
            case MahjongManager.Kyoku.Ton2:
                {
                    str1 = "東2局";
                }
                break;
            case MahjongManager.Kyoku.Ton3:
                {
                    str1 = "東3局";
                }
                break;
            case MahjongManager.Kyoku.Ton4:
                {
                    str1 = "東4局";
                }
                break;
            case MahjongManager.Kyoku.Nan1:
                {
                    str1 = "南1局";
                }
                break;
            case MahjongManager.Kyoku.Nan2:
                {
                    str1 = "南2局";
                }
                break;
            case MahjongManager.Kyoku.Nan3:
                {
                    str1 = "南3局";
                }
                break;
            case MahjongManager.Kyoku.Nan4:
                {
                    str1 = "南4局";
                }
                break;
            default:
                {

                }
                break;
        }

        str2 = $"{_honba}";

        kyokuText.text = str1;
        honbaText.text = str2;
    }

    /// <summary>
    /// 残り何枚あるかのテキストを変更する
    /// </summary>
    /// <param name="_nokoriCount"></param>
    public void ChangeNokoriText(int _nokoriCount)
    {
        nokoriCountText.text = _nokoriCount.ToString();
    }

    /// <summary>
    /// 鳴きを選択するUIを生成する
    /// </summary>
    /// <returns></returns>
    private UiNakiPrefab InstatinateUiNakiPrefab()
    {
        UiNakiPrefab _prefab = Instantiate(uiNakiPrefab, transform).GetComponent<UiNakiPrefab>();

        return _prefab;
    }

    /// <summary>
    /// 鳴いたときに表示する発声のUIを生成する
    /// </summary>
    /// <returns></returns>
    private UiNakiVoice InstatinateUiNakiVoicePrefab()
    {
        UiNakiVoice _prefab = Instantiate(uiNakiVoicePrefab, nakiVoiceParentTf).GetComponent<UiNakiVoice>();

        return _prefab;
    }

    /// <summary>
    /// 自ターンでアクション可能な時に表示するUIを生成する
    /// </summary>
    /// <returns></returns>
    private UiTurnActionPrefab InstatinateUiTurnActionPrefab()
    {
        UiTurnActionPrefab _prefab = Instantiate(uiTurnActionPrefab, transform).GetComponent<UiTurnActionPrefab>();

        return _prefab;
    }

    /// <summary>
    /// 局が終わったときに表示するUIを生成する
    /// </summary>
    /// <returns></returns>
    private UiKyokuFinishPanelPrefab InstatinateUiKyokuFinishPanelPrefab()
    {
        UiKyokuFinishPanelPrefab _prefab = Instantiate(uiKyokuFinishPanelPrefab, transform).GetComponent<UiKyokuFinishPanelPrefab>();

        return _prefab;
    }

    /// <summary>
    /// 鳴きを選択するUIを表示する
    /// </summary>
    /// <param name="_chiLow"></param>
    /// <param name="_chiMid"></param>
    /// <param name="_chiHigh"></param>
    /// <param name="_pon"></param>
    /// <param name="_kan"></param>
    /// <param name="_ron"></param>
    /// <param name="_friten"></param>
    /// <param name="_sutePai"></param>
    public void DisplayNaki(bool _chiLow, bool _chiMid, bool _chiHigh, bool _pon, bool _kan, bool _ron, bool _friten, MahjongManager.PaiKinds _sutePai)
    {
        MahjongManager.Instance.ReceptionUiNakiPrefabForChangeNakiWaitFlg(true);
        var prefab = InstatinateUiNakiPrefab();
        prefab.SetMyStatus(_chiLow, _chiMid, _chiHigh, _pon, _kan, _ron, _friten, _sutePai);
    }

    /// <summary>
    /// 鳴いたときに表示する発声のUIを表示する
    /// </summary>
    /// <param name="_nakiKind"></param>
    /// <param name="_playerKind"></param>
    public UiNakiVoice DisplayNakiVoice(MahjongManager.NakiKinds _nakiKind, MahjongManager.PlayerKind _playerKind)
    {
        var prefab = InstatinateUiNakiVoicePrefab();
        prefab.DisplayMe(_nakiKind, _playerKind);
        return prefab;
    }

    /// <summary>
    /// 鳴いたときに表示する発声のUIを破壊する
    /// </summary>
    /// <param name="_uiNakiVoicePrefab"></param>
    public void DestroyNakiVoice(UiNakiVoice _uiNakiVoicePrefab)
    {
        _uiNakiVoicePrefab.DestroyMe();
    }

    /// <summary>
    /// 自ターンでアクション可能な時に表示するUIを表示する
    /// </summary>
    /// <param name="_tsumo"></param>
    /// <param name="_ryuukyoku"></param>
    /// <param name="_reachList"></param>
    /// <param name="_ankanList"></param>
    /// <param name="_kakanList"></param>
    public void DisplayTurnAction(bool _tsumo, bool _ryuukyoku,
        List<MahjongManager.PaiKinds> _reachList, List<MahjongManager.PaiKinds> _ankanList, List<MahjongManager.PaiKinds> _kakanList)
    {
        var prefab = InstatinateUiTurnActionPrefab();
        prefab.SetMyStatus(_tsumo, _ryuukyoku, _reachList, _ankanList, _kakanList);

        activeUiTurnActionPrefab = prefab;

        //string str = "DisplayTurnAction : List And Count\n";
        //str += $"_reachList , count = {_reachList.Count}";
        //foreach (var item in _reachList)
        //{
        //    str += $",[{item}]";
        //}
        //str += "\n";
        //str += $"_ankanList , count = {_ankanList.Count}";
        //foreach (var item in _ankanList)
        //{
        //    str += $",[{item}]";
        //}
        //str += "\n";
        //str += $"_kakanList , count = {_kakanList.Count}";
        //foreach (var item in _kakanList)
        //{
        //    str += $",[{item}]";
        //}
        //Debug.Log(str);
    }

    /// <summary>
    /// MahjongManagerから流局を受信する
    /// </summary>
    /// <param name="_kind"></param>
    public void ReceptionMahjongManagerForRyuukyoku(MahjongManager.RyuukyokuOfTochuu _kind)
    {
        var prefab = InstatinateUiKyokuFinishPanelPrefab();
        prefab.SetMyStatusForRyuukyoku(_kind);
    }

    /// <summary>
    /// MahjongManagerからアガリを受信する
    /// </summary>
    /// <param name="_playerKind">和了ったプレイヤー</param>
    /// <param name="_ronPai">自摸和了ならnull</param>
    public void ReceptionMahjongManagerForAgari(MahjongManager.PlayerKind _playerKind, MahjongManager.PaiStatus _ronPai)
    {
        var prefab = InstatinateUiKyokuFinishPanelPrefab();
        prefab.SetMyStatusForAgari(_playerKind, _ronPai);
    }

    /// <summary>
    /// 自ターンにプレイヤーが捨てたことを受信する
    /// </summary>
    public void ReceptionPlayerSute()
    {
        if(activeUiTurnActionPrefab != null)
        {
            activeUiTurnActionPrefab.ReceptionUiManagerForDestroyMe();
            activeUiTurnActionPrefab = null;
        }
    }

    /// <summary>
    /// 自ターンにプレイヤーがカンをして選択したことを受信する
    /// </summary>
    public void ReceptionPlayerSelectKan()
    {
        if (activeUiTurnActionPrefab != null)
        {
            activeUiTurnActionPrefab.ReceptionUiManagerForDestroyMe();
            activeUiTurnActionPrefab = null;
        }
    }

    /// <summary>
    /// 表示中の自ターンアクションのPrefabを返すゲッター
    /// </summary>
    /// <returns></returns>
    public UiTurnActionPrefab GetActiveUiTurnActionPrefab()
    {
        return activeUiTurnActionPrefab;
    }

    /// <summary>
    /// ゲームスタートボタンを押したとき
    /// </summary>
    public void PushGameStartButton()
    {
        MahjongManager.Instance.GameStart();
        GameModeManager.Instance.ReceptionGameStart();
    }

    /// <summary>
    /// メニューへ戻るボタンを押したとき
    /// </summary>
    public void PushGoToMenuSceneButton()
    {
        MoveMenuScene();
    }

    /// <summary>
    /// メニューシーンへ移動する
    /// </summary>
    private void MoveMenuScene()
    {
        FadeManager.Instance.LoadScene("Menu", 0.5f);
    }

}
