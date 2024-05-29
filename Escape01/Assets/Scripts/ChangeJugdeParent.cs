using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �M�~�b�N����Ő��딻�肷��X�N���v�g�̐e
/// </summary>
public class ChangeJugdeParent : MonoBehaviour
{
    [SerializeField] protected AudioSource se = null;
    [SerializeField] protected TapObjectChange[] TapChanges;
    [SerializeField] protected int[] AnserIndexes;
    [SerializeField] protected string OpenPositionName;

    protected GimmickClearManager myGimmick;

    protected bool isOpen = false;

    protected void Start()
    {
        myGimmick = this.GetComponent<GimmickClearManager>();
        if (myGimmick == null)
        {
            Debug.Log($"No GimmickClearManager : {this.name}");
        }
    }

    protected void Update()
    {
        if (isOpen) return;

        for (int i = 0; i < TapChanges.Length; i++)
        {
            if (TapChanges[i].Index != AnserIndexes[i])
                return;
        }

        //�����̂Ƃ�
        StartCoroutine(RightStopBefore());
    }

    protected void CameraMove(bool isButtonActive)
    {
        CameraManager.instance.ChangeCameraPosition(OpenPositionName, isButtonActive);

    }

    virtual protected void RightNow()
    {

    }

    private IEnumerator RightStopBefore()
    {
        ControlStopper.instance.SetIsControlStop(true);

        isOpen = true;

        if (se != null)
        {
            se.Play();
        }

        foreach (var TapChange in TapChanges) //TapChange�̋@�\���~����
        {
            TapChange.enabled = false;
            TapChange.gameObject.GetComponent<BoxCollider>().enabled = false;
            Destroy(TapChange);
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
        for (int i = 0; i < TapChanges.Length; i++)
        {
            TapChanges[i].CoercionRightIndex(AnserIndexes[i]);
        }
        isOpen = true;
        foreach (var TapChange in TapChanges) //TapChange�̋@�\���~����
        {
            TapChange.enabled = false;
            TapChange.gameObject.GetComponent<BoxCollider>().enabled = false;
            Destroy(TapChange);
        }
        RightNow();
    }


}
