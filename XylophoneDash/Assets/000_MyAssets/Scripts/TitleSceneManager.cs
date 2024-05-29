using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitleSceneManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI versionText;

    [SerializeField] private TitleSceneInformationTextPanel informationTextPanel;

    [SerializeField] private TitleSceneMenuPanel menuPanel;

    [SerializeField] private AudioClip stageMusicClip;

    [SerializeField] private Image imageTitle;
    [SerializeField] private Image imageXylophone;
    [SerializeField] private TextMeshProUGUI tapScreenText;

    private GameObject fadeManagerObject;

    private string beforeRepairCode;

    private TitleStatus titleStatus;

    private float imageTimer;
    private float textTimer;

    private Color defaultTapStringColor;

    private const float COLOR_SPEED = 1.0f;
    private const float COLOR_RANGE = 1.2f;
    private const float COLOR_A_MIN = 0.2f;
    private const float COLOR_A_MAX = 1.0f;

    private const float SPEED_IMAGE_TITLE = 1.0f;
    private const float SPEED_IMAGE_XYLOPHONE = 0.5f;
    private const float ANGLE_IMAGE_XYLOPHONE = 60f;

    public enum TitleStatus
    {
        BeforeTap,
        Menu,
        Repair,
        Reboot,
        LogIn,
        SaveCheck,
        FailureLogIn,
        Finish,
    }

    public static TitleSceneManager Instance;
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
        imageTimer = 0f;

        beforeRepairCode = "";
        versionText.text = $"Ver.{Application.version}";

        fadeManagerObject = FadeManager.Instance.gameObject;

        defaultTapStringColor = tapScreenText.color;

        InitStageMusic();

        ChangeTitleStatus(TitleStatus.BeforeTap);
    }

    private void Update()
    {
        UpdateImages();

        if (titleStatus == TitleStatus.BeforeTap)
        {
            textTimer += Time.deltaTime;
            float value = Mathf.PingPong(textTimer * COLOR_SPEED, COLOR_RANGE - COLOR_A_MIN) + COLOR_A_MIN;
            if (value > COLOR_A_MAX) value = COLOR_A_MAX;
            tapScreenText.color = new Color(defaultTapStringColor.r, defaultTapStringColor.g, defaultTapStringColor.b, value);
        }
    }

    /// <summary>
    /// �^�C�g���V�[����BGM��ݒ肵�ė���
    /// </summary>
    private void InitStageMusic()
    {
        if (stageMusicClip == null) return;

        SoundManager.Instance.SetStageMusic(stageMusicClip);
        SoundManager.Instance.PlayStageMusic();
    }

    /// <summary>
    /// �摜�𓮂���
    /// </summary>
    private void UpdateImages()
    {
        imageTimer += Time.deltaTime;

        // 0 <= value <= 1
        float valueTitle = (Mathf.Sin(imageTimer * SPEED_IMAGE_TITLE) + 1) / 2;
        float valueXylophone = (Mathf.Sin(imageTimer * SPEED_IMAGE_XYLOPHONE) + 1) / 2;

        imageTitle.transform.localScale = Vector3.one * (valueTitle * 0.2f + 0.8f);
        imageXylophone.transform.rotation = Quaternion.Euler(valueXylophone * ANGLE_IMAGE_XYLOPHONE, 0, 0);

    }

    /// <summary>
    /// �^�C�g����ʂŃ^�b�`�p�p�l�����������Ƃ�
    /// </summary>
    public void TouchTitle()
    {
        if (GetTitleStatus() != TitleStatus.BeforeTap) return;

        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Ok);

        ChangeTitleStatus(TitleStatus.LogIn);

        PlayFabManager.Instance.Login();

    }

    /// <summary>
    /// ���O�C��������������Ɏ��s����
    /// </summary>
    public void AfterLogIn(bool newPlayerFlg)
    {
        //Debug.Log($"TitleManager.AfterLogIn : newPlayerFlg = {newPlayerFlg}");

        // if (PlayerInformationManager.Instance.GetNewPlayerFlg())
        if (newPlayerFlg)
        {
            //ChangeTitleStatus(TitleStatus.NameEdit);

            //DisplayNewPlayerPanel(true);

            AfterLogIn(false);
        }
        else
        {
            //ChangeTitleStatus(TitleStatus.UpdateSaveFile);
            //PlayFabManager.Instance.UpdateUserData();

            ChangeTitleStatus(TitleStatus.SaveCheck);
            LoadSavedata();

            // Debug.Log($"TitleManager.TouchTitle : False");
        }
    }

    /// <summary>
    /// �Z�[�u�f�[�^�����[�h����
    /// </summary>
    private void LoadSavedata()
    {
        PlayerInformationManager.Instance.InitAndSettingPlayerInformation();
    }

    /// <summary>
    /// �Z�[�u�f�[�^�����[�h������ɌĂ΂��
    /// </summary>
    public void AfterLoadSaveData(bool nullFlg)
    {
        if (nullFlg)
        {
            AfterLogIn(false);
        }
        else
        {
            ChangeTitleStatus(TitleStatus.Finish);
            MoveMenuScene();
        }
    }

    /// <summary>
    /// ���O�C�����s���ɌĂ΂��
    /// </summary>
    public void FailureLogIn()
    {
        ChangeTitleStatus(TitleStatus.FailureLogIn);
        StartCoroutine(RestartApplication(2f));
    }

    /// <summary>
    /// �������O�C���̌�Ɏ��s����
    /// </summary>
    public void AfterRepairLogin()
    {
        CloseAllMenuPanel();

        PlayFabManager.Instance.SaveNewCustomID(beforeRepairCode);

        AfterRepair();
    }

    /// <summary>
    /// �����f�[�^��DL�̌�Ɏ��s����
    /// </summary>
    /// <param name="repeaiSaveData"></param>
    public void AfterRepair()
    {
        StartCoroutine(RestartApplication(0f));
    }

    /// <summary>
    /// �������s��
    /// </summary>
    public void RepairFailure()
    {
        menuPanel.DisplayRepairFailureText(true);

        informationTextPanel.ActiveInformationTextPanel(false);
        titleStatus = TitleStatus.Menu;
    }

    /// <summary>
    /// ���[�U�[�f�[�^�̃A�b�v���[�h����������Ɏ��s����
    /// </summary>
    public void AfterUpdateUserData()
    {
        Debug.Log($"TitleManager.AfterUpdateUserData");
    }

    /// <summary>
    /// ���j���[�V�[���ֈڂ�
    /// </summary>
    public void MoveMenuScene()
    {

        FadeManager.Instance.LoadScene("Menu", 0.3f);
    }

    /// <summary>
    /// �^�C�g����ʂ̃��j���{�^��(or���j����ʂ���߂�{�^��)���������Ƃ�
    /// </summary>
    /// <param name="flg">true : ���j���{�^�� , false : ���j����ʂ���߂�{�^�� </param>
    public void PushMenuButton(bool flg)
    {
        if (flg)
        {
            //SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes);

            ChangeTitleStatus(TitleStatus.Menu);
        }
        else
        {
            //SoundManager.Instance.PlaySE(SoundManager.SoundSeType.No);

            ChangeTitleStatus(TitleStatus.BeforeTap);
        }
    }

    /// <summary>
    /// ���݂̃X�e�[�^�X�i���[�h�j��ύX����
    /// </summary>
    /// <param name="ts"></param>
    public void ChangeTitleStatus(TitleStatus ts)
    {
        titleStatus = ts;

        if (ts == TitleStatus.BeforeTap)
        {
            textTimer = 0f;
            informationTextPanel.ActiveInformationTextPanel(false);
            informationTextPanel.SetInformationText("BeforeTap");
            if (menuPanel.GetActiveFlg())
            {
                menuPanel.ActiveMenuPanel(false);
            }
        }
        else if (ts == TitleStatus.Menu)
        {
            informationTextPanel.ActiveInformationTextPanel(false);
            informationTextPanel.SetInformationText("Menu");
            menuPanel.ActiveMenuPanel(true);
        }
        else if (ts == TitleStatus.Repair)
        {
            informationTextPanel.ActiveInformationTextPanel(true);
            informationTextPanel.SetInformationText("�f�[�^������");
        }
        else if (ts == TitleStatus.Reboot)
        {
            informationTextPanel.ActiveInformationTextPanel(true);
            informationTextPanel.SetInformationText("�A�v����\n�ċN����");
        }
        else if (ts == TitleStatus.LogIn)
        {
            informationTextPanel.ActiveInformationTextPanel(true);
            informationTextPanel.SetInformationText("���O�C����");
        }
        else if (ts == TitleStatus.SaveCheck)
        {
            informationTextPanel.ActiveInformationTextPanel(true);
            informationTextPanel.SetInformationText("�f�[�^�ǂݍ��ݒ�");
        }
        else if (ts == TitleStatus.FailureLogIn)
        {
            informationTextPanel.ActiveInformationTextPanel(true);
            informationTextPanel.SetInformationText("���O�C���Ɏ��s");
        }
        else if (ts == TitleStatus.Finish)
        {
            informationTextPanel.ActiveInformationTextPanel(true);
            informationTextPanel.SetInformationText("����");
        }
    }

    /// <summary>
    /// TitleStatus�̏�Ԃ�Ԃ��Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public TitleStatus GetTitleStatus()
    {
        return titleStatus;
    }

    /// <summary>
    /// Menu�̃f�[�^���������s����{�^�����������Ƃ�
    /// </summary>
    public void PushRepairButton()
    {
        string code = menuPanel.GetRepairCode();

        if (beforeRepairCode == code)
        {
            SoundManager.Instance.PlaySE(SoundManager.SoundSeType.No);
            return;
        }

        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes);

        ChangeTitleStatus(TitleStatus.Repair);

        beforeRepairCode = code;

        PlayFabManager.Instance.RepairLogin(code);
    }

    /// <summary>
    /// �S�Ă�Menu�̃p�l�������
    /// </summary>
    public void CloseAllMenuPanel()
    {
        menuPanel.CloseAllPanel();
    }

    /// <summary>
    /// �A�v�����ċN������
    /// </summary>
    /// <param name="delayTime">�ċN���܂ł̑ҋ@����</param>
    /// <returns></returns>
    public IEnumerator RestartApplication(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        ChangeTitleStatus(TitleStatus.Reboot);

        // DontDestroyOnLoad�Ŏ���Ă���I�u�W�F�N�g��j������
        var objects = FindObjectsOfType<MonoBehaviour>();

        yield return new WaitForSeconds(0.5f);

        foreach (var obj in objects)
        {
            if (obj.gameObject != gameObject && obj.gameObject != fadeManagerObject)
            {
                Destroy(obj.gameObject);
            }
        }

        yield return new WaitForSeconds(0.5f);

        // ���߂���X�^�[�g����V�[����ǂݍ���
        FadeManager.Instance.LoadScene("Title", 0.5f);
    }

}
