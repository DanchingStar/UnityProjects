using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapGameClear : TapCollider
{
    /// <summary> �h�A�J�̏������K�v�Ȃ̂��ݒ肷�� </summary>
    [SerializeField] private bool doorConditions = false;
    /// <summary> �����Ɋւ��h�A�̃I�u�W�F�N�g��ݒ肷�� </summary>
    [SerializeField] private DoorManager myDoorManager;
    /// <summary> �R���C�_�[���L���Ȃ̂́A�h�A���J���Ă�ꍇ�����Ă�ꍇ���ݒ肷�� </summary>
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
