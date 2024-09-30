using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShantenCheck : MonoBehaviour
{
    private int[] tehaiList;//手牌の配列：37種
    private int[] tempTehai;//preTempTehai配列のクローン

    private int toitsu_suu;//トイツ数
    private int koutsu_suu;//コーツ数
    private int shuntsu_suu;//シュンツ数
    private int taatsu_suu;//ターツ数
    private int mentsu_suu;//メンツ数
    private int syanten_temp;//シャンテン数（作業用用）
    private int syanten_normal;//シャンテン数（結果用）
    private int kanzen_koutsu_suu;//完全コーツ数
    private int kanzen_shuntsu_suu;//完全シュンツ数
    private int kanzen_Koritsu_suu;//完全孤立牌数

    /// <summary>
    /// シャンテン数を調べて返す
    /// </summary>
    /// <param name="_tehaiInformation"></param>
    /// <returns></returns>
    public int GetSyantenSuu(int[] _tehaiInformation)
    {
        InitValue(_tehaiInformation);

        int result = syantenCheck();

        //Debug.Log($"GlobalValue\n{GetDebugGlobalValue()}");

        //Debug.Log($"_tehaiInformation\n{GetDebugTehaiArray(_tehaiInformation)}");
        //Debug.Log($"tehaiList\n{GetDebugTehaiArray(tehaiList)}");
        //Debug.Log($"tempTehai\n{GetDebugTehaiArray(tempTehai)}");

        return result;
    }

    /// <summary>
    /// 完全孤立牌を調べてリストを返す
    /// </summary>
    /// <param name="_tehaiInformation"></param>
    /// <returns></returns>
    public List<int> GetKanzenKoritsuPaiList(int[] _tehaiInformation)
    {
        List<int> resultList = new List<int>();

        // 字牌の完全孤立牌を抜き出す
        for (int i = 31; i < 38; i++)
        {
            if (_tehaiInformation[i] == 1)
            {
                resultList.Add(i);
            }
        }

        // 数牌の完全孤立牌を抜き出す
        for (int i = 0; i < 30; i += 10) // マンズ→ピンズ→ソーズ
        {
            // 1の孤立牌を抜く
            if (_tehaiInformation[i + 1] == 1 && _tehaiInformation[i + 2] == 0 && _tehaiInformation[i + 3] == 0)
            {
                resultList.Add(i + 1);
            }

            // 2の孤立牌を抜く
            if (_tehaiInformation[i + 1] == 0 && _tehaiInformation[i + 2] == 1 && _tehaiInformation[i + 3] == 0 && _tehaiInformation[i + 4] == 0)
            {
                resultList.Add(i + 2);
            }

            // 3~7の孤立牌を抜く
            for (int j = 0; j < 5; j++)
            {
                if (_tehaiInformation[i + j + 1] == 0 && _tehaiInformation[i + j + 2] == 0 && _tehaiInformation[i + j + 3] == 1 && _tehaiInformation[i + j + 4] == 0 && _tehaiInformation[i + j + 5] == 0)
                {
                    resultList.Add(i + j + 3);
                }
            }

            // 8の孤立牌を抜く
            if (_tehaiInformation[i + 6] == 0 && _tehaiInformation[i + 7] == 0 && _tehaiInformation[i + 8] == 1 && _tehaiInformation[i + 9] == 0)
            {
                resultList.Add(i + 8);
            }

            // 9の孤立牌を抜く
            if (_tehaiInformation[i + 7] == 0 && _tehaiInformation[i + 8] == 0 && _tehaiInformation[i + 9] == 1)
            {
                resultList.Add(i + 9);
            }
        }

        return resultList;
    }







    /// <summary>
    /// 手牌情報の配列を文字列で返す
    /// </summary>
    /// <param name="_tehaiInformation"></param>
    /// <returns></returns>
    private string GetDebugTehaiArray(int[] _tehaiInformation)
    {
        string str = "";
        for (int i = 0; i < 38; i++)
        {
            if (i % 10 == 0)
            {
                str += "[";
            }
            else if (i % 10 == 9)
            {
                str += $"{_tehaiInformation[i]}]\n";
            }
            else
            {
                str += $"{_tehaiInformation[i]}, ";
            }
        }
        str += $"]";

        return str;
    }

    /// <summary>
    /// グローバル変数の値を文字列で返す
    /// </summary>
    /// <returns></returns>
    private string GetDebugGlobalValue()
    {
        string str = "";

        str += $"GetSyantenSuu : value\n";
        str += $"toitsu_suu = {toitsu_suu}\n";
        str += $"koutsu_suu = {koutsu_suu}\n";
        str += $"shuntsu_suu = {shuntsu_suu}\n";
        str += $"taatsu_suu = {taatsu_suu}\n";
        str += $"mentsu_suu = {mentsu_suu}\n";
        str += $"syanten_temp = {syanten_temp}\n";
        str += $"syanten_normal = {syanten_normal}\n";
        str += $"kanzen_koutsu_suu = {kanzen_koutsu_suu}\n";
        str += $"kanzen_shuntsu_suu = {kanzen_shuntsu_suu}\n";
        str += $"kanzen_Koritsu_suu = {kanzen_Koritsu_suu}";

        return str;
    }

    /// <summary>
    /// 変数の初期化
    /// </summary>
    /// <param name="_tehaiInformation"></param>
    private void InitValue(int[] _tehaiInformation)
    {
        // 手牌情報のコピー
        tehaiList = new int[38];
        Array.Copy(_tehaiInformation, tehaiList, 38);

        // 手牌枚数のカウントと持っていない字牌のリスト作成
        int paiCount = 0;
        List<int> noHaveJihaiList = new List<int>();
        for (int i = 0; i < 38; i++)
        {
            if (tehaiList[i] > 0) paiCount += tehaiList[i];
            else if (i > 30) noHaveJihaiList.Add(i);
        }

        // 鳴き部分を持っていない字牌の暗刻で埋める
        while(paiCount < 13)
        {
            const int ANKO_NUM = 3;
            int index = noHaveJihaiList[0];
            tehaiList[index] += ANKO_NUM;
            paiCount += ANKO_NUM;
            noHaveJihaiList.RemoveAt(0);
        }

        // tehai配列のバックアップを取る
        tempTehai = new int[38];
        tempTehai = (int[])tehaiList.Clone();

        // グローバル変数の初期化
        toitsu_suu = 0; // トイツ数
        koutsu_suu = 0; // コーツ数
        shuntsu_suu = 0; // シュンツ数
        taatsu_suu = 0; // ターツ数
        mentsu_suu = 0; // メンツ数
        syanten_temp = 0; // シャンテン数（計算用）
        syanten_normal = 8; // シャンテン数（結果用）
        kanzen_koutsu_suu = 0; // 完全コーツ数
        kanzen_shuntsu_suu = 0; // 完全シュンツ数
        kanzen_Koritsu_suu = 0; // 完全孤立牌数
    }

    /// <summary>
    /// アガリ判定とシャンテン数を返す
    /// </summary>
    /// <returns></returns>
    private int syantenCheck()
    {
        // 完全なシュンツ・コーツ・孤立牌を抜いておく
        kanzen_koutsu_suu = KanzenKoutsuCheck(); // 完全に独立したコーツを抜き出して個数を返す関数呼び出し
        kanzen_shuntsu_suu = kanzenShuntsuCheck(); // 完全に独立したシュンツを抜き出して個数を返す関数呼び出し
        kanzen_Koritsu_suu = KanzenKoritsuCheck(); // 完全に独立した孤立牌を抜き出して個数を返す関数の実行

        // 雀頭抜き出し→コーツ抜き出し→シュンツ抜き出し→ターツ候補抜き出し
        for (int i = 1; i < 38; i++)
        {
            // 頭抜き出し
            if (tempTehai[i] >= 2)
            {
                toitsu_suu++;
                tempTehai[i] -= 2;
                mentu_cut1(1); // メンツ切り出し関数の呼び出し
                tempTehai[i] += 2;
                toitsu_suu--;
            }
        }

        toitsu_suu = 0;
        // 【雀頭が無い場合の処理】コーツ抜き出し→シュンツ抜き出し→ターツ候補抜き出し
        mentu_cut1(1); // メンツ切り出し関数の呼び出し

        return syanten_normal; // 最終的な結果
    }

    /// <summary>
    /// メンツ抜き出し1【→コーツ抜き出し→シュンツ抜き出し】
    /// </summary>
    /// <param name="i"></param>
    private void mentu_cut1(int i)
    {
        for (int j = i; j < 30; j++) // ※字牌のコーツは完全コーツ処理で抜いているので数牌だけで良い
        {
            // コーツ抜き出し
            if (tempTehai[j] >= 3)
            {
                mentsu_suu++;
                koutsu_suu++;
                tempTehai[j] -= 3;
                mentu_cut1(j);
                tempTehai[j] += 3;
                koutsu_suu--;
            }

            // シュンツ抜き出し
            if (tempTehai[j] > 0 && tempTehai[j + 1] > 0 && tempTehai[j + 2] > 0 && j < 28)
            {
                shuntsu_suu++;
                tempTehai[j]--;
                tempTehai[j + 1]--;
                tempTehai[j + 2]--;
                mentu_cut1(j); // 自身を呼び出す
                tempTehai[j]++;
                tempTehai[j + 1]++;
                tempTehai[j + 2]++;
                shuntsu_suu--;
            }
        }

        taatu_cut(1); // ターツ抜きへ
    }

    /// <summary>
    /// メンツ抜き出し2【シュンツ抜き出し→コーツ抜き出し】
    /// </summary>
    /// <param name="i"></param>
    private void mentu_cut2(int i)
    {
        for (int j = i; j < 30; j++) // ※字牌のコーツは完全コーツ処理で抜いているので数牌だけで良い
        {
            // シュンツ抜き出し
            if (tempTehai[j] > 0 && tempTehai[j + 1] > 0 && tempTehai[j + 2] > 0 && j < 28)
            {
                shuntsu_suu++;
                tempTehai[j]--;
                tempTehai[j + 1]--;
                tempTehai[j + 2]--;
                mentu_cut2(j); // 自身を呼び出す
                tempTehai[j]++;
                tempTehai[j + 1]++;
                tempTehai[j + 2]++;
                shuntsu_suu--;
            }

            // コーツ抜き出し
            if (tempTehai[j] >= 3)
            {
                mentsu_suu++;
                koutsu_suu++;
                tempTehai[j] -= 3;
                mentu_cut2(j);
                tempTehai[j] += 3;
                koutsu_suu--;
            }
        }

        taatu_cut(1); // ターツ抜きへ
    }

    /// <summary>
    /// ターツ抜き出し
    /// </summary>
    /// <param name="i"></param>
    private void taatu_cut(int i)
    {
        for (int j = i; j < 38; j++)
        {
            mentsu_suu = kanzen_koutsu_suu + koutsu_suu + kanzen_shuntsu_suu + shuntsu_suu;

            if (mentsu_suu + taatsu_suu < 4) // メンツとターツの合計は4まで
            {
                // トイツ抜き出し
                if (tempTehai[j] == 2)
                {
                    taatsu_suu++;
                    tempTehai[j] -= 2;
                    taatu_cut(j);
                    tempTehai[j] += 2;
                    taatsu_suu--;
                }

                // 字牌の時を省く
                if (j < 30)
                {
                    // リャンメン・ペンチャン抜き出し
                    if (tempTehai[j] > 0 && tempTehai[j + 1] > 0 && j < 29 && j % 10 < 9)
                    {
                        taatsu_suu++;
                        tempTehai[j]--;
                        tempTehai[j + 1]--;
                        taatu_cut(j);
                        tempTehai[j]++;
                        tempTehai[j + 1]++;
                        taatsu_suu--;
                    }

                    // カンチャン抜き出し
                    if (tempTehai[j] > 0 && tempTehai[j + 1] == 0 && tempTehai[j + 2] > 0 && j < 28 && j % 10 < 8)
                    {
                        taatsu_suu++;
                        tempTehai[j]--;
                        tempTehai[j + 2]--;
                        taatu_cut(j);
                        tempTehai[j]++;
                        tempTehai[j + 2]++;
                        taatsu_suu--;
                    }
                }
            }
        }

        syanten_temp = 8 - mentsu_suu * 2 - taatsu_suu - toitsu_suu;
        if (syanten_temp < syanten_normal)
        {
            syanten_normal = syanten_temp;
        }
    }

    /// <summary>
    /// 完全コーツを抜き出して個数を返す
    /// </summary>
    /// <returns></returns>
    private int KanzenKoutsuCheck()
    {
        int Kanzenkoutsu_suu = 0;
        int i, j;

        // 字牌の完全コーツを抜き出す
        for (i = 31; i < 38; i++)
        {
            if (tempTehai[i] >= 3)
            {
                tempTehai[i] -= 3;
                Kanzenkoutsu_suu++;
            }
        }

        // 数牌の完全コーツを抜き出す
        for (i = 0; i < 30; i += 10)
        {
            if (tempTehai[i + 1] >= 3 && tempTehai[i + 2] == 0 && tempTehai[i + 3] == 0)
            {
                tempTehai[i + 1] -= 3;
                Kanzenkoutsu_suu++;
            }
            if (tempTehai[i + 1] == 0 && tempTehai[i + 2] >= 3 && tempTehai[i + 3] == 0 && tempTehai[i + 4] == 0)
            {
                tempTehai[i + 2] -= 3;
                Kanzenkoutsu_suu++;
            }

            // 3~7の完全コーツを抜く
            for (j = 0; j < 5; j++)
            {
                if (tempTehai[i + j + 1] == 0 && tempTehai[i + j + 2] == 0 && tempTehai[i + j + 3] >= 3 && tempTehai[i + j + 4] == 0 && tempTehai[i + j + 5] == 0)
                {
                    tempTehai[i + j + 3] -= 3;
                    Kanzenkoutsu_suu++;
                }
            }
            if (tempTehai[i + 6] == 0 && tempTehai[i + 7] == 0 && tempTehai[i + 8] >= 3 && tempTehai[i + 9] == 0)
            {
                tempTehai[i + 8] -= 3;
                Kanzenkoutsu_suu++;
            }

            if (tempTehai[i + 7] == 0 && tempTehai[i + 8] == 0 && tempTehai[i + 9] >= 3)
            {
                tempTehai[i + 9] -= 3;
                Kanzenkoutsu_suu++;
            }
        }

        return Kanzenkoutsu_suu;
    }

    /// <summary>
    /// 完全シュンツを抜き出して個数を返す
    /// </summary>
    /// <returns></returns>
    private int kanzenShuntsuCheck()
    {
        int kanzenshuntsu_suu = 0;
        int i;

        // 123, 456のような完全に独立したシュンツを抜き出すための処理
        // 【注意】番地0，10，20，30が「0」の必要あり。事前に赤ドラを移動させる処理をしておく。
        for (i = 0; i < 30; i += 10) // マンズ→ピンズ→ソーズ
        {
            // 123▲▲
            if (tempTehai[i + 1] == 2 && tempTehai[i + 2] == 2 && tempTehai[i + 3] == 2 && tempTehai[i + 4] == 0 && tempTehai[i + 5] == 0)
            {
                tempTehai[i + 1] -= 2;
                tempTehai[i + 2] -= 2;
                tempTehai[i + 3] -= 2;
                kanzenshuntsu_suu += 2;
            }

            // ▲234▲▲
            if (tempTehai[i + 1] == 0 && tempTehai[i + 2] == 2 && tempTehai[i + 3] == 2 && tempTehai[i + 4] == 2 && tempTehai[i + 5] == 0 && tempTehai[i + 6] == 0)
            {
                tempTehai[i + 2] -= 2;
                tempTehai[i + 3] -= 2;
                tempTehai[i + 4] -= 2;
                kanzenshuntsu_suu += 2;
            }

            // ▲▲345▲▲
            if (tempTehai[i + 1] == 0 && tempTehai[i + 2] == 0 && tempTehai[i + 3] == 2 && tempTehai[i + 4] == 2 && tempTehai[i + 5] == 2 && tempTehai[i + 6] == 0 && tempTehai[i + 7] == 0)
            {
                tempTehai[i + 3] -= 2;
                tempTehai[i + 4] -= 2;
                tempTehai[i + 5] -= 2;
                kanzenshuntsu_suu += 2;
            }

            // ▲▲456▲▲
            if (tempTehai[i + 2] == 0 && tempTehai[i + 3] == 0 && tempTehai[i + 4] == 2 && tempTehai[i + 5] == 2 && tempTehai[i + 6] == 2 && tempTehai[i + 7] == 0 && tempTehai[i + 8] == 0)
            {
                tempTehai[i + 4] -= 2;
                tempTehai[i + 5] -= 2;
                tempTehai[i + 6] -= 2;
                kanzenshuntsu_suu += 2;
            }

            // ▲▲567▲▲
            if (tempTehai[i + 3] == 0 && tempTehai[i + 4] == 0 && tempTehai[i + 5] == 2 && tempTehai[i + 6] == 2 && tempTehai[i + 7] == 2 && tempTehai[i + 8] == 0 && tempTehai[i + 9] == 0)
            {
                tempTehai[i + 5] -= 2;
                tempTehai[i + 6] -= 2;
                tempTehai[i + 7] -= 2;
                kanzenshuntsu_suu += 2;
            }

            // ▲▲678▲
            if (tempTehai[i + 4] == 0 && tempTehai[i + 5] == 0 && tempTehai[i + 6] == 2 && tempTehai[i + 7] == 2 && tempTehai[i + 8] == 2 && tempTehai[i + 9] == 0)
            {
                tempTehai[i + 6] -= 2;
                tempTehai[i + 7] -= 2;
                tempTehai[i + 8] -= 2;
                kanzenshuntsu_suu += 2;
            }

            // ▲▲789
            if (tempTehai[i + 5] == 0 && tempTehai[i + 6] == 0 && tempTehai[i + 7] == 2 && tempTehai[i + 8] == 2 && tempTehai[i + 9] == 2)
            {
                tempTehai[i + 7] -= 2;
                tempTehai[i + 8] -= 2;
                tempTehai[i + 9] -= 2;
                kanzenshuntsu_suu += 2;
            }
        }

        for (i = 0; i < 30; i += 10) // マンズ→ピンズ→ソーズ
        {
            // 123▲▲
            if (tempTehai[i + 1] == 1 && tempTehai[i + 2] == 1 && tempTehai[i + 3] == 1 && tempTehai[i + 4] == 0 && tempTehai[i + 5] == 0)
            {
                tempTehai[i + 1]--;
                tempTehai[i + 2]--;
                tempTehai[i + 3]--;
                kanzenshuntsu_suu++;
            }

            // ▲234▲▲
            if (tempTehai[i + 1] == 0 && tempTehai[i + 2] == 1 && tempTehai[i + 3] == 1 && tempTehai[i + 4] == 1 && tempTehai[i + 5] == 0 && tempTehai[i + 6] == 0)
            {
                tempTehai[i + 2]--;
                tempTehai[i + 3]--;
                tempTehai[i + 4]--;
                kanzenshuntsu_suu++;
            }

            // ▲▲345▲▲
            if (tempTehai[i + 1] == 0 && tempTehai[i + 2] == 0 && tempTehai[i + 3] == 1 && tempTehai[i + 4] == 1 && tempTehai[i + 5] == 1 && tempTehai[i + 6] == 0 && tempTehai[i + 7] == 0)
            {
                tempTehai[i + 3]--;
                tempTehai[i + 4]--;
                tempTehai[i + 5]--;
                kanzenshuntsu_suu++;
            }

            // ▲▲456▲▲
            if (tempTehai[i + 2] == 0 && tempTehai[i + 3] == 0 && tempTehai[i + 4] == 1 && tempTehai[i + 5] == 1 && tempTehai[i + 6] == 1 && tempTehai[i + 7] == 0 && tempTehai[i + 8] == 0)
            {
                tempTehai[i + 4]--;
                tempTehai[i + 5]--;
                tempTehai[i + 6]--;
                kanzenshuntsu_suu++;
            }

            // ▲▲567▲▲
            if (tempTehai[i + 3] == 0 && tempTehai[i + 4] == 0 && tempTehai[i + 5] == 1 && tempTehai[i + 6] == 1 && tempTehai[i + 7] == 1 && tempTehai[i + 8] == 0 && tempTehai[i + 9] == 0)
            {
                tempTehai[i + 5]--;
                tempTehai[i + 6]--;
                tempTehai[i + 7]--;
                kanzenshuntsu_suu++;
            }

            // ▲▲678▲
            if (tempTehai[i + 4] == 0 && tempTehai[i + 5] == 0 && tempTehai[i + 6] == 1 && tempTehai[i + 7] == 1 && tempTehai[i + 8] == 1 && tempTehai[i + 9] == 0)
            {
                tempTehai[i + 6]--;
                tempTehai[i + 7]--;
                tempTehai[i + 8]--;
                kanzenshuntsu_suu++;
            }

            // ▲▲789
            if (tempTehai[i + 5] == 0 && tempTehai[i + 6] == 0 && tempTehai[i + 7] == 1 && tempTehai[i + 8] == 1 && tempTehai[i + 9] == 1)
            {
                tempTehai[i + 7]--;
                tempTehai[i + 8]--;
                tempTehai[i + 9]--;
                kanzenshuntsu_suu++;
            }
        }
        return kanzenshuntsu_suu;
    }

    /// <summary>
    /// 完全孤立牌を抜き出して個数を返す
    /// </summary>
    /// <returns></returns>
    private int KanzenKoritsuCheck()
    {
        int KanzenKoritsu_suu = 0;
        int i, j;

        // 字牌の完全孤立牌を抜き出す
        for (i = 31; i < 38; i++)
        {
            if (tempTehai[i] == 1)
            {
                tempTehai[i]--;
                KanzenKoritsu_suu++;
            }
        }

        // 数牌の完全孤立牌を抜き出す
        for (i = 0; i < 30; i += 10) // マンズ→ピンズ→ソーズ
        {
            // 1の孤立牌を抜く
            if (tempTehai[i + 1] == 1 && tempTehai[i + 2] == 0 && tempTehai[i + 3] == 0)
            {
                tempTehai[i + 1]--;
                KanzenKoritsu_suu++;
            }

            // 2の孤立牌を抜く
            if (tempTehai[i + 1] == 0 && tempTehai[i + 2] == 1 && tempTehai[i + 3] == 0 && tempTehai[i + 4] == 0)
            {
                tempTehai[i + 2]--;
                KanzenKoritsu_suu++;
            }

            // 3~7の孤立牌を抜く
            for (j = 0; j < 5; j++)
            {
                if (tempTehai[i + j + 1] == 0 && tempTehai[i + j + 2] == 0 && tempTehai[i + j + 3] == 1 && tempTehai[i + j + 4] == 0 && tempTehai[i + j + 5] == 0)
                {
                    tempTehai[i + j + 3]--;
                    KanzenKoritsu_suu++;
                }
            }

            // 8の孤立牌を抜く
            if (tempTehai[i + 6] == 0 && tempTehai[i + 7] == 0 && tempTehai[i + 8] == 1 && tempTehai[i + 9] == 0)
            {
                tempTehai[i + 8]--;
                KanzenKoritsu_suu++;
            }

            // 9の孤立牌を抜く
            if (tempTehai[i + 7] == 0 && tempTehai[i + 8] == 0 && tempTehai[i + 9] == 1)
            {
                tempTehai[i + 9]--;
                KanzenKoritsu_suu++;
            }
        }

        return KanzenKoritsu_suu;
    }





}


public class Old_ShantenCheck : MonoBehaviour
{

    //マンズ関連
    private int manMentsuMax;
    private int manTaatsuMax;

    //ピンズ関連
    private int pinMentsuMax;
    private int pinTaatsuMax;

    //ソーズ関連
    private int souMentsuMax;
    private int souTaatsuMax;

    //字牌関連
    private int jiTaatsuMax;

    //コーツ数カウント用
    private int koutsuCount;

    //kanzen_koutsu_suuとkanzen_shuntsu_suu格納用
    private int preMentsuCount;

    // 手牌情報
    private int[] tehai;

    //tempTehaiの前処理用クローン。事前に完全メンツや孤立牌を抜いておく
    private int[] preTempTehai;

    //tehai配列のクローン
    private int[] tempTehai;


    public int SyantenCheckAll(int[] _tehaiInformation)
    {
        InitValue(_tehaiInformation);

        int syantenkokusi = SyantenKokusi(); // 国士無双のシャンテン数を算出する関数の呼び出し
        if (syantenkokusi == 0 || syantenkokusi == -1)
        {
            return syantenkokusi;
        }

        int syanten7toitsu = Syanten7Toitsu(); // チートイツのシャンテン数を算出する関数の呼び出し
        if (syanten7toitsu == 0 || syanten7toitsu == -1)
        {
            return syanten7toitsu;
        }

        int syanten_suu = SyantenCheck(0); // 通常手のシャンテン数を算出する関数の呼び出し

        return Math.Min(Math.Min(syantenkokusi, syanten7toitsu), syanten_suu);
    }

    /// <summary>
    /// 変数の初期化
    /// </summary>
    /// <param name="_tehaiInformation"></param>
    private void InitValue(int[] _tehaiInformation)
    {
        // 手牌情報のコピー
        tehai = new int[38];
        Array.Copy(_tehaiInformation, tehai, 38);

        // グローバル変数の初期化
        manMentsuMax = 0;
        manTaatsuMax = 0;
        pinMentsuMax = 0;
        pinTaatsuMax = 0;
        souMentsuMax = 0;
        souTaatsuMax = 0;
        jiTaatsuMax = 0;
        koutsuCount = 0;

        // tehai配列のバックアップを取る
        preTempTehai = new int[38];
        preTempTehai = (int[])tehai.Clone();
    }

    /// <summary>
    /// 国士無双のアガリ判定とシャンテン数を返す
    /// </summary>
    /// <returns></returns>
    private int SyantenKokusi()
    {
        int syantenKokusi = 13;
        int toitsuSuu = 0; // 雀頭の数

        // 19牌をチェック
        for (int i = 1; i < 30; i++)
        {
            if (i % 10 == 1 || i % 10 == 9)
            {
                if (tehai[i] > 0)
                {
                    syantenKokusi--;
                }
                if (tehai[i] >= 2 && toitsuSuu == 0)
                {
                    toitsuSuu = 1;
                }
            }
        }

        // 字牌をチェック
        for (int i = 31; i < 38; i++)
        {
            if (tehai[i] > 0)
            {
                syantenKokusi--;
            }
            if (tehai[i] >= 2 && toitsuSuu == 0)
            {
                toitsuSuu = 1;
            }
        }

        // 雀頭がある場合の処理
        syantenKokusi -= toitsuSuu;
        return syantenKokusi;
    }

    /// <summary>
    /// チートイツのアガリ判定とシャンテン数を返す
    /// </summary>
    /// <returns></returns>
    private int Syanten7Toitsu()
    {
        int toitsuSuu = 0; // 対子数
        int syuruiSuu = 0; // 牌の種類数
        int syanten7Toitsu = 6; // 七対子のシャンテン数

        // 対子を数える
        for (int i = 1; i < 38; i++)
        {
            if (tehai[i] > 0)
            {
                syuruiSuu++;
                if (tehai[i] >= 2)
                {
                    toitsuSuu++;
                }
            }
        }

        // 七対子のシャンテン数を計算
        syanten7Toitsu = 6 - toitsuSuu;

        // 4枚チートイツの回避
        if (syuruiSuu < 7)
        {
            syanten7Toitsu = 7 - syuruiSuu;
        }

        return syanten7Toitsu;
    }

    /// <summary>
    /// 通常手のアガリ判定とシャンテン数を返す
    /// </summary>
    /// <param name="tehaimode"></param>
    /// <returns></returns>
    private int SyantenCheck(int tehaimode)
    {
        int toitsu_suu = 0; // 雀頭
        int syanten_temp = 0; // シャンテン数（計算用）
        int syanten_suu = 8;

        int manMentsuTaatsu_suu = 0;
        int pinMentsuTaatsu_suu = 0;
        int souMentsuTaatsu_suu = 0;
        int jiTaatsu_suu = 0;

        // 前もって完全なシュンツ・コーツ・孤立牌を抜いておく
        int kanzen_koutsu_suu = KanzenKoutsuCheck(); // 完全に独立したコーツを抜き出して個数を返す関数の実行
        int kanzen_shuntsu_suu = KanzenShuntsuCheck(); // 完全に独立したシュンツを抜き出して個数を返す関数の実行
        preMentsuCount = kanzen_koutsu_suu + kanzen_shuntsu_suu; // グローバル変数に格納

        // 5枚目の単騎待ちを阻止する処置
        int kanzen_Koritsu_suu = KanzenKoritsuCheck(); // 完全に独立した孤立牌を抜き出して個数を返す関数の実行

        // 独立した完全トイツのチェック
        int kanzen_toitsu_check = KanzenToitsuCheck(); // ある場合は「1」、無い場合は「0」を返す

        // マンズ・ピンズ・ソーズの有無のチェック
        int isCheckMan = preTempTehai[1] + preTempTehai[2] + preTempTehai[3] + preTempTehai[4] + preTempTehai[5] + preTempTehai[6] + preTempTehai[7] + preTempTehai[8] + preTempTehai[9];
        int isCheckPin = preTempTehai[11] + preTempTehai[12] + preTempTehai[13] + preTempTehai[14] + preTempTehai[15] + preTempTehai[16] + preTempTehai[17] + preTempTehai[18] + preTempTehai[19];
        int isCheckSou = preTempTehai[21] + preTempTehai[22] + preTempTehai[23] + preTempTehai[24] + preTempTehai[25] + preTempTehai[26] + preTempTehai[27] + preTempTehai[28] + preTempTehai[29];
        int isCheckJi = preTempTehai[31] + preTempTehai[32] + preTempTehai[33] + preTempTehai[34] + preTempTehai[35] + preTempTehai[36] + preTempTehai[37];
        int adjustment = kanzen_koutsu_suu + kanzen_shuntsu_suu;

        // 【雀頭あり】雀頭抜き出し→コーツ抜き出し→シュンツ抜き出し→ターツ候補抜き出し
        for (int i = 1; i < 38; i++)
        {
            if (i % 10 == 0) continue;

            if (preTempTehai[i] >= 2)
            {
                preTempTehai[i] -= 2; // 雀頭を抜き出す
                int jantou = i; // 雀頭の番号を格納する
                toitsu_suu = 1; // 雀頭をカウントする

                if (isCheckMan > 0)
                {
                    manMentsuMax = 0;
                    manTaatsuMax = 0;
                    manMentsuTaatsu_suu = manMentsuTaatsuCheck(1);
                }
                if (isCheckPin > 0)
                {
                    pinMentsuMax = 0;
                    pinTaatsuMax = 0;
                    pinMentsuTaatsu_suu = pinMentsuTaatsuCheck(1);
                }
                if (isCheckSou > 0)
                {
                    souMentsuMax = 0;
                    souTaatsuMax = 0;
                    souMentsuTaatsu_suu = souMentsuTaatsuCheck(1);
                }
                if (isCheckJi > 0)
                {
                    jiTaatsuMax = 0;
                    jiTaatsu_suu = jiTaatuCheck();
                }

                // シャンテン数の算出
                syanten_temp = SyantenHantei(toitsu_suu, adjustment);
                if (syanten_suu > syanten_temp) syanten_suu = syanten_temp;
                if (syanten_suu == 0 && (isCheckMan + isCheckPin + isCheckSou + preMentsuCount * 3) == tehaimode)
                {
                    return 0; // テンパイ判定
                }
                if (syanten_suu == -1)
                {
                    return -1; // アガリ判定
                }

                preTempTehai[i] += 2;
                toitsu_suu--;
            }
        }

        // 【雀頭あり】雀頭抜き出し→シュンツ抜き出し→コーツ抜き出し→ターツ候補抜き出し
        if (koutsuCount > 0)
        {
            for (int i = 1; i < 38; i++)
            {
                if (i % 10 == 0) continue;

                if (preTempTehai[i] >= 2)
                {
                    preTempTehai[i] -= 2; // 雀頭を抜き出す
                    int jantou = i; // 雀頭の番号を格納する
                    toitsu_suu = 1; // 雀頭をカウントする

                    if (isCheckMan > 0)
                    {
                        manMentsuMax = 0;
                        manTaatsuMax = 0;
                        manMentsuTaatsu_suu = manMentsuTaatsuCheck(2);
                    }
                    if (isCheckPin > 0)
                    {
                        pinMentsuMax = 0;
                        pinTaatsuMax = 0;
                        pinMentsuTaatsu_suu = pinMentsuTaatsuCheck(2);
                    }
                    if (isCheckSou > 0)
                    {
                        souMentsuMax = 0;
                        souTaatsuMax = 0;
                        souMentsuTaatsu_suu = souMentsuTaatsuCheck(2);
                    }
                    if (isCheckJi > 0)
                    {
                        jiTaatsuMax = 0;
                        jiTaatuCheck();
                    }

                    // シャンテン数の算出
                    syanten_temp = SyantenHantei(toitsu_suu, adjustment);
                    if (syanten_suu > syanten_temp) syanten_suu = syanten_temp;
                    if (syanten_suu == 0 && (isCheckMan + isCheckPin + isCheckSou + preMentsuCount * 3) == tehaimode)
                    {
                        return 0; // テンパイ判定
                    }
                    if (syanten_suu == -1)
                    {
                        return -1; // アガリ判定
                    }

                    preTempTehai[i] += 2;
                    toitsu_suu--;
                }
            }
        }

        // 【雀頭無し】コーツ抜き出し→シュンツ抜き出し→ターツ候補抜き出し
        if (kanzen_toitsu_check == 0)
        {
            if (isCheckMan > 0)
            {
                manMentsuMax = 0;
                manTaatsuMax = 0;
                manMentsuTaatsu_suu = manMentsuTaatsuCheck(1);
            }
            if (isCheckPin > 0)
            {
                pinMentsuMax = 0;
                pinTaatsuMax = 0;
                pinMentsuTaatsu_suu = pinMentsuTaatsuCheck(1);
            }
            if (isCheckSou > 0)
            {
                souMentsuMax = 0;
                souTaatsuMax = 0;
                souMentsuTaatsu_suu = souMentsuTaatsuCheck(1);
            }
            if (isCheckJi > 0)
            {
                jiTaatsuMax = 0;
                jiTaatuCheck();
            }

            // シャンテン数の算出
            syanten_temp = SyantenHantei(toitsu_suu, adjustment);
            if (syanten_suu > syanten_temp) syanten_suu = syanten_temp;
            if (syanten_suu == 0 && (isCheckMan + isCheckPin + isCheckSou + preMentsuCount * 3) == tehaimode)
            {
                return 0; // テンパイ判定
            }
            if (syanten_suu == -1)
            {
                return -1; // アガリ判定
            }
        }

        return syanten_suu; // シャンテン数を返す
    }

    /// <summary>
    /// シャンテン数を算出する
    /// </summary>
    /// <param name="toitsu_suu"></param>
    /// <param name="adjustment"></param>
    /// <returns></returns>
    private int SyantenHantei(int toitsu_suu, int adjustment)
    {
        int syanten_temp = 0;
        int block_suu = 0;

        // ブロック数の計算
        block_suu = (manMentsuMax + pinMentsuMax + souMentsuMax + adjustment) +
                    (manTaatsuMax + pinTaatsuMax + souTaatsuMax + jiTaatsuMax);

        // シャンテン数算出
        if (block_suu > 4)
        {
            syanten_temp = 8 - (manMentsuMax + pinMentsuMax + souMentsuMax + adjustment) * 2 -
                           (manTaatsuMax + pinTaatsuMax + souTaatsuMax + jiTaatsuMax) - toitsu_suu +
                           (block_suu - 4);
        }
        else
        {
            syanten_temp = 8 - (manMentsuMax + pinMentsuMax + souMentsuMax + adjustment) * 2 -
                           (manTaatsuMax + pinTaatsuMax + souTaatsuMax + jiTaatsuMax) - toitsu_suu;
        }

        return syanten_temp;
    }

    /// <summary>
    /// 字牌ターツ抜き出し
    /// </summary>
    /// <returns></returns>
    private int jiTaatuCheck()
    {
        int jiTaatsu = 0;

        for (int i = 31; i < 38; i++)
        {
            if (tempTehai[i] == 2)
            {
                tempTehai[i] -= 2; // トイツを抜き出す
                jiTaatsu++;
            }
        }

        jiTaatsuMax = jiTaatsu;

        return jiTaatsu;
    }

    /// <summary>
    /// マンズのコーツを抜き出して個数を返す
    /// </summary>
    /// <param name="n">n=1:最初に見つかったコーツを1つだけ抜く</param>
    /// <returns></returns>
    private int manKoutsuCheck(int n)
    {
        int koutsu_suu = 0;

        for (int i = 1; i < 10; i++)
        {
            if (tempTehai[i] >= 3)
            {
                tempTehai[i] -= 3;
                koutsu_suu++;
                koutsuCount++;

                // 「n=1」の時、1回だけコーツを抜いて処理を抜ける
                if (n == 1)
                {
                    return koutsu_suu;
                }
            }
        }

        return koutsu_suu;
    }

    /// <summary>
    /// マンズのコーツ・シュンツ・ターツを抜き出して個数を返す
    /// </summary>
    /// <param name="n">n=1:コーツ→シュンツ→ターツの順に抜く , n=2:シュンツ→コーツ→ターツの順に抜く , n=3:1コーツ→シュンツ→コーツ→ターツの順に抜く</param>
    /// <returns></returns>
    private int manMentsuTaatsuCheck(int n)
    {
        int suujishuntsu_suu = 0;
        int manMentsu;
        int manTaatsu;
        int manMentsu_temp;
        int isCheckMan;
        int i, j;

        int manMax_temp = 0;

        for (j = 0; j < 8; j++)
        {
            manMentsu = 0;
            manTaatsu = 0;
            tempTehai = (int[])preTempTehai.Clone(); // preTempTehai配列の内容をコピーする

            if (n == 1) { manMentsu += manKoutsuCheck(0); } // マンズのコーツを抜いてコーツ数を返す関数の実行
            if (n == 3) { manMentsu += manKoutsuCheck(1); } // マンズのコーツを抜いてコーツ数を返す関数の実行

            // マンズのシュンツを抜く処理
            for (i = 1; i < 8; i++)
            {
                while (tempTehai[i + j] >= 1 && tempTehai[i + 1 + j] >= 1 && tempTehai[i + 2 + j] >= 1 && (i + j) < 8)
                {
                    tempTehai[i + j]--;
                    tempTehai[i + 1 + j]--;
                    tempTehai[i + 2 + j]--;
                    manMentsu++;
                }
            }

            // マンズのシュンツを抜く処理2
            if (j > 2)
            {
                for (i = 1; i < 8; i++)
                {
                    while (tempTehai[i] >= 1 && tempTehai[i + 1] >= 1 && tempTehai[i + 2] >= 1 && i < 8)
                    {
                        tempTehai[i]--;
                        tempTehai[i + 1]--;
                        tempTehai[i + 2]--;
                        manMentsu++;
                    }
                }
            }

            if (n == 2 || n == 3) { manMentsu += manKoutsuCheck(0); } // マンズのコーツを抜いてコーツ数を返す関数の実行

            // マンズのターツを抜く処理
            for (i = 1; i < 8; i++)
            {
                // トイツの抜き出し
                if (tempTehai[i] >= 2 && i < 10)
                {
                    tempTehai[i] -= 2; // トイツを抜き出す
                    manTaatsu++;
                }

                // リャンメンとペンチャンの抜き出し
                if (tempTehai[i] >= 1 && tempTehai[i + 1] >= 1 && i < 9)
                {
                    tempTehai[i]--;
                    tempTehai[i + 1]--;
                    manTaatsu++;
                }

                // カンチャンの抜き出し
                if (tempTehai[i] >= 1 && tempTehai[i + 2] >= 1 && i < 8)
                {
                    tempTehai[i]--;
                    tempTehai[i + 2]--;
                    manTaatsu++;
                }

                manMentsu_temp = manMentsu * 10 + manTaatsu;
                if (manMentsu_temp > manMax_temp)
                {
                    manMax_temp = manMentsu_temp;
                    manMentsuMax = manMentsu;
                    manTaatsuMax = manTaatsu;
                    suujishuntsu_suu = manMentsu * 10 + manTaatsu;
                }

                isCheckMan = tempTehai[1] + tempTehai[2] + tempTehai[3] + tempTehai[4] + tempTehai[5] + tempTehai[6] + tempTehai[7] + tempTehai[8] + tempTehai[9];
                if (isCheckMan == 0) { return suujishuntsu_suu; }
            }

            isCheckMan = tempTehai[1] + tempTehai[2] + tempTehai[3] + tempTehai[4] + tempTehai[5] + tempTehai[6] + tempTehai[7] + tempTehai[8] + tempTehai[9];
            if (isCheckMan == 0) { return suujishuntsu_suu; }
            if (manMentsu == 0) { return suujishuntsu_suu; }
        }

        return suujishuntsu_suu;
    }

    /// <summary>
    /// ピンズのコーツを抜き出して個数を返す
    /// </summary>
    /// <param name="n">n=1:最初に見つかったコーツを1つだけ抜く</param>
    /// <returns></returns>
    private int pinKoutsuCheck(int n)
    {
        int koutsu_suu = 0;
        for (int i = 11; i < 20; i++)
        {
            if (tempTehai[i] >= 3)
            {
                tempTehai[i] -= 3;
                koutsu_suu++;
                koutsuCount++;

                // 「n=1」の時、1回だけコーツを抜いて処理を抜ける
                if (n == 1)
                {
                    return koutsu_suu;
                }
            }
        }
        return koutsu_suu;
    }

    /// <summary>
    /// ピンズのコーツ・シュンツ・ターツを抜き出して個数を返す
    /// </summary>
    /// <param name="n">n=1:コーツ→シュンツ→ターツの順に抜く , n=2:シュンツ→コーツ→ターツの順に抜く , n=3:1コーツ→シュンツ→コーツ→ターツの順に抜く</param>
    /// <returns></returns>
    private int pinMentsuTaatsuCheck(int n)
    {
        int suujishuntsu_suu = 0;
        int pinMentsu;
        int pinTaatsu;
        int pinMentsu_temp;
        int pinMax_temp = 0;
        int isCheckPin;

        for (int j = 0; j < 8; j++)
        {
            pinMentsu = 0;
            pinTaatsu = 0;
            tempTehai = (int[])preTempTehai.Clone(); // preTempTehai配列の内容をコピーする

            if (n == 1) pinMentsu += pinKoutsuCheck(0); // ピンズのコーツを抜いてコーツ数を返す関数の実行
            if (n == 3) pinMentsu += pinKoutsuCheck(1); // ピンズのコーツを抜いてコーツ数を返す関数の実行

            // ピンズのシュンツを抜く処理
            for (int i = 11; i < 18; i++)
            {
                while (tempTehai[i + j] >= 1 && tempTehai[i + 1 + j] >= 1 && tempTehai[i + 2 + j] >= 1 && (i + j) < 18)
                {
                    tempTehai[i + j]--;
                    tempTehai[i + 1 + j]--;
                    tempTehai[i + 2 + j]--;
                    pinMentsu++;
                }
            }

            // ピンズのシュンツを抜く処理2
            if (j > 2)
            {
                for (int i = 11; i < 18; i++)
                {
                    while (tempTehai[i] >= 1 && tempTehai[i + 1] >= 1 && tempTehai[i + 2] >= 1 && i < 18)
                    {
                        tempTehai[i]--;
                        tempTehai[i + 1]--;
                        tempTehai[i + 2]--;
                        pinMentsu++;
                    }
                }
            }

            if (n == 2 || n == 3) pinMentsu += pinKoutsuCheck(0); // ピンズのコーツを抜いてコーツ数を返す関数の実行

            // ピンズのターツを抜く処理
            for (int i = 11; i < 18; i++)
            {
                // トイツの抜き出し
                if (tempTehai[i] >= 2 && i < 20)
                {
                    tempTehai[i] -= 2; // トイツを抜き出す
                    pinTaatsu++;
                }

                // リャンメンとペンチャンの抜き出し
                if (tempTehai[i] > 0 && tempTehai[i + 1] > 0 && i < 19)
                {
                    tempTehai[i]--;
                    tempTehai[i + 1]--;
                    pinTaatsu++;
                }

                // カンチャンの抜き出し
                if (tempTehai[i] > 0 && tempTehai[i + 1] == 0 && tempTehai[i + 2] > 0 && i < 18)
                {
                    tempTehai[i]--;
                    tempTehai[i + 2]--;
                    pinTaatsu++;
                }

                pinMentsu_temp = pinMentsu * 10 + pinTaatsu;
                if (pinMentsu_temp > pinMax_temp)
                {
                    pinMax_temp = pinMentsu_temp;
                    pinMentsuMax = pinMentsu;
                    pinTaatsuMax = pinTaatsu;
                    suujishuntsu_suu = pinMentsu * 10 + pinTaatsu;
                }

                isCheckPin = tempTehai[11] + tempTehai[12] + tempTehai[13] + tempTehai[14] + tempTehai[15] + tempTehai[16] + tempTehai[17] + tempTehai[18] + tempTehai[19];
                if (isCheckPin == 0) return suujishuntsu_suu;
            }

            isCheckPin = tempTehai[11] + tempTehai[12] + tempTehai[13] + tempTehai[14] + tempTehai[15] + tempTehai[16] + tempTehai[17] + tempTehai[18] + tempTehai[19];
            if (isCheckPin == 0) return suujishuntsu_suu;
            if (pinMentsu == 0) return suujishuntsu_suu;
        }

        return suujishuntsu_suu;
    }

    /// <summary>
    /// ソーズのコーツを抜き出して個数を返す
    /// </summary>
    /// <param name="n">n=1:最初に見つかったコーツを1つだけ抜く</param>
    /// <returns></returns>
    private int souKoutsuCheck(int n)
    {
        int koutsu_suu = 0;

        for (int i = 21; i < 30; i++)
        {
            if (tempTehai[i] >= 3)
            {
                tempTehai[i] -= 3;
                koutsu_suu++;
                koutsuCount++;

                // 「n=1」の時、1回だけコーツを抜いて処理を抜ける
                if (n == 1)
                {
                    return koutsu_suu;
                }
            }
        }

        return koutsu_suu;
    }

    /// <summary>
    /// ソーズのコーツ・シュンツ・ターツを抜き出して個数を返す
    /// </summary>
    /// <param name="n">n=1:コーツ→シュンツ→ターツの順に抜く , n=2:シュンツ→コーツ→ターツの順に抜く , n=3:1コーツ→シュンツ→コーツ→ターツの順に抜く</param>
    /// <returns></returns>
    private int souMentsuTaatsuCheck(int n)
    {
        int suujishuntsu_suu = 0;
        int souMentsu;
        int souTaatsu;
        int souMentsu_temp;
        int isCheckSou;
        int souMax_temp = 0;

        for (int j = 0; j < 8; j++)
        {
            souMentsu = 0;
            souTaatsu = 0;
            tempTehai = (int[])preTempTehai.Clone(); // preTempTehai配列の内容をコピーする

            if (n == 1) souMentsu += souKoutsuCheck(0); // ソーズのコーツを抜いてコーツ数を返す関数の実行
            if (n == 3) souMentsu += souKoutsuCheck(1); // ソーズのコーツを抜いてコーツ数を返す関数の実行

            // ソーズのシュンツを抜く処理
            for (int i = 21; i < 28; i++)
            {
                while (tempTehai[i + j] >= 1 && tempTehai[i + 1 + j] >= 1 && tempTehai[i + 2 + j] >= 1 && (i + j) < 28)
                {
                    tempTehai[i + j]--;
                    tempTehai[i + 1 + j]--;
                    tempTehai[i + 2 + j]--;
                    souMentsu++;
                }
            }

            // ソーズのシュンツを抜く処理2
            if (j > 2)
            {
                for (int i = 21; i < 28; i++)
                {
                    while (tempTehai[i] >= 1 && tempTehai[i + 1] >= 1 && tempTehai[i + 2] >= 1 && i < 28)
                    {
                        tempTehai[i]--;
                        tempTehai[i + 1]--;
                        tempTehai[i + 2]--;
                        souMentsu++;
                    }
                }
            }

            if (n == 2 || n == 3) souMentsu += souKoutsuCheck(0); // ソーズのコーツを抜いてコーツ数を返す関数の実行

            // ソーズのターツを抜く処理
            for (int i = 21; i < 28; i++)
            {
                // トイツの抜き出し
                if (tempTehai[i] >= 2 && i < 30)
                {
                    tempTehai[i] -= 2; // トイツを抜き出す
                    souTaatsu++;
                }

                // リャンメンとペンチャンの抜き出し
                if (tempTehai[i] >= 1 && tempTehai[i + 1] >= 1 && i < 29)
                {
                    tempTehai[i]--;
                    tempTehai[i + 1]--;
                    souTaatsu++;
                }

                // カンチャンの抜き出し
                if (tempTehai[i] >= 1 && tempTehai[i + 1] == 0 && tempTehai[i + 2] >= 1 && i < 28)
                {
                    tempTehai[i]--;
                    tempTehai[i + 2]--;
                    souTaatsu++;
                }

                souMentsu_temp = souMentsu * 10 + souTaatsu;
                if (souMentsu_temp > souMax_temp)
                {
                    souMax_temp = souMentsu_temp;
                    souMentsuMax = souMentsu;
                    souTaatsuMax = souTaatsu;
                    suujishuntsu_suu = souMentsu * 10 + souTaatsu;
                }

                isCheckSou = tempTehai[21] + tempTehai[22] + tempTehai[23] + tempTehai[24] + tempTehai[25] + tempTehai[26] + tempTehai[27] + tempTehai[28] + tempTehai[29];
                if (isCheckSou == 0) return suujishuntsu_suu;
            }

            isCheckSou = tempTehai[21] + tempTehai[22] + tempTehai[23] + tempTehai[24] + tempTehai[25] + tempTehai[26] + tempTehai[27] + tempTehai[28] + tempTehai[29];
            if (isCheckSou == 0) return suujishuntsu_suu;
            if (souMentsu == 0) return suujishuntsu_suu;
        }

        return suujishuntsu_suu;
    }

    /// <summary>
    /// 完全コーツを抜き出して個数を返す
    /// </summary>
    /// <returns></returns>
    private int KanzenKoutsuCheck()
    {
        int Kanzenkoutsu_suu = 0;
        int i, j;

        // 字牌の完全コーツを抜き出す
        for (i = 31; i < 38; i++)
        {
            if (preTempTehai[i] >= 3)
            {
                preTempTehai[i] -= 3;
                Kanzenkoutsu_suu++;
            }
        }

        // 数牌の完全コーツを抜き出す
        for (i = 0; i < 30; i += 10)
        {
            if (preTempTehai[i + 1] >= 3 && preTempTehai[i + 2] == 0 && preTempTehai[i + 3] == 0)
            {
                preTempTehai[i + 1] -= 3;
                Kanzenkoutsu_suu++;
            }
            if (preTempTehai[i + 1] == 0 && preTempTehai[i + 2] >= 3 && preTempTehai[i + 3] == 0 && preTempTehai[i + 4] == 0)
            {
                preTempTehai[i + 2] -= 3;
                Kanzenkoutsu_suu++;
            }

            // 3~7の完全コーツを抜く
            for (j = 0; j < 5; j++)
            {
                if (preTempTehai[i + j + 1] == 0 && preTempTehai[i + j + 2] == 0 && preTempTehai[i + j + 3] >= 3 && preTempTehai[i + j + 4] == 0 && preTempTehai[i + j + 5] == 0)
                {
                    preTempTehai[i + j + 3] -= 3;
                    Kanzenkoutsu_suu++;
                }
            }
            if (preTempTehai[i + 6] == 0 && preTempTehai[i + 7] == 0 && preTempTehai[i + 8] >= 3 && preTempTehai[i + 9] == 0)
            {
                preTempTehai[i + 8] -= 3;
                Kanzenkoutsu_suu++;
            }

            if (preTempTehai[i + 7] == 0 && preTempTehai[i + 8] == 0 && preTempTehai[i + 9] >= 3)
            {
                preTempTehai[i + 9] -= 3;
                Kanzenkoutsu_suu++;
            }
        }

        return Kanzenkoutsu_suu;
    }

    /// <summary>
    /// 完全シュンツを抜き出して個数を返す
    /// </summary>
    /// <returns></returns>
    private int KanzenShuntsuCheck()
    {
        int kanzenshuntsu_suu = 0;
        int i;

        // 123, 456 のような完全に独立したシュンツを抜き出すための処理
        // 【注意】番地0，10，20，30が「0」の必要あり。事前に赤ドラを移動させる処理をしておく。
        for (i = 0; i < 30; i += 10) // マンズ→ピンズ→ソーズ
        {
            // 123▲▲
            if (preTempTehai[i + 1] == 2 && preTempTehai[i + 2] == 2 && preTempTehai[i + 3] == 2 &&
                preTempTehai[i + 4] == 0 && preTempTehai[i + 5] == 0)
            {
                preTempTehai[i + 1] -= 2;
                preTempTehai[i + 2] -= 2;
                preTempTehai[i + 3] -= 2;
                kanzenshuntsu_suu += 2;
            }

            // ▲234▲▲
            if (preTempTehai[i + 1] == 0 && preTempTehai[i + 2] == 2 && preTempTehai[i + 3] == 2 &&
                preTempTehai[i + 4] == 2 && preTempTehai[i + 5] == 0 && preTempTehai[i + 6] == 0)
            {
                preTempTehai[i + 2] -= 2;
                preTempTehai[i + 3] -= 2;
                preTempTehai[i + 4] -= 2;
                kanzenshuntsu_suu += 2;
            }

            // ▲▲345▲▲
            if (preTempTehai[i + 1] == 0 && preTempTehai[i + 2] == 0 && preTempTehai[i + 3] == 2 &&
                preTempTehai[i + 4] == 2 && preTempTehai[i + 5] == 2 && preTempTehai[i + 6] == 0 &&
                preTempTehai[i + 7] == 0)
            {
                preTempTehai[i + 3] -= 2;
                preTempTehai[i + 4] -= 2;
                preTempTehai[i + 5] -= 2;
                kanzenshuntsu_suu += 2;
            }

            // ▲▲456▲▲
            if (preTempTehai[i + 2] == 0 && preTempTehai[i + 3] == 0 && preTempTehai[i + 4] == 2 &&
                preTempTehai[i + 5] == 2 && preTempTehai[i + 6] == 2 && preTempTehai[i + 7] == 0 &&
                preTempTehai[i + 8] == 0)
            {
                preTempTehai[i + 4] -= 2;
                preTempTehai[i + 5] -= 2;
                preTempTehai[i + 6] -= 2;
                kanzenshuntsu_suu += 2;
            }

            // ▲▲567▲▲
            if (preTempTehai[i + 3] == 0 && preTempTehai[i + 4] == 0 && preTempTehai[i + 5] == 2 &&
                preTempTehai[i + 6] == 2 && preTempTehai[i + 7] == 2 && preTempTehai[i + 8] == 0 &&
                preTempTehai[i + 9] == 0)
            {
                preTempTehai[i + 5] -= 2;
                preTempTehai[i + 6] -= 2;
                preTempTehai[i + 7] -= 2;
                kanzenshuntsu_suu += 2;
            }

            // ▲▲678▲
            if (preTempTehai[i + 4] == 0 && preTempTehai[i + 5] == 0 && preTempTehai[i + 6] == 2 &&
                preTempTehai[i + 7] == 2 && preTempTehai[i + 8] == 2 && preTempTehai[i + 9] == 0)
            {
                preTempTehai[i + 6] -= 2;
                preTempTehai[i + 7] -= 2;
                preTempTehai[i + 8] -= 2;
                kanzenshuntsu_suu += 2;
            }

            // ▲▲789
            if (preTempTehai[i + 5] == 0 && preTempTehai[i + 6] == 0 && preTempTehai[i + 7] == 2 &&
                preTempTehai[i + 8] == 2 && preTempTehai[i + 9] == 2)
            {
                preTempTehai[i + 7] -= 2;
                preTempTehai[i + 8] -= 2;
                preTempTehai[i + 9] -= 2;
                kanzenshuntsu_suu += 2;
            }
        }

        for (i = 0; i < 30; i += 10) // マンズ→ピンズ→ソーズ
        {
            // 123▲▲
            if (preTempTehai[i + 1] == 1 && preTempTehai[i + 2] == 1 && preTempTehai[i + 3] == 1 &&
                preTempTehai[i + 4] == 0 && preTempTehai[i + 5] == 0)
            {
                preTempTehai[i + 1]--;
                preTempTehai[i + 2]--;
                preTempTehai[i + 3]--;
                kanzenshuntsu_suu++;
            }

            // ▲234▲▲
            if (preTempTehai[i + 1] == 0 && preTempTehai[i + 2] == 1 && preTempTehai[i + 3] == 1 &&
                preTempTehai[i + 4] == 1 && preTempTehai[i + 5] == 0 && preTempTehai[i + 6] == 0)
            {
                preTempTehai[i + 2]--;
                preTempTehai[i + 3]--;
                preTempTehai[i + 4]--;
                kanzenshuntsu_suu++;
            }

            // ▲▲345▲▲
            if (preTempTehai[i + 1] == 0 && preTempTehai[i + 2] == 0 && preTempTehai[i + 3] == 1 &&
                preTempTehai[i + 4] == 1 && preTempTehai[i + 5] == 1 && preTempTehai[i + 6] == 0 &&
                preTempTehai[i + 7] == 0)
            {
                preTempTehai[i + 3]--;
                preTempTehai[i + 4]--;
                preTempTehai[i + 5]--;
                kanzenshuntsu_suu++;
            }

            // ▲▲456▲▲
            if (preTempTehai[i + 2] == 0 && preTempTehai[i + 3] == 0 && preTempTehai[i + 4] == 1 &&
                preTempTehai[i + 5] == 1 && preTempTehai[i + 6] == 1 && preTempTehai[i + 7] == 0 &&
                preTempTehai[i + 8] == 0)
            {
                preTempTehai[i + 4]--;
                preTempTehai[i + 5]--;
                preTempTehai[i + 6]--;
                kanzenshuntsu_suu++;
            }

            // ▲▲567▲▲
            if (preTempTehai[i + 3] == 0 && preTempTehai[i + 4] == 0 && preTempTehai[i + 5] == 1 &&
                preTempTehai[i + 6] == 1 && preTempTehai[i + 7] == 1 && preTempTehai[i + 8] == 0 &&
                preTempTehai[i + 9] == 0)
            {
                preTempTehai[i + 5]--;
                preTempTehai[i + 6]--;
                preTempTehai[i + 7]--;
                kanzenshuntsu_suu++;
            }

            // ▲▲678▲
            if (preTempTehai[i + 4] == 0 && preTempTehai[i + 5] == 0 && preTempTehai[i + 6] == 1 &&
                preTempTehai[i + 7] == 1 && preTempTehai[i + 8] == 1 && preTempTehai[i + 9] == 0)
            {
                preTempTehai[i + 6]--;
                preTempTehai[i + 7]--;
                preTempTehai[i + 8]--;
                kanzenshuntsu_suu++;
            }

            // ▲▲789
            if (preTempTehai[i + 5] == 0 && preTempTehai[i + 6] == 0 && preTempTehai[i + 7] == 1 &&
                preTempTehai[i + 8] == 1 && preTempTehai[i + 9] == 1)
            {
                preTempTehai[i + 7]--;
                preTempTehai[i + 8]--;
                preTempTehai[i + 9]--;
                kanzenshuntsu_suu++;
            }
        }

        return kanzenshuntsu_suu;
    }

    /// <summary>
    /// 完全孤立牌を抜き出して個数を返す
    /// </summary>
    /// <returns></returns>
    private int KanzenKoritsuCheck()
    {
        int KanzenKoritsu_suu = 0;
        int i, j;

        // 字牌の完全孤立牌を抜き出す
        for (i = 31; i < 38; i++)
        {
            if (preTempTehai[i] == 1)
            {
                // koritsu = i; // 孤立牌を変数に格納する（コメントアウトされているが、必要に応じて使用可能）
                preTempTehai[i]--;
                KanzenKoritsu_suu++;
            }
        }

        // 数牌の完全孤立牌を抜き出す
        for (i = 0; i < 30; i += 10) // マンズ→ピンズ→ソーズ
        {
            // 1の孤立牌を抜く
            if (preTempTehai[i + 1] == 1 && preTempTehai[i + 2] == 0 && preTempTehai[i + 3] == 0)
            {
                // koritsu = i + 1; // 孤立牌を変数に格納する（コメントアウトされているが、必要に応じて使用可能）
                preTempTehai[i + 1]--;
                KanzenKoritsu_suu++;
            }

            // 2の孤立牌を抜く
            if (preTempTehai[i + 1] == 0 && preTempTehai[i + 2] == 1 && preTempTehai[i + 3] == 0 && preTempTehai[i + 4] == 0)
            {
                preTempTehai[i + 2]--;
                KanzenKoritsu_suu++;
            }

            // 3~7の完全孤立牌を抜く
            for (j = 0; j < 5; j++)
            {
                if (preTempTehai[i + j + 1] == 0 && preTempTehai[i + j + 2] == 0 && preTempTehai[i + j + 3] == 1 &&
                    preTempTehai[i + j + 4] == 0 && preTempTehai[i + j + 5] == 0)
                {
                    preTempTehai[i + j + 3]--;
                    KanzenKoritsu_suu++;
                }
            }

            // 8の孤立牌を抜く
            if (preTempTehai[i + 6] == 0 && preTempTehai[i + 7] == 0 && preTempTehai[i + 8] == 1 && preTempTehai[i + 9] == 0)
            {
                preTempTehai[i + 8]--;
                KanzenKoritsu_suu++;
            }

            // 9の孤立牌を抜く
            if (preTempTehai[i + 7] == 0 && preTempTehai[i + 8] == 0 && preTempTehai[i + 9] == 1)
            {
                preTempTehai[i + 9]--;
                KanzenKoritsu_suu++;
            }
        }

        return KanzenKoritsu_suu;
    }

    /// <summary>
    /// 完全トイツをチェックする(雀頭が無い場合の処理をスルーするための措置)
    /// </summary>
    /// <returns>見つかった場合は「1」を、見つからなかった場合は「0」を返す</returns>
    private int KanzenToitsuCheck()
    {
        int i, j;

        // 字牌のトイツチェック
        for (i = 31; i < 38; i++)
        {
            if (preTempTehai[i] == 2)
            {
                return 1;
            }
        }

        // 数牌の完全トイツをチェック
        for (i = 0; i < 30; i += 10)
        {
            // 1の完全トイツをチェック
            if (preTempTehai[i + 1] == 2 && preTempTehai[i + 2] == 0)
            {
                return 1;
            }

            // 2の完全トイツをチェック
            if (preTempTehai[i + 1] == 0 && preTempTehai[i + 2] == 2 && preTempTehai[i + 3] == 0)
            {
                return 1;
            }

            // 3~7の完全トイツをチェック
            for (j = 0; j < 5; j++)
            {
                if (preTempTehai[i + j + 2] == 0 && preTempTehai[i + j + 3] == 2 && preTempTehai[i + j + 4] == 0)
                {
                    return 1;
                }
            }

            // 8の完全トイツをチェック
            if (preTempTehai[i + 7] == 0 && preTempTehai[i + 8] == 2 && preTempTehai[i + 9] == 0)
            {
                return 1;
            }

            // 9の完全トイツをチェック
            if (preTempTehai[i + 8] == 0 && preTempTehai[i + 9] == 2)
            {
                return 1;
            }
        }

        return 0;
    }



}
