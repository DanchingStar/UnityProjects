using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeAttackContents : MonoBehaviour
{

    /// <summary>
    /// 配牌サポートする牌種のリストを返す
    /// </summary>
    /// <param name="_supportSetForTimeAttack"></param>
    /// <returns></returns>
    public List<MahjongManager.PaiKinds> GetSupportList(MahjongManager.SupportSetForTimeAttack _supportSetForTimeAttack)
    {
        List<MahjongManager.PaiKinds> resultList = new List<MahjongManager.PaiKinds>();

        switch (_supportSetForTimeAttack.yaku)
        {
            case MahjongManager.SupportSetForTimeAttack.Yaku.SuAnKo:
                {
                    List<MahjongManager.PaiKinds> kouhoList = new List<MahjongManager.PaiKinds>();
                    while (kouhoList.Count < 4)
                    {
                        MahjongManager.PaiKinds kouho = RandomPaiKind(_supportSetForTimeAttack.yaku);
                        bool flg = false;
                        foreach (var item in kouhoList)
                        {
                            if (item == kouho) 
                            {
                                flg = true;
                                break;
                            }                            
                        }
                        if (!flg)kouhoList.Add(kouho);
                    }

                    resultList.Add(kouhoList[0]);
                    resultList.Add(kouhoList[0]);
                    resultList.Add(kouhoList[0]);
                    resultList.Add(kouhoList[1]);
                    resultList.Add(kouhoList[1]);
                    resultList.Add(kouhoList[1]);
                    resultList.Add(kouhoList[2]);
                    resultList.Add(kouhoList[2]);

                    switch (_supportSetForTimeAttack.rank)
                    {
                        case MahjongManager.SupportSetForTimeAttack.Rank.A:
                            {
                                resultList.Add(kouhoList[2]);
                                resultList.Add(kouhoList[3]);
                                resultList.Add(kouhoList[3]);
                            }
                            break;
                        case MahjongManager.SupportSetForTimeAttack.Rank.B:
                            {
                                resultList.Add(kouhoList[2]);
                            }
                            break;
                        case MahjongManager.SupportSetForTimeAttack.Rank.C:
                            {
                                resultList.Add(kouhoList[3]);
                                resultList.Add(kouhoList[3]);
                            }
                            break;
                    }
                }
                break;
            case MahjongManager.SupportSetForTimeAttack.Yaku.SuKanTsu:
                {
                    List<MahjongManager.PaiKinds> kouhoList = new List<MahjongManager.PaiKinds>();
                    while (kouhoList.Count < 4)
                    {
                        MahjongManager.PaiKinds kouho = RandomPaiKind(_supportSetForTimeAttack.yaku);
                        bool flg = false;
                        foreach (var item in kouhoList)
                        {
                            if (item == kouho)
                            {
                                flg = true;
                                break;
                            }
                        }
                        if (!flg) kouhoList.Add(kouho);
                    }

                    resultList.Add(kouhoList[0]);
                    resultList.Add(kouhoList[1]);
                    resultList.Add(kouhoList[1]);
                    resultList.Add(kouhoList[1]);
                    resultList.Add(kouhoList[1]);
                    resultList.Add(kouhoList[2]);
                    resultList.Add(kouhoList[2]);
                    resultList.Add(kouhoList[2]);
                    resultList.Add(kouhoList[2]);
                    resultList.Add(kouhoList[3]);
                    resultList.Add(kouhoList[3]);

                    switch (_supportSetForTimeAttack.rank)
                    {
                        case MahjongManager.SupportSetForTimeAttack.Rank.A:
                            {
                                resultList.Add(kouhoList[0]);
                                resultList.Add(kouhoList[3]);
                            }
                            break;
                        case MahjongManager.SupportSetForTimeAttack.Rank.B:
                            {
                                resultList.Add(kouhoList[3]);
                            }
                            break;
                        case MahjongManager.SupportSetForTimeAttack.Rank.C:
                            break;
                    }
                }
                break;
            case MahjongManager.SupportSetForTimeAttack.Yaku.DaiSanGen:
                {
                    resultList.Add(MahjongManager.PaiKinds.J5);
                    resultList.Add(MahjongManager.PaiKinds.J5);
                    resultList.Add(MahjongManager.PaiKinds.J6);
                    resultList.Add(MahjongManager.PaiKinds.J6);
                    resultList.Add(MahjongManager.PaiKinds.J7);
                    resultList.Add(MahjongManager.PaiKinds.J7);

                    switch (_supportSetForTimeAttack.rank)
                    {
                        case MahjongManager.SupportSetForTimeAttack.Rank.A:
                            {
                                resultList.Add(MahjongManager.PaiKinds.J5);
                                resultList.Add(MahjongManager.PaiKinds.J6);
                                resultList.Add(MahjongManager.PaiKinds.J7);
                            }
                            break;
                        case MahjongManager.SupportSetForTimeAttack.Rank.B:
                            {
                                resultList.Add((MahjongManager.PaiKinds)(Random.Range(0, 3) + 35));
                            }
                            break;
                        case MahjongManager.SupportSetForTimeAttack.Rank.C:
                            break;
                    }
                }
                break;
            case MahjongManager.SupportSetForTimeAttack.Yaku.SuShiHo:
                {
                    resultList.Add(MahjongManager.PaiKinds.J1);
                    resultList.Add(MahjongManager.PaiKinds.J1);
                    resultList.Add(MahjongManager.PaiKinds.J2);
                    resultList.Add(MahjongManager.PaiKinds.J2);
                    resultList.Add(MahjongManager.PaiKinds.J3);
                    resultList.Add(MahjongManager.PaiKinds.J3);
                    resultList.Add(MahjongManager.PaiKinds.J4);
                    resultList.Add(MahjongManager.PaiKinds.J4);

                    switch (_supportSetForTimeAttack.rank)
                    {
                        case MahjongManager.SupportSetForTimeAttack.Rank.A:
                            {
                                MahjongManager.PaiKinds kouhoA = (MahjongManager.PaiKinds)(Random.Range(0, 4) + 31);
                                MahjongManager.PaiKinds kouhoB = (MahjongManager.PaiKinds)(Random.Range(0, 4) + 31);
                                while (kouhoA == kouhoB) kouhoB = (MahjongManager.PaiKinds)(Random.Range(0, 4) + 31);
                                resultList.Add(kouhoA);
                                resultList.Add(kouhoB);
                            }
                            break;
                        case MahjongManager.SupportSetForTimeAttack.Rank.B:
                            {
                                resultList.Add((MahjongManager.PaiKinds)(Random.Range(0, 4) + 31));
                            }
                            break;
                        case MahjongManager.SupportSetForTimeAttack.Rank.C:
                            break;
                    }
                }
                break;
            case MahjongManager.SupportSetForTimeAttack.Yaku.TsuIiSo:
                {
                    List<MahjongManager.PaiKinds> kouhoList = new List<MahjongManager.PaiKinds>();
                    while (kouhoList.Count < 5)
                    {
                        MahjongManager.PaiKinds kouho = RandomPaiKind(_supportSetForTimeAttack.yaku);
                        bool flg = false;
                        foreach (var item in kouhoList)
                        {
                            if (item == kouho)
                            {
                                flg = true;
                                break;
                            }
                        }
                        if (!flg) kouhoList.Add(kouho);
                    }

                    resultList.Add(kouhoList[0]);
                    resultList.Add(kouhoList[0]);
                    resultList.Add(kouhoList[0]);
                    resultList.Add(kouhoList[1]);
                    resultList.Add(kouhoList[1]);
                    resultList.Add(kouhoList[2]);
                    resultList.Add(kouhoList[2]);
                    resultList.Add(kouhoList[3]);
                    resultList.Add(kouhoList[4]);

                    switch (_supportSetForTimeAttack.rank)
                    {
                        case MahjongManager.SupportSetForTimeAttack.Rank.A:
                            {
                                resultList.Add(kouhoList[3]);
                                resultList.Add(kouhoList[4]);
                            }
                            break;
                        case MahjongManager.SupportSetForTimeAttack.Rank.B:
                            {
                                resultList.Add(kouhoList[3]);
                            }
                            break;
                        case MahjongManager.SupportSetForTimeAttack.Rank.C:
                            break;
                    }
                }
                break;
            case MahjongManager.SupportSetForTimeAttack.Yaku.ChinRouTo:
                {
                    List<MahjongManager.PaiKinds> kouhoList = new List<MahjongManager.PaiKinds>();
                    while (kouhoList.Count < 5)
                    {
                        MahjongManager.PaiKinds kouho = RandomPaiKind(_supportSetForTimeAttack.yaku);
                        bool flg = false;
                        foreach (var item in kouhoList)
                        {
                            if (item == kouho)
                            {
                                flg = true;
                                break;
                            }
                        }
                        if (!flg) kouhoList.Add(kouho);
                    }

                    resultList.Add(kouhoList[0]);
                    resultList.Add(kouhoList[0]);
                    resultList.Add(kouhoList[0]);
                    resultList.Add(kouhoList[1]);
                    resultList.Add(kouhoList[1]);
                    resultList.Add(kouhoList[2]);
                    resultList.Add(kouhoList[2]);
                    resultList.Add(kouhoList[3]);
                    resultList.Add(kouhoList[4]);

                    switch (_supportSetForTimeAttack.rank)
                    {
                        case MahjongManager.SupportSetForTimeAttack.Rank.A:
                            {
                                resultList.Add(kouhoList[3]);
                                resultList.Add(kouhoList[4]);
                            }
                            break;
                        case MahjongManager.SupportSetForTimeAttack.Rank.B:
                            {
                                resultList.Add(kouhoList[3]);
                            }
                            break;
                        case MahjongManager.SupportSetForTimeAttack.Rank.C:
                            break;
                    }
                }
                break;
            case MahjongManager.SupportSetForTimeAttack.Yaku.RyuIiSo:
                {
                    int num = 0;
                    switch (_supportSetForTimeAttack.rank)
                    {
                        case MahjongManager.SupportSetForTimeAttack.Rank.A:
                            {
                                num = 11;
                            }
                            break;
                        case MahjongManager.SupportSetForTimeAttack.Rank.B:
                            {
                                num = 10;
                            }
                            break;
                        case MahjongManager.SupportSetForTimeAttack.Rank.C:
                            {
                                num = 9;
                            }
                            break;
                    }

                    while (resultList.Count < num)
                    {
                        MahjongManager.PaiKinds kouho = RandomPaiKind(_supportSetForTimeAttack.yaku);

                        int counter = 0;
                        foreach(var item in resultList)
                        {
                            if (kouho == item) counter++;
                        }
                        if (counter <= 3) resultList.Add(kouho);
                        else if(counter == 4)
                        {
                            if(kouho == MahjongManager.PaiKinds.S2 || kouho == MahjongManager.PaiKinds.S3 || kouho == MahjongManager.PaiKinds.S4)
                            {
                                resultList.Add(kouho);
                            }
                        }
                    }
                }
                break;
            case MahjongManager.SupportSetForTimeAttack.Yaku.KokuShiMuSou:
                {
                    int num = 0;
                    switch (_supportSetForTimeAttack.rank)
                    {
                        case MahjongManager.SupportSetForTimeAttack.Rank.A:
                            {
                                num = 1;
                            }
                            break;
                        case MahjongManager.SupportSetForTimeAttack.Rank.B:
                            {
                                num = 2;
                            }
                            break;
                        case MahjongManager.SupportSetForTimeAttack.Rank.C:
                            {
                                num = 3;
                            }
                            break;
                    }

                    resultList.Add(MahjongManager.PaiKinds.M1);
                    resultList.Add(MahjongManager.PaiKinds.M9);
                    resultList.Add(MahjongManager.PaiKinds.P1);
                    resultList.Add(MahjongManager.PaiKinds.P9);
                    resultList.Add(MahjongManager.PaiKinds.S1);
                    resultList.Add(MahjongManager.PaiKinds.S9);
                    resultList.Add(MahjongManager.PaiKinds.J1);
                    resultList.Add(MahjongManager.PaiKinds.J2);
                    resultList.Add(MahjongManager.PaiKinds.J3);
                    resultList.Add(MahjongManager.PaiKinds.J4);
                    resultList.Add(MahjongManager.PaiKinds.J5);
                    resultList.Add(MahjongManager.PaiKinds.J6);
                    resultList.Add(MahjongManager.PaiKinds.J7);

                    for(int i = 0; i < num; i++)
                    {
                        int removeNum = Random.Range(0, resultList.Count);
                        resultList.RemoveAt(removeNum);
                    }
                }
                break;
            case MahjongManager.SupportSetForTimeAttack.Yaku.ChuRenPoTo:
                {
                    int num = 0;
                    switch (_supportSetForTimeAttack.rank)
                    {
                        case MahjongManager.SupportSetForTimeAttack.Rank.A:
                            {
                                num = 1;
                            }
                            break;
                        case MahjongManager.SupportSetForTimeAttack.Rank.B:
                            {
                                num = 2;
                            }
                            break;
                        case MahjongManager.SupportSetForTimeAttack.Rank.C:
                            {
                                num = 3;
                            }
                            break;
                    }

                    int iroNum = Random.Range(0, 3);
                    resultList.Add((MahjongManager.PaiKinds)(iroNum * 10 + 1));
                    resultList.Add((MahjongManager.PaiKinds)(iroNum * 10 + 1));
                    resultList.Add((MahjongManager.PaiKinds)(iroNum * 10 + 1));
                    resultList.Add((MahjongManager.PaiKinds)(iroNum * 10 + 2));
                    resultList.Add((MahjongManager.PaiKinds)(iroNum * 10 + 3));
                    resultList.Add((MahjongManager.PaiKinds)(iroNum * 10 + 4));
                    resultList.Add((MahjongManager.PaiKinds)(iroNum * 10 + 5));
                    resultList.Add((MahjongManager.PaiKinds)(iroNum * 10 + 6));
                    resultList.Add((MahjongManager.PaiKinds)(iroNum * 10 + 7));
                    resultList.Add((MahjongManager.PaiKinds)(iroNum * 10 + 8));
                    resultList.Add((MahjongManager.PaiKinds)(iroNum * 10 + 9));
                    resultList.Add((MahjongManager.PaiKinds)(iroNum * 10 + 9));
                    resultList.Add((MahjongManager.PaiKinds)(iroNum * 10 + 9));

                    for (int i = 0; i < num; i++)
                    {
                        int removeNum = Random.Range(0, resultList.Count);
                        resultList.RemoveAt(removeNum);
                    }
                }
                break;
            case MahjongManager.SupportSetForTimeAttack.Yaku.SuRenKo:
                {
                    MahjongManager.PaiKinds kouho = RandomPaiKind(_supportSetForTimeAttack.yaku);
                    resultList.Add(kouho);
                    resultList.Add(kouho);
                    resultList.Add(kouho + 1);
                    resultList.Add(kouho + 1);
                    resultList.Add(kouho + 2);
                    resultList.Add(kouho + 2);
                    resultList.Add(kouho + 3);
                    resultList.Add(kouho + 3);
                    switch (_supportSetForTimeAttack.rank)
                    {
                        case MahjongManager.SupportSetForTimeAttack.Rank.A:
                            {
                                MahjongManager.PaiKinds kouhoRemove = kouho + Random.Range(0, 4);
                                for (int i = 0; i < 4; i++)
                                {
                                    if (kouho + i != kouhoRemove) resultList.Add(kouho);
                                }
                            }
                            break;
                        case MahjongManager.SupportSetForTimeAttack.Rank.B:
                            {
                                MahjongManager.PaiKinds kouhoA = kouho + Random.Range(0, 4);
                                MahjongManager.PaiKinds kouhoB = kouho + Random.Range(0, 4);
                                while (kouhoA == kouhoB) kouhoB = kouho + Random.Range(0, 4);
                                resultList.Add(kouhoA);
                                resultList.Add(kouhoB);
                            }
                            break;
                        case MahjongManager.SupportSetForTimeAttack.Rank.C:
                            {
                                MahjongManager.PaiKinds kouhoA = kouho + Random.Range(0, 4);
                                resultList.Add(kouhoA);
                            }
                            break;
                    }
                }
                break;
            case MahjongManager.SupportSetForTimeAttack.Yaku.HyakuManGoku:
                {
                    resultList.Add(MahjongManager.PaiKinds.M9);
                    resultList.Add(MahjongManager.PaiKinds.M9);
                    resultList.Add(MahjongManager.PaiKinds.M9);
                    resultList.Add(MahjongManager.PaiKinds.M9);
                    resultList.Add(MahjongManager.PaiKinds.M8);
                    resultList.Add(MahjongManager.PaiKinds.M8);
                    resultList.Add(MahjongManager.PaiKinds.M8);
                    resultList.Add(MahjongManager.PaiKinds.M8);
                    resultList.Add(MahjongManager.PaiKinds.M7);
                    resultList.Add(MahjongManager.PaiKinds.M7);

                    switch (_supportSetForTimeAttack.rank)
                    {
                        case MahjongManager.SupportSetForTimeAttack.Rank.A:
                            {
                                resultList.Add(MahjongManager.PaiKinds.M7);
                                resultList.Add((MahjongManager.PaiKinds)(Random.Range(0, 5) + 1));
                            }
                            break;
                        case MahjongManager.SupportSetForTimeAttack.Rank.B:
                            {
                                resultList.Add((MahjongManager.PaiKinds)(Random.Range(0, 5) + 1));
                            }
                            break;
                        case MahjongManager.SupportSetForTimeAttack.Rank.C:
                            {
                                resultList.Add((MahjongManager.PaiKinds)(Random.Range(0, 3) + 1));
                            }
                            break;
                    }
                }
                break;
            case MahjongManager.SupportSetForTimeAttack.Yaku.DaiShaRin:
                {
                    int num = 0;
                    switch (_supportSetForTimeAttack.rank)
                    {
                        case MahjongManager.SupportSetForTimeAttack.Rank.A:
                            {
                                num = 1;
                            }
                            break;
                        case MahjongManager.SupportSetForTimeAttack.Rank.B:
                            {
                                num = 2;
                            }
                            break;
                        case MahjongManager.SupportSetForTimeAttack.Rank.C:
                            {
                                num = 3;
                            }
                            break;
                    }

                    resultList.Add(MahjongManager.PaiKinds.P2);
                    resultList.Add(MahjongManager.PaiKinds.P3);
                    resultList.Add(MahjongManager.PaiKinds.P4);
                    resultList.Add(MahjongManager.PaiKinds.P5);
                    resultList.Add(MahjongManager.PaiKinds.P6);
                    resultList.Add(MahjongManager.PaiKinds.P7);
                    resultList.Add(MahjongManager.PaiKinds.P8);
                    resultList.Add(MahjongManager.PaiKinds.P2);
                    resultList.Add(MahjongManager.PaiKinds.P3);
                    resultList.Add(MahjongManager.PaiKinds.P4);
                    resultList.Add(MahjongManager.PaiKinds.P5);
                    resultList.Add(MahjongManager.PaiKinds.P6);
                    resultList.Add(MahjongManager.PaiKinds.P7);
                    resultList.Add(MahjongManager.PaiKinds.P8);

                    for (int i = 0; i < num; i++)
                    {
                        int removeNum = Random.Range(0, resultList.Count);
                        resultList.RemoveAt(removeNum);
                    }
                }
                break;
            case MahjongManager.SupportSetForTimeAttack.Yaku.BeniKujaku:
                {
                    int num = 0;
                    switch (_supportSetForTimeAttack.rank)
                    {
                        case MahjongManager.SupportSetForTimeAttack.Rank.A:
                            {
                                num = 11;
                            }
                            break;
                        case MahjongManager.SupportSetForTimeAttack.Rank.B:
                            {
                                num = 10;
                            }
                            break;
                        case MahjongManager.SupportSetForTimeAttack.Rank.C:
                            {
                                num = 9;
                            }
                            break;
                    }

                    while (resultList.Count < num)
                    {
                        MahjongManager.PaiKinds kouho = RandomPaiKind(_supportSetForTimeAttack.yaku);

                        int counter = 0;
                        foreach (var item in resultList)
                        {
                            if (kouho == item) counter++;
                        }
                        if (counter <= 3) resultList.Add(kouho);
                    }
                }
                break;
            default:
                {

                }
                break;
        }

        return resultList;
    }

    /// <summary>
    /// ランダムな牌種を作る
    /// </summary>
    /// <param name="_yaku"></param>
    /// <returns></returns>
    private MahjongManager.PaiKinds RandomPaiKind(MahjongManager.SupportSetForTimeAttack.Yaku _yaku) 
    {
        MahjongManager.PaiKinds result = MahjongManager.PaiKinds.None_00;
        switch (_yaku)
        {
            case MahjongManager.SupportSetForTimeAttack.Yaku.TsuIiSo:
                {
                    result = (MahjongManager.PaiKinds)(Random.Range(0, 7) + 31);
                }
                break;
            case MahjongManager.SupportSetForTimeAttack.Yaku.ChinRouTo:
                {
                    int num1 = Random.Range(0, 2) == 0 ? 1 : 9;
                    int num2 = Random.Range(0, 3);
                    result = (MahjongManager.PaiKinds)(num2 * 10 + num1);
                }
                break;
            case MahjongManager.SupportSetForTimeAttack.Yaku.RyuIiSo:
                {
                    switch (Random.Range(0, 6))
                    {
                        case 0: result = MahjongManager.PaiKinds.S2; break;
                        case 1: result = MahjongManager.PaiKinds.S3; break;
                        case 2: result = MahjongManager.PaiKinds.S4; break;
                        case 3: result = MahjongManager.PaiKinds.S6; break;
                        case 4: result = MahjongManager.PaiKinds.S8; break;
                        case 5: result = MahjongManager.PaiKinds.J6; break;
                    }
                }
                break;
            //case MahjongManager.SupportSetForTimeAttack.Yaku.KokuShiMuSou:
            //    {
            //        switch (Random.Range(0, 13))
            //        {
            //            case 0: result = MahjongManager.PaiKinds.M1; break;
            //            case 1: result = MahjongManager.PaiKinds.M9; break;
            //            case 2: result = MahjongManager.PaiKinds.P1; break;
            //            case 3: result = MahjongManager.PaiKinds.P9; break;
            //            case 4: result = MahjongManager.PaiKinds.S1; break;
            //            case 5: result = MahjongManager.PaiKinds.S9; break;
            //            case 6: result = MahjongManager.PaiKinds.J1; break;
            //            case 7: result = MahjongManager.PaiKinds.J2; break;
            //            case 8: result = MahjongManager.PaiKinds.J3; break;
            //            case 9: result = MahjongManager.PaiKinds.J4; break;
            //            case 10: result = MahjongManager.PaiKinds.J5; break;
            //            case 11: result = MahjongManager.PaiKinds.J6; break;
            //            case 12: result = MahjongManager.PaiKinds.J7; break;
            //        }
            //    }
            //    break;
            case MahjongManager.SupportSetForTimeAttack.Yaku.SuRenKo:
                {
                    int num1 = Random.Range(0, 6) + 1;
                    int num2 = Random.Range(0, 3);
                    result = (MahjongManager.PaiKinds)(num2 * 10 + num1);
                }
                break;
            case MahjongManager.SupportSetForTimeAttack.Yaku.DaiShaRin:
                {
                    result = (MahjongManager.PaiKinds)(Random.Range(0, 7) + 12);
                }
                break;
            case MahjongManager.SupportSetForTimeAttack.Yaku.BeniKujaku:
                {
                    switch (Random.Range(0, 5))
                    {
                        case 0: result = MahjongManager.PaiKinds.S1; break;
                        case 1: result = MahjongManager.PaiKinds.S5; break;
                        case 2: result = MahjongManager.PaiKinds.S7; break;
                        case 3: result = MahjongManager.PaiKinds.S9; break;
                        case 4: result = MahjongManager.PaiKinds.J7; break;
                    }
                }
                break;
            default:
                {
                    int num = 0;
                    while (num % 10 == 0)
                    {
                        num = Random.Range(0, 37) + 1;
                    }
                    result = (MahjongManager.PaiKinds)num;
                }
                break;
        }
        return result;
    }

}
