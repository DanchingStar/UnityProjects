using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapCameraMove : TapCollider
{
    [SerializeField] private string MovePositionName;

    /// <summary> ドア開閉の条件が必要なのか設定する </summary>
    [SerializeField] private bool doorConditions = false;
    /// <summary> 条件に関わるドアのオブジェクトを設定する </summary>
    [SerializeField] private DoorManager myDoorManager;
    /// <summary> コライダーが有効なのは、ドアが開いてる場合か閉じてる場合か設定する </summary>
    [SerializeField] private bool myDoorOpen = false;

    /// <summary> ギミッククリアの条件が必要なのか設定する </summary>
    [SerializeField] private bool gimmickConditions = false;
    /// <summary> 条件に関わるギミックのオブジェクトを設定する </summary>
    [SerializeField] private GimmickClearManager myGimmickClearManager;
    /// <summary> コライダーが有効なのは、ギミックをクリアしている場合か未クリアの場合か設定する </summary>
    [SerializeField] private bool myGimmickIsClear = false;

    private void Update()
    {
        if (EnableCameraPositionName != CameraManager.instance.CurrentPositionName)
        {
            this.gameObject.GetComponent<BoxCollider>().enabled = false;
            return;
        }

        if (doorConditions && gimmickConditions)
        {
            if (myDoorManager.getDoorOpen() == myDoorOpen && myGimmickClearManager.GetIsClear() == myGimmickIsClear)
            {
                this.gameObject.GetComponent<BoxCollider>().enabled = true;
            }
            else
            {
                this.gameObject.GetComponent<BoxCollider>().enabled = false;
            }
        }
        else if (doorConditions)
        {
            if (myDoorManager.getDoorOpen() == myDoorOpen)
            {
                this.gameObject.GetComponent<BoxCollider>().enabled = true;
            }
            else
            {
                this.gameObject.GetComponent<BoxCollider>().enabled = false;
            }
        }
        else if (gimmickConditions)
        {
            if (myGimmickClearManager.GetIsClear() == myGimmickIsClear)
            {
                this.gameObject.GetComponent<BoxCollider>().enabled = true;
            }
            else
            {
                this.gameObject.GetComponent<BoxCollider>().enabled = false;
            }
        }
        else
        {
            this.gameObject.GetComponent<BoxCollider>().enabled = true;
        }

    }

    protected override void OnTap()
    {
        if (ControlStopper.instance.GetIsControlStop()) return;

        base.OnTap();

        CameraManager.instance.ChangeCameraPosition(MovePositionName);

    }
}
