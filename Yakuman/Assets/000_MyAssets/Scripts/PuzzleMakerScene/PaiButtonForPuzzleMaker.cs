using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PaiButtonForPuzzleMaker : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI counterText;
    [SerializeField] private MahjongManager.PaiKinds myPaiKinds;
    [SerializeField] private Image myImage;

    private Button myButton;

    private void Start()
    {
        myButton = GetComponent<Button>();

        myImage.sprite = MahjongManager.Instance.GetGaraSprite(myPaiKinds);

        myButton.onClick.AddListener(() => PushMyButton());

        InitMyButton();

        ChangeZaikoText(PuzzleMakerManager.Instance.GetZaikoCounter(myPaiKinds));
    }

    private void InitMyButton()
    {
        PuzzleMakerManager.Instance.InitPaiButton(this, myPaiKinds);
    }

    private void PushMyButton()
    {
        PuzzleMakerManager.Instance.ReceptionPaiButton(this);
    }

    public MahjongManager.PaiKinds getMyPaiKinds()
    {
        return myPaiKinds;
    }

    public void ChangeZaikoText(int _counter)
    {
        counterText.text = _counter.ToString();
    }

}
