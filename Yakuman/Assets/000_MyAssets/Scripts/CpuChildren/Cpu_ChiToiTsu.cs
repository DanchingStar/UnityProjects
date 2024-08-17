using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cpu_ChiToiTsu : CpuParent
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

        if(result == _tsumoPai) // ��؂肹���A�c���؂�ɂ���
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
    private int ReturnBackPriorityPointForSute(MahjongManager.PaiKinds _paiKind , int[] _tehaiInformation)
    {
        int point = 0;

        int thisPaiIndex = (int)_paiKind;
        int thisPaiCount = _tehaiInformation[thisPaiIndex];

        if (thisPaiCount == 2) // ��v��2�����鎞
        {
            point = 1;
        }
        else
        {
            int paiValue = 0; // ���v���Ⴍ�A5������

            if (_paiKind >= MahjongManager.PaiKinds.J1)
            {
                paiValue = thisPaiIndex % 10;
            }
            else
            {
                int num = thisPaiIndex % 10;
                if (num > 5) num = 10 - num;
                paiValue = num * 10;
            }

            if (thisPaiCount < 2) // ��v��1�����鎞
            {
                int thisPaiLookCount = MahjongManager.Instance.GetLookPaiCount(_paiKind) + thisPaiCount; // ��Ǝ�v�Ɍ����Ă��閇��

                if (thisPaiLookCount > 4) Debug.LogWarning($"ReturnBackPriorityPointForSute : {_paiKind} thisPaiLookCount = {thisPaiLookCount}");
                point = (thisPaiLookCount * 100) + (2 * paiValue);
            }
            else // ��v��3���ȏ゠�鎞
            {
                point = 1000 * thisPaiCount + paiValue;
            }
        }

        return point;
    }
}
