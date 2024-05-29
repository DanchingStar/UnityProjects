using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectItemButtonPrefab : MonoBehaviour
{
    [SerializeField] private Image myImage;
    [SerializeField] private Image myBackground;

    [SerializeField] private Sprite likeFade;
    [SerializeField] private Sprite charaBack;

    public enum Category
    {
        None,
        Character,
        IconBackground,
        IconFrame,
    }

    private Category thisCategory = Category.None;
    private int listNumber = -1;


    /// <summary>
    /// �J�e�S����ݒ肷��
    /// </summary>
    /// <param name="category"></param>
    public void SetCategory(Category category)
    {
        thisCategory = category;

        //if (thisCategory == Category.IconBackground)
        //{
        //    myBackground.color = new Color(255, 255, 255 , 255);
        //}
        //else
        //{
        //    myBackground.color = new Color(255, 255, 255, 0);
        //}

        myBackground.sprite = thisCategory == Category.Character ? charaBack : likeFade;
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
    /// ���g�̉摜��ݒ肷��
    /// </summary>
    /// <param name="sprite"></param>
    public void SetSprite(Sprite sprite)
    {
        myImage.sprite = sprite;
    }

    /// <summary>
    /// �ݒ�ς݂̏�񂩂玩�g�̉摜��ݒ肷��
    /// </summary>
    public void SetSprite()
    {
        bool errorFlg = false;

        if (listNumber == -1)
        {
            errorFlg = true;
        }
        else if (thisCategory == Category.Character)
        {
            myImage.sprite = PlayerInformationManager.Instance.GetCharacterListGenerator().GetCharacterSprite(listNumber);
        }
        else if (thisCategory == Category.IconBackground)
        {
            myImage.sprite = PlayerInformationManager.Instance.GetIconBackListGenerator().GetSprite(listNumber);
        }
        else if (thisCategory == Category.IconFrame)
        {
            myImage.sprite = PlayerInformationManager.Instance.GetIconFrameListGenerator().GetSprite(listNumber);
        }
        else
        {
            errorFlg = true;
        }

        if(errorFlg)
        {
            Debug.LogError($"SelectItemButtonPrefab.SetSprite : Error\n" +
                $"listNumber = {listNumber} , thisCategory = {thisCategory}");
        }
    }

    /// <summary>
    /// ���̃{�^�����������Ƃ�
    /// </summary>
    public void PushThisButton()
    {
        MenuSceneManager.Instance.ReceptionSelectItemButtonPrefab(listNumber);
    }
}
