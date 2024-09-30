using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cpu_ShantenByShanten : CpuParent
{
    private const int NUM_PAI_ARRAY = 38;
    private const int NUM_POINT_MIN = -1;

    protected new void Start()
    {
        myName = GetType().Name;
    }

    public override MahjongManager.PaiKinds Sute(MahjongManager.PaiKinds _tsumoPai, int[] _tehaiInformation, List<PlayerTehai.MentsuStatusForTehai> nakiList)
    {
        MahjongManager.PaiKinds result = MahjongManager.PaiKinds.None_00;

        int highPoint = NUM_POINT_MIN;

        int nowShanten = MahjongManager.Instance.GetShantenSuu(_tehaiInformation);

        List<MahjongManager.PaiKinds> koritsuList = MahjongManager.Instance.GetKoritsuPaiList(_tehaiInformation);
        int toitsuCounter = 0;

        // Œ©‚½–Ú–‡”‚ğŒvZ‚·‚é
        int[] paiLookCounter = new int[NUM_PAI_ARRAY];
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
            int indexPai = i + 30;
            if (indexPai >= NUM_PAI_ARRAY) indexPai -= NUM_PAI_ARRAY;
            if (indexPai % 10 == 0) continue;
            if (_tehaiInformation[indexPai] <= 0) continue;

            int[] copyList = new int[_tehaiInformation.Length];
            Array.Copy(_tehaiInformation, copyList, _tehaiInformation.Length);
            copyList[indexPai] -= 1;
            int shantenSuu = MahjongManager.Instance.GetShantenSuu(copyList);

            if (nowShanten != shantenSuu) continue;

            int point = ReturnBackPriorityPointForSute((MahjongManager.PaiKinds)indexPai, _tehaiInformation, paiLookCounter, koritsuList, toitsuCounter);

            if (point > highPoint)
            {
                highPoint = point;
                result = (MahjongManager.PaiKinds)indexPai;
            }
        }

        if (result == _tsumoPai) // ‹óØ‚è‚¹‚¸Aƒcƒ‚Ø‚è‚É‚·‚é
        {
            result = MahjongManager.PaiKinds.None_00;
        }

        return result;
    }

    public override MahjongManager.NakiKinds Naki(MahjongManager.PaiKinds _sutePai, int[] _tehaiInformation, List<PlayerTehai.MentsuStatusForTehai> nakiList,
        MahjongManager.PlayerKind myPlayer, MahjongManager.PlayerKind _sutePlayer)
    {
        MahjongManager.NakiKinds result = MahjongManager.NakiKinds.Through;

        bool flgChiL = MahjongManager.Instance.GetPlayerTehaiComponent(myPlayer).GetAbleChiLow(_sutePai, _sutePlayer);
        bool flgChiM = MahjongManager.Instance.GetPlayerTehaiComponent(myPlayer).GetAbleChiMid(_sutePai, _sutePlayer);
        bool flgChiH = MahjongManager.Instance.GetPlayerTehaiComponent(myPlayer).GetAbleChiHigh(_sutePai, _sutePlayer);
        bool flgPon = MahjongManager.Instance.GetPlayerTehaiComponent(myPlayer).GetAblePon(_sutePai);
        //bool flgKan = MahjongManager.Instance.GetPlayerTehaiComponent(myPlayer).GetAbleDaiminkan(_sutePai);

        if (!(flgChiL || flgChiM || flgChiH || flgPon)) return result;

        int[] copyList = new int[_tehaiInformation.Length];
        Array.Copy(_tehaiInformation, copyList, _tehaiInformation.Length);
        copyList[(int)_sutePai] += 1;

        int beforeShanten = MahjongManager.Instance.GetShantenSuu(_tehaiInformation);
        int afterShanten = MahjongManager.Instance.GetShantenSuu(copyList);

        if (beforeShanten <= afterShanten) return result;

        int nakiFlgCounter = 0;
        if (flgPon) nakiFlgCounter++;
        if (flgChiL) nakiFlgCounter++;
        if (flgChiM) nakiFlgCounter++;
        if (flgChiH) nakiFlgCounter++;

        if (nakiFlgCounter == 0)
        {
            result = MahjongManager.NakiKinds.Through;
            Debug.LogError($"Sute : Error , nakiFlgCounter = {nakiFlgCounter}");
        }
        else if (nakiFlgCounter == 1)
        {
            if (flgPon) result = MahjongManager.NakiKinds.Pon;
            else if (flgChiL) result = MahjongManager.NakiKinds.ChiNumLow;
            else if (flgChiM) result = MahjongManager.NakiKinds.ChiNumMiddle;
            else if (flgChiH) result = MahjongManager.NakiKinds.ChiNumHigh;
        }
        else
        {
            if (flgPon)
            {
                int[] copycopyList = new int[copyList.Length];
                Array.Copy(copyList, copycopyList, copyList.Length);
                copycopyList[(int)_sutePai] -= 3;

                flgPon = afterShanten == MahjongManager.Instance.GetShantenSuu(copycopyList) ? true : false;
            }
            if (flgChiL)
            {
                int[] copycopyList = new int[copyList.Length];
                Array.Copy(copyList, copycopyList, copyList.Length);
                copycopyList[(int)(_sutePai)] -= 1;
                copycopyList[(int)(_sutePai + 1)] -= 1;
                copycopyList[(int)(_sutePai + 2)] -= 1;
                flgChiL = afterShanten == MahjongManager.Instance.GetShantenSuu(copycopyList) ? true : false;
            }
            if (flgChiM)
            {
                int[] copycopyList = new int[copyList.Length];
                Array.Copy(copyList, copycopyList, copyList.Length);
                copycopyList[(int)(_sutePai - 1)] -= 1;
                copycopyList[(int)(_sutePai)] -= 1;
                copycopyList[(int)(_sutePai + 1)] -= 1;
                flgChiM = afterShanten == MahjongManager.Instance.GetShantenSuu(copycopyList) ? true : false;
            }
            if (flgChiH)
            {
                int[] copycopyList = new int[copyList.Length];
                Array.Copy(copyList, copycopyList, copyList.Length);
                copycopyList[(int)(_sutePai - 2)] -= 1;
                copycopyList[(int)(_sutePai - 1)] -= 1;
                copycopyList[(int)(_sutePai)] -= 1;
                flgChiH = afterShanten == MahjongManager.Instance.GetShantenSuu(copycopyList) ? true : false;
            }

            nakiFlgCounter = 0;
            if (flgPon) nakiFlgCounter++;
            if (flgChiL) nakiFlgCounter++;
            if (flgChiM) nakiFlgCounter++;
            if (flgChiH) nakiFlgCounter++;

            if (nakiFlgCounter == 0)
            {
                result = MahjongManager.NakiKinds.Through;
                Debug.LogError($"Sute : Error , nakiFlgCounter = {nakiFlgCounter}");
            }
            else if (nakiFlgCounter == 1)
            {
                if (flgPon) result = MahjongManager.NakiKinds.Pon;
                else if (flgChiL) result = MahjongManager.NakiKinds.ChiNumLow;
                else if (flgChiM) result = MahjongManager.NakiKinds.ChiNumMiddle;
                else if (flgChiH) result = MahjongManager.NakiKinds.ChiNumHigh;
            }
            else
            {
                if (flgPon) result = MahjongManager.NakiKinds.Pon;
                else
                {
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

        return result;
    }

    /// <summary>
    /// ”v‚ğl‚¦‚ÄØ‚é‚½‚ß‚É•]‰¿’l‚ğŒˆ‚ß‚Ä•Ô‚·
    /// </summary>
    /// <param name="_paiKind"></param>
    /// <param name="_tehaiInformation"></param>
    /// <param name="_paiLookCounter"></param>
    /// <returns>’l‚ª‚‚¢‚Ù‚¤‚ªÌ‚Ä‚éŒó•â‚É‚È‚é</returns>
    private int ReturnBackPriorityPointForSute(MahjongManager.PaiKinds _paiKind, int[] _tehaiInformation,
        int[] _paiLookCounter, List<MahjongManager.PaiKinds> _koritsuList, int _toitsuCounter)
    {
        int point = 0;
        int indexPai = (int)_paiKind;

        int[] copyList = new int[_tehaiInformation.Length];
        Array.Copy(_tehaiInformation, copyList, _tehaiInformation.Length);
        copyList[indexPai] -= 1;
        int shantenSuu = MahjongManager.Instance.GetShantenSuu(copyList);

        int paiKindPoint = 0;
        if (_koritsuList.Count != 0) //ŒÇ—§”v‚ª‚ ‚é‚Æ‚«
        {
            bool iAmKoritsuPaiFlg = false;
            foreach (var item in _koritsuList)
            {
                if (item == (MahjongManager.PaiKinds)indexPai)
                {
                    iAmKoritsuPaiFlg |= true;
                    break;
                }
            }

            if (iAmKoritsuPaiFlg)
            {
                paiKindPoint = ReturnBackPriorityPointForKoritsuPai((MahjongManager.PaiKinds)indexPai);
                point += paiKindPoint;
            }
        }
        else //ŒÇ—§”v‚ª‚È‚¢‚Æ‚«
        {
            int yuukouPaiCount = 0;
            for (int j = 0; j < NUM_PAI_ARRAY; j++)
            {
                if (j % 10 == 0) continue;
                if (_paiLookCounter[j] >= 4) continue;
                int[] copyCopyList = new int[copyList.Length];
                Array.Copy(copyList, copyCopyList, copyList.Length);
                copyCopyList[j] += 1;
                int checkShantensuu = MahjongManager.Instance.GetShantenSuu(copyCopyList);
                if (checkShantensuu < shantenSuu)
                {
                    yuukouPaiCount += (4 - _paiLookCounter[j]);
                }
            }

            point += yuukouPaiCount * 1000;

            paiKindPoint = ReturnBackPriorityPointForKanrenPai((MahjongManager.PaiKinds)indexPai, _tehaiInformation, _paiLookCounter, _toitsuCounter);
            point += paiKindPoint;
        }

        return point;
    }

    /// <summary>
    /// ŒÇ—§”v‚Ì‰¿’l‚ğ•Ô‚·
    /// </summary>
    /// <param name="_paiKind"></param>
    /// <returns></returns>
    private int ReturnBackPriorityPointForKoritsuPai(MahjongManager.PaiKinds _paiKind)
    {
        int thisPaiIndex = (int)_paiKind;

        int paiValue = 0; // ‚‚¢‚Ù‚¤‚ªÌ‚Ä‚é‚×‚« (š”v‚ª‚‚­A5‚ªˆø‚­‚¢)

        if (_paiKind >= MahjongManager.PaiKinds.J5)
        {
            paiValue = 7;
        }
        else if (_paiKind >= MahjongManager.PaiKinds.J1)
        {
            paiValue = 8;
        }
        else
        {
            int num = thisPaiIndex % 10;
            if (num > 5) num = 10 - num;
            paiValue = 6 - num;
        }

        return paiValue;
    }

    /// <summary>
    /// ŒÇ—§‚Å‚È‚¢”v‚Ì‰¿’l‚ğ•Ô‚·
    /// </summary>
    /// <param name="_paiKind"></param>
    /// <param name="_tehaiInformation"></param>
    /// <returns></returns>
    private int ReturnBackPriorityPointForKanrenPai(MahjongManager.PaiKinds _paiKind, int[] _tehaiInformation, int[] _paiLookCounter, int _toitsuCounter)
    {
        int thisPaiIndex = (int)_paiKind;

        int paiValue = 0; // ‚‚¢‚Ù‚¤‚ªÌ‚Ä‚é‚×‚«

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
            if (_tehaiInformation[thisPaiIndex] >= 3) // ŠY“–‚Ì”v‚ğ3–‡ˆÈã‚Á‚Ä‚¢‚é
            {
                paiValue = 0;
            }
            else
            {
                int num = thisPaiIndex % 10;
                if (num > 5) num = 10 - num;
                paiValue = 20 - num; // 19 ` 15 

                if (_tehaiInformation[thisPaiIndex] == 1) // ŠY“–‚Ì”v‚ğ1–‡‚Á‚Ä‚¢‚é
                {
                    if (_tehaiInformation[thisPaiIndex - 1] == 0 && _tehaiInformation[thisPaiIndex + 1] == 0)
                    {
                        paiValue += 50;
                    }
                }
                else // ŠY“–‚Ì”v‚ğ2–‡‚Á‚Ä‚¢‚é
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
