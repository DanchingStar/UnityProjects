using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiTurnActionPrefab : MonoBehaviour
{
    [SerializeField] private Button tsumoButton;
    [SerializeField] private Button ryuukyokuButton;
    [SerializeField] private Button reachButton;
    [SerializeField] private Button kanButton;
    [SerializeField] private Button cancelButton;

    private bool flgAbleTsumo;
    private bool flgAbleRyuukyoku;

    private List<MahjongManager.PaiKinds> reachAbleList;
    private List<MahjongManager.PaiKinds> ankanAbleList;
    private List<MahjongManager.PaiKinds> kakanAbleList;

    private bool actionFlg;

    private bool startFinifhFlg = false;

    private void Start()
    {
        if (startFinifhFlg) return;
        startFinifhFlg = true;

        reachAbleList = new List<MahjongManager.PaiKinds>();
        ankanAbleList = new List<MahjongManager.PaiKinds>();
        kakanAbleList = new List<MahjongManager.PaiKinds>();

        actionFlg = false;
    }

    public void SetMyStatus(bool _tsumo, bool _ryuukyoku,
        List<MahjongManager.PaiKinds> _reachList, List<MahjongManager.PaiKinds> _ankanList, List<MahjongManager.PaiKinds> _kakanList)
    {
        Start();

        flgAbleTsumo = _tsumo;
        flgAbleRyuukyoku = _ryuukyoku;
        reachAbleList = new List<MahjongManager.PaiKinds>(_reachList);
        ankanAbleList = new List<MahjongManager.PaiKinds>(_ankanList);
        kakanAbleList = new List<MahjongManager.PaiKinds>(_kakanList);

        DisplayButtons();
    }

    private void DisplayButtons()
    {
        tsumoButton.gameObject.SetActive(flgAbleTsumo);
        ryuukyokuButton.gameObject.SetActive(flgAbleRyuukyoku);
        reachButton.gameObject.SetActive(reachAbleList.Count != 0);
        kanButton.gameObject.SetActive(ankanAbleList.Count != 0 || kakanAbleList.Count != 0);

        cancelButton.gameObject.SetActive(false);
    }

    private void DisplayActionCancelButton()
    {
        tsumoButton.gameObject.SetActive(false);
        ryuukyokuButton.gameObject.SetActive(false);
        reachButton.gameObject.SetActive(false);
        kanButton.gameObject.SetActive(false);

        cancelButton.gameObject.SetActive(true);
    }

    public void PushTsumoButton()
    {
        MahjongManager.Instance.ReceptionUiTurnActionPrefab(MahjongManager.TurnActionKind.Tsumo ,MahjongManager.PaiKinds.None_00);
        DestroyMe();
    }

    public void PushRyuukyokuButton()
    {
        MahjongManager.Instance.ReceptionUiTurnActionPrefab(MahjongManager.TurnActionKind.Ryuukyoku ,MahjongManager.PaiKinds.None_00);
        DestroyMe();
    }

    public void PushReachButton()
    {
        DisplayActionCancelButton();
        MahjongManager.Instance.ReceptionUiTurnActionPrefab(MahjongManager.TurnActionKind.Reach, MahjongManager.PaiKinds.None_00);
    }

    public void PushKanButton()
    {
        if (ankanAbleList.Count + kakanAbleList.Count == 1) // カンが1択の場合
        {
            if (ankanAbleList.Count == 1)
            {
                MahjongManager.Instance.ReceptionUiTurnActionPrefab(MahjongManager.TurnActionKind.Ankan, ankanAbleList[0]);
                DestroyMe();
            }
            else
            {
                MahjongManager.Instance.ReceptionUiTurnActionPrefab(MahjongManager.TurnActionKind.Kakan, kakanAbleList[0]);
                DestroyMe();
            }
        }
        else // カンが2択以上の場合
        {
            DisplayActionCancelButton();
            MahjongManager.Instance.ReceptionUiTurnActionPrefab(MahjongManager.TurnActionKind.Kakan, MahjongManager.PaiKinds.None_00);
        }
    }

    public void PushActionCancelButton()
    {
        DisplayButtons();
        MahjongManager.Instance.ReceptionUiTurnActionForCancel();
    }

    public void ActionCancelButton()
    {

        MahjongManager.Instance.ReceptionUiTurnActionForCancel();
    }

    public void ReceptionUiManagerForDestroyMe()
    {
        DestroyMe();
    }

    private void DestroyMe()
    {
        Destroy(gameObject);
    }

    public bool GetActionFlg()
    {
        return actionFlg;
    }

    public List<MahjongManager.PaiKinds> GetReachAbleList()
    {
        return reachAbleList;
    }

    public List<MahjongManager.PaiKinds> GetKanAbleList()
    {
        List<MahjongManager.PaiKinds> resultList = new List<MahjongManager.PaiKinds> ();

        foreach (var item in ankanAbleList)
        {
            resultList.Add(item);
        }
        foreach (var item in kakanAbleList)
        {
            resultList.Add(item);
        }

        return resultList;
    }
}
