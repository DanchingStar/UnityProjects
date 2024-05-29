using System.Collections;
//using System.Collections.Generic;
//using Unity.VisualScripting;
//using UnityEditor.Media;
using UnityEngine;

/// <summary>
/// アイテムを使った後に指定したオブジェクトの表示・非表示を変える
/// DoorManagerを使ってドアの開閉を変えることもできる
/// </summary>
public class TapItemUse : TapCollider
{
    [SerializeField] private string MovePositionName;
    [SerializeField] private AudioSource se = null;
    [SerializeField] private Item.Type correctItemType;

    [SerializeField] private bool beforeActiveObjectCondition = false;
    [SerializeField] private GameObject[] BeforeActiveObjects;
    [SerializeField] private bool activeObjectCondition = false;
    [SerializeField] private GameObject[] ActiveObjects;
    [SerializeField] private bool deleteObjectCondition = false;
    [SerializeField] private GameObject[] DeleteObjects;
    [SerializeField] private bool openDoor = false;
    [SerializeField] private DoorManager[] Doors;

    private GimmickClearManager myGimmick;

    protected new void Start()
    {
        base.Start();

        myGimmick = this.GetComponent<GimmickClearManager>();
        if (myGimmick == null)
        {
            Debug.Log($"No GimmickClearManager : {this.name}");
        }
    }

    protected void CameraMove(bool isButtonActive)
    {
        CameraManager.instance.ChangeCameraPosition(MovePositionName, isButtonActive);

    }

    protected override void OnTap()
    {
        if (ControlStopper.instance.GetIsControlStop()) return;

        base.OnTap();

        ItemSlot mySlot = ItemBox.instance.GetActiveSlot();
        if (mySlot != null) 
        {
            if (mySlot.GetHaveItem().type == correctItemType) //選択されたアイテムが正解の場合
            {
                StartCoroutine(RightItem(mySlot));
            }
        }
    }

    private IEnumerator RightItem(ItemSlot mySlot)
    {
        ControlStopper.instance.SetIsControlStop(true);

        if (beforeActiveObjectCondition)
        {
            foreach (var obj in BeforeActiveObjects)
            {
                obj.SetActive(true);
            }
        }

        if (se != null)
        {
            se.Play();
        }

        yield return new WaitForSeconds(1.0f);

        CameraMove(false);

        yield return new WaitForSeconds(0.5f);

        if (activeObjectCondition)
        {
            foreach (var obj in ActiveObjects)
            {
                obj.SetActive(true);
            }
        }
        if (deleteObjectCondition)
        {
            foreach (var obj in DeleteObjects)
            {
                //obj.SetActive(false);
                Destroy(obj);
            }
        }
        if (openDoor)
        {
            foreach (var door in Doors)
            {
                door.setDoorOpen(true);
            }
        }

        myGimmick.SetClear();

        ContinueManager.instance.SaveGimmickClear(myGimmick.GetGimmickName(), 1);

        ContinueManager.instance.SaveItemStatus(correctItemType, 2);

        ItemBox.instance.DeleteItem(mySlot);

        Destroy(myBoxCollider);

        ControlStopper.instance.SetIsControlStop(false);
    }

    public void PlayContinue()
    {
        if (beforeActiveObjectCondition)
        {
            foreach (var obj in BeforeActiveObjects)
            {
                obj.SetActive(true);
            }
        }
        if (activeObjectCondition)
        {
            foreach (var obj in ActiveObjects)
            {
                obj.SetActive(true);
            }
        }
        if (deleteObjectCondition)
        {
            foreach (var obj in DeleteObjects)
            {
                //obj.SetActive(false);
                Destroy(obj);
            }
        }
        if (openDoor)
        {
            foreach (var door in Doors)
            {
                door.setDoorOpen(true);
            }
        }

        if (myBoxCollider != null)
        {
            Destroy(myBoxCollider);
        }
        else
        {
            BoxCollider boxCollider = GetComponent<BoxCollider>();
            if (boxCollider != null)
            {
                Destroy(boxCollider);
            }
        }

    }

}
