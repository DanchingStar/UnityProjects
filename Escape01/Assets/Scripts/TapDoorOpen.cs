using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// タップすると自身のDooManagerのドアを開ける
/// </summary>
public class TapDoorOpen : TapCollider
{
    [SerializeField] private bool isNeedGimmickClear = false;
    [SerializeField] private GimmickClearManager myGimmick;
    private DoorManager myDoorManager;

    protected new void Start()
    {
        base.Start();

        myDoorManager = this.gameObject.GetComponent<DoorManager>();
    }

    protected override void OnTap()
    {
        if (ControlStopper.instance.GetIsControlStop()) return;

        base.OnTap();

        if (isNeedGimmickClear == true)
        {
            if (myGimmick.GetIsClear() == false) 
            {
                return;
            }
        }

        bool flg = myDoorManager.getDoorOpen();
        myDoorManager.setDoorOpen(!flg);
    }


}
