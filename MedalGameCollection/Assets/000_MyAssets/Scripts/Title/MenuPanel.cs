using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Title
{
    public class MenuPanel : MonoBehaviour
    {
        [SerializeField] private GameObject insidePanel;
        [SerializeField] private GameObject dataRepairPanel;
        [SerializeField] private GameObject creditPanel;

        [SerializeField] private TextMeshProUGUI repairFailureText;
        [SerializeField] private TextMeshProUGUI creditText;
        [SerializeField] private TMP_InputField inputRepairCode;

        private RectTransform rt;

        private float rtWidth;
        private float rtHeight;

        private bool activeFlg;
        private bool soundFlg = false;

        private void Start()
        {
            rt = GetComponent<RectTransform>();

            rtWidth = rt.sizeDelta.x;
            rtHeight = rt.sizeDelta.y;

            CloseAllPanel();

            soundFlg = true;
        }

        /// <summary>
        /// �S�Ă�Menu�̃p�l�������
        /// </summary>
        public void CloseAllPanel()
        {
            ActiveMenuPanel(false);
            ActiveDataRepairPanel(false);
            ActiveCreditPanel(false);
        }

        /// <summary>
        /// �N���W�b�g�e�L�X�g��ǂݍ���ŁA������ɑ������
        /// </summary>
        public void SetCreditText()
        {
            string resourcePath = "Credit/CreditText";

            TextAsset textAsset = Resources.Load<TextAsset>(resourcePath);

            string fileContents = textAsset.text;

            creditText.text = fileContents;
        }

        /// <summary>
        /// ���̃p�l���̃A�N�e�B�u��ύX����
        /// </summary>
        /// <param name="flg"></param>
        public void ActiveMenuPanel(bool flg)
        {
            if (flg)
            {
                rt.sizeDelta = new Vector2(rtWidth, rtHeight);
                insidePanel.SetActive(true);
                activeFlg = true;
            }
            else
            {
                rt.sizeDelta = Vector2.zero;
                insidePanel.SetActive(false);
                activeFlg = false;
            }
        }

        /// <summary>
        /// �A�N�e�B�u���̃t���O��Ԃ��Q�b�^�[
        /// </summary>
        /// <returns></returns>
        public bool GetActiveFlg()
        {
            return activeFlg;
        }

        /// <summary>
        /// DataRepairPanel�̃A�N�e�B�u��ύX
        /// </summary>
        /// <param name="flg"></param>
        public void ActiveDataRepairPanel(bool flg)
        {
            if (flg)
            {
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes);
                DisplayRepairFailureText(false);
                dataRepairPanel.SetActive(true);
            }
            else
            {
                if (soundFlg)
                {
                    SoundManager.Instance.PlaySE(SoundManager.SoundSeType.No);
                }
                dataRepairPanel.SetActive(false);
            }
        }

        /// <summary>
        /// CreditPanel�̃A�N�e�B�u��ύX
        /// </summary>
        /// <param name="flg"></param>
        public void ActiveCreditPanel(bool flg)
        {
            if (flg)
            {
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes);
                creditPanel.SetActive(true);
                SetCreditText();
            }
            else
            {
                if (soundFlg)
                {
                    SoundManager.Instance.PlaySE(SoundManager.SoundSeType.No);
                }
                creditPanel.SetActive(false);
            }
        }

        /// <summary>
        /// �����R�[�h��Ԃ��Q�b�^�[
        /// </summary>
        /// <returns></returns>
        public string GetRepairCode()
        {
            return inputRepairCode.text;
        }

        /// <summary>
        /// �������s�̃e�L�X�g�̕\����\����؂�ւ���
        /// </summary>
        /// <param name="flg"></param>
        public void DisplayRepairFailureText(bool flg)
        {
            repairFailureText.gameObject.SetActive(flg);
        }

    }
}

