using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Title
{
    public class InformationTextPanel : MonoBehaviour
    {
        [SerializeField] private GameObject insidePanel;
        [SerializeField] private TextMeshProUGUI informationText;

        private RectTransform rt;

        private float rtWidth;
        private float rtHeight;

        private bool activeFlg;

        private void Start()
        {
            rt = GetComponent<RectTransform>();

            rtWidth= rt.sizeDelta.x;
            rtHeight= rt.sizeDelta.y;

            ActiveInformationTextPanel(false);
        }

        /// <summary>
        /// �e�L�X�g�������̕�����ɂ���
        /// </summary>
        /// <param name="str"></param>
        public void SetInformationText (string str)
        {
            informationText.text = str;
        }

        /// <summary>
        /// ���̃p�l���̃A�N�e�B�u��ύX����
        /// </summary>
        /// <param name="flg"></param>
        public void ActiveInformationTextPanel(bool flg)
        {
            // if (activeFlg == flg) return;

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

    }
}

