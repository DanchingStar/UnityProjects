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
        // 初期設定のマイセット
        ReceptionCustomDiceButtonPrefab(-1);

        // 全てのサイコロを生み出す処理
        InstantiateCustomDiceButtonPrefab(-1);
        for (int i = 0; i < SaveDataManager.Instance.GetMyDiceCount(); i++)
        {
            InstantiateCustomDiceButtonPrefab(i);
        }
    }

    /// <summary>
    /// CustomDiceButtonPrefabを生成して、ステータスを設定する
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
    /// CustomDiceButtonPrefabから、ボタン押下を受け取る
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
    /// このPrefabを閉じたとき
    /// </summary>
    public void PushCloseButton()
    {
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes);
        DiceController.Instance.ReceptionCustomDicePanelPrefab(choiceMysetNumber);
        Destroy(gameObject);
    }
}
