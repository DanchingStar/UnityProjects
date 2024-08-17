using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CpuParent : MonoBehaviour
{
    protected string myName = "";

    protected void Start()
    {
    }

    /// <summary>
    /// 捨てる牌を思考して選ぶ
    /// </summary>
    /// <param name="_tehaiInformationList"></param>
    /// <param name="nakiList"></param>
    /// <returns></returns>
    public virtual MahjongManager.PaiKinds Sute(MahjongManager.PaiKinds _tsumoPai, int[] _tehaiInformationList , List<PlayerTehai.MentsuStatusForTehai> nakiList)
    {
        return MahjongManager.PaiKinds.None_00;
    }

    /// <summary>
    /// 鳴きかを判断して選ぶ
    /// </summary>
    /// <param name="_sutePai"></param>
    /// <param name="_tehaiInformationList"></param>
    /// <param name="nakiList"></param>
    /// <param name="myPlayer"></param>
    /// <param name="_sutePlayer"></param>
    /// <returns></returns>
    public virtual MahjongManager.NakiKinds Naki(MahjongManager.PaiKinds _sutePai, int[] _tehaiInformationList, List<PlayerTehai.MentsuStatusForTehai> nakiList,
        MahjongManager.PlayerKind myPlayer, MahjongManager.PlayerKind _sutePlayer)
    {
        return MahjongManager.NakiKinds.Through;
    }

    /// <summary>
    /// 関数名を返すゲッター
    /// </summary>
    /// <returns></returns>
    public string GetName()
    {
        return myName;
    }

}
