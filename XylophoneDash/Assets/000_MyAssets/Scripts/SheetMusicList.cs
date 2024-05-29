using System;
using UnityEngine;

[Serializable]
public class SheetMusicList
{
    public string name;
    public TextAsset sheetMusicFile;
    public AudioClip successMusicClip;

    public SheetMusicList(string name, TextAsset sheetMusicFile, AudioClip successMusicClip)
    {
        this.name = name;
        this.sheetMusicFile = sheetMusicFile;
        this.successMusicClip = successMusicClip;
    }
}
