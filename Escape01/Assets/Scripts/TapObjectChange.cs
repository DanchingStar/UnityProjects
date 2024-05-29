using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapObjectChange : TapCollider
{
    public int Index { get; private set; }

    [SerializeField] private GameObject[] Objects; 

    protected override void OnTap()
    {
        if (ControlStopper.instance.GetIsControlStop()) return;

        base.OnTap();

        Objects[Index].SetActive(false);

        Index++;
        if (Index >= Objects.Length)
        {
            Index = 0;
        }

        Objects[Index].SetActive(true);
    }

    public void CoercionRightIndex(int index)
    {
        for (int i = 0; i < Objects.Length; i++) 
        {
            if (i == index) 
            {
                Objects[i].SetActive(true);
            }
            else
            {
                Objects[i].SetActive(false);
            }
        }
    }
}
