using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiKyokuFinishPanelPrefab : MonoBehaviour
{
    [SerializeField] private Button nextKyokuButton;

    [SerializeField] private Image insideImageOfRyuukyoku;
    [SerializeField] private TextMeshProUGUI ryuukyokuKindText;

    [SerializeField] private Image insideImageOfAgari;
    [SerializeField] private TextMeshProUGUI agariKindText;

    private float timerOfStaging;
    private const float MAX_TIMER = 1f;

    private StandByFlg standByFlg;

    public enum StandByFlg
    {
        None,
        RyuuKyoku,
        Agari,
    }


    private bool startFinifhFlg = false;

    private void Start()
    {
        if (startFinifhFlg) return;
        startFinifhFlg = true;

        timerOfStaging = 0f;
        standByFlg = StandByFlg.None;

        insideImageOfRyuukyoku.gameObject.SetActive(false);
        insideImageOfAgari.gameObject.SetActive(false);

        ryuukyokuKindText.text = "";
    }

    private void Update()
    {
        if (standByFlg != StandByFlg.None)
        {
            AddTimerOfStaging();

            if (standByFlg == StandByFlg.RyuuKyoku)
            {
                ChangeRyuuKyokuTextColorA();
            }
            else if (standByFlg == StandByFlg.Agari)
            {
                ChangeAgariTextColorA();
            }

        }
    }

    private void AddTimerOfStaging()
    {
        if (timerOfStaging > MAX_TIMER) return;
        timerOfStaging += Time.deltaTime;
        if (timerOfStaging > MAX_TIMER) timerOfStaging = MAX_TIMER;
    }

    private float GetTimerOfStaging()
    {
        return timerOfStaging;
    }

    public void SetMyStatusForRyuukyoku(MahjongManager.RyuukyokuOfTochuu _kind)
    {
        Start();

        if (_kind == MahjongManager.RyuukyokuOfTochuu.None)
        {
            ryuukyokuKindText.text = "ó¨ã«";
        }
        else if (_kind == MahjongManager.RyuukyokuOfTochuu.Kyuusyu)
        {
            ryuukyokuKindText.text = "ã„éÌã„îv";
        }
        else
        {
            ryuukyokuKindText.text = "???";
        }

        ChangeRyuuKyokuTextColorA();

        insideImageOfRyuukyoku.gameObject.SetActive(true);
        standByFlg = StandByFlg.RyuuKyoku;
    }


    public void SetMyStatusForAgari(MahjongManager.PlayerKind _playerKind, MahjongManager.PaiStatus _ronPai)
    {
        Start();

        if (_ronPai == null)
        {
            agariKindText.text = "ÉcÉÇÉAÉKÉä!";
        }
        else
        {
            agariKindText.text = "ÉçÉìÉAÉKÉä!";
        }

        agariKindText.text += "\nÇ®ÇﬂÇ≈Ç∆Ç§";

        ChangeAgariTextColorA();

        insideImageOfAgari.gameObject.SetActive(true);
        standByFlg = StandByFlg.Agari;
    }

    private void ChangeRyuuKyokuTextColorA()
    {
        ryuukyokuKindText.color = new Color(ryuukyokuKindText.color.r, ryuukyokuKindText.color.g, ryuukyokuKindText.color.b, GetTimerOfStaging());
    }

    private void ChangeAgariTextColorA()
    {
        agariKindText.color = new Color(agariKindText.color.r, agariKindText.color.g, agariKindText.color.b, GetTimerOfStaging());
    }

    public void PushNextKyokuButton()
    {
        if (standByFlg == StandByFlg.None) return;
        if (GetTimerOfStaging() < MAX_TIMER) return;

        DestroyMe();
    }

    private void DestroyMe()
    {
        Destroy(gameObject);
    }


}
