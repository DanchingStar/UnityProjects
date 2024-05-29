using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiSeManager : MonoBehaviour
{
    [SerializeField] private AudioSource[] seSource;

    private float SE_Volume;

    private GameObject SEObjectParent;

    [Serializable]
    public enum SoundSeType
    {
        SE_Yes,
        SE_No,
        SE_OK,
    }

    private void Start()
    {
        SEObjectParent = this.gameObject;

        SE_Volume = -1;

        VolumeChange();
    }

    private void Update()
    {
        if (UICommonPanelManager.Instance.GetIsActiveSoundSettingPanel())
        {
            VolumeChange();
        }
    }

    /// <summary>
    /// SEÇçƒê∂Ç∑ÇÈ
    /// </summary>
    /// <param name="seType"></param>
    public void PlaySE(SoundSeType seType)
    {
        seSource[(int)seType].Play();
    }

    /// <summary>
    /// SEÇÃÉ{ÉäÉÖÅ[ÉÄÇïœçXÇ∑ÇÈ
    /// </summary>
    private void VolumeChange()
    {
        if (UICommonPanelManager.Instance.GetSEVolume() != SE_Volume)
        {
            SE_Volume = UICommonPanelManager.Instance.GetSEVolume();
            foreach (Transform childTF in SEObjectParent.transform)
            {
                childTF.gameObject.GetComponent<AudioSource>().volume = SE_Volume;
            }
        }
    }



}
