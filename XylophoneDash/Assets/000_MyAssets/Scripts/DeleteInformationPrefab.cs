using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeleteInformationPrefab : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text1;
    [SerializeField] TextMeshProUGUI text2;

    private bool deleteFlg;

    /// <summary>
    /// �����̏����Z�b�g����
    /// </summary>
    /// <param name="flg">true : �폜����</param>
    public void SetMyStatus(bool flg)
    {
        deleteFlg = flg;
        UpdateTexts();
    }

    /// <summary>
    /// �e�L�X�g���X�V����
    /// </summary>
    private void UpdateTexts()
    {
        if (deleteFlg)
        {
            text1.text = $"�f�[�^���폜���܂����B";
            text2.text = $"���̃Q�[����V��ł��������A\n���肪�Ƃ��������܂����B";
        }
        else
        {
            text1.text = $"�f�[�^�̍폜�Ɏ��s���܂����B";
            text2.text = $"���O�C��������ɂł��Ă��邩�A\n���m�F���������B";
        }
    }

    /// <summary>
    /// ������������Ƃ�
    /// </summary>
    public void PushCloseButton()
    {
        if (deleteFlg)
        {
            MenuSceneManager.Instance.ReceptionAfterDelete();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
