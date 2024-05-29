using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �Q�[���S�̂̃R���g���[�������Ǘ�����
/// </summary>
public class ControlStopper : MonoBehaviour
{
    [SerializeField] private bool isControlStop = false;

    public static ControlStopper instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Update()
    {
        
    }

    public bool GetIsControlStop()
    {
        return isControlStop;
    }

    public void SetIsControlStop(bool b)
    {
        isControlStop = b;
    }
}
