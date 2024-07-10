using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI kyokuText;
    [SerializeField] private TextMeshProUGUI honbaText;
    [SerializeField] private TextMeshProUGUI nokoriCountText;
    [SerializeField] private GameObject uiNakiPrefab;
    [SerializeField] private GameObject uiTurnActionPrefab;
    [SerializeField] private GameObject uiKyokuFinishPanelPrefab;

    private UiTurnActionPrefab activeUiTurnActionPrefab = null;

    /// <summary>
    /// UIの値や表示をリセットする
    /// </summary>
    public void ResetUi()
    {
        ReceptionPlayerSute();
        ChangeKyokuText("");
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
    /// 鳴いた時に表示するUIを生成する
    /// </summary>
    /// <returns></returns>
    private UiNakiPrefab InstatinateUiNakiPrefab()
    {
        UiNakiPrefab _prefab = Instantiate(uiNakiPrefab, transform).GetComponent<UiNakiPrefab>();

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
    /// 鳴いた時に表示するUIを表示する
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

    ///// <summary>
    ///// 局が終わったときに表示するUIを表示する
    ///// </summary>
    //private void DisplayKyokuFinish()
    //{
    //    var prefab = InstatinateUiKyokuFinishPanelPrefab();
    //    prefab.SetMyStatus();
    //}

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
    /// 表示中の自ターンアクションのPrefabを返すゲッター
    /// </summary>
    /// <returns></returns>
    public UiTurnActionPrefab GetActiveUiTurnActionPrefab()
    {
        return activeUiTurnActionPrefab;
    }

}
