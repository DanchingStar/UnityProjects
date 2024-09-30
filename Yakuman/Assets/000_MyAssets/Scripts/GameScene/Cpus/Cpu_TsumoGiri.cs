using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cpu_TsumoGiri : CpuParent
{
    protected new void Start()
    {
        myName = GetType().Name;
    }

    //public override MahjongManager.PaiKinds Sute(int[] _tehaiInformation, List<PlayerTehai.MentsuStatusForTehai> nakiList)
    //{
    //    return MahjongManager.PaiKinds.None_00;
    //}

    //public override MahjongManager.NakiKinds Naki(MahjongManager.PaiKinds _sutePai, int[] _tehaiInformationList, List<PlayerTehai.MentsuStatusForTehai> nakiList,
    //    MahjongManager.PlayerKind myPlayer, MahjongManager.PlayerKind _sutePlayer)
    //{
    //    return MahjongManager.NakiKinds.Through;
    //}
}
