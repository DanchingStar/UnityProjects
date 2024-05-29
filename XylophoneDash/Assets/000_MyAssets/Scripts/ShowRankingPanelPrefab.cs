using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShowRankingPanelPrefab : MonoBehaviour
{
    [SerializeField] private GameObject scrollViewForAroundTop;
    [SerializeField] private GameObject scrollViewForAroundPlayer;

    [SerializeField] private Transform contentForAroundTop;
    [SerializeField] private Transform contentForAroundPlayer;

    [SerializeField] private TextMeshProUGUI errorText;
    [SerializeField] private Button aroundPlayerButton;

    [SerializeField] private GameObject rankingCardPrefab;

    private bool gameSceneFlg;
    private bool menuSceneFlg;
    private ResultForRankingModePrefab resultForRankingModePrefab;

    private bool displayAroundTopFlg;

    private string rankingKey;


    /// <summary>
    /// ������(�V�[������)
    /// </summary>
    private void InitValue()
    {
        aroundPlayerButton.interactable = false;
        errorText.gameObject.SetActive(false);
    }

    /// <summary>
    /// GameScene���琶������ۂɕK���Ă�
    /// </summary>
    public void SetGameScene(string _key, ResultForRankingModePrefab _resultForRankingModePrefab)
    {
        InitValue();

        resultForRankingModePrefab = _resultForRankingModePrefab;
        rankingKey = _key;
        gameSceneFlg = true;
        menuSceneFlg = false;

        RequestGetRankingToPlayFabManager();

        displayAroundTopFlg = false;
        PushAroundTopButton();
    }

    /// <summary>
    /// MenuScene���琶������ۂɕK���Ă�
    /// </summary>
    public void SetMenuScene(string _key)
    {
        InitValue();

        rankingKey = _key;
        gameSceneFlg = false;
        menuSceneFlg = true;

        RequestGetRankingToPlayFabManager();

        displayAroundTopFlg = false;
        PushAroundTopButton();
    }

    private void RequestGetRankingToPlayFabManager()
    {
        PlayFabManager.Instance.RankingGetLeaderboardAroundTop(rankingKey, gameObject.GetComponent<ShowRankingPanelPrefab>());
        PlayFabManager.Instance.RankingGetLeaderboardAroundPlayer(rankingKey, gameObject.GetComponent<ShowRankingPanelPrefab>());
    }

    /// <summary>
    /// PlayFabManager���珇�ʂ̏��(���g����)�̓ǂݍ��݂̐�������M�����Ƃ�
    /// </summary>
    public void ReceptionSuccessFromLeaderboardAroundPlayer()
    {
        aroundPlayerButton.interactable = true;
    }

    /// <summary>
    /// PlayFabManager���珇�ʂ̏��(���)�̓ǂݍ��݂̎��s����M�����Ƃ�
    /// </summary>
    /// <param name="errorMessage"></param>
    public void ReceptionFailureFromLeaderboardAroundTop(string errorMessage)
    {
        errorText.text = errorMessage;
        errorText.gameObject.SetActive(true);
    }

    /// <summary>
    /// PlayFabManager���珇�ʂ̏�����M�����Ƃ�(���)
    /// </summary>
    /// <param name="rankPosition"></param>
    /// <param name="playerName"></param>
    /// <param name="playerId"></param>
    /// <param name="score"></param>
    public void ReceptionLeaderboardAroundTop(int rankPosition, string playerName, string playerId, float score)
    {
        RankingCardPrefab card = Instantiate(rankingCardPrefab, contentForAroundTop).GetComponent<RankingCardPrefab>();
        card.SetMyStatus(rankPosition, playerName, playerId, score);
    }

    /// <summary>
    /// PlayFabManager���珇�ʂ̏�����M�����Ƃ�(���g����)
    /// </summary>
    /// <param name="rankPosition"></param>
    /// <param name="playerName"></param>
    /// <param name="playerId"></param>
    /// <param name="score"></param>
    public void ReceptionLeaderboardAroundPlayer(int rankPosition, string playerName, string playerId, float score)
    {
        RankingCardPrefab card = Instantiate(rankingCardPrefab, contentForAroundPlayer).GetComponent<RankingCardPrefab>();
        card.SetMyStatus(rankPosition, playerName, playerId, score);
    }

    /// <summary>
    /// ��ʂ̃����L���O��\��������{�^�����������Ƃ�
    /// </summary>
    public void PushAroundTopButton()
    {
        if (displayAroundTopFlg) return;

        displayAroundTopFlg = true;

        scrollViewForAroundTop.SetActive(true);
        scrollViewForAroundPlayer.SetActive(false);
    }

    /// <summary>
    /// ���g���ӂ̃����L���O��\��������{�^�����������Ƃ�
    /// </summary>
    public void PushAroundPlayerButton()
    {
        if (!displayAroundTopFlg) return;

        displayAroundTopFlg = false;

        scrollViewForAroundTop.SetActive(false);
        scrollViewForAroundPlayer.SetActive(true);
    }

    /// <summary>
    /// �߂�{�^�����������Ƃ�
    /// </summary>
    public void PushBackButton()
    {
        if (menuSceneFlg)
        {

        }
        else if (gameSceneFlg)
        {
            resultForRankingModePrefab.ReceptionBackFromShowRankingPanelPrefab();
        }
        else
        {

        }

        Destroy(gameObject);
    }


}
