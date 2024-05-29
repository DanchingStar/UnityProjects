using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TapCollider : MonoBehaviour
{
    public string EnableCameraPositionName;

    protected BoxCollider myBoxCollider;

    protected string currentPlaceName = "";

    protected void Start()
    {
        myBoxCollider = GetComponent<BoxCollider>();
        var CurrentTrigger = gameObject.AddComponent<EventTrigger>();

        var EntryClick = new EventTrigger.Entry();
        EntryClick.eventID = EventTriggerType.PointerClick;
        EntryClick.callback.AddListener((x) => OnTap());

        CurrentTrigger.triggers.Add(EntryClick);
    }

    void Update()
    {
        if (myBoxCollider == null) return;

        ChangeAbleCollider();
    }

    protected virtual void OnTap()
    {
        // �p����ł͎n�߂ɉ��L�̕����L�q���邱��
        //if (ControlStopper.instance.GetIsControlStop()) return;
        //base.OnTap();
    }


    protected void ChangeAbleCollider()
    {
        if (currentPlaceName == CameraManager.instance.CurrentPositionName) return;

        if (EnableCameraPositionName == CameraManager.instance.CurrentPositionName)
        {
            myBoxCollider.enabled = true;
        }
        else
        {
            myBoxCollider.enabled = false;
        }
        currentPlaceName = CameraManager.instance.CurrentPositionName;
    }
}
