using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankingModeListGenerator : MonoBehaviour
{
    [SerializeField] private RankingModeListEntry rankingModeListEntry;

    public string GetSheetMusicName(int elementNumber)
    {
        string hoge = "";

        if (elementNumber < 0 || elementNumber > rankingModeListEntry.itemList.Count)
        {
            Debug.LogError($"RankingModeListGenerator.GetSheetMusicName : Number Error -> {elementNumber}");
        }
        else
        {
            hoge = rankingModeListEntry.itemList[elementNumber].sheetMusicName;
        }

        return hoge;
    }

    public PlayerInformationManager.GameLevel GetGameLevel(int elementNumber)
    {
        PlayerInformationManager.GameLevel hoge = PlayerInformationManager.GameLevel.None;

        if (elementNumber < 0 || elementNumber > rankingModeListEntry.itemList.Count)
        {
            Debug.LogError($"RankingModeListGenerator.GetGameLevel : Number Error -> {elementNumber}");
        }
        else
        {
            hoge = rankingModeListEntry.itemList[elementNumber].gameLevel;
        }

        return hoge;
    }

    public int GetUnlockStageNumber(int elementNumber)
    {
        int hoge = -1;

        if (elementNumber < 0 || elementNumber > rankingModeListEntry.itemList.Count)
        {
            Debug.LogError($"RankingModeListGenerator.GetUnlockStageNumber : Number Error -> {elementNumber}");
        }
        else
        {
            hoge = rankingModeListEntry.itemList[elementNumber].unlockStageNumber;
        }

        return hoge;
    }

    public string GetKey(int elementNumber)
    {
        string hoge = "";

        if (elementNumber < 0 || elementNumber > rankingModeListEntry.itemList.Count)
        {
            Debug.LogError($"RankingModeListGenerator.GetKey : Number Error -> {elementNumber}");
        }
        else
        {
            hoge = rankingModeListEntry.itemList[elementNumber].key;
        }

        return hoge;
    }

    public int GetLength()
    {
        return rankingModeListEntry.itemList.Count;
    }

}
