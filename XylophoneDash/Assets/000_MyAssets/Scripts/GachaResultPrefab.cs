using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GachaResultPrefab : MonoBehaviour
{
    [SerializeField] Image myImage;
    [SerializeField] TextMeshProUGUI myNewText;
    [SerializeField] TextMeshProUGUI failureText;

    [SerializeField] private GameObject gachaInformationPanelPrefab;

    private PlayerInformationManager.GachaItemKind myKinds;
    private int myNumber;
    private bool newFlg;
    private string myName;

    private readonly Color COLOR_FADE = new Color(0, 0, 0, 0);
    private readonly Color COLOR_NEW_1 = Color.yellow;
    private readonly Color COLOR_NEW_2 = Color.red;

    private void Update()
    {
        UpdateNewTextColor();
    }

    /// <summary>
    /// 自分の情報をセットする
    /// </summary>
    /// <param name="partsStatus"></param>
    public void SetMyStatus(PlayerInformationManager.GachaItemKind _itemKinds, int _itemNumber,bool _newFlg)
    {
        myKinds = _itemKinds;
        myNumber = _itemNumber;
        newFlg = _newFlg;

        switch (_itemKinds)
        {
            case PlayerInformationManager.GachaItemKind.Character:
                myImage.sprite = PlayerInformationManager.Instance.GetCharacterListGenerator().GetCharacterSprite(myNumber);
                myName = PlayerInformationManager.Instance.GetCharacterListGenerator().GetCharacterName(myNumber);
                break;
            case PlayerInformationManager.GachaItemKind.IconBackground:
                myImage.sprite = PlayerInformationManager.Instance.GetIconBackListGenerator().GetSprite(myNumber);
                myName = PlayerInformationManager.Instance.GetIconBackListGenerator().GetName(myNumber);
                break;
            case PlayerInformationManager.GachaItemKind.IconFrame:
                myImage.sprite = PlayerInformationManager.Instance.GetIconFrameListGenerator().GetSprite(myNumber);
                myName = PlayerInformationManager.Instance.GetIconFrameListGenerator().GetName(myNumber);
                break;
            default:
                Debug.LogError("GachaResultPrefab.SetMyStatus : Error");
                break;
        }

        myNewText.gameObject.SetActive(_newFlg);
        failureText.gameObject.SetActive(false);
    }

    /// <summary>
    /// NEWのテキストのカラーを変える
    /// </summary>
    private void UpdateNewTextColor()
    {
        if (newFlg)
        {
            myNewText.color = Color.Lerp(COLOR_NEW_1, COLOR_NEW_2, MenuSceneManager.Instance.GetColorLerpValueForGachaNewText());
        }
    }

    /// <summary>
    /// 失敗時にメッセージを受け取る
    /// </summary>
    /// <param name="str"></param>
    public void SetMyStatusFailure(string str)
    {
        GetComponent<Button>().interactable = false;
        GetComponent<Image>().color = COLOR_FADE;
        myImage.color = COLOR_FADE;
        myNewText.gameObject.SetActive(false);
        failureText.gameObject.SetActive(true);
        failureText.text = str;
    }

    /// <summary>
    /// 自分のボタンを押したとき
    /// </summary>
    public void PushMyImageButton()
    {
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes);

        Transform canvas = MenuSceneManager.Instance.GetCanvasTransform();

        GameObject obj = Instantiate(gachaInformationPanelPrefab, canvas);

        obj.GetComponent<GachaInformationPanelPrefab>().SetMyPanel(myImage.sprite, myName, myKinds, !newFlg);
    }
}
