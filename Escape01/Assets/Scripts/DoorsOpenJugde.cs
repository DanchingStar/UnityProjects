using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// •¡”‚ÌDooManager‚ğ‚à‚ÂƒMƒ~ƒbƒN‚Ì³Œë”»’è‚Æ³‰ğŒã‚Ìˆ—
/// </summary>
public class DoorsOpenJugde : MonoBehaviour
{
    [SerializeField] protected AudioSource se = null;

    [SerializeField] private DoorManager[] DoorManagers;
    [SerializeField] private bool[] AnserIndexes;
    [SerializeField] private string OpenPositionName;

    [SerializeField] private bool activeObjectCondition = false;
    [SerializeField] private GameObject[] ActiveObjects;
    [SerializeField] private bool deleteObjectCondition = false;
    [SerializeField] private GameObject[] DeleteObjects;

    private bool isOpen = false;

    protected GimmickClearManager myGimmick;

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
        if (isOpen) return;

        for (int i = 0; i < DoorManagers.Length; i++)
        {
            if (DoorManagers[i].getDoorOpen() != AnserIndexes[i])
                return;
        }

        //³‰ğ‚Ì‚Æ‚«
        StartCoroutine(RightStopBefore());

    }

    private void CameraMove(bool isButtonActive)
    {
        CameraManager.instance.ChangeCameraPosition(OpenPositionName, isButtonActive);

    }

    private void RightNow()
    {
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
    }

    private IEnumerator RightStopBefore()
    {
        ControlStopper.instance.SetIsControlStop(true);

        isOpen = true;

        if (se != null)
        {
            se.Play();
        }

        yield return new WaitForSeconds(1.5f);

        CameraMove(false);

        yield return new WaitForSeconds(0.5f);

        RightNow();

        yield return new WaitForSeconds(0.5f);

        myGimmick.SetClear();

        ContinueManager.instance.SaveGimmickClear(myGimmick.GetGimmickName(), 1);

        ControlStopper.instance.SetIsControlStop(false);
    }

    public void PlayContinue()
    {
        isOpen = true;
        RightNow();
    }
}
