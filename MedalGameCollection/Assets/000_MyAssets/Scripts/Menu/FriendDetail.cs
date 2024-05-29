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

        /// <summary> ���ꂼ��̊�p�[�c��Image </summary>
        [SerializeField] private Image[] pertsImages;

        /// <summary> �A�C�R���\���̔{��(400�~500��1.0f) </summary>
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
        /// ����{�^�����������Ƃ�
        /// </summary>
        public void PushCloseButton()
        {
            ResetValues();

            backPanel.SetActive(false);
            detailImage.SetActive(false);
            CloseDialogPanel();
        }

        /// <summary>
        /// ���[�U�[���ڍ׃{�^�����������Ƃ�
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
        /// �e�L�X�g�ƃA�C�R�����Z�b�g����
        /// </summary>
        private void SetTextsAndIcon()
        {
            playerNameText.text = myPlayerInformation.playerName;
            playerLevelText.text = "Level : " + myPlayerInformation.playerLevel.ToString();
            playerExperienceText.text = "Exp : " + myPlayerInformation.playerExperience.ToString();
            playerMedalsText.text = "���_�� : " + myPlayerInformation.playerHaveMedals.ToString();
            playerSpCoinsText.text = "SP�R�C�� : " + myPlayerInformation.playerHaveSpCoins.ToString();
            playerIdText.text = "ID : " + myPlayerInformation.playerId;
            playerLoginTimeText.text = "���O�C�� : " + myPlayerInformation.playerLoginTime;

            for (int i = 0; i < myPlayerInformation.pertsNumber.Length; i++)
            {
                GameObject obj = pertsImages[i].gameObject;
                pertsImages[i].sprite = PlayerInformationManager.Instance.profileListEntries[i].itemList[myPlayerInformation.pertsNumber[i]].sprite;
                obj.GetComponent<Transform>().localPosition =
                    new Vector3(myPlayerInformation.pertsPositionX[i] * scaleMagnification, myPlayerInformation.pertsPositionY[i] * scaleMagnification, 0);
            }
        }

        /// <summary>
        /// �S�Ă̕ϐ��̒l�����Z�b�g����
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
        /// �t�H���[�{�^���Ȃǂ�ݒu����
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
                UnFollowerButton.SetActive(false);//�A���t�H���[����������true�ɂ���
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
        /// �_�C�A���O�p�l����\�����A�e�L�X�g���X�V����
        /// </summary>
        /// <param name="str"></param>
        public void ActiveDialogPanel(string str)
        {
            dialogText.text = str;
            dialogPanel.SetActive(true);
        }

        /// <summary>
        /// �_�C�A���O�p�l�������
        /// </summary>
        public void CloseDialogPanel()
        {
            dialogText.text = "";
            dialogPanel.SetActive(false);
        }

    }
}
