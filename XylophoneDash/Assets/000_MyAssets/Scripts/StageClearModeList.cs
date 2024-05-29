using System;
using UnityEngine;

[Serializable]
public class StageClearModeList
{
    public string sheetMusicName;
    public float targetTime;
    public int unlockCondition;

    public StageClearModeList(string sheetMusicName, float targetTime, int unlockCondition)
    {
        this.sheetMusicName = sheetMusicName;
        this.targetTime = targetTime;
        this.unlockCondition = unlockCondition;
    }
}
