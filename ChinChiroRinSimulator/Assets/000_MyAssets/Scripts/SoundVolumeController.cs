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
    /// スライダーの初期設定
    /// </summary>
    private void InitSlider()
    {
        sliderBgm.onValueChanged.AddListener(OnValueChangedForBGM);
        sliderSe.onValueChanged.AddListener(OnValueChangedForSE);

        sliderBgm.value = SoundManager.Instance.GetVolumeForBGM();
        sliderSe.value = SoundManager.Instance.GetVolumeForSE();
    }

    /// <summary>
    /// BGMスライダーの値が変わったとき
    /// </summary>
    /// <param name="value">変わった値</param>
    private void OnValueChangedForBGM(float value)
    {
        SoundManager.Instance.ChangeVolumeForBGM(value);
    }

    /// <summary>
    /// SEスライダーの値が変わったとき
    /// </summary>
    /// <param name="value">変わった値</param>
    private void OnValueChangedForSE(float value)
    {
        SoundManager.Instance.ChangeVolumeForSE(value);
    }

    /// <summary>
    /// 音量設定の変更を保存する
    /// </summary>
    public void SaveSoundSetting()
    {
        SoundManager.Instance.SavePlayerPrefs();
    }

    /// <summary>
    /// このPrefabを閉じたとき
    /// </summary>
    public void PushCloseButton()
    {
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.No);
        SaveSoundSetting();
        Destroy(gameObject);
    }

}
