using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GimmickClearManager : MonoBehaviour
{
    public enum Type
    {
        None,
        TapItemUse,
        OpenJudge,
        ChangeObjectJugde,
        DoorOpenJugde,
        TapGimmickClear,
        GimmickClearJudge,
    }

    [SerializeField] private GimmickName.Type gimmickName;
    [SerializeField] private Type gimmickType;
    [SerializeField] private GameObject gimmickObject;

    private bool isClear = false;

    private void Start()
    {
        if (gimmickName == GimmickName.Type.None)
        {
            Debug.LogError("GimmickClearManager : Gimmick Name is None");
        }
        if (gimmickType == Type.None)
        {
            Debug.LogError("GimmickClearManager : Gimmick Type is None");
        }
        if (gimmickObject == null)
        {
            Debug.LogError("GimmickClearManager : Gimmick Object is Null");
        }

        int isContinue = PlayerPrefs.GetInt("IsContinue", 0);
        int gimmickFlg = PlayerPrefs.GetInt(Enum.GetName(typeof(GimmickName.Type), gimmickName), 0);

        if (isContinue != 0 && gimmickFlg == 1)
        {
            switch (gimmickType)
            {
                case Type.TapItemUse:
                    TapItemUse comTIU = gimmickObject.GetComponent<TapItemUse>();
                    if (comTIU == null)
                    {
                        Debug.LogError("GimmickClearManager : TapItemUse Component is Null");
                    }
                    else
                    {
                        comTIU.PlayContinue();
                    }
                    break;
                case Type.OpenJudge:
                    OpenJudge comOJ = gimmickObject.GetComponent<OpenJudge>();
                    if (comOJ == null)
                    {
                        Debug.LogError("GimmickClearManager : OpenJudge Component is Null");
                    }
                    else
                    {
                        comOJ.PlayContinue();
                    }
                    break;
                case Type.ChangeObjectJugde:
                    ChangeObjectJugde comCOJ = gimmickObject.GetComponent<ChangeObjectJugde>();
                    if (comCOJ == null)
                    {
                        Debug.LogError("GimmickClearManager : ChangeObjectJugde Component is Null");
                    }
                    else
                    {
                        comCOJ.PlayContinue();
                    }
                    break;
                case Type.DoorOpenJugde:
                    DoorsOpenJugde comDOJ = gimmickObject.GetComponent<DoorsOpenJugde>();
                    if (comDOJ == null)
                    {
                        Debug.LogError("GimmickClearManager : DoorsOpenJugde Component is Null");
                    }
                    else
                    {
                        comDOJ.PlayContinue();
                    }
                    break;
                case Type.TapGimmickClear:
                    break;
                case Type.GimmickClearJudge:
                    GimmickClearJudge comGCJ = gimmickObject.GetComponent<GimmickClearJudge>();
                    if (comGCJ == null)
                    {
                        Debug.LogError("GimmickClearManager : GimmickClearJudge Component is Null");
                    }
                    else
                    {
                        comGCJ.PlayContinue();
                    }
                    break;
                default:
                    Debug.LogError("GimmickClearManager : Gimmick Type is Default");
                    break;
            }
            SetClear();
        }
    }

    public void SetClear()
    {
        isClear = true;
    }

    public bool GetIsClear()
    {
        return isClear;
    }

    public GimmickName.Type GetGimmickName()
    {
        return gimmickName;
    }

}
