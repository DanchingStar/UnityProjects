using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleStageGenerator : MonoBehaviour
{
    [SerializeField] private PuzzleStageEntry puzzleStageEntry;

    public string GetTitleName(int number)
    {
        return puzzleStageEntry.itemList[number].name;
    }

    public TextAsset GetStageTextFile(int number)
    {
        return puzzleStageEntry.itemList[number].stageTextFile;
    }

    public int GetStageItemCount()
    {
        return puzzleStageEntry.itemList.Count;
    }
}
