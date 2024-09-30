using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PositionButtonForPuzzleMaker : MonoBehaviour
{
    [SerializeField] private Image paiImage;
    [SerializeField] private Image cursorImage;

    private Button myButton;

    private int myIndex;

    private MahjongManager.PaiKinds selectKind;
    private bool activeFlg;

    private bool startFinifhFlg = false;

    private void Start()
    {
        if (startFinifhFlg) return;
        startFinifhFlg = true;

        myButton = GetComponent<Button>();
        myButton.onClick.AddListener(() => PushMyButton());
    }

    private void Update()
    {
        if (activeFlg)
        {
            cursorImage.color = Color.Lerp(Color.red, Color.yellow, Mathf.PingPong(Time.time, 1));
        }
    }

    public void SetMyIndex(int _index)
    {
        Start();
        myIndex = _index;
    }

    public void SetMyPaiKind(MahjongManager.PaiKinds _paiKinds)
    {
        selectKind = _paiKinds;

        if (selectKind != MahjongManager.PaiKinds.None_00)
        {
            paiImage.sprite = MahjongManager.Instance.GetGaraSprite(selectKind);
            paiImage.color = Color.white;
        }
        else
        {
            paiImage.sprite = MahjongManager.Instance.GetGaraSpriteWhite();
            paiImage.color = Color.gray;
        }
    }

    public void ReceptionPuzzleMakerManager(int _activeIndex)
    {
        if(myIndex == _activeIndex)
        {
            ChangeActiveFlg(true);
        }
        else
        {
            ChangeActiveFlg(false);
        }
    }

    private void PushMyButton()
    {
        PuzzleMakerManager.Instance.ReceptionPositionButton(myIndex);
    }

    private void ChangeActiveFlg(bool _flg)
    {
        activeFlg = _flg;

        if (!activeFlg)
        {
            cursorImage.color = new Color(0, 0, 0, 0);
        }
    }

    public MahjongManager.PaiKinds GetMySelectPaiKind()
    {
        return selectKind;
    }

}
