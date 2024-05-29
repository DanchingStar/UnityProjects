using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject gameMenuPrefab;
    [SerializeField] private GameObject resultForStageClearModePrefab;
    [SerializeField] private GameObject resultForRankingModePrefab;
    [SerializeField] private GameObject longTimeBackGroundPrefab;

    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI missCountText;
    [SerializeField] private TextMeshProUGUI next1Text;
    [SerializeField] private TextMeshProUGUI next2Text;
    [SerializeField] private TextMeshProUGUI restNotesText;

    [SerializeField] private TextMeshProUGUI centerText;

    /// <summary> �^�񒆂̕����̃f�t�H���g�J���[ </summary>
    private readonly Color CENTER_COLOR_DEFAULT = new Color32(255, 0, 0, 255);
    /// <summary> �^�񒆂̕����̏�����J���[ </summary>
    private readonly Color CENTER_COLOR_TRANSPARENT = new Color32(255, 0, 0, 0);
    /// <summary> 0����1���Ƃ�A�傫���Ȃ�ɂ�����Ă��� </summary>
    private float changeColorLeap;
    /// <summary> �F�����̐F�ւƕω�����X�s�[�h </summary>
    private const float CHANGE_COLOR_SPEED = 0.3f;

    [SerializeField] private TextMeshProUGUI testText1;
    [SerializeField] private TextMeshProUGUI testText2;
    [SerializeField] private TextMeshProUGUI testText3;

    [SerializeField] private GameObject skipButtonObject;
    private Button skipButton;
    [SerializeField] private GameObject menuButtonObject;
    private Button menuButton;

    private void Start()
    {
        centerText.color = CENTER_COLOR_TRANSPARENT;
        changeColorLeap = 1f;

        skipButton = skipButtonObject.GetComponent<Button>();
        menuButton = menuButtonObject.GetComponent<Button>();
    }

    private void Update()
    {
        UpdateCenterTextColor();
        SettingButtons();
    }

    /// <summary>
    /// �f�o�b�O�p�e�X�g�{�^���̏���
    /// </summary>
    /// <param name="num"></param>
    public void PushTestButton(int num)
    {
        switch (num)
        {
            case 1:
                //
                break;
            case 2:
                //
                break;
            case 3:
                //
                break;
            case 4:
                //
                break;
            case 5:
                //
                break;
            case 6:
                //
                break;
            case 7:
                //
                break;
            case 8:
                // ��_�J����
                GameSceneManager.Instance.ChangeFixedPointCamera(true);
                break;
            case 9:
                // �Ǐ]�J����
                GameSceneManager.Instance.ChangeFixedPointCamera(false);
                break;
            case 10:
                // ��_�J�����ƒǏ]�J������؂�ւ���
                GameSceneManager.Instance.SwitchFixedPointCamera();
                break;
            case 11:
                // 

                break;
            case 12:
                // 

                break;
            case 13:
                // 

                break;
            case 14:
                // 

                break;
            default:
                Debug.Log("GameSceneManager.PushTestButton : No Test Number");
                break;
        }
    }

    /// <summary>
    /// �f�o�b�O�p�e�L�X�g��ύX����
    /// </summary>
    /// <param name="text"></param>
    /// <param name="number"></param>
    public void SetTestText(string text, int number)
    {
        switch (number)
        {
            case 1:
                testText1.text = text;
                break;
            case 2:
                testText2.text = text;
                break;
            case 3:
                testText3.text = text;
                break;
            default:
                Debug.LogError($"UIManager.SetTestText : Number Error -> {number}");
                break;
        }
    }

    /// <summary>
    /// ���Ԃ̃e�L�X�g��ύX����
    /// </summary>
    /// <param name="num"></param>
    public void SetTimerText(float num)
    {
        timerText.text = "�^�C�� : " + num.ToString("000.0");
    }

    /// <summary>
    /// ���Ԃ̃e�L�X�g��ύX����
    /// </summary>
    /// <param name="str"></param>
    public void SetTimerText(string str)
    {
        timerText.text = str;
    }

    /// <summary>
    /// �c��m�[�c���̃e�L�X�g��ύX����
    /// </summary>
    /// <param name="num"></param>
    public void SetRestNotesText(int num)
    {
        restNotesText.text = $"�m�[�c�� : {num}";
    }

    /// <summary>
    /// �c��m�[�c���̃e�L�X�g��ύX����
    /// </summary>
    /// <param name="str"></param>
    public void SetRestNotesText(string str)
    {
        restNotesText.text = str;
    }

    /// <summary>
    /// �~�X�J�E���g�̃e�L�X�g��ύX����
    /// </summary>
    /// <param name="text"></param>
    public void SetMissCountText(int num)
    {
        missCountText.text = $"�~�X : {num}";
    }

    /// <summary>
    /// �~�X�J�E���g�̃e�L�X�g��ύX����
    /// </summary>
    /// <param name="str"></param>
    public void SetMissCountText(string str)
    {
        missCountText.text = str;
    }

    /// <summary>
    /// �l�N�X�g�̃e�L�X�g��ύX����
    /// </summary>
    /// <param name="text"></param>
    public void SetNext1Text(XylophoneManager.Notes notes)
    {
        if (notes == XylophoneManager.Notes.None)
        {
            next1Text.text = "";
        }
        else
        {
            next1Text.text = XylophoneManager.Instance.GetNotesName(notes);
        }
    }

    /// <summary>
    /// �l�N�l�N�̃e�L�X�g��ύX����
    /// </summary>
    /// <param name="text"></param>
    public void SetNext2Text(XylophoneManager.Notes notes)
    {
        if (notes == XylophoneManager.Notes.None)
        {
            next2Text.text = "";
        }
        else
        {
            next2Text.text = XylophoneManager.Instance.GetNotesName(notes);
        }
    }

    /// <summary>
    /// �^�񒆂̃e�L�X�g�̕�����ݒ肷��
    /// </summary>
    /// <param name="text"></param>
    public void SetCenterText(string text)
    {
        centerText.text = text;
        changeColorLeap = 0;
    }

    /// <summary>
    /// �t���[���[�h���̃e�L�X�g�ύX
    /// </summary>
    /// <param name="level"></param>
    public void SetFreeMode(PlayerInformationManager.GameLevel level)
    {
        SetTimerText("�t���[���[�h");
        SetMissCountText("��Փx  : ");
        SetRestNotesText(level.ToString());
        SetNext1Text(XylophoneManager.Notes.None);
        SetNext2Text(XylophoneManager.Notes.None);

        missCountText.alignment = TextAlignmentOptions.Right;
        restNotesText.alignment = TextAlignmentOptions.Left;
    }

    /// <summary>
    /// �^�񒆂̃e�L�X�g�̐F���X�V����
    /// </summary>
    private void UpdateCenterTextColor()
    {
        if (changeColorLeap > 1f) return;

        changeColorLeap += Time.deltaTime * CHANGE_COLOR_SPEED;
        centerText.color = Color.Lerp(CENTER_COLOR_DEFAULT, CENTER_COLOR_TRANSPARENT, changeColorLeap);

    }

    /// <summary>
    /// Menu�{�^�����������Ƃ�
    /// </summary>
    public void PushMenuButton()
    {
        if (GameSceneManager.Instance.GetPauseFlg()) return;

        GameSceneManager.Instance.ChangePauseFlg(true);
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.GameMenuOpen);
        Instantiate(gameMenuPrefab, this.transform);
    }

    /// <summary>
    /// ���ʉ�ʂ�\������
    /// </summary>
    public void DisplayResultUI()
    {
        var gameMode = GameSceneManager.Instance.GetGameInformation().mode;

        if (gameMode == PlayerInformationManager.GameMode.StageClear)
        {
            Instantiate(resultForStageClearModePrefab, this.transform);
        }
        else if (gameMode == PlayerInformationManager.GameMode.Ranking)
        {
            Instantiate(resultForRankingModePrefab, this.transform);
        }
        else
        {
            Debug.LogError($"UIManager.DisplayResultUI : Mode Error -> {gameMode}");
        }
    }

    /// <summary>
    /// Skip�{�^�����������Ƃ�
    /// </summary>
    public void PushSkipButton()
    {
        GameSceneManager.Instance.ReceptionSkipForExample();
        skipButton.interactable = false;
    }

    /// <summary>
    /// �{�^���̕\���Ȃǂ̐ݒ�
    /// </summary>
    private void SettingButtons()
    {
        if (GameSceneManager.Instance.GetNowPhase() == GameSceneManager.NowPhase.ForExample && skipButtonObject.activeSelf == false)
        {
            skipButtonObject.SetActive(true);
            skipButton.interactable = true;
        }
        else if (GameSceneManager.Instance.GetNowPhase() != GameSceneManager.NowPhase.ForExample && skipButtonObject.activeSelf == true)
        {
            skipButton.interactable = false;
            skipButtonObject.SetActive(false);
        }

        if (GameSceneManager.Instance.GetNowPhase() == GameSceneManager.NowPhase.Play && menuButton.interactable == false)
        {
            menuButton.interactable = true;
        }
        else if (GameSceneManager.Instance.GetNowPhase() != GameSceneManager.NowPhase.Play && menuButton.interactable == true)
        {
            menuButton.interactable = false;
        }
    }

    /// <summary>
    /// LongTimeBackGroundPrefab�𐶐�����
    /// </summary>
    public void InstantiateLongTimeBackGroundPrefab()
    {
        Instantiate(longTimeBackGroundPrefab, transform);
    }

}
