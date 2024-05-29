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
        /// <summary> 情報が自分のものであるかのフラグ </summary>
        [SerializeField] private bool isMyInformation = false;

        /// <summary> それぞれの顔パーツのImage </summary>
        [SerializeField] private Image[] pertsImages;

        /// <summary> アイコン表示の倍率(400×500で1.0f) </summary>
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
        /// 必要な配列の初期化
        /// </summary>
        private void InitArray()
        {
            pertsNumber = new int[pertsImages.Length];
            pertsPositionX = new int[pertsImages.Length];
            pertsPositionY = new int[pertsImages.Length];
        }

        /// <summary>
        /// プレイヤーの情報を得て、変数に格納する
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
        /// 他人のプレイヤーデータを保存
        /// </summary>
        /// <param name="save">プレイヤーのセーブデータ</param>
        /// <param name="playerPlayFabId">プレイヤーのPlayFabID</param>
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
        /// プレイヤー情報を表示する
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
        /// プレイヤー情報のテキスト表示を更新する
        /// </summary>
        public void UpdateDisplayInformationText()
        {
            SettingTextValues();
            UpdateTexts();
        }

        /// <summary>
        /// テキストの変数をセットする
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
        /// テキストを更新する
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
        /// PlayFabIdを返すゲッター
        /// </summary>
        /// <returns></returns>
        public string GetPlayFabId()
        {
            return playerId;
        }

        /// <summary>
        /// ログイン時間に応じて、表示する文字列を返す
        /// </summary>
        /// <param name="loginTime"></param>
        /// <returns></returns>
        private string GetLoginTimeString(DateTime loginTime)
        {
            // ログイン時間を日本の時間に更新する
            loginTime = loginTime.AddHours(9);

            // 現在時刻を取得
            DateTime currentTime = DateTime.Now;

            // 時間差を計算
            TimeSpan timeDifference = currentTime - loginTime;

            // 出力する文字列の変数
            string output;

            // 時間差に基づいて条件分岐
            if (timeDifference.TotalMinutes <= 10)
            {
                output = "10分以内";
            }
            else if (timeDifference.TotalMinutes <= 30)
            {
                output = "30分以内";
            }
            else if (timeDifference.TotalMinutes <= 60)
            {
                output = "60分以内";
            }
            else if (timeDifference.TotalHours <= 24)
            {
                output = (int)timeDifference.TotalHours + "時間以内";
            }
            else if (timeDifference.TotalDays <= 7)
            {
                output = (int)timeDifference.TotalDays + "日以上前";
            }
            else
            {
                output = "1週間以上前";
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
