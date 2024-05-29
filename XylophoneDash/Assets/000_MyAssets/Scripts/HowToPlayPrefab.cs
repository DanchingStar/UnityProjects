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
    /// 選択した内容に変更する
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
    /// 画像・ページ情報を変える
    /// </summary>
    /// <param name="afterIndex">変えたい目次の番号</param>
    private void ChangeImage(int afterIndex)
    {
        index = afterIndex;
        mainImage.sprite = listSprites[index];

        buttonLeft.interactable = index > 0 ? true : false;
        buttonRight.interactable = index < listSprites.Count - 1 ? true : false;
        pageText.text = $"{index + 1} / {listSprites.Count}";
    }

    /// <summary>
    /// 内容選択ボタンを押したとき
    /// </summary>
    /// <param name="number"></param>
    public void PushSelectButton(int number)
    {
        ChangeContents((PlayerInformationManager.GameMode)number);
    }

    /// <summary>
    /// 閉じるボタンを押したとき
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
    /// 確認OKボタンを押したとき
    /// </summary>
    public void PushCloseConfirmationButton()
    {
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Ok);
        GameSceneManager.Instance.ReceptionHowToPlayPrefab();
        Destroy(gameObject);
    }

    /// <summary>
    /// 左ボタンを押したとき
    /// </summary>
    public void PushLeftButton()
    {
        if (index - 1 < 0) return;
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Select);
        ChangeImage(index - 1);
    }

    /// <summary>
    /// 右ボタンを押したとき
    /// </summary>
    public void PushRightButton()
    {
        if (index + 1 >= listSprites.Count) return;
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Select);
        ChangeImage(index + 1);
    }

}
