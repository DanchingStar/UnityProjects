using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CustomDicePanelPrefab : MonoBehaviour
{
    [SerializeField] private GameObject customDiceButtonPrefab;
    [SerializeField] private Transform contentTransform;
    [SerializeField] private TextMeshProUGUI mysetNameText;

    private int choiceMysetNumber;

    private void Start()
    {
        // �����ݒ�̃}�C�Z�b�g
        ReceptionCustomDiceButtonPrefab(-1);

        // �S�ẴT�C�R���𐶂ݏo������
        InstantiateCustomDiceButtonPrefab(-1);
        for (int i = 0; i < SaveDataManager.Instance.GetMyDiceCount(); i++)
        {
            InstantiateCustomDiceButtonPrefab(i);
        }
    }

    /// <summary>
    /// CustomDiceButtonPrefab�𐶐����āA�X�e�[�^�X��ݒ肷��
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    private bool InstantiateCustomDiceButtonPrefab(int num)
    {
        GameObject obj = Instantiate(customDiceButtonPrefab, contentTransform);
        CustomDiceButtonPrefab prefab;
        if (num == -1) 
        {
            prefab = obj.GetComponent<CustomDiceButtonPrefab>();
            prefab.SetMyStatusDefault(gameObject);
        }
        else
        {
            if (obj != null)
            {
                prefab = obj.GetComponent<CustomDiceButtonPrefab>();
                prefab.SetMyStatus(gameObject, num);
            }
            else
            {
                Debug.LogWarning($"CustomDicePanelPrefab.InstantiateCustomDiceButtonPrefab : obj = null ! ( num = {num} )");
                Destroy(obj);
                return false;   
            }
        }
        return true;
    }

    /// <summary>
    /// CustomDiceButtonPrefab����A�{�^���������󂯎��
    /// </summary>
    /// <param name="num"></param>
    public void ReceptionCustomDiceButtonPrefab(int num)
    {
        choiceMysetNumber = num;

        if (num == -1)
        {
            mysetNameText.text = DiceController.Instance.GetDefaultDiceName();
        }
        else
        {
            mysetNameText.text = SaveDataManager.Instance.GetMyDice(choiceMysetNumber).name;
        }

    }

    /// <summary>
    /// ����Prefab������Ƃ�
    /// </summary>
    public void PushCloseButton()
    {
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes);
        DiceController.Instance.ReceptionCustomDicePanelPrefab(choiceMysetNumber);
        Destroy(gameObject);
    }
}
