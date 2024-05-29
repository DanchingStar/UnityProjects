using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ギミック正解時にDoorManagerのドアを開ける
/// </summary>
public class OpenJudge : ChangeJugdeParent
{
    [SerializeField] protected bool isSynchronizationDoor = false;
    [SerializeField] protected DoorManager[] doors;

    protected override void RightNow()
    {
        base.RightNow();

        this.GetComponent<DoorManager>().setDoorOpen(true);

        if(isSynchronizationDoor)
        {
            foreach (var door in doors)
            {
                door.setDoorOpen(true);
            }
        }
    }
}
