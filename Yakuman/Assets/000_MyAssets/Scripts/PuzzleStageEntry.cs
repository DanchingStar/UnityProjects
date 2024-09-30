using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PuzzleStageEntry", menuName = "Puzzle Stage Entry")]
public class PuzzleStageEntry : ScriptableObject
{
    public List<PuzzleStage> itemList = new List<PuzzleStage>();
}
