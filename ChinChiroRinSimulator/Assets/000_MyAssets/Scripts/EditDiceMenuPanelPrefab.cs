using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditDiceMenuPanelPrefab : MonoBehaviour
{
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject editPanel;
    [SerializeField] private GameObject addPanel;

    [SerializeField] private GameObject editMorePanel;
    [SerializeField] private GameObject addMorePanel;

    [SerializeField] private Image[] defaultImages;
    [SerializeField] private Image[] afterImages;

    [SerializeField] private TMP_InputField mysetNameField;

    [SerializeField] private TextMeshProUGUI adResultText;

    [SerializeField] private GameObject customDiceButtonPrefab;
    [SerializeField] private Transform contentTransform;

    [SerializeField] private Sprite noChangeSprite;

    private int activeMysetNumber;
    private DiceController.DiceEyeKinds[] afterKinds;

    private void Start()
    {
        mainPanel.SetActive(true);
        editPanel.SetActive(false);
        addPanel.SetActive(false);
        editMorePanel.SetActive(false);
        addMorePanel.SetActive(false);

        afterKinds = new DiceController.DiceEyeKinds[defaultImages.Length];
        for (int i = 0; i < defaultImages.Length; i++)
        {
            defaultImages[i].sprite = DiceController.Instance.GetEyeSprite((DiceController.DiceEyeKinds)(i + 1));
        }
    }

    public void PushEditButton(bool flg)
    {
        if (flg)
        {
            SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes);
        }
        else
        {
            SoundManager.Instance.PlaySE(SoundManager.SoundSeType.No);
        }

        mainPanel.SetActive(!flg);
        editPanel.SetActive(flg);

        if (flg)
        {
            foreach (Transform item in contentTransform)
            {
                Destroy(item.gameObject);
            }

            // 全てのサイコロを生み出す処理
            InstantiateCustomDiceButtonPrefab(-1);
            for (int i = 0; i < SaveDataManager.Instance.GetMyDiceCount(); i++)
            {
                InstantiateCustomDiceButtonPrefab(i);
            }
        }
    }

    public void PushAddButton(bool flg)
    {
        if (flg)
        {
            SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes);
        }
        else
        {
            SoundManager.Instance.PlaySE(SoundManager.SoundSeType.No);
        }

        mainPanel.SetActive(!flg);
        addPanel.SetActive(flg);
    }

    public void ReceptionCustomDiceButtonPrefab(int _mysetNumber)
    {
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes);

        editMorePanel.SetActive(true);

        activeMysetNumber = _mysetNumber;

        mysetNameField.text = SaveDataManager.Instance.GetMyDice(activeMysetNumber).name;

        for (int i = 0; i < afterImages.Length; i++)
        {
            afterKinds[i] = SaveDataManager.Instance.GetMyDice(activeMysetNumber).changeEyes[i + 1];
            afterImages[i].sprite = GetDiceEyeSprite(afterKinds[i]);
        }
    }

    private Sprite GetDiceEyeSprite(DiceController.DiceEyeKinds _DiceEyeKinds)
    {
        Sprite _sprite = DiceController.Instance.GetEyeSprite(_DiceEyeKinds);
        if (_sprite == null)
        {
            return noChangeSprite;
        }
        else
        {
            return _sprite;
        }
    }

    public void PushChangeDiceEyeButton(int eyeNumberMinusOne)
    {
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Select);

        int value = (int)afterKinds[eyeNumberMinusOne] + 1;
        if (value == eyeNumberMinusOne + 1) value++;
        if (value > 6) value = 0;
        afterKinds[eyeNumberMinusOne] = (DiceController.DiceEyeKinds)value;
        afterImages[eyeNumberMinusOne].sprite = GetDiceEyeSprite(afterKinds[eyeNumberMinusOne]);
    }

    public void PushEditSaveButton(bool flg)
    {
        if (flg)
        {
            SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes);
        }
        else
        {
            SoundManager.Instance.PlaySE(SoundManager.SoundSeType.No);
        }

        if (flg)
        {
            MyDice _dice = new MyDice();
            _dice.name = mysetNameField.text;
            _dice.changeEyes[0] = DiceController.DiceEyeKinds.None;
            for (int i = 0; i < afterKinds.Length; i++)
            {
                _dice.changeEyes[i + 1] = afterKinds[i];
            }
            SaveDataManager.Instance.SaveDice(activeMysetNumber, _dice);
        }

        editMorePanel.SetActive(false);
        mainPanel.SetActive(false);
        editPanel.SetActive(true);

        if (flg)
        {
            foreach (Transform item in contentTransform)
            {
                Destroy(item.gameObject);
            }

            // 全てのサイコロを生み出す処理
            InstantiateCustomDiceButtonPrefab(-1);
            for (int i = 0; i < SaveDataManager.Instance.GetMyDiceCount(); i++)
            {
                InstantiateCustomDiceButtonPrefab(i);
            }
        }
    }

    public void PushShowRewardButton()
    {
        AdMobManager.Instance.ShowRewardForAddMySet(this);
    }

    public void ReceptionAdMobReward(bool flg)
    {
        addMorePanel.SetActive(true);
        if (flg)
        {
            SaveDataManager.Instance.AddSaikoroMyset("");
            adResultText.text = $"広告の視聴に\n成功しました。\n\nマイセット数が\n1つ増えました。\n\nマイセット数 : {SaveDataManager.Instance.GetMyDiceCount()}";
        }
        else
        {
            adResultText.text = "広告の視聴に失敗しました。";
        }
    }

    public void PushCloseAddMorePanelButton()
    {
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.No);
        addMorePanel.SetActive(false);
        PushAddButton(false);
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
                Debug.LogWarning($"EditDiceMenuPanelPrefab.InstantiateCustomDiceButtonPrefab : obj = null ! ( num = {num} )");
                Destroy(obj);
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// このPrefabを閉じたとき
    /// </summary>
    public void PushCloseButton()
    {
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.No);
        Destroy(gameObject);
    }

}
