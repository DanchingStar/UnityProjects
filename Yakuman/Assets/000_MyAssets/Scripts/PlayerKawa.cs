using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKawa : MonoBehaviour
{
    /// <summary>自身のプレイヤーの種類</summary>
    [SerializeField] private MahjongManager.PlayerKind myPlayerKind;

    /// <summary>手牌情報</summary>
    private List<PaiStatusForKawa> myKawas;

    /// <summary>ロン可能な場合にオーラが出る演出を実施したフラグ</summary>
    private bool ronPaiAuraFlg;

    public class PaiStatusForKawa
    {
        public MahjongManager.PaiStatus myPaiStatus;
        public GameObject myPaiObject;
        public PaiPrefab myPaiPrefab;
        public int masterArrayNumber;

        public PaiStatusForKawa(MahjongManager.PaiStatus _myPaiStatus, GameObject _myPaiObject, PaiPrefab _myPaiPrefab, int _masterArrayNumber)
        {
            myPaiStatus = _myPaiStatus;
            myPaiObject = _myPaiObject;
            myPaiPrefab = _myPaiPrefab;
            masterArrayNumber = _masterArrayNumber;
        }
    }

    private void Start()
    {
        ResetKawa();
    }

    /// <summary>
    /// 河情報をリセットする
    /// </summary>
    public void ResetKawa()
    {
        myKawas = new List<PaiStatusForKawa>();
        ronPaiAuraFlg = false;
    }

    /// <summary>
    /// 河を追加する
    /// </summary>
    /// <param name="_pai"></param>
    /// <param name="_masterArrayNumber"></param>
    public void AddKawa(MahjongManager.PaiStatus _pai, int _masterArrayNumber, int _reachTurn)
    {
        int index = myKawas.Count;

        myKawas.Add(new PaiStatusForKawa(_pai, null, null, _masterArrayNumber));

        var pai = InstantiateKawa(myKawas[index].myPaiStatus, _masterArrayNumber,
            MahjongManager.Instance.GetPositionKawa(myPlayerKind, index, _reachTurn),
            MahjongManager.Instance.GetRotation(myPlayerKind, false, _reachTurn == GetKawaLength()));

        //Debug.Log($"AddKawa : _reachTurn = {_reachTurn} , GetKawaLength() = {GetKawaLength()}");

        myKawas[index].myPaiPrefab = pai;
        myKawas[index].myPaiObject = pai.gameObject;
    }

    /// <summary>
    /// 河の最後な牌を消す(鳴いた時など)
    /// </summary>
    public void RemoveKawaLast()
    {
        int index = myKawas.Count - 1;

        Destroy(myKawas[index].myPaiObject);
        myKawas.Remove(myKawas[index]);
    }

    /// <summary>
    /// 河として牌のオブジェクトを生成し、PaiPrefabを返す
    /// </summary>
    /// <param name="_paiStatus"></param>
    /// <param name="_arrayNumber"></param>
    /// <param name="_position"></param>
    /// <param name="_rotation"></param>
    /// <returns></returns>
    private PaiPrefab InstantiateKawa(MahjongManager.PaiStatus _paiStatus, int _arrayNumber, Vector3 _position, Vector3 _rotation)
    {
        var pai = Instantiate(MahjongManager.Instance.GetPaiPrefab(), MahjongManager.Instance.GetpaiParentTransform()).GetComponent<PaiPrefab>();
        pai.SetStatus(_paiStatus.thisKind, _paiStatus.totalNumber, _arrayNumber, myPlayerKind);
        pai.ChangeTransformPosition(_position);
        pai.ChangeTransformRotate(_rotation);

        return pai;
    }

    /// <summary>
    /// フリテンかどうかを調べて返す
    /// </summary>
    /// <param name="_tehaiInformation">和了る候補のプレイヤーの手牌</param>
    /// <param name="_ronPlayerKind">ロンされそうな相手のプレイヤー</param>
    /// <returns></returns>
    public bool CheckFuriten(int[] _tehaiInformation , MahjongManager.PlayerKind _ronPlayerKind)
    {
        bool furitenFlg = false;
        MahjongManager.PaiKinds furitenPai = MahjongManager.PaiKinds.None_00;

        List<PaiStatusForKawa> copyKawaList = new List<PaiStatusForKawa>(myKawas);

        MahjongManager.PlayerKind migi = (int)myPlayerKind + 1 > (int)MahjongManager.PlayerKind.Kamicha ?
            myPlayerKind - 4 + 1 : myPlayerKind + 1;
        MahjongManager.PlayerKind mae = (int)myPlayerKind + 2 > (int)MahjongManager.PlayerKind.Kamicha ?
            myPlayerKind - 4 + 2 : myPlayerKind + 2;
        MahjongManager.PlayerKind hidari = (int)myPlayerKind + 3 > (int)MahjongManager.PlayerKind.Kamicha ?
            myPlayerKind - 4 + 3 : myPlayerKind + 3;

        // 同巡フリテンの可能性を追加
        if (_ronPlayerKind == migi)
        {
            // なし
        }
        else if (_ronPlayerKind == mae)
        {
            PaiStatusForKawa lastPai = MahjongManager.Instance.GetPlayerKawaComponent(migi).GetLastSutePai();
            if (lastPai != null)
            {
                copyKawaList.Add(lastPai);
            }
        }
        else if (_ronPlayerKind == hidari)
        {
            PaiStatusForKawa lastPai1 = MahjongManager.Instance.GetPlayerKawaComponent(migi).GetLastSutePai();
            if (lastPai1 != null)
            {
                copyKawaList.Add(lastPai1);
            }
            PaiStatusForKawa lastPai2 = MahjongManager.Instance.GetPlayerKawaComponent(mae).GetLastSutePai();
            if (lastPai2 != null)
            {
                copyKawaList.Add(lastPai2);
            }
        }
        else
        {
            Debug.LogError($"CheckFuriten : Error , myPlayerKind = {myPlayerKind} , ronPlayerKind = {_ronPlayerKind}\n" +
                $"migi = {migi} , mae = {mae} , hidari = {hidari}");
        }

        copyKawaList.Sort((a, b) => a.myPaiStatus.totalNumber.CompareTo(b.myPaiStatus.totalNumber));

        MahjongManager.PaiKinds backNumberKind = MahjongManager.PaiKinds.None_00;
        foreach (var item in copyKawaList)
        {
            if (backNumberKind == item.myPaiStatus.thisKind) continue;
            backNumberKind = item.myPaiStatus.thisKind;

            int[] copyTehaiInformation = new int[System.Enum.GetValues(typeof(MahjongManager.PaiKinds)).Length];
            System.Array.Copy(_tehaiInformation, copyTehaiInformation, _tehaiInformation.Length);
            copyTehaiInformation[(int)item.myPaiStatus.thisKind]++;

            if (MahjongManager.Instance.CheckAgariAll(copyTehaiInformation))
            {
                furitenFlg = true;
                furitenPai = item.myPaiStatus.thisKind;
                break;
            }
        }

        string logStr = $"CheckFuriten : Furiten is {furitenFlg} , myPlayerKind = {myPlayerKind} , ronPlayerKind = {_ronPlayerKind}\nmigi = {migi} , mae = {mae} , hidari = {hidari}";
        if (furitenFlg) logStr += $" , furitenPai = {furitenPai}";
        Debug.Log(logStr);

        return furitenFlg;
    }

    /// <summary>
    /// 最後に捨てた牌の情報を返すゲッター
    /// </summary>
    /// <returns></returns>
    public PaiStatusForKawa GetLastSutePai()
    {
        PaiStatusForKawa result;
        if (myKawas.Count > 0)
        {
            result = myKawas[myKawas.Count - 1];
        }
        else
        {
            result = null;
        }
        //Debug.Log($"GetLastSutePai : PlayerKind = {myPlayerKind} , Sutehai {result.myPaiStatus.thisKind}");
        return result;
    }

    /// <summary>
    /// 河の捨牌の数を返すゲッター
    /// </summary>
    /// <returns></returns>
    public int GetKawaLength()
    {
        return myKawas.Count;
    }

    /// <summary>
    /// 川の牌情報を返すゲッター
    /// </summary>
    /// <returns></returns>
    public List<PaiStatusForKawa> GetKawaAll()
    {
        return myKawas;
    }

    /// <summary>
    /// 牌からオーラが出るエフェクト
    /// </summary>
    /// <param name="_flg"></param>
    public void SetRonPaiAura(bool _flg)
    {
        if (_flg != ronPaiAuraFlg)
        {
            ronPaiAuraFlg = _flg;
            GetLastSutePai().myPaiPrefab.PlayParticleAura(ronPaiAuraFlg);
        }
    }

    /// <summary>
    /// 鳴き候補の牌を目立たせる
    /// </summary>
    /// <param name="_flg"></param>
    public void SetNakiPaiObjects(bool _flg)
    {
        if (GetLastSutePai() != null) GetLastSutePai().myPaiPrefab.PlayNakiObjects(_flg);
    }


}
