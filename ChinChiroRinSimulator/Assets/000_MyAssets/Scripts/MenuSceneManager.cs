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
    /// ゲームを始めるボタンを押したとき
    /// </summary>
    public void PushGameStartButton()
    {
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Ok);
        FadeManager.Instance.LoadScene("Game", 0.5f);
    }

    /// <summary>
    /// サイコロ編集ボタンを押したとき
    /// </summary>
    public void PushEditDiceButton()
    {
        if (activePanelPrefab != null) return;
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes);
        activePanelPrefab = Instantiate(editDiceMenuPanelPrefab, canvasTF);
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
    /// クレジットボタンを押したとき
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
