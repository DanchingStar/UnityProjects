using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleStageButtonPrefab : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI stageNumberText;
    private int stageNumber;
    private Button myButton;

    public void SetMyStatus(int _stageNumber)
    {
        stageNumber = _stageNumber;

        stageNumberText.text = stageNumber.ToString();

        myButton = GetComponent<Button>();
    }

    public Button GetMyButton()
    {
        return myButton;
    }

    public void PushMyButton()
    {
        UiManagerForMenuScene.Instance.ReceptionPuzzleStageButton(stageNumber);
    }

}
