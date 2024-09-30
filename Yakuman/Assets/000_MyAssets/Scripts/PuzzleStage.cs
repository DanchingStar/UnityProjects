using System;
using UnityEngine;

[Serializable]
public class PuzzleStage
{
    public string name;
    public TextAsset stageTextFile;

    public PuzzleStage(string _name, TextAsset _sheetMusicFile)
    {
        name = _name;
        stageTextFile = _sheetMusicFile;
    }
}
