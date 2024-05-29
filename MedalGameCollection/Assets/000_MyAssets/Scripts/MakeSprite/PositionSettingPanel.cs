using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MakeSprite
{
    public class PositionSettingPanel : MonoBehaviour
    {
        public int myPanelNumber;
        public Slider verticalSlider;
        public Slider horizontalSlider;
        public Text verticalSliderText;
        public Text horizontalSliderText;
        public Text partsNameText;

        private int verticalValue;
        private int horizontalValue;

        private void Start()
        {
            InitSlider();

            CombineSprites.Instance.InitPositionSettingPanels(this);
        }

        /// <summary>
        /// スライダーの初期化
        /// </summary>
        private void InitSlider()
        {
            // スライダーの値が変更された時のイベントに関数を登録
            verticalSlider.onValueChanged.AddListener(OnVerticalSliderValueChanged);
            horizontalSlider.onValueChanged.AddListener(OnHorizontalSliderValueChanged);

            // スライダーの初期値を設定
            firstSliderValueSetting();

            // 初期値でテキストを更新
            UpdateTextValues();
        }

        /// <summary>
        /// VerticalSliderの値が変わったときに呼ぶ
        /// </summary>
        /// <param name="value">スライダーの値</param>
        private void OnVerticalSliderValueChanged(float value)
        {
            // スライダーの値を整数に変換して保持
            verticalValue = Mathf.RoundToInt(value);

            // テキストを更新
            UpdateTextValues();

            // 描画位置を更新
            CombineSprites.Instance.MovePartsPosition(myPanelNumber, horizontalValue, verticalValue);
        }

        /// <summary>
        /// HorizontalSliderの値が変わったときに呼ぶ
        /// </summary>
        /// <param name="value">スライダーの値</param>
        private void OnHorizontalSliderValueChanged(float value)
        {
            // スライダーの値を整数に変換して保持
            horizontalValue = Mathf.RoundToInt(value);

            // テキストを更新
            UpdateTextValues();

            // 描画位置を更新
            CombineSprites.Instance.MovePartsPosition(myPanelNumber, horizontalValue, verticalValue);
        }

        /// <summary>
        /// テキストを更新する
        /// </summary>
        private void UpdateTextValues()
        {
            // テキストに値を表示
            verticalSliderText.text = verticalValue.ToString();
            horizontalSliderText.text = horizontalValue.ToString();
        }

        /// <summary>
        /// スライダーの初期値をデータベースから得て、スライダーに反映させる
        /// </summary>
        private void firstSliderValueSetting()
        {
            verticalSlider.value = PlayerInformationManager.Instance.settingPartsPositionV[myPanelNumber];
            horizontalSlider.value = PlayerInformationManager.Instance.settingPartsPositionH[myPanelNumber];

            // Backgroundはスライダーで変更できないようにする
            if (myPanelNumber == 0) 
            {
                verticalSlider.interactable = false;
                horizontalSlider.interactable = false;
            }
        }




    }

}
