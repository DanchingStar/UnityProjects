using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GachaInformationPanelPrefab : MonoBehaviour
{
    [SerializeField] private Image myImage;
    [SerializeField] private TextMeshProUGUI myName;
    [SerializeField] private TextMeshProUGUI myKinds;
    [SerializeField] private TextMeshProUGUI myIsHave;

    private readonly Color COLOR_NEW_1 = Color.yellow;
    private readonly Color COLOR_NEW_2 = Color.red;

    private bool isHaveFlg;

    public void SetMyPanel(Sprite _mySprite, string _myName, PlayerInformationManager.GachaItemKind _myKinds, bool _isHave)
    {
        myImage.sprite = _mySprite;
        myName.text = _myName;
        myKinds.text = MakeKindsString(_myKinds);
        myIsHave.text = MakeIsHaveText(_isHave);

        isHaveFlg = _isHave;
    }

    private void Update()
    {
        if (!isHaveFlg)
        {
            myIsHave.color = Color.Lerp(COLOR_NEW_1, COLOR_NEW_2, MenuSceneManager.Instance.GetColorLerpValueForGachaNewText());
        }
    }

    private string MakeKindsString(PlayerInformationManager.GachaItemKind kind)
    {
        string str;

        switch (kind)
        {
            case PlayerInformationManager.GachaItemKind.Character:
                str = "�L�����N�^�[";
                break;
            case PlayerInformationManager.GachaItemKind.IconBackground:
                str = "�A�C�R���w�i";
                break;
            case PlayerInformationManager.GachaItemKind.IconFrame:
                str = "�A�C�R���g";
                break;
            default:
                str = "";
                break;
        }

        return str;
    }

    private string MakeIsHaveText(bool flg)
    {
        string str;

        if (flg)
        {
            str = "�����ς�";
        }
        else
        {
            str = "���Q�b�g!!";
        }

        return str;
    }

    public void PushCloseButton()
    {
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.No);
        Destroy(this.gameObject);
    }
}
