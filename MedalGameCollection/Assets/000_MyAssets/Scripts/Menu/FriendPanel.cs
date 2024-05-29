using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    public class FriendPanel : MonoBehaviour
    {
        [SerializeField] private PlayerInformation myPlayerInformation;

        [SerializeField] private Button followButton;
        [SerializeField] private Button followerButton;
        [SerializeField] private Button searchButton;

        [SerializeField] private GameObject followCard;
        [SerializeField] private GameObject followerCard;
        [SerializeField] private GameObject searchCard;

        [SerializeField] private Button copyMyIdButton;
        [SerializeField] private Text copyCompleteText;

        [SerializeField] private TMP_InputField searchIdInputField;

        [SerializeField] private GameObject scrollContentObject;
        [SerializeField] private GameObject userProfileButtonPrefab;
        [SerializeField] private GameObject userFailurePrefab;

        private CurrentCard currentCard;

        private const string DEFAULT_SEARCH_INPUT_FIELD_TEXT = "ここにIDを入力";

        public enum CurrentCard
        {
            FollowCard,
            FollowerCard,
            SearchCard,
        }

        /// <summary>
        /// パネル情報をリセットする(FrendPanelを開いたときに呼ぶ)
        /// </summary>
        public void ResetFriendPanel()
        {
            ChangeCurrentCard(CurrentCard.FollowCard);

            copyCompleteText.gameObject.SetActive(false);

            searchIdInputField.text = DEFAULT_SEARCH_INPUT_FIELD_TEXT;

            myPlayerInformation.UpdateDisplayInformationText();
        }

        /// <summary>
        /// Cardの表示/非表示を設定する
        /// </summary>
        /// <param name="card"></param>
        /// <param name="flg"></param>
        private void ActiveCard(GameObject card,bool flg)
        {
            card.SetActive(flg);
        }

        /// <summary>
        /// Content内の要素を全削除
        /// </summary>
        private void ResetContent()
        {
            foreach (Transform child in scrollContentObject.transform)
            {
                Destroy(child.gameObject);
            }
        }

        /// <summary>
        /// 表示するパネルを変更する
        /// </summary>
        /// <param name="current"></param>
        private void ChangeCurrentCard(CurrentCard current)
        {
            ResetContent();

            ActiveCard(followCard, false);
            ActiveCard(followerCard, false);
            ActiveCard(searchCard, false);

            currentCard = current;

            if (currentCard == CurrentCard.FollowCard)
            {
                ActiveCard(followCard, true);
            }
            else if (currentCard == CurrentCard.FollowerCard)
            {
                ActiveCard(followerCard, true);
            }
            else if (currentCard == CurrentCard.SearchCard)
            {
                ActiveCard(searchCard, true);
            }
        }

        /// <summary>
        /// Card変更のボタンを押したとき
        /// </summary>
        /// <param name="num"> 0 : FollowCard , 1 : FollowerCard , 2 : SearchCard </param>
        public void PushChangeCardButton(int num)
        {
            if (num > System.Enum.GetValues(typeof(CurrentCard)).Length) return;
            if (num < 0) return;

            CurrentCard nextCard = (CurrentCard)num;

            if (currentCard == nextCard) return;

            ChangeCurrentCard(nextCard);
        }

        /// <summary>
        /// 自分のIDをコピーするボタンを押したとき
        /// </summary>
        public void PushCopyMyIdButton()
        {
            GUIUtility.systemCopyBuffer = myPlayerInformation.GetPlayFabId();
            copyCompleteText.gameObject.SetActive(true);
        }

        /// <summary>
        /// フレンドリストを表示するボタンを押したとき
        /// </summary>
        /// <param name="num"> 0 : FollowCard , 1 : FollowerCard , 2 : SearchCard </param>
        public void PushDisplayCardButton(int num)
        {
            CurrentCard nowCard = (CurrentCard)num;

            ResetContent();

            switch (nowCard)
            {
                case CurrentCard.FollowCard:
                    PlayFabManager.Instance.GetFriends(PlayFabManager.FriendTag.Follow);
                    break;
                case CurrentCard.FollowerCard:
                    // PlayFabManager.Instance.GetFriends(PlayFabManager.FriendTag.Follower);
                    ReceptionSearchIdFailure("申し訳ありませんが\nフォロワー機能は\n実装されておりません。");
                    break;
                case CurrentCard.SearchCard:
                    string str = searchIdInputField.text;
                    PlayFabManager.Instance.GetOtherPlayerProfile(str, null);
                    break;
                default:
                    break;
            }

        }

        /// <summary>
        /// 正常にアカウント情報を取得したとき、Prefabを生成する
        /// </summary>
        /// <param name="save"></param>
        /// <param name="playerPlayFabId"></param>
        /// <param name="playerPlayFabLoginTime"></param>
        public void ReceptionSearchIdSuccess(SaveData save, string playerPlayFabId,
            DateTime playerPlayFabLoginTime, PlayFab.ClientModels.FriendInfo friendInfo)
        {
            GameObject obj = Instantiate(userProfileButtonPrefab, Vector3.zero, Quaternion.identity, scrollContentObject.transform);
            PlayerInformation playerInformation = obj.GetComponent<PlayerInformation>();

            playerInformation.SetOtherPlayerInformation(save, playerPlayFabId, playerPlayFabLoginTime, friendInfo);
            playerInformation.DisplayMyInformation();
        }

        /// <summary>
        /// アカウント情報が異常だった時、異常用のPrefabを生成する
        /// </summary>
        /// <param name="str"></param>
        public void ReceptionSearchIdFailure(string str)
        {
            GameObject obj = Instantiate(userFailurePrefab, Vector3.zero, Quaternion.identity, scrollContentObject.transform);
            UserFailurePrefab prefab = obj.GetComponent<UserFailurePrefab>();

            prefab.SetText(str);
        }


    }
}
