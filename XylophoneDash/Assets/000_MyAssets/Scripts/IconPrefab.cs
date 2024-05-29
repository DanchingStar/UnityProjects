using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconPrefab : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image frameImage;
    [SerializeField] private Image characterImage;

    /// <summary>
    /// 自身の画像を更新する(不要な引数はnullでOK)
    /// </summary>
    /// <param name="_background">背景のSprite</param>
    /// <param name="_frame">枠のSprite</param>
    /// <param name="_character">キャラクターのSprite</param>
    public void UpdateMySprites(Sprite _background, Sprite _frame, Sprite _character)
    {
        SetBackgroundImage(_background);
        SetFrameImage(_frame);
        SetCharacterImage(_character);
    }

    private void SetBackgroundImage(Sprite sprite)
    {
        if (sprite == null) return;
        backgroundImage.sprite = sprite;
    }

    private void SetFrameImage(Sprite sprite)
    {
        if (sprite == null) return;
        frameImage.sprite = sprite;
    }

    private void SetCharacterImage(Sprite sprite)
    {
        if (sprite == null) return;
        characterImage.sprite = sprite;
    }


}
