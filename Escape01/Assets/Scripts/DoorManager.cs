using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ÉhÉAÇÃäJï¬Çä«óùÇ∑ÇÈ
/// </summary>
public class DoorManager : MonoBehaviour
{
    [SerializeField] private Vector3 BeforePosition = Vector3.zero;
    [SerializeField] private Vector3 BeforeRotation = Vector3.zero;
    [SerializeField] private Vector3 AfterPosition = Vector3.zero;
    [SerializeField] private Vector3 AfterRotation = Vector3.zero;

    [SerializeField] private bool doorOpen = false;

    void Start()
    {
        this.transform.localPosition = BeforePosition;
        this.transform.localEulerAngles = BeforeRotation;
    }

    void Update()
    {
        if (doorOpen)
        {
            this.transform.localPosition = AfterPosition;
            this.transform.localEulerAngles = AfterRotation;
        }
        else
        {
            this.transform.localPosition = BeforePosition;
            this.transform.localEulerAngles = BeforeRotation;
        }
    }

    public bool getDoorOpen()
    {
        return doorOpen;
    }

    public void setDoorOpen(bool isDoor)
    {
        doorOpen = isDoor;
    }

}
