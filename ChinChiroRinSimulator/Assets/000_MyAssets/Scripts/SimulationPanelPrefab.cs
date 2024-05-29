using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SimulationPanelPrefab : MonoBehaviour
{
    [SerializeField] private Transform contentTF;
    [SerializeField] private TextMeshProUGUI timesText;
    [SerializeField] private Button[] changeTimesButtons;

    [SerializeField] private GameObject simulationResultTextPrefab;
    [SerializeField] private GameObject simulationWaitingTextPrefab;
    [SerializeField] private GameObject adFinishButtonPrefab;

    private SimulationTimes nowTimes;
    private int times;

    private string resultStr1st;
    private string resultStr2nd;

    private Coroutine simulationCoroutine;
    private float simulationCompleteRate;

    private const SimulationTimes DEFAULT_TIMES = SimulationTimes.T_100;
    private const int SERVICE_TIMES = 100;

    private enum SimulationTimes
    {
        T_10,
        T_20,
        T_50,
        T_80,
        T_100,
        T_200,
        T_500,
        T_800,
        T_1000,
        T_2000,
        T_5000,
        T_8000,
        T_10000,
    }

    private void Start()
    {
        simulationCoroutine = null;
        ResetResultString();
        ChangeTimes(DEFAULT_TIMES);
    }

    public void PushDownButtton()
    {
        if (simulationCoroutine != null) return;
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Select);
        if (nowTimes != 0)
        {
            ChangeTimes(nowTimes - 1);
        }
    }

    public void PushUpButtton()
    {
        if (simulationCoroutine != null) return;
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Select);
        if (nowTimes != (SimulationTimes)(Enum.GetValues(typeof(SimulationTimes)).Length - 1)) 
        {
            ChangeTimes(nowTimes + 1);
        }
    }

    public void PushMinButton()
    {
        if (simulationCoroutine != null) return;
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Select);
        ChangeTimes(0);
    }

    public void PushMaxButton()
    {
        if (simulationCoroutine != null) return;
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Select);
        ChangeTimes((SimulationTimes)Enum.GetValues(typeof(SimulationTimes)).Length - 1);
    }

    /// <summary>
    /// �V�~�����[�V�������s�񐔂�ς���
    /// </summary>
    /// <param name="_times">�ς�������</param>
    private void ChangeTimes(SimulationTimes _times)
    {
        nowTimes = _times;
        string[] arr = _times.ToString().Split('_');
        times = int.Parse(arr[1]);
        timesText.text = times.ToString() + " ��";

        if (nowTimes == 0)
        {
            changeTimesButtons[0].interactable = false;
            changeTimesButtons[1].interactable = false;
            changeTimesButtons[2].interactable = true;
            changeTimesButtons[3].interactable = true;
        }
        else if(nowTimes == (SimulationTimes)(Enum.GetValues(typeof(SimulationTimes)).Length - 1))
        {
            changeTimesButtons[0].interactable = true;
            changeTimesButtons[1].interactable = true;
            changeTimesButtons[2].interactable = false;
            changeTimesButtons[3].interactable = false;
        }
        else
        {
            changeTimesButtons[0].interactable = true;
            changeTimesButtons[1].interactable = true;
            changeTimesButtons[2].interactable = true;
            changeTimesButtons[3].interactable = true;
        }
    }

    /// <summary>
    /// ���ʂ̕���������Z�b�g����
    /// </summary>
    private void ResetResultString()
    {
        resultStr2nd = "";
        resultStr1st = "";

        foreach (Transform item in contentTF)
        {
            Destroy(item.gameObject);
        }

        contentTF.parent.parent.gameObject.GetComponent<ScrollRect>().verticalNormalizedPosition = 1;
    }

    /// <summary>
    /// �V�~�����[�V�������s�{�^�����������Ƃ�
    /// </summary>
    /// <param name="_times">���s�����</param>
    public void PushSimulationButton()
    {
        if (simulationCoroutine != null) return;

        if (!DiceController.Instance.GetRollingFlg())
        {
            if (times <= SERVICE_TIMES)
            {
                DoSimulation();
            }
            else
            {
                // �L��������
                AdMobManager.Instance.ShowRewardForSimulation(this);
            }
        }
        else
        {
            Debug.Log("��]���ł�");
        }
    }

    /// <summary>
    /// Reward�L���̎������ʂ���M
    /// </summary>
    /// <param name="flg">true : �����ƌ���</param>
    public void ReceptionAdMobReward(bool flg)
    {
        if (flg)
        {
            bool testFlg = false;
            if (testFlg)
            {
                ResetResultString();
                Instantiate(simulationResultTextPrefab, contentTF).GetComponent<TextMeshProUGUI>().text = $"�L���̎����ɐ������܂����B\n" +
                    $"���L�̃{�^�����������Ƃ�\n�V�~�����[�V�������J�n���܂��B\n\n\n";
                Instantiate(adFinishButtonPrefab, contentTF).GetComponent<Button>().onClick.AddListener(DoSimulation);
            }
            else
            {
                DoSimulation();
            }
        }
        else
        {
            ResetResultString();
            Instantiate(simulationResultTextPrefab, contentTF).GetComponent<TextMeshProUGUI>().text = $"�L���̎����Ɏ��s���܂����B";
        }
    }

    /// <summary>
    /// �V�~�����[�V���������s����
    /// </summary>
    private void DoSimulation()
    {
        SoundManager.Instance.PlayDiceRandomSE();

        simulationCoroutine = StartCoroutine(DoSimulationCoroutine());

     
    }

    /// <summary>
    /// �V�~�����[�V�����̃R���[�`��
    /// </summary>
    /// <returns></returns>
    private IEnumerator DoSimulationCoroutine()
    {
        ResetResultString();

        simulationCompleteRate = 0f;
        var waitTextObject = Instantiate(simulationWaitingTextPrefab, contentTF);
        waitTextObject.GetComponent<SimulationWaitingTextPrefab>().SetMyStatus(this);

        yield return null;

        int padNum = 1; // �����̌������v�Z����
        for (int i = times; i >= 10; i /= 10)
        {
            padNum++;
        }

        int[] yakuCounters = new int[Enum.GetValues(typeof(DiceController.Yaku)).Length];

        for (int i = 0; i < times; i++)
        {
            List<DiceController.DiceEyeKinds> _list = new List<DiceController.DiceEyeKinds>();
            for (int j = 0; j < DiceController.Instance.GetDiceCount(); j++)
            {
                _list.Add(DiceController.Instance.ReceptionSimulation(j));
            }
            DiceController.Yaku yaku = DiceController.Instance.CheckYakuForSimulation(_list);
            yakuCounters[(int)yaku]++;

            resultStr2nd += $"\t{(i + 1)}��� : " +
                $"{(int)_list[0]},{(int)_list[1]},{(int)_list[2]} " +
                $"�� {DiceController.Instance.GetYakuName(yaku)}\n";

            // �ǂ��܂�(?%)�I���������\�����邽�߂̒l���Z�o
            float num = 0f;
            if (times <= 100)
            {
                num = 10f;
            }
            else if (times <= 1000)
            {
                num = 2f;
            }
            else
            {
                num = 1f;
            }
            if (i % (int)(times / (100f/num)) == 0)
            {
                simulationCompleteRate = i / (times / (100f/num)) * num;
                yield return null;
            }
        }

        for (int i = 0; i < Enum.GetValues(typeof(DiceController.Yaku)).Length; i++)
        {
            if (i != 0)
            {
                resultStr1st += $"\t{DiceController.Instance.GetYakuName((DiceController.Yaku)i)}\t : " +
                    $"{yakuCounters[i]} �� " +
                    $"({(100f * yakuCounters[i] / times).ToString("N2")}%)\n";
            }
            else
            {
                resultStr1st += $"�y �V�~�����[�V������ : {times}�� �z\n";
            }
        }

        Debug.Log(resultStr1st);
        Debug.Log(resultStr2nd);

        yield return null;

        Destroy(waitTextObject);
        Instantiate(simulationResultTextPrefab, contentTF).GetComponent<TextMeshProUGUI>().text = resultStr1st;
        Instantiate(simulationResultTextPrefab, contentTF).GetComponent<TextMeshProUGUI>().text = $"\n�y �V�~�����[�V�����ڍ� �z";

        if (times <= 1000)
        {
            Instantiate(simulationResultTextPrefab, contentTF).GetComponent<TextMeshProUGUI>().text = resultStr2nd;
        }
        else
        {
            string[] array2nd = resultStr2nd.Split('\n');
            string newResult2nd = "";
            for (int i = 0; i < times; i++)
            {
                newResult2nd += array2nd[i] + "\n";
                if ((i + 1) % 1000 == 0)
                {
                    Instantiate(simulationResultTextPrefab, contentTF).GetComponent<TextMeshProUGUI>().text = newResult2nd;
                    newResult2nd = "";
                    yield return null;
                }
            }
            if (newResult2nd != "")
            {
                Instantiate(simulationResultTextPrefab, contentTF).GetComponent<TextMeshProUGUI>().text = newResult2nd;
            }
        }

        simulationCoroutine = null;
        yield break;
    }

    /// <summary>
    /// �V�~�����[�V�����̊����x��Ԃ��Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public float GetSimulationCompleteRate()
    {
        return simulationCompleteRate;
    }

    /// <summary>
    /// ����Prefab������Ƃ�
    /// </summary>
    public void PushCloseButton()
    {
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.No);

        if (simulationCoroutine != null) 
        {
            StopCoroutine(simulationCoroutine);
        }

        Destroy(gameObject);
    }
}
