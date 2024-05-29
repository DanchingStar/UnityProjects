using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheetMusicListGenerator : MonoBehaviour
{
    [SerializeField] private SheetMusicListEntry sheetMusicListEntry;

    /// <summary>
    /// 楽譜名と一致するTextファイルを返す
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
    /// 楽譜名と一致する正解の音楽Clipを返す
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
    /// サイズを返す
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
