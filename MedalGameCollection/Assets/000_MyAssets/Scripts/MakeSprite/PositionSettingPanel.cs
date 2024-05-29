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
        /// �X���C�_�[�̏�����
        /// </summary>
        private void InitSlider()
        {
            // �X���C�_�[�̒l���ύX���ꂽ���̃C�x���g�Ɋ֐���o�^
            verticalSlider.onValueChanged.AddListener(OnVerticalSliderValueChanged);
            horizontalSlider.onValueChanged.AddListener(OnHorizontalSliderValueChanged);

            // �X���C�_�[�̏����l��ݒ�
            firstSliderValueSetting();

            // �����l�Ńe�L�X�g���X�V
            UpdateTextValues();
        }

        /// <summary>
        /// VerticalSlider�̒l���ς�����Ƃ��ɌĂ�
        /// </summary>
        /// <param name="value">�X���C�_�[�̒l</param>
        private void OnVerticalSliderValueChanged(float value)
        {
            // �X���C�_�[�̒l�𐮐��ɕϊ����ĕێ�
            verticalValue = Mathf.RoundToInt(value);

            // �e�L�X�g���X�V
            UpdateTextValues();

            // �`��ʒu���X�V
            CombineSprites.Instance.MovePartsPosition(myPanelNumber, horizontalValue, verticalValue);
        }

        /// <summary>
        /// HorizontalSlider�̒l���ς�����Ƃ��ɌĂ�
        /// </summary>
        /// <param name="value">�X���C�_�[�̒l</param>
        private void OnHorizontalSliderValueChanged(float value)
        {
            // �X���C�_�[�̒l�𐮐��ɕϊ����ĕێ�
            horizontalValue = Mathf.RoundToInt(value);

            // �e�L�X�g���X�V
            UpdateTextValues();

            // �`��ʒu���X�V
            CombineSprites.Instance.MovePartsPosition(myPanelNumber, horizontalValue, verticalValue);
        }

        /// <summary>
        /// �e�L�X�g���X�V����
        /// </summary>
        private void UpdateTextValues()
        {
            // �e�L�X�g�ɒl��\��
            verticalSliderText.text = verticalValue.ToString();
            horizontalSliderText.text = horizontalValue.ToString();
        }

        /// <summary>
        /// �X���C�_�[�̏����l���f�[�^�x�[�X���瓾�āA�X���C�_�[�ɔ��f������
        /// </summary>
        private void firstSliderValueSetting()
        {
            verticalSlider.value = PlayerInformationManager.Instance.settingPartsPositionV[myPanelNumber];
            horizontalSlider.value = PlayerInformationManager.Instance.settingPartsPositionH[myPanelNumber];

            // Background�̓X���C�_�[�ŕύX�ł��Ȃ��悤�ɂ���
            if (myPanelNumber == 0) 
            {
                verticalSlider.interactable = false;
                horizontalSlider.interactable = false;
            }
        }




    }

}
