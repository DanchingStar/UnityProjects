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

        if (koritsuList.Count == 0) //孤立牌がないとき
        {
            // 見た目枚数を計算する
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
        else //孤立牌があるとき
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

        if (result == _tsumoPai) // 空切りせず、ツモ切りにする
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
    /// 牌を考えて切るために評価値を決めて返す
    /// </summary>
    /// <param name="_paiKind"></param>
    /// <param name="_tehaiInformation"></param>
    /// <param name="_paiLookCounter"></param>
    /// <returns>値が高いほうが捨てる候補になる</returns>
    private int ReturnBackPriorityPointForSute(MahjongManager.PaiKinds _paiKind, int[] _tehaiInformation, int[] _paiLookCounter ,int _toitsuCouner)
    {
        int thisPaiIndex = (int)_paiKind;

        int[] copyList = new int[_tehaiInformation.Length];
        Array.Copy(_tehaiInformation, copyList, _tehaiInformation.Length);

        copyList[thisPaiIndex] -= 1; // 該当の牌を切った際を考える

        int shantenSuu = MahjongManager.Instance.GetShantenSuu(copyList);

        int point = 100000 - (shantenSuu * 10000);

        if(shantenSuu == 0) // テンパイしているとき
        {
            List<MahjongManager.PaiKinds> machiList = new List<MahjongManager.PaiKinds>();
            machiList = MahjongManager.Instance.GetAgariMachiPaiList(copyList);

            int allNum = machiList.Count * 4; //待ちの最大枚数
            int lookMachiTotal = 0; //待ちの見た目枚数の合計
            foreach (var item in machiList)
            {
                lookMachiTotal += _paiLookCounter[(int)item];
            }
            int machiCount = allNum - lookMachiTotal; //残っている待ちの枚数

            point += machiCount * 1000; // 待ちが多いほうが高ポイント

            int num = thisPaiIndex % 10;
            if (num > 5) num = 10 - num;
            point += 10 - num; // 端牌のほうが高ポイント
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
    /// 孤立牌の価値を返す
    /// </summary>
    /// <param name="_paiKind"></param>
    /// <returns></returns>
    private int ReturnBackPriorityPointForKoritsuPai(MahjongManager.PaiKinds _paiKind)
    {
        int thisPaiIndex = (int)_paiKind;

        int paiValue = 0; // 高いほうが捨てるべき (字牌が高く、5が引くい)

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
    /// 孤立でない牌の価値を返す
    /// </summary>
    /// <param name="_paiKind"></param>
    /// <param name="_tehaiInformation"></param>
    /// <returns></returns>
    private int ReturnBackPriorityPointForKanrenPai(MahjongManager.PaiKinds _paiKind, int[] _tehaiInformation, int[] _paiLookCounter , int _toitsuCounter)
    {
        int thisPaiIndex = (int)_paiKind;

        int paiValue = 0; // 高いほうが捨てるべき

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
            if (_tehaiInformation[thisPaiIndex] >= 3) // 該当の牌を3枚以上持っている
            {
                paiValue = 0;
            }
            else
            {
                int num = thisPaiIndex % 10;
                if (num > 5) num = 10 - num;
                paiValue = 20 - num; // 19 〜 15 

                if (_tehaiInformation[thisPaiIndex] == 1) // 該当の牌を1枚持っている
                {
                    if(_tehaiInformation[thisPaiIndex - 1] == 0 && _tehaiInformation[thisPaiIndex + 1] == 0)
                    {
                        paiValue += 50;
                    }
                }
                else // 該当の牌を2枚持っている
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
