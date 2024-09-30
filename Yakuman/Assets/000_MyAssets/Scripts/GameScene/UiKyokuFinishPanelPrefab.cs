using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiKyokuFinishPanelPrefab : MonoBehaviour
{
    [SerializeField] private GameObject underPanel;

    [SerializeField] private Button nextKyokuButton;

    [SerializeField] private Image insideImageOfRyuukyoku;
    [SerializeField] private TextMeshProUGUI ryuukyokuKindText;

    [SerializeField] private Image insideImageOfAgari;
    [SerializeField] private TextMeshProUGUI agariKindText;
    [SerializeField] private Transform tehaiAndNakiAreaTF;
    [SerializeField] private Transform agariPaiAreaTF;

    [SerializeField] private GameObject uiPaiImagePrefab;
    [SerializeField] private GameObject uiPaiImageForNakiPrefab;
    [SerializeField] private GameObject uiPaiImageForNakiOfKakanPrefab;
    [SerializeField] private GameObject uiAgariBackgroundPrefab;

    private const float SIZE_PAI_MAGNIFICATION = 0.7f;

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
        else if (_kind == MahjongManager.RyuukyokuOfTochuu.SuuFuuRenda)
        {
            ryuukyokuKindText.text = "élïóéqòAë≈";
        }
        else
        {
            ryuukyokuKindText.text = "???";
        }

        //ChangeRyuuKyokuTextColorA();

        insideImageOfRyuukyoku.gameObject.SetActive(true);
        standByFlg = StandByFlg.RyuuKyoku;
    }


    public void SetMyStatusForAgari(MahjongManager.PlayerKind _playerKind, MahjongManager.PaiStatus _ronPai)
    {
        Start();

        //if (_ronPai == null)
        //{
        //    agariKindText.text = "ÉcÉÇÉAÉKÉä!";
        //}
        //else
        //{
        //    agariKindText.text = "ÉçÉìÉAÉKÉä!";
        //}

        //agariKindText.text += "\nÇ®ÇﬂÇ≈Ç∆Ç§";

        //ChangeAgariTextColorA();

        InstantiateAgariKei(_playerKind, _ronPai);

        CheckYakumanList(_playerKind, _ronPai);



        insideImageOfAgari.gameObject.SetActive(true);
        standByFlg = StandByFlg.Agari;
    }

    private void InstantiateAgariKei(MahjongManager.PlayerKind _playerKind, MahjongManager.PaiStatus _ronPai)
    {
        List<MahjongManager.PaiStatus> tehaiList = MahjongManager.Instance.GetPlayerTehaiComponent(_playerKind).GetTehais();
        List<NakiPrefab> nakiList = MahjongManager.Instance.GetPlayerTehaiComponent(_playerKind).GetNakisForTehai();
        MahjongManager.PaiStatus agariPai = _ronPai == null ? tehaiList[tehaiList.Count - 1] : _ronPai;

        List<GameObject> nakiBackgroundList = new List<GameObject>();
        GameObject agariNotNakiBackground = Instantiate(uiAgariBackgroundPrefab , tehaiAndNakiAreaTF);

        int longNakiCount = 0;
        int tehaiNum;
        float defaultSizeX = agariNotNakiBackground.GetComponent<RectTransform>().sizeDelta.x / 13;
        float defaultSizeY = agariNotNakiBackground.GetComponent<RectTransform>().sizeDelta.y;
        //Debug.Log($"defaultSizeX = {defaultSizeX} , defaultSizeY = {defaultSizeY}");
        if (tehaiList.Count >= 13)
        {
            tehaiNum = 13;
        }
        else if (tehaiList.Count >= 10)
        {
            tehaiNum = 10;
        }
        else if (tehaiList.Count >= 7)
        {
            tehaiNum = 7;
        }
        else if (tehaiList.Count >= 4)
        {
            tehaiNum = 4;
        }
        else
        {
            tehaiNum = 1;
        }
        agariNotNakiBackground.GetComponent<RectTransform>().sizeDelta = new Vector2(defaultSizeX * tehaiNum, defaultSizeY);

        for (int i = 0; i < tehaiNum ; i++)
        {
            var paiItem = Instantiate(uiPaiImagePrefab, agariNotNakiBackground.transform).GetComponent<UiPaiImagePrefab>();
            paiItem.SetPaiKind(tehaiList[i].thisKind);
        }

        nakiList.Reverse();
        foreach (var item in nakiList)
        {
            var nakiBackImage = Instantiate(uiAgariBackgroundPrefab, tehaiAndNakiAreaTF);

            if(item.GetMyMentsuStatus().nakiKinds == MahjongManager.NakiKinds.Ankan)
            {
                nakiBackImage.GetComponent<RectTransform>().sizeDelta = new Vector2(defaultSizeX * 4, defaultSizeY);
                for(int i = 0; i < 4; i++)
                {
                    var paiItem = Instantiate(uiPaiImagePrefab, nakiBackImage.transform).GetComponent<UiPaiImagePrefab>();
                    if (i == 1 || i== 2)
                    {
                        paiItem.SetPaiKind(item.GetMyMentsuStatus().minimumPai);
                    }
                    else
                    {
                        paiItem.SetPaiKindForAnkan(item.GetMyMentsuStatus().minimumPai);
                    }
                }
                longNakiCount++;
            }
            else if (item.GetMyMentsuStatus().nakiKinds == MahjongManager.NakiKinds.MinKan)
            {
                if(item.GetMinkanKind() == NakiPrefab.MinkanKind.Daiminkan)
                {
                    NakiPrefab.NakiPlace nakiPlace = item.GetNakiPlace();
                    nakiBackImage.GetComponent<RectTransform>().sizeDelta = new Vector2(defaultSizeX * 3 + defaultSizeY, defaultSizeY);

                    for (int i = 0; i < 4; i++)
                    {
                        UiPaiImagePrefab paiItem;
                        if (nakiPlace == NakiPrefab.NakiPlace.Right  && i == 3 ||
                            nakiPlace == NakiPrefab.NakiPlace.Center && i == 2 ||
                            nakiPlace == NakiPrefab.NakiPlace.Left   && i == 0 )
                        {
                            paiItem = Instantiate(uiPaiImageForNakiPrefab, nakiBackImage.transform).GetComponent<UiPaiImagePrefab>();
                        }
                        else
                        {
                            paiItem = Instantiate(uiPaiImagePrefab, nakiBackImage.transform).GetComponent<UiPaiImagePrefab>();
                        }
                        paiItem.SetPaiKind(item.GetMyMentsuStatus().minimumPai);
                    }
                    longNakiCount++;
                }
                else if (item.GetMinkanKind() == NakiPrefab.MinkanKind.Kakan)
                {
                    NakiPrefab.NakiPlace nakiPlace = item.GetNakiPlace();
                    nakiBackImage.GetComponent<RectTransform>().sizeDelta = new Vector2(defaultSizeX * 2 + defaultSizeY, defaultSizeX * 2);

                    for (int i = 0; i < 3; i++)
                    {
                        UiPaiImagePrefab paiItem;
                        if (nakiPlace == NakiPrefab.NakiPlace.Right  && i == 2 ||
                            nakiPlace == NakiPrefab.NakiPlace.Center && i == 1 ||
                            nakiPlace == NakiPrefab.NakiPlace.Left   && i == 0 )
                        {
                            paiItem = Instantiate(uiPaiImageForNakiOfKakanPrefab, nakiBackImage.transform).GetComponent<UiPaiImagePrefab>();
                        }
                        else
                        {
                            paiItem = Instantiate(uiPaiImagePrefab, nakiBackImage.transform).GetComponent<UiPaiImagePrefab>();
                        }
                        paiItem.SetPaiKind(item.GetMyMentsuStatus().minimumPai);
                    }
                }
                else
                {
                    Debug.LogWarning($"InstantiateAgariKei : Error\n" +
                        $"item.GetMinkanKind() = {item.GetMinkanKind()}");
                }
            }
            else if (item.GetMyMentsuStatus().nakiKinds == MahjongManager.NakiKinds.Pon)
            {
                NakiPrefab.NakiPlace nakiPlace = item.GetNakiPlace();
                nakiBackImage.GetComponent<RectTransform>().sizeDelta = new Vector2(defaultSizeX * 2 + defaultSizeY, defaultSizeY);

                for (int i = 0; i < 3; i++)
                {
                    UiPaiImagePrefab paiItem;
                    if (nakiPlace == NakiPrefab.NakiPlace.Right  && i == 2 ||
                        nakiPlace == NakiPrefab.NakiPlace.Center && i == 1 || 
                        nakiPlace == NakiPrefab.NakiPlace.Left   && i == 0 )
                    {
                        paiItem = Instantiate(uiPaiImageForNakiPrefab, nakiBackImage.transform).GetComponent<UiPaiImagePrefab>();
                    }
                    else
                    {
                        paiItem = Instantiate(uiPaiImagePrefab, nakiBackImage.transform).GetComponent<UiPaiImagePrefab>();
                    }
                    paiItem.SetPaiKind(item.GetMyMentsuStatus().minimumPai);
                }
            }
            else if (item.GetMyMentsuStatus().nakiKinds == MahjongManager.NakiKinds.Chi)
            {
                nakiBackImage.GetComponent<RectTransform>().sizeDelta = new Vector2(defaultSizeX * 2 + defaultSizeY, defaultSizeY);

                for (int i = 0; i < 3; i++)
                {
                    UiPaiImagePrefab paiItem;
                    if (i == 0)
                    {
                        paiItem = Instantiate(uiPaiImageForNakiPrefab, nakiBackImage.transform).GetComponent<UiPaiImagePrefab>();
                    }
                    else
                    {
                        paiItem = Instantiate(uiPaiImagePrefab, nakiBackImage.transform).GetComponent<UiPaiImagePrefab>();
                    }
                    paiItem.SetPaiKind(item.GetPaiPrefabForChi(i).GetThisKind());
                }
            }
            else
            {
                Debug.LogWarning($"InstantiateAgariKei : Error\n" +
                    $"item.GetMyMentsuStatus().nakiKinds = {item.GetMyMentsuStatus().nakiKinds}");
            }

            nakiBackgroundList.Add(nakiBackImage);
        }

        var agariPaiImage = Instantiate(uiPaiImagePrefab, agariPaiAreaTF).GetComponent<UiPaiImagePrefab>();
        agariPaiImage.SetPaiKind(agariPai.thisKind);

        // ÉXÉPÅ[ÉãÇÃïœçX
        float finalMagnification = SIZE_PAI_MAGNIFICATION;
        for (int i = 0; i < nakiList.Count; i++) finalMagnification *= 0.98f;
        for (int i = 0; i < longNakiCount; i++) finalMagnification *= 0.95f;

        agariNotNakiBackground.transform.localScale *= finalMagnification;
        foreach (var item in nakiBackgroundList) item.transform.localScale *= finalMagnification;
        agariPaiImage.gameObject.transform.localScale *= finalMagnification;

        //LayoutGroup layoutGroup = tehaiAndNakiAreaTF.gameObject.GetComponent<HorizontalLayoutGroup>();
        //layoutGroup.CalculateLayoutInputHorizontal();
        //layoutGroup.CalculateLayoutInputVertical();
        //layoutGroup.SetLayoutHorizontal();
        //layoutGroup.SetLayoutVertical();
    }

    private void CheckYakumanList(MahjongManager.PlayerKind _playerKind, MahjongManager.PaiStatus _ronPai)
    {
        List<MahjongManager.MentsuStatus> nakiList = MahjongManager.Instance.GetPlayerTehaiComponent(_playerKind).GetNakis();
        List<MahjongManager.PaiStatus> tehaiList = MahjongManager.Instance.GetPlayerTehaiComponent(_playerKind).GetTehais();
        MahjongManager.PaiKinds ronPaiKind = _ronPai == null ? MahjongManager.PaiKinds.None_00 : _ronPai.thisKind;

        List<MahjongManager.YakumanKind> yakumanList = new List<MahjongManager.YakumanKind>();
        List<MahjongManager.YakumanOfLocalKind> lokalYalumanList = new List<MahjongManager.YakumanOfLocalKind>();

        yakumanList = MahjongManager.Instance.GetYakumanList(tehaiList, nakiList, ronPaiKind);
        lokalYalumanList = MahjongManager.Instance.GetLocalYakumanList(tehaiList, nakiList, ronPaiKind);

        ChangeAgariText(yakumanList, lokalYalumanList, _playerKind);
    }

    private void ChangeRyuuKyokuTextColorA()
    {
        ryuukyokuKindText.color = new Color(ryuukyokuKindText.color.r, ryuukyokuKindText.color.g, ryuukyokuKindText.color.b, GetTimerOfStaging());
    }

    private void ChangeAgariTextColorA()
    {
        agariKindText.color = new Color(agariKindText.color.r, agariKindText.color.g, agariKindText.color.b, GetTimerOfStaging());
    }

    private void ChangeAgariText(List<MahjongManager.YakumanKind> _yakumanList, List<MahjongManager.YakumanOfLocalKind> _lokalYalumanList , MahjongManager.PlayerKind _agariPlayer)
    {
        string str = "";

        if (_agariPlayer == MahjongManager.PlayerKind.Player)
        {
            if (_yakumanList.Count + _lokalYalumanList.Count <= 0)
            {
                str = "NOT ññû";
            }
            else
            {
                foreach (var item in _yakumanList)
                {
                    switch (item)
                    {
                        case MahjongManager.YakumanKind.TenHo:
                            {
                                str += "ìVòa";
                            }
                            break;
                        case MahjongManager.YakumanKind.ChiHo:
                            {
                                str += "ínòa";
                            }
                            break;
                        case MahjongManager.YakumanKind.SuAnKo:
                            {
                                str += "élà√çè";
                            }
                            break;
                        case MahjongManager.YakumanKind.SuKanTsu:
                            {
                                str += "élû»éq";
                            }
                            break;
                        case MahjongManager.YakumanKind.DaiSanGen:
                            {
                                str += "ëÂéOå≥";
                            }
                            break;
                        case MahjongManager.YakumanKind.SyouSuuShi:
                            {
                                str += "è¨éläÏ";
                            }
                            break;
                        case MahjongManager.YakumanKind.DaiSuuShi:
                            {
                                str += "ëÂéläÏ";
                            }
                            break;
                        case MahjongManager.YakumanKind.TsuIiSo:
                            {
                                str += "éöàÍêF";
                            }
                            break;
                        case MahjongManager.YakumanKind.ChinRouTo:
                            {
                                str += "ê¥òVì™";
                            }
                            break;
                        case MahjongManager.YakumanKind.RyuIiSo:
                            {
                                str += "óŒàÍêF";
                            }
                            break;
                        case MahjongManager.YakumanKind.KokuShiMuSou:
                            {
                                str += "çëémñ≥ëo";
                            }
                            break;
                        case MahjongManager.YakumanKind.ChuRenPoTo:
                            {
                                str += "ã„ò@ïÛìï";
                            }
                            break;
                    }
                    str += "\n";
                }

                foreach (var item in _lokalYalumanList)
                {
                    switch (item)
                    {
                        case MahjongManager.YakumanOfLocalKind.SuRenKo:
                            {
                                str += "élòAçè";
                            }
                            break;
                        case MahjongManager.YakumanOfLocalKind.HyakuManGoku:
                            {
                                str += "ïSñúêŒ";
                            }
                            break;
                        case MahjongManager.YakumanOfLocalKind.DaiShaRin:
                            {
                                str += "ëÂé‘ó÷";
                            }
                            break;
                        case MahjongManager.YakumanOfLocalKind.BeniKujaku:
                            {
                                str += "çgçEêù";
                            }
                            break;
                        case MahjongManager.YakumanOfLocalKind.DaiChiShin:
                            {
                                str += "ëÂéµêØ";
                            }
                            break;
                    }
                    str += "\n";
                }
                str = str.TrimEnd('\n');
            }
        }
        else
        {
            str += "ëºâ∆Ç…òaóπÇÁÇÍÇΩ...";
        }

        agariKindText.text = str;
    }

    public void PushNextKyokuButton()
    {
        if (standByFlg == StandByFlg.None) return;
        if (GetTimerOfStaging() < MAX_TIMER) return;

        DestroyMe();

        MahjongManager.Instance.ReceptionKyokuFinishPanelGoNext(standByFlg == StandByFlg.Agari);
    }

    public void PushOutsidePanel(bool flg)
    {
        underPanel.SetActive(flg);
    }

    private void DestroyMe()
    {
        Destroy(gameObject);
    }


}
