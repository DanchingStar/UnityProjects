using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuSceneManager : MonoBehaviour
{
    [SerializeField] private Transform canvasTF;
    [SerializeField] private TextMeshProUGUI versionText;

    [SerializeField] private GameObject editDiceMenuPanelPrefab;
    [SerializeField] private GameObject settingVolumePanelPrefab;
    [SerializeField] private GameObject creditPanelPrefab;

    private GameObject activePanelPrefab;

    public static MenuSceneManager Instance;
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

    private void Start()
    {
        activePanelPrefab = null;
        versionText.text = $"Ver.{Application.version}";
    }

    /// <summary>
    /// �Q�[�����n�߂�{�^�����������Ƃ�
    /// </summary>
    public void PushGameStartButton()
    {
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Ok);
        FadeManager.Instance.LoadScene("Game", 0.5f);
    }

    /// <summary>
    /// �T�C�R���ҏW�{�^�����������Ƃ�
    /// </summary>
    public void PushEditDiceButton()
    {
        if (activePanelPrefab != null) return;
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes);
        activePanelPrefab = Instantiate(editDiceMenuPanelPrefab, canvasTF);
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
    /// �N���W�b�g�{�^�����������Ƃ�
    /// </summary>
    public void PushCreditButton()
    {
        if (activePanelPrefab != null) return;
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes);
        activePanelPrefab = Instantiate(creditPanelPrefab, canvasTF);
    }

    public Transform GetCanvasTransform()
    {
        return canvasTF;
    }



}
