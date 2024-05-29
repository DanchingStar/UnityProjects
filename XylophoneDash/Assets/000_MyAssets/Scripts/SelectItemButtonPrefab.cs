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
    /// カテゴリを設定する
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
    /// 自身の画像を設定する
    /// </summary>
    /// <param name="sprite"></param>
    public void SetSprite(Sprite sprite)
    {
        myImage.sprite = sprite;
    }

    /// <summary>
    /// 設定済みの情報から自身の画像を設定する
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
    /// このボタンを押したとき
    /// </summary>
    public void PushThisButton()
    {
        MenuSceneManager.Instance.ReceptionSelectItemButtonPrefab(listNumber);
    }
}
