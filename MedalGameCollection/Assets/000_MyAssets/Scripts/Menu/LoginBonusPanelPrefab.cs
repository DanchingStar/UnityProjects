using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Menu
{
    public class LoginBonusPanelPrefab : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI loginDayText;
        [SerializeField] TextMeshProUGUI loginBonusText;

        private const int LOGIN_BONUS_MEDALS = 50;

        /// <summary>
        /// 自分の情報をセットする
        /// </summary>
        /// <param name="partsStatus"></param>
        public void SetMyStatus()
        {
            int loginDays = PlayerInformationManager.Instance.GetLoginDays();

            loginDayText.text = $"ログイン\n{loginDays}日目!";
            loginBonusText.text = $"毎日メダル\n{LOGIN_BONUS_MEDALS}枚プレゼント!!";
        }

        /// <summary>
        /// 閉じるを押したとき
        /// </summary>
        public void PushCloseButton()
        {
            PlayerInformationManager.Instance.AcquisitionMedal(LOGIN_BONUS_MEDALS);
            MenuSceneManager.Instance.UpdateHaveMedalsAndSPCoinsAndPlayerInformation();

            //MenuSceneManager.Instance.SetWaitingLoginBonusFlgFalse();
            SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes1);
            Destroy(this.gameObject);
        }
    }
}
