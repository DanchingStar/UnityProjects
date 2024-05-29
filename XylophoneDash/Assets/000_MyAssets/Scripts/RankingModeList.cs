using System;
using UnityEngine;

[Serializable]
public class RankingModeList
{
    public string key;
    public string sheetMusicName;
    public PlayerInformationManager.GameLevel gameLevel;
    public int unlockStageNumber;

    public RankingModeList(string sheetMusicName, PlayerInformationManager.GameLevel gameLevel, int unlockStageNumber, string key)
    {
        this.sheetMusicName = sheetMusicName;
        this.gameLevel = gameLevel;
        this.unlockStageNumber = unlockStageNumber;
        this.key = key;
    }
}
