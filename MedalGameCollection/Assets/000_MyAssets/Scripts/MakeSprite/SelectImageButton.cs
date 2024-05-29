using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MakeSprite
{
    public class SelectImageButton : MonoBehaviour
    {
        public enum Category
        {
            backgroundImage,
            outlineImage, 
            accessoryImage,
            wrinkleImage,
            earImage, 
            mouthImage, 
            noseImage,
            eyebrowImage, 
            eyeImagei,
            glassesImage, 
            hairImage,
            none,
        }

        private Category thisCategory = Category.none;
        private int listNumber = -1;
        public Text rarityText;

        /// <summary>
        /// �J�e�S����ݒ肷��
        /// </summary>
        /// <param name="category"></param>
        public void SetCategory(Category category)
        {
            thisCategory = category;
        }

        /// <summary>
        /// �J�e�S���𓾂�
        /// </summary>
        /// <returns></returns>
        public Category GetThisCategory()
        {
            return thisCategory;
        }

        /// <summary>
        /// �p�[�c���ƂɊ��蓖�Ă��Ă���Ǝ��̔ԍ���ݒ肷��
        /// </summary>
        /// <param name="number">�ԍ�</param>
        public void SetListNumber(int number)
        {
            listNumber = number;
        }

        /// <summary>
        /// �p�[�c���ƂɊ��蓖�Ă��Ă���Ǝ��̔ԍ���Ԃ�
        /// </summary>
        /// <returns>�ԍ�</returns>
        public int GetListNumber()
        {
            return listNumber;
        }

        /// <summary>
        /// ���̃{�^�����������Ƃ�
        /// </summary>
        public void PushThisButton()
        {
            CombineSprites.Instance.ReceptionPushImageButton(thisCategory, listNumber);
        }
    }

}
