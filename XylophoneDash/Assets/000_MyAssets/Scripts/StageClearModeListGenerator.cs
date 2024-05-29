using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageClearModeListGenerator : MonoBehaviour
{
    /// <summary> ”z—ñ”Ô† -> 0:Normal 1:Hard 2:Veryhard </summary>
    [SerializeField] private StageClearModeListEntry[] stageClearModeListEntry;

    public int GetListLength(PlayerInformationManager.GameLevel level)
    {
        int hoge = 0;

        if (level == PlayerInformationManager.GameLevel.Normal)
        {
            hoge = stageClearModeListEntry[0].itemList.Count;
        }
        else if (level == PlayerInformationManager.GameLevel.Hard)
        {
            hoge = stageClearModeListEntry[1].itemList.Count;
        }
        else if (level == PlayerInformationManager.GameLevel.VeryHard)
        {
            hoge = stageClearModeListEntry[2].itemList.Count;
        }
        else
        {
            Debug.LogError($"StageClearModeListGenerator.GetListLength : Level Error");
        }

        return hoge;
    }

    public string GetSheetMusicName(PlayerInformationManager.GameLevel level, int elementNumber)
    {
        string hoge = "";

        if (level == PlayerInformationManager.GameLevel.Normal)
        {
            hoge = stageClearModeListEntry[0].itemList[elementNumber].sheetMusicName;
        }
        else if (level == PlayerInformationManager.GameLevel.Hard)
        {
            hoge = stageClearModeListEntry[1].itemList[elementNumber].sheetMusicName;
        }
        else if (level == PlayerInformationManager.GameLevel.VeryHard)
        {
            hoge = stageClearModeListEntry[2].itemList[elementNumber].sheetMusicName;
        }
        else
        {
            Debug.LogError($"StageClearModeListGenerator.GetSheetMusicName : Level Error");
        }

        return hoge;
    }

    public float GetTargetTime(PlayerInformationManager.GameLevel level, int elementNumber)
    {
        float hoge = 0f;

        if (level == PlayerInformationManager.GameLevel.Normal)
        {
            hoge = stageClearModeListEntry[0].itemList[elementNumber].targetTime;
        }
        else if (level == PlayerInformationManager.GameLevel.Hard)
        {
            hoge = stageClearModeListEntry[1].itemList[elementNumber].targetTime;
        }
        else if (level == PlayerInformationManager.GameLevel.VeryHard)
        {
            hoge = stageClearModeListEntry[2].itemList[elementNumber].targetTime;
        }
        else
        {
            Debug.LogError($"StageClearModeListGenerator.GetTargetTime : Level Error");
        }

        return hoge;
    }

    public int GetUnlockCondition(PlayerInformationManager.GameLevel level, int elementNumber)
    {
        int hoge = 0;

        if (level == PlayerInformationManager.GameLevel.Normal)
        {
            hoge = stageClearModeListEntry[0].itemList[elementNumber].unlockCondition;
        }
        else if (level == PlayerInformationManager.GameLevel.Hard)
        {
            hoge = stageClearModeListEntry[1].itemList[elementNumber].unlockCondition;
        }
        else if (level == PlayerInformationManager.GameLevel.VeryHard)
        {
            hoge = stageClearModeListEntry[2].itemList[elementNumber].unlockCondition;
        }
        else
        {
            Debug.LogError($"StageClearModeListGenerator.GetUnlockCondition : Level Error");
        }

        return hoge;
    }
}
