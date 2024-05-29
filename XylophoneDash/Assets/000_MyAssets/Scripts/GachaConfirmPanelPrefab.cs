using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GachaConfirmPanelPrefab : MonoBehaviour
{
    [SerializeField] private Button okButton;
    [SerializeField] private TextMeshProUGUI informationText1;
    [SerializeField] private TextMeshProUGUI informationText2;
    [SerializeField] private TextMeshProUGUI informationText3;

    private int myGachaTimes;
    private int myPrice;
    private int coinCount;

    private readonly Color COLOR_ERROR = Color.red;

    public void SetMyStatus(int _gachaTimes)
    {
        myGachaTimes = _gachaTimes;
        myPrice = PlayFabManager.Instance.GetGachaPrice(myGachaTimes);
        coinCount = PlayFabManager.Instance.GetHaveGachaTicket();

        bool isAbleGachaFlg = true;

        if (myPrice <= 0)
        {
            informationText1.text = "�G���[���������܂����B";
            informationText2.text = "";
            informationText3.text = "";
            informationText1.color = COLOR_ERROR;
            isAbleGachaFlg = false;
        }
        else
        {
            string str = myGachaTimes == 1 ? "1��" : $"{myGachaTimes}�A";
            informationText1.text = $"SP�R�C����{myPrice}������āA\n{str}�K�`�������s���܂��B";
            informationText2.text = $"�����Ă���SP�R�C�� : {coinCount}��";

            int afterCoin = coinCount - myPrice;

            if (afterCoin < 0)
            {
                informationText3.text = $"SP�R�C��������܂���B";
                informationText3.color = COLOR_ERROR;
                isAbleGachaFlg = false;
            }
            else
            {
                informationText3.text = $"�K�`�����SP�R�C�� : {afterCoin}��";
            }
        }

        SetOkButtonEnable(isAbleGachaFlg);
    }

    private void SetOkButtonEnable(bool flg)
    {
        okButton.interactable = flg;
    }

    public void PushOkButton()
    {
        MenuSceneManager.Instance.ShoppingGacha(myGachaTimes);
        Destroy(gameObject);
    }

    public void PushNgButton()
    {
        Destroy(gameObject);
    }

}
