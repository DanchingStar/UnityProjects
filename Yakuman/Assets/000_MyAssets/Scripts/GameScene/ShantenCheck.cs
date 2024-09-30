using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShantenCheck : MonoBehaviour
{
    private int[] tehaiList;//��v�̔z��F37��
    private int[] tempTehai;//preTempTehai�z��̃N���[��

    private int toitsu_suu;//�g�C�c��
    private int koutsu_suu;//�R�[�c��
    private int shuntsu_suu;//�V�����c��
    private int taatsu_suu;//�^�[�c��
    private int mentsu_suu;//�����c��
    private int syanten_temp;//�V�����e�����i��Ɨp�p�j
    private int syanten_normal;//�V�����e�����i���ʗp�j
    private int kanzen_koutsu_suu;//���S�R�[�c��
    private int kanzen_shuntsu_suu;//���S�V�����c��
    private int kanzen_Koritsu_suu;//���S�Ǘ��v��

    /// <summary>
    /// �V�����e�����𒲂ׂĕԂ�
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
    /// ���S�Ǘ��v�𒲂ׂă��X�g��Ԃ�
    /// </summary>
    /// <param name="_tehaiInformation"></param>
    /// <returns></returns>
    public List<int> GetKanzenKoritsuPaiList(int[] _tehaiInformation)
    {
        List<int> resultList = new List<int>();

        // ���v�̊��S�Ǘ��v�𔲂��o��
        for (int i = 31; i < 38; i++)
        {
            if (_tehaiInformation[i] == 1)
            {
                resultList.Add(i);
            }
        }

        // ���v�̊��S�Ǘ��v�𔲂��o��
        for (int i = 0; i < 30; i += 10) // �}���Y���s���Y���\�[�Y
        {
            // 1�̌Ǘ��v�𔲂�
            if (_tehaiInformation[i + 1] == 1 && _tehaiInformation[i + 2] == 0 && _tehaiInformation[i + 3] == 0)
            {
                resultList.Add(i + 1);
            }

            // 2�̌Ǘ��v�𔲂�
            if (_tehaiInformation[i + 1] == 0 && _tehaiInformation[i + 2] == 1 && _tehaiInformation[i + 3] == 0 && _tehaiInformation[i + 4] == 0)
            {
                resultList.Add(i + 2);
            }

            // 3~7�̌Ǘ��v�𔲂�
            for (int j = 0; j < 5; j++)
            {
                if (_tehaiInformation[i + j + 1] == 0 && _tehaiInformation[i + j + 2] == 0 && _tehaiInformation[i + j + 3] == 1 && _tehaiInformation[i + j + 4] == 0 && _tehaiInformation[i + j + 5] == 0)
                {
                    resultList.Add(i + j + 3);
                }
            }

            // 8�̌Ǘ��v�𔲂�
            if (_tehaiInformation[i + 6] == 0 && _tehaiInformation[i + 7] == 0 && _tehaiInformation[i + 8] == 1 && _tehaiInformation[i + 9] == 0)
            {
                resultList.Add(i + 8);
            }

            // 9�̌Ǘ��v�𔲂�
            if (_tehaiInformation[i + 7] == 0 && _tehaiInformation[i + 8] == 0 && _tehaiInformation[i + 9] == 1)
            {
                resultList.Add(i + 9);
            }
        }

        return resultList;
    }







    /// <summary>
    /// ��v���̔z��𕶎���ŕԂ�
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
    /// �O���[�o���ϐ��̒l�𕶎���ŕԂ�
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
    /// �ϐ��̏�����
    /// </summary>
    /// <param name="_tehaiInformation"></param>
    private void InitValue(int[] _tehaiInformation)
    {
        // ��v���̃R�s�[
        tehaiList = new int[38];
        Array.Copy(_tehaiInformation, tehaiList, 38);

        // ��v�����̃J�E���g�Ǝ����Ă��Ȃ����v�̃��X�g�쐬
        int paiCount = 0;
        List<int> noHaveJihaiList = new List<int>();
        for (int i = 0; i < 38; i++)
        {
            if (tehaiList[i] > 0) paiCount += tehaiList[i];
            else if (i > 30) noHaveJihaiList.Add(i);
        }

        // �������������Ă��Ȃ����v�̈Í��Ŗ��߂�
        while(paiCount < 13)
        {
            const int ANKO_NUM = 3;
            int index = noHaveJihaiList[0];
            tehaiList[index] += ANKO_NUM;
            paiCount += ANKO_NUM;
            noHaveJihaiList.RemoveAt(0);
        }

        // tehai�z��̃o�b�N�A�b�v�����
        tempTehai = new int[38];
        tempTehai = (int[])tehaiList.Clone();

        // �O���[�o���ϐ��̏�����
        toitsu_suu = 0; // �g�C�c��
        koutsu_suu = 0; // �R�[�c��
        shuntsu_suu = 0; // �V�����c��
        taatsu_suu = 0; // �^�[�c��
        mentsu_suu = 0; // �����c��
        syanten_temp = 0; // �V�����e�����i�v�Z�p�j
        syanten_normal = 8; // �V�����e�����i���ʗp�j
        kanzen_koutsu_suu = 0; // ���S�R�[�c��
        kanzen_shuntsu_suu = 0; // ���S�V�����c��
        kanzen_Koritsu_suu = 0; // ���S�Ǘ��v��
    }

    /// <summary>
    /// �A�K������ƃV�����e������Ԃ�
    /// </summary>
    /// <returns></returns>
    private int syantenCheck()
    {
        // ���S�ȃV�����c�E�R�[�c�E�Ǘ��v�𔲂��Ă���
        kanzen_koutsu_suu = KanzenKoutsuCheck(); // ���S�ɓƗ������R�[�c�𔲂��o���Č���Ԃ��֐��Ăяo��
        kanzen_shuntsu_suu = kanzenShuntsuCheck(); // ���S�ɓƗ������V�����c�𔲂��o���Č���Ԃ��֐��Ăяo��
        kanzen_Koritsu_suu = KanzenKoritsuCheck(); // ���S�ɓƗ������Ǘ��v�𔲂��o���Č���Ԃ��֐��̎��s

        // ���������o�����R�[�c�����o�����V�����c�����o�����^�[�c��┲���o��
        for (int i = 1; i < 38; i++)
        {
            // �������o��
            if (tempTehai[i] >= 2)
            {
                toitsu_suu++;
                tempTehai[i] -= 2;
                mentu_cut1(1); // �����c�؂�o���֐��̌Ăяo��
                tempTehai[i] += 2;
                toitsu_suu--;
            }
        }

        toitsu_suu = 0;
        // �y�����������ꍇ�̏����z�R�[�c�����o�����V�����c�����o�����^�[�c��┲���o��
        mentu_cut1(1); // �����c�؂�o���֐��̌Ăяo��

        return syanten_normal; // �ŏI�I�Ȍ���
    }

    /// <summary>
    /// �����c�����o��1�y���R�[�c�����o�����V�����c�����o���z
    /// </summary>
    /// <param name="i"></param>
    private void mentu_cut1(int i)
    {
        for (int j = i; j < 30; j++) // �����v�̃R�[�c�͊��S�R�[�c�����Ŕ����Ă���̂Ő��v�����ŗǂ�
        {
            // �R�[�c�����o��
            if (tempTehai[j] >= 3)
            {
                mentsu_suu++;
                koutsu_suu++;
                tempTehai[j] -= 3;
                mentu_cut1(j);
                tempTehai[j] += 3;
                koutsu_suu--;
            }

            // �V�����c�����o��
            if (tempTehai[j] > 0 && tempTehai[j + 1] > 0 && tempTehai[j + 2] > 0 && j < 28)
            {
                shuntsu_suu++;
                tempTehai[j]--;
                tempTehai[j + 1]--;
                tempTehai[j + 2]--;
                mentu_cut1(j); // ���g���Ăяo��
                tempTehai[j]++;
                tempTehai[j + 1]++;
                tempTehai[j + 2]++;
                shuntsu_suu--;
            }
        }

        taatu_cut(1); // �^�[�c������
    }

    /// <summary>
    /// �����c�����o��2�y�V�����c�����o�����R�[�c�����o���z
    /// </summary>
    /// <param name="i"></param>
    private void mentu_cut2(int i)
    {
        for (int j = i; j < 30; j++) // �����v�̃R�[�c�͊��S�R�[�c�����Ŕ����Ă���̂Ő��v�����ŗǂ�
        {
            // �V�����c�����o��
            if (tempTehai[j] > 0 && tempTehai[j + 1] > 0 && tempTehai[j + 2] > 0 && j < 28)
            {
                shuntsu_suu++;
                tempTehai[j]--;
                tempTehai[j + 1]--;
                tempTehai[j + 2]--;
                mentu_cut2(j); // ���g���Ăяo��
                tempTehai[j]++;
                tempTehai[j + 1]++;
                tempTehai[j + 2]++;
                shuntsu_suu--;
            }

            // �R�[�c�����o��
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

        taatu_cut(1); // �^�[�c������
    }

    /// <summary>
    /// �^�[�c�����o��
    /// </summary>
    /// <param name="i"></param>
    private void taatu_cut(int i)
    {
        for (int j = i; j < 38; j++)
        {
            mentsu_suu = kanzen_koutsu_suu + koutsu_suu + kanzen_shuntsu_suu + shuntsu_suu;

            if (mentsu_suu + taatsu_suu < 4) // �����c�ƃ^�[�c�̍��v��4�܂�
            {
                // �g�C�c�����o��
                if (tempTehai[j] == 2)
                {
                    taatsu_suu++;
                    tempTehai[j] -= 2;
                    taatu_cut(j);
                    tempTehai[j] += 2;
                    taatsu_suu--;
                }

                // ���v�̎����Ȃ�
                if (j < 30)
                {
                    // �����������E�y���`���������o��
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

                    // �J���`���������o��
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
    /// ���S�R�[�c�𔲂��o���Č���Ԃ�
    /// </summary>
    /// <returns></returns>
    private int KanzenKoutsuCheck()
    {
        int Kanzenkoutsu_suu = 0;
        int i, j;

        // ���v�̊��S�R�[�c�𔲂��o��
        for (i = 31; i < 38; i++)
        {
            if (tempTehai[i] >= 3)
            {
                tempTehai[i] -= 3;
                Kanzenkoutsu_suu++;
            }
        }

        // ���v�̊��S�R�[�c�𔲂��o��
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

            // 3~7�̊��S�R�[�c�𔲂�
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
    /// ���S�V�����c�𔲂��o���Č���Ԃ�
    /// </summary>
    /// <returns></returns>
    private int kanzenShuntsuCheck()
    {
        int kanzenshuntsu_suu = 0;
        int i;

        // 123, 456�̂悤�Ȋ��S�ɓƗ������V�����c�𔲂��o�����߂̏���
        // �y���Ӂz�Ԓn0�C10�C20�C30���u0�v�̕K�v����B���O�ɐԃh�����ړ������鏈�������Ă����B
        for (i = 0; i < 30; i += 10) // �}���Y���s���Y���\�[�Y
        {
            // 123����
            if (tempTehai[i + 1] == 2 && tempTehai[i + 2] == 2 && tempTehai[i + 3] == 2 && tempTehai[i + 4] == 0 && tempTehai[i + 5] == 0)
            {
                tempTehai[i + 1] -= 2;
                tempTehai[i + 2] -= 2;
                tempTehai[i + 3] -= 2;
                kanzenshuntsu_suu += 2;
            }

            // ��234����
            if (tempTehai[i + 1] == 0 && tempTehai[i + 2] == 2 && tempTehai[i + 3] == 2 && tempTehai[i + 4] == 2 && tempTehai[i + 5] == 0 && tempTehai[i + 6] == 0)
            {
                tempTehai[i + 2] -= 2;
                tempTehai[i + 3] -= 2;
                tempTehai[i + 4] -= 2;
                kanzenshuntsu_suu += 2;
            }

            // ����345����
            if (tempTehai[i + 1] == 0 && tempTehai[i + 2] == 0 && tempTehai[i + 3] == 2 && tempTehai[i + 4] == 2 && tempTehai[i + 5] == 2 && tempTehai[i + 6] == 0 && tempTehai[i + 7] == 0)
            {
                tempTehai[i + 3] -= 2;
                tempTehai[i + 4] -= 2;
                tempTehai[i + 5] -= 2;
                kanzenshuntsu_suu += 2;
            }

            // ����456����
            if (tempTehai[i + 2] == 0 && tempTehai[i + 3] == 0 && tempTehai[i + 4] == 2 && tempTehai[i + 5] == 2 && tempTehai[i + 6] == 2 && tempTehai[i + 7] == 0 && tempTehai[i + 8] == 0)
            {
                tempTehai[i + 4] -= 2;
                tempTehai[i + 5] -= 2;
                tempTehai[i + 6] -= 2;
                kanzenshuntsu_suu += 2;
            }

            // ����567����
            if (tempTehai[i + 3] == 0 && tempTehai[i + 4] == 0 && tempTehai[i + 5] == 2 && tempTehai[i + 6] == 2 && tempTehai[i + 7] == 2 && tempTehai[i + 8] == 0 && tempTehai[i + 9] == 0)
            {
                tempTehai[i + 5] -= 2;
                tempTehai[i + 6] -= 2;
                tempTehai[i + 7] -= 2;
                kanzenshuntsu_suu += 2;
            }

            // ����678��
            if (tempTehai[i + 4] == 0 && tempTehai[i + 5] == 0 && tempTehai[i + 6] == 2 && tempTehai[i + 7] == 2 && tempTehai[i + 8] == 2 && tempTehai[i + 9] == 0)
            {
                tempTehai[i + 6] -= 2;
                tempTehai[i + 7] -= 2;
                tempTehai[i + 8] -= 2;
                kanzenshuntsu_suu += 2;
            }

            // ����789
            if (tempTehai[i + 5] == 0 && tempTehai[i + 6] == 0 && tempTehai[i + 7] == 2 && tempTehai[i + 8] == 2 && tempTehai[i + 9] == 2)
            {
                tempTehai[i + 7] -= 2;
                tempTehai[i + 8] -= 2;
                tempTehai[i + 9] -= 2;
                kanzenshuntsu_suu += 2;
            }
        }

        for (i = 0; i < 30; i += 10) // �}���Y���s���Y���\�[�Y
        {
            // 123����
            if (tempTehai[i + 1] == 1 && tempTehai[i + 2] == 1 && tempTehai[i + 3] == 1 && tempTehai[i + 4] == 0 && tempTehai[i + 5] == 0)
            {
                tempTehai[i + 1]--;
                tempTehai[i + 2]--;
                tempTehai[i + 3]--;
                kanzenshuntsu_suu++;
            }

            // ��234����
            if (tempTehai[i + 1] == 0 && tempTehai[i + 2] == 1 && tempTehai[i + 3] == 1 && tempTehai[i + 4] == 1 && tempTehai[i + 5] == 0 && tempTehai[i + 6] == 0)
            {
                tempTehai[i + 2]--;
                tempTehai[i + 3]--;
                tempTehai[i + 4]--;
                kanzenshuntsu_suu++;
            }

            // ����345����
            if (tempTehai[i + 1] == 0 && tempTehai[i + 2] == 0 && tempTehai[i + 3] == 1 && tempTehai[i + 4] == 1 && tempTehai[i + 5] == 1 && tempTehai[i + 6] == 0 && tempTehai[i + 7] == 0)
            {
                tempTehai[i + 3]--;
                tempTehai[i + 4]--;
                tempTehai[i + 5]--;
                kanzenshuntsu_suu++;
            }

            // ����456����
            if (tempTehai[i + 2] == 0 && tempTehai[i + 3] == 0 && tempTehai[i + 4] == 1 && tempTehai[i + 5] == 1 && tempTehai[i + 6] == 1 && tempTehai[i + 7] == 0 && tempTehai[i + 8] == 0)
            {
                tempTehai[i + 4]--;
                tempTehai[i + 5]--;
                tempTehai[i + 6]--;
                kanzenshuntsu_suu++;
            }

            // ����567����
            if (tempTehai[i + 3] == 0 && tempTehai[i + 4] == 0 && tempTehai[i + 5] == 1 && tempTehai[i + 6] == 1 && tempTehai[i + 7] == 1 && tempTehai[i + 8] == 0 && tempTehai[i + 9] == 0)
            {
                tempTehai[i + 5]--;
                tempTehai[i + 6]--;
                tempTehai[i + 7]--;
                kanzenshuntsu_suu++;
            }

            // ����678��
            if (tempTehai[i + 4] == 0 && tempTehai[i + 5] == 0 && tempTehai[i + 6] == 1 && tempTehai[i + 7] == 1 && tempTehai[i + 8] == 1 && tempTehai[i + 9] == 0)
            {
                tempTehai[i + 6]--;
                tempTehai[i + 7]--;
                tempTehai[i + 8]--;
                kanzenshuntsu_suu++;
            }

            // ����789
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
    /// ���S�Ǘ��v�𔲂��o���Č���Ԃ�
    /// </summary>
    /// <returns></returns>
    private int KanzenKoritsuCheck()
    {
        int KanzenKoritsu_suu = 0;
        int i, j;

        // ���v�̊��S�Ǘ��v�𔲂��o��
        for (i = 31; i < 38; i++)
        {
            if (tempTehai[i] == 1)
            {
                tempTehai[i]--;
                KanzenKoritsu_suu++;
            }
        }

        // ���v�̊��S�Ǘ��v�𔲂��o��
        for (i = 0; i < 30; i += 10) // �}���Y���s���Y���\�[�Y
        {
            // 1�̌Ǘ��v�𔲂�
            if (tempTehai[i + 1] == 1 && tempTehai[i + 2] == 0 && tempTehai[i + 3] == 0)
            {
                tempTehai[i + 1]--;
                KanzenKoritsu_suu++;
            }

            // 2�̌Ǘ��v�𔲂�
            if (tempTehai[i + 1] == 0 && tempTehai[i + 2] == 1 && tempTehai[i + 3] == 0 && tempTehai[i + 4] == 0)
            {
                tempTehai[i + 2]--;
                KanzenKoritsu_suu++;
            }

            // 3~7�̌Ǘ��v�𔲂�
            for (j = 0; j < 5; j++)
            {
                if (tempTehai[i + j + 1] == 0 && tempTehai[i + j + 2] == 0 && tempTehai[i + j + 3] == 1 && tempTehai[i + j + 4] == 0 && tempTehai[i + j + 5] == 0)
                {
                    tempTehai[i + j + 3]--;
                    KanzenKoritsu_suu++;
                }
            }

            // 8�̌Ǘ��v�𔲂�
            if (tempTehai[i + 6] == 0 && tempTehai[i + 7] == 0 && tempTehai[i + 8] == 1 && tempTehai[i + 9] == 0)
            {
                tempTehai[i + 8]--;
                KanzenKoritsu_suu++;
            }

            // 9�̌Ǘ��v�𔲂�
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

    //�}���Y�֘A
    private int manMentsuMax;
    private int manTaatsuMax;

    //�s���Y�֘A
    private int pinMentsuMax;
    private int pinTaatsuMax;

    //�\�[�Y�֘A
    private int souMentsuMax;
    private int souTaatsuMax;

    //���v�֘A
    private int jiTaatsuMax;

    //�R�[�c���J�E���g�p
    private int koutsuCount;

    //kanzen_koutsu_suu��kanzen_shuntsu_suu�i�[�p
    private int preMentsuCount;

    // ��v���
    private int[] tehai;

    //tempTehai�̑O�����p�N���[���B���O�Ɋ��S�����c��Ǘ��v�𔲂��Ă���
    private int[] preTempTehai;

    //tehai�z��̃N���[��
    private int[] tempTehai;


    public int SyantenCheckAll(int[] _tehaiInformation)
    {
        InitValue(_tehaiInformation);

        int syantenkokusi = SyantenKokusi(); // ���m���o�̃V�����e�������Z�o����֐��̌Ăяo��
        if (syantenkokusi == 0 || syantenkokusi == -1)
        {
            return syantenkokusi;
        }

        int syanten7toitsu = Syanten7Toitsu(); // �`�[�g�C�c�̃V�����e�������Z�o����֐��̌Ăяo��
        if (syanten7toitsu == 0 || syanten7toitsu == -1)
        {
            return syanten7toitsu;
        }

        int syanten_suu = SyantenCheck(0); // �ʏ��̃V�����e�������Z�o����֐��̌Ăяo��

        return Math.Min(Math.Min(syantenkokusi, syanten7toitsu), syanten_suu);
    }

    /// <summary>
    /// �ϐ��̏�����
    /// </summary>
    /// <param name="_tehaiInformation"></param>
    private void InitValue(int[] _tehaiInformation)
    {
        // ��v���̃R�s�[
        tehai = new int[38];
        Array.Copy(_tehaiInformation, tehai, 38);

        // �O���[�o���ϐ��̏�����
        manMentsuMax = 0;
        manTaatsuMax = 0;
        pinMentsuMax = 0;
        pinTaatsuMax = 0;
        souMentsuMax = 0;
        souTaatsuMax = 0;
        jiTaatsuMax = 0;
        koutsuCount = 0;

        // tehai�z��̃o�b�N�A�b�v�����
        preTempTehai = new int[38];
        preTempTehai = (int[])tehai.Clone();
    }

    /// <summary>
    /// ���m���o�̃A�K������ƃV�����e������Ԃ�
    /// </summary>
    /// <returns></returns>
    private int SyantenKokusi()
    {
        int syantenKokusi = 13;
        int toitsuSuu = 0; // �����̐�

        // 19�v���`�F�b�N
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

        // ���v���`�F�b�N
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

        // ����������ꍇ�̏���
        syantenKokusi -= toitsuSuu;
        return syantenKokusi;
    }

    /// <summary>
    /// �`�[�g�C�c�̃A�K������ƃV�����e������Ԃ�
    /// </summary>
    /// <returns></returns>
    private int Syanten7Toitsu()
    {
        int toitsuSuu = 0; // �Ύq��
        int syuruiSuu = 0; // �v�̎�ސ�
        int syanten7Toitsu = 6; // ���Ύq�̃V�����e����

        // �Ύq�𐔂���
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

        // ���Ύq�̃V�����e�������v�Z
        syanten7Toitsu = 6 - toitsuSuu;

        // 4���`�[�g�C�c�̉��
        if (syuruiSuu < 7)
        {
            syanten7Toitsu = 7 - syuruiSuu;
        }

        return syanten7Toitsu;
    }

    /// <summary>
    /// �ʏ��̃A�K������ƃV�����e������Ԃ�
    /// </summary>
    /// <param name="tehaimode"></param>
    /// <returns></returns>
    private int SyantenCheck(int tehaimode)
    {
        int toitsu_suu = 0; // ����
        int syanten_temp = 0; // �V�����e�����i�v�Z�p�j
        int syanten_suu = 8;

        int manMentsuTaatsu_suu = 0;
        int pinMentsuTaatsu_suu = 0;
        int souMentsuTaatsu_suu = 0;
        int jiTaatsu_suu = 0;

        // �O�����Ċ��S�ȃV�����c�E�R�[�c�E�Ǘ��v�𔲂��Ă���
        int kanzen_koutsu_suu = KanzenKoutsuCheck(); // ���S�ɓƗ������R�[�c�𔲂��o���Č���Ԃ��֐��̎��s
        int kanzen_shuntsu_suu = KanzenShuntsuCheck(); // ���S�ɓƗ������V�����c�𔲂��o���Č���Ԃ��֐��̎��s
        preMentsuCount = kanzen_koutsu_suu + kanzen_shuntsu_suu; // �O���[�o���ϐ��Ɋi�[

        // 5���ڂ̒P�R�҂���j�~���鏈�u
        int kanzen_Koritsu_suu = KanzenKoritsuCheck(); // ���S�ɓƗ������Ǘ��v�𔲂��o���Č���Ԃ��֐��̎��s

        // �Ɨ��������S�g�C�c�̃`�F�b�N
        int kanzen_toitsu_check = KanzenToitsuCheck(); // ����ꍇ�́u1�v�A�����ꍇ�́u0�v��Ԃ�

        // �}���Y�E�s���Y�E�\�[�Y�̗L���̃`�F�b�N
        int isCheckMan = preTempTehai[1] + preTempTehai[2] + preTempTehai[3] + preTempTehai[4] + preTempTehai[5] + preTempTehai[6] + preTempTehai[7] + preTempTehai[8] + preTempTehai[9];
        int isCheckPin = preTempTehai[11] + preTempTehai[12] + preTempTehai[13] + preTempTehai[14] + preTempTehai[15] + preTempTehai[16] + preTempTehai[17] + preTempTehai[18] + preTempTehai[19];
        int isCheckSou = preTempTehai[21] + preTempTehai[22] + preTempTehai[23] + preTempTehai[24] + preTempTehai[25] + preTempTehai[26] + preTempTehai[27] + preTempTehai[28] + preTempTehai[29];
        int isCheckJi = preTempTehai[31] + preTempTehai[32] + preTempTehai[33] + preTempTehai[34] + preTempTehai[35] + preTempTehai[36] + preTempTehai[37];
        int adjustment = kanzen_koutsu_suu + kanzen_shuntsu_suu;

        // �y��������z���������o�����R�[�c�����o�����V�����c�����o�����^�[�c��┲���o��
        for (int i = 1; i < 38; i++)
        {
            if (i % 10 == 0) continue;

            if (preTempTehai[i] >= 2)
            {
                preTempTehai[i] -= 2; // �����𔲂��o��
                int jantou = i; // �����̔ԍ����i�[����
                toitsu_suu = 1; // �������J�E���g����

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

                // �V�����e�����̎Z�o
                syanten_temp = SyantenHantei(toitsu_suu, adjustment);
                if (syanten_suu > syanten_temp) syanten_suu = syanten_temp;
                if (syanten_suu == 0 && (isCheckMan + isCheckPin + isCheckSou + preMentsuCount * 3) == tehaimode)
                {
                    return 0; // �e���p�C����
                }
                if (syanten_suu == -1)
                {
                    return -1; // �A�K������
                }

                preTempTehai[i] += 2;
                toitsu_suu--;
            }
        }

        // �y��������z���������o�����V�����c�����o�����R�[�c�����o�����^�[�c��┲���o��
        if (koutsuCount > 0)
        {
            for (int i = 1; i < 38; i++)
            {
                if (i % 10 == 0) continue;

                if (preTempTehai[i] >= 2)
                {
                    preTempTehai[i] -= 2; // �����𔲂��o��
                    int jantou = i; // �����̔ԍ����i�[����
                    toitsu_suu = 1; // �������J�E���g����

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

                    // �V�����e�����̎Z�o
                    syanten_temp = SyantenHantei(toitsu_suu, adjustment);
                    if (syanten_suu > syanten_temp) syanten_suu = syanten_temp;
                    if (syanten_suu == 0 && (isCheckMan + isCheckPin + isCheckSou + preMentsuCount * 3) == tehaimode)
                    {
                        return 0; // �e���p�C����
                    }
                    if (syanten_suu == -1)
                    {
                        return -1; // �A�K������
                    }

                    preTempTehai[i] += 2;
                    toitsu_suu--;
                }
            }
        }

        // �y���������z�R�[�c�����o�����V�����c�����o�����^�[�c��┲���o��
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

            // �V�����e�����̎Z�o
            syanten_temp = SyantenHantei(toitsu_suu, adjustment);
            if (syanten_suu > syanten_temp) syanten_suu = syanten_temp;
            if (syanten_suu == 0 && (isCheckMan + isCheckPin + isCheckSou + preMentsuCount * 3) == tehaimode)
            {
                return 0; // �e���p�C����
            }
            if (syanten_suu == -1)
            {
                return -1; // �A�K������
            }
        }

        return syanten_suu; // �V�����e������Ԃ�
    }

    /// <summary>
    /// �V�����e�������Z�o����
    /// </summary>
    /// <param name="toitsu_suu"></param>
    /// <param name="adjustment"></param>
    /// <returns></returns>
    private int SyantenHantei(int toitsu_suu, int adjustment)
    {
        int syanten_temp = 0;
        int block_suu = 0;

        // �u���b�N���̌v�Z
        block_suu = (manMentsuMax + pinMentsuMax + souMentsuMax + adjustment) +
                    (manTaatsuMax + pinTaatsuMax + souTaatsuMax + jiTaatsuMax);

        // �V�����e�����Z�o
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
    /// ���v�^�[�c�����o��
    /// </summary>
    /// <returns></returns>
    private int jiTaatuCheck()
    {
        int jiTaatsu = 0;

        for (int i = 31; i < 38; i++)
        {
            if (tempTehai[i] == 2)
            {
                tempTehai[i] -= 2; // �g�C�c�𔲂��o��
                jiTaatsu++;
            }
        }

        jiTaatsuMax = jiTaatsu;

        return jiTaatsu;
    }

    /// <summary>
    /// �}���Y�̃R�[�c�𔲂��o���Č���Ԃ�
    /// </summary>
    /// <param name="n">n=1:�ŏ��Ɍ��������R�[�c��1��������</param>
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

                // �un=1�v�̎��A1�񂾂��R�[�c�𔲂��ď����𔲂���
                if (n == 1)
                {
                    return koutsu_suu;
                }
            }
        }

        return koutsu_suu;
    }

    /// <summary>
    /// �}���Y�̃R�[�c�E�V�����c�E�^�[�c�𔲂��o���Č���Ԃ�
    /// </summary>
    /// <param name="n">n=1:�R�[�c���V�����c���^�[�c�̏��ɔ��� , n=2:�V�����c���R�[�c���^�[�c�̏��ɔ��� , n=3:1�R�[�c���V�����c���R�[�c���^�[�c�̏��ɔ���</param>
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
            tempTehai = (int[])preTempTehai.Clone(); // preTempTehai�z��̓��e���R�s�[����

            if (n == 1) { manMentsu += manKoutsuCheck(0); } // �}���Y�̃R�[�c�𔲂��ăR�[�c����Ԃ��֐��̎��s
            if (n == 3) { manMentsu += manKoutsuCheck(1); } // �}���Y�̃R�[�c�𔲂��ăR�[�c����Ԃ��֐��̎��s

            // �}���Y�̃V�����c�𔲂�����
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

            // �}���Y�̃V�����c�𔲂�����2
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

            if (n == 2 || n == 3) { manMentsu += manKoutsuCheck(0); } // �}���Y�̃R�[�c�𔲂��ăR�[�c����Ԃ��֐��̎��s

            // �}���Y�̃^�[�c�𔲂�����
            for (i = 1; i < 8; i++)
            {
                // �g�C�c�̔����o��
                if (tempTehai[i] >= 2 && i < 10)
                {
                    tempTehai[i] -= 2; // �g�C�c�𔲂��o��
                    manTaatsu++;
                }

                // �����������ƃy���`�����̔����o��
                if (tempTehai[i] >= 1 && tempTehai[i + 1] >= 1 && i < 9)
                {
                    tempTehai[i]--;
                    tempTehai[i + 1]--;
                    manTaatsu++;
                }

                // �J���`�����̔����o��
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
    /// �s���Y�̃R�[�c�𔲂��o���Č���Ԃ�
    /// </summary>
    /// <param name="n">n=1:�ŏ��Ɍ��������R�[�c��1��������</param>
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

                // �un=1�v�̎��A1�񂾂��R�[�c�𔲂��ď����𔲂���
                if (n == 1)
                {
                    return koutsu_suu;
                }
            }
        }
        return koutsu_suu;
    }

    /// <summary>
    /// �s���Y�̃R�[�c�E�V�����c�E�^�[�c�𔲂��o���Č���Ԃ�
    /// </summary>
    /// <param name="n">n=1:�R�[�c���V�����c���^�[�c�̏��ɔ��� , n=2:�V�����c���R�[�c���^�[�c�̏��ɔ��� , n=3:1�R�[�c���V�����c���R�[�c���^�[�c�̏��ɔ���</param>
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
            tempTehai = (int[])preTempTehai.Clone(); // preTempTehai�z��̓��e���R�s�[����

            if (n == 1) pinMentsu += pinKoutsuCheck(0); // �s���Y�̃R�[�c�𔲂��ăR�[�c����Ԃ��֐��̎��s
            if (n == 3) pinMentsu += pinKoutsuCheck(1); // �s���Y�̃R�[�c�𔲂��ăR�[�c����Ԃ��֐��̎��s

            // �s���Y�̃V�����c�𔲂�����
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

            // �s���Y�̃V�����c�𔲂�����2
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

            if (n == 2 || n == 3) pinMentsu += pinKoutsuCheck(0); // �s���Y�̃R�[�c�𔲂��ăR�[�c����Ԃ��֐��̎��s

            // �s���Y�̃^�[�c�𔲂�����
            for (int i = 11; i < 18; i++)
            {
                // �g�C�c�̔����o��
                if (tempTehai[i] >= 2 && i < 20)
                {
                    tempTehai[i] -= 2; // �g�C�c�𔲂��o��
                    pinTaatsu++;
                }

                // �����������ƃy���`�����̔����o��
                if (tempTehai[i] > 0 && tempTehai[i + 1] > 0 && i < 19)
                {
                    tempTehai[i]--;
                    tempTehai[i + 1]--;
                    pinTaatsu++;
                }

                // �J���`�����̔����o��
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
    /// �\�[�Y�̃R�[�c�𔲂��o���Č���Ԃ�
    /// </summary>
    /// <param name="n">n=1:�ŏ��Ɍ��������R�[�c��1��������</param>
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

                // �un=1�v�̎��A1�񂾂��R�[�c�𔲂��ď����𔲂���
                if (n == 1)
                {
                    return koutsu_suu;
                }
            }
        }

        return koutsu_suu;
    }

    /// <summary>
    /// �\�[�Y�̃R�[�c�E�V�����c�E�^�[�c�𔲂��o���Č���Ԃ�
    /// </summary>
    /// <param name="n">n=1:�R�[�c���V�����c���^�[�c�̏��ɔ��� , n=2:�V�����c���R�[�c���^�[�c�̏��ɔ��� , n=3:1�R�[�c���V�����c���R�[�c���^�[�c�̏��ɔ���</param>
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
            tempTehai = (int[])preTempTehai.Clone(); // preTempTehai�z��̓��e���R�s�[����

            if (n == 1) souMentsu += souKoutsuCheck(0); // �\�[�Y�̃R�[�c�𔲂��ăR�[�c����Ԃ��֐��̎��s
            if (n == 3) souMentsu += souKoutsuCheck(1); // �\�[�Y�̃R�[�c�𔲂��ăR�[�c����Ԃ��֐��̎��s

            // �\�[�Y�̃V�����c�𔲂�����
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

            // �\�[�Y�̃V�����c�𔲂�����2
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

            if (n == 2 || n == 3) souMentsu += souKoutsuCheck(0); // �\�[�Y�̃R�[�c�𔲂��ăR�[�c����Ԃ��֐��̎��s

            // �\�[�Y�̃^�[�c�𔲂�����
            for (int i = 21; i < 28; i++)
            {
                // �g�C�c�̔����o��
                if (tempTehai[i] >= 2 && i < 30)
                {
                    tempTehai[i] -= 2; // �g�C�c�𔲂��o��
                    souTaatsu++;
                }

                // �����������ƃy���`�����̔����o��
                if (tempTehai[i] >= 1 && tempTehai[i + 1] >= 1 && i < 29)
                {
                    tempTehai[i]--;
                    tempTehai[i + 1]--;
                    souTaatsu++;
                }

                // �J���`�����̔����o��
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
    /// ���S�R�[�c�𔲂��o���Č���Ԃ�
    /// </summary>
    /// <returns></returns>
    private int KanzenKoutsuCheck()
    {
        int Kanzenkoutsu_suu = 0;
        int i, j;

        // ���v�̊��S�R�[�c�𔲂��o��
        for (i = 31; i < 38; i++)
        {
            if (preTempTehai[i] >= 3)
            {
                preTempTehai[i] -= 3;
                Kanzenkoutsu_suu++;
            }
        }

        // ���v�̊��S�R�[�c�𔲂��o��
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

            // 3~7�̊��S�R�[�c�𔲂�
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
    /// ���S�V�����c�𔲂��o���Č���Ԃ�
    /// </summary>
    /// <returns></returns>
    private int KanzenShuntsuCheck()
    {
        int kanzenshuntsu_suu = 0;
        int i;

        // 123, 456 �̂悤�Ȋ��S�ɓƗ������V�����c�𔲂��o�����߂̏���
        // �y���Ӂz�Ԓn0�C10�C20�C30���u0�v�̕K�v����B���O�ɐԃh�����ړ������鏈�������Ă����B
        for (i = 0; i < 30; i += 10) // �}���Y���s���Y���\�[�Y
        {
            // 123����
            if (preTempTehai[i + 1] == 2 && preTempTehai[i + 2] == 2 && preTempTehai[i + 3] == 2 &&
                preTempTehai[i + 4] == 0 && preTempTehai[i + 5] == 0)
            {
                preTempTehai[i + 1] -= 2;
                preTempTehai[i + 2] -= 2;
                preTempTehai[i + 3] -= 2;
                kanzenshuntsu_suu += 2;
            }

            // ��234����
            if (preTempTehai[i + 1] == 0 && preTempTehai[i + 2] == 2 && preTempTehai[i + 3] == 2 &&
                preTempTehai[i + 4] == 2 && preTempTehai[i + 5] == 0 && preTempTehai[i + 6] == 0)
            {
                preTempTehai[i + 2] -= 2;
                preTempTehai[i + 3] -= 2;
                preTempTehai[i + 4] -= 2;
                kanzenshuntsu_suu += 2;
            }

            // ����345����
            if (preTempTehai[i + 1] == 0 && preTempTehai[i + 2] == 0 && preTempTehai[i + 3] == 2 &&
                preTempTehai[i + 4] == 2 && preTempTehai[i + 5] == 2 && preTempTehai[i + 6] == 0 &&
                preTempTehai[i + 7] == 0)
            {
                preTempTehai[i + 3] -= 2;
                preTempTehai[i + 4] -= 2;
                preTempTehai[i + 5] -= 2;
                kanzenshuntsu_suu += 2;
            }

            // ����456����
            if (preTempTehai[i + 2] == 0 && preTempTehai[i + 3] == 0 && preTempTehai[i + 4] == 2 &&
                preTempTehai[i + 5] == 2 && preTempTehai[i + 6] == 2 && preTempTehai[i + 7] == 0 &&
                preTempTehai[i + 8] == 0)
            {
                preTempTehai[i + 4] -= 2;
                preTempTehai[i + 5] -= 2;
                preTempTehai[i + 6] -= 2;
                kanzenshuntsu_suu += 2;
            }

            // ����567����
            if (preTempTehai[i + 3] == 0 && preTempTehai[i + 4] == 0 && preTempTehai[i + 5] == 2 &&
                preTempTehai[i + 6] == 2 && preTempTehai[i + 7] == 2 && preTempTehai[i + 8] == 0 &&
                preTempTehai[i + 9] == 0)
            {
                preTempTehai[i + 5] -= 2;
                preTempTehai[i + 6] -= 2;
                preTempTehai[i + 7] -= 2;
                kanzenshuntsu_suu += 2;
            }

            // ����678��
            if (preTempTehai[i + 4] == 0 && preTempTehai[i + 5] == 0 && preTempTehai[i + 6] == 2 &&
                preTempTehai[i + 7] == 2 && preTempTehai[i + 8] == 2 && preTempTehai[i + 9] == 0)
            {
                preTempTehai[i + 6] -= 2;
                preTempTehai[i + 7] -= 2;
                preTempTehai[i + 8] -= 2;
                kanzenshuntsu_suu += 2;
            }

            // ����789
            if (preTempTehai[i + 5] == 0 && preTempTehai[i + 6] == 0 && preTempTehai[i + 7] == 2 &&
                preTempTehai[i + 8] == 2 && preTempTehai[i + 9] == 2)
            {
                preTempTehai[i + 7] -= 2;
                preTempTehai[i + 8] -= 2;
                preTempTehai[i + 9] -= 2;
                kanzenshuntsu_suu += 2;
            }
        }

        for (i = 0; i < 30; i += 10) // �}���Y���s���Y���\�[�Y
        {
            // 123����
            if (preTempTehai[i + 1] == 1 && preTempTehai[i + 2] == 1 && preTempTehai[i + 3] == 1 &&
                preTempTehai[i + 4] == 0 && preTempTehai[i + 5] == 0)
            {
                preTempTehai[i + 1]--;
                preTempTehai[i + 2]--;
                preTempTehai[i + 3]--;
                kanzenshuntsu_suu++;
            }

            // ��234����
            if (preTempTehai[i + 1] == 0 && preTempTehai[i + 2] == 1 && preTempTehai[i + 3] == 1 &&
                preTempTehai[i + 4] == 1 && preTempTehai[i + 5] == 0 && preTempTehai[i + 6] == 0)
            {
                preTempTehai[i + 2]--;
                preTempTehai[i + 3]--;
                preTempTehai[i + 4]--;
                kanzenshuntsu_suu++;
            }

            // ����345����
            if (preTempTehai[i + 1] == 0 && preTempTehai[i + 2] == 0 && preTempTehai[i + 3] == 1 &&
                preTempTehai[i + 4] == 1 && preTempTehai[i + 5] == 1 && preTempTehai[i + 6] == 0 &&
                preTempTehai[i + 7] == 0)
            {
                preTempTehai[i + 3]--;
                preTempTehai[i + 4]--;
                preTempTehai[i + 5]--;
                kanzenshuntsu_suu++;
            }

            // ����456����
            if (preTempTehai[i + 2] == 0 && preTempTehai[i + 3] == 0 && preTempTehai[i + 4] == 1 &&
                preTempTehai[i + 5] == 1 && preTempTehai[i + 6] == 1 && preTempTehai[i + 7] == 0 &&
                preTempTehai[i + 8] == 0)
            {
                preTempTehai[i + 4]--;
                preTempTehai[i + 5]--;
                preTempTehai[i + 6]--;
                kanzenshuntsu_suu++;
            }

            // ����567����
            if (preTempTehai[i + 3] == 0 && preTempTehai[i + 4] == 0 && preTempTehai[i + 5] == 1 &&
                preTempTehai[i + 6] == 1 && preTempTehai[i + 7] == 1 && preTempTehai[i + 8] == 0 &&
                preTempTehai[i + 9] == 0)
            {
                preTempTehai[i + 5]--;
                preTempTehai[i + 6]--;
                preTempTehai[i + 7]--;
                kanzenshuntsu_suu++;
            }

            // ����678��
            if (preTempTehai[i + 4] == 0 && preTempTehai[i + 5] == 0 && preTempTehai[i + 6] == 1 &&
                preTempTehai[i + 7] == 1 && preTempTehai[i + 8] == 1 && preTempTehai[i + 9] == 0)
            {
                preTempTehai[i + 6]--;
                preTempTehai[i + 7]--;
                preTempTehai[i + 8]--;
                kanzenshuntsu_suu++;
            }

            // ����789
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
    /// ���S�Ǘ��v�𔲂��o���Č���Ԃ�
    /// </summary>
    /// <returns></returns>
    private int KanzenKoritsuCheck()
    {
        int KanzenKoritsu_suu = 0;
        int i, j;

        // ���v�̊��S�Ǘ��v�𔲂��o��
        for (i = 31; i < 38; i++)
        {
            if (preTempTehai[i] == 1)
            {
                // koritsu = i; // �Ǘ��v��ϐ��Ɋi�[����i�R�����g�A�E�g����Ă��邪�A�K�v�ɉ����Ďg�p�\�j
                preTempTehai[i]--;
                KanzenKoritsu_suu++;
            }
        }

        // ���v�̊��S�Ǘ��v�𔲂��o��
        for (i = 0; i < 30; i += 10) // �}���Y���s���Y���\�[�Y
        {
            // 1�̌Ǘ��v�𔲂�
            if (preTempTehai[i + 1] == 1 && preTempTehai[i + 2] == 0 && preTempTehai[i + 3] == 0)
            {
                // koritsu = i + 1; // �Ǘ��v��ϐ��Ɋi�[����i�R�����g�A�E�g����Ă��邪�A�K�v�ɉ����Ďg�p�\�j
                preTempTehai[i + 1]--;
                KanzenKoritsu_suu++;
            }

            // 2�̌Ǘ��v�𔲂�
            if (preTempTehai[i + 1] == 0 && preTempTehai[i + 2] == 1 && preTempTehai[i + 3] == 0 && preTempTehai[i + 4] == 0)
            {
                preTempTehai[i + 2]--;
                KanzenKoritsu_suu++;
            }

            // 3~7�̊��S�Ǘ��v�𔲂�
            for (j = 0; j < 5; j++)
            {
                if (preTempTehai[i + j + 1] == 0 && preTempTehai[i + j + 2] == 0 && preTempTehai[i + j + 3] == 1 &&
                    preTempTehai[i + j + 4] == 0 && preTempTehai[i + j + 5] == 0)
                {
                    preTempTehai[i + j + 3]--;
                    KanzenKoritsu_suu++;
                }
            }

            // 8�̌Ǘ��v�𔲂�
            if (preTempTehai[i + 6] == 0 && preTempTehai[i + 7] == 0 && preTempTehai[i + 8] == 1 && preTempTehai[i + 9] == 0)
            {
                preTempTehai[i + 8]--;
                KanzenKoritsu_suu++;
            }

            // 9�̌Ǘ��v�𔲂�
            if (preTempTehai[i + 7] == 0 && preTempTehai[i + 8] == 0 && preTempTehai[i + 9] == 1)
            {
                preTempTehai[i + 9]--;
                KanzenKoritsu_suu++;
            }
        }

        return KanzenKoritsu_suu;
    }

    /// <summary>
    /// ���S�g�C�c���`�F�b�N����(�����������ꍇ�̏������X���[���邽�߂̑[�u)
    /// </summary>
    /// <returns>���������ꍇ�́u1�v���A������Ȃ������ꍇ�́u0�v��Ԃ�</returns>
    private int KanzenToitsuCheck()
    {
        int i, j;

        // ���v�̃g�C�c�`�F�b�N
        for (i = 31; i < 38; i++)
        {
            if (preTempTehai[i] == 2)
            {
                return 1;
            }
        }

        // ���v�̊��S�g�C�c���`�F�b�N
        for (i = 0; i < 30; i += 10)
        {
            // 1�̊��S�g�C�c���`�F�b�N
            if (preTempTehai[i + 1] == 2 && preTempTehai[i + 2] == 0)
            {
                return 1;
            }

            // 2�̊��S�g�C�c���`�F�b�N
            if (preTempTehai[i + 1] == 0 && preTempTehai[i + 2] == 2 && preTempTehai[i + 3] == 0)
            {
                return 1;
            }

            // 3~7�̊��S�g�C�c���`�F�b�N
            for (j = 0; j < 5; j++)
            {
                if (preTempTehai[i + j + 2] == 0 && preTempTehai[i + j + 3] == 2 && preTempTehai[i + j + 4] == 0)
                {
                    return 1;
                }
            }

            // 8�̊��S�g�C�c���`�F�b�N
            if (preTempTehai[i + 7] == 0 && preTempTehai[i + 8] == 2 && preTempTehai[i + 9] == 0)
            {
                return 1;
            }

            // 9�̊��S�g�C�c���`�F�b�N
            if (preTempTehai[i + 8] == 0 && preTempTehai[i + 9] == 2)
            {
                return 1;
            }
        }

        return 0;
    }



}
