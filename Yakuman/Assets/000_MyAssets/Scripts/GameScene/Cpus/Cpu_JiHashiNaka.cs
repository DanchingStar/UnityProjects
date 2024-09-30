using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cpu_JiHashiNaka : CpuParent
{
    protected new void Start()
    {
        myName = GetType().Name;
    }

    public override MahjongManager.PaiKinds Sute(MahjongManager.PaiKinds _tsumoPai, int[] _tehaiInformation, List<PlayerTehai.MentsuStatusForTehai> nakiList)
    {
        MahjongManager.PaiKinds result = MahjongManager.PaiKinds.None_00;

        int highPoint = -1;
        for (int i = 0; i < _tehaiInformation.Length; i++)
        {
            if (_tehaiInformation[i] > 0)
            {
                int thisPoint = ReturnBackPriorityPointForSute((MahjongManager.PaiKinds)i);
                if (thisPoint > highPoint)
                {
                    highPoint = thisPoint;
                    result = (MahjongManager.PaiKinds)i;
                }
            }
        }

        //Debug.Log($"Cpu_JiHashiNaka.Sute : result = {result}");

        return result;
    }

    public override MahjongManager.NakiKinds Naki(MahjongManager.PaiKinds _sutePai, int[] _tehaiInformationList, List<PlayerTehai.MentsuStatusForTehai> nakiList,
        MahjongManager.PlayerKind myPlayer, MahjongManager.PlayerKind _sutePlayer)
    {
        MahjongManager.NakiKinds result = MahjongManager.NakiKinds.Through;

        bool flgChiL = MahjongManager.Instance.GetPlayerTehaiComponent(myPlayer).GetAbleChiLow(_sutePai, _sutePlayer);
        bool flgChiM = MahjongManager.Instance.GetPlayerTehaiComponent(myPlayer).GetAbleChiMid(_sutePai, _sutePlayer);
        bool flgChiH = MahjongManager.Instance.GetPlayerTehaiComponent(myPlayer).GetAbleChiHigh(_sutePai, _sutePlayer);
        bool flgPon = MahjongManager.Instance.GetPlayerTehaiComponent(myPlayer).GetAblePon(_sutePai);
        bool flgKan = MahjongManager.Instance.GetPlayerTehaiComponent(myPlayer).GetAbleDaiminkan(_sutePai);

        if (flgPon && !flgKan) result = MahjongManager.NakiKinds.Pon;
        else if (flgChiL) result = MahjongManager.NakiKinds.ChiNumLow;
        else if (flgChiM) result = MahjongManager.NakiKinds.ChiNumMiddle;
        else if (flgChiH) result = MahjongManager.NakiKinds.ChiNumHigh;
        else result = MahjongManager.NakiKinds.Through;

        //Debug.Log($"Cpu_JiHashiNaka.Naki : result = {result}");

        return result;
    }

    /// <summary>
    /// 牌を考えて切るために評価値を決めて返す
    /// </summary>
    /// <param name="_paiKind"></param>
    /// <returns>値が高いほうが捨てる候補になる</returns>
    private int ReturnBackPriorityPointForSute(MahjongManager.PaiKinds _paiKind)
    {
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
}
