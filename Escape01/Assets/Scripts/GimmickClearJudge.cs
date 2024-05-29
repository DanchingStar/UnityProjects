using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class GimmickClearJudge : MonoBehaviour
{
    [SerializeField] private AudioSource se = null;
    [SerializeField] private GimmickClearManager[] gimmicks;
    [SerializeField] private string MovePositionName;

    [SerializeField] private bool beforeActiveObjectCondition = false;
    [SerializeField] private GameObject[] BeforeActiveObjects;
    [SerializeField] private bool activeObjectCondition = false;
    [SerializeField] private GameObject[] ActiveObjects;
    [SerializeField] private bool deleteObjectCondition = false;
    [SerializeField] private GameObject[] DeleteObjects;
    [SerializeField] private bool openDoor = false;
    [SerializeField] private DoorManager[] Doors;

    private GimmickClearManager myGimmick;
    private bool isClear = false;

    private void Start()
    {
        myGimmick = this.GetComponent<GimmickClearManager>();
        if (myGimmick == null)
        {
            Debug.Log($"No GimmickClearManager : {this.name}");
        }
    }

    private void Update()
    {
        if (isClear) return;
        if (ControlStopper.instance.GetIsControlStop()) return;

        foreach (var gimmick in gimmicks)
        {
            if (!gimmick.GetIsClear())
            {
                return;
            }
        }

        //èåèíBê¨ÇµÇΩÇ∆Ç´
        StartCoroutine(AllClear());
    }

    private void CameraMove(bool isButtonActive)
    {
        CameraManager.instance.ChangeCameraPosition(MovePositionName, isButtonActive);

    }

    private IEnumerator AllClear()
    {
        ControlStopper.instance.SetIsControlStop(true);

        isClear = true;

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

        yield return new WaitForSeconds(0.5f);

        myGimmick.SetClear();

        ContinueManager.instance.SaveGimmickClear(myGimmick.GetGimmickName(), 1);

        ControlStopper.instance.SetIsControlStop(false);
    }

    public void PlayContinue()
    {
        isClear = true;

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
    }
}
