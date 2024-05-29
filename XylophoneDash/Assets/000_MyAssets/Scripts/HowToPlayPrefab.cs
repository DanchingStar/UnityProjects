using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HowToPlayPrefab : MonoBehaviour
{
    [SerializeField] private GameObject mainInsideImage;
    [SerializeField] private GameObject confirmationImage;

    [SerializeField] private Button buttonStageClearMode;
    [SerializeField] private Button buttonRankingMode;
    [SerializeField] private Button buttonFreeMode;

    [SerializeField] private Image mainImage;

    [SerializeField] private Button buttonLeft;
    [SerializeField] private Button buttonRight;
    [SerializeField] private TextMeshProUGUI pageText;

    [SerializeField] private Sprite[] stageClearModeSprites;
    [SerializeField] private Sprite[] rankingModeSprites;
    [SerializeField] private Sprite[] freeModeSprites;

    private int index;
    private List<Sprite> listSprites;

    private bool waitFlg;

    private const string SCENE_NAME_TITLE = "Title";
    private const string SCENE_NAME_MENU = "Menu";
    private const string SCENE_NAME_GAME = "Game";

    private void Start()
    {
        waitFlg = false;
        index = 0;
        listSprites = new List<Sprite>();

        if (SceneManager.GetActiveScene().name == SCENE_NAME_GAME)
        {
            ChangeContents(GameSceneManager.Instance.GetGameInformation().mode);
        }
        else
        {
            ChangeContents(PlayerInformationManager.GameMode.None);
        }

        mainInsideImage.SetActive(true);
        confirmationImage.SetActive(false);
    }

    /// <summary>
    /// �I���������e�ɕύX����
    /// </summary>
    /// <param name="mode"></param>
    private void ChangeContents(PlayerInformationManager.GameMode mode)
    {
        if (waitFlg) return;

        waitFlg = true;

        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes);

        buttonStageClearMode.interactable = true;
        buttonRankingMode.interactable = true;
        buttonFreeMode.interactable = true;

        listSprites.Clear();

        if (mode == PlayerInformationManager.GameMode.FreeStyle)
        {
            buttonFreeMode.interactable = false;
            foreach (var item in freeModeSprites)
            {
                listSprites.Add(item);
            }
        }
        else if (mode == PlayerInformationManager.GameMode.Ranking)
        {
            buttonRankingMode.interactable = false;
            foreach (var item in rankingModeSprites)
            {
                listSprites.Add(item);
            }
        }
        else
        {
            buttonStageClearMode.interactable = false;
            foreach (var item in stageClearModeSprites)
            {
                listSprites.Add(item);
            }
        }

        ChangeImage(0);

        waitFlg = false;
    }

    /// <summary>
    /// �摜�E�y�[�W����ς���
    /// </summary>
    /// <param name="afterIndex">�ς������ڎ��̔ԍ�</param>
    private void ChangeImage(int afterIndex)
    {
        index = afterIndex;
        mainImage.sprite = listSprites[index];

        buttonLeft.interactable = index > 0 ? true : false;
        buttonRight.interactable = index < listSprites.Count - 1 ? true : false;
        pageText.text = $"{index + 1} / {listSprites.Count}";
    }

    /// <summary>
    /// ���e�I���{�^�����������Ƃ�
    /// </summary>
    /// <param name="number"></param>
    public void PushSelectButton(int number)
    {
        ChangeContents((PlayerInformationManager.GameMode)number);
    }

    /// <summary>
    /// ����{�^�����������Ƃ�
    /// </summary>
    public void PushCloseButton()
    {
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes);

        if (SceneManager.GetActiveScene().name == SCENE_NAME_TITLE)
        {
            Destroy(gameObject);
        }
        else if (SceneManager.GetActiveScene().name == SCENE_NAME_MENU)
        {
            Destroy(gameObject);
        }
        else if (SceneManager.GetActiveScene().name == SCENE_NAME_GAME)
        {
            if (GameSceneManager.Instance.GetNowPhase() == GameSceneManager.NowPhase.HowToPlay)
            {
                mainInsideImage.SetActive(false);
                confirmationImage.SetActive(true);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// �m�FOK�{�^�����������Ƃ�
    /// </summary>
    public void PushCloseConfirmationButton()
    {
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Ok);
        GameSceneManager.Instance.ReceptionHowToPlayPrefab();
        Destroy(gameObject);
    }

    /// <summary>
    /// ���{�^�����������Ƃ�
    /// </summary>
    public void PushLeftButton()
    {
        if (index - 1 < 0) return;
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Select);
        ChangeImage(index - 1);
    }

    /// <summary>
    /// �E�{�^�����������Ƃ�
    /// </summary>
    public void PushRightButton()
    {
        if (index + 1 >= listSprites.Count) return;
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Select);
        ChangeImage(index + 1);
    }

}
