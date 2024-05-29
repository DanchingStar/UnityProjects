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

        /// <summary> 結果画面の文字列を表示させる時間の間隔 </summary>
        private const float DISPLAY_STRING_DISTANCE = 0.8f;

        private bool getUpFlg = true;


        public void DisplayAndUpdateThisPanel(int bet,int bingo,float ods)
        {
            if (getUpFlg)
            {
                ResetTexts();
                getUpFlg = false;
            }

            texts[0].text = $"ベット : {bet}枚";
            texts[1].text = $"BINGO数 : {bingo}";
            texts[2].text = $"ボーナス倍率 : {ods}倍";
            texts[3].text = "  獲得枚数";
            texts[4].text = $"= {bet}×{bingo}×{ods}";
            texts[5].text = $"= {bet*bingo*ods}枚";

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
        /// 表示する画面を切り替える
        /// </summary>
        /// <param name="flg">true : 通常画面 , false : 盤面確認画面</param>
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
