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
    /// 初期化(シーン共通)
    /// </summary>
    private void InitValue()
    {
        aroundPlayerButton.interactable = false;
        errorText.gameObject.SetActive(false);
    }

    /// <summary>
    /// GameSceneから生成する際に必ず呼ぶ
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
    /// MenuSceneから生成する際に必ず呼ぶ
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
    /// PlayFabManagerから順位の情報(自身周辺)の読み込みの成功を受信したとき
    /// </summary>
    public void ReceptionSuccessFromLeaderboardAroundPlayer()
    {
        aroundPlayerButton.interactable = true;
    }

    /// <summary>
    /// PlayFabManagerから順位の情報(上位)の読み込みの失敗を受信したとき
    /// </summary>
    /// <param name="errorMessage"></param>
    public void ReceptionFailureFromLeaderboardAroundTop(string errorMessage)
    {
        errorText.text = errorMessage;
        errorText.gameObject.SetActive(true);
    }

    /// <summary>
    /// PlayFabManagerから順位の情報を受信したとき(上位)
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
    /// PlayFabManagerから順位の情報を受信したとき(自身周辺)
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
    /// 上位のランキングを表示させるボタンを押したとき
    /// </summary>
    public void PushAroundTopButton()
    {
        if (displayAroundTopFlg) return;

        displayAroundTopFlg = true;

        scrollViewForAroundTop.SetActive(true);
        scrollViewForAroundPlayer.SetActive(false);
    }

    /// <summary>
    /// 自身周辺のランキングを表示させるボタンを押したとき
    /// </summary>
    public void PushAroundPlayerButton()
    {
        if (!displayAroundTopFlg) return;

        displayAroundTopFlg = false;

        scrollViewForAroundTop.SetActive(false);
        scrollViewForAroundPlayer.SetActive(true);
    }

    /// <summary>
    /// 戻るボタンを押したとき
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
