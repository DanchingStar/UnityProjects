using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Menu
{
    public class PlayerInformation : MonoBehaviour
    {
        /// <summary> ��񂪎����̂��̂ł��邩�̃t���O </summary>
        [SerializeField] private bool isMyInformation = false;

        /// <summary> ���ꂼ��̊�p�[�c��Image </summary>
        [SerializeField] private Image[] pertsImages;

        /// <summary> �A�C�R���\���̔{��(400�~500��1.0f) </summary>
        [SerializeField] private float scaleMagnification;

        [SerializeField] private Text playerNameText;
        [SerializeField] private Text playerLevelText;
        [SerializeField] private Text playerExperienceText;
        [SerializeField] private Text playerIdText;
        [SerializeField] private Text playerLoginTimeText;

        [HideInInspector] public string playerName;
        [HideInInspector] public int playerLevel;
        [HideInInspector] public int playerExperience;
        [HideInInspector] public int playerHaveMedals;
        [HideInInspector] public int playerHaveSpCoins;
        [HideInInspector] public string playerId;
        [HideInInspector] public string playerLoginTime;
        [HideInInspector] public int[] pertsNumber;
        [HideInInspector] public int[] pertsPositionX;
        [HideInInspector] public int[] pertsPositionY;
        [HideInInspector] public PlayFab.ClientModels.FriendInfo myFriendInfo;

        private SaveData saveData;

        private bool otherFlg;

        private void Awake()
        {
            otherFlg = false;
        }

        private void Start()
        {
            if (!isMyInformation) return;

            InitArray();
            SetInformation();
            DisplayMyInformation();
        }

        /// <summary>
        /// �K�v�Ȕz��̏�����
        /// </summary>
        private void InitArray()
        {
            pertsNumber = new int[pertsImages.Length];
            pertsPositionX = new int[pertsImages.Length];
            pertsPositionY = new int[pertsImages.Length];
        }

        /// <summary>
        /// �v���C���[�̏��𓾂āA�ϐ��Ɋi�[����
        /// </summary>
        private void SetInformation()
        {
            if (!isMyInformation) return;

            SettingTextValues();

            for (int i = 0; i < pertsNumber.Length; i++)
            {
                pertsNumber[i] = PlayerInformationManager.Instance.settingPartsNumber[i];
                pertsPositionX[i] = PlayerInformationManager.Instance.settingPartsPositionH[i];
                pertsPositionY[i] = PlayerInformationManager.Instance.settingPartsPositionV[i];
            }
        }

        /// <summary>
        /// ���l�̃v���C���[�f�[�^��ۑ�
        /// </summary>
        /// <param name="save">�v���C���[�̃Z�[�u�f�[�^</param>
        /// <param name="playerPlayFabId">�v���C���[��PlayFabID</param>
        public void SetOtherPlayerInformation(SaveData save, string playerPlayFabId, 
            DateTime playerPlayFabLoginTime , PlayFab.ClientModels.FriendInfo friendInfo)
        {
            if (isMyInformation) return;

            InitArray();

            myFriendInfo = friendInfo;

            saveData = save;

            playerId = playerPlayFabId;
            playerLoginTime = GetLoginTimeString(playerPlayFabLoginTime);

            SettingTextValues();

            for (int i = 0; i < pertsNumber.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        pertsNumber[i] = saveData.profileData.myBackgroundNumber;
                        pertsPositionX[i] = 0;
                        pertsPositionY[i] = 0;
                        break;
                    case 1:
                        pertsNumber[i] = saveData.profileData.myOutlineNumber;
                        pertsPositionY[i] = saveData.profileData.myOutlineVerticalPosition;
                        pertsPositionX[i] = saveData.profileData.myOutlineHorizontalPosition;
                        break;
                    case 2:
                        pertsNumber[i] = saveData.profileData.myAccessoryNumber;
                        pertsPositionY[i] = saveData.profileData.myAccessoryVerticalPosition;
                        pertsPositionX[i] = saveData.profileData.myAccessoryHorizontalPosition;
                        break;
                    case 3:
                        pertsNumber[i] = saveData.profileData.myWrinkleNumber;
                        pertsPositionY[i] = saveData.profileData.myWrinkleVerticalPosition;
                        pertsPositionX[i] = saveData.profileData.myWrinkleHorizontalPosition;
                        break;
                    case 4:
                        pertsNumber[i] = saveData.profileData.myEarNumber;
                        pertsPositionY[i] = saveData.profileData.myEarVerticalPosition;
                        pertsPositionX[i] = saveData.profileData.myEarHorizontalPosition;
                        break;
                    case 5:
                        pertsNumber[i] = saveData.profileData.myMouthNumber;
                        pertsPositionY[i] = saveData.profileData.myMouthVerticalPosition;
                        pertsPositionX[i] = saveData.profileData.myMouthHorizontalPosition;
                        break;
                    case 6:
                        pertsNumber[i] = saveData.profileData.myNoseNumber;
                        pertsPositionY[i] = saveData.profileData.myNoseVerticalPosition;
                        pertsPositionX[i] = saveData.profileData.myNoseHorizontalPosition;
                        break;
                    case 7:
                        pertsNumber[i] = saveData.profileData.myEyebrowNumber;
                        pertsPositionY[i] = saveData.profileData.myEyebrowVerticalPosition;
                        pertsPositionX[i] = saveData.profileData.myEyebrowHorizontalPosition;
                        break;
                    case 8:
                        pertsNumber[i] = saveData.profileData.myEyeNumber;
                        pertsPositionY[i] = saveData.profileData.myEyeVerticalPosition;
                        pertsPositionX[i] = saveData.profileData.myEyeHorizontalPosition;
                        break;
                    case 9:
                        pertsNumber[i] = saveData.profileData.myGlassesNumber;
                        pertsPositionY[i] = saveData.profileData.myGlassesVerticalPosition;
                        pertsPositionX[i] = saveData.profileData.myGlassesHorizontalPosition;
                        break;
                    case 10:
                        pertsNumber[i] = saveData.profileData.myHairNumber;
                        pertsPositionY[i] = saveData.profileData.myHairVerticalPosition;
                        pertsPositionX[i] = saveData.profileData.myHairHorizontalPosition;
                        break;
                    default:
                        break;
                }
            }

            otherFlg = true;

            DisplayMyInformation();
        }

        /// <summary>
        /// �v���C���[����\������
        /// </summary>
        public void DisplayMyInformation()
        {
            if (!isMyInformation && !otherFlg)
            {
                return;
            }

            for (int i = 0; i < pertsNumber.Length; i++)
            {
                GameObject obj = pertsImages[i].gameObject;
                pertsImages[i].sprite = PlayerInformationManager.Instance.profileListEntries[i].itemList[pertsNumber[i]].sprite;
                obj.GetComponent<Transform>().localPosition =
                    new Vector3(pertsPositionX[i] * scaleMagnification, pertsPositionY[i] * scaleMagnification, 0);
            }

            UpdateTexts();

        }

        /// <summary>
        /// �v���C���[���̃e�L�X�g�\�����X�V����
        /// </summary>
        public void UpdateDisplayInformationText()
        {
            SettingTextValues();
            UpdateTexts();
        }

        /// <summary>
        /// �e�L�X�g�̕ϐ����Z�b�g����
        /// </summary>
        private void SettingTextValues()
        {
            if (isMyInformation)
            {
                playerName = PlayerInformationManager.Instance.GetPlayerName();
                playerLevel = PlayerInformationManager.Instance.GetPlayerLevel();
                playerExperience = PlayerInformationManager.Instance.GetPlayerExperience();
                playerHaveMedals = PlayerInformationManager.Instance.GetHaveMedal();
                playerHaveSpCoins = PlayerInformationManager.Instance.GetSPCoin();
                playerId = PlayFabManager.Instance.GetPlayFabID();
            }
            else
            {
                playerName = saveData.commonData.playerName;
                playerLevel = saveData.commonData.playerLevel;
                playerExperience = saveData.commonData.playerExperience;
                playerHaveMedals = saveData.commonData.haveMedal;
                playerHaveSpCoins = saveData.commonData.haveSPCoin;
            }
        }

        /// <summary>
        /// �e�L�X�g���X�V����
        /// </summary>
        private void UpdateTexts()
        {
            if (playerNameText != null)
            {
                playerNameText.text = playerName;
            }
            if (playerLevelText != null)
            {
                playerLevelText.text = "Level : " + playerLevel.ToString();
            }
            if (playerExperienceText != null)
            {
                playerExperienceText.text = "Exp : " + playerExperience.ToString();
            }
            if (playerIdText != null)
            {
                playerIdText.text = playerId;
            }
            if (playerLoginTimeText != null)
            {
                playerLoginTimeText.text = playerLoginTime;
            }
        }

        /// <summary>
        /// PlayFabId��Ԃ��Q�b�^�[
        /// </summary>
        /// <returns></returns>
        public string GetPlayFabId()
        {
            return playerId;
        }

        /// <summary>
        /// ���O�C�����Ԃɉ����āA�\�����镶�����Ԃ�
        /// </summary>
        /// <param name="loginTime"></param>
        /// <returns></returns>
        private string GetLoginTimeString(DateTime loginTime)
        {
            // ���O�C�����Ԃ���{�̎��ԂɍX�V����
            loginTime = loginTime.AddHours(9);

            // ���ݎ������擾
            DateTime currentTime = DateTime.Now;

            // ���ԍ����v�Z
            TimeSpan timeDifference = currentTime - loginTime;

            // �o�͂��镶����̕ϐ�
            string output;

            // ���ԍ��Ɋ�Â��ď�������
            if (timeDifference.TotalMinutes <= 10)
            {
                output = "10���ȓ�";
            }
            else if (timeDifference.TotalMinutes <= 30)
            {
                output = "30���ȓ�";
            }
            else if (timeDifference.TotalMinutes <= 60)
            {
                output = "60���ȓ�";
            }
            else if (timeDifference.TotalHours <= 24)
            {
                output = (int)timeDifference.TotalHours + "���Ԉȓ�";
            }
            else if (timeDifference.TotalDays <= 7)
            {
                output = (int)timeDifference.TotalDays + "���ȏ�O";
            }
            else
            {
                output = "1�T�Ԉȏ�O";
            }

            return output;
        }



        public void PushMyButton()
        {


            GameObject friengDetailObject = GameObject.Find("FriendDetail");


            FriendDetail friendDetail = friengDetailObject.GetComponent<FriendDetail>();

            friendDetail.PushOpenButton(this);



        }


    }
}
