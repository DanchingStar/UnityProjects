using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RankingCardPrefab : MonoBehaviour
{
    [SerializeField] private Image insideImage;

    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI scoreText;

    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image frameImage;
    [SerializeField] private Image characterImage;

    private int rankPosition;
    private string playerName;
    private string playerId;
    private float score;

    private readonly Color COLOR_MINE = Color.red;
    private readonly Color COLOR_OTHER = Color.cyan;

    private RankingCardPrefab myRankingCardPrefab;

    public void SetMyStatus(int _rankPosition, string _playerName, string _playerId, float _score)
    {
        rankPosition = _rankPosition;
        playerName = _playerName;
        playerId = _playerId;
        score = _score;

        myRankingCardPrefab = GetComponent<RankingCardPrefab>();

        PlayFabManager.Instance.GetRankingPlayerData(playerId, myRankingCardPrefab);

        DisplaySetting();
    }

    private void DisplaySetting()
    {
        rankText.text = rankPosition.ToString();
        nameText.text = playerName;
        scoreText.text = score.ToString("F2");

        if (playerId == PlayFabManager.Instance.GetPlayFabID())
        {
            insideImage.color = COLOR_MINE;
        }
        else
        {
            insideImage.color= COLOR_OTHER;
        }
    }

    /// <summary>
    /// ���g�̉摜���X�V����(�s�v�Ȉ�����null��OK)
    /// </summary>
    /// <param name="_background">�w�i��Sprite</param>
    /// <param name="_frame">�g��Sprite</param>
    /// <param name="_character">�L�����N�^�[��Sprite</param>
    public void UpdateMySprites(Sprite _background, Sprite _frame, Sprite _character)
    {
        SetBackgroundImage(_background);
        SetFrameImage(_frame);
        SetCharacterImage(_character);
    }

    private void SetBackgroundImage(Sprite sprite)
    {
        if (sprite == null) return;
        backgroundImage.sprite = sprite;
    }

    private void SetFrameImage(Sprite sprite)
    {
        if (sprite == null) return;
        frameImage.sprite = sprite;
    }

    private void SetCharacterImage(Sprite sprite)
    {
        if (sprite == null) return;
        characterImage.sprite = sprite;
    }

    /// <summary>
    /// PlayFab����A�C�R���f�[�^����M����
    /// </summary>
    /// <param name="_BackName"></param>
    /// <param name="_FrameName"></param>
    /// <param name="_CharaName"></param>
    public void ReceptionSuccessFromPlayFab(string _BackName, string _FrameName, string _CharaName)
    {
        UpdateMySprites(
            PlayerInformationManager.Instance.GetIconBackListGenerator().GetSprite(_BackName),
            PlayerInformationManager.Instance.GetIconFrameListGenerator().GetSprite(_FrameName),
            PlayerInformationManager.Instance.GetCharacterListGenerator().GetCharacterSprite(_CharaName)
            );
    }

    public void ReceptionFailureFromPlayFab()
    {

    }

}
