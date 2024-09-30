using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cpu_MentsuTeForShantenSuu : CpuParent
{
    protected new void Start()
    {
        myName = GetType().Name;
    }

    public override MahjongManager.PaiKinds Sute(MahjongManager.PaiKinds _tsumoPai, int[] _tehaiInformation, List<PlayerTehai.MentsuStatusForTehai> nakiList)
    {
        MahjongManager.PaiKinds result = MahjongManager.PaiKinds.None_00;

        int highPoint = -1;

        int toitsuCounter = 0;

        List<MahjongManager.PaiKinds> koritsuList = MahjongManager.Instance.GetKoritsuPaiList(_tehaiInformation);

        //Debug.Log($"Sute : koritsuList.Count = {koritsuList.Count}\n{string.Join(",", koritsuList)}");

        if (koritsuList.Count == 0) //�Ǘ��v���Ȃ��Ƃ�
        {
            // �����ږ������v�Z����
            int[] paiLookCounter = new int[38];
            paiLookCounter = MahjongManager.Instance.GetLookPaiCount();
            for (int i = 0; i < _tehaiInformation.Length; i++)
            {
                if (_tehaiInformation[i] > 0)
                {
                    paiLookCounter[i] += _tehaiInformation[i];
                }
                if (_tehaiInformation[i] == 2)
                {
                    toitsuCounter++;
                }
            }

            for (int i = 0; i < _tehaiInformation.Length; i++)
            {
                if (_tehaiInformation[i] > 0)
                {
                    int thisPoint = ReturnBackPriorityPointForSute((MahjongManager.PaiKinds)i, _tehaiInformation, paiLookCounter ,toitsuCounter);
                    if (thisPoint > highPoint)
                    {
                        highPoint = thisPoint;
                        result = (MahjongManager.PaiKinds)i;
                    }
                }
            }
        }
        else //�Ǘ��v������Ƃ�
        {
            foreach (var i in koritsuList)
            {
                int thisPoint = ReturnBackPriorityPointForKoritsuPai(i);
                if (thisPoint > highPoint)
                {
                    highPoint = thisPoint;
                    result = i;
                }
            }
        }

        if (result == _tsumoPai) // ��؂肹���A�c���؂�ɂ���
        {
            result = MahjongManager.PaiKinds.None_00;
        }

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
        //bool flgKan = MahjongManager.Instance.GetPlayerTehaiComponent(myPlayer).GetAbleDaiminkan(_sutePai);

        if (!(flgChiL || flgChiM || flgChiH || flgPon)) return result;

        int[] copyList = new int[_tehaiInformationList.Length];
        Array.Copy(_tehaiInformationList, copyList, _tehaiInformationList.Length);
        copyList[(int)_sutePai] += 1;

        int beforeShanten = MahjongManager.Instance.GetShantenSuu(_tehaiInformationList);
        int afterShanten = MahjongManager.Instance.GetShantenSuu(copyList);

        if (beforeShanten <= afterShanten) return result;

        if (flgPon)
        {
            Array.Copy(_tehaiInformationList, copyList, _tehaiInformationList.Length);
            copyList[(int)_sutePai] -= 2;

            int nakiShanten = MahjongManager.Instance.GetShantenSuu(copyList);

            if(nakiShanten == afterShanten)
            {
                result = MahjongManager.NakiKinds.Pon;
            }
        }

        if(result == MahjongManager.NakiKinds.Through)
        {
            int counter = 0;
            if (flgChiL) counter++;
            if (flgChiM) counter++;
            if (flgChiH) counter++;

            if(counter == 0)
            {
                result = MahjongManager.NakiKinds.Through;
            }
            else if(counter == 1)
            {
                if (flgChiL) result = MahjongManager.NakiKinds.ChiNumLow;
                else if (flgChiM) result = MahjongManager.NakiKinds.ChiNumMiddle;
                else if (flgChiH) result = MahjongManager.NakiKinds.ChiNumHigh;
                else result = MahjongManager.NakiKinds.Through;
            }
            else
            {
                if (flgChiL)
                {
                    Array.Copy(_tehaiInformationList, copyList, _tehaiInformationList.Length);
                    copyList[(int)_sutePai + 1] -= 1;
                    copyList[(int)_sutePai + 2] -= 1;

                    int nakiShanten = MahjongManager.Instance.GetShantenSuu(copyList);

                    if (nakiShanten != afterShanten)
                    {
                        flgChiL = false;
                    }
                }
                if (flgChiM)
                {
                    Array.Copy(_tehaiInformationList, copyList, _tehaiInformationList.Length);
                    copyList[(int)_sutePai + 1] -= 1;
                    copyList[(int)_sutePai - 1] -= 1;

                    int nakiShanten = MahjongManager.Instance.GetShantenSuu(copyList);

                    if (nakiShanten != afterShanten)
                    {
                        flgChiM = false;
                    }
                }
                if (flgChiH)
                {
                    Array.Copy(_tehaiInformationList, copyList, _tehaiInformationList.Length);
                    copyList[(int)_sutePai - 1] -= 1;
                    copyList[(int)_sutePai - 2] -= 1;

                    int nakiShanten = MahjongManager.Instance.GetShantenSuu(copyList);

                    if (nakiShanten != afterShanten)
                    {
                        flgChiH = false;
                    }
                }

                counter = 0;
                if (flgChiL) counter++;
                if (flgChiM) counter++;
                if (flgChiH) counter++;

                if (counter == 0)
                {
                    result = MahjongManager.NakiKinds.Through;
                }
                else if (counter == 1)
                {
                    if (flgChiL) result = MahjongManager.NakiKinds.ChiNumLow;
                    else if (flgChiM) result = MahjongManager.NakiKinds.ChiNumMiddle;
                    else if (flgChiH) result = MahjongManager.NakiKinds.ChiNumHigh;
                    else result = MahjongManager.NakiKinds.Through;
                }
                else
                {
                    Debug.Log($"Naki : Final Jugde , {_sutePai}\nflgChiL = {flgChiL} , flgChiM = {flgChiM} , flgChiH = {flgChiH}");
                    int num = (int)_sutePai % 10;

                    if (num == 2 || num == 8)
                    {
                        if (flgChiM) result = MahjongManager.NakiKinds.ChiNumMiddle;
                    }
                    else if (num == 3)
                    {
                        if (flgChiH) result = MahjongManager.NakiKinds.ChiNumHigh;
                        else if (flgChiM) result = MahjongManager.NakiKinds.ChiNumMiddle;
                    }
                    else if (num == 7)
                    {
                        if (flgChiL) result = MahjongManager.NakiKinds.ChiNumLow;
                        else if (flgChiM) result = MahjongManager.NakiKinds.ChiNumMiddle;
                    }
                    else
                    {
                        if (flgChiM) result = MahjongManager.NakiKinds.ChiNumMiddle;
                        else if (flgChiL) result = MahjongManager.NakiKinds.ChiNumLow;
                        else if (flgChiH) result = MahjongManager.NakiKinds.ChiNumHigh;
                    }
                }
            }
        }

        //Debug.Log($"Cpu_JiHashiNaka.Naki : result = {result}");

        return result;
    }

    /// <summary>
    /// �v���l���Đ؂邽�߂ɕ]���l�����߂ĕԂ�
    /// </summary>
    /// <param name="_paiKind"></param>
    /// <param name="_tehaiInformation"></param>
    /// <param name="_paiLookCounter"></param>
    /// <returns>�l�������ق����̂Ă���ɂȂ�</returns>
    private int ReturnBackPriorityPointForSute(MahjongManager.PaiKinds _paiKind, int[] _tehaiInformation, int[] _paiLookCounter ,int _toitsuCouner)
    {
        int thisPaiIndex = (int)_paiKind;

        int[] copyList = new int[_tehaiInformation.Length];
        Array.Copy(_tehaiInformation, copyList, _tehaiInformation.Length);

        copyList[thisPaiIndex] -= 1; // �Y���̔v��؂����ۂ��l����

        int shantenSuu = MahjongManager.Instance.GetShantenSuu(copyList);

        int point = 100000 - (shantenSuu * 10000);

        if(shantenSuu == 0) // �e���p�C���Ă���Ƃ�
        {
            List<MahjongManager.PaiKinds> machiList = new List<MahjongManager.PaiKinds>();
            machiList = MahjongManager.Instance.GetAgariMachiPaiList(copyList);

            int allNum = machiList.Count * 4; //�҂��̍ő喇��
            int lookMachiTotal = 0; //�҂��̌����ږ����̍��v
            foreach (var item in machiList)
            {
                lookMachiTotal += _paiLookCounter[(int)item];
            }
            int machiCount = allNum - lookMachiTotal; //�c���Ă���҂��̖���

            point += machiCount * 1000; // �҂��������ق������|�C���g

            int num = thisPaiIndex % 10;
            if (num > 5) num = 10 - num;
            point += 10 - num; // �[�v�̂ق������|�C���g
        }
        else if (shantenSuu < 0)
        {
            Debug.LogError($"ReturnBackPriorityPointForSute : Error , {_paiKind} -> shantenSuu = {shantenSuu}");
        }
        else
        {
            point += ReturnBackPriorityPointForKanrenPai(_paiKind, _tehaiInformation, _paiLookCounter, _toitsuCouner);
        }

        Debug.Log($"ReturnBackPriorityPointForSute : {_paiKind} -> shantenSuu = {shantenSuu} , point = {point}");

        return point;
    }

    /// <summary>
    /// �Ǘ��v�̉��l��Ԃ�
    /// </summary>
    /// <param name="_paiKind"></param>
    /// <returns></returns>
    private int ReturnBackPriorityPointForKoritsuPai(MahjongManager.PaiKinds _paiKind)
    {
        int thisPaiIndex = (int)_paiKind;

        int paiValue = 0; // �����ق����̂Ă�ׂ� (���v�������A5��������)

        if (_paiKind >= MahjongManager.PaiKinds.J1)
        {
            paiValue = thisPaiIndex % 10 + 1000;
        }
        else
        {
            int num = thisPaiIndex % 10;
            if (num > 5) num = 10 - num;
            paiValue = 100 - num;
        }

        return paiValue;
    }

    /// <summary>
    /// �Ǘ��łȂ��v�̉��l��Ԃ�
    /// </summary>
    /// <param name="_paiKind"></param>
    /// <param name="_tehaiInformation"></param>
    /// <returns></returns>
    private int ReturnBackPriorityPointForKanrenPai(MahjongManager.PaiKinds _paiKind, int[] _tehaiInformation, int[] _paiLookCounter , int _toitsuCounter)
    {
        int thisPaiIndex = (int)_paiKind;

        int paiValue = 0; // �����ق����̂Ă�ׂ�

        if (_paiKind >= MahjongManager.PaiKinds.J1)
        {
            if (_tehaiInformation[thisPaiIndex] == 2)
            {
                paiValue = (int)Mathf.Pow(10, _paiLookCounter[thisPaiIndex] - 2); // 100 or 10 or 1
            }
            else if (_tehaiInformation[thisPaiIndex] == 3)
            {
                paiValue = 0;
            }
            else
            {
                paiValue = 200;
                Debug.Log($"ReturnBackPriorityPointForKanrenPai : Why?\n[ {_paiKind} ] have {_tehaiInformation[thisPaiIndex]} , look {_paiLookCounter[thisPaiIndex]}");
            }
        }
        else
        {
            if (_tehaiInformation[thisPaiIndex] >= 3) // �Y���̔v��3���ȏ㎝���Ă���
            {
                paiValue = 0;
            }
            else
            {
                int num = thisPaiIndex % 10;
                if (num > 5) num = 10 - num;
                paiValue = 20 - num; // 19 �` 15 

                if (_tehaiInformation[thisPaiIndex] == 1) // �Y���̔v��1�������Ă���
                {
                    if(_tehaiInformation[thisPaiIndex - 1] == 0 && _tehaiInformation[thisPaiIndex + 1] == 0)
                    {
                        paiValue += 50;
                    }
                }
                else // �Y���̔v��2�������Ă���
                {
                    if (_toitsuCounter == 1)
                    {
                        paiValue -= 10;
                    }
                }
            }
        }

        return paiValue;
    }

}
