using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopResultPanelPrefab : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI sub1Text;
    [SerializeField] private TextMeshProUGUI sub2Text;

    private const string NO_STRING = "";

    //private Content myContent;

    public enum Content
    {
        None,
        AdReward,
        SnsShare,
        ObtainSpCoin,
    }

    public void SetMyContent(Content content)
    {
        //myContent = content;

        sub1Text.text = NO_STRING;
        sub2Text.text = NO_STRING;

        switch (content)
        {
            case Content.AdReward:
                titleText.text = "��������";
                break;
            case Content.SnsShare:
                titleText.text = "�V�F�A����";
                break;
            case Content.ObtainSpCoin:
                titleText.text = "����";
                break;
            case Content.None:
                titleText.text = NO_STRING;
                break;
        }
    }

    public void SetMySub1Text(string str)
    {
        sub1Text.text = str;
    }

    public void SetMySub2Text(string str)
    {
        sub2Text.text = str;
    }

    public void PushCloseButton()
    {
        MenuSceneManager.Instance.ReceptionCloseFromShopResultPanelPrefab();

        Destroy(gameObject);
    }
}
