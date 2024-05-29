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
        /// �����̏����Z�b�g����
        /// </summary>
        /// <param name="partsStatus"></param>
        public void SetMyStatus()
        {
            int loginDays = PlayerInformationManager.Instance.GetLoginDays();

            loginDayText.text = $"���O�C��\n{loginDays}����!";
            loginBonusText.text = $"�������_��\n{LOGIN_BONUS_MEDALS}���v���[���g!!";
        }

        /// <summary>
        /// ������������Ƃ�
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
