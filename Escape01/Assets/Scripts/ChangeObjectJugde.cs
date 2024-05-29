using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �M�~�b�N�������Ɏw�肵���I�u�W�F�N�g��\���E��\���ɂ���
/// </summary>
public class ChangeObjectJugde : ChangeJugdeParent
{
    [SerializeField] private bool activeObjectCondition = false;
    [SerializeField] private GameObject[] ActiveObjects;
    [SerializeField] private bool deleteObjectCondition = false;
    [SerializeField] private GameObject[] DeleteObjects;

    protected override void RightNow()
    {
        base.RightNow();

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
}
