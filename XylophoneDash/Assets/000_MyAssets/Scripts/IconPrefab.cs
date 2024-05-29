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
    /// ���g�̉摜���X�V����(�s�v�Ȉ�����null��OK)
    /// </summary>
    /// <param name="_background">�w�i��Sprite</param>
    /// <param name="_frame">�g��Sprite</param>
    /// <param name="_character">�L�����N�^�[��Sprite</param>
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
