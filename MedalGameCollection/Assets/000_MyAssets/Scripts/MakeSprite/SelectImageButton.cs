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
        /// カテゴリを設定する
        /// </summary>
        /// <param name="category"></param>
        public void SetCategory(Category category)
        {
            thisCategory = category;
        }

        /// <summary>
        /// カテゴリを得る
        /// </summary>
        /// <returns></returns>
        public Category GetThisCategory()
        {
            return thisCategory;
        }

        /// <summary>
        /// パーツごとに割り当てられている独自の番号を設定する
        /// </summary>
        /// <param name="number">番号</param>
        public void SetListNumber(int number)
        {
            listNumber = number;
        }

        /// <summary>
        /// パーツごとに割り当てられている独自の番号を返す
        /// </summary>
        /// <returns>番号</returns>
        public int GetListNumber()
        {
            return listNumber;
        }

        /// <summary>
        /// このボタンを押したとき
        /// </summary>
        public void PushThisButton()
        {
            CombineSprites.Instance.ReceptionPushImageButton(thisCategory, listNumber);
        }
    }

}
