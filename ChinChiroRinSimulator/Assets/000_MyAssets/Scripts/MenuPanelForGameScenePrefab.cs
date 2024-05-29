using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPanelForGameScenePrefab : MonoBehaviour
{
    [SerializeField] private GameObject settingVolumePanelPrefab;
    [SerializeField] private GameObject customDicePanelPrefab;
    [SerializeField] private GameObject gotoMenuScenePanelPrefab;

    private Transform canvasTF;
    private GameObject activePanelPrefab;

    private void Start()
    {
        canvasTF = GameSceneUIController.Instance.GetCanvasTransform();
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
    /// �T�C�R���ύX�{�^�����������Ƃ�
    /// </summary>
    public void PushChangeDiceButton()
    {
        if (activePanelPrefab != null) return;
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes);
        activePanelPrefab = Instantiate(customDicePanelPrefab, canvasTF);
    }

    /// <summary>
    /// ���j���[�V�[���ɖ߂�{�^�����������Ƃ�
    /// </summary>
    public void PushMenuSceneButton()
    {
        if (activePanelPrefab != null) return;
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes);
        activePanelPrefab = Instantiate(gotoMenuScenePanelPrefab, canvasTF);
    }

    /// <summary>
    /// ����Prefab������Ƃ�
    /// </summary>
    public void PushCloseButton()
    {
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.No);
        Destroy(gameObject);
    }

}
