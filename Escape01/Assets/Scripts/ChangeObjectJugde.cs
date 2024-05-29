using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ギミック正解時に指定したオブジェクトを表示・非表示にする
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
