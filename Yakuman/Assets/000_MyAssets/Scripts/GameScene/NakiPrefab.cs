using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NakiPrefab : MonoBehaviour
{
    [SerializeField] protected NakiPlace nakiPlace;
    [SerializeField] protected PaiPrefab[] paiPrefabs;
    [SerializeField] private PaiPrefab paiPrefabOfKakan;
    [SerializeField] private PaiPrefab paiPrefabOfDaiminkan;

    protected MahjongManager.NakiKinds myNakiKind;
    protected MahjongManager.MentsuStatus myMentsuStatus;
    protected MinkanKind minkanKind;

    protected bool kanFlg;

    protected bool startFinifhFlg = false;

    public enum NakiPlace
    {
        None,
        Right,
        Center,
        Left,
    }

    public enum PaiOfChi
    {
        None,
        Low,
        Mid,
        High,
    }

    public enum PaiPrefabKind
    {
        None,
        NakiPlace,
        Other1,
        Other2,
        KaKan, 
        Other3,
    }

    public enum MinkanKind
    {
        None,
        Daiminkan,
        Kakan
    }

    protected void Start()
    {
        if (startFinifhFlg) return;
        startFinifhFlg = true;

        kanFlg = false;
    }

    public void SetStatusFirst(NakiPlace _nakiPlace , MahjongManager.NakiKinds _myNakiKind)
    {
        Start();

        nakiPlace = _nakiPlace;
        myNakiKind = _myNakiKind;

        if (myNakiKind == MahjongManager.NakiKinds.DaiminkanFromKamicha || 
            myNakiKind == MahjongManager.NakiKinds.DaiminkanFromToimen  || 
            myNakiKind == MahjongManager.NakiKinds.DaiminkanFromShimocha )
        {
            minkanKind = MinkanKind.Daiminkan;
        }
        else
        {
            minkanKind= MinkanKind.None;
        }

        //Debug.Log($"SetStatusFirst : _myNakiKind = {_myNakiKind}");
    }

    public void SetPaiPrefab(PaiPrefabKind _paiPrefabKind,
        MahjongManager.PaiKinds _thisKind, int _totalNumber, int _arrayNumber, MahjongManager.PlayerKind _tehaiFlg)
    {
        if (nakiPlace == NakiPlace.Left)
        {
            if (_paiPrefabKind == PaiPrefabKind.NakiPlace)
            {
                paiPrefabs[0].SetStatus(_thisKind, _totalNumber, _arrayNumber, _tehaiFlg);
            }
            else if (_paiPrefabKind == PaiPrefabKind.Other1)
            {
                paiPrefabs[1].SetStatus(_thisKind, _totalNumber, _arrayNumber, _tehaiFlg);
            }
            else if (_paiPrefabKind == PaiPrefabKind.Other2)
            {
                paiPrefabs[2].SetStatus(_thisKind, _totalNumber, _arrayNumber, _tehaiFlg);
            }
            else if (_paiPrefabKind == PaiPrefabKind.Other3)
            {
                kanFlg = true;
                paiPrefabOfDaiminkan.SetStatus(_thisKind, _totalNumber, _arrayNumber, _tehaiFlg);
                paiPrefabOfDaiminkan.gameObject.SetActive(true);

                foreach(var item in paiPrefabs)
                {
                    item.transform.localPosition += new Vector3(-2, 0, 0);
                }
            }
        }
        else if (nakiPlace == NakiPlace.Center)
        {
            if (_paiPrefabKind == PaiPrefabKind.NakiPlace)
            {
                paiPrefabs[1].SetStatus(_thisKind, _totalNumber, _arrayNumber, _tehaiFlg);
            }
            else if (_paiPrefabKind == PaiPrefabKind.Other1)
            {
                paiPrefabs[0].SetStatus(_thisKind, _totalNumber, _arrayNumber, _tehaiFlg);
            }
            else if (_paiPrefabKind == PaiPrefabKind.Other2)
            {
                paiPrefabs[2].SetStatus(_thisKind, _totalNumber, _arrayNumber, _tehaiFlg);
            }
            else if (_paiPrefabKind == PaiPrefabKind.Other3)
            {
                kanFlg = true;
                paiPrefabOfDaiminkan.SetStatus(_thisKind, _totalNumber, _arrayNumber, _tehaiFlg);
                paiPrefabOfDaiminkan.gameObject.SetActive(true);
            }
        }
        else if (nakiPlace == NakiPlace.Right)
        {
            if (_paiPrefabKind == PaiPrefabKind.NakiPlace)
            {
                paiPrefabs[2].SetStatus(_thisKind, _totalNumber, _arrayNumber, _tehaiFlg);
            }
            else if (_paiPrefabKind == PaiPrefabKind.Other1)
            {
                paiPrefabs[0].SetStatus(_thisKind, _totalNumber, _arrayNumber, _tehaiFlg);
            }
            else if (_paiPrefabKind == PaiPrefabKind.Other2)
            {
                paiPrefabs[1].SetStatus(_thisKind, _totalNumber, _arrayNumber, _tehaiFlg);
            }
            else if (_paiPrefabKind == PaiPrefabKind.Other3)
            {
                kanFlg = true;
                paiPrefabOfDaiminkan.SetStatus(_thisKind, _totalNumber, _arrayNumber, _tehaiFlg);
                paiPrefabOfDaiminkan.gameObject.SetActive(true);
            }
        }
    }

    public void SetStatusFinal()
    {
        MahjongManager.PaiKinds lowPai = paiPrefabs[0].GetThisKind() < paiPrefabs[1].GetThisKind() ?
             paiPrefabs[0].GetThisKind() : paiPrefabs[1].GetThisKind();

        MahjongManager.MentsuKinds mentsuKind = paiPrefabs[0].GetThisKind() == paiPrefabs[1].GetThisKind() ?
            MahjongManager.MentsuKinds.Kootsu : MahjongManager.MentsuKinds.Juntsu;

        if(mentsuKind == MahjongManager.MentsuKinds.Kootsu && kanFlg)
        {
            mentsuKind = MahjongManager.MentsuKinds.Kantsu;
        }

        myMentsuStatus = MahjongManager.Instance.GetMentsuStatus(mentsuKind, lowPai, true);
    }

    public void Kakan(PaiPrefabKind _paiPrefabKind,
        MahjongManager.PaiKinds _thisKind, int _totalNumber, int _arrayNumber, MahjongManager.PlayerKind _tehaiFlg)
    {
        minkanKind = MinkanKind.Kakan;

        if (myNakiKind == MahjongManager.NakiKinds.PonFromShimocha)
        {
            myNakiKind = MahjongManager.NakiKinds.KakanFromShimocha;
        }
        else if (myNakiKind == MahjongManager.NakiKinds.PonFromToimen)
        {
            myNakiKind = MahjongManager.NakiKinds.KakanFromToimen;
        }
        else if (myNakiKind == MahjongManager.NakiKinds.PonFromKamicha)
        {
            myNakiKind = MahjongManager.NakiKinds.KakanFromKamicha;
        }
        else if (myNakiKind == MahjongManager.NakiKinds.Pon)
        {
            myNakiKind = MahjongManager.NakiKinds.MinKan;
        }

        kanFlg = true;
        paiPrefabOfKakan.SetStatus(_thisKind, _totalNumber, _arrayNumber, _tehaiFlg);
        paiPrefabOfKakan.gameObject.SetActive(true);

        myMentsuStatus = MahjongManager.Instance.GetMentsuStatus(MahjongManager.MentsuKinds.Kantsu, _thisKind, true);
    }

    public void ChangeTransformPosition(Vector3 _position)
    {
        transform.localPosition = _position;
    }

    public void ChangeTransformRotate(Vector3 _rotate)
    {
        transform.localRotation = Quaternion.Euler(_rotate);
    }

    public NakiPlace GetNakiPlace()
    {
        return nakiPlace;
    }

    public MinkanKind GetMinkanKind()
    {
        return minkanKind;
    }

    public PaiPrefab GetPaiPrefabForChi(int _paiPlace)
    {
        if (0 <= _paiPlace && _paiPlace <= 2)
        {
            return paiPrefabs[_paiPlace];
        }
        else
        {
            return null;
        }
    }

    public MahjongManager.MentsuStatus GetMyMentsuStatus()
    {
        return myMentsuStatus;
    }

    /// <summary>
    /// インスペクターのボタンを押したとき
    /// </summary>
    public void InspectorButtonFunction()
    {
        Debug.Log($"InspectorButtonFunction : Display NakiPrefab MentsuStatus\n" +
            $"nakiPlace = {nakiPlace}\n" +
            $"minimumPai = {myMentsuStatus.minimumPai}\n" +
            $"tanyao = {myMentsuStatus.tanyao}\n" +
            $"mentsuKind = {myMentsuStatus.mentsuKind}\n" +
            $"fu = {myMentsuStatus.fu}\n" +
            $"nakiKinds = {myMentsuStatus.nakiKinds}\n" +
            $"iroKinds = {myMentsuStatus.iroKinds}\n" +
            $"midori = {myMentsuStatus.midori}");
    }
}

[CustomEditor(typeof(NakiPrefab))] // NakiPrefabを拡張する
public class NakiPrefabDisplayLog : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Display Log of Status"))
        {
            NakiPrefab yourScript = (NakiPrefab)target;
            yourScript.InspectorButtonFunction();
        }

    }
}
