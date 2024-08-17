using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiPaiImagePrefab : MonoBehaviour
{
    [SerializeField] private Image[] paiImage;

    private MahjongManager.PaiKinds paiKind;

    private float DEFAULT_SCALE_X;
    private float DEFAULT_SCALE_Y;
    private float DEFAULT_SCALE_Z;

    private bool startFinifhFlg = false;

    private void Start()
    {
        if (startFinifhFlg) return;
        startFinifhFlg = true;

        DEFAULT_SCALE_X = transform.localScale.x;
        DEFAULT_SCALE_Y = transform.localScale.y;
        DEFAULT_SCALE_Z = transform.localScale.z;
    }


    public void SetPaiKind(MahjongManager.PaiKinds _paiKind)
    {
        Start();

        paiKind = _paiKind;
        ChangeSprite(paiKind);
    }

    public void SetPaiKindForAnkan(MahjongManager.PaiKinds _paiKind)
    {
        Start();

        paiKind = _paiKind;
        ChangeSpriteForAnkan();
    }

    private void ChangeSprite(MahjongManager.PaiKinds _paiKind)
    {
        foreach(var item in paiImage)
        {
            if (_paiKind == MahjongManager.PaiKinds.None_00)
            {
                item.sprite = MahjongManager.Instance.GetGaraSpriteWhite();
            }
            else
            {
                item.sprite = MahjongManager.Instance.GetGaraSprite(_paiKind);
            }
        }
    }

    private void ChangeSpriteForAnkan()
    {
        ChangeSprite(MahjongManager.PaiKinds.None_00);
        foreach (var item in paiImage)
        {
            item.color = new Color(219f / 255f, 125f / 255f, 67f / 255f);
        }
    }

    public void ChangeScale(float _magnification)
    {
        Start();

        transform.localScale = new Vector3(DEFAULT_SCALE_X * _magnification, DEFAULT_SCALE_Y * _magnification, DEFAULT_SCALE_Z);
    }

    public Vector3 GetLocalScale()
    {
        return transform.localScale;
    }

}
