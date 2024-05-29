using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundVolumeController : MonoBehaviour
{
    [SerializeField] private Slider sliderBgm;
    [SerializeField] private Slider sliderSe;

    private void Start()
    {
        InitSlider();
    }

    /// <summary>
    /// �X���C�_�[�̏����ݒ�
    /// </summary>
    private void InitSlider()
    {
        sliderBgm.onValueChanged.AddListener(OnValueChangedForBGM);
        sliderSe.onValueChanged.AddListener(OnValueChangedForSE);

        sliderBgm.value = SoundManager.Instance.GetVolumeForBGM();
        sliderSe.value = SoundManager.Instance.GetVolumeForSE();
    }

    /// <summary>
    /// BGM�X���C�_�[�̒l���ς�����Ƃ�
    /// </summary>
    /// <param name="value">�ς�����l</param>
    private void OnValueChangedForBGM(float value)
    {
        SoundManager.Instance.ChangeVolumeForBGM(value);
    }

    /// <summary>
    /// SE�X���C�_�[�̒l���ς�����Ƃ�
    /// </summary>
    /// <param name="value">�ς�����l</param>
    private void OnValueChangedForSE(float value)
    {
        SoundManager.Instance.ChangeVolumeForSE(value);
    }

    /// <summary>
    /// ���ʐݒ�̕ύX��ۑ�����
    /// </summary>
    public void SaveSoundSetting()
    {
        SoundManager.Instance.SavePlayerPrefs();
    }

    /// <summary>
    /// ����Prefab������Ƃ�
    /// </summary>
    public void PushCloseButton()
    {
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.No);
        SaveSoundSetting();
        Destroy(gameObject);
    }

}
