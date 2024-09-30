using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleContents : MonoBehaviour
{
    [SerializeField] private PuzzleStageGenerator puzzleStageGenerator;

    public PuzzleStageGenerator GetPuzzleStageGenerator()
    {
        return puzzleStageGenerator;
    }

}
