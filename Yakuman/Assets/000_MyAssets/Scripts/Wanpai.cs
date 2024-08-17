using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wanpai : MonoBehaviour
{
    private PaiPrefab[] doraDisplays;
    private int nowKanIndex;
    private List<MahjongManager.PaiKinds> doraHyoujiList;

    private const int COUNT_DORA = 5;
    private const int COUNT_PAI = 136;

    /// <summary>
    /// 王牌の変数の初期化
    /// </summary>
    public void ResetWanpai()
    {
        doraDisplays = new PaiPrefab[COUNT_DORA];
        nowKanIndex = 0;
        doraHyoujiList = new List<MahjongManager.PaiKinds>();
    }

    /// <summary>
    /// カン時にツモるべき嶺上牌のインデックスを返し、カン回数のインデックスをインクリメントして、ついでに新ドラをめくる
    /// </summary>
    /// <returns></returns>
    public int GetAndAddRinshanIndex()
    {
        nowKanIndex++;

        int result = COUNT_PAI - nowKanIndex;

        AddDoraDisplay(nowKanIndex);

        return result;
    }

    /// <summary>
    /// ドラ表示を生成する
    /// </summary>
    public void MakeDoraHyouji()
    {
        int reIndex = COUNT_PAI - 1;
        for (int i = 0; i < COUNT_DORA - 1; i++)
        {
            reIndex--;
        }
        for (int i = 0; i < COUNT_DORA; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                var pai = MahjongManager.Instance.GetPaiyama(reIndex);
                var prefab = InstantiateWanpai(pai, reIndex, MahjongManager.Instance.GetPositionWanpai(i * 2 + j), MahjongManager.Instance.GetRotationUra());
                if (j == 0) doraDisplays[i] = prefab;
                reIndex--;
            }
        }
        AddDoraDisplay(0);
    }

    /// <summary>
    /// ドラ表示牌をめくる
    /// </summary>
    /// <param name="_openPlace"></param>
    private void AddDoraDisplay(int _openPlace)
    {
        //Debug.Log($"AddDoraDisplay : _openPlace = {_openPlace}");

        doraDisplays[_openPlace].ChangeTransformRotate(MahjongManager.Instance.GetRotation(MahjongManager.PlayerKind.Other, false, false));
        doraHyoujiList.Add(doraDisplays[_openPlace].GetThisKind()); // ドラ表示牌リストに追加
    }

    /// <summary>
    /// 王牌を生成する
    /// </summary>
    /// <param name="_paiStatus"></param>
    /// <param name="_arrayNumber"></param>
    /// <param name="_position"></param>
    /// <param name="_rotation"></param>
    /// <returns></returns>
    private PaiPrefab InstantiateWanpai(MahjongManager.PaiStatus _paiStatus, int _arrayNumber, Vector3 _position, Vector3 _rotation)
    {
        var pai = Instantiate(MahjongManager.Instance.GetPaiPrefab(), MahjongManager.Instance.GetpaiParentTransform()).GetComponent<PaiPrefab>();
        pai.SetStatus(_paiStatus.thisKind, _paiStatus.totalNumber, _arrayNumber, MahjongManager.PlayerKind.Other);
        pai.ChangeTransformPosition(_position);
        pai.ChangeTransformRotate(_rotation);

        return pai;
    }

    /// <summary>
    /// ドラ表示牌のリストを返すゲッター
    /// </summary>
    /// <returns></returns>
    public List<MahjongManager.PaiKinds> GetDoraHyoujiList()
    {
        //Debug.Log($"GetDoraHyoujiList : {string.Join(",",doraHyoujiList)}");

        return doraHyoujiList;
    }

}
