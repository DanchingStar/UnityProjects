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
    /// 自身の画像を更新する(不要な引数はnullでOK)
    /// </summary>
    /// <param name="_background">背景のSprite</param>
    /// <param name="_frame">枠のSprite</param>
    /// <param name="_character">キャラクターのSprite</param>
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
    /// PlayFabからアイコンデータを受信する
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
