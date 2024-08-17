using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class UiNakiPrefab : MonoBehaviour
{
    [SerializeField] private GameObject chiSelectButtonPrefab;

    [SerializeField] private Button canselButton;
    [SerializeField] private Button ronButton;
    [SerializeField] private Button kanButton;
    [SerializeField] private Button ponButton;
    [SerializeField] private Button chiButton;
    [SerializeField] private Button chiCanselButton;

    private bool flgAbleRon;
    private bool flgAbleKan;
    private bool flgAblePon;
    private bool flgAbleChiL;
    private bool flgAbleChiM;
    private bool flgAbleChiH;

    private bool flgFriten;

    private MahjongManager.PaiKinds paiKind;

    private List<ChiSelectButtonPrefab> chiSelectButtonPrefabs;

    private bool startFinifhFlg = false;

    private void Start()
    {
        if (startFinifhFlg) return;
        startFinifhFlg = true;

        chiSelectButtonPrefabs = new List<ChiSelectButtonPrefab>();
    }

    public void SetMyStatus(bool _chiLow, bool _chiMid, bool _chiHigh, bool _pon, bool _kan, bool _ron, bool _friten, MahjongManager.PaiKinds _paiKind)
    {
        Start();

        flgAbleChiL = _chiLow;
        flgAbleChiM = _chiMid;
        flgAbleChiH = _chiHigh;
        flgAblePon = _pon;
        flgAbleKan = _kan;
        flgAbleRon = _ron;
        flgFriten = _friten;

        paiKind = _paiKind;

        DisplayButtons();
    }

    public void PushPonButton()
    {
        MahjongManager.Instance.ReceptionUiNakiPrefabForNaki(MahjongManager.NakiKinds.Pon);
        Destroy(gameObject);
    }

    public void PushKanButton()
    {
        MahjongManager.Instance.ReceptionUiNakiPrefabForNaki(MahjongManager.NakiKinds.MinKan);
        Destroy(gameObject);
    }

    public void PushChiButton()
    {
        int counter = 0;
        if (flgAbleChiL) counter++;
        if (flgAbleChiM) counter++;
        if (flgAbleChiH) counter++;

        if (counter == 1)
        {
            if (flgAbleChiL)
                MahjongManager.Instance.ReceptionUiNakiPrefabForNaki(MahjongManager.NakiKinds.ChiNumLow);
            else if (flgAbleChiM)
                MahjongManager.Instance.ReceptionUiNakiPrefabForNaki(MahjongManager.NakiKinds.ChiNumMiddle);
            else if (flgAbleChiH)
                MahjongManager.Instance.ReceptionUiNakiPrefabForNaki(MahjongManager.NakiKinds.ChiNumHigh);

            Destroy(gameObject);
        }
        else
        {
            DisplayChiButtons();
        }
    }

    public void PushRonButton()
    {
        MahjongManager.Instance.ReceptionUiNakiPrefabForNaki(MahjongManager.NakiKinds.Ron);
        Destroy(gameObject);
    }

    public void PushCancelButton()
    {
        MahjongManager.Instance.ReceptionUiNakiPrefabForChangeNakiWaitFlg(false);
        Destroy(gameObject);
    }

    public void PushChiCancelButton()
    {
        foreach (var item in chiSelectButtonPrefabs)
        {
            item.DestroyMe();
        }
        chiSelectButtonPrefabs = new List<ChiSelectButtonPrefab>();
        DisplayButtons();
    }

    private void DisplayButtons()
    {
        canselButton.gameObject.SetActive(true);
        ronButton.gameObject.SetActive(flgAbleRon);
        kanButton.gameObject.SetActive(flgAbleKan);
        ponButton.gameObject.SetActive(flgAblePon);
        chiButton.gameObject.SetActive(flgAbleChiL || flgAbleChiM || flgAbleChiH);

        if(flgAbleRon)
        {
            ronButton.interactable = !flgFriten;
        }

        chiCanselButton.gameObject.SetActive(false);
    }

    private void DisplayChiButtons()
    {
        canselButton.gameObject.SetActive(false);
        ronButton.gameObject.SetActive(false);
        kanButton.gameObject.SetActive(false);
        ponButton.gameObject.SetActive(false);
        chiButton.gameObject.SetActive(false);

        chiCanselButton.gameObject.SetActive(true);

        if (flgAbleChiL)
        {
            chiSelectButtonPrefabs.Add(InstantiateChiSelectButtonPrefab(paiKind, MahjongManager.NakiKinds.ChiNumLow, this));
        }
        if (flgAbleChiM)
        {
            chiSelectButtonPrefabs.Add(InstantiateChiSelectButtonPrefab(paiKind, MahjongManager.NakiKinds.ChiNumMiddle, this));
        }
        if (flgAbleChiH)
        {
            chiSelectButtonPrefabs.Add(InstantiateChiSelectButtonPrefab(paiKind, MahjongManager.NakiKinds.ChiNumHigh, this));
        }
    }

    private ChiSelectButtonPrefab InstantiateChiSelectButtonPrefab(MahjongManager.PaiKinds _paiKind, MahjongManager.NakiKinds _naki, UiNakiPrefab _uiNakiPrefab)
    {
        var result = Instantiate(chiSelectButtonPrefab, transform).GetComponent<ChiSelectButtonPrefab>();
        result.SetStatus(_paiKind, _naki, _uiNakiPrefab);
        return result;
    }

    public void ReceptionChiSelectButtonPrefab(MahjongManager.NakiKinds _naki)
    {
        MahjongManager.Instance.ReceptionUiNakiPrefabForNaki(_naki);
        Destroy(gameObject);
    }

    /// <summary>
    /// インスペクターのボタンを押したとき
    /// </summary>
    public void InspectorButtonFunction()
    {
        Debug.Log($"InspectorButtonFunction : Display UiNakiPrefab Status\n" +
            $"paiKind = {paiKind}\n" +
            $"flgAbleRon = {flgAbleRon}\n" +
            $"flgAbleKan = {flgAbleKan}\n" +
            $"flgAblePon = {flgAblePon}\n" +
            $"flgAbleChiL = {flgAbleChiL}\n" +
            $"flgAbleChiM = {flgAbleChiM}\n" +
            $"flgAbleChiH = {flgAbleChiH}\n" +
            $"flgFriten = {flgFriten}\n");
    }
}

[CustomEditor(typeof(UiNakiPrefab))] // NakiPrefabを拡張する
public class UiNakiPrefabDisplayLog : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Display Log of Status"))
        {
            UiNakiPrefab yourScript = (UiNakiPrefab)target;
            yourScript.InspectorButtonFunction();
        }

    }
}
