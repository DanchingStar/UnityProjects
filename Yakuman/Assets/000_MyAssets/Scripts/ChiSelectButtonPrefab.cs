using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChiSelectButtonPrefab : MonoBehaviour
{
    [SerializeField] private Image[] paiImages;

    private MahjongManager.PaiKinds paiKind;
    private MahjongManager.NakiKinds nakiKind;
    private UiNakiPrefab uiNakiPrefab;

    bool startFinifhFlg = false;

    private void Start()
    {
        if (startFinifhFlg) return;
        startFinifhFlg = true;

    }

    public void DestroyMe()
    {
        Destroy(gameObject);
    }

    public void SetStatus(MahjongManager.PaiKinds _paiKind, MahjongManager.NakiKinds _naki, UiNakiPrefab _uiNakiPrefab)
    {
        Start();

        paiKind = _paiKind;
        nakiKind = _naki;
        uiNakiPrefab = _uiNakiPrefab;

        SetImages();
    }

    public void PushMyButton()
    {
        uiNakiPrefab.ReceptionChiSelectButtonPrefab(nakiKind);
    }

    private void SetImages()
    {
        paiImages[0].sprite = MahjongManager.Instance.GetGaraSprite(paiKind);
        if(nakiKind == MahjongManager.NakiKinds.ChiNumSmall)
        {
            paiImages[1].sprite = MahjongManager.Instance.GetGaraSprite(paiKind + 1);
            paiImages[2].sprite = MahjongManager.Instance.GetGaraSprite(paiKind + 2);
        }
        else if (nakiKind == MahjongManager.NakiKinds.ChiNumMiddle)
        {
            paiImages[1].sprite = MahjongManager.Instance.GetGaraSprite(paiKind - 1);
            paiImages[2].sprite = MahjongManager.Instance.GetGaraSprite(paiKind + 1);
        }
        else if (nakiKind == MahjongManager.NakiKinds.ChiNumBig)
        {
            paiImages[1].sprite = MahjongManager.Instance.GetGaraSprite(paiKind - 2);
            paiImages[2].sprite = MahjongManager.Instance.GetGaraSprite(paiKind - 1);
        }
        else
        {
            Debug.LogWarning($"SetImages : Error\npaiKind = {paiKind} , nakiKind = {nakiKind}");
        }

    }
}
