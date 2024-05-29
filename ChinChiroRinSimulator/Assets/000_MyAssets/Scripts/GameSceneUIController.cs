using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameSceneUIController : MonoBehaviour
{
    [SerializeField] private GameObject menuPanelForGameScenePrefab;
    [SerializeField] private GameObject settingVolumePanelPrefab;
    [SerializeField] private GameObject simulationPanelPrefab;

    [SerializeField] private TextMeshProUGUI yakuText;

    private Transform canvasTF;

    private GameObject activePanelPrefab;


#region Awake�E�C���X�^���X

    public static GameSceneUIController Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

#endregion

    private void Start()
    {
        canvasTF = transform;
    }

    /// <summary>
    /// ���j���[�{�^�����������Ƃ�
    /// </summary>
    public void PushMenuButton()
    {
        if (activePanelPrefab != null) return;
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes);
        activePanelPrefab = Instantiate(menuPanelForGameScenePrefab, canvasTF);
    }

    /// <summary>
    /// �{�����[���ύX�{�^�����������Ƃ�
    /// </summary>
    public void PushSettingVolumeButton()
    {
        if (activePanelPrefab != null) return;
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes);
        activePanelPrefab = Instantiate(settingVolumePanelPrefab, canvasTF);
    }

    /// <summary>
    /// �X�^���o�C�{�^�����������Ƃ�
    /// </summary>
    /// <param name="number"></param>
    public void Test_PushStandByButton(int number)
    {
        DiceController.Instance.Test_ReceptionStandByButton(number);
    }

    /// <summary>
    /// �X�^���o�C�{�^�����������Ƃ�
    /// </summary>
    /// <param name="number"></param>
    public void PushStandByButton()
    {
        DiceController.Instance.ReceptionStandByButton();

        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.StandBy);
    }

    /// <summary>
    /// �΂�U��{�^�����������Ƃ�
    /// </summary>
    public void PushFireButton()
    {
        bool flg = DiceController.Instance.ReceptionFireButton();

        if (flg)
        {
            SoundManager.Instance.PlayDiceRandomSE();
        }
    }

    /// <summary>
    /// �V�~�����[�V�����{�^�����������Ƃ�
    /// </summary>
    public void PushSimulationButton()
    {
        if (activePanelPrefab != null) return;
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes);
        activePanelPrefab = Instantiate(simulationPanelPrefab, canvasTF);
    }

    /// <summary>
    /// ����\������e�L�X�g��ύX����
    /// </summary>
    /// <param name="str">������</param>
    public void ChangeYakuText(string str)
    {
        yakuText.text = str;
    }

    /// <summary>
    /// ����\������e�L�X�g��ύX����
    /// </summary>
    /// <param name="str">������</param>
    /// <param name="color">�F</param>
    public void ChangeYakuText(string str, Color color)
    {
        ChangeYakuText(str);
        yakuText.color = color;
    }

    /// <summary>
    /// ����\������e�L�X�g��ύX����
    /// </summary>
    /// <param name="yaku">��</param>
    public void ChangeYakuText(DiceController.Yaku yaku)
    {
        switch (yaku)
        {
            case DiceController.Yaku.Hifumi:
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.ResultE);
                ChangeYakuText("�q�t�~!!!", Color.red);
                break;
            case DiceController.Yaku.Menashi:
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.ResultD);
                ChangeYakuText("�ږ���!");
                break;
            case DiceController.Yaku.Deme1:
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.ResultC);
                ChangeYakuText("�o�� ��!!", Color.red); 
                break;
            case DiceController.Yaku.Deme2:
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.ResultC);
                ChangeYakuText("�o�� ��!");
                break;
            case DiceController.Yaku.Deme3:
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.ResultC);
                ChangeYakuText("�o�� �Q!");
                break;
            case DiceController.Yaku.Deme4:
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.ResultC);
                ChangeYakuText("�o�� ��!");
                break;
            case DiceController.Yaku.Deme5:
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.ResultC);
                ChangeYakuText("�o�� ��!"); 
                break;
            case DiceController.Yaku.Deme6:
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.ResultC);
                ChangeYakuText("�o�� ��!!", Color.blue);
                break;
            case DiceController.Yaku.Shigoro:
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.ResultB);
                ChangeYakuText("�V�S��!!!", Color.blue); 
                break;
            case DiceController.Yaku.Arashi2:
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.ResultA);
                ChangeYakuText("��]����\n�A���V!!!", Color.blue);
                break;
            case DiceController.Yaku.Arashi3:
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.ResultA);
                ChangeYakuText("�Q�]����\n�A���V!!!", Color.blue);
                break;
            case DiceController.Yaku.Arashi4:
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.ResultA);
                ChangeYakuText("��]����\n�A���V!!!", Color.blue); 
                break;
            case DiceController.Yaku.Arashi5:
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.ResultA);
                ChangeYakuText("�ރ]����\n�A���V!!!", Color.blue);
                break;
            case DiceController.Yaku.Arashi6:
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.ResultA);
                ChangeYakuText("���]����\n�A���V!!!", Color.blue);
                break;
            case DiceController.Yaku.Arashi1:
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.ResultS);
                ChangeYakuText("�s���]����\n�A���V!!!!!", Color.blue);
                break;
            default: ChangeYakuText("�G���[");
                break;
        }
    }

    /// <summary>
    /// Canvas��Transform��Ԃ��Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public Transform GetCanvasTransform()
    {
        return canvasTF;
    }

}
