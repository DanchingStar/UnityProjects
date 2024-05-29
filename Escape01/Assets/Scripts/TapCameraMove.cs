using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapCameraMove : TapCollider
{
    [SerializeField] private string MovePositionName;

    /// <summary> �h�A�J�̏������K�v�Ȃ̂��ݒ肷�� </summary>
    [SerializeField] private bool doorConditions = false;
    /// <summary> �����Ɋւ��h�A�̃I�u�W�F�N�g��ݒ肷�� </summary>
    [SerializeField] private DoorManager myDoorManager;
    /// <summary> �R���C�_�[���L���Ȃ̂́A�h�A���J���Ă�ꍇ�����Ă�ꍇ���ݒ肷�� </summary>
    [SerializeField] private bool myDoorOpen = false;

    /// <summary> �M�~�b�N�N���A�̏������K�v�Ȃ̂��ݒ肷�� </summary>
    [SerializeField] private bool gimmickConditions = false;
    /// <summary> �����Ɋւ��M�~�b�N�̃I�u�W�F�N�g��ݒ肷�� </summary>
    [SerializeField] private GimmickClearManager myGimmickClearManager;
    /// <summary> �R���C�_�[���L���Ȃ̂́A�M�~�b�N���N���A���Ă���ꍇ�����N���A�̏ꍇ���ݒ肷�� </summary>
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
