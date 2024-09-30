using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cpu_Kokushimusou : CpuParent
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
                int thisPoint = ReturnBackPriorityPointForSute((MahjongManager.PaiKinds)i, _tehaiInformation);
                if (thisPoint > highPoint)
                {
                    highPoint = thisPoint;
                    result = (MahjongManager.PaiKinds)i;
                }
            }
        }

        if (result == _tsumoPai) // ‹óØ‚è‚¹‚¸Aƒcƒ‚Ø‚è‚É‚·‚é
        {
            result = MahjongManager.PaiKinds.None_00;
        }

        return result;
    }

    /// <summary>
    /// ”v‚ğl‚¦‚ÄØ‚é‚½‚ß‚É•]‰¿’l‚ğŒˆ‚ß‚Ä•Ô‚·
    /// </summary>
    /// <param name="_paiKind"></param>
    /// <param name="_tehaiInformation"></param>
    /// <returns>’l‚ª‚‚¢‚Ù‚¤‚ªÌ‚Ä‚éŒó•â‚É‚È‚é</returns>
    private int ReturnBackPriorityPointForSute(MahjongManager.PaiKinds _paiKind, int[] _tehaiInformation)
    {
        int point = 0;

        int thisPaiIndex = (int)_paiKind;
        int thisPaiCount = _tehaiInformation[thisPaiIndex];

        int paiValue = 0; // š”v‚ª’á‚­A5‚ª‚‚¢

        if (_paiKind >= MahjongManager.PaiKinds.J1)
        {
            paiValue = 1;
        }
        else
        {
            int num = thisPaiIndex % 10;
            if (num > 5) num = 10 - num;
            paiValue = num == 1 ? 10 : num * 1000;
        }

        if (thisPaiCount == 1) // è”v‚É1–‡‚ ‚é
        {
            point = paiValue;
        }
        else // è”v‚É2–‡ˆÈã‚ ‚é
        {
            point = 100 * thisPaiCount + paiValue;
        }

        return point;
    }
}
