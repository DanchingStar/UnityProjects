using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �G����Behavior
/// </summary>
public class Enemy : MonoBehaviour
{
    //[SerializeField]
    protected float _DamageAngle = 0.0f;

    /// <summary>
    /// �����������ɌX���p�x
    /// </summary>
    public float DamageAngle
    {
        get
        {
            return _DamageAngle;
        }
    }

    //[SerializeField]
    protected bool _IsDependsPlayerAngle = false;

    /// <summary>
    /// Player�̊p�x�Ɉˑ����邩�ǂ���
    /// Player�ƂԂ���������Enemy��DamageAngle�݂̂Ŋ������Ȃ�
    /// True = Player���łǂ����Ɍ������l������
    /// False = Player����DamageAngle�𑫂�����
    /// </summary>
    public bool IsDependsPlayerAngle
    {
        get
        {
            return _IsDependsPlayerAngle;
        }
    }

    #region GameSystem
    private void Start()
    {

    }

    /// <summary>
    /// �p�����Update��������Ă鎞�͌Ă΂�Ȃ�
    /// </summary>
    private void Update()
    {
        UpdatePosition();
    }
    #endregion

    public virtual Vector3 GenerateEnemyPos() 
    {
        Debug.Log($"[Enemy] Base��GenerateEnemyPos���Ă΂�Ă�B�p�����override��������]�܂���");
        return new Vector3(0.0f, 15.0f, 0.0f);
    }

    /// <summary>
    /// �i�ޕ���
    /// </summary>
    protected virtual Vector3 Direction
    {
        get
        {
            return Vector3.zero;
        }
    }

    protected virtual void UpdatePosition()
    {
        var diff = Time.deltaTime * Direction;
        var pos = transform.position + diff;
        transform.position = pos;
        //Debug.Log($"dir : {dir}");
        //Debug.Log($"pos : {pos}");
        //Debug.Log($"transform pos: {transform.position}");
    }
}
