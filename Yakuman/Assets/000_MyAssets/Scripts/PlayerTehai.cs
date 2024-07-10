using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlayerTehai : MonoBehaviour
{
    /// <summary>自身のプレイヤーの種類</summary>
    [SerializeField] private MahjongManager.PlayerKind myPlayerKind;

    /// <summary>手牌情報(鳴いてない牌たち)</summary>
    private List<PaiStatusForTehai> myTehais;

    /// <summary>手牌情報の詳細(鳴いてない手牌で牌種ごとに何枚ずつもっているか)</summary>
    private int[] tehaiInformation;

    /// <summary>鳴いてある手牌情報</summary>
    private List<MentsuStatusForTehai> myNakis;

    /// <summary>リーチしたターン</summary>
    private int reachTurn;

    /// <summary>リーチ後に見逃した場合のフラグ</summary>
    private bool reachMinogashiFlg;

    /// <summary>ツモった場合に燃える演出を実施したフラグ</summary>
    private bool tsumoPaiFireFlg;

    private const float TIME_MOTION_SUTE_PLAYER = 0.1f;
    private const float TIME_MOTION_SUTE_OTHER = 0.4f;

    private const int NUM_OF_NONE = -1;

    /// <summary>
    /// 牌の情報を記録するクラス(手牌用)
    /// </summary>
    public class PaiStatusForTehai
    {
        public MahjongManager.PaiStatus myPaiStatus;
        public GameObject myPaiObject;
        public PaiPrefab myPaiPrefab;
        public int masterArrayNumber;

        public PaiStatusForTehai(MahjongManager.PaiStatus _myPaiStatus, GameObject _myPaiObject, PaiPrefab _myPaiPrefab, int _masterArrayNumber)
        {
            myPaiStatus = _myPaiStatus;
            myPaiObject = _myPaiObject;
            myPaiPrefab = _myPaiPrefab;
            masterArrayNumber = _masterArrayNumber;
        }
    }

    /// <summary>
    /// 鳴いた牌の情報を記録するクラス(手牌用)
    /// </summary>
    public class MentsuStatusForTehai
    {
        //public MahjongManager.MentsuStatus myMentsuStatus;
        public GameObject myNakiObject;
        public NakiPrefab myNakiPrefab;

        public MentsuStatusForTehai(/*MahjongManager.MentsuStatus _myMentsuStatus, */GameObject _myNakiObject, NakiPrefab _myNakiPrefab)
        {
            //myMentsuStatus = _myMentsuStatus;
            myNakiObject = _myNakiObject;
            myNakiPrefab = _myNakiPrefab;
        }
    }

    private void Start()
    {
        ResetTehai();
    }

    /// <summary>
    /// 手牌情報をリセットする
    /// </summary>
    public void ResetTehai()
    {
        myTehais = new List<PaiStatusForTehai>();
        myNakis = new List<MentsuStatusForTehai>();
        tehaiInformation = new int[Enum.GetValues(typeof(MahjongManager.PaiKinds)).Length];

        reachTurn = MahjongManager.INDEX_NONE;
        reachMinogashiFlg = false;
    }

    /// <summary>
    /// 手牌を追加する(配牌時・ツモ時に使用)
    /// </summary>
    /// <param name="_pai"></param>
    /// <param name="_masterArrayNumber"></param>
    public void AddTehai(MahjongManager.PaiStatus _pai, int _masterArrayNumber)
    {
        myTehais.Add(new PaiStatusForTehai(_pai, null, null, _masterArrayNumber));
        tehaiInformation[(int)_pai.thisKind]++;

        //Debug.Log($"AddTehai : Add = {_pai.thisKind}");
    }

    /// <summary>
    /// 全ての手牌のオブジェクトを生成する(事前に手牌リストを生成しておく必要アリ)
    /// </summary>
    public void MakeAllTehaiObjects()
    {
        int roopIndex = 0;
        foreach (var item in myTehais)
        {
            var pai = InstantiateTehai(item.myPaiStatus, item.masterArrayNumber,
                MahjongManager.Instance.GetPositionTehai(myPlayerKind, roopIndex, false), MahjongManager.Instance.GetRotation(myPlayerKind, true, false));

            item.myPaiPrefab = pai;
            item.myPaiObject = pai.gameObject;

            roopIndex++;
        }
    }

    /// <summary>
    /// 理牌する
    /// </summary>
    public void RiiPai()
    {
        if (tsumoPaiFireFlg)
        {
            SetTsumoPaiFire(false);
        }

        SortTehai();
        int roopIndex = 0;
        foreach (var item in myTehais)
        {
            item.myPaiPrefab.MoveThisPaiOnTehai(roopIndex);
            roopIndex++;
        }
    }

    /// <summary>
    /// 手牌として牌のオブジェクトを生成し、PaiPrefabを返す
    /// </summary>
    /// <param name="_paiStatus"></param>
    /// <param name="_arrayNumber"></param>
    /// <param name="_position"></param>
    /// <param name="_rotation"></param>
    /// <returns></returns>
    private PaiPrefab InstantiateTehai(MahjongManager.PaiStatus _paiStatus, int _arrayNumber, Vector3 _position, Vector3 _rotation)
    {
        var pai = Instantiate(MahjongManager.Instance.GetPaiPrefab(), MahjongManager.Instance.GetpaiParentTransform()).GetComponent<PaiPrefab>();
        pai.SetStatus(_paiStatus.thisKind, _paiStatus.totalNumber, _arrayNumber, myPlayerKind);
        pai.ChangeTransformPosition(_position);
        pai.ChangeTransformRotate(_rotation);

        return pai;
    }

    /// <summary>
    /// 手牌のリストと牌のオブジェクトを紐づける
    /// </summary>
    /// <param name="_arrayNumber"></param>
    /// <param name="_PaiPrefab"></param>
    public void LinkTehaiListAndPaiObject(int _arrayNumber, PaiPrefab _PaiPrefab)
    {
        if (myTehais[_arrayNumber].myPaiStatus.totalNumber == _PaiPrefab.GetTotalNumber())
        {
            myTehais[_arrayNumber].myPaiPrefab = _PaiPrefab;
            myTehais[_arrayNumber].myPaiObject = _PaiPrefab.gameObject;
        }
        else
        {
            Debug.LogWarning($"LinkTehaiListAndPaiObject : Error\n_arrayNumber = {_arrayNumber}\n" +
                $"totalNumber(tehaiList) = {myTehais[_arrayNumber].myPaiStatus.totalNumber}\n" +
                $"totalNumber(_PaiPrefab) = {_PaiPrefab.GetTotalNumber()}");
        }
    }

    /// <summary>
    /// 手牌のリストをソートする
    /// </summary>
    private void SortTehai()
    {
        myTehais.Sort((a, b) => a.myPaiStatus.totalNumber.CompareTo(b.myPaiStatus.totalNumber));
    }

    /// <summary>
    /// 牌をツモったとき(ドローの意味)
    /// </summary>
    /// <param name="_pai"></param>
    /// <param name="_masterArrayNumber"></param>
    public void Tsumo(MahjongManager.PaiStatus _pai, int _masterArrayNumber)
    {
        int index = myTehais.Count;

        AddTehai(_pai, _masterArrayNumber);

        var pai = InstantiateTehai(myTehais[index].myPaiStatus, myTehais[index].masterArrayNumber,
            MahjongManager.Instance.GetPositionTehai(myPlayerKind, index, true), MahjongManager.Instance.GetRotation(myPlayerKind, true, false));

        myTehais[index].myPaiPrefab = pai;
        myTehais[index].myPaiObject = pai.gameObject;
    }

    /// <summary>
    /// 牌を捨てる(切る牌を指定して)
    /// </summary>
    /// <param name="_index"></param>
    public void SuteMandatory(int _index , bool _reachFlg)
    {
        StartCoroutine(SuteStaging(_index, _reachFlg));
    }

    /// <summary>
    /// 牌を捨てる(考えて)
    /// </summary>
    public void SuteThink()
    {
        int index = 0;

        int highPoint = -1;
        for (int i = 0; i < myTehais.Count; i++)
        {
            int thisPoint = ReturnBackiPriorityPointForSute(myTehais[i]);
            if (thisPoint > highPoint)
            {
                highPoint = thisPoint;
                index = i;
            }
        }

        StartCoroutine(SuteStaging(index, false));
    }

    /// <summary>
    /// 牌を考えて切るために評価値を決めて返す
    /// </summary>
    /// <param name="_paiStatus"></param>
    /// <returns></returns>
    private int ReturnBackiPriorityPointForSute(PaiStatusForTehai _paiStatus)
    {
        var _paiKind = _paiStatus.myPaiStatus.thisKind;

        int point = 0;

        {   // このカッコ内で評価値を決める
            if (_paiKind >= MahjongManager.PaiKinds.J1)
            {
                point = (int)_paiKind % 10;
            }
            else
            {
                int num = (int)_paiKind % 10;
                if (num > 5) num = 10 - num;
                point = (num * 10) + ((int)_paiKind / 10);
            }
            point = 100 - point;
        }

        return point;
    }

    /// <summary>
    /// 牌を捨てたときの演出をこみで処理するコルーチン
    /// </summary>
    /// <param name="_index"></param>
    /// <returns></returns>
    private IEnumerator SuteStaging(int _index, bool _reachFlg)
    {
        MahjongManager.PaiStatus _memoryPaiStatus = myTehais[_index].myPaiStatus;
        int _memoryMasterArrayNumber = myTehais[_index].masterArrayNumber;

        var paiObject = myTehais[_index].myPaiObject;

        {   // 牌を捨てるときに傾けて捨てる演出
            int x = 0, y = 0, z = 0;
            if (myPlayerKind == MahjongManager.PlayerKind.Player)
            {
                y = -1;
            }
            else if (myPlayerKind == MahjongManager.PlayerKind.Shimocha)
            {
                x = -1;
            }
            else if (myPlayerKind == MahjongManager.PlayerKind.Toimen)
            {
                z = -1;
            }
            else if (myPlayerKind == MahjongManager.PlayerKind.Kamicha)
            {
                x = 1;
            }

            paiObject.transform.localPosition += new Vector3(x, y, z);
            paiObject.transform.Rotate(-15, 0, 0);

            yield return new WaitForSeconds(myPlayerKind == MahjongManager.PlayerKind.Player ? TIME_MOTION_SUTE_PLAYER : TIME_MOTION_SUTE_OTHER);
        }

        Destroy(paiObject);
        myTehais.Remove(myTehais[_index]);
        RiiPai();

        tehaiInformation[(int)_memoryPaiStatus.thisKind]--;

        if (_reachFlg) reachTurn = MahjongManager.Instance.GetPlayerKawaComponent(myPlayerKind).GetKawaLength() + 1;

        MahjongManager.Instance.GetPlayerKawaComponent(myPlayerKind).AddKawa(_memoryPaiStatus, _memoryMasterArrayNumber , reachTurn);

        yield return null;

        MahjongManager.Instance.ReceptionPlayerTehaiForTurnEnd();
    }

    /// <summary>
    /// 鳴きの牌のオブジェクト群を生成し、Prefabを返す
    /// </summary>
    /// <param name="_nakiStatus"></param>
    /// <param name="_tehaiStatus1"></param>
    /// <param name="_tehaiStatus2"></param>
    /// <param name="_nakiArrayNumber"></param>
    /// <param name="_tehaiArrayNumber1"></param>
    /// <param name="_tehaiArrayNumber2"></param>
    /// <param name="_nakiPlace"></param>
    /// <param name="_myNakiKind"></param>
    /// <returns></returns>
    private NakiPrefab InstantiateNaki(MahjongManager.PaiStatus _nakiStatus, MahjongManager.PaiStatus _tehaiStatus1, MahjongManager.PaiStatus _tehaiStatus2,
         int _nakiArrayNumber, int _tehaiArrayNumber1, int _tehaiArrayNumber2,
         NakiPrefab.NakiPlace _nakiPlace, MahjongManager.NakiKinds _myNakiKind)
    {
        var naki = Instantiate(MahjongManager.Instance.GetNakiPrefab(_nakiPlace), MahjongManager.Instance.GetpaiParentTransform()).GetComponent<NakiPrefab>();

        naki.SetStatusFirst(_nakiPlace, _myNakiKind);
        naki.SetPaiPrefab(NakiPrefab.PaiPrefabKind.NakiPlace, _nakiStatus.thisKind, _nakiStatus.totalNumber, _nakiArrayNumber, myPlayerKind);
        naki.SetPaiPrefab(NakiPrefab.PaiPrefabKind.Other1, _tehaiStatus1.thisKind, _tehaiStatus1.totalNumber, _tehaiArrayNumber1, myPlayerKind);
        naki.SetPaiPrefab(NakiPrefab.PaiPrefabKind.Other2, _tehaiStatus2.thisKind, _tehaiStatus2.totalNumber, _tehaiArrayNumber2, myPlayerKind);
        naki.SetStatusFinal();

        return naki;
    }

    /// <summary>
    /// 暗槓の牌のオブジェクト群を生成し、Prefabを返す
    /// </summary>
    /// <param name="_tehaiStatus1"></param>
    /// <param name="_tehaiStatus2"></param>
    /// <param name="_tehaiStatus3"></param>
    /// <param name="_tehaiStatus4"></param>
    /// <param name="_tehaiArrayNumber1"></param>
    /// <param name="_tehaiArrayNumber2"></param>
    /// <param name="_tehaiArrayNumber3"></param>
    /// <param name="_tehaiArrayNumber4"></param>
    /// <returns></returns>
    private AnkanPrefab InstantiateNakiOfAnkan(
        MahjongManager.PaiStatus _tehaiStatus1, MahjongManager.PaiStatus _tehaiStatus2,
        MahjongManager.PaiStatus _tehaiStatus3, MahjongManager.PaiStatus _tehaiStatus4,
        int _tehaiArrayNumber1, int _tehaiArrayNumber2, int _tehaiArrayNumber3, int _tehaiArrayNumber4)
    {
        var ankan = Instantiate(MahjongManager.Instance.GetAnkanPrefab(), MahjongManager.Instance.GetpaiParentTransform()).GetComponent<AnkanPrefab>();

        ankan.SetStatus(_tehaiStatus1.thisKind,
            _tehaiStatus1.totalNumber, _tehaiArrayNumber1,
            _tehaiStatus2.totalNumber, _tehaiArrayNumber2,
            _tehaiStatus3.totalNumber, _tehaiArrayNumber3,
            _tehaiStatus4.totalNumber, _tehaiArrayNumber4);

        return ankan;
    }

    /// <summary>
    /// 大明槓の牌のオブジェクト群を生成し、Prefabを返す
    /// </summary>
    /// <param name="_nakiStatus"></param>
    /// <param name="_tehaiStatus1"></param>
    /// <param name="_tehaiStatus2"></param>
    /// <param name="_tehaiStatus3"></param>
    /// <param name="_nakiArrayNumber"></param>
    /// <param name="_tehaiArrayNumber1"></param>
    /// <param name="_tehaiArrayNumber2"></param>
    /// <param name="_tehaiArrayNumber3"></param>
    /// <param name="_nakiPlace"></param>
    /// <param name="_myNakiKind"></param>
    /// <returns></returns>
    private NakiPrefab InstantiateNakiOfDaiminkan(MahjongManager.PaiStatus _nakiStatus,
        MahjongManager.PaiStatus _tehaiStatus1, MahjongManager.PaiStatus _tehaiStatus2, MahjongManager.PaiStatus _tehaiStatus3,
         int _nakiArrayNumber, int _tehaiArrayNumber1, int _tehaiArrayNumber2, int _tehaiArrayNumber3,
         NakiPrefab.NakiPlace _nakiPlace, MahjongManager.NakiKinds _myNakiKind)
    {
        var naki = Instantiate(MahjongManager.Instance.GetNakiPrefab(_nakiPlace), MahjongManager.Instance.GetpaiParentTransform()).GetComponent<NakiPrefab>();

        naki.SetStatusFirst(_nakiPlace, _myNakiKind);
        naki.SetPaiPrefab(NakiPrefab.PaiPrefabKind.NakiPlace, _nakiStatus.thisKind, _nakiStatus.totalNumber, _nakiArrayNumber, myPlayerKind);
        naki.SetPaiPrefab(NakiPrefab.PaiPrefabKind.Other1, _tehaiStatus1.thisKind, _tehaiStatus1.totalNumber, _tehaiArrayNumber1, myPlayerKind);
        naki.SetPaiPrefab(NakiPrefab.PaiPrefabKind.Other2, _tehaiStatus2.thisKind, _tehaiStatus2.totalNumber, _tehaiArrayNumber2, myPlayerKind);
        naki.SetPaiPrefab(NakiPrefab.PaiPrefabKind.Other3, _tehaiStatus3.thisKind, _tehaiStatus3.totalNumber, _tehaiArrayNumber3, myPlayerKind);
        naki.SetStatusFinal();

        return naki;
    }

    /// <summary>
    /// 暗槓を実行したとき
    /// </summary>
    /// <param name="_paiKind"></param>
    public void Ankan(MahjongManager.PaiKinds _paiKind)
    {
        int index1 = NUM_OF_NONE, index2 = NUM_OF_NONE, index3 = NUM_OF_NONE, index4 = NUM_OF_NONE;
        for (int i = 0; i < myTehais.Count; i++)
        {
            if (myTehais[i].myPaiStatus.thisKind == _paiKind)
            {
                if (index1 == NUM_OF_NONE)
                {
                    index1 = i;
                }
                else if (index2 == NUM_OF_NONE)
                {
                    index2 = i;
                }
                else if (index3 == NUM_OF_NONE)
                {
                    index3 = i;
                }
                else
                {
                    index4 = i;
                    break;
                }
            }
        }

        if (index4 == NUM_OF_NONE)
        {
            Debug.LogError($"Ankan : Error , index1 = {index1} , index2 = {index2}, index3 = {index3}, index4 = {index4}");
            return;
        }

        AnkanPrefab _ankanPrefab = InstantiateNakiOfAnkan(
            myTehais[index1].myPaiStatus, myTehais[index2].myPaiStatus,
            myTehais[index3].myPaiStatus, myTehais[index4].myPaiStatus,
            myTehais[index1].masterArrayNumber, myTehais[index2].masterArrayNumber,
            myTehais[index3].masterArrayNumber, myTehais[index4].masterArrayNumber);

        myNakis.Add(new MentsuStatusForTehai(_ankanPrefab.gameObject, _ankanPrefab));
        //myNakis.Add(new MentsuStatusForTehai(_ankanPrefab.GetMyMentsuStatus(), _ankanPrefab.gameObject, _ankanPrefab));

        int _nakiCount = (14 - myTehais.Count) / 3;
        _ankanPrefab.ChangeTransformPosition(MahjongManager.Instance.GetPositionNaki(myPlayerKind, _nakiCount));

        DeleteForNaki(index4);
        DeleteForNaki(index3);
        DeleteForNaki(index2);
        DeleteForNaki(index1);

        RiiPai();

        MahjongManager.Instance.ReceptionPlayerTehaiForKanOfMyTurn(myPlayerKind);
    }

    /// <summary>
    /// 明槓(加槓)を実行したとき
    /// </summary>
    /// <param name="_paiKind"></param>
    public void Kakan(MahjongManager.PaiKinds _paiKind)
    {
        int _tehaiIndex = NUM_OF_NONE;
        for (int i = 0; i < myTehais.Count; i++)
        {
            if (myTehais[i].myPaiStatus.thisKind == _paiKind)
            {
                _tehaiIndex = i;
                break;
            }
        }

        if (_tehaiIndex == NUM_OF_NONE)
        {
            Debug.LogError($"Kakan : Error , index = {_tehaiIndex}");
            return;
        }

        int _nakiIndex = NUM_OF_NONE;
        for (int i = 0; i < myNakis.Count; i++)
        {
            if (myNakis[i].myNakiPrefab.GetMyMentsuStatus().minimumPai == _paiKind &&
                myNakis[i].myNakiPrefab.GetMyMentsuStatus().mentsuKind == MahjongManager.MentsuKinds.Kootsu)
            {
                _nakiIndex = i;
                break;
            }
        }

        myNakis[_nakiIndex].myNakiPrefab.Kakan(NakiPrefab.PaiPrefabKind.KaKan, _paiKind,
            myTehais[_tehaiIndex].myPaiStatus.totalNumber, myTehais[_tehaiIndex].masterArrayNumber, myPlayerKind);

        DeleteForNaki(_tehaiIndex);

        RiiPai();

        MahjongManager.Instance.ReceptionPlayerTehaiForKanOfMyTurn(myPlayerKind);

    }

    /// <summary>
    /// 明槓(大明槓)を実行したとき
    /// </summary>
    /// <param name="_nakiPlace"></param>
    /// <param name="_nakiStatus"></param>
    /// <param name="_nakiArrayNumber"></param>
    public void Daiminkan(NakiPrefab.NakiPlace _nakiPlace, MahjongManager.PaiStatus _nakiStatus, int _nakiArrayNumber)
    {
        MahjongManager.PaiKinds _paiKind = _nakiStatus.thisKind;
        MahjongManager.NakiKinds _nakiKind = MahjongManager.NakiKinds.None;
        switch (_nakiPlace)
        {
            case NakiPrefab.NakiPlace.Right:
                _nakiKind = MahjongManager.NakiKinds.DaiminkanFromShimocha;
                break;
            case NakiPrefab.NakiPlace.Center:
                _nakiKind = MahjongManager.NakiKinds.DaiminkanFromToimen;
                break;
            case NakiPrefab.NakiPlace.Left:
                _nakiKind = MahjongManager.NakiKinds.DaiminkanFromKamicha;
                break;
            default:
                Debug.LogError($"Daiminkan : Error , _nakiPlace = {_nakiPlace}");
                return;
        }

        int index1 = NUM_OF_NONE, index2 = NUM_OF_NONE, index3 = NUM_OF_NONE;
        for (int i = 0; i < myTehais.Count; i++)
        {
            if (myTehais[i].myPaiStatus.thisKind == _paiKind)
            {
                if (index1 == NUM_OF_NONE)
                {
                    index1 = i;
                }
                else if (index2 == NUM_OF_NONE)
                {
                    index2 = i;
                }
                else
                {
                    index3 = i;
                    break;
                }
            }
        }

        if (index3 == NUM_OF_NONE)
        {
            Debug.LogError($"Daiminkan : Error , index1 = {index1} , index2 = {index2}, index3 = {index3}");
            return;
        }

        NakiPrefab _nakiPrefab = InstantiateNakiOfDaiminkan(_nakiStatus, myTehais[index1].myPaiStatus, myTehais[index2].myPaiStatus, myTehais[index3].myPaiStatus,
            _nakiArrayNumber, myTehais[index1].masterArrayNumber, myTehais[index2].masterArrayNumber, myTehais[index3].masterArrayNumber, _nakiPlace, _nakiKind);

        myNakis.Add(new MentsuStatusForTehai(_nakiPrefab.gameObject, _nakiPrefab));
        //myNakis.Add(new MentsuStatusForTehai(_nakiPrefab.GetMyMentsuStatus(), _nakiPrefab.gameObject, _nakiPrefab));

        int _nakiCount = (13 - myTehais.Count) / 3;
        _nakiPrefab.ChangeTransformPosition(MahjongManager.Instance.GetPositionNaki(myPlayerKind, _nakiCount));

        DeleteForNaki(index3);
        DeleteForNaki(index2);
        DeleteForNaki(index1);

        RiiPai();

        MahjongManager.Instance.ReceptionPlayerTehaiForNaki(myPlayerKind, true);
    }

    /// <summary>
    /// ポンを実行したとき
    /// </summary>
    /// <param name="_nakiPlace"></param>
    /// <param name="_nakiStatus"></param>
    /// <param name="_nakiArrayNumber"></param>
    public void Pon(NakiPrefab.NakiPlace _nakiPlace, MahjongManager.PaiStatus _nakiStatus, int _nakiArrayNumber)
    {
        MahjongManager.PaiKinds _paiKind = _nakiStatus.thisKind;
        MahjongManager.NakiKinds _nakiKind = MahjongManager.NakiKinds.None;
        switch (_nakiPlace)
        {
            case NakiPrefab.NakiPlace.Right:
                _nakiKind = MahjongManager.NakiKinds.PonFromShimocha;
                break;
            case NakiPrefab.NakiPlace.Center:
                _nakiKind = MahjongManager.NakiKinds.PonFromToimen;
                break;
            case NakiPrefab.NakiPlace.Left:
                _nakiKind = MahjongManager.NakiKinds.PonFromKamicha;
                break;
            default:
                Debug.LogError($"Pon : Error , _nakiPlace = {_nakiPlace}");
                return;
        }

        int index1 = NUM_OF_NONE, index2 = NUM_OF_NONE;
        for (int i = 0; i < myTehais.Count; i++)
        {
            if (myTehais[i].myPaiStatus.thisKind == _paiKind)
            {
                if (index1 == NUM_OF_NONE)
                {
                    index1 = i;
                }
                else
                {
                    index2 = i;
                    break;
                }
            }
        }

        if (index2 == NUM_OF_NONE)
        {
            Debug.LogError($"Pon : Error , index1 = {index1} , index2 = {index2}");
            return;
        }

        NakiPrefab _nakiPrefab = InstantiateNaki(_nakiStatus, myTehais[index1].myPaiStatus, myTehais[index2].myPaiStatus,
            _nakiArrayNumber, myTehais[index1].masterArrayNumber, myTehais[index2].masterArrayNumber, _nakiPlace, _nakiKind);

        myNakis.Add(new MentsuStatusForTehai(_nakiPrefab.gameObject, _nakiPrefab));
        //myNakis.Add(new MentsuStatusForTehai(_nakiPrefab.GetMyMentsuStatus(), _nakiPrefab.gameObject, _nakiPrefab));

        int _nakiCount = (13 - myTehais.Count) / 3;
        _nakiPrefab.ChangeTransformPosition(MahjongManager.Instance.GetPositionNaki(myPlayerKind, _nakiCount));

        DeleteForNaki(index2);
        DeleteForNaki(index1);

        RiiPai();

        MahjongManager.Instance.ReceptionPlayerTehaiForNaki(myPlayerKind, false);
    }

    /// <summary>
    /// チーを実行したとき
    /// </summary>
    /// <param name="_chiNumber"></param>
    /// <param name="_nakiStatus"></param>
    /// <param name="_nakiArrayNumber"></param>
    public void Chi(NakiPrefab.PaiOfChi _chiNumber, MahjongManager.PaiStatus _nakiStatus, int _nakiArrayNumber)
    {
        MahjongManager.PaiKinds _paiKind = _nakiStatus.thisKind;
        MahjongManager.NakiKinds _nakiKind = MahjongManager.NakiKinds.None;
        MahjongManager.PaiKinds targetKind1 = MahjongManager.PaiKinds.None_00;
        MahjongManager.PaiKinds targetKind2 = MahjongManager.PaiKinds.None_00;
        switch (_chiNumber)
        {
            case NakiPrefab.PaiOfChi.Low:
                _nakiKind = MahjongManager.NakiKinds.ChiNumSmall;
                targetKind1 = _paiKind + 1;
                targetKind2 = _paiKind + 2;
                break;
            case NakiPrefab.PaiOfChi.Mid:
                _nakiKind = MahjongManager.NakiKinds.ChiNumMiddle;
                targetKind1 = _paiKind - 1;
                targetKind2 = _paiKind + 1;
                break;
            case NakiPrefab.PaiOfChi.High:
                _nakiKind = MahjongManager.NakiKinds.ChiNumBig;
                targetKind1 = _paiKind - 2;
                targetKind2 = _paiKind - 1;
                break;
            default:
                Debug.LogError($"Chi : Error , _chiNumber = {_chiNumber}");
                return;
        }

        int index1 = NUM_OF_NONE, index2 = NUM_OF_NONE;
        for (int i = 0; i < myTehais.Count; i++)
        {
            if (index1 == NUM_OF_NONE && myTehais[i].myPaiStatus.thisKind == targetKind1)
            {
                index1 = i;
            }
            else if (index1 != NUM_OF_NONE && myTehais[i].myPaiStatus.thisKind == targetKind2)
            {
                index2 = i;
                break;
            }
        }

        if (index2 == NUM_OF_NONE)
        {
            Debug.LogError($"Chi : Error , index1 = {index1} , index2 = {index2}\ntargetKind1 = {targetKind1} , targetKind2 = {targetKind2}");
            return;
        }

        NakiPrefab _nakiPrefab = InstantiateNaki(_nakiStatus, myTehais[index1].myPaiStatus, myTehais[index2].myPaiStatus,
            _nakiArrayNumber, myTehais[index1].masterArrayNumber, myTehais[index2].masterArrayNumber, NakiPrefab.NakiPlace.Left, _nakiKind);

        myNakis.Add(new MentsuStatusForTehai(_nakiPrefab.gameObject, _nakiPrefab));
        //myNakis.Add(new MentsuStatusForTehai(_nakiPrefab.GetMyMentsuStatus(), _nakiPrefab.gameObject, _nakiPrefab));

        int _nakiCount = (13 - myTehais.Count) / 3;
        _nakiPrefab.ChangeTransformPosition(MahjongManager.Instance.GetPositionNaki(myPlayerKind, _nakiCount));

        DeleteForNaki(index2);
        DeleteForNaki(index1);

        RiiPai();

        MahjongManager.Instance.ReceptionPlayerTehaiForNaki(myPlayerKind, false);
    }

    /// <summary>
    /// 鳴きによって消えさる門前手牌のオブジェクトと情報を削除する
    /// </summary>
    /// <param name="_index"></param>
    private void DeleteForNaki(int _index)
    {
        MahjongManager.PaiKinds _paiKind = myTehais[_index].myPaiStatus.thisKind;

        var paiObject = myTehais[_index].myPaiObject;

        Destroy(paiObject);
        myTehais.Remove(myTehais[_index]);

        tehaiInformation[(int)_paiKind]--;
    }

    /// <summary>
    /// 手牌のタップ可能かを変える
    /// </summary>
    /// <param name="_paiKindList">タップ可能な牌のリスト(nullなら全部タップ可能にする)</param>
    public void ChangeTehaiChangeInteractableTap(List<MahjongManager.PaiKinds> _paiKindList)
    {
        if (_paiKindList == null)
        {
            foreach (var item in myTehais)
            {
                item.myPaiPrefab.ChangeInteractableTap(true);
            }
        }
        else
        {
            foreach (var item in myTehais)
            {
                bool flg = false;
                foreach (var item2 in _paiKindList)
                {
                    if (item2 == item.myPaiPrefab.GetThisKind())
                    {
                        flg = true;
                        break;
                    }
                }
                item.myPaiPrefab.ChangeInteractableTap(flg);
            }
        }
    }

    /// <summary>
    /// 指定したトータル番号の牌を持っているかを返すゲッター(返り値は手牌のインデックス)
    /// </summary>
    /// <param name="_totalNumber"></param>
    /// <returns>返り値は手牌のインデックス(エラーは-1)</returns>
    public int CheckHavePai(int _totalNumber)
    {
        for (int i = 0; i < myTehais.Count; i++)
        {
            if (myTehais[i].myPaiStatus.totalNumber == _totalNumber)
            {
                return i;
            }
        }

        //Debug.LogWarning($"CheckHavePai : Dont have it , totalnumber = {_totalNumber}");
        return -1;
    }

    /// <summary>
    /// 捨てることが可能なのかを返すゲッター
    /// </summary>
    /// <returns></returns>
    public bool CheckAbleSute()
    {
        if (myTehais.Count % 3 == 2) return true;
        else return false;
    }

    /// <summary>
    /// 指定した牌種を何枚持っているかを返すゲッター
    /// </summary>
    /// <param name="paiKind"></param>
    /// <returns></returns>
    public int GetHavePaiKindCount(MahjongManager.PaiKinds paiKind)
    {
        return tehaiInformation[(int)paiKind];
    }

    /// <summary>
    /// ロンが可能かを返すゲッター
    /// </summary>
    /// <param name="_paiKind"></param>
    /// <returns></returns>
    public bool GetAbleRon(MahjongManager.PaiKinds _paiKind)
    {
        int[] copyTehaiInformation = new int[Enum.GetValues(typeof(MahjongManager.PaiKinds)).Length];

        Array.Copy(tehaiInformation, copyTehaiInformation, tehaiInformation.Length);

        copyTehaiInformation[(int)_paiKind]++;

        bool result = MahjongManager.Instance.CheckAgariAll(copyTehaiInformation);

        return result;
    }

    /// <summary>
    /// ポンが可能かを返すゲッター
    /// </summary>
    /// <param name="paiKind"></param>
    /// <returns></returns>
    public bool GetAblePon(MahjongManager.PaiKinds paiKind)
    {
        if (MahjongManager.Instance.GetFlgTsumoCountEqualFinishCount()) return false;
        else if (GetHavePaiKindCount(paiKind) >= 2)
        {
            return true;
        }
        else return false;
    }

    /// <summary>
    /// カンが可能かを返すゲッター
    /// </summary>
    /// <param name="paiKind"></param>
    /// <returns></returns>
    public bool GetAbleDaiminkan(MahjongManager.PaiKinds paiKind)
    {
        if (MahjongManager.Instance.GetFlgTsumoCountEqualFinishCount()) return false;
        else if (GetHavePaiKindCount(paiKind) >= 3)
        {
            return true;
        }
        else return false;
    }

    /// <summary>
    /// チー(対象が二面で、メンツの中で一番小さい数字の牌)が可能かを返すゲッター
    /// </summary>
    /// <param name="paiKind"></param>
    /// <returns></returns>
    public bool GetAbleChiLow(MahjongManager.PaiKinds paiKind, MahjongManager.PlayerKind playerKind)
    {
        if (MahjongManager.Instance.GetFlgTsumoCountEqualFinishCount()) return false;
        else if (!GetAbleChiPlayer(playerKind)) return false;
        else if (paiKind >= MahjongManager.PaiKinds.S8) return false;
        else if (paiKind <= MahjongManager.PaiKinds.None_00) return false;
        else if (GetHavePaiKindCount(paiKind + 1) >= 1 && GetHavePaiKindCount(paiKind + 2) >= 1)
        {
            return true;
        }
        else return false;
    }

    /// <summary>
    /// チー(嵌張)が可能かを返すゲッター
    /// </summary>
    /// <param name="paiKind"></param>
    /// <returns></returns>
    public bool GetAbleChiMid(MahjongManager.PaiKinds paiKind, MahjongManager.PlayerKind playerKind)
    {
        if (MahjongManager.Instance.GetFlgTsumoCountEqualFinishCount()) return false;
        else if (!GetAbleChiPlayer(playerKind)) return false;
        else if (paiKind >= MahjongManager.PaiKinds.S9) return false;
        else if (paiKind <= MahjongManager.PaiKinds.M1) return false;
        else if (GetHavePaiKindCount(paiKind - 1) >= 1 && GetHavePaiKindCount(paiKind + 1) >= 1)
        {
            return true;
        }
        else return false;
    }

    /// <summary>
    /// チー(対象が二面で、メンツの中で一番大きい数字の牌)が可能かを返すゲッター
    /// </summary>
    /// <param name="paiKind"></param>
    /// <returns></returns>
    public bool GetAbleChiHigh(MahjongManager.PaiKinds paiKind, MahjongManager.PlayerKind playerKind)
    {
        if (MahjongManager.Instance.GetFlgTsumoCountEqualFinishCount()) return false;
        else if (!GetAbleChiPlayer(playerKind)) return false;
        else if (paiKind >= MahjongManager.PaiKinds.None_30) return false;
        else if (paiKind <= MahjongManager.PaiKinds.M2) return false;
        else if (GetHavePaiKindCount(paiKind - 1) >= 1 && GetHavePaiKindCount(paiKind - 2) >= 1)
        {
            return true;
        }
        else return false;
    }

    /// <summary>
    /// 対象が自身にとってチー可能な相手(上家)かを返すゲッター
    /// </summary>
    /// <param name="_kind"></param>
    /// <returns></returns>
    public bool GetAbleChiPlayer(MahjongManager.PlayerKind _kind)
    {
        var kind = (int)myPlayerKind - (int)_kind;
        if (kind <= 0) kind += 4;
        if (kind == 1) return true;
        else return false;
    }

    /// <summary>
    /// 暗槓が可能かを返すゲッター
    /// </summary>
    /// <returns></returns>
    public bool GetAbleAnkan(bool _reachFlg)
    {
        if (_reachFlg)
        {
            var _pai = GetPaiStatusTehaiOfRight().thisKind;

            if (tehaiInformation[(int)_pai] == 4)
            {
                // 聴牌形が変わる場合は暗槓できないようにする処理

                int counter1 = 0;
                {
                    int[] copyTehaiInformationList = new int[GetTehaiInformation().Length];
                    Array.Copy(GetTehaiInformation(), copyTehaiInformationList, GetTehaiInformation().Length);

                    copyTehaiInformationList[(int)_pai] = 3;

                    for (int i = 0; i < copyTehaiInformationList.Length; i++)
                    {
                        if (i % 10 == 0) continue;
                        int[] copyCopyTehaiInformationList = new int[copyTehaiInformationList.Length];
                        Array.Copy(copyTehaiInformationList, copyCopyTehaiInformationList, copyTehaiInformationList.Length);

                        copyCopyTehaiInformationList[i]++;
                        bool result = MahjongManager.Instance.CheckAgariAll(copyCopyTehaiInformationList);
                        if (result)
                        {
                            counter1++;
                        }
                    }
                }

                int counter2 = 0;
                {
                    int[] copyTehaiInformationList = new int[GetTehaiInformation().Length];
                    Array.Copy(GetTehaiInformation(), copyTehaiInformationList, GetTehaiInformation().Length);

                    copyTehaiInformationList[(int)_pai] = 0;

                    for (int i = 0; i < copyTehaiInformationList.Length; i++)
                    {
                        if (i % 10 == 0) continue;
                        int[] copyCopyTehaiInformationList = new int[copyTehaiInformationList.Length];
                        Array.Copy(copyTehaiInformationList, copyCopyTehaiInformationList, copyTehaiInformationList.Length);

                        copyCopyTehaiInformationList[i]++;
                        bool result = MahjongManager.Instance.CheckAgariAll(copyCopyTehaiInformationList);
                        if (result)
                        {
                            counter2++;
                        }
                    }
                }

                return counter1 == counter2 ? true : false;
            }
            else
            {
                return false;
            }
        }
        else
        {
            for (int i = 0; i < tehaiInformation.Length; i++)
            {
                if (tehaiInformation[i] == 4)
                {
                    return true;
                }
            }
            return false;
        }
    }

    /// <summary>
    /// 指定した牌種が暗槓が可能かを返すゲッター
    /// </summary>
    /// <param name="_paiKind"></param>
    /// <returns></returns>
    public bool GetAbleAnkanForPaiKind(MahjongManager.PaiKinds _paiKind)
    {
        if (tehaiInformation[(int)_paiKind] == 4)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 加槓が可能かを返すゲッター
    /// </summary>
    /// <returns></returns>
    public bool GetAbleKakan()
    {
        for (int i = 0; i < myNakis.Count; i++)
        {
            if (myNakis[i].myNakiPrefab.GetMyMentsuStatus().mentsuKind == MahjongManager.MentsuKinds.Kootsu &&
                tehaiInformation[(int)myNakis[i].myNakiPrefab.GetMyMentsuStatus().minimumPai] == 1)
            {
                return true;
            }
            //Debug.Log($"myNakis[i].myMentsuStatus.mentsuKind = {myNakis[i].myNakiPrefab.GetMyMentsuStatus().mentsuKind} , " +
            //    $"tehaiInformation[(int)myNakis[i].myMentsuStatus.minimumPai] {tehaiInformation[(int)myNakis[i].myNakiPrefab.GetMyMentsuStatus().minimumPai]}");
        }
        return false;
    }

    /// <summary>
    /// 指定した牌種が加槓が可能かを返すゲッター
    /// </summary>
    /// <param name="_paiKind"></param>
    /// <returns></returns>
    public bool GetAbleKakanForPaiKind(MahjongManager.PaiKinds _paiKind)
    {
        for (int i = 0; i < myNakis.Count; i++)
        {
            if (myNakis[i].myNakiPrefab.GetMyMentsuStatus().mentsuKind == MahjongManager.MentsuKinds.Kootsu &&
                myNakis[i].myNakiPrefab.GetMyMentsuStatus().minimumPai == _paiKind &&
                tehaiInformation[(int)_paiKind] == 1)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// ツモ(和了)が可能かを返すゲッター
    /// </summary>
    /// <returns></returns>
    public bool GetAbleTsumoAgari()
    {
        bool result = MahjongManager.Instance.CheckAgariAll(tehaiInformation);

        return result;
    }

    /// <summary>
    /// 流局(九種九牌)が可能かを返すゲッター
    /// </summary>
    /// <returns></returns>
    public bool GetAbleRyuukyoku()
    {
        if (MahjongManager.Instance.GetPlayerKawaComponent(myPlayerKind).GetKawaLength() != 0) return false;

        int counter = 0;

        if (tehaiInformation[(int)MahjongManager.PaiKinds.M1] >= 1) counter++;
        if (tehaiInformation[(int)MahjongManager.PaiKinds.M9] >= 1) counter++;
        if (tehaiInformation[(int)MahjongManager.PaiKinds.P1] >= 1) counter++;
        if (tehaiInformation[(int)MahjongManager.PaiKinds.P9] >= 1) counter++;
        if (tehaiInformation[(int)MahjongManager.PaiKinds.S1] >= 1) counter++;
        if (tehaiInformation[(int)MahjongManager.PaiKinds.S9] >= 1) counter++;
        if (tehaiInformation[(int)MahjongManager.PaiKinds.J1] >= 1) counter++;
        if (tehaiInformation[(int)MahjongManager.PaiKinds.J2] >= 1) counter++;
        if (tehaiInformation[(int)MahjongManager.PaiKinds.J3] >= 1) counter++;
        if (tehaiInformation[(int)MahjongManager.PaiKinds.J4] >= 1) counter++;
        if (tehaiInformation[(int)MahjongManager.PaiKinds.J5] >= 1) counter++;
        if (tehaiInformation[(int)MahjongManager.PaiKinds.J6] >= 1) counter++;
        if (tehaiInformation[(int)MahjongManager.PaiKinds.J7] >= 1) counter++;

        if (counter >= 9) return true;
        else return false;
    }

    /// <summary>
    /// リーチが可能かを返すゲッター
    /// </summary>
    /// <returns>捨てる牌</returns>
    public List<MahjongManager.PaiKinds> GetAbleReach()
    {
        List<MahjongManager.PaiKinds> resultList = new List<MahjongManager.PaiKinds>();

        foreach (var item in myNakis) // 鳴きに暗槓以外が存在するか調べる
        {
            if (item.myNakiPrefab.GetMyMentsuStatus().nakiKinds != MahjongManager.NakiKinds.Ankan)
            {
                return resultList;
            }
        }

        MahjongManager.PaiKinds backNumberKind = MahjongManager.PaiKinds.None_00;
        for (int i = 0; i < myTehais.Count; i++)
        {
            if (backNumberKind == myTehais[i].myPaiStatus.thisKind) continue;

            if (i == myTehais.Count - 1)
            {
                foreach (var item in resultList)
                {
                    if (item == myTehais[i].myPaiStatus.thisKind) return resultList;
                }
            }

            backNumberKind = myTehais[i].myPaiStatus.thisKind;

            int[] copyTehaiInformation = new int[Enum.GetValues(typeof(MahjongManager.PaiKinds)).Length];

            Array.Copy(tehaiInformation, copyTehaiInformation, tehaiInformation.Length);

            copyTehaiInformation[(int)myTehais[i].myPaiStatus.thisKind]--;

            if (MahjongManager.Instance.CheckTenpai(copyTehaiInformation))
            {
                resultList.Add(myTehais[i].myPaiStatus.thisKind);
            }

        }

        return resultList;
    }

    /// <summary>
    /// リーチしたターンを返すゲッター
    /// </summary>
    /// <returns></returns>
    public int GetReachTurn()
    {
        return reachTurn;
    }

    /// <summary>
    /// 手牌情報を返すゲッター
    /// </summary>
    /// <returns></returns>
    public int[] GetTehaiInformation()
    {
        return tehaiInformation;
    }

    /// <summary>
    /// 鳴きのメンツリストを返すゲッター
    /// </summary>
    /// <returns></returns>
    public List<MahjongManager.MentsuStatus> GetNakis()
    {
        List<MahjongManager.MentsuStatus> resultList = new List<MahjongManager.MentsuStatus>();

        foreach(var item in myNakis)
        {
            resultList.Add(item.myNakiPrefab.GetMyMentsuStatus());
        }

        return resultList;
    }

    /// <summary>
    /// 手牌のリストを返すゲッター
    /// </summary>
    /// <returns></returns>
    public List<MahjongManager.PaiStatus> GetTehais()
    {
        List<MahjongManager.PaiStatus> resultList = new List<MahjongManager.PaiStatus>();

        foreach (var item in myTehais)
        {
            resultList.Add(item.myPaiStatus);
        }

        return resultList;
    }

    /// <summary>
    /// 手牌の一番右の牌を返すゲッター
    /// </summary>
    /// <returns></returns>
    public MahjongManager.PaiStatus GetPaiStatusTehaiOfRight()
    {
        return myTehais[myTehais.Count - 1].myPaiStatus;
    }

    /// <summary>
    /// 立直後に見逃しをしたかのフラグを返すゲッター
    /// </summary>
    /// <returns></returns>
    public bool GetMinogashiAfterReachFlg()
    {
        return reachMinogashiFlg;
    }

    /// <summary>
    /// 立直後の見逃しをしたフラグをTrueにする
    /// </summary>
    public void SetMinogashiAfterReachFlg()
    {
        reachMinogashiFlg = true;
    }

    /// <summary>
    /// 牌を燃やすエフェクト
    /// </summary>
    /// <param name="_flg"></param>
    public void SetTsumoPaiFire(bool _flg)
    {
        tsumoPaiFireFlg = _flg;
        myTehais[myTehais.Count - 1].myPaiPrefab.PlayParticleFire(tsumoPaiFireFlg);
    }

    /// <summary>
    /// インスペクターのボタンを押したとき
    /// </summary>
    public void InspectorButtonFunction()
    {
        string str = "InspectorButtonFunction : Display Log of Tehai Status";
        for (int i = 0; i < tehaiInformation.Length; i++)
        {
            if (i % 10 == 0)
            {
                if (i == 0) str += "\nM : ";
                else if (i == 10) str += "\nP : ";
                else if (i == 20) str += "\nS : ";
                else if (i == 30) str += "\nJ : ";
            }
            else
            {
                str += $"{tehaiInformation[i]}, ";
            }
        }
        str += $"\n[{myTehais.Count}Count] : ";
        foreach (var item in myTehais)
        {
            str += $"{item.myPaiStatus.thisKind}, ";
        }
        Debug.Log(str);
    }
}

[CustomEditor(typeof(PlayerTehai))] // PaiPrefabを拡張する
public class PaiPrefabDisplayLogForPlayerTehai : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Display Log of Tehai Status"))
        {
            PlayerTehai yourScript = (PlayerTehai)target;
            yourScript.InspectorButtonFunction();
        }
    }
}
