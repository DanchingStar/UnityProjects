using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapGameClear : TapCollider
{
    /// <summary> ドア開閉の条件が必要なのか設定する </summary>
    [SerializeField] private bool doorConditions = false;
    /// <summary> 条件に関わるドアのオブジェクトを設定する </summary>
    [SerializeField] private DoorManager myDoorManager;
    /// <summary> コライダーが有効なのは、ドアが開いてる場合か閉じてる場合か設定する </summary>
    [SerializeField] private bool myDoorOpen = false;


    private void Update()
    {
        if (EnableCameraPositionName != CameraManager.instance.CurrentPositionName)
        {
            this.gameObject.GetComponent<BoxCollider>().enabled = false;
            return;
        }

        if (doorConditions)
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
    }

    protected override void OnTap()
    {
        if (ControlStopper.instance.GetIsControlStop()) return;

        base.OnTap();

        GameClear();
    }


    private void GameClear()
    {
        FadeManager.Instance.LoadScene("Result", 2.0f);
    }
}
