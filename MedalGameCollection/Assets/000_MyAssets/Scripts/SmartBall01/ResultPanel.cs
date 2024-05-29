using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SmartBall01
{
    public class ResultPanel : MonoBehaviour
    {
        [SerializeField] private Text[] texts;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button okButton;

        [SerializeField] private GameObject insidePanel;
        [SerializeField] private GameObject insidePanelAnother;
        [SerializeField] private Text anotherText;

        /// <summary> ���ʉ�ʂ̕������\�������鎞�Ԃ̊Ԋu </summary>
        private const float DISPLAY_STRING_DISTANCE = 0.8f;

        private bool getUpFlg = true;


        public void DisplayAndUpdateThisPanel(int bet,int bingo,float ods)
        {
            if (getUpFlg)
            {
                ResetTexts();
                getUpFlg = false;
            }

            texts[0].text = $"�x�b�g : {bet}��";
            texts[1].text = $"BINGO�� : {bingo}";
            texts[2].text = $"�{�[�i�X�{�� : {ods}�{";
            texts[3].text = "  �l������";
            texts[4].text = $"= {bet}�~{bingo}�~{ods}";
            texts[5].text = $"= {bet*bingo*ods}��";

            anotherText.text = texts[1].text;

            StartCoroutine(DisplayTexts());
        }

        private IEnumerator DisplayTexts()
        {
            ChangeDisplayPanel(true);

            yield return null;

            foreach (var text in texts)
            {
                text.gameObject.SetActive(true);
                yield return new WaitForSeconds(DISPLAY_STRING_DISTANCE * GameManager.TIME_SPEED);
            }

            while (PayOut.Instance.GetIsGameWait())
            {
                yield return null;
            }

            confirmButton.gameObject.SetActive(true);
            okButton.gameObject.SetActive(true);
        }

        public void ResetTexts()
        {
            foreach (var text in texts)
            {
                text.text = "Error";
                text.gameObject.SetActive(false);
            }
            confirmButton.gameObject.SetActive(false);
            okButton.gameObject.SetActive(false);
        }

        /// <summary>
        /// �\�������ʂ�؂�ւ���
        /// </summary>
        /// <param name="flg">true : �ʏ��� , false : �Ֆʊm�F���</param>
        public void ChangeDisplayPanel(bool flg)
        {
            if (!flg)
            {
                insidePanel.SetActive(false);
                insidePanelAnother.SetActive(true);
            }
            else
            {
                insidePanel.SetActive(true);
                insidePanelAnother.SetActive(false);
            }
        }

    }
}
