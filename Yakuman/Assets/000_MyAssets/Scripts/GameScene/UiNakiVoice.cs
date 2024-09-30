using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiNakiVoice : MonoBehaviour
{
    [SerializeField] private Image backImage;
    [SerializeField] private TextMeshProUGUI voiceText;

    private readonly Vector3 POSITION_PLAYER = new Vector3(0, -375, 0);
    private readonly Vector3 POSITION_SHIMOCHA = new Vector3(300, 35, 0);
    private readonly Vector3 POSITION_TOIMEN = new Vector3(0, 375, 0);
    private readonly Vector3 POSITION_KAMICHA = new Vector3(-300, 35, 0);

    private bool startFinifhFlg = false;

    private void Start()
    {
        if (startFinifhFlg) return;
        startFinifhFlg = true;

        gameObject.SetActive(false);
    }

    public void DisplayMe(MahjongManager.NakiKinds _nakiKind , MahjongManager.PlayerKind _playerKind)
    {
        Start();

        if(MahjongManager.NakiKinds.Ankan <=_nakiKind && _nakiKind <= MahjongManager.NakiKinds.KakanFromKamicha)
        {
            voiceText.text = "カン";
            backImage.color = Color.cyan;
        }
        else if (MahjongManager.NakiKinds.Pon <= _nakiKind && _nakiKind <= MahjongManager.NakiKinds.PonFromKamicha)
        {
            voiceText.text = "ポン";
            backImage.color = Color.cyan;
        }
        else if (MahjongManager.NakiKinds.Chi <= _nakiKind && _nakiKind <= MahjongManager.NakiKinds.ChiNumHigh)
        {
            voiceText.text = "チー";
            backImage.color = Color.cyan;
        }
        else if (MahjongManager.NakiKinds.Ron == _nakiKind)
        {
            voiceText.text = "ロン";
            backImage.color = Color.red;
        }
        else
        {
            voiceText.text = "ツモ";
            backImage.color = Color.red;
        }

        transform.localPosition = GetPosition(_playerKind);

        gameObject.SetActive(true);
    }

    public void DestroyMe()
    {
        Destroy(gameObject);
    }

    private Vector3 GetPosition(MahjongManager.PlayerKind _playerKind)
    {
        Vector3 result;

        if (_playerKind == MahjongManager.PlayerKind.Player)
        {
            result = POSITION_PLAYER;
        }
        else if (_playerKind == MahjongManager.PlayerKind.Shimocha)
        {
            result = POSITION_SHIMOCHA;
        }
        else if (_playerKind == MahjongManager.PlayerKind.Toimen)
        {
            result = POSITION_TOIMEN;
        }
        else if (_playerKind == MahjongManager.PlayerKind.Kamicha)
        {
            result = POSITION_KAMICHA;
        }
        else
        {
            result = Vector3.zero;
        }

        return result;
    }

}
