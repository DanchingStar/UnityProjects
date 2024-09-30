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

        if (result == _tsumoPai) // ��؂肹���A�c���؂�ɂ���
        {
            result = MahjongManager.PaiKinds.None_00;
        }

        return result;
    }

    /// <summary>
    /// �v���l���Đ؂邽�߂ɕ]���l�����߂ĕԂ�
    /// </summary>
    /// <param name="_paiKind"></param>
    /// <param name="_tehaiInformation"></param>
    /// <returns>�l�������ق����̂Ă���ɂȂ�</returns>
    private int ReturnBackPriorityPointForSute(MahjongManager.PaiKinds _paiKind, int[] _tehaiInformation)
    {
        int point = 0;

        int thisPaiIndex = (int)_paiKind;
        int thisPaiCount = _tehaiInformation[thisPaiIndex];

        int paiValue = 0; // ���v���Ⴍ�A5������

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

        if (thisPaiCount == 1) // ��v��1�����鎞
        {
            point = paiValue;
        }
        else // ��v��2���ȏ゠�鎞
        {
            point = 100 * thisPaiCount + paiValue;
        }

        return point;
    }
}
