using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Menu
{
    public class FriendDetail : MonoBehaviour
    {
        [SerializeField] private GameObject backPanel;
        [SerializeField] private GameObject detailImage;

        /// <summary> それぞれの顔パーツのImage </summary>
        [SerializeField] private Image[] pertsImages;

        /// <summary> アイコン表示の倍率(400×500で1.0f) </summary>
        [SerializeField] private float scaleMagnification;

        [SerializeField] private TextMeshProUGUI playerNameText;
        [SerializeField] private TextMeshProUGUI playerLevelText;
        [SerializeField] private TextMeshProUGUI playerExperienceText;
        [SerializeField] private TextMeshProUGUI playerMedalsText;
        [SerializeField] private TextMeshProUGUI playerSpCoinsText;
        [SerializeField] private TextMeshProUGUI playerIdText;
        [SerializeField] private TextMeshProUGUI playerLoginTimeText;

        [SerializeField] private GameObject FollowButton;
        [SerializeField] private GameObject UnFollowButton;
        [SerializeField] private GameObject UnFollowerButton;

        [SerializeField] private GameObject dialogPanel;
        [SerializeField] private TextMeshProUGUI dialogText;

        private PlayerInformation myPlayerInformation;

        private void Start()
        {
            PushCloseButton();
        }

        /// <summary>
        /// 閉じるボタンを押したとき
        /// </summary>
        public void PushCloseButton()
        {
            ResetValues();

            backPanel.SetActive(false);
            detailImage.SetActive(false);
            CloseDialogPanel();
        }

        /// <summary>
        /// ユーザー情報詳細ボタンを押したとき
        /// </summary>
        /// <param name="info"></param>
        public void PushOpenButton(PlayerInformation info)
        {
            backPanel.SetActive(true);
            detailImage.SetActive(true);

            myPlayerInformation = info;

            SetTextsAndIcon();
            ArrangementButtons();
        }

        /// <summary>
        /// テキストとアイコンをセットする
        /// </summary>
        private void SetTextsAndIcon()
        {
            playerNameText.text = myPlayerInformation.playerName;
            playerLevelText.text = "Level : " + myPlayerInformation.playerLevel.ToString();
            playerExperienceText.text = "Exp : " + myPlayerInformation.playerExperience.ToString();
            playerMedalsText.text = "メダル : " + myPlayerInformation.playerHaveMedals.ToString();
            playerSpCoinsText.text = "SPコイン : " + myPlayerInformation.playerHaveSpCoins.ToString();
            playerIdText.text = "ID : " + myPlayerInformation.playerId;
            playerLoginTimeText.text = "ログイン : " + myPlayerInformation.playerLoginTime;

            for (int i = 0; i < myPlayerInformation.pertsNumber.Length; i++)
            {
                GameObject obj = pertsImages[i].gameObject;
                pertsImages[i].sprite = PlayerInformationManager.Instance.profileListEntries[i].itemList[myPlayerInformation.pertsNumber[i]].sprite;
                obj.GetComponent<Transform>().localPosition =
                    new Vector3(myPlayerInformation.pertsPositionX[i] * scaleMagnification, myPlayerInformation.pertsPositionY[i] * scaleMagnification, 0);
            }
        }

        /// <summary>
        /// 全ての変数の値をリセットする
        /// </summary>
        private void ResetValues()
        {
            foreach(var i in pertsImages)
            {
                i.sprite = null;
                i.GetComponent<Transform>().localPosition = Vector3.zero;
            }

            playerNameText.text = "";
            playerLevelText.text = "";
            playerExperienceText.text = "";
            playerMedalsText.text = "";
            playerSpCoinsText.text = "";
            playerIdText.text = "";
            playerLoginTimeText.text = "";

            myPlayerInformation = null;
        }

        /// <summary>
        /// フォローボタンなどを設置する
        /// </summary>
        private void ArrangementButtons()
        {
            if (myPlayerInformation.myFriendInfo == null)
            {
                FollowButton.SetActive(true);
                UnFollowButton.SetActive(false);
                UnFollowerButton.SetActive(false);
            }
            else
            {
                FollowButton.SetActive(false);
                UnFollowButton.SetActive(true);
                UnFollowerButton.SetActive(false);//アンフォロー実装したらtrueにする
            }
        }

        public void PushFollowButton()
        {
            PlayFabManager.Instance.AddFriend(myPlayerInformation.playerId);
        }

        public void PushUnFollowButton()
        {
            PlayFabManager.Instance.RemoveFriend(myPlayerInformation.myFriendInfo);
        }

        public void PushUnFollowerButton()
        {

        }

        /// <summary>
        /// ダイアログパネルを表示し、テキストを更新する
        /// </summary>
        /// <param name="str"></param>
        public void ActiveDialogPanel(string str)
        {
            dialogText.text = str;
            dialogPanel.SetActive(true);
        }

        /// <summary>
        /// ダイアログパネルを閉じる
        /// </summary>
        public void CloseDialogPanel()
        {
            dialogText.text = "";
            dialogPanel.SetActive(false);
        }

    }
}
