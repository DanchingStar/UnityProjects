using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheetMusicListGenerator : MonoBehaviour
{
    [SerializeField] private SheetMusicListEntry sheetMusicListEntry;

    /// <summary>
    /// �y�����ƈ�v����Text�t�@�C����Ԃ�
    /// </summary>
    /// <param name="sheetMusicName"></param>
    /// <returns></returns>
    public TextAsset GetSheetMusicFile(string sheetMusicName)
    {
        foreach (var item in sheetMusicListEntry.itemList)
        {
            if (item.name == sheetMusicName)
            {
                return item.sheetMusicFile;
            }
        }

        if (sheetMusicListEntry.itemList[0] != null)
        {
            return sheetMusicListEntry.itemList[0].sheetMusicFile;
        }

        return null;
    }

    /// <summary>
    /// �y�����ƈ�v���鐳���̉��yClip��Ԃ�
    /// </summary>
    /// <param name="sheetMusicName"></param>
    /// <returns></returns>
    public AudioClip GetSuccessMusicClip(string sheetMusicName)
    {
        foreach (var item in sheetMusicListEntry.itemList)
        {
            if (item.name == sheetMusicName)
            {
                return item.successMusicClip;
            }
        }

        if (sheetMusicListEntry.itemList[0] != null)
        {
            return sheetMusicListEntry.itemList[0].successMusicClip;
        }

        return null;
    }

    /// <summary>
    /// �T�C�Y��Ԃ�
    /// </summary>
    /// <returns></returns>
    public int GetLength()
    {
        return sheetMusicListEntry.itemList.Count;
    }

    public string GetTitleName(int number)
    {
        return sheetMusicListEntry.itemList[number].name;
    }

    public TextAsset GetSheetMusicFile(int number)
    {
        return sheetMusicListEntry.itemList[number].sheetMusicFile;
    }

    public AudioClip GetSuccessMusicClip(int number)
    {
        return sheetMusicListEntry.itemList[number].successMusicClip;
    }

}
