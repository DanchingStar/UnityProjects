using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapGimmickClear : TapCameraMove
{
    [SerializeField] private GimmickClearManager clearGimmick;

    protected override void OnTap()
    {
        if (ControlStopper.instance.GetIsControlStop()) return;

        base.OnTap();

        if (!clearGimmick.GetIsClear())
        {
            clearGimmick.SetClear();
            ContinueManager.instance.SaveGimmickClear(clearGimmick.GetGimmickName(), 1);
        }
    }
}
