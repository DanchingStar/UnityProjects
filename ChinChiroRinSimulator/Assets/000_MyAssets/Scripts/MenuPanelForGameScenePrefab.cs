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
    /// ボリューム変更ボタンを押したとき
    /// </summary>
    public void PushSettingVolumeButton()
    {
        if (activePanelPrefab != null) return;
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes);
        activePanelPrefab = Instantiate(settingVolumePanelPrefab, canvasTF);
    }

    /// <summary>
    /// サイコロ変更ボタンを押したとき
    /// </summary>
    public void PushChangeDiceButton()
    {
        if (activePanelPrefab != null) return;
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes);
        activePanelPrefab = Instantiate(customDicePanelPrefab, canvasTF);
    }

    /// <summary>
    /// メニューシーンに戻るボタンを押したとき
    /// </summary>
    public void PushMenuSceneButton()
    {
        if (activePanelPrefab != null) return;
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes);
        activePanelPrefab = Instantiate(gotoMenuScenePanelPrefab, canvasTF);
    }

    /// <summary>
    /// このPrefabを閉じたとき
    /// </summary>
    public void PushCloseButton()
    {
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.No);
        Destroy(gameObject);
    }

}
